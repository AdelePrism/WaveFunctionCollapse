Shader "Hidden/Pixelize"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
		

		HLSLINCLUDE
		#pragma vertex vert
		#pragma fragment frag
		//#include "UnityRP.cginc"
		#include "UnityCG.cginc"
		//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		//#include "ShaderLibrary/DepthNormals.hlsl"

		//UNITY_REVERSE
		struct Attributes {
			float4 positionOS : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct Varyings {
			float4 positionHCS : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		Texture2D _MainTex;
		float4 _MainTex_TexelSize;
		float4 _MainTex_ST;
		Texture2D _CameraDepthNormalsTexture;
		Texture2D _CameraDepthTexture;

		SamplerState sampler_point_clamp_MainTex;
		SamplerState sampler_point_clamp_DepthNormals;

		uniform float2 _BlockCount;
		uniform float2 _BlockSize;
		uniform float2 _HalfBlockSize;

		Varyings vert(Attributes IN) {
			Varyings OUT;
			OUT.positionHCS = mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4(IN.positionOS.xyz, 1.0)));
			OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
			return OUT;
		}

		ENDHLSL

        Pass
        {
			
			Name "Pixelation"

			
			//CGPROGRAM
			HLSLPROGRAM

			//Sample and linearize depth for orthographic camera
			float DepthCalc(Varyings IN, int x, int y) {
				float sampleDepth = _CameraDepthTexture.Sample(sampler_point_clamp_MainTex, IN.uv + float2((x * _MainTex_TexelSize.x), (y * _MainTex_TexelSize.y))).r;
				sampleDepth * _ProjectionParams.z + _ProjectionParams.w - 5;
				float normalizedDepth = (sampleDepth - _ProjectionParams.w) / (_ProjectionParams.z - _ProjectionParams.w);
				float depthRange = normalizedDepth * 50.0;  //Amplify depth range
				return pow(depthRange, 5.0);  //Exponentiation to emphasize distant objects
			}

			float3 DecodeNormal(float4 packedData) {
				//Decode normal from RG channels (Stereographic Projection)
				float3 normalVS = float3(packedData.rg, 1.0);
				normalVS.z = 1.0 - dot(normalVS.xy, normalVS.xy);
				return normalize(normalVS);
			}


			//float NormalCalc(Varyings IN, int x, int y) {
			//	float4 depthNormals = _CameraDepthNormalsTexture.Sample(sampler_point_clamp_MainTex, IN.uv + float2((x * _MainTex_TexelSize.x), (y * _MainTex_TexelSize.y)));  
			//	float depth;
			//	float3 normals;
			//	DecodeDepthNormal(depthNormals, depth, normals);	
			//	return normals;		
			//}

			half4 frag(Varyings IN) : SV_TARGET {
				
				float2 blockPos = floor(IN.uv * _BlockCount);
				float2 blockCenter = blockPos * _BlockSize + _HalfBlockSize;
				float4 tex = _MainTex.Sample(sampler_point_clamp_MainTex, blockCenter);
			
				float depth = DepthCalc(IN, 0, 0);
				
				//return float4(depth, depth, depth, 1);

				float diff = 0;
				diff += DepthCalc(IN, -1, 0) - depth;
				diff += DepthCalc(IN, 1, 0) - depth;
				diff += DepthCalc(IN, 0, -1) - depth;
				diff += DepthCalc(IN, 0, 1) - depth;

				//for (int i = 0; i < 4; i++) {
				//	diff = max(diff, )
				//}

				float depthDiff = smoothstep(0.01, 1, diff);
				
				if (depthDiff > 0) {
					float4 outline = {0, 0, 0, 1};
					//return outline;
				}

				return tex;
			}
			ENDHLSL
        }
    }
}
