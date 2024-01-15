Shader "Custom/Terrain"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _LightColor ("Light Color", Color) = (1,1,1,1)

        _Heightmap ("Heightmap", 2D) = "white" {}
        _MossTex ("Moss (Ground)", 2D) = "white" {}
        _DirtTex ("Dirt (Ground)", 2D) = "white" {}
        _RockTex ("Rock (Slope)", 2D) = "white" {}
        _SnowTex ("Snow (Peak)", 2D) = "white" {}

        _GrassColor ("Grass Color", Color) = (1,1,1,1)
        _DirtColor ("Dirt Color", Color) = (1,1,1,1)
        _RockColor ("Rock Color", Color) = (1,1,1,1)
        _SnowColor ("Snow Color", Color) = (1,1,1,1)

        _GlobalScale ("Global Scale", Range(1, 1024)) = 64

        // _SnowThreshold("Snow Threshold", Range(0, 1)) = 0.7
        _RockThreshold("Rock Threshold", Range(0, 1)) = 0.3
        // _SnowBand("Snow Band", Range(0, 1)) = 0.3
        _RockSlope("Rock Slope", Range(0, 1)) = 0.3 

        _RawFuelsMap ("RawFuelsMap", 2D) = "white" {}
        _FuelsMap ("FuelsMap", 2D) = "white" {}
        _FiresMap ("FiresMap", 2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float4 _LightColor;
            float4 _SpecularColor;
            float _SpecularIntensity;
            float _Shininess;

            sampler2D _Heightmap;
            sampler2D _MossTex;
            sampler2D _DirtTex;
            sampler2D _RockTex;
            sampler2D _SnowTex;

            float4 _GrassColor;
            float4 _DirtColor;
            float4 _RockColor;
            float4 _SnowColor;

            float _GlobalScale;

            float _SnowThreshold;

            float _RockThreshold;
            float _RockSlope;
            
            sampler2D _RawFuelsMap;
            sampler2D _FuelsMap;
            sampler2D _FiresMap;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                 // Get the height from the heightmap texture
                float height = tex2D(_Heightmap, i.uv.xy).r;

                float ground = 1.0 - height;
                float slope = 1 - i.normal.y; 



                // float snowWeight = smoothstep(_SnowThreshold - _SnowBand, _SnowThreshold, height);
                // return height;

                // Get the slope from the world normal

                // Calculate weights for each texture using smoothstep function
                float slope1 = 1.0 - smoothstep(0.0, _RockSlope, slope);
                float slope2 = smoothstep(0.0, _RockSlope, slope) - smoothstep(_RockSlope, 1.0, slope);
                float slope3 = smoothstep(_RockSlope, 1.0, slope);

                float height1 = 1.0 - smoothstep(0.0, _RockThreshold, height);
                float height2 = smoothstep(0.0, _RockThreshold, height) - smoothstep(_RockThreshold, 1.0, height);
                float height3 = smoothstep(_RockThreshold, 1.0, height);



                float weight1 = slope1 * height1;
                float weight2 = slope2 * height2;
                float weight3 = slope3 * height3;
                float total = weight1 + weight2 + weight3;
                weight1 = weight1 / total;
                weight2 = weight2 / total;
                weight3 = weight3 / total;


                fixed4 tex = weight1 * _GrassColor + weight2 * _DirtColor + weight3 * _RockColor;
                
                // float totalWeight = snowWeight * height + rockWeight * height + dirtWeight * ground + grassWeight * ground;
                // snowWeight = snowWeight * height / totalWeight;
                // rockWeight = rockWeight * height / totalWeight;
                // dirtWeight = dirtWeight * ground / totalWeight;
                // grassWeight = grassWeight * ground / totalWeight;

                // Sample each texture
                // fixed4 snow = tex2D(_SnowTex, i.uv * _GlobalScale) * _SnowColor * snowWeight;
                // fixed4 grass = tex2D(_MossTex, i.uv * _GlobalScale) * _GrassColor * weight1 * slope1;
                // fixed4 dirt = tex2D(_DirtTex, i.uv * _GlobalScale) * _DirtColor * weight2 * slope2;
                // fixed4 rock = tex2D(_RockTex, i.uv * _GlobalScale) * _RockColor * weight3 * slope3;

                // fixed4 tex = snow + rock + dirt + moss;
                // fixed4 tex = rock + dirt + grass;

                // return tex;
                
                // return dir.xxxx;
                
                // return snow.xxxx;
                // return (dirt * (1 - height)).xxxx;
                // fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
                // fixed4 tex = fixed4(1,1,1,1);
                // fixed4 tex = finalColor * _Color;

                float3 normal = normalize(i.normal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

                // Ambient
                fixed4 ambient = tex * 0.5;

                // Diffuse
                float dotNL = max(dot(normal, lightDir), 0.0);
                fixed4 diffuse = dotNL * _LightColor * tex;

                // fuel
                float rawFuel = tex2D(_RawFuelsMap, i.uv).r;
                float fuel = tex2D(_FuelsMap, i.uv).r / rawFuel;
                if (fuel > 1.0) {fuel = 1.0;}
                if (rawFuel < 0.2) {fuel = 1.0;}

                fixed4 nativeColor = (diffuse + ambient) * _Color;
                
                fixed4 burnedColor = fixed4(0,0,0,1);

                fixed4 noFireColor = lerp(burnedColor, nativeColor, fuel);

                float fire = tex2D(_FiresMap, i.uv).r;

                return noFireColor + float4(fire * 2, fire * 0.5, 0, 1);
            }
            ENDCG
        }
    }
}
