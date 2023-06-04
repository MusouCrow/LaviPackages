using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Koiyun.Render {
    public static class RenderUtil {
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

        public static RenderTextureDescriptor CreateCameraRenderTextureDescriptor(Camera camera, MSAASamples msaaSamples, float renderScale) {
            var width = (int)(camera.pixelWidth * renderScale);
            var height = (int)(camera.pixelHeight * renderScale);
            var rtd = new RenderTextureDescriptor(width, height);
            rtd.graphicsFormat = GraphicsFormat.B10G11R11_UFloatPack32;
            rtd.depthBufferBits = 32;
            rtd.msaaSamples = (int)msaaSamples;
            rtd.sRGB = true;
            rtd.bindMS = false;
            rtd.enableRandomWrite = false;

            return rtd;
        }

        public static int GetMainLightIndex(ref CullingResults cullingResults) {
            if (cullingResults.visibleLights.Length == 0) {
                return -1;
            }

            for (int i = 0; i < cullingResults.visibleLights.Length; i++) {
                var light = cullingResults.visibleLights[i];

                if (light.lightType == LightType.Directional && light.light.shadows != LightShadows.None) {
                    return i;
                }
            }

            return -1;
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
    }
}