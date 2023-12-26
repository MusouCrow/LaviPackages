using System;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    public static class CreateShaderGraph {
        [MenuItem("Assets/Create/Shader Graph/Lavivagnar/Toon")]
        public static void CreateToonGraph() {
            var target = (LaviTarget)Activator.CreateInstance(typeof(LaviTarget));
            target.TrySetActiveSubTarget(typeof(ToonSubTarget));

            var blockDescriptors = new BlockFieldDescriptor[] {
                BlockFields.SurfaceDescription.BaseColor,
                ShaderPropertyUtil.SurfaceDescription.Glow
            };
            
            GraphUtil.CreateNewGraphWithOutputs(new [] {target}, blockDescriptors);
        }

        [MenuItem("Assets/Create/Shader Graph/Lavivagnar/Sprite")]
        public static void CreateSpriteGraph() {
            var target = (LaviTarget)Activator.CreateInstance(typeof(LaviTarget));
            target.TrySetActiveSubTarget(typeof(SpriteSubTarget));
            target.cullMode = CullMode.Off;
            target.zWrite = false;

            var blockDescriptors = new BlockFieldDescriptor[] {
                BlockFields.SurfaceDescription.BaseColor,
                BlockFields.SurfaceDescription.Alpha,
                ShaderPropertyUtil.SurfaceDescription.Glow
            };
            
            GraphUtil.CreateNewGraphWithOutputs(new [] {target}, blockDescriptors);
        }
    }
}