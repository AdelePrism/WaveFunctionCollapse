/*
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

class PixelRenderPass: ScriptableRenderPass {
    private PixelFeature.PixelFeatureSettings settings;

    private RenderTargetIdentifier colorBuffer, pixelBuffer;
    private int pixelHeight, pixelWidth;
    private Material material;

    private int pixBufID = Shader.PropertyToID("_PixelBuffer");
    //private int pixDepID = Shader.PropertyToID("_CameraDepthNormalsTexture");


    public PixelRenderPass(PixelFeature.PixelFeatureSettings settings) {
        this.settings = settings;
        renderPassEvent = settings.PassEvent;
        if (material == null) { material = CoreUtils.CreateEngineMaterial("Hidden/Pixelize"); }
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
        ConfigureInput(ScriptableRenderPassInput.Normal); // Ensure normal texture is available
        ConfigureInput(ScriptableRenderPassInput.Depth); // Ensure depth texture is available
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        //depthBuffer = renderingData.cameraData.renderer.cameraDepthTargetHandle;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        
        pixelHeight = settings.screenHeight;
        pixelWidth = (int)(pixelHeight * renderingData.cameraData.camera.aspect + 0.5f); //0.5f so that (int) rounds up

        material.SetVector("_BlockCount", new Vector2(pixelWidth, pixelHeight));
        material.SetVector("_BlockSize", new Vector2(1f / pixelWidth, 1f / pixelHeight));
        material.SetVector("_HalfBlockSize", new Vector2(0.5f / pixelWidth, 0.5f / pixelHeight));

        descriptor.height = pixelHeight;
        descriptor.width =  pixelWidth;
        
        cmd.GetTemporaryRT(pixBufID, descriptor, FilterMode.Point);
        //cmd.GetTemporaryRT(pixDepID, pixelWidth, pixelHeight, 24, FilterMode.Point, RenderTextureFormat.Depth);
        pixelBuffer = new RenderTargetIdentifier(pixBufID);
        //depthBuffer = new RenderTargetIdentifier(pixDepID);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler("Pixel Render Pass"))) {
            cmd.Blit(colorBuffer, pixelBuffer, material, 0);
            cmd.Blit(pixelBuffer, colorBuffer);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd) {
        if (cmd == null) throw new System.ArgumentNullException("cmd");
        cmd.ReleaseTemporaryRT(pixBufID);
    }



    //public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
    //    SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
    //    DrawingSettings drawingSettings = CreateDrawingSettings(AShaderTagList, ref renderingData, sortingCriteria);
    //    ref CameraData cameraData = ref renderingData.cameraData;

    //    Camera camera = cameraData.camera;
    //    Rect pixelRect = camera.pixelRect;
    //    int pixelWidth = (int)(camera.pixelWidth / pixelDensity);
    //    int pixelHeigth = (int)(camera.pixelHeight / pixelDensity);
    //    CommandBuffer cmd = CommandBufferPool.Get("PixelFeature");
    //    using (new ProfilingScope(cmd, profilingSampler)) {
    //        cmd.GetTemporaryRT(pixTexID, pixelWidth, pixelHeigth, 0, FilterMode.Point);
    //        cmd.GetTemporaryRT(pixDepID, pixelWidth, pixelHeigth, 24, FilterMode.Point/*, RenderTextureFormat);
    //        cmd.SetRenderTarget(pixTexID, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, pixDepID, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);

    //        cmd.ClearRenderTarget(true, true, Color.clear);
    //        context.ExecuteCommandBuffer(cmd);

    //        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref AFilterSettings, ref ARenderStateBlock);
    //        cmd.SetRenderTarget(cameraID, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);

    //        cmd.Blit(new RenderTargetIdentifier(pixTexID), BuiltinRenderTextureType.CurrentActive, blitMaterial);

    //        cmd.ReleaseTemporaryRT(pixTexID);
    //        cmd.ReleaseTemporaryRT(pixDepID);

    //        context.ExecuteCommandBuffer(cmd);
    //        cmd.Clear();

    //    }
    //}
}

// This method is called before executing the render pass.
// It can be used to configure render targets and their clear state. Also to create temporary render target textures.
// When empty this render pass will render to the active camera render target.
// You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
// The render pipeline will ensure target setup and clearing happens in a performant manner.
//public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
//{
//}

// Here you can implement the rendering logic.
// Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
// https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
// You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.

// Cleanup any allocated resources that were created during the execution of this render pass.
//public override void OnCameraCleanup(CommandBuffer cmd)
//{
//}

*/