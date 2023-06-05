Shader "Hidden/Lavi RP/Blit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "" {}
    }
    SubShader
    {
        Pass
        {
            Name "CopyColor"
            
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.texcoord;

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                return color;
            }

            ENDHLSL
        }

        Pass
        {
            Name "CopyDepth"
            
            ZTest Always
            ZWrite On
            Cull Off
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.koiyun.render-pipelines.lavi/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            TEXTURE2D_FLOAT(_MainTex);
            SAMPLER(sampler_MainTex);

            struct Attributes
            {
                uint vertexID : SV_VertexID;
                float4 positionOS : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = GetQuadVertexPosition(input.vertexID);
                output.positionCS.xy = output.positionCS.xy * float2(2.0f, -2.0f) + float2(-1.0f, 1.0f);
                output.uv = GetQuadTexCoord(input.vertexID);

                /*
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.texcoord;
                output.positionCS.y *= SCALE_BIAS.x;
                */

                return output;
            }

            float Frag(Varyings input) : SV_Depth
            {
                // half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float depth = SAMPLE_DEPTH_TEXTURE(_MainTex, sampler_MainTex, input.uv);

                return depth;
            }

            ENDHLSL
        }
    }
}