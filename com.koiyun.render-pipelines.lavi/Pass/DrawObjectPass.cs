using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class DrawObjectPass : RenderPass {
        private string lightMode;
        private bool isOpaque;
        private RTClearFlags clearFlags;
        private FilteringSettings filteringSettings;
        private RenderTargetIdentifier[] colorRTIs;
        private RenderTexutreRegister depthRTR;

        public DrawObjectPass(string lightMode, bool isOpaque, RTClearFlags clearFlags, RenderTexutreRegister colorRTR, RenderTexutreRegister paramRTR, RenderTexutreRegister depthRTR) {
            this.lightMode = lightMode;
            this.isOpaque = isOpaque;
            this.clearFlags = clearFlags;

            var renderQueueRange = isOpaque ? RenderQueueRange.opaque : RenderQueueRange.transparent;
            this.filteringSettings = new FilteringSettings(renderQueueRange);

            this.depthRTR = depthRTR;
            this.colorRTIs = new RenderTargetIdentifier[] {
                colorRTR.RTI,
                paramRTR.RTI
            };
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("DrawObjectPass");
            cmd.SetRenderTarget(this.colorRTIs, this.depthRTR.RTI);

            if (this.clearFlags > RTClearFlags.None) {
                cmd.ClearRenderTarget(this.clearFlags, Color.clear, 1, 0);
            }
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            var drawingSettings = RenderUtil.CreateDrawingSettings(ref data, this.lightMode, this.isOpaque);
            context.DrawRenderers(data.cullingResults, ref drawingSettings, ref this.filteringSettings);
        }
    }
}