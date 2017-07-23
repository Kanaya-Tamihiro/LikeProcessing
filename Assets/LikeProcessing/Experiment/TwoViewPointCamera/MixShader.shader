Shader "TwoViewPointCamera/MixShader"
{
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" {}
		_SecondMaskDepthTex ("SecondMaskTexture", 2D) = "white" {}
		_SecondColorTex ("SecondColorTexture", 2D) = "white" {}
		_SecondDepthTex ("SecondDepthTexture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _SecondMaskDepthTex;
			sampler2D _SecondColorTex;
			sampler2D _SecondDepthTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mainColor = tex2D(_MainTex, i.uv);
				float secondMaskDepth = Linear01Depth(tex2D(_SecondMaskDepthTex, i.uv));
				fixed4 secondColor = tex2D(_SecondColorTex, i.uv);
				float secondDepth = Linear01Depth(tex2D(_SecondDepthTex, i.uv));
				if (secondDepth < secondMaskDepth) {
					return secondColor;
				} else {
					return mainColor;
				}
			}
			ENDCG
		}
	}
}
