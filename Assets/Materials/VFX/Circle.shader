Shader "Custom/SectorGradientAlpha" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _StartAngle ("Start Angle", float) = 0.0
        _EndAngle ("End Angle", float) = 90.0
        _Radius ("Radius", float) = 0.5
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _StartAngle;
            float _EndAngle;
            float _Radius;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Convert uv coordinates to range -0.5 to 0.5 for circular pattern
                float2 uvCentered = i.uv - 0.5;
                uvCentered.y *= -1; // Flip Y to match Unity's coordinate system

                // Calculate angle and distance from the center
                float angle = atan2(uvCentered.y, uvCentered.x) * 180 / UNITY_PI;
                if (angle < 0) angle += 360;
                float dist = length(uvCentered);

                // Check if the pixel is within the angle range and radius
                float alpha = (angle >= _StartAngle && angle <= _EndAngle && dist <= _Radius) ? 1.0 : 0.0;
                if (angle >= _StartAngle + 0.5f && angle <= _EndAngle - 0.5f && dist <= _Radius)
                {
                    alpha = pow(dist / _Radius, 5);
                }

                // Set color and alpha
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.a = alpha * 0.5;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
