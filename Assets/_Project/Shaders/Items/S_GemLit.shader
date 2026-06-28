Shader "SurvivorsLike/GemLit"
{
    Properties
    {
        _BaseColor          ("Base Color",     Color)      = (1, 1, 1, 1)
        _Metallic           ("Metallic",       Range(0,1)) = 0.3
        _Smoothness         ("Smoothness",     Range(0,1)) = 0.95
        [HDR] _EmissionColor("Emission Color", Color)      = (0, 0, 0, 1)
        [HDR] _RimColor     ("Rim Glow Color", Color)      = (1, 1, 1, 1)
        _RimPower           ("Rim Power",      Range(1,6)) = 2.5
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Geometry"
        }

        // ─── ForwardLit ───────────────────────────────────────────────────────
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float  _Metallic;
                float  _Smoothness;
                float4 _EmissionColor;
                float4 _RimColor;
                float  _RimPower;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float  fogFactor  : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs posInputs  = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs   normInputs = GetVertexNormalInputs(input.normalOS);

                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS   = normInputs.normalWS;
                output.fogFactor  = ComputeFogFactor(posInputs.positionCS.z);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float3 normalWS  = normalize(input.normalWS);
                float3 viewDirWS = normalize(GetWorldSpaceViewDir(input.positionWS));

                // ── PBR ─────────────────────────────────────────────────────
                InputData lightData               = (InputData)0;
                lightData.positionWS              = input.positionWS;
                lightData.normalWS                = normalWS;
                lightData.viewDirectionWS         = viewDirWS;
#if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                lightData.shadowCoord             = TransformWorldToShadowCoord(input.positionWS);
#else
                lightData.shadowCoord             = float4(0, 0, 0, 0);
#endif
                lightData.fogCoord                = input.fogFactor;
                lightData.bakedGI                 = SampleSH(normalWS);
                lightData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

                SurfaceData surfData  = (SurfaceData)0;
                surfData.albedo       = _BaseColor.rgb;
                surfData.metallic     = _Metallic;
                surfData.smoothness   = _Smoothness;
                surfData.occlusion    = 1.0;
                surfData.emission     = _EmissionColor.rgb;
                surfData.alpha        = 1.0;

                half4 color = UniversalFragmentPBR(lightData, surfData);

                // ── Fresnel 림 글로우 ────────────────────────────────────────
                // NdotV가 0에 가까울수록(구체 가장자리) fresnel이 1에 가까워짐
                float NdotV   = saturate(dot(normalWS, viewDirWS));
                float fresnel = pow(1.0 - NdotV, _RimPower);
                color.rgb    += _RimColor.rgb * fresnel;

                color.rgb = MixFog(color.rgb, input.fogFactor);
                return color;
            }
            ENDHLSL
        }

        // ─── ShadowCaster ─────────────────────────────────────────────────────
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex   ShadowVert
            #pragma fragment ShadowFrag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            float3 _LightDirection;

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float  _Metallic;
                float  _Smoothness;
                float4 _EmissionColor;
                float4 _RimColor;
                float  _RimPower;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings ShadowVert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                float3 posWS  = TransformObjectToWorld(input.positionOS.xyz);
                float3 normWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = TransformWorldToHClip(ApplyShadowBias(posWS, normWS, _LightDirection));
                return output;
            }

            half4 ShadowFrag(Varyings input) : SV_Target { return 0; }
            ENDHLSL
        }

        // ─── DepthOnly ────────────────────────────────────────────────────────
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            ZWrite On
            ColorMask R
            Cull Back

            HLSLPROGRAM
            #pragma vertex   DepthVert
            #pragma fragment DepthFrag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float  _Metallic;
                float  _Smoothness;
                float4 _EmissionColor;
                float4 _RimColor;
                float  _RimPower;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings DepthVert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half DepthFrag(Varyings input) : SV_Target { return input.positionCS.z; }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
