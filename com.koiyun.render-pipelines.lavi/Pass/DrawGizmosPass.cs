using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace Koiyun.Render {
    public class DrawGizmosPass : IRenderPass {
        public GizmoSubset mode;

        public DrawGizmosPass(GizmoSubset mode) {
            this.mode = mode;
        }

        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
        #if UNITY_EDITOR
            return Handles.ShouldRenderGizmos() && data.camera.sceneViewFilterMode == Camera.SceneViewFilterMode.Off;
        #else
            return false;        
        #endif
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            context.DrawGizmos(data.camera, this.mode);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {
            
        }
    }
}