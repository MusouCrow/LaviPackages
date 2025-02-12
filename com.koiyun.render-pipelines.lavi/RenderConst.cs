using UnityEngine;

namespace Koiyun.Render {
    public static class RenderConst {
        public static int VIEW_PROJ_INV_MTX_ID = Shader.PropertyToID("unity_MatrixInvVP");

        public static int WORLD_TO_SHADOW_MTX_ID = Shader.PropertyToID("_WorldToShadowMatrix");
        public static int MAIN_LIGHT_DIRECTION_ID = Shader.PropertyToID("_LightDirection");
        
        public static int SHADOW_BIAS_ID = Shader.PropertyToID("_ShadowBias");
        public static int SHADOW_STEP_ID = Shader.PropertyToID("_ShadowStep");
        public static int SHADOW_ATTEN_ID = Shader.PropertyToID("_ShadowAttens");

        public static int SCREEN_PARAMS_ID = Shader.PropertyToID("_ScreenParams");
        public static int SCALED_SCREEN_PARAMS_ID = Shader.PropertyToID("_ScaledScreenParams");
        public static int ZBUFFER_PARAMS_ID = Shader.PropertyToID("_ZBufferParams");
        public static int PROJ_PARAMS_ID = Shader.PropertyToID("_ProjectionParams");
        public static int CAMERA_POSWS_ID = Shader.PropertyToID("_WorldSpaceCameraPos");
        public static int SCREEN_TO_WROLD_ID = Shader.PropertyToID("_ScreenToWorld");
        public static int RENDER_SCALE_ID = Shader.PropertyToID("_RenderScale");
        
        public static int AO_VP_MATRIX_ID = Shader.PropertyToID("_AOMatrixVP");
        public static int AO_VIEW_EXTENTX_ID = Shader.PropertyToID("_AOViewExtentX");
        public static int AO_VIEW_EXTENTY_ID = Shader.PropertyToID("_AOViewExtentY");
        public static int AO_VIEW_EXTENTZ_ID = Shader.PropertyToID("_AOViewExtentZ");

        public static int TIME_ID = Shader.PropertyToID("_Time");
        public static int FOG_RANGE_ID = Shader.PropertyToID("_FogRange");
        public static int FOG_COLOR_ID = Shader.PropertyToID("_FogColor");
        public static int LIGHT_INTENSTY_ID = Shader.PropertyToID("_LightIntensty");
        public static int LIGHT_COLOR_ID = Shader.PropertyToID("_LightColor");

        public static int OUTLINE_PARAMS_ID = Shader.PropertyToID("_OutlineParams");

        public static int COPIED_TEXTURE_ID = Shader.PropertyToID("_CopiedTexture");
        public static int BLOOM_BLUR_TEXTURE_ID = Shader.PropertyToID("_BloomBlurTexture");
        public static int BLOOM_BLUR_LOW_TEXTURE_ID = Shader.PropertyToID("_BloomBlurLowTexture");
        public static int BLOOM_BLUR_HIGH_TEXTURE_ID = Shader.PropertyToID("_BloomBlurHighTexture");
        
        public static float SHADOW_BIAS_RADIUS = 2.5f;
        public static int BLOOM_STEP = 5;

        public static string MAIN_LIGHT_SHADOW_KEYWORD = "_MAIN_LIGHT_SHADOWS";
    }
}