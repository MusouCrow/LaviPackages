using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class ReadyDrawPass : IRenderPass {
        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            return true;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("ReadyDrawPass");
            var colorTID = RenderConst.CAMERA_TEXTURE_ID;
            var depthTID = RenderConst.CAMERA_DEPTH_TEXTURE_ID;
            var colorRTI = new RenderTargetIdentifier(colorTID);
            var depthRTI = new RenderTargetIdentifier(depthTID);
            var colorRTD = data.cameraRTD;
            var depthRTD = data.cameraRTD;
            depthRTD.colorFormat = RenderTextureFormat.Depth;
            
            cmd.GetTemporaryRT(colorTID, colorRTD, FilterMode.Bilinear);
            cmd.GetTemporaryRT(depthTID, depthRTD, FilterMode.Bilinear);

            cmd.SetRenderTarget(colorRTI, depthRTI);
            cmd.ClearRenderTarget(true, true, data.camera.backgroundColor.linear);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("ReadyDrawPass");
            var colorTID = RenderConst.CAMERA_TEXTURE_ID;
            var depthTID = RenderConst.CAMERA_DEPTH_TEXTURE_ID;

            cmd.ReleaseTemporaryRT(colorTID);
            cmd.ReleaseTemporaryRT(depthTID);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}