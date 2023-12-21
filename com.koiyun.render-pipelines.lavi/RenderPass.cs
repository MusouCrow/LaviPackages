using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class RenderPass {
        public virtual bool IsActived(ref RenderData data) {
            return true;
        }

        public virtual void Execute(ref ScriptableRenderContext context, ref RenderData data) {}
    }
}