Shader "Hidden/Lavi RP/Blit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "" {}
    }
    
    SubShader
    {
        Pass
        {
            Name "CopyColor"
            
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #pragma shader_feature_local _POINT_FILTER

            #include "./CopyColorPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "CopyDepth"
            
            ZTest Always
            ZWrite On
            Cull Off
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #pragma shader_feature_local _POINT_FILTER

            #include "./CopyDepthPass.hlsl"

            ENDHLSL
        }
    }
}