using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class DrawOpaquePass : RenderPass {
        private string lightMode;
        private FilteringSettings filteringSettings;
        private RenderTexutreRegister colorRTR;
        private RenderTexutreRegister glowRTR;
        private RenderTexutreRegister normalRTR;
        private RenderTexutreRegister depthRTR;

        private RenderTargetIdentifier[] colorRTIs;

        public DrawOpaquePass(string lightMode, RenderTexutreRegister colorRTR, RenderTexutreRegister glowRTR, RenderTexutreRegister normalRTR, RenderTexutreRegister depthRTR) {
            this.lightMode = lightMode;

            var renderQueueRange = RenderQueueRange.opaque;
            this.filteringSettings = new FilteringSettings(renderQueueRange);

            this.colorRTR = colorRTR;
            this.glowRTR = glowRTR;
            this.normalRTR = normalRTR;
            this.depthRTR = depthRTR;

            this.colorRTIs = new RenderTargetIdentifier[] {
                this.colorRTR.RTI,
                this.glowRTR.RTI,
                this.normalRTR.RTI
            };
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("DrawOpaquePass");
            cmd.SetRenderTarget(this.colorRTIs, this.depthRTR.RTI);
            cmd.ClearRenderTarget(true, true, Color.black);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            var drawingSettings = RenderUtil.CreateDrawingSettings(ref data, this.lightMode, true);
            context.DrawRenderers(data.cullingResults, ref drawingSettings, ref this.filteringSettings);
        }
    }
}