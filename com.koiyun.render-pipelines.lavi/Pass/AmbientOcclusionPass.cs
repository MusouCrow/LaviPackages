using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class AmbientOcclusionPass : RenderPass {
        private Material material;
        private RenderTexutreRegister colorRTR;
        private RenderTexutreRegister depthRTR;
        
        public AmbientOcclusionPass(Material material, RenderTexutreRegister colorRTR, RenderTexutreRegister depthRTR) {
            this.material = material;
            this.colorRTR = colorRTR;
            this.depthRTR = depthRTR;
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("AmbientOcclusionPass");

            var view = data.camera.worldToCameraMatrix;
            var proj = data.camera.projectionMatrix;
            var vp = proj * view;

            view.SetColumn(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f)); 

            var cviewProj = proj * view;
            var cviewProjInv = cviewProj.inverse;

            Vector4 topLeftCorner = cviewProjInv.MultiplyPoint(new Vector4(-1, 1, -1, 1));
            Vector4 topRightCorner = cviewProjInv.MultiplyPoint(new Vector4(1, 1, -1, 1));
            Vector4 bottomLeftCorner = cviewProjInv.MultiplyPoint(new Vector4(-1, -1, -1, 1));

            cmd.SetGlobalMatrix(RenderConst.AO_VP_MATRIX_ID, vp);
            cmd.SetGlobalVector(RenderConst.AO_VIEW_EXTENTX_ID, topLeftCorner);
            cmd.SetGlobalVector(RenderConst.AO_VIEW_EXTENTY_ID, topRightCorner - topLeftCorner);
            cmd.SetGlobalVector(RenderConst.AO_VIEW_EXTENTZ_ID, bottomLeftCorner - topLeftCorner);

            cmd.SetRenderTarget(this.colorRTR.RTI, this.depthRTR.RTI);
            cmd.DrawProcedural(Matrix4x4.identity, this.material, 0, MeshTopology.Triangles, 3, 1);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}