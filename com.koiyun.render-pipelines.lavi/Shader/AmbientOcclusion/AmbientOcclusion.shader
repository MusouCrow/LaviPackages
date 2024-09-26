Shader "Hidden/Lavi RP/AmbientOcclusion"
{
    SubShader
    {
        Pass
        {
            ZTest Always
            ZWrite Off
            Stencil
            {
                Ref 1
                Comp Equal
            }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "./Pass.hlsl"

            ENDHLSL
        }
    }
}