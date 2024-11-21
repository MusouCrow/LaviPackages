using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

namespace Koiyun.Render {
    public class ViewPass : RenderPass {
        private LaviRenderPipelineAsset asset;

        public ViewPass(LaviRenderPipelineAsset asset) {
            this.asset = asset;
        }
        
        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("ViewPass");
            context.SetupCameraProperties(data.camera);

            this.SetScreenParams(cmd, ref data);
            this.SetZBufferParams(cmd, ref data);
            this.SetMatries(cmd, ref data);

            var cameraPos = data.camera.transform.position;
            cmd.SetGlobalVector(RenderConst.CAMERA_POSWS_ID, cameraPos);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void SetScreenParams(CommandBuffer cmd, ref RenderData data) {
            var pixelRect = data.camera.pixelRect;
            var renderScale = this.asset.RenderScale;
            float scaledCameraWidth = (float)pixelRect.width * renderScale;
            float scaledCameraHeight = (float)pixelRect.height * renderScale;
            
            var scaledScreenParams = new Vector4(scaledCameraWidth, scaledCameraHeight, 1.0f + 1.0f / scaledCameraWidth, 1.0f + 1.0f / scaledCameraHeight);

            cmd.SetGlobalVector(RenderConst.SCREEN_PARAMS_ID, scaledScreenParams);
            cmd.SetGlobalVector(RenderConst.SCALED_SCREEN_PARAMS_ID, scaledScreenParams);
            cmd.SetGlobalFloat(RenderConst.RENDER_SCALE_ID, this.asset.RenderScale);
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

            var projectionParams = new Vector4(-1, near, far, invFar);
            cmd.SetGlobalVector(RenderConst.PROJ_PARAMS_ID, projectionParams);
        }

        private void SetMatries(CommandBuffer cmd, ref RenderData data) {
            var proj = data.camera.projectionMatrix;
            var view = data.camera.worldToCameraMatrix;
            var gpuProj = GL.GetGPUProjectionMatrix(proj, true);

            var invView = Matrix4x4.Inverse(view);
            var invProj = Matrix4x4.Inverse(gpuProj);
            var invViewProj = invView * invProj;
            
            var pixelRect = data.camera.pixelRect;
            var renderScale = this.asset.RenderScale;
            float scaledCameraWidth = (float)pixelRect.width * renderScale;
            float scaledCameraHeight = (float)pixelRect.height * renderScale;

            // xy coordinates in range [-1; 1] go to pixel coordinates.
            var toScreen = new Matrix4x4(
                new Vector4(0.5f * scaledCameraWidth, 0.0f, 0.0f, 0.0f),
                new Vector4(0.0f, 0.5f * scaledCameraHeight, 0.0f, 0.0f),
                new Vector4(0.0f, 0.0f, 1.0f, 0.0f),
                new Vector4(0.5f * scaledCameraWidth, 0.5f * scaledCameraHeight, 0.0f, 1.0f)
            );
            
            var mtx = Matrix4x4.Inverse(toScreen * gpuProj * view);

            cmd.SetViewProjectionMatrices(view, proj);
            cmd.SetGlobalMatrix(RenderConst.SCREEN_TO_WROLD_ID, mtx);
            cmd.SetGlobalMatrix(RenderConst.VIEW_PROJ_INV_MTX_ID, invViewProj);
        }
    }
}