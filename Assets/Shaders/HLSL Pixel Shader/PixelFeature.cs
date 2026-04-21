/*
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Scripting.APIUpdating;
using System.Collections.Generic;

public class PixelFeature : ScriptableRendererFeature
{
    [System.Serializable] public class PixelFeatureSettings {
        //public LayerMask LayerMask = 0;
        public RenderPassEvent PassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        //public Material mat = null;
        [Range(16, 1080)] public int screenHeight = 144; //Good sizes: 80, 144, 256
    }

    [SerializeField] private PixelFeatureSettings settings;

    private PixelRenderPass myPass;
 
    /// <inheritdoc/>
    public override void Create()
    {
        Camera.main.depthTextureMode = DepthTextureMode.DepthNormals;
            ////Camera.main.depthTextureMode = DepthTextureMode.Depth;
        myPass = new PixelRenderPass(settings);
            //// Configures where the render pass should be injected.
        myPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isSceneViewCamera) { 
            return; 
        }
        myPass.ConfigureInput(ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Depth);
        renderer.EnqueuePass(myPass);
    }
}


*/