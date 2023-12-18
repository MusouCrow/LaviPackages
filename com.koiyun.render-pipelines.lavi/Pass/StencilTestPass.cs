using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Koiyun.Render {
    public class StencilTestPass : IRenderPass {
        private Material material;
        private RenderTexutreRegister rtr;
        private int srcTID;
        private int depthTID;

        public StencilTestPass(LaviRenderPipelineAsset asset, string shaderName, int srcTID, int depthTID) {
            this.material = CoreUtils.CreateEngineMaterial(shaderName);
            this.srcTID = srcTID;
            this.depthTID = depthTID;

            this.rtr = new RenderTexutreRegister() {
                tid = Shader.PropertyToID("_StencilTestTexture"),
                RTDHandler = (RenderTextureDescriptor rtd) => {
                    rtd.graphicsFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.HDR);
                    rtd.width = (int)(rtd.width * asset.RenderScale);
                    rtd.height = (int)(rtd.height * asset.RenderScale);

                    return rtd;
                }
            };
        }

        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            return true;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("StencilTestPass");
            var srcRTI = new RenderTargetIdentifier(this.srcTID);
            var depthRTI = new RenderTargetIdentifier(this.depthTID);

            RenderUtil.ReadyRT(cmd, ref data, ref this.rtr);
            cmd.SetRenderTarget(this.rtr.RTI, depthRTI);
            cmd.ClearRenderTarget(false, true, data.camera.backgroundColor.linear);
            cmd.SetGlobalTexture(RenderConst.BLIT_TEXTURE_ID, srcRTI);
            cmd.DrawProcedural(Matrix4x4.identity, material, 0, MeshTopology.Triangles, 3, 1);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("StencilTestPass");
            cmd.ReleaseTemporaryRT(this.rtr.tid);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose() {

        }
    }
}