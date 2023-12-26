using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class CopyDepthPass : RenderPass {
        private Material material;
        private RenderTexutreRegister targetRTR;

        public CopyDepthPass(Material material, RenderTexutreRegister targetRTR) {
            this.material = material;
            this.targetRTR = targetRTR;
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("CopyDepthPass");
            cmd.SetRenderTarget(this.targetRTR.RTI);
            cmd.DrawProcedural(Matrix4x4.identity, this.material, 0, MeshTopology.Triangles, 3, 1);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}