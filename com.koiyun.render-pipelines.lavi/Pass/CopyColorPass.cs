using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class CopyColorPass : IRenderPass {
        private int srcTID;
        private RenderTexutreRegister dstRTR;
        private Material material;
        private int passIndex;
        private bool isPixel;
        private bool allocDST;
        
        public CopyColorPass(string shaderName, int srcTID, RenderTexutreRegister dstRTR, bool allocDST, bool isPixel) {
            var shader = Shader.Find(shaderName);
            this.material = new Material(shader);
            this.passIndex = this.material.FindPass("CopyColor");
            
            this.srcTID = srcTID;
            this.dstRTR = dstRTR;
            this.allocDST = allocDST;
            this.isPixel = isPixel;
        }

        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            return true;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("CopyColorPass");
            var srcRTI = new RenderTargetIdentifier(this.srcTID);
            var dstRTI = this.allocDST ? RenderUtil.ReadyRT(cmd, ref data, ref this.dstRTR) : new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
            
            if (this.isPixel) {
                var keyword = new LocalKeyword(this.material.shader, RenderConst.POINT_FILTER_KEYWORD);
                cmd.EnableKeyword(this.material, keyword);
            }
            
            cmd.Blit(srcRTI, dstRTI, this.material, this.passIndex);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("CopyColorPass");

            if (this.allocDST) {
                cmd.ReleaseTemporaryRT(this.dstRTR.tid);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}