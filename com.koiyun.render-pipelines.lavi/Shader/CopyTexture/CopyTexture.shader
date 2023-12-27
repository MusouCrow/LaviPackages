Shader "Hidden/Lavi RP/CopyTexture"
{
    SubShader
    {
        Pass
        {
            Name "CopyColor"
            
            ZTest Always
            ZWrite On

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
            ColorMask R

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "./CopyDepthPass.hlsl"

            ENDHLSL
        }
    }
}