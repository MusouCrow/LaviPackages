using System;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    using Blend = UnityEngine.Rendering.BlendMode;

    // Used by the Material Inspector to draw UI for shader graph based materials, when no custom Editor GUI has been specified
    class LaviShaderGUI : ShaderGUI {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props) {
            EditorGUI.BeginChangeCheck();
            var material = materialEditor.target as Material;
            
            this.DrawBlendMode(material, out BlendMode blendMode);
            this.DrawCullMode(material, out CullMode cullMode);
            this.DrawZWrite(material, out var zWrite);
            this.DrawZTest(material, out ZTest zTest);

            if (EditorGUI.EndChangeCheck()) {
                LaviShaderSvc.SetBlendMode(material, blendMode);
                LaviShaderSvc.SetCullMode(material, cullMode);
                LaviShaderSvc.SetZWrite(material, zWrite);
                LaviShaderSvc.SetZTest(material, zTest);
            }

            ShaderGraphPropertyDrawers.DrawShaderGraphGUI(materialEditor, props);
        }

        private void DrawBlendMode(Material material, out BlendMode blendMode) {
            if (!LaviShaderSvc.HasBlendMode(material)) {
                blendMode = BlendMode.Alpha;
                return;
            }

            blendMode = LaviShaderSvc.GetBlendMode(material);
            blendMode = (BlendMode)EditorGUILayout.EnumPopup("混合模式", blendMode);
        }

        private void DrawCullMode(Material material, out CullMode cullMode) {
            if (!LaviShaderSvc.HasCullMode(material)) {
                cullMode = CullMode.Off;
                return;
            }

            cullMode = LaviShaderSvc.GetCullMode(material);
            cullMode = (CullMode)EditorGUILayout.EnumPopup("剔除模式", cullMode);
        }

        private void DrawZWrite(Material material, out bool zWrite) {
            if (!LaviShaderSvc.HasZWrite(material)) {
                zWrite = false;
                return;
            }

            zWrite = LaviShaderSvc.GetZWrite(material);
            zWrite = EditorGUILayout.Toggle("写入深度", zWrite);
        }

        private void DrawZTest(Material material, out ZTest zTest) {
            if (!LaviShaderSvc.HasZTest(material)) {
                zTest = ZTest.LEqual;
                return;
            }

            zTest = LaviShaderSvc.GetZTest(material);
            zTest = (ZTest)EditorGUILayout.EnumPopup("深度测试", zTest);
        }
    }
}