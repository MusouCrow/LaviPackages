using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Koiyun.Render {
    public class FinalPass : IRenderPass {
        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            return true;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("FinalPass");

            var tid = RenderConst.CAMERA_TEXTURE_ID;
            var srcRTI = new RenderTargetIdentifier(tid);
            var dstRTI = new RenderTargetIdentifier(data.camera.targetTexture);
            cmd.Blit(srcRTI, dstRTI);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {
            
        }
    }
}