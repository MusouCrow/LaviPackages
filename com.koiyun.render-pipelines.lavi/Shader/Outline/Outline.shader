Shader "Hidden/Lavi RP/Outline"
{
    SubShader
    {
        Pass
        {
            ZTest Always
            ZWrite Off
            Stencil
            {
                Ref 2
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