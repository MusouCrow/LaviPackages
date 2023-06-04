using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class DrawObjectPass : IRenderPass {
        private string[] lightModes;
        private FilteringSettings filteringSettings;

        public bool IsOpaque {
            get;
            private set;
        }

        public DrawObjectPass(bool isOpaque) {
            this.IsOpaque = isOpaque;
            this.lightModes = new string[] {"Forward", "SRPDefaultUnlit"};

            var renderQueueRange = isOpaque ? RenderQueueRange.opaque : RenderQueueRange.transparent;
            this.filteringSettings = new FilteringSettings(renderQueueRange);
        }

        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            return true;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var drawingSettings = RenderUtil.CreateDrawingSettings(ref data, this.lightModes, this.IsOpaque);
            context.DrawRenderers(data.cullingResults, ref drawingSettings, ref this.filteringSettings);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {

        }
    }
}