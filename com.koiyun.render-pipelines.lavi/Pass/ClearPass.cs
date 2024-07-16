using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class ClearPass : RenderPass {
        private RenderTexutreRegister targetRTR;
        private RTClearFlags clearFlags;
        private bool backgroundColor;

        public ClearPass(RenderTexutreRegister targetRTR, RTClearFlags clearFlags, bool backgroundColor=false) {
            this.targetRTR = targetRTR;
            this.clearFlags = clearFlags;
            this.backgroundColor = backgroundColor;
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var color = this.backgroundColor ? data.backgroundColor : Color.clear;

            var cmd = CommandBufferPool.Get("ClearPass");
            cmd.SetRenderTarget(this.targetRTR.RTI);
            cmd.ClearRenderTarget(this.clearFlags, color, 1, 0);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}