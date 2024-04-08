using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class DrawOpaquePass : RenderPass {
        private string lightMode;
        private FilteringSettings filteringSettings;
        private RenderTargetIdentifier[] colorRTIs;
        private RenderTexutreRegister depthRTR;

        public DrawOpaquePass(string lightMode, RenderTexutreRegister colorRTR, RenderTexutreRegister paramRTR, RenderTexutreRegister depthRTR) {
            this.lightMode = lightMode;

            var renderQueueRange = RenderQueueRange.opaque;
            this.filteringSettings = new FilteringSettings(renderQueueRange);

            this.depthRTR = depthRTR;
            this.colorRTIs = new RenderTargetIdentifier[] {
                colorRTR.RTI,
                paramRTR.RTI
            };
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("DrawOpaquePass");
            cmd.SetRenderTarget(this.colorRTIs, this.depthRTR.RTI);
            cmd.ClearRenderTarget(true, true, Color.clear);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            var drawingSettings = RenderUtil.CreateDrawingSettings(ref data, this.lightMode, true);
            context.DrawRenderers(data.cullingResults, ref drawingSettings, ref this.filteringSettings);
        }
    }
}