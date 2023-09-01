using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Koiyun.Render {
    public class MainLightShadowPass : IRenderPass {
        private LaviRenderPipelineAsset asset;

        public MainLightShadowPass(LaviRenderPipelineAsset asset) {
            this.asset = asset;
        }

        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            var ok = data.mainLightIndex > -1 && data.cullingResults.GetShadowCasterBounds(data.mainLightIndex, out var bounds);
            var cmd = CommandBufferPool.Get("MainLightShadowPass");

            CoreUtils.SetKeyword(cmd, RenderConst.MAIN_LIGHT_SHADOW_KEYWORD, ok);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            return ok;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("MainLightShadowPass");

            int shadowResolution = (int)this.asset.ShadowResolution;
            var rti = this.ReadyTexture(cmd, ref context, ref data, shadowResolution);

            var light = data.cullingResults.visibleLights[data.mainLightIndex];
            data.cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(0, 0, 1, new Vector3(1, 0, 0), shadowResolution, light.light.shadowNearPlane,
            out var viewMatrix, out var projMatrix, out var shadowSplitData);
            
            var viewport = new Rect(0, 0, shadowResolution, shadowResolution);
            cmd.SetViewport(viewport);

            cmd.SetGlobalDepthBias(1.0f, 2.5f);
            cmd.SetViewProjectionMatrices(viewMatrix, projMatrix);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            var shadowSettings = new ShadowDrawingSettings(data.cullingResults, 0, BatchCullingProjectionType.Orthographic);
            shadowSettings.splitData = shadowSplitData;

            var worldToShadowMatrix = this.CalculateWorldToShadowMatrix(ref viewMatrix, ref projMatrix);
            context.DrawShadows(ref shadowSettings);

            cmd.SetGlobalDepthBias(0.0f, 0.0f);
            cmd.SetGlobalTexture(RenderConst.SHADOW_TEXTURE_ID, rti);
            cmd.SetGlobalMatrix(RenderConst.WORLD_TO_SHADOW_MTX_ID, worldToShadowMatrix);

            var lightDirection = -light.localToWorldMatrix.GetColumn(2);
            cmd.SetGlobalVector(RenderConst.MAIN_LIGHT_DIRECTION_ID, lightDirection);

            var softShadow = light.light.shadows == LightShadows.Soft;
            var shadowBias = RenderUtil.GetShadowBias(ref light, projMatrix, shadowResolution);
            var shadowParam = new Vector4(shadowBias.x, shadowBias.y, light.light.shadowStrength, softShadow ? 1 : 0);
            cmd.SetGlobalVector(RenderConst.SHADOW_PARAMS_ID, shadowParam);
            
            var shadowTextureSize = new Vector4(1.0f / shadowResolution, 1.0f / shadowResolution, shadowResolution, shadowResolution);
            cmd.SetGlobalVector(RenderConst.SHADOW_TEXUTRE_SIZE_ID, shadowTextureSize);

            CoreUtils.SetKeyword(cmd, RenderConst.SOFT_SHADOW_KEYWORD, softShadow);

            context.ExecuteCommandBuffer(cmd);
            context.SetupCameraProperties(data.camera);
            cmd.Clear();
            
            CommandBufferPool.Release(cmd);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("MainLightShadowPass");
            var tid = RenderConst.SHADOW_TEXTURE_ID;
            cmd.ReleaseTemporaryRT(tid);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private RenderTargetIdentifier ReadyTexture(CommandBuffer cmd, ref ScriptableRenderContext context, ref RenderData data, int size) {
            var tid = RenderConst.SHADOW_TEXTURE_ID;
            var rtd = this.CreateRenderTextureDescriptor(size);
            var rti = new RenderTargetIdentifier(tid);

            cmd.GetTemporaryRT(tid, rtd, FilterMode.Bilinear);
            cmd.SetRenderTarget(rti, 
                RenderBufferLoadAction.Load, RenderBufferStoreAction.Store,
                RenderBufferLoadAction.Load, RenderBufferStoreAction.Store
            );
            cmd.ClearRenderTarget(true, true, data.camera.backgroundColor.linear);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            return rti;
        }

        private RenderTextureDescriptor CreateRenderTextureDescriptor(int size) {
            var rtd = new RenderTextureDescriptor(size, size, RenderTextureFormat.Shadowmap, 16);

            return rtd;
        }

        private Matrix4x4 CalculateWorldToShadowMatrix(ref Matrix4x4 viewMatrix, ref Matrix4x4 projMatrix) {
            if (SystemInfo.usesReversedZBuffer) {
                projMatrix.m20 = -projMatrix.m20;
                projMatrix.m21 = -projMatrix.m21;
                projMatrix.m22 = -projMatrix.m22;
                projMatrix.m23 = -projMatrix.m23;
            }

            // [-1, 1] -> [0, 1]
            var scaleOffset = Matrix4x4.identity;
            scaleOffset.m00 = scaleOffset.m11 = scaleOffset.m22 = 0.5f;
		    scaleOffset.m03 = scaleOffset.m13 = scaleOffset.m23 = 0.5f;

            return scaleOffset * (projMatrix * viewMatrix);
        }
    }
}