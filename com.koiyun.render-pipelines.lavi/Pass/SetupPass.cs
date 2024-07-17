using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

namespace Koiyun.Render {
    public class SetupPass : RenderPass {
        private LaviRenderPipelineAsset asset;
        private List<RenderTexutreRegister> rtrs;

        public SetupPass(LaviRenderPipelineAsset asset, List<RenderTexutreRegister> rtrs) {
            this.asset = asset;
            this.rtrs = rtrs;
        }
        
        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("SetupPass");
            
            foreach (var rtr in this.rtrs) {
                var rtd = rtr.GetRTD(data.camera);
                cmd.GetTemporaryRT(rtr.tid, rtd, FilterMode.Bilinear);
                
                if (rtr.global) {
                    cmd.SetGlobalTexture(rtr.tid, rtr.RTI);
                }
            }

            CoreUtils.SetKeyword(cmd, RenderConst.MAIN_LIGHT_SHADOW_KEYWORD, data.mainLightIndexes.Count > 0);

            this.SetScreenParams(cmd, ref data);
            this.SetZBufferParams(cmd, ref data);
            this.SetScreenToWorld(cmd, ref data);
            this.SetOutlineParams(cmd);
            this.SetTimeParams(cmd);
            this.SetFogParams(cmd);
            this.SetLightParams(cmd);

            var cameraPos = data.camera.transform.position;
            cmd.SetGlobalVector(RenderConst.CAMERA_POSWS_ID, cameraPos);

            VFXManager.ProcessCameraCommand(data.camera, cmd, new VFXCameraXRSettings(), data.cullingResults);

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
        }

        private void SetScreenToWorld(CommandBuffer cmd, ref RenderData data) {
            var proj = data.camera.projectionMatrix;
            var view = data.camera.worldToCameraMatrix;
            var gpuProj = GL.GetGPUProjectionMatrix(proj, false);
            
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

            cmd.SetGlobalMatrix(RenderConst.SCREEN_TO_WROLD_ID, mtx);
        }

        private void SetOutlineParams(CommandBuffer cmd) {
            var param = new Vector2(this.asset.OutlineBrightness, this.asset.OutlineThickness);
            cmd.SetGlobalVector(RenderConst.OUTLINE_PARAMS_ID, param);
        }

        private void SetTimeParams(CommandBuffer cmd) {
            var time = Application.isPlaying ? this.asset.Time : Time.realtimeSinceStartup;
            
            cmd.SetGlobalFloat(RenderConst.TIME_ID, time);
        }

        private void SetFogParams(CommandBuffer cmd) {
            cmd.SetGlobalVector(RenderConst.FOG_RANGE_ID, this.asset.FogRange);
            cmd.SetGlobalColor(RenderConst.FOG_COLOR_ID, this.asset.FogColor);
        }

        private void SetLightParams(CommandBuffer cmd) {
            cmd.SetGlobalFloat(RenderConst.LIGHT_INTENSTY_ID, this.asset.LightIntensty);
            cmd.SetGlobalColor(RenderConst.LIGHT_COLOR_ID, this.asset.LightColor);
        }
    }
}