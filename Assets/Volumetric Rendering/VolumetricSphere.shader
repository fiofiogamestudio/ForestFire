Shader "Unlit/VolumetricSphere"
{
    Properties
    {
        _Sphere ("Sphere", vector) = (0, 0, 0, 0.4)


        _RaySteps ("Ray Steps", float) = 100
        _RayStepSize ("Ray Step Size", float) = 100
        _DensityScale ("Density Scale", float) = 1
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 wpos : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz; // 不要把xyz写在里面
                // o.centre = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0)).xyz;
                return o;
            }

            float4 _Sphere;
            float _RaySteps;
            float _RayStepSize;
            float _DensityScale;

            float raymarch(float3 ro, float3 rd, float step, float step_size)
            {
                float density = 0;
                for (int i = 0; i < step; i++)
                {
                    ro += rd * step_size;
                    // get sphere density
                    float dist = distance(ro, _Sphere.xyz);
                    if (dist < _Sphere.w)
                    {
                        density += 0.1 * _DensityScale;
                        // density += 0.1 * (_Sphere.w - dist);
                    }

                }
                return exp(-density);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 wpos = i.wpos;
                half3 col = half3(1, 1, 1);
                
                half3 view = normalize(_WorldSpaceCameraPos.xyz - wpos);
                col = raymarch(wpos, -view, _RaySteps, _RayStepSize);
                // col = wpos;
                
                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }
}
