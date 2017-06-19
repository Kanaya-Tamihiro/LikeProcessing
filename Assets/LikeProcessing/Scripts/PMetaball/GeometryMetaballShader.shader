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

            float4 latticeWorldPosition;
            int detail;
            float deltaLen;
            float size;

            float4 _Cores[50];
            int _CoreCount;

//            float4 _Points[1000];
//			StructuredBuffer<float4> _Points;

            v2f vert (appdata v)
            {
                v2f o;
//                o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex = float4(v.id, 0, 0, 1);
				//o.vertex = mul(UNITY_MATRIX_MVP, _Points[v.id]);
//				o.vertex = _Points[v.id];
//                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//                o.normal = v.normal;
//                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.color = _Color;
                return o;
            }

            struct Point {
            	float3 loc;
            	float isoValue;
            	float3 normal;
            };

            struct Edge {
            	Point p1;
            	Point p2;
            	float3 intersection;
            	float3 intersectionNormal;
            };

            float culcIsoValue(float3 p, float4 core) {
	            float[] result = new float[4];
				Vector3 position = coreLocalPosition;
				float sqrMagnitude = (p.loc - position).sqrMagnitude;
				result[0] = Mathf.Max(1.0f - (sqrMagnitude / colliderRadiusSqrt), 0);
	//			Debug.Log (result[0]);
				Vector3 normal = Vector3.zero;
				if (result[0] != 0) {
					normal = (2.0f * 1.0f / colliderRadiusSqrt) * (p.loc - position);	
				}
				result [1] = normal.x;
				result [2] = normal.y;
				result [3] = normal.z;
            	return 1;
            }
 
            [maxvertexcount(6)]
            void geom(point v2f input[1], inout TriangleStream<v2f> OutputStream)
            {
            	int z = input[0].vertex / (detail * detail);
            	int y = (input[0].vertex % (detail * detail)) / detail;
            	int x = (input[0].vertex % (detail * detail)) % detail;

            	float3 leftDown = float3(
            		-size + x*deltaLen + latticeWorldPosition.x,
            		-size + y*deltaLen + latticeWorldPosition.y,
            		-size + z*deltaLen + latticeWorldPosition.z);

            	Point points[8];
            	points[0].loc = leftDown;
            	points[1].loc = leftDown + float3(deltaLen, 0, 0);
            	points[2].loc = leftDown + float3(deltaLen, 0, deltaLen);
            	points[3].loc = leftDown + float3(0, 0, deltaLen);
            	points[4].loc = leftDown + float3(0, deltaLen, 0);
            	points[5].loc = leftDown + float3(deltaLen, deltaLen, 0);
            	points[6].loc = leftDown + float3(deltaLen, deltaLen, deltaLen);
            	points[7].loc = leftDown + float3(0, deltaLen, deltaLen);

            	Edge edges[12];
            	edges[0].p1 = points[0]; edges[0].p2 = points[1];
            	edges[1].p1 = points[1]; edges[1].p2 = points[2];
            	edges[2].p1 = points[3]; edges[2].p2 = points[2];
            	edges[3].p1 = points[0]; edges[3].p2 = points[3];
            	edges[4].p1 = points[4]; edges[4].p2 = points[5];
            	edges[5].p1 = points[5]; edges[5].p2 = points[6];
            	edges[6].p1 = points[7]; edges[6].p2 = points[6];
            	edges[7].p1 = points[4]; edges[7].p2 = points[7];
            	edges[8].p1 = points[0]; edges[8].p2 = points[4];
            	edges[9].p1 = points[1]; edges[8].p2 = points[5];
            	edges[10].p1 = points[2]; edges[10].p2 = points[6];
            	edges[11].p1 = points[3]; edges[11].p2 = points[7];

            	for (int i=0; i<8; i++) {
            		Point p = points[i];
            		for (int j=0; j<_CoreCount; j++) {
            			p.isoValue += culcIsoValue(p.loc, _Cores[j]);
            		}
            	}
            	
            	v2f test = (v2f)0;
            	test.color = input[0].color;
            	test.vertex = UnityObjectToClipPos(leftDown);
            	OutputStream.Append (test);
            	test.vertex = UnityObjectToClipPos(leftDown + float4(0, deltaLen, 0, 0));
            	OutputStream.Append (test);
            	test.vertex = UnityObjectToClipPos(leftDown + float4(deltaLen, deltaLen, 0, 0));
            	OutputStream.Append (test);
		      	OutputStream.RestartStrip();
		      	test.vertex = UnityObjectToClipPos(leftDown + float4(deltaLen, deltaLen, 0, 0));
            	OutputStream.Append (test);
            	test.vertex = UnityObjectToClipPos(leftDown + float4(deltaLen, 0, 0, 0));
            	OutputStream.Append (test);
            	test.vertex = UnityObjectToClipPos(leftDown);
            	OutputStream.Append (test);
                // 四角形になるように頂点を生産
//		      	for(int x = 0; x < 2; x++)
//		      	{
//			      	for(int y = 0; y < 2; y++)
//			      	{
//				      	v2f test = (v2f)0;
//		                test.color = input[0].color;
//		                float4 pos = input[0].vertex;
//			      		// 頂点座標を計算し、射影変換
//				      	test.vertex = pos + float4(float2(x, y) * 0.2, 0, 0);
//			          	test.vertex = UnityObjectToClipPos(test.vertex.xyz);
//			          	test.color = input[0].color;
//			          	// ストリームに頂点を追加
//				      	OutputStream.Append (test);
//			      	}
//		      	}
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