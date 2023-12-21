using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class CleanPass : RenderPass {
        private List<RenderTexutreRegister> rtrs;

        public CleanPass(List<RenderTexutreRegister> rtrs) {
            this.rtrs = rtrs;
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("CleanPass");
            
            foreach (var rtr in this.rtrs) {
                cmd.ReleaseTemporaryRT(rtr.tid);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}