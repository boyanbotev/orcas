Shader "Custom/URPOrcaStructured" // Changed path to be more standard
{
    Properties
    {
        // Use _MainTex for the main texture to align with URP Unlit conventions
        // The material inspector will still show "Texture" due to the display name.
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        // _MainTex is often used by TRANSFORM_TEX, so URP shaders usually have _MainTex for the actual texture slot
        // and then _MainTex might be used internally or an alias. For clarity, stick to _MainTex.

        _LightColor ("Light Color", Color) = (1, 1, 1, 1)
        _DarkColor ("Dark Color", Color) = (0, 0, 0, 1)

        // Properties from URP Unlit that might be expected by includes (even if not used by you)
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [ToggleUI] _AlphaClip ("Alpha Clipping", Float) = 0.0
        _Surface("__surface", Float) = 0.0 // Opaque
        _Blend("__mode", Float) = 0.0     // Alpha
        // Add other URP unlit properties if necessary, or ensure they are defaulted in includes
    }

    SubShader
    {
        // Add "UniversalMaterialType" = "Unlit" and "IgnoreProjector"
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalRenderPipeline"
            "UniversalMaterialType" = "Unlit"
            "IgnoreProjector" = "True"
        }
        LOD 100

        Pass
        {
            Name "ForwardUnlit" // Standard URP pass name
            Tags { "LightMode" = "UniversalForward" }

            // Standard URP Unlit blend/ZWrite/Cull states
            // If you need transparency, these would change based on _Surface and _Blend properties
            Blend One Zero // Opaque
            ZWrite On
            Cull Back // Default culling

            HLSLPROGRAM
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            // Define necessary keywords if your logic depends on them or if includes expect them
            // For this shader, probably not needed, but good to be aware of:
            // #pragma shader_feature_local_fragment _ALPHATEST_ON
            // #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT

            // Core URP Includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // Use the URP Unlit input structure. This defines _MainTex, _BaseColor, _MainTex_ST, etc.
            // It also sets up the CBUFFER_START(UnityPerMaterial) block correctly for these properties.
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl" // Contains SurfaceData, InitializeSurfaceData etc.
                                                                                                 // For simple unlit, may not be fully needed, but often present.
                                                                                                 // More directly, UnlitInput.hlsl is what we'd use if directly copying Unlit.shader:
            // For properties like _MainTex, _BaseColor, URP uses a standard CBUFFER structure
            // Let's define our custom properties within the standard URP CBUFFER context.

            // URP standard CBUFFER for Unlit properties.
            // Your _LightColor and _DarkColor will be added here.
            // _MainTex and _MainTex_ST (for tiling/offset) are implicitly handled by TEXTURE2D and TRANSFORM_TEX
            // if the _MainTex property exists.
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST; // Automatically handled by Unity for _MainTex if used with TRANSFORM_TEX
                // float4 _BaseColor; // If you had a _BaseColor property
                float4 _LightColor;
                float4 _DarkColor;
                // float _Cutoff; // If you had a _Cutoff property
                // float _Surface; // If you had surface type properties
                // float _Blend;   // If you had blend mode properties
            CBUFFER_END

            // Texture declaration (matches _MainTex property)
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);


            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // For instancing
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO // For stereo rendering
            };

            Varyings UnlitVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                // Use TRANSFORM_TEX for correct UV handling with tiling/offset from _MainTex_ST
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 UnlitFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 baseMapColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                // Your custom logic
                half brightness = dot(baseMapColor.rgb, half3(0.5, 0.5, 0.5));
                // Use saturate to ensure brightness is [0,1] if smoothstep behaves unexpectedly with out-of-range inputs
                // brightness = saturate(brightness); 
                half4 finalColor = (brightness > 0.3h) ? _LightColor : _DarkColor;
                // Ensure smoothstep inputs are in correct order (edge0 < edge1)
                finalColor.rgb = lerp(_DarkColor.rgb, _LightColor.rgb, smoothstep(0.3h, 0.7h, brightness));

                // Alpha handling (if you were doing transparency)
                // finalColor.a = baseMapColor.a;
                // if (_AlphaClip > 0.5) {
                //     clip(finalColor.a - _Cutoff);
                // }

                return finalColor;
            }
            ENDHLSL
        }

        // Optional: Add other passes if needed (e.g., ShadowCaster, DepthOnly)
        // For a basic opaque unlit shader, the ForwardUnlit pass is often sufficient.
        // Pass
        // {
        //     Name "ShadowCaster"
        //     Tags { "LightMode" = "ShadowCaster" }
        //     // ... ShadowCaster HLSL ...
        // }
    }

    // Crucial: Keep the Fallback that URP expects
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    // Optional: Custom Editor for URP Unlit shaders if you use its features
    // CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.UnlitShaderGUI"
}