Shader "Custom/Terrain"
{
    Properties
    {
        testTexture("Texture", 2D) = "white"{}
        testScale("Scale", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0 

        const static int maxlayersCount = 10;
        const static float epsilon = 1E-4;

        int layersCount;
        float3 baseColours[maxlayersCount];
        float baseStartHeights[maxlayersCount];
        float baseBlends[maxlayersCount];
        float baseColourStrength[maxlayersCount];
        float baseTextureScale[maxlayersCount];

        float minHeight;
        float maxHeight;

        UNITY_DECLARE_TEX2DARRAY(baseTextures);

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        float inverseLerp(float a, float b, float value)
        {
            return saturate((value - a) / (b - a));
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        //���� ��� ���� �ؽ��ĸ� ��� (Tri planar mapping texturing technique)
        float3 triPlanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex)
        {
            float3 scaleWorldPos = worldPos / scale;

            float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaleWorldPos.y, scaleWorldPos.z, textureIndex)) * blendAxes.x;
            float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaleWorldPos.x, scaleWorldPos.z, textureIndex)) * blendAxes.y;
            float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaleWorldPos.x, scaleWorldPos.y, textureIndex)) * blendAxes.z;

            return xProjection + yProjection + zProjection;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
            float3 blendAxes = abs(IN.worldNormal);
            blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

            //o.Albedo = baseColors[0];
            
            for (int i = 0; i < layersCount; i++)
            {
                float drawStrength = inverseLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);

                float3 baseColour = baseColours[i] * baseColourStrength[i];
                float3 textureColour = triPlanar(IN.worldPos, baseTextureScale[i], blendAxes, i) * (1 - baseColourStrength[i]);

                o.Albedo = o.Albedo * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
            }
        }

        ENDCG
    }
    FallBack "Diffuse"
}