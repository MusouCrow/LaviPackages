using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class OutlinePass : RenderPass {
        private Material material;
        private RenderTexutreRegister paramRTR;
        private RenderTexutreRegister depthRTR;

        public OutlinePass(Material material, RenderTexutreRegister paramRTR, RenderTexutreRegister depthRTR) {
            this.material = material;
            this.paramRTR = paramRTR;
            this.depthRTR = depthRTR;
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("OutlinePass");
            cmd.SetRenderTarget(this.paramRTR.RTI, this.depthRTR.RTI);
            cmd.DrawProcedural(Matrix4x4.identity, this.material, 0, MeshTopology.Triangles, 3, 1);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}