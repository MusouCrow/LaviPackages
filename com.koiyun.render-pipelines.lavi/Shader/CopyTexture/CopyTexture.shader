Shader "Hidden/Lavi RP/CopyTexture"
{
    SubShader
    {
        Pass
        {
            Name "CopyDepth"
            
            ZTest Always
            ZWrite On
            Cull Off
            ColorMask R

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "./CopyDepthPass.hlsl"

            ENDHLSL
        }
    }
}