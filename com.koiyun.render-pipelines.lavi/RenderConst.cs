using UnityEngine;

namespace Koiyun.Render {
    public static class RenderConst {
        public static int CAMERA_COLOR_TEXTURE_ID = Shader.PropertyToID("_CameraColorTexture");
        public static int CAMERA_GLOW_TEXTURE_ID = Shader.PropertyToID("_CameraGlowTexture");
        public static int CAMERA_DEPTH_TEXTURE_ID = Shader.PropertyToID("_CameraDepthTexture");
        public static int SHADOW_TEXTURE_ID = Shader.PropertyToID("_ShadowTexture");
        public static int SHADOW_TEXUTRE_SIZE_ID = Shader.PropertyToID("_ShadowTexture_TexelSize");
        public static int WORLD_TO_SHADOW_MTX_ID = Shader.PropertyToID("_WorldToShadowMatrix");
        public static int MAIN_LIGHT_DIRECTION_ID = Shader.PropertyToID("_LightDirection");
        public static int SHADOW_PARAMS_ID = Shader.PropertyToID("_ShadowParams");
        public static int SCALED_SCREEN_PARAMS_ID = Shader.PropertyToID("_ScaledScreenParams");
        public static int ZBUFFER_PARAMS_ID = Shader.PropertyToID("_ZBufferParams");
        public static int PROJ_PARAMS_ID = Shader.PropertyToID("_ProjectionParams");
        public static int POST_TEXTURE_ID = Shader.PropertyToID("_PostTexture");
        public static int BLOOM_BLUR_TEXTURE_ID = Shader.PropertyToID("_BloomBlurTexture");
        public static int PIXEL_TEXTURE_ID = Shader.PropertyToID("_PixelTexture");
        
        public static float SHADOW_BIAS_RADIUS = 2.5f;

        public static string MAIN_LIGHT_SHADOW_KEYWORD = "_MAIN_LIGHT_SHADOWS";
        public static string SOFT_SHADOW_KEYWORD = "_SHADOWS_SOFT";
        public static string POINT_FILTER_KEYWORD = "_POINT_FILTER";
        public static string BLOOM_BLUR_H_TEXTURE_NAME = "_BloomBlurHTexture";
        public static string BLOOM_BLUR_V_TEXTURE_NAME = "_BloomBlurVTexture";
    }
}