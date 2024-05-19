using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class Renderer : IDisposable {
        private List<RenderPass> passes;
        private List<RenderTexutreRegister> rtrs;
        private List<Material> materials;
        private LaviRenderPipelineAsset asset;

        public Renderer(LaviRenderPipelineAsset asset) {
            this.asset = asset;
            this.passes = new List<RenderPass>();
            this.rtrs = new List<RenderTexutreRegister>();
            this.materials = new List<Material>();

            GraphicsSettings.useScriptableRenderPipelineBatching = this.asset.SRPBatch;
        }

        public void Ready() {
            var scale = this.asset.RenderScale;

            var copyTextureMaterial = this.NewMaterial("Hidden/Lavi RP/CopyTexture");
            var outlineMaterial = this.NewMaterial("Hidden/Lavi RP/Outline");
            var bloomMaterial = this.NewMaterial("Hidden/Lavi RP/Bloom");
            var blitMaterial = this.NewMaterial("Hidden/Lavi RP/Blit");

            var sceneShadowRTR = this.NewRTR("_SceneShadowTexture", TextureFormat.Shadow, 1, true, false, (int)this.asset.ShadowResolution);
            var unitShadowRTR = this.NewRTR("_UnitShadowTexture", TextureFormat.Shadow, 1, true, false, (int)this.asset.ShadowResolution);
            var rawDepthRTR = this.NewRTR("_RawDepthTexture", TextureFormat.Depth, scale, true);
            var rawColorRTR = this.NewRTR("_RawColorTexture", TextureFormat.LDR, scale, false, true);
            var rawParamRTR = this.NewRTR("_RawParamTexture", TextureFormat.LDR, scale, true, false);      
            var colorRTR = this.NewRTR("_ColorTexture", TextureFormat.LDR, scale, true, true);
            var paramRTR = this.NewRTR("_ParamTexture", TextureFormat.LDR, scale, true, false);
            var bloomRTR = this.NewRTR("_BloomTexture", TextureFormat.HDR, scale, false, true);
            var bloomBlurHRTRs = new RenderTexutreRegister[RenderConst.BLOOM_STEP];
            var bloomBlurVRTRs = new RenderTexutreRegister[RenderConst.BLOOM_STEP];

            var bloomScale = 1.0f;

            for (var i = 0; i < RenderConst.BLOOM_STEP; i++) {
                bloomScale /= 2;
                bloomBlurHRTRs[i] = this.NewRTR("_BloomBlurHTexture" + i, TextureFormat.HDR, bloomScale, false, true);
                bloomBlurVRTRs[i] = this.NewRTR("_BloomBlurVTexture" + i, TextureFormat.HDR, bloomScale, false, true);
            }
            
            var setupPass = new SetupPass(this.asset, this.rtrs);
            var mainShadowPass = new MainShadowPass(this.asset, sceneShadowRTR, unitShadowRTR);
            var drawOpaquePass = new DrawObjectPass("Opaque", true, true, true, rawColorRTR, rawParamRTR, rawDepthRTR);
            // var copyDepthPass = new CopyTexturePass(copyTextureMaterial, depthRTR);
            var copyColorPass = new CopyTexturePass(copyTextureMaterial, rawColorRTR, colorRTR);
            var copyParamPass = new CopyTexturePass(copyTextureMaterial, rawParamRTR, paramRTR);
            var outlinePass = new OutlinePass(outlineMaterial, rawParamRTR, rawDepthRTR);
            var drawTransparentPass = new DrawObjectPass("Transparent", false, false, false, rawColorRTR, rawParamRTR, rawDepthRTR);
            var bloomPass = new BloomPass(bloomMaterial, rawColorRTR, bloomRTR, bloomBlurHRTRs, bloomBlurVRTRs);
            var drawGrabPass = new DrawObjectPass("Grab", false, false, false, rawColorRTR, rawParamRTR, rawDepthRTR);
            var drawErrorPass = new DrawErrorPass("SRPDefaultUnlit", rawColorRTR, rawDepthRTR);
            var drawGizmosPass = new DrawGizmosPass(rawColorRTR, rawDepthRTR);
            var finalBlitPass = new FinalBlitPass(rawColorRTR, blitMaterial);
            var finalDepthBlitPass = new FinalBlitPass(rawDepthRTR, blitMaterial);
            var cleanPass = new CleanPass(this.rtrs);
            
            this.passes.Add(setupPass);
            this.passes.Add(mainShadowPass);
            this.passes.Add(drawOpaquePass);
            // this.passes.Add(copyDepthPass);
            this.passes.Add(copyColorPass);
            this.passes.Add(copyParamPass);
            this.passes.Add(outlinePass);
            this.passes.Add(drawTransparentPass);
            this.passes.Add(copyColorPass);
            this.passes.Add(bloomPass);
            this.passes.Add(copyColorPass);
            this.passes.Add(drawGrabPass);
            this.passes.Add(drawErrorPass);
            this.passes.Add(drawGizmosPass);
            this.passes.Add(finalBlitPass);
            this.passes.Add(finalDepthBlitPass);
            this.passes.Add(cleanPass);
        }

        public void Render(ref ScriptableRenderContext context, Camera camera) {
            camera.TryGetCullingParameters(out var cullingParameters);
            cullingParameters.shadowDistance = this.asset.ShadowDistance;

            var cullingResults = context.Cull(ref cullingParameters);
            context.SetupCameraProperties(camera);
            
            var data = new RenderData() {
                camera = camera,
                cullingResults = cullingResults,
                mainLightIndexes = RenderUtil.GetMainLightIndexes(ref cullingResults)
            };

            foreach (var pass in this.passes) {
                if (pass.IsActived(ref data)) {
                    pass.Execute(ref context, ref data);
                }
            }

            context.Submit();
        }

        public void Dispose() {
            foreach (var material in this.materials) {
                CoreUtils.Destroy(material);
            }
        }

        private RenderTexutreRegister NewRTR(string name, TextureFormat format = TextureFormat.LDR, float scale = 1, bool global = false, bool srgb = false, int size = 0) {
            var rtr = new RenderTexutreRegister() {
                tid = Shader.PropertyToID(name),
                size = size,
                scale = scale,
                format = format,
                global = global,
                srgb = srgb
            };

            this.rtrs.Add(rtr);
            
            return rtr;
        }

        private Material NewMaterial(string shaderName) {
            var material = CoreUtils.CreateEngineMaterial(shaderName);
            this.materials.Add(material);

            return material;
        }
    }
}