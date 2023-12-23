Shader "Hidden/Lavi RP/Deferred"
{
    SubShader
    {
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

            #include "./Pass.hlsl"

            ENDHLSL
        }
    }
}