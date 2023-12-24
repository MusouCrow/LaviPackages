using System;
using UnityEditor;
using UnityEditor.ShaderGraph;

namespace Koiyun.Render.ShaderGraph.Editor {
    public static class CreateShaderGraph {
        [MenuItem("Assets/Create/Shader Graph/Lavivagnar/Toon Shader Graph")]
        public static void CreateToonGraph() {
            var target = (LaviTarget)Activator.CreateInstance(typeof(LaviTarget));
            target.TrySetActiveSubTarget(typeof(ToonSubTarget));

            var blockDescriptors = new BlockFieldDescriptor[] {
                ShaderPropertyUtil.SurfaceDescription.Color,
                ShaderPropertyUtil.SurfaceDescription.Glow,
                ShaderPropertyUtil.SurfaceDescription.LutUV,
            };
            
            GraphUtil.CreateNewGraphWithOutputs(new [] {target}, blockDescriptors);
        }
    }
}