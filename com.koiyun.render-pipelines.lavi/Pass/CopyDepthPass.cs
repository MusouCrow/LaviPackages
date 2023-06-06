using UnityEngine;
using UnityEngine.Rendering;

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
            return data.camera.cameraType == CameraType.SceneView;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("CopyDepthPass");

            var rtd = data.cameraRTD;
            var srcTID = RenderConst.CAMERA_DEPTH_TEXTURE_ID;
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