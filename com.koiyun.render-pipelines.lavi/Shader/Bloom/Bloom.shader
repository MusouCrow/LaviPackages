Shader "Hidden/Lavi RP/Bloom"
{
    SubShader
    {
        ZTest Always
        ZWrite Off
        
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