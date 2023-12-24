using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
            this.SetScreenToWorld(cmd, ref data);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void SetScreenParams(CommandBuffer cmd, ref RenderData data) {
            var pixelRect = data.camera.pixelRect;
            var renderScale = this.asset.RenderScale;
            float scaledCameraWidth = (float)pixelRect.width * renderScale;
            float scaledCameraHeight = (float)pixelRect.height * renderScale;

            var scaledScreenParams = new Vector4(scaledCameraWidth, scaledCameraHeight, 1.0f + 1.0f / scaledCameraWidth, 1.0f + 1.0f / scaledCameraHeight);
            cmd.SetGlobalVector(RenderConst.SCALED_SCREEN_PARAMS_ID, scaledScreenParams);
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
    }
}