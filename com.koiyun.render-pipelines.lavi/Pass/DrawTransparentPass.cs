using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class DrawTransparentPass : RenderPass {
        private string lightMode;
        private FilteringSettings filteringSettings;
        private RenderTargetIdentifier[] colorRTIs;
        private RenderTexutreRegister colorRTR;
        private RenderTexutreRegister glowRTR;
        private RenderTexutreRegister depthRTR;
        
        public DrawTransparentPass(string lightMode, RenderTexutreRegister colorRTR, RenderTexutreRegister glowRTR, RenderTexutreRegister depthRTR) {
            this.lightMode = lightMode;
            
            var renderQueueRange = RenderQueueRange.transparent;
            this.filteringSettings = new FilteringSettings(renderQueueRange);

            this.colorRTR = colorRTR;
            this.glowRTR = glowRTR;
            this.depthRTR = depthRTR;
            this.colorRTIs = new RenderTargetIdentifier[] {
                this.colorRTR.RTI,
                this.glowRTR.RTI
            };
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("DrawTransparentPass");
            cmd.SetRenderTarget(this.colorRTIs, this.depthRTR.RTI);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            
            var drawingSettings = RenderUtil.CreateDrawingSettings(ref data, this.lightMode, false);
            context.DrawRenderers(data.cullingResults, ref drawingSettings, ref this.filteringSettings);
        }
    }
}