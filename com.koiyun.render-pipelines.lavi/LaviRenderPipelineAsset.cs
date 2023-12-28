using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    [CreateAssetMenu(menuName = "Rendering/LaviRenderPipelineAsset")]
    public class LaviRenderPipelineAsset : RenderPipelineAsset {
        public Shader DefaultShader;
        public Material DefaultMaterial;
        public Material DefaultSpriteMaterial;
        public Texture2D ColorTableTexture;
        public ShadowResolution ShadowResolution = ShadowResolution._2048;
        public float ShadowDistance = 50;
        public bool SRPBatch = true;

        private LaviRenderPipeline renderPipeline;

        public override Material defaultMaterial {
            get {
                return this.DefaultMaterial;
            }
        }

        public override Material default2DMaterial {
            get {
                return this.DefaultSpriteMaterial;
            }
        }

        public override Shader defaultShader {
            get {
                return this.DefaultShader;
            }
        }

        public float RenderScale {
            get {
                return 2;
            }
        }

        protected override RenderPipeline CreatePipeline() {
            this.renderPipeline?.Dispose();
            this.renderPipeline = new LaviRenderPipeline(this);

            return this.renderPipeline;
        }

        protected override void OnValidate() {
            this.renderPipeline?.Dispose();
            this.renderPipeline = null;

            base.OnValidate();
        }

        protected override void OnDisable() {
            this.renderPipeline?.Dispose();
            this.renderPipeline = null;

            base.OnDisable();
        }
    }
}