Shader "Hidden/Lavi RP/Bloom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "" {}
    }

    SubShader
    {
        ZTest Always
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Pack"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "./PackPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "BlurV"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "./BlurVPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "BlurH"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "./BlurHPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "UpSample"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "./UpSamplePass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "Blend"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "./BlendPass.hlsl"

            ENDHLSL
        }
    }
}