using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

namespace Koiyun.Render {
    public class LaviRenderPipeline : RenderPipeline, IDisposable {
        private Renderer renderer;

        public LaviRenderPipeline(LaviRenderPipelineAsset asset) {
            this.renderer = new Renderer(asset);
            this.renderer.Ready();
        }

        public void Dispose() {
            this.renderer?.Dispose();
            this.renderer = null;
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            this.Dispose();
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras) {
            
        }

        protected override void Render(ScriptableRenderContext context, List<Camera> cameras) {
            foreach (var camera in cameras) {
                VFXManager.PrepareCamera(camera);
                this.renderer?.Render(ref context, camera);
            }
        }
    }
}