using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class BloomPass : RenderPass {
        private Material material;
        private RenderTexutreRegister colorRTR;
        private RenderTexutreRegister bloomRTR;
        private RenderTexutreRegister[] bloomBlurHRTRs;
        private RenderTexutreRegister[] bloomBlurVRTRs;

        private int packPassIndex;
        private int blurHPassIndex;
        private int blurVPassIndex;
        private int upSamplePassIndex;
        private int blendPassIndex;

        public BloomPass(Material material, RenderTexutreRegister colorRTR, RenderTexutreRegister bloomRTR, RenderTexutreRegister[] bloomBlurHRTRs, RenderTexutreRegister[] bloomBlurVRTRs) {
            this.material = material;
            this.colorRTR = colorRTR;
            this.bloomRTR = bloomRTR;
            this.bloomBlurHRTRs = bloomBlurHRTRs;
            this.bloomBlurVRTRs = bloomBlurVRTRs;
            
            this.packPassIndex = this.material.FindPass("Pack");
            this.blurHPassIndex = this.material.FindPass("BlurH");
            this.blurVPassIndex = this.material.FindPass("BlurV");
            this.upSamplePassIndex = this.material.FindPass("UpSample");
            this.blendPassIndex = this.material.FindPass("Blend");
        }

        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("BloomPass");

            // Pack
            cmd.SetRenderTarget(this.bloomRTR.RTI);
            cmd.DrawProcedural(Matrix4x4.identity, this.material, this.packPassIndex, MeshTopology.Triangles, 3, 1);
           
            var bloomBlurRTR = this.bloomRTR;
            var step = this.bloomBlurHRTRs.Length;

            // Blur Loop
            for (int i = 0; i < step; i++) {
                cmd.SetRenderTarget(this.bloomBlurHRTRs[i].RTI);
                cmd.SetGlobalTexture(RenderConst.BLOOM_BLUR_TEXTURE_ID, bloomBlurRTR.RTI);
                cmd.DrawProcedural(Matrix4x4.identity, this.material, this.blurHPassIndex, MeshTopology.Triangles, 3, 1);

                cmd.SetRenderTarget(this.bloomBlurVRTRs[i].RTI);
                cmd.SetGlobalTexture(RenderConst.BLOOM_BLUR_TEXTURE_ID, this.bloomBlurHRTRs[i].RTI);
                cmd.DrawProcedural(Matrix4x4.identity, this.material, this.blurVPassIndex, MeshTopology.Triangles, 3, 1);

                bloomBlurRTR = this.bloomBlurVRTRs[i];
            }

            // UpSample
            for (int i = step - 2; i >= 0; i--) {
                if (i == step - 2) {
                    cmd.SetGlobalTexture(RenderConst.BLOOM_BLUR_HIGH_TEXTURE_ID, this.bloomBlurVRTRs[i].RTI);
                    cmd.SetGlobalTexture(RenderConst.BLOOM_BLUR_LOW_TEXTURE_ID, this.bloomBlurVRTRs[i + 1].RTI);
                }
                else {
                    cmd.SetGlobalTexture(RenderConst.BLOOM_BLUR_HIGH_TEXTURE_ID, this.bloomBlurVRTRs[i].RTI);
                    cmd.SetGlobalTexture(RenderConst.BLOOM_BLUR_LOW_TEXTURE_ID, this.bloomBlurHRTRs[i + 1].RTI);
                }

                cmd.SetRenderTarget(this.bloomBlurHRTRs[i].RTI);
                cmd.DrawProcedural(Matrix4x4.identity, this.material, this.upSamplePassIndex, MeshTopology.Triangles, 3, 1);
            }
            
            // Blend
            cmd.SetGlobalTexture(RenderConst.BLOOM_BLUR_TEXTURE_ID, this.bloomBlurHRTRs[0].RTI);
            cmd.SetRenderTarget(this.colorRTR.RTI);
            cmd.DrawProcedural(Matrix4x4.identity, this.material, this.blendPassIndex, MeshTopology.Triangles, 3, 1);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}