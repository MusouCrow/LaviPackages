using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class LaviRenderPipeline : RenderPipeline {
        private Renderer renderer;

        public LaviRenderPipeline(LaviRenderPipelineAsset asset) {
            this.renderer = new Renderer(asset);
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras) {
            foreach (var camera in cameras) {
                this.renderer?.Render(ref context, camera);
            }
        }

        protected override void Render(ScriptableRenderContext context, List<Camera> cameras) {
            foreach (var camera in cameras) {
                this.renderer?.Render(ref context, camera);
            }
        }
    }
}