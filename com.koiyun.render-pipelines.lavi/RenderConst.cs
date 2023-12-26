using UnityEngine;

namespace Koiyun.Render {
    public static class RenderConst {
        public static int WORLD_TO_SHADOW_MTX_ID = Shader.PropertyToID("_WorldToShadowMatrix");
        public static int MAIN_LIGHT_DIRECTION_ID = Shader.PropertyToID("_LightDirection");
        public static int SHADOW_PARAMS_ID = Shader.PropertyToID("_ShadowParams");
        public static int SCALED_SCREEN_PARAMS_ID = Shader.PropertyToID("_ScaledScreenParams");
        public static int ZBUFFER_PARAMS_ID = Shader.PropertyToID("_ZBufferParams");
        public static int PROJ_PARAMS_ID = Shader.PropertyToID("_ProjectionParams");
        public static int CAMERA_POSWS_ID = Shader.PropertyToID("_WorldSpaceCameraPos");
        public static int SCREEN_TO_WROLD_ID = Shader.PropertyToID("_ScreenToWorld");

        public static int BLOOM_BLUR_TEXTURE_ID = Shader.PropertyToID("_BloomBlurTexture");
        public static int BLOOM_BLUR_H_TEXTURE_ID = Shader.PropertyToID("_BloomBlurHTexture");
        public static int BLOOM_BLUR_V_TEXTURE_ID = Shader.PropertyToID("_BloomBlurVTexture");
        public static int COLOR_TABLE_TEXTURE_ID = Shader.PropertyToID("_ColorTableTexture");
        
        public static float SHADOW_BIAS_RADIUS = 2.5f;
        public static int BLOOM_STEP = 5;

        public static string MAIN_LIGHT_SHADOW_KEYWORD = "_MAIN_LIGHT_SHADOWS";
    }
}