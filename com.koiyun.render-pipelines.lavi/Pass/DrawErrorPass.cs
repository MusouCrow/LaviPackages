using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class DrawErrorPass : RenderPass {
        private string lightMode;
        private FilteringSettings filteringSettings;
        private RenderTexutreRegister colorRTR;
        private RenderTexutreRegister depthRTR;
        
        public DrawErrorPass(string lightMode, RenderTexutreRegister colorRTR, RenderTexutreRegister depthRTR) {
            this.lightMode = lightMode;
            
            var renderQueueRange = RenderQueueRange.all;
            this.filteringSettings = new FilteringSettings(renderQueueRange);

            this.colorRTR = colorRTR;
            this.depthRTR = depthRTR;
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("DrawErrorPass");
            cmd.SetRenderTarget(this.colorRTR.RTI, this.depthRTR.RTI);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            
            var drawingSettings = RenderUtil.CreateDrawingSettings(ref data, this.lightMode, true);
            context.DrawRenderers(data.cullingResults, ref drawingSettings, ref this.filteringSettings);
        }
    }
}