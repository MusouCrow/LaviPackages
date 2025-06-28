using UnityEngine;
using UnityEngine.Rendering;
using System;


#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Koiyun.Render {
    [CreateAssetMenu(menuName = "Rendering/LaviRenderPipelineAsset")]
    public class LaviRenderPipelineAsset : RenderPipelineAsset {
    #if ODIN_INSPECTOR
        [TitleGroup("初始资源")]
        [LabelText("初始着色器")]
        public Shader DefaultShader;
        
        [TitleGroup("初始资源")]
        [LabelText("默认材质")]
        public Material DefaultMaterial;

        [TitleGroup("初始资源")]
        [LabelText("默认精灵材质")]
        public Material DefaultSpriteMaterial;

        [TitleGroup("阴影")]
        [LabelText("阴影分辨率")]
        public ShadowResolution ShadowResolution = ShadowResolution._2048;
        
        [TitleGroup("阴影")]
        [LabelText("阴影距离")]
        public float ShadowDistance = 50;

        [TitleGroup("阴影")]
        [LabelText("深度偏移")]
        [Range(0, 2)]
        public float DepthBias;

        [TitleGroup("阴影")]
        [LabelText("法线偏移")]
        [Range(0, 3)]
        public float NormalBias;

        [TitleGroup("阴影")]
        [LabelText("阴影深度范围")]
        [MinMaxSlider(0, 10, true)]
        public Vector2 ShadowDepthRange = new Vector2(1, 2.5f);

        [TitleGroup("阴影")]
        [LabelText("阴影刻度")]
        [Range(0, 1)]
        public float ShadowStep;

        [TitleGroup("阴影")]
        [LabelText("阴影强度范围")]
        [MinMaxSlider(0, 1, true)]
        public Vector2 ShadowAttenRange;

        [TitleGroup("阴影")]
        [LabelText("阴影强度范围（明暗面）")]
        [MinMaxSlider(0, 1, true)]
        public Vector2 DarkBrightAttenRange;

        [TitleGroup("描边")]
        [LabelText("描边明度")]
        [Range(0, 1)]
        public float OutlineBrightness;

        [TitleGroup("描边")]
        [LabelText("描边粗细")]
        public float OutlineThickness;

        [TitleGroup("其他")]
        [LabelText("SRP批处理")]
        public bool SRPBatch = true;
    #else
        [Header("初始着色器")]
        public Shader DefaultShader;
        
        [Header("默认材质")]
        public Material DefaultMaterial;

        [Header("默认精灵材质")]
        public Material DefaultSpriteMaterial;

        [Header("阴影分辨率")]
        public ShadowResolution ShadowResolution = ShadowResolution._2048;
        
        [Header("阴影距离")]
        public float ShadowDistance = 50;

        [Header("深度偏移")]
        [Range(0, 2)]
        public float DepthBias;

        [Header("法线偏移")]
        [Range(0, 3)]
        public float NormalBias;

        [Header("阴影深度范围(0 - 10)")]
        public Vector2 ShadowDepthRange = new Vector2(1, 2.5f);

        [Header("阴影刻度")]
        [Range(0, 1)]
        public float ShadowStep;

        [Header("阴影强度范围(0 - 1)")]
        public Vector2 ShadowAttenRange;

        [Header("阴影强度范围（明暗面）(0 - 1)")]
        public Vector2 DarkBrightAttenRange;

        [Header("描边明度")]
        [Range(0, 1)]
        public float OutlineBrightness;

        [Header("描边粗细")]
        public float OutlineThickness;

        [Header("SRP批处理")]
        public bool SRPBatch = true;
    #endif

        [NonSerialized]
        public float Time;

        [NonSerialized]
        public Vector2 FogRange;

        [NonSerialized]
        public Color FogColor;

        [NonSerialized]
        public float LightIntensty;

        [NonSerialized]
        public Color LightColor;

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

        public override Material defaultUIMaterial {
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
                return 1;
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