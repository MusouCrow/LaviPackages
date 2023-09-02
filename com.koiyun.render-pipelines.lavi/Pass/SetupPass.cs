using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

namespace Koiyun.Render {
    public class SetupPass : IRenderPass {
        private LaviRenderPipelineAsset asset;
        private RenderTexutreRegister postRTR;
        
        public SetupPass(LaviRenderPipelineAsset asset, RenderTexutreRegister postRTR) {
            this.asset = asset;
            this.postRTR = postRTR;
        }

        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            return true;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("SetupPass");
            
            this.SetScreenParams(cmd, ref data);
            this.SetZBufferParams(cmd, ref data);
            VFXManager.ProcessCameraCommand(data.camera, cmd, new VFXCameraXRSettings(), data.cullingResults);
            RenderUtil.ReadyRT(cmd, ref data, ref this.postRTR);

            var cameraPos = data.camera.transform.position;
            cmd.SetGlobalVector(RenderConst.CAMERA_POSWS_ID, cameraPos);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("SetupPass");

            cmd.ReleaseTemporaryRT(this.postRTR.tid);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void SetScreenParams(CommandBuffer cmd, ref RenderData data) {
            var pixelRect = data.camera.pixelRect;
            var renderScale = this.asset.RenderScale;
            float scaledCameraWidth = (float)pixelRect.width * renderScale;
            float scaledCameraHeight = (float)pixelRect.height * renderScale;
            float cameraWidth = (float)pixelRect.width;
            float cameraHeight = (float)pixelRect.height;

            var scaledScreenParams = new Vector4(scaledCameraWidth, scaledCameraHeight, 1.0f + 1.0f / scaledCameraWidth, 1.0f + 1.0f / scaledCameraHeight);
            cmd.SetGlobalVector(RenderConst.SCALED_SCREEN_PARAMS_ID, scaledScreenParams);
        }

        private void SetZBufferParams(CommandBuffer cmd, ref RenderData data) {
            float near = data.camera.nearClipPlane;
            float far = data.camera.farClipPlane;
            float invNear = Mathf.Approximately(near, 0.0f) ? 0.0f : 1.0f / near;
            float invFar = Mathf.Approximately(far, 0.0f) ? 0.0f : 1.0f / far;
            float zc0 = 1.0f - far * invNear;
            float zc1 = far * invNear;
            var zBufferParams = new Vector4(zc0, zc1, zc0 * invFar, zc1 * invFar);

            if (SystemInfo.usesReversedZBuffer) {
                zBufferParams.y += zBufferParams.x;
                zBufferParams.x = -zBufferParams.x;
                zBufferParams.w += zBufferParams.z;
                zBufferParams.z = -zBufferParams.z;
            }

            cmd.SetGlobalVector(RenderConst.ZBUFFER_PARAMS_ID, zBufferParams);
        }
        
        private void SetProjectionParams(CommandBuffer cmd, ref RenderData data) {
            float near = data.camera.nearClipPlane;
            float far = data.camera.farClipPlane;
            float invFar = Mathf.Approximately(far, 0.0f) ? 0.0f : 1.0f / far;
            bool isOffscreen = data.camera.targetTexture != null;
            bool invertProjectionMatrix = isOffscreen && SystemInfo.graphicsUVStartsAtTop;
            float projectionFlipSign = invertProjectionMatrix ? -1.0f : 1.0f;
            var projectionParams = new Vector4(projectionFlipSign, near, far, 1.0f * invFar);

            cmd.SetGlobalVector(RenderConst.PROJ_PARAMS_ID, projectionParams);
        }

        public void Dispose() {
            
        }
    }
}