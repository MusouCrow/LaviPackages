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

            #include "./Pass.hlsl"

            ENDHLSL
        }
    }
}