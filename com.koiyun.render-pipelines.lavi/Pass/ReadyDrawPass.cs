using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class ReadyDrawPass : IRenderPass {
        private RenderTexutreRegister[] colorRTRs;
        private RenderTexutreRegister depthRTR;
        private RenderTargetIdentifier[] colorRTIs;

        public ReadyDrawPass(RenderTexutreRegister[] colorRTRs, RenderTexutreRegister depthRTR) {
            this.colorRTRs = colorRTRs;
            this.depthRTR = depthRTR;
            this.colorRTIs = new RenderTargetIdentifier[this.colorRTRs.Length];
        }

        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            return true;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("ReadyDrawPass");
            var depthRTI = RenderUtil.ReadyRT(cmd, ref data, ref this.depthRTR);

            for (int i = 0; i < this.colorRTIs.Length; i++) {
                this.colorRTIs[i] = RenderUtil.ReadyRT(cmd, ref data, ref this.colorRTRs[i]);
            }

            if (data.camera.cameraType == CameraType.SceneView) {
                // 先设置颜色贴图的背景颜色
                cmd.SetRenderTarget(this.colorRTIs[0], depthRTI);
                cmd.ClearRenderTarget(true, true, data.camera.backgroundColor.linear);
                context.ExecuteCommandBuffer(cmd);

                // 再清空Glow贴图
                cmd.SetRenderTarget(this.colorRTIs[1]);
                cmd.ClearRenderTarget(true, true, Color.clear);
                context.ExecuteCommandBuffer(cmd);

                // 最后进行总部署
                cmd.SetRenderTarget(this.colorRTIs, depthRTI);
                context.ExecuteCommandBuffer(cmd);
            }
            else {
                cmd.SetRenderTarget(this.colorRTIs, depthRTI);
                cmd.ClearRenderTarget(true, true, Color.clear);
                context.ExecuteCommandBuffer(cmd);
            }
            
            CommandBufferPool.Release(cmd);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("ReadyDrawPass");

            for (int i = 0; i < this.colorRTRs.Length; i++) {
                cmd.ReleaseTemporaryRT(this.colorRTRs[i].tid);
            }

            cmd.ReleaseTemporaryRT(this.depthRTR.tid);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose() {
            
        }
    }
}