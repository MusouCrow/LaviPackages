using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class DrawObjectPass : RenderPass {
        private string lightMode;
        private bool isOpaque;
        private bool clearColor;
        private bool clearDepth;
        private FilteringSettings filteringSettings;
        private RenderTargetIdentifier[] colorRTIs;
        private RenderTexutreRegister depthRTR;

        public DrawObjectPass(string lightMode, bool isOpaque, bool clearColor, bool clearDepth, RenderTexutreRegister colorRTR, RenderTexutreRegister paramRTR, RenderTexutreRegister depthRTR) {
            this.lightMode = lightMode;
            this.isOpaque = isOpaque;
            this.clearColor = clearColor;
            this.clearDepth = clearDepth;

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

            if (this.clearColor || this.clearDepth) {
                cmd.ClearRenderTarget(this.clearDepth, this.clearColor, Color.clear);
            }
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            var drawingSettings = RenderUtil.CreateDrawingSettings(ref data, this.lightMode, this.isOpaque);
            context.DrawRenderers(data.cullingResults, ref drawingSettings, ref this.filteringSettings);
        }
    }
}