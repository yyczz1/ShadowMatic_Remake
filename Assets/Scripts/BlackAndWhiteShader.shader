Shader "Custom/BlackAndWhiteShader" {
    SubShader{
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                return float4(gray, gray, gray, 1);
            }
            ENDCG
        }
    }
}

    
