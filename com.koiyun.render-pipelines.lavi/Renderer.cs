using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class Renderer {
        private List<IRenderPass> passes;
        private List<IRenderPass> culledPasses;
        private LaviRenderPipelineAsset asset;

        public Renderer(LaviRenderPipelineAsset asset) {
            this.asset = asset;
            var lightModes = new string[] {"Forward", "SRPDefaultUnlit"};
            var colorRTRs = new RenderTexutreRegister[] {
                new RenderTexutreRegister() {
                    tid = RenderConst.CAMERA_COLOR_TEXTURE_ID
                },
                new RenderTexutreRegister() {
                    tid = RenderConst.CAMERA_ITENSITY_TEXTURE_ID
                }
            };

            var depthRTR = new RenderTexutreRegister() {
                tid = RenderConst.CAMERA_DEPTH_TEXTURE_ID,
                RTDHandler = (RenderTextureDescriptor rtd) => {
                    rtd.colorFormat = RenderTextureFormat.Depth;

                    return rtd;
                }
            };

            this.passes = new List<IRenderPass>() {
                new SetupPass(this.asset),
                new MainLightShadowPass(this.asset),
                new ReadyDrawPass(colorRTRs, depthRTR),
                new DrawObjectPass(lightModes, true),
                new DrawObjectPass(new string[] {"Outline"}, true),
                new DrawObjectPass(lightModes, false),
                new DrawGizmosPass(GizmoSubset.PreImageEffects),
                new DrawGizmosPass(GizmoSubset.PostImageEffects),
                new FinalPass(RenderConst.CAMERA_COLOR_TEXTURE_ID),
                new CopyDepthPass(),
            };

            this.culledPasses = new List<IRenderPass>(this.passes.Count);
            GraphicsSettings.useScriptableRenderPipelineBatching = this.asset.SRPBatch;
        }

        public void Render(ref ScriptableRenderContext context, Camera camera) {
            camera.TryGetCullingParameters(out var cullingParameters);
            cullingParameters.shadowDistance = this.asset.ShadowDistance;

            var cullingResults = context.Cull(ref cullingParameters);
            context.SetupCameraProperties(camera);
            
            var data = new RenderData() {
                camera = camera,
                cullingResults = cullingResults,
                cameraRTD = RenderUtil.CreateCameraRenderTextureDescriptor(camera, MSAASamples.None, this.asset.RenderScale),
                mainLightIndex = RenderUtil.GetMainLightIndex(ref cullingResults)
            };

            this.culledPasses.Clear();

            for (int i = 0; i < this.passes.Count; i++) {
                if (this.passes[i].Setup(ref context, ref data)) {
                    this.culledPasses.Add(this.passes[i]);
                }
            }

            for (int i = 0; i < this.culledPasses.Count; i++) {
                this.culledPasses[i].Render(ref context, ref data);
            }

            for (int i = this.culledPasses.Count - 1; i >= 0; i--) {
                this.culledPasses[i].Clean(ref context, ref data);
            }

            context.Submit();
        }
    }
}