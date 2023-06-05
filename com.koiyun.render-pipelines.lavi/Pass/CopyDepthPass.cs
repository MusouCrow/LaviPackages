using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Koiyun.Render {
    public class CopyDepthPass : IRenderPass {
        private Material blitMaterial;
        private int passIndex;

        public CopyDepthPass() {
            var shader = Shader.Find("Hidden/Lavi RP/Blit");
            this.blitMaterial = new Material(shader);
            this.passIndex = this.blitMaterial.FindPass("CopyDepth");
        }

        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            return true;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("CopyDepthPass");

            var rtd = data.cameraRTD;
            var srcTID = RenderConst.CAMERA_TEXTURE_ID;
            var dstTID = BuiltinRenderTextureType.CameraTarget;
            var srcRTI = new RenderTargetIdentifier(srcTID);
            var dstRTI = new RenderTargetIdentifier(dstTID);
            
            // cmd.GetTemporaryRT((int)dstTID, rtd, FilterMode.Point);
            // cmd.SetRenderTarget(dstTID, dstTID);
            // cmd.ClearRenderTarget(true, true, Color.black);
            cmd.Blit(srcRTI, dstRTI, this.blitMaterial, this.passIndex);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {
            /*
            var cmd = CommandBufferPool.Get("CopyDepthPass");
            var tid = (int)BuiltinRenderTextureType.CameraTarget;
            cmd.ReleaseTemporaryRT(tid);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            */
        }
    }
}