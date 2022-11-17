
Shader "Custom/GrassCompute"
{
    Properties
    {
        [Toggle(FADE)] _TransparentBottom("Transparency at Bottom", Float) = 0
        _Fade("Fade Multiplier", Range(1,10)) = 6
        _ShadowReceiveStrength("Shadow Receive Strength", Range(0,1)) = 0.5
    }

    HLSLINCLUDE
    // Include some helper functions
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

    // This describes a vertex on the generated mesh
    struct DrawVertex
    {
        float3 positionWS; // The position in world space
        float2 uv;
        float3 diffuseColor;
    };

    // A triangle on the generated mesh
    struct DrawTriangle
    {
        float3 normalOS;
        DrawVertex vertices[3]; // The three points on the triangle
    };

    // A buffer containing the generated mesh
    StructuredBuffer<DrawTriangle> _DrawTriangles;

    struct v2f
    {
        float4 positionCS : SV_POSITION; // Position in clip space
        float2 uv : TEXCOORD0;          // The height of this vertex on the grass blade
        float3 positionWS : TEXCOORD1; // Position in world space
        float3 normalWS : TEXCOORD2;   // Normal vector in world space
        float3 diffuseColor : COLOR;
        float fogFactor : TEXCOORD5;
    };

    // Properties
    float4 _TopTint;
    float4 _BottomTint;
    float _AmbientStrength;

    float _ShadowReceiveStrength;
    float _Fade;

    // ----------------------------------------

    // Vertex function

    // -- retrieve data generated from compute shader
    v2f vert(uint vertexID : SV_VertexID)
    {
        // Initialize the output struct
        v2f output = (v2f)0;

        // Get the vertex from the buffer
        // Since the buffer is structured in triangles, we need to divide the vertexID by three
        // to get the triangle, and then modulo by 3 to get the vertex on the triangle
        DrawTriangle tri = _DrawTriangles[vertexID / 3];
        DrawVertex input = tri.vertices[vertexID % 3];

        output.positionCS = TransformWorldToHClip(input.positionWS);
        output.positionWS = input.positionWS;
        
        float3 faceNormal = GetMainLight().direction * tri.normalOS;
        output.normalWS = TransformObjectToWorldNormal(faceNormal, true);
        float fogFactor = ComputeFogFactor(output.positionCS.z);
        output.fogFactor = fogFactor;
        output.uv = input.uv;

        output.diffuseColor = input.diffuseColor;

        return output;
    }

    // ----------------------------------------

    // Fragment function

    half4 frag(v2f i) : SV_Target
    {
        // For Shadow Caster Pass
        #ifdef SHADERPASS_SHADOWCASTER
            return 0;
        #else
            // For Color Pass
            
            #if SHADOWS_SCREEN
                // Defines the color variable
                half4 shadowCoord = ComputeScreenPos(i.positionCS);
            #else
                half4 shadowCoord = TransformWorldToShadowCoord(i.positionWS);
            #endif  
            #if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
                Light mainLight = GetMainLight(shadowCoord);
            #else
                Light mainLight = GetMainLight();
            #endif
            float shadow = mainLight.shadowAttenuation;
            shadow = saturate((1-_ShadowReceiveStrength)+ shadow );
            // extra point lights support
            float3 extraLights;
            int pixelLightCount = GetAdditionalLightsCount();
            for (int j = 0; j < pixelLightCount; ++j) {
                Light light = GetAdditionalLight(j, i.positionWS, half4(1, 1, 1, 1));
                float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
                extraLights += attenuatedLightColor;
            }
            float4 baseColor = lerp(_BottomTint, _TopTint, saturate(i.uv.y)) * float4(i.diffuseColor, 1);

            // multiply with lighting color
            float4 litColor = (baseColor * float4(mainLight.color,1));

            litColor += float4(extraLights,1);
            // multiply with vertex color, and shadows
            float4 final = litColor * shadow;
            // add in basecolor when lights turned down
            final += saturate((1 - shadow) * baseColor * 0.2);
            // fog
            float fogFactor = i.fogFactor;

            // Mix the pixel color with fogColor. 
            final.rgb = MixFog(final.rgb, fogFactor);
            // add in ambient color
            final += (unity_AmbientSky * _AmbientStrength);

            // fade to bottom transparency
            #if FADE
                float alpha = lerp(0, 1, saturate(i.uv.y * _Fade));
                final.a = alpha;
            #else
                final.a = 1;
            #endif
            return final;

        #endif  // SHADERPASS_SHADOWCASTER
    }
    ENDHLSL

    SubShader {
        // UniversalPipeline needed to have this render in URP
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }

        // Forward Lit Pass
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Cull Off // No culling since the grass must be double sided
            Blend SrcAlpha OneMinusSrcAlpha //transparency at bottom of grass

            HLSLPROGRAM
            // Signal this shader requires a compute buffer
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 5.0

            // Lighting and shadow keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma shader_feature FADE
            // Register our functions
            #pragma vertex vert
            #pragma fragment frag

            // Include vertex and fragment functions

            ENDHLSL
        }
        
        // Shadow Casting Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual
            Cull Off
            
            HLSLPROGRAM
            // Signal this shader requires geometry function support
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 5.0

            // Support all the various light  ypes and shadow paths
            #pragma multi_compile_shadowcaster

            // Register our functions
            #pragma vertex vert
            #pragma fragment frag

            // A custom keyword to modify logic during the shadow caster pass
            #define SHADERPASS_SHADOWCASTER

            #pragma shader_feature_local _ DISTANCE_DETAIL
            
            // Include vertex and fragment functions
            
            ENDHLSL
        }
    }
}