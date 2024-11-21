using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

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
                var rtd = rtr.GetRTD(data.camera);
                cmd.GetTemporaryRT(rtr.tid, rtd, FilterMode.Bilinear);
                
                if (rtr.global) {
                    cmd.SetGlobalTexture(rtr.tid, rtr.RTI);
                }
            }

            context.SetupCameraProperties(data.camera);
            CoreUtils.SetKeyword(cmd, RenderConst.MAIN_LIGHT_SHADOW_KEYWORD, data.mainLightIndexes.Count > 0);

            this.SetOutlineParams(cmd);
            this.SetTimeParams(cmd);
            this.SetFogParams(cmd);
            this.SetLightParams(cmd);

            VFXManager.ProcessCameraCommand(data.camera, cmd, new VFXCameraXRSettings(), data.cullingResults);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void SetOutlineParams(CommandBuffer cmd) {
            var param = new Vector2(this.asset.OutlineBrightness, this.asset.OutlineThickness);
            cmd.SetGlobalVector(RenderConst.OUTLINE_PARAMS_ID, param);
        }

        private void SetTimeParams(CommandBuffer cmd) {
            var time = Application.isPlaying ? this.asset.Time : Time.realtimeSinceStartup;
            
            cmd.SetGlobalFloat(RenderConst.TIME_ID, time);
        }

        private void SetFogParams(CommandBuffer cmd) {
            cmd.SetGlobalVector(RenderConst.FOG_RANGE_ID, this.asset.FogRange);
            cmd.SetGlobalColor(RenderConst.FOG_COLOR_ID, this.asset.FogColor);
        }

        private void SetLightParams(CommandBuffer cmd) {
            cmd.SetGlobalFloat(RenderConst.LIGHT_INTENSTY_ID, this.asset.LightIntensty);
            cmd.SetGlobalColor(RenderConst.LIGHT_COLOR_ID, this.asset.LightColor);
        }
    }
}