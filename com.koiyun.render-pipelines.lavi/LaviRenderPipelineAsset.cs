using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    [CreateAssetMenu(menuName = "Rendering/LaviRenderPipelineAsset")]
    public class LaviRenderPipelineAsset : RenderPipelineAsset {
        public Shader DefaultShader;
        public Material DefaultMaterial;
        [Range(0.25f, 2)]
        public float RenderScale = 1;
        public ShadowResolution ShadowResolution = ShadowResolution._2048;
        public float ShadowDistance = 50;
        public bool SRPBatch = true;

        public override Material defaultMaterial {
            get {
                return this.DefaultMaterial;
            }
        }

        public override Shader defaultShader {
            get {
                return this.DefaultShader;
            }
        }

        protected override RenderPipeline CreatePipeline() {
            return new LaviRenderPipeline(this);
        }
    }
}