Shader "Hidden/Lavi RP/AmbientOcclusion"
{
    SubShader
    {
        Pass
        {
            ZTest Always
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "./Pass.hlsl"

            ENDHLSL
        }
    }
}