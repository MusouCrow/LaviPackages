using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SocialPlatforms;

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
            var deferredMaterial = this.NewMaterial("Hidden/Lavi RP/Deferred");
            var packGlowMaterial = this.NewMaterial("Hidden/Lavi RP/PackGlow");
            var bloomMaterial = this.NewMaterial("Hidden/Lavi RP/Bloom");
            var blitMaterial = this.NewMaterial("Hidden/Lavi RP/Blit");

            var sceneShadowRTR = this.NewRTR("_SceneShadowTexture", TextureFormat.Shadow, 1, true, (int)this.asset.ShadowResolution);
            var unitShadowRTR = this.NewRTR("_UnitShadowTexture", TextureFormat.Shadow, 1, true, (int)this.asset.ShadowResolution);
            var rawDepthRTR = this.NewRTR("_RawDepthTexture", TextureFormat.Depth, scale, true);
            var gBufferColorRTR = this.NewRTR("_GBufferColor", TextureFormat.LDR, scale, true);
            var gBufferOtherRTR = this.NewRTR("_GBufferOther", TextureFormat.Data, scale, true);
            var gBufferNormalRTR = this.NewRTR("_GBufferNormal", TextureFormat.Normal, scale, true);
            var depthRTR = this.NewRTR("_DepthTexture", TextureFormat.Depth, scale);
            var colorRTR = this.NewRTR("_ColorTexture", TextureFormat.LDR, scale);
            var glowRTR = this.NewRTR("_GlowTexture", TextureFormat.HDR, scale, true);
            var bloomRTR = this.NewRTR("_BloomTexture", TextureFormat.LDR, 1, true);
            var bloomBlurHRTRs = new RenderTexutreRegister[RenderConst.BLOOM_STEP];
            var bloomBlurVRTRs = new RenderTexutreRegister[RenderConst.BLOOM_STEP];

            var bloomScale = 1.0f;

            for (var i = 0; i < RenderConst.BLOOM_STEP; i++) {
                bloomScale /= 2;
                bloomBlurHRTRs[i] = this.NewRTR("_BloomBlurHTexture" + i, TextureFormat.HDR, bloomScale);
                bloomBlurVRTRs[i] = this.NewRTR("_BloomBlurVTexture" + i, TextureFormat.HDR, bloomScale);
            }
            
            var setupPass = new SetupPass(this.asset, this.rtrs);
            var mainShadowPass = new MainShadowPass(this.asset, sceneShadowRTR, unitShadowRTR);
            var gBufferPass = new GBufferPass("GBuffer", gBufferColorRTR, gBufferOtherRTR, gBufferNormalRTR, rawDepthRTR);
            var copyDepthPass = new CopyDepthPass(copyTextureMaterial, depthRTR);
            var deferredPass = new DeferredPass(deferredMaterial, colorRTR, rawDepthRTR);
            var packGlowPass = new PackGlowPass(packGlowMaterial, glowRTR);
            var drawTransparentPass = new DrawTransparentPass("Transparent", colorRTR, glowRTR, rawDepthRTR);
            var bloomPass = new BloomPass(bloomMaterial, colorRTR, glowRTR, bloomRTR, bloomBlurHRTRs, bloomBlurVRTRs);
            var drawErrorPass = new DrawErrorPass("SRPDefaultUnlit", colorRTR, rawDepthRTR);
            var drawGizmosPass = new DrawGizmosPass(colorRTR, rawDepthRTR);
            var finalBlitPass = new FinalBlitPass(colorRTR, blitMaterial);
            var finalDepthBlitPass = new FinalBlitPass(rawDepthRTR, blitMaterial);
            var cleanPass = new CleanPass(this.rtrs);
            
            this.passes.Add(setupPass);
            this.passes.Add(mainShadowPass);
            this.passes.Add(gBufferPass);
            this.passes.Add(copyDepthPass);
            this.passes.Add(deferredPass);
            this.passes.Add(packGlowPass);
            this.passes.Add(drawTransparentPass);
            this.passes.Add(bloomPass);
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

        private RenderTexutreRegister NewRTR(string name, TextureFormat format = TextureFormat.LDR, float scale = 1, bool global = false, int size = 0) {
            var rtr = new RenderTexutreRegister() {
                tid = Shader.PropertyToID(name),
                size = size,
                scale = scale,
                format = format,
                global = global
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