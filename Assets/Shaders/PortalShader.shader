Shader "Unlit/PortalShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Offset -1, -1

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
				float4 pos : SV_POSITION;
				float4 screenPos : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.pos);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target {
				float2 uv = i.screenPos.xy / i.screenPos.w;
				uv = TRANSFORM_TEX(uv, _MainTex);

				// sample the texture
				fixed4 col = tex2D(_MainTex, uv);
				return col;
			}
			ENDCG
		}
	}
}
