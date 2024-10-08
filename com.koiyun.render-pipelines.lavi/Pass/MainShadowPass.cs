using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class MainShadowPass : RenderPass {
        private LaviRenderPipelineAsset asset;
        private RenderTexutreRegister sceneShadowRTR;
        private RenderTexutreRegister unitShadowRTR;
        private RenderTargetIdentifier[] shadowRTIs;

        public MainShadowPass(LaviRenderPipelineAsset asset, RenderTexutreRegister sceneShadowRTR, RenderTexutreRegister unitShadowRTR) {
            this.asset = asset;
            this.sceneShadowRTR = sceneShadowRTR;
            this.unitShadowRTR = unitShadowRTR;
            this.shadowRTIs = new RenderTargetIdentifier[] { this.sceneShadowRTR.RTI, this.unitShadowRTR.RTI };
        }

        public override bool IsActived(ref RenderData data) {
            return data.mainLightIndexes.Count > 0 && data.cullingResults.GetShadowCasterBounds(data.mainLightIndexes[0], out var bounds);
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("MainShadowPass");
            int shadowResolution = (int)this.asset.ShadowResolution;
            var viewport = new Rect(0, 0, shadowResolution, shadowResolution);
            var light = data.cullingResults.visibleLights[data.mainLightIndexes[0]];

            data.cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(0, 0, 1, new Vector3(1, 0, 0), shadowResolution, light.light.shadowNearPlane,
            out var viewMatrix, out var projMatrix, out var shadowSplitData);

            for (int i = 0; i < data.mainLightIndexes.Count; i++) {
                cmd.SetRenderTarget(this.shadowRTIs[i]);
                cmd.ClearRenderTarget(true, true, Color.black);
                cmd.SetViewport(viewport);
                cmd.SetGlobalDepthBias(this.asset.ShadowDepthRange.x, this.asset.ShadowDepthRange.y);
                cmd.SetViewProjectionMatrices(viewMatrix, projMatrix);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var shadowSettings = new ShadowDrawingSettings(data.cullingResults, data.mainLightIndexes[i], BatchCullingProjectionType.Orthographic);
                shadowSettings.splitData = shadowSplitData;
                shadowSettings.useRenderingLayerMaskTest = true;
                context.DrawShadows(ref shadowSettings);
            }

            var worldToShadowMatrix = this.CalculateWorldToShadowMatrix(ref viewMatrix, ref projMatrix);
            cmd.SetGlobalDepthBias(0.0f, 0.0f);
            cmd.SetGlobalMatrix(RenderConst.WORLD_TO_SHADOW_MTX_ID, worldToShadowMatrix);

            var lightDirection = -light.localToWorldMatrix.GetColumn(2);
            cmd.SetGlobalVector(RenderConst.MAIN_LIGHT_DIRECTION_ID, lightDirection);

            this.SetShadowParams(cmd, shadowResolution, ref projMatrix);
            
            context.ExecuteCommandBuffer(cmd);
            context.SetupCameraProperties(data.camera);

            CommandBufferPool.Release(cmd);
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

        private void SetShadowParams(CommandBuffer cmd, int shadowResolution, ref Matrix4x4 projMatrix) {
            float frustumSize = 2.0f / projMatrix.m00;
            float texelSize = frustumSize / shadowResolution;
            float depthBias = -this.asset.DepthBias* texelSize;
            float normalBias = -this.asset.NormalBias * texelSize;

            depthBias *= RenderConst.SHADOW_BIAS_RADIUS;
            normalBias *= RenderConst.SHADOW_BIAS_RADIUS;
            
            var shadowBias = new Vector2(depthBias, normalBias);
            var shadowAtten = new Vector4(this.asset.ShadowAttenRange.x, this.asset.ShadowAttenRange.y, this.asset.DarkBrightAttenRange.x, this.asset.DarkBrightAttenRange.y);

            cmd.SetGlobalVector(RenderConst.SHADOW_BIAS_ID, shadowBias);
            cmd.SetGlobalFloat(RenderConst.SHADOW_STEP_ID, this.asset.ShadowStep);
            cmd.SetGlobalVector(RenderConst.SHADOW_ATTEN_ID, shadowAtten);
        }
    }
}