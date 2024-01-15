Shader "Unlit/VolumetricTexture"
{
    Properties
    {
        _VolumeTex ("Volume Tex", 3D) = "" {}
        _VolumeScale ("Volume Scale", Range(0, 1)) = 1.0
        _DensityScale ("Density Scale", Range(0, 1)) = 1.0 

        _GValue ("G Value", Range(-1, 1)) = 0.0

        _RaySteps ("Ray Steps", float) = 100
        _RayStepSize ("Ray Step Size", float) = 0.1
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 wpos : TEXCOORD1;
                float3 centre : TEXCOORD2;
                float3 normal : TEXCOORD3;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.centre = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0)).xyz;
                o.normal = UnityObjectToWorldNormal(v.normal);   
                return o;
            }

            sampler3D _VolumeTex;
            float _VolumeScale;

            float _RaySteps;
            float _RayStepSize;
            float _DensityScale;
            float _GValue;

            float phase (float c, float g)
            {
                float g2 = g * g;
                float denom = pow(1 + g2 - 2 * g * c, 1.5);
                return (1 - g2) / denom;
            }

            float2 raymarch(float3 ro, float3 rd, float3 ld, float step, float step_size)
            {
                float density, in_depth, total, alpha;
                step /= _VolumeScale;
                for (int i = 0; i < step; i++)
                {
                    ro += rd * step_size;
                    density = tex3D(_VolumeTex, ro).r * _DensityScale;
                    in_depth += density;
                    // light loop
                    float3 lo = ro;
                    float out_depth, light_accumulation;
                    for (int j = 0; j < step; j++)
                    {
                        lo += ld * step_size;
                        out_depth += tex3D(_VolumeTex, lo).r * _DensityScale;
                    }
                    float transmittance = exp(- (in_depth + out_depth));
                    total += transmittance * density;
                    alpha = 1 - exp(- in_depth);
                }
                float cosa = max(0.0, dot(-rd, ld));
                return float2(total, alpha);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 wpos = i.wpos;
                half3 col = half3(1, 1, 1);
                
                half3 normal = normalize(i.normal);
                half3 light = normalize(_WorldSpaceLightPos0.xyz);
                half3 view = normalize(_WorldSpaceCameraPos.xyz - wpos);
                half ndv = max(0.0, dot(normal, view));
                half2 cloud = raymarch(
                    (wpos - i.centre) / _VolumeScale + float3(0.5, 0.5, 0.5),
                    -view, 
                    light,
                    _RaySteps, 
                    _RayStepSize
                );
                col = cloud.r;
                // col = wpos;
                // clip(ndv - 0.2);
                // return ndv;
                return fixed4(col, cloud.g);
            }
            ENDCG
        }
    }
}
