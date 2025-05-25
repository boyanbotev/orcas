Shader"Unlit/URPOrca"
{

    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        _LightColor ("Light Color", Color) = (1, 1, 1, 1)
        _DarkColor ("Dark Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalRenderPipeline" 
            "IgnoreProjector" = "True"
            "UniversalMaterialType" = "Unlit"
        }
        LOD 100

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            Blend One Zero // Opaque
            ZWrite On
            Cull Back // Default culling

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            CBUFFER_START(UnityPerMaterial)
                float4 _LightColor;
                float4 _DarkColor;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {               
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {             
                Varyings OUT;

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
    
                return OUT;
            }
          
            half4 frag(Varyings IN) : SV_Target
            {
                half4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                half brightness = dot(baseColor.rgb, half3(0.33, 0.33, 0.33));
                half4 finalColor = (brightness > 0.3h) ? _LightColor : _DarkColor;
                finalColor.rgb = lerp(_DarkColor.rgb, _LightColor.rgb, smoothstep(0.3h, 0.7h, brightness));

                return half4(0,1,1,1);
            }

            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}