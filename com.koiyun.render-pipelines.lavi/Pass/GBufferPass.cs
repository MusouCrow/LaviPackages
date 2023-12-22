using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class GBufferPass : RenderPass {
        private string lightMode;
        private FilteringSettings filteringSettings;
        private RenderTexutreRegister gBufferColorRTR;
        private RenderTexutreRegister gBufferOtherRTR;
        private RenderTexutreRegister gBufferNormalRTR;
        private RenderTexutreRegister depthRTR;

        private RenderTargetIdentifier[] colorRTIs;

        public GBufferPass(string lightMode, RenderTexutreRegister gBufferColorRTR, RenderTexutreRegister gBufferOtherRTR, RenderTexutreRegister gBufferNormalRTR, RenderTexutreRegister depthRTR) {
            this.lightMode = lightMode;

            var renderQueueRange = RenderQueueRange.opaque;
            this.filteringSettings = new FilteringSettings(renderQueueRange);

            this.gBufferColorRTR = gBufferColorRTR;
            this.gBufferOtherRTR = gBufferOtherRTR;
            this.gBufferNormalRTR = gBufferNormalRTR;
            this.depthRTR = depthRTR;

            this.colorRTIs = new RenderTargetIdentifier[] {
                this.gBufferColorRTR.RTI,
                this.gBufferOtherRTR.RTI,
                this.gBufferNormalRTR.RTI
            };
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("GBufferPass");
            cmd.SetRenderTarget(this.colorRTIs, this.depthRTR.RTI);
            cmd.ClearRenderTarget(true, true, Color.black);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            var drawingSettings = RenderUtil.CreateDrawingSettings(ref data, this.lightMode, true);
            context.DrawRenderers(data.cullingResults, ref drawingSettings, ref this.filteringSettings);
        }
    }
}