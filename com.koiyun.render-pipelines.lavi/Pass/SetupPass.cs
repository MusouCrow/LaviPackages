using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Koiyun.Render {
    public class SetupPass : RenderPass {
        private LaviRenderPipelineAsset asset;
        private List<RenderTexutreRegister> rtrs;

        public SetupPass(LaviRenderPipelineAsset asset, List<RenderTexutreRegister> rtrs) {
            this.asset = asset;
            this.rtrs = rtrs;
        }
        
        public override void Execute(ref ScriptableRenderContext context, ref RenderData data) {
            var cmd = CommandBufferPool.Get("SetupPass");
            
            foreach (var rtr in this.rtrs) {
                var rtd = rtr.GetRTD(data.camera, this.asset.RenderScale);
                cmd.GetTemporaryRT(rtr.tid, rtd, FilterMode.Bilinear);
                
                if (rtr.global) {
                    cmd.SetGlobalTexture(rtr.tid, rtr.RTI);
                }
            }

            CoreUtils.SetKeyword(cmd, RenderConst.MAIN_LIGHT_SHADOW_KEYWORD, data.mainLightIndexes.Count > 0);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}