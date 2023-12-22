using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class DeferredPass : RenderPass {
        private Material material;
        private RenderTexutreRegister colorRTR;
        private RenderTexutreRegister depthRTR;
        
        public DeferredPass(Material material, RenderTexutreRegister colorRTR, RenderTexutreRegister depthRTR) {
            this.material = material;
            this.colorRTR = colorRTR;
            this.depthRTR = depthRTR;
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("DeferredPass");
            cmd.SetRenderTarget(this.colorRTR.RTI, this.depthRTR.RTI);
            cmd.DrawProcedural(Matrix4x4.identity, this.material, 0, MeshTopology.Triangles, 3, 1);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}