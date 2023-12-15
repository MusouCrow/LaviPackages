using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Koiyun.Render {
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

        public static RenderTextureDescriptor CreateCameraRenderTextureDescriptor(Camera camera, MSAASamples msaaSamples) {
            var width = Mathf.Min(camera.pixelWidth, RenderConst.MAX_PIXEL_WIDTH);
            var height = Mathf.Min(camera.pixelHeight, RenderConst.MAX_PIXEL_HEIGHT);
            var rtd = new RenderTextureDescriptor(width, height);
            // rtd.graphicsFormat = GraphicsFormat.B10G11R11_UFloatPack32;
            rtd.graphicsFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
            rtd.depthBufferBits = 32;
            rtd.msaaSamples = (int)msaaSamples;
            rtd.sRGB = true;
            rtd.bindMS = false;
            rtd.enableRandomWrite = false;

            return rtd;
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

        public static RenderTargetIdentifier ReadyRT(CommandBuffer cmd, ref RenderData data, ref RenderTexutreRegister rtr) {
            var tid = rtr.tid;
            var rtd = rtr.HandleRTD(data.cameraRTD);
            var rti = new RenderTargetIdentifier(tid);
            cmd.GetTemporaryRT(tid, rtd, FilterMode.Bilinear);
            
            return rti;
        }

    #if UNITY_EDITOR
        private static float GetGameViewScale() {
            var unityEditorAssembly = typeof(EditorWindow).Assembly;
            var gameViewType = unityEditorAssembly.GetType("UnityEditor.GameView");
            var obj = Resources.FindObjectsOfTypeAll(gameViewType);
            var gameViewWindow = obj[0] as EditorWindow;

            if (gameViewWindow == null) {
                return 1;
            }

            var areaField = gameViewType.GetField("m_ZoomArea", BindingFlags.Instance | BindingFlags.NonPublic);
            var areaObj = areaField.GetValue(gameViewWindow);
            var scaleField = areaObj.GetType().GetField("m_Scale", BindingFlags.Instance | BindingFlags.NonPublic);
            var scale = (Vector2)scaleField.GetValue(areaObj);

            return scale.x;
        }
    #else
        private static float GetGameViewScale() {
            return 1;
        }
    #endif

        public static float GetPixelScale(int width) {
            var scale = Mathf.Min(GetGameViewScale(), 1);
            var rate = Mathf.Min(RenderConst.MAX_PIXEL_WIDTH / width, 1);
            var ret = RenderConst.PIXEL_RATE * rate * scale;
            
            return ret;
        }
    }
}