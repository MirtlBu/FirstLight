Shader "Custom/ProceduralStarfield"
{
    Properties
    {
        [Header(Stars)]
        _StarScale1     ("Layer 1 Scale (density)",  Range(50,  500))  = 150
        _StarScale2     ("Layer 2 Scale (small)",    Range(100, 800))  = 350
        _StarSize       ("Star Size",                Range(0.001, 0.05)) = 0.012
        _StarBrightness ("Star Brightness",          Range(0.5, 5))    = 2.0
        _StarColor      ("Star Tint",                Color)            = (1, 0.95, 0.85, 1)

        [Header(Background)]
        _SkyColor       ("Deep Space Color",         Color)            = (0, 0.01, 0.03, 1)
        _NebulaColor    ("Nebula Tint",              Color)            = (0.05, 0.02, 0.08, 1)
        _NebulaScale    ("Nebula Scale",             Range(1, 10))     = 4
        _NebulaStrength ("Nebula Strength",          Range(0, 1))      = 0.3
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings   { float4 positionCS : SV_POSITION; float3 dir : TEXCOORD0; };

            float  _StarScale1, _StarScale2, _StarSize, _StarBrightness;
            float4 _StarColor, _SkyColor, _NebulaColor;
            float  _NebulaScale, _NebulaStrength;

            // --- hash helpers ---
            float hash1(float3 p)
            {
                p = frac(p * 0.31830 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }
            float3 hash3(float3 p)
            {
                return frac(sin(float3(
                    dot(p, float3(127.1, 311.7,  74.7)),
                    dot(p, float3(269.5, 183.3, 246.1)),
                    dot(p, float3(113.5, 271.9, 124.6))
                )) * 43758.5453);
            }

            // --- star layer ---
            float StarLayer(float3 dir, float scale, float size)
            {
                float3 p      = dir * scale;
                float3 cellID = floor(p);
                float3 local  = frac(p) - 0.5;
                float3 offset = (hash3(cellID) - 0.5) * 0.8;
                float  dist   = length(local - offset);
                float  bright = pow(hash1(cellID + 7.3), 2.5);
                return smoothstep(size, 0.0, dist) * bright;
            }

            // --- smooth noise for nebula ---
            float Noise(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                return lerp(
                    lerp(lerp(hash1(i),             hash1(i + float3(1,0,0)), f.x),
                         lerp(hash1(i+float3(0,1,0)), hash1(i+float3(1,1,0)), f.x), f.y),
                    lerp(lerp(hash1(i+float3(0,0,1)), hash1(i+float3(1,0,1)), f.x),
                         lerp(hash1(i+float3(0,1,1)), hash1(i+float3(1,1,1)), f.x), f.y),
                    f.z);
            }

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.dir        = IN.positionOS.xyz;
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                float3 dir = normalize(IN.dir);

                // Nebula background
                float nebula = Noise(dir * _NebulaScale) * 0.5
                             + Noise(dir * _NebulaScale * 2.1) * 0.3
                             + Noise(dir * _NebulaScale * 4.3) * 0.2;
                nebula = pow(nebula, 2.0) * _NebulaStrength;

                // Stars (two layers: large bright + small dim)
                float s1 = StarLayer(dir, _StarScale1, _StarSize)          * _StarBrightness;
                float s2 = StarLayer(dir, _StarScale2, _StarSize * 0.6)    * _StarBrightness * 0.4;
                float stars = saturate(s1 + s2);

                float3 col = _SkyColor.rgb
                           + _NebulaColor.rgb * nebula
                           + _StarColor.rgb   * stars;

                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }
}
