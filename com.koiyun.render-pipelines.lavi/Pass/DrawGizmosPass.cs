using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace Koiyun.Render {
    public class DrawGizmosPass : RenderPass {
        private RenderTexutreRegister colorRTR;
        private RenderTexutreRegister depthRTR;

        public DrawGizmosPass(RenderTexutreRegister colorRTR, RenderTexutreRegister depthRTR) {
            this.colorRTR = colorRTR;
            this.depthRTR = depthRTR;
        }

        public override bool IsActived(ref RenderData data) {
        #if UNITY_EDITOR
            return Handles.ShouldRenderGizmos() && data.camera.sceneViewFilterMode == Camera.SceneViewFilterMode.Off;
        #else
            return false;        
        #endif
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("DrawGizmosPass");
            cmd.SetRenderTarget(this.colorRTR.RTI, this.depthRTR.RTI);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            context.DrawGizmos(data.camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(data.camera, GizmoSubset.PostImageEffects);
        }
    }
}