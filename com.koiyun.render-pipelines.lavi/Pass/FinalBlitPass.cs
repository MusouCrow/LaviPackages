using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class FinalBlitPass : RenderPass {
        private RenderTexutreRegister sourceRTR;
        private Material material;
        private int passIndex;
        
        public FinalBlitPass(RenderTexutreRegister sourceRTR, Material material) {
            this.sourceRTR = sourceRTR;
            this.material = material;
            this.passIndex = this.sourceRTR.format == TextureFormat.Depth ? this.material.FindPass("CopyDepth") : this.material.FindPass("CopyColor");
        }

        public override bool IsActived(ref RenderData data) {
            return this.sourceRTR.format != TextureFormat.Depth || (this.sourceRTR.format == TextureFormat.Depth && data.camera.cameraType == CameraType.SceneView);
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("FinalBlitPass");
            var dstRTI = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);

            cmd.Blit(this.sourceRTR.RTI, dstRTI, this.material, this.passIndex);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}