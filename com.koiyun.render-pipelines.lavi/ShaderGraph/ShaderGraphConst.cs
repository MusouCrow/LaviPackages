namespace Koiyun.Render.ShaderGraph {
    public static class ShaderGraphConst {
        public static string TEMPLATE_PATH = "Packages/com.koiyun.render-pipelines.lavi/ShaderGraph/Template";
        public static string INNER_TEMPLATE_PATH = "Packages/com.unity.shadergraph/Editor/Generation/Templates";
        public static string SHADER_PASS_PATH = TEMPLATE_PATH + "/ShaderPass.template";
        public static string SHADERLIB_CORE = "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl";
        public static string SHADERLIB_FUNCTIONS = "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl";
        public static string SHADERLIB_GBUFFER_PASS = "Packages/com.koiyun.render-pipelines.lavi/ShaderGraph/ShaderLibrary/GBufferPass.hlsl";
        public static string SHADERLIB_TRANSPARENT_PASS = "Packages/com.koiyun.render-pipelines.lavi/ShaderGraph/ShaderLibrary/TransparentPass.hlsl";
        public static string SHADERLIB_SHADOWCASTER_PASS = "Packages/com.koiyun.render-pipelines.lavi/ShaderGraph/ShaderLibrary/ShadowCasterPass.hlsl";

        public static string SRC_BLEND_PROPERTY = "_SrcBlend";
        public static string DST_BLEND_PROPERTY = "_DstBlend";
        public static string ZWRITE_PROPERTY = "_ZWrite";
        public static string ZTEST_PROPERTY = "_ZTest";
        public static string CULL_PROPERTY = "_Cull";

        public static string ADDITIVE_KEYWORD = "_ADDITIVE";
    }
}