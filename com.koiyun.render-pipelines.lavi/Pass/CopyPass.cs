using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class CopyPass : IRenderPass {
        private int srcTID;
        private RenderTexutreRegister dstRTR;
        private Material material;
        private int passIndex;
        private string passName;
        private bool isPixel;
        private bool allocDST;
        private bool onlyScene;
        
        public CopyPass(string shaderName, int srcTID, RenderTexutreRegister dstRTR, bool allocDST, bool isPixel=false, bool isDepth=false, bool onlyScene=false) {
            this.material = CoreUtils.CreateEngineMaterial(shaderName);
            this.passIndex = isDepth ? this.material.FindPass("CopyDepth") :  this.material.FindPass("CopyColor");
            this.passName = isDepth ? "CopyDepthPass" : "CopyColorPass";
            
            this.srcTID = srcTID;
            this.dstRTR = dstRTR;
            this.allocDST = allocDST;
            this.isPixel = isPixel;
            this.onlyScene = onlyScene;
        }

        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            return this.onlyScene ? data.camera.cameraType == CameraType.SceneView : true;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get(this.passName);
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
            var cmd = CommandBufferPool.Get(this.passName);

            if (this.allocDST) {
                cmd.ReleaseTemporaryRT(this.dstRTR.tid);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose() {
            CoreUtils.Destroy(this.material);
        }
    }
}