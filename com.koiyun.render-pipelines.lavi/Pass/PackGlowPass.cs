using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class PackGlowPass : RenderPass {
        private RenderTexutreRegister targetRTR;

        public PackGlowPass(RenderTexutreRegister targetRTR) {
            this.targetRTR = targetRTR;
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("PackGlowPass");
            cmd.SetRenderTarget(this.targetRTR.RTI);
            cmd.ClearRenderTarget(true, true, Color.clear);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}