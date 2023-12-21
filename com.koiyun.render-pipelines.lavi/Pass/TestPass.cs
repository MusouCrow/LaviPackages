using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class TestPass : RenderPass {
        private RenderTexutreRegister colorRTR;
        private RenderTexutreRegister depthRTR;
        
        public TestPass(RenderTexutreRegister colorRTR, RenderTexutreRegister depthRTR) {
            this.colorRTR = colorRTR;
            this.depthRTR = depthRTR;
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("TestPass");
            cmd.SetRenderTarget(this.colorRTR.RTI, this.depthRTR.RTI);
            cmd.ClearRenderTarget(true, true, Color.black);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}