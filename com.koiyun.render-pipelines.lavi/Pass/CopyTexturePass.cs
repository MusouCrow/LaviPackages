using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class CopyTexturePass : RenderPass {
        private Material material;
        private RenderTexutreRegister coiedRTR;
        private RenderTexutreRegister targetRTR;

        private int copyColorIndex;
        private int copyDepthIndex;

        public CopyTexturePass(Material material, RenderTexutreRegister coiedRTR, RenderTexutreRegister targetRTR) {
            this.material = material;
            this.coiedRTR = coiedRTR;
            this.targetRTR = targetRTR;

            this.copyColorIndex = this.material.FindPass("CopyColor");
            this.copyDepthIndex = this.material.FindPass("CopyDepth");
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var passName = this.targetRTR.format == TextureFormat.Depth ? "CopyDepthPass" : "CopyColorPass";
            var index = this.targetRTR.format == TextureFormat.Depth ? this.copyDepthIndex : this.copyColorIndex;
            var cmd = CommandBufferPool.Get(passName);
            cmd.SetRenderTarget(this.targetRTR.RTI);
            cmd.SetGlobalTexture(RenderConst.COPIED_TEXTURE_ID, this.coiedRTR.RTI);
            cmd.DrawProcedural(Matrix4x4.identity, this.material, index, MeshTopology.Triangles, 3, 1);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}