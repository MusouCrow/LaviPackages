using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class FinalPass : IRenderPass {
        private int srcTID;
        private Material blitMaterial;
        private int passIndex;

        public FinalPass(int srcTID) {
            this.srcTID = srcTID;

            var shader = Shader.Find("Hidden/Lavi RP/Blit");
            this.blitMaterial = new Material(shader);
            this.passIndex = this.blitMaterial.FindPass("CopyColor");
        }

        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            return true;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("FinalPass");
            var srcTID = this.srcTID;
            var dstTID = BuiltinRenderTextureType.CameraTarget;
            var srcRTI = new RenderTargetIdentifier(srcTID);
            var dstRTI = new RenderTargetIdentifier(dstTID);
            
            cmd.Blit(srcRTI, dstRTI, this.blitMaterial, this.passIndex);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {
            
        }
    }
}