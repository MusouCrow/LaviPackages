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

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "./CopyColorPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "CopyDepth"
            
            ZTest Always
            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "./CopyDepthPass.hlsl"

            ENDHLSL
        }
    }
}