using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

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
            var blitMaterial = this.NewMaterial("Hidden/Lavi RP/Blit");
            var deferredMaterial = this.NewMaterial("Hidden/Lavi RP/Deferred");

            var sceneShadowRTR = this.NewRTR("_SceneShadowMap", TextureFormat.Shadow, false, true, (int)this.asset.ShadowResolution);
            var unitShadowRTR = this.NewRTR("_UnitShadowMap", TextureFormat.Shadow, false, true, (int)this.asset.ShadowResolution);
            var gBufferColorRTR = this.NewRTR("_GBufferColor", TextureFormat.LDR, true, true);
            var gBufferOtherRTR = this.NewRTR("_GBufferOther", TextureFormat.LDR, true, true);
            var gBufferNormalRTR = this.NewRTR("_GBufferNormal", TextureFormat.Normal, true, true);
            var colorRTR = this.NewRTR("_ColorMap", TextureFormat.LDR, true);
            var depthRTR = this.NewRTR("_DepthMap", TextureFormat.Depth, true);
            var glowRTR = this.NewRTR("_GlowMap", TextureFormat.HDR, true, true);
            
            var setupPass = new SetupPass(this.asset, this.rtrs);
            var mainShadowPass = new MainShadowPass(this.asset, sceneShadowRTR, unitShadowRTR);
            var gBufferPass = new GBufferPass("GBuffer", gBufferColorRTR, gBufferOtherRTR, gBufferNormalRTR, depthRTR);
            var deferredPass = new DeferredPass(deferredMaterial, colorRTR, depthRTR);
            var packGlowPass = new PackGlowPass(glowRTR);
            var drawTransparentPass = new DrawTransparentPass("Transparent", colorRTR, glowRTR, depthRTR);
            var drawErrorPass = new DrawErrorPass("SRPDefaultUnlit", colorRTR, depthRTR);
            var drawGizmosPass = new DrawGizmosPass(colorRTR, depthRTR);
            var finalBlitPass = new FinalBlitPass(colorRTR, blitMaterial);
            var finalDepthBlitPass = new FinalBlitPass(depthRTR, blitMaterial);
            var cleanPass = new CleanPass(this.rtrs);
            
            this.passes.Add(setupPass);
            this.passes.Add(mainShadowPass);
            this.passes.Add(gBufferPass);
            
            this.passes.Add(deferredPass);
            this.passes.Add(packGlowPass);
            this.passes.Add(drawTransparentPass);
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
                cameraRTD = RenderUtil.CreateCameraRenderTextureDescriptor(camera, MSAASamples.None),
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

        private RenderTexutreRegister NewRTR(string name, TextureFormat format = TextureFormat.LDR, bool scaling = true, bool global = false, int size = 0) {
            var rtr = new RenderTexutreRegister() {
                tid = Shader.PropertyToID(name),
                size = size,
                scaling = scaling,
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