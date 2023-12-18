Shader "Hidden/Lavi RP/StencilTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "" {}
    }

    SubShader
    {
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off
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