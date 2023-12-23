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
        
        public static float SHADOW_BIAS_RADIUS = 2.5f;
        public static int MAX_PIXEL_WIDTH = 1920;
        public static int MAX_PIXEL_HEIGHT = 1080;
        public static float PIXEL_RATE = 0.5f;

        public static string MAIN_LIGHT_SHADOW_KEYWORD = "_MAIN_LIGHT_SHADOWS";
        public static string SOFT_SHADOW_KEYWORD = "_SHADOWS_SOFT";
        public static string POINT_FILTER_KEYWORD = "_POINT_FILTER";
        public static string BLOOM_BLUR_H_TEXTURE_NAME = "_BloomBlurHTexture";
        public static string BLOOM_BLUR_V_TEXTURE_NAME = "_BloomBlurVTexture";
    }
}