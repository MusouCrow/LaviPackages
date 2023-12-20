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

            context.Submit();
        }

        public void Dispose() {
            foreach (var material in this.materials) {
                CoreUtils.Destroy(material);
            }
        }

        private RenderTexutreRegister NewRTR(string name, int width, int height, GraphicsFormat format) {
            var rtr = new RenderTexutreRegister() {
                tid = Shader.PropertyToID(name),
                width = width,
                height = height,
                format = format
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