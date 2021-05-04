Shader "Unlit/PortalShader2" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader {
		Tags {"RenderType" = "Transparent"}
		LOD 100

		Cull Front
		ZTest Always

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
				float3 worldPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 globalPlane;

			v2f vert (appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.pos);
				o.worldPos = v.vertex;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target {
				if (length(globalPlane.xyz) > 0.0) {
					clip(-1);
				}

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
