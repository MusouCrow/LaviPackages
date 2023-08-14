using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Koiyun.Render {
    public class BloomPass : IRenderPass {
        private Material material;
        private int packPassIndex;
        private int blurHPassIndex;
        private int blurVPassIndex;
        private int upSamplePassIndex;
        private int blendPassIndex;

        private int postTID;
        private int colorTID;
        private int glowTID;

        private RenderTexutreRegister[] blurHRTRs;
        private RenderTexutreRegister[] blurVRTRs;
        
        public BloomPass(string shaderName, int postTID, int colorTID, int glowTID, int step) {
            var shader = Shader.Find(shaderName);
            this.material = new Material(shader);
            this.packPassIndex = this.material.FindPass("Pack");
            this.blurHPassIndex = this.material.FindPass("BlurH");
            this.blurVPassIndex = this.material.FindPass("BlurV");
            this.upSamplePassIndex = this.material.FindPass("UpSample");
            this.blendPassIndex = this.material.FindPass("Blend");
            
            this.postTID = postTID;
            this.colorTID = colorTID;
            this.glowTID = glowTID;

            this.blurHRTRs = new RenderTexutreRegister[step];
            this.blurVRTRs = new RenderTexutreRegister[step];

            for (int i = 0; i < step; i++) {
                int n = i;

                Func<RenderTextureDescriptor, RenderTextureDescriptor> Handler = (RenderTextureDescriptor rtd) => {
                    var rate = 2 * (n + 1);
                    rtd.width /= rate;
                    rtd.height /= rate;
                    rtd.graphicsFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.HDR);
                    
                    return rtd;
                };

                this.blurHRTRs[i] = new RenderTexutreRegister() {
                    tid = Shader.PropertyToID(RenderConst.BLOOM_BLUR_H_TEXTURE_NAME + i),
                    RTDHandler = Handler
                };

                this.blurVRTRs[i] = new RenderTexutreRegister() {
                    tid = Shader.PropertyToID(RenderConst.BLOOM_BLUR_V_TEXTURE_NAME + i),
                    RTDHandler = Handler
                };
            }
        }

        public bool Setup(ref ScriptableRenderContext context, ref RenderData data) {
            return true;
        }

        public void Render(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("BloomPass");
            var colorRTI = new RenderTargetIdentifier(this.colorTID);
            var glowRTI = new RenderTargetIdentifier(this.glowTID);
            var postRTI = new RenderTargetIdentifier(this.postTID);
            var step = this.blurHRTRs.Length;

            for (int i = 0; i < step; i++) {
                RenderUtil.ReadyRT(cmd, ref data, ref this.blurHRTRs[i]);
                RenderUtil.ReadyRT(cmd, ref data, ref this.blurVRTRs[i]);
            }

            // Pack
            cmd.SetGlobalTexture(this.glowTID, glowRTI);
            cmd.Blit(colorRTI, postRTI, this.material, this.packPassIndex);
            
            // Blur
            cmd.Blit(postRTI, this.blurHRTRs[0].RTI, this.material, this.blurHPassIndex);
            cmd.Blit(this.blurHRTRs[0].RTI, this.blurVRTRs[0].RTI, this.material, this.blurVPassIndex);
            
            for (int i = 1; i < step; i++) {
                cmd.Blit(this.blurVRTRs[i - 1].RTI, this.blurHRTRs[i].RTI, this.material, this.blurHPassIndex);
                cmd.Blit(this.blurHRTRs[i].RTI, this.blurVRTRs[i].RTI, this.material, this.blurVPassIndex);
            }
            
            // UpSample
            for (int i = step - 2; i >= 0; i--) {
                cmd.SetGlobalTexture(RenderConst.BLOOM_BLUR_TEXTURE_ID, this.blurHRTRs[i + 1].RTI);
                cmd.Blit(this.blurVRTRs[i].RTI, this.blurHRTRs[i].RTI, this.material, this.upSamplePassIndex);
            }
            
            // Blend
            cmd.SetGlobalTexture(RenderConst.BLOOM_BLUR_TEXTURE_ID, this.blurHRTRs[0].RTI);
            cmd.Blit(colorRTI, postRTI, this.material, this.blendPassIndex);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Clean(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("BloomPass");

            for (int i = 0; i < this.blurHRTRs.Length; i++) {
                cmd.ReleaseTemporaryRT(this.blurHRTRs[i].tid);
                cmd.ReleaseTemporaryRT(this.blurVRTRs[i].tid);
            }
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}