using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Koiyun.Render {
    using ShaderGraph;

    public static class RenderUtil {
        private static List<int> MainLightIndexes = new List<int>();

        public static DrawingSettings CreateDrawingSettings(ref RenderData data, string lightMode, bool isOpaque) {
            var criteria = isOpaque ? SortingCriteria.CommonOpaque : SortingCriteria.CommonTransparent;
            var sortingSettings = new SortingSettings(data.camera) {criteria = criteria};
            var tagId = new ShaderTagId(lightMode);
            var drawingSettings = new DrawingSettings(tagId, sortingSettings) {
                enableInstancing = data.camera.cameraType != CameraType.Preview
            };
            
            return drawingSettings;
        }

        public static DrawingSettings CreateDrawingSettings(ref RenderData data, string[] lightModes, bool isOpaque) {
            var settings = CreateDrawingSettings(ref data, lightModes[0], isOpaque);
            
            for (int i = 1; i < lightModes.Length; i++) {
                var tagId = new ShaderTagId(lightModes[i]);
                settings.SetShaderPassName(i, tagId);
            }

            return settings;
        }

        public static List<int> GetMainLightIndexes(ref CullingResults cullingResults) {
            MainLightIndexes.Clear();

            for (int i = 0; i < cullingResults.visibleLights.Length; i++) {
                var light = cullingResults.visibleLights[i];

                if (light.lightType == LightType.Directional && light.light.shadows != LightShadows.None) {
                    light.light.renderingLayerMask = i + 1;
                    MainLightIndexes.Add(i);
                }
            }

            return MainLightIndexes;
        }

        public static Vector2 GetShadowBias(ref VisibleLight light, Matrix4x4 projMatrix, int shadowResolution) {
            if (light.lightType != LightType.Directional) {
                return Vector2.zero;
            }

            float frustumSize = 2.0f / projMatrix.m00;
            float texelSize = frustumSize / shadowResolution;
            float depthBias = -light.light.shadowBias * texelSize;
            float normalBias = -light.light.shadowNormalBias * texelSize;

            if (light.light.shadows == LightShadows.Soft) {
                depthBias *= RenderConst.SHADOW_BIAS_RADIUS;
                normalBias *= RenderConst.SHADOW_BIAS_RADIUS;
            }
            
            return new Vector2(depthBias, normalBias);
        }

        public static RenderQueueRange GetRenderQueueRange(RenderQueues a, RenderQueues b, int range=0) {
            return new RenderQueueRange((int)a + 1000 - range, (int)b + 1000 + range);
        }

        public static RenderQueueRange GetRenderQueueRange(int a, int b) {
            return new RenderQueueRange(a + 1000, b + 1000);
        }
    }
}