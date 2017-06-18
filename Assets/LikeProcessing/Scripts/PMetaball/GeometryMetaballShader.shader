Shader "LikeProcessing/GeometryMetaballShader"
{
	Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0.1, 0.5, 0.7, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
 
        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
           
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
//                float3 normal : NORMAL;
//                float2 uv : TEXCOORD0;
                uint id : SV_VertexID;
            };
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
//                float3 normal : NORMAL;
//                float2 uv : TEXCOORD0;
//                float3 worldPosition : TEXCOORD1;
                float4 color : COLOR;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            float4 _Points[100];

            v2f vert (appdata v)
            {
                v2f o;
//                o.vertex = UnityObjectToClipPos(v.vertex);
//				o.vertex = float4(v.id, 0, 0, 1);
				o.vertex = mul(UNITY_MATRIX_MVP, _Points[v.id]);
//                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//                o.normal = v.normal;
//                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.color = _Color;
                return o;
            }
 
            [maxvertexcount(4)]
            void geom(point v2f input[1], inout TriangleStream<v2f> OutputStream)
            {
                

                // 四角形になるように頂点を生産
		      	for(int x = 0; x < 2; x++)
		      	{
			      	for(int y = 0; y < 2; y++)
			      	{
				      	v2f test = (v2f)0;
		                test.color = input[0].color;
		                float4 pos = input[0].vertex;
			      		// 頂点座標を計算し、射影変換
				      	test.vertex = pos + float4(float2(x, y) * 0.2, 0, 0);
			          	//test.vertex = UnityObjectToClipPos(test.vertex.xyz);
			          	test.color = input[0].color;
			          	// ストリームに頂点を追加
				      	OutputStream.Append (test);
			      	}
		      	}
//				test.vertex = pos + float4(0.1,0,0,0);
//				test.vertex = mul (UNITY_MATRIX_VP, test.vertex);
//				OutputStream.Append (test);
//				test.vertex = pos + float4(-0.1,0,0,0);
//				test.vertex = mul (UNITY_MATRIX_VP, test.vertex);
//				OutputStream.Append (test);
//				test.vertex = pos + float4(0,-0.1,0,0);
//				test.vertex = mul (UNITY_MATRIX_VP, test.vertex);
//				OutputStream.Append (test);
		      	
		      	// トライアングルストリップを終了
		      	OutputStream.RestartStrip();

//                float3 normal = normalize(cross(input[1].worldPosition.xyz - input[0].worldPosition.xyz, input[2].worldPosition.xyz - input[0].worldPosition.xyz));
//                for(int i = 0; i < 3; i++)
//                {
//                    test.normal = normal;
//                    test.vertex = input[i].vertex;
//                    test.uv = input[i].uv;
//                    OutputStream.Append(test);
//                }
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
//                fixed4 col = tex2D(_MainTex, i.uv);
// 
//                float3 lightDir = float3(1, 1, 0);
//                float ndotl = dot(i.normal, normalize(lightDir));
// 
//                return col * ndotl;
				//return float4(i.vertex.rg * 0.001, 0, 1);
				return i.color;
           }
            ENDCG
        }
    }
}