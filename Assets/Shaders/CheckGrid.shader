Shader "Custom/CheckGrid"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GridLineColor ("Grid Line Color", Color) = (1,1,1,1)
        _GridLineCount ("Grid Line Count", Range(1, 1024)) = 64
        _GridLineWidth ("Grid Line Width (Percentage)", Range(0.0, 1.0)) = 0.05
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GridLineColor;
            float _GridLineCount;
            float _GridLineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv.xy);
                float2 grid = frac(i.uv * _GridLineCount);

                // Check if the current fragment is near the grid edge
                float edge = step(1 - _GridLineWidth, fmod(grid.x, 1.0)) + step(1 - _GridLineWidth, fmod(grid.y, 1.0));

                // Blend the texture with the grid color based on the edge value
                fixed4 col = lerp(tex, _GridLineColor, edge);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
