using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class ReadyDrawPass : IRenderPass {
        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            return true;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("ReadyDrawPass");
            var tid = RenderConst.CAMERA_TEXTURE_ID;
            var rtd = data.cameraRTD;
            var rti = new RenderTargetIdentifier(tid);
            
            cmd.GetTemporaryRT(tid, rtd, FilterMode.Bilinear);
            cmd.SetRenderTarget(rti);
            cmd.ClearRenderTarget(true, true, data.camera.backgroundColor.linear);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("ReadyDrawPass");
            var tid = RenderConst.CAMERA_TEXTURE_ID;
            cmd.ReleaseTemporaryRT(tid);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}