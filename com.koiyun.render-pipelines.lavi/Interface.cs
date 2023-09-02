using System;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public interface IRenderPass : IDisposable {
        public bool Setup(ref ScriptableRenderContext context, ref RenderData data);
        public void Render(ref ScriptableRenderContext context, ref RenderData data);
        public void Clean(ref ScriptableRenderContext context, ref RenderData data);
    }
}