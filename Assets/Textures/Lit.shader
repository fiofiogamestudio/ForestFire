Shader "FIOFIO/Toon/Lit"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _MainColor("Main Color", Color) = (1, 1, 1, 1)
        _ToneColor("Tone Color", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            Tags 
            {
                "LightMode" = "ForwardBase"
                "PassFlags" = "OnlyDirectional"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv : TEXCOORD0;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 norm : NORMAL;
                float2 uv : TEXCOORD0;
                float3 light : TEXCOORD2;
                // SHADOW_COORDS(3)
            };
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _MainColor;
            fixed4 _ToneColor;
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.norm = UnityObjectToWorldNormal(v.normal);
                o.light = UnityWorldSpaceLightDir(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // TRANSFER_SHADOW(o)
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 norm = normalize(i.norm);
                fixed3 light = normalize(i.light);
                fixed ndl = saturate(dot(norm, light));

                // fixed shadow = SHADOW_ATTENUATION(i);

                float4 col = tex2D(_MainTex, i.uv);
                float phong = saturate(floor(ndl * 3) / (2 - 0.5)) * _LightColor0;

                return (col * _MainColor) * phong + (1 - phong) * _ToneColor;
            }

            ENDCG
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}