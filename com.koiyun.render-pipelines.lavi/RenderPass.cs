using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class RenderPass {
        public virtual bool IsActived(ref RenderData data) {
            return true;
        }

        public virtual void Render(ref ScriptableRenderContext context, ref RenderData data) {}
    }
}