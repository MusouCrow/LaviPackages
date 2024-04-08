using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class DrawTransparentPass : RenderPass {
        private string lightMode;
        private FilteringSettings filteringSettings;
        private RenderTargetIdentifier[] colorRTIs;
        private RenderTexutreRegister depthRTR;
        
        public DrawTransparentPass(string lightMode, RenderTexutreRegister colorRTR, RenderTexutreRegister paramRTR, RenderTexutreRegister depthRTR) {
            this.lightMode = lightMode;
            
            var renderQueueRange = RenderQueueRange.transparent;
            this.filteringSettings = new FilteringSettings(renderQueueRange);

            this.depthRTR = depthRTR;
            this.colorRTIs = new RenderTargetIdentifier[] {
                colorRTR.RTI,
                paramRTR.RTI
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