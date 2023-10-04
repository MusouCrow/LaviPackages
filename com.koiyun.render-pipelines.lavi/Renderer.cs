using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Koiyun.Render {
    public class Renderer : IDisposable {
        private List<IRenderPass> passes;
        private List<IRenderPass> culledPasses;
        private LaviRenderPipelineAsset asset;

        public Renderer(LaviRenderPipelineAsset asset) {
            this.asset = asset;
            var lightModes = new string[] {"Forward", "SRPDefaultUnlit"};
            var postRTR = new RenderTexutreRegister() {
                tid = RenderConst.POST_TEXTURE_ID,
                RTDHandler = (RenderTextureDescriptor rtd) => {
                    rtd.graphicsFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.HDR);

                    return rtd;
                }
            };
            
            var colorRTRs = new RenderTexutreRegister[] {
                new RenderTexutreRegister() {
                    tid = RenderConst.CAMERA_COLOR_TEXTURE_ID,
                    RTDHandler = (RenderTextureDescriptor rtd) => {
                        rtd.width = (int)(rtd.width * asset.RenderScale);
                        rtd.height = (int)(rtd.height * asset.RenderScale);

                        return rtd;
                    }
                },
                new RenderTexutreRegister() {
                    tid = RenderConst.CAMERA_GLOW_TEXTURE_ID,
                    RTDHandler = (RenderTextureDescriptor rtd) => {
                        rtd.width = (int)(rtd.width * asset.RenderScale);
                        rtd.height = (int)(rtd.height * asset.RenderScale);

                        return rtd;
                    }
                }
            };

            var depthRTR = new RenderTexutreRegister() {
                tid = RenderConst.CAMERA_DEPTH_TEXTURE_ID,
                RTDHandler = (RenderTextureDescriptor rtd) => {
                    rtd.colorFormat = RenderTextureFormat.Depth;
                    rtd.width = (int)(rtd.width * asset.RenderScale);
                    rtd.height = (int)(rtd.height * asset.RenderScale);

                    return rtd;
                }
            };

            var pixelRTR = new RenderTexutreRegister() {
                tid = RenderConst.PIXEL_TEXTURE_ID,
                RTDHandler = (RenderTextureDescriptor rtd) => {
                    var scale = RenderUtil.GetPixelScale(rtd.width);
                    rtd.width = (int)(rtd.width * scale);
                    rtd.height = (int)(rtd.height * scale);

                    return rtd;
                }
            };

            var finalRTR = new RenderTexutreRegister() {
                tid = (int)BuiltinRenderTextureType.CameraTarget
            };

            this.passes = new List<IRenderPass>() {
                new SetupPass(this.asset, postRTR),
                new MainLightShadowPass(this.asset),
                new ReadyDrawPass(colorRTRs, depthRTR),
                new DrawObjectPass(lightModes, true),
                new DrawObjectPass(new string[] {"Outline"}, true),
                new DrawObjectPass(lightModes, false),
                new DrawGizmosPass(GizmoSubset.PreImageEffects),
                new DrawGizmosPass(GizmoSubset.PostImageEffects),

                new BloomPass("Hidden/Lavi RP/Bloom", RenderConst.POST_TEXTURE_ID, RenderConst.CAMERA_COLOR_TEXTURE_ID, RenderConst.CAMERA_GLOW_TEXTURE_ID, 5),
                new CopyColorPass("Hidden/Lavi RP/Blit", RenderConst.POST_TEXTURE_ID, pixelRTR, true, false),
                new CopyColorPass("Hidden/Lavi RP/Blit", RenderConst.PIXEL_TEXTURE_ID, finalRTR, false, true),

                // new CopyColorPass("Hidden/Lavi RP/Blit", RenderConst.POST_TEXTURE_ID, finalRTR, false, false),
                new CopyDepthPass("Hidden/Lavi RP/Blit"),
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
                cameraRTD = RenderUtil.CreateCameraRenderTextureDescriptor(camera, MSAASamples.None),
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

        public void Dispose() {
            for (int i = 0; i < this.passes.Count; i++) {
                this.passes[i].Dispose();
            }
        }
    }
}