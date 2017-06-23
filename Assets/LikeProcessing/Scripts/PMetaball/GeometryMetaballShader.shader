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

            v2f vert (appdata v)
            {
                v2f o = (v2f)0;
                return o;
            }

            struct Point {
            	float3 loc;
            	float isoValue;
            	float3 normal;
            };

			struct Triangle {
				float3 vertices[3];
			};

            float culcIsoValue(float3 p, float4 core) {
	            //float result = new float[4];
				//Vector3 position = coreLocalPosition;
				//float sqrMagnitude = (p.loc - position).sqrMagnitude;
				float3 _core = float3(core.x, core.y, core.z);
				float3 v = p - _core;
				float sqrMagnitude = dot(v, v);
				float result = max(1.0 - (sqrMagnitude / 1.0), 0);
	//			Debug.Log (result[0]);
//				Vector3 normal = Vector3.zero;
//				if (result[0] != 0) {
//					normal = (2.0f * 1.0f / colliderRadiusSqrt) * (p.loc - position);	
//				}
//				result [1] = normal.x;
//				result [2] = normal.y;
//				result [3] = normal.z;
//            	return 1;
				return result;
            }

            bool lessThan (float3 left, float3 right)
			{
				if (left.x < right.x)
					return true;
				else if (left.x > right.x)
					return false;

				if (left.y < right.y)
					return true;
				else if (left.y > right.y)
					return false;

				if (left.z < right.z)
					return true;
				else if (left.z > right.z)
					return false;

				return false;
			}

            float3 LinearInterpolation (float isoLevel, Point p1, Point p2)
			{
				if (lessThan (p2.loc, p1.loc)) {
					Point temp = (Point)0;
					temp = p1;
					p1 = p2;
					p2 = temp;    
				}
				float3 intersection;
				if (abs (p1.isoValue - p2.isoValue) > 0.00001) {
					intersection = p1.loc + ((p2.loc - p1.loc) / (p2.isoValue - p1.isoValue)) * (isoLevel - p1.isoValue);
					//intersectionNormal = p1.normal + (p2.normal - p1.normal) / (p2.isoValue - p1.isoValue) * (isoLevel - p1.isoValue);
				} else {
					intersection = p1.loc;
					//intersectionNormal = p1.normal;
				}
				return intersection;
			}


 
            [maxvertexcount(12)]
            void geom(point v2f input[1], uint primitiveId : SV_PrimitiveID, inout TriangleStream<v2f> OutputStream)
            {
				int cubeIndex = primitiveId & 0xffff;
            	int z = cubeIndex / (detail * detail);
            	int y = (cubeIndex % (detail * detail)) / detail;
            	int x = (cubeIndex % (detail * detail)) % detail;

            	float3 leftDown = float3(
            		-size + x*deltaLen + latticeWorldPosition.x,
            		-size + y*deltaLen + latticeWorldPosition.y,
            		-size + z*deltaLen + latticeWorldPosition.z);

            	Point cubePoints[8] = {(Point)0, (Point)0, (Point)0, (Point)0, (Point)0, (Point)0, (Point)0, (Point)0};

            	cubePoints[0].loc = leftDown + float3(0, 0, deltaLen);	//left front down
            	cubePoints[1].loc = leftDown + float3(deltaLen, 0, deltaLen);	//right front down
            	cubePoints[2].loc = leftDown + float3(deltaLen, 0, 0); //right back down
            	cubePoints[3].loc = leftDown;	//left back down
            	cubePoints[4].loc = leftDown + float3(0, deltaLen, deltaLen);	//left front up
            	cubePoints[5].loc = leftDown + float3(deltaLen, deltaLen, deltaLen);	//right front up
            	cubePoints[6].loc = leftDown + float3(deltaLen, deltaLen, 0);	//right back up
            	cubePoints[7].loc = leftDown + float3(0, deltaLen, 0);	//left back up

            	int pointTable[6][4] = {
            		{0, 2, 3, 7},
            		{0, 2, 6, 7},
            		{0, 4, 6, 7},
            		{0, 6, 1, 2},
            		{0, 6, 1, 4},
            		{5, 6, 1, 4}
            	};

            	for (int i=0; i<8; i++) {
            		Point p = cubePoints[i];
            		for (int j=0; j<_CoreCount; j++) {
            			p.isoValue += culcIsoValue(p.loc, _Cores[j]);
            		}
            		cubePoints[i] = p;
            	}

//            	float len = deltaLen / 1.0;
//				v2f test = (v2f)0;
//            	test.vertex = UnityObjectToClipPos(leftDown);
//            	OutputStream.Append (test);
//            	test.vertex = UnityObjectToClipPos(leftDown + float4(0, len, 0, 0));
//            	OutputStream.Append (test);
//            	test.vertex = UnityObjectToClipPos(leftDown + float4(len, len, 0, 0));
//            	OutputStream.Append (test);
//		      	OutputStream.RestartStrip();
//		      	test.vertex = UnityObjectToClipPos(leftDown + float4(len, len, 0, 0));
//            	OutputStream.Append (test);
//            	test.vertex = UnityObjectToClipPos(leftDown + float4(len, 0, 0, 0));
//            	OutputStream.Append (test);
//            	test.vertex = UnityObjectToClipPos(leftDown);
//            	OutputStream.Append (test);
//            	OutputStream.RestartStrip();

            	for (int tetraIndex=0; tetraIndex<6; tetraIndex++) {
	            	Point points[4] = {(Point)0, (Point)0, (Point)0, (Point)0};
	            	for (int j=0; j<4; j++) {
	            		points[j] = cubePoints[pointTable[tetraIndex][j]];
	            	}

          			float isoLevel = 0.15;
	            	int triIndex = 0;
					if (points[0].isoValue > isoLevel)
						triIndex |= 1;
					if (points [1].isoValue > isoLevel)
						triIndex |= 2;
					if (points [2].isoValue > isoLevel)
						triIndex |= 4;
					if (points [3].isoValue > isoLevel)
						triIndex |= 8;

					Triangle triangles[2] = {(Triangle)0, (Triangle)0};
					int nTriangle = 0;
					switch (triIndex) {
					   case 0x00:
					   case 0x0F:
					      break;
					   case 0x0E:
					   case 0x01:
					      triangles[0].vertices[0] = LinearInterpolation(isoLevel, points[0], points[1]);
					      triangles[0].vertices[1] = LinearInterpolation(isoLevel, points[0], points[2]);
					      triangles[0].vertices[2] = LinearInterpolation(isoLevel, points[0], points[3]);
					      nTriangle++;
					      break;
					   case 0x0D:
					   case 0x02:
					      triangles[0].vertices[0] = LinearInterpolation(isoLevel, points[1], points[0]);
					      triangles[0].vertices[1] = LinearInterpolation(isoLevel, points[1], points[3]);
					      triangles[0].vertices[2] = LinearInterpolation(isoLevel, points[1], points[2]);
					      nTriangle++;
					      break;
					   case 0x0C:
					   case 0x03:
					      triangles[0].vertices[0] = LinearInterpolation(isoLevel, points[0], points[3]);
					      triangles[0].vertices[1] = LinearInterpolation(isoLevel, points[0], points[2]);
					      triangles[0].vertices[2] = LinearInterpolation(isoLevel, points[1], points[3]);
					      nTriangle++;
					      triangles[1].vertices[0] = triangles[0].vertices[2];
					      triangles[1].vertices[1] = LinearInterpolation(isoLevel, points[1], points[2]);
					      triangles[1].vertices[2] = triangles[0].vertices[1];
					      nTriangle++;
					      break;
					   case 0x0B:
					   case 0x04:
					      triangles[0].vertices[0] = LinearInterpolation(isoLevel, points[2], points[0]);
					      triangles[0].vertices[1] = LinearInterpolation(isoLevel, points[2], points[1]);
					      triangles[0].vertices[2] = LinearInterpolation(isoLevel, points[2], points[3]);
					      nTriangle++;
					      break;
					   case 0x0A:
					   case 0x05:
					      triangles[0].vertices[0] = LinearInterpolation(isoLevel, points[0], points[1]);
					      triangles[0].vertices[1] = LinearInterpolation(isoLevel, points[2], points[3]);
					      triangles[0].vertices[2] = LinearInterpolation(isoLevel, points[0], points[3]);
					      nTriangle++;
					      triangles[1].vertices[0] = triangles[0].vertices[0];
					      triangles[1].vertices[1] = LinearInterpolation(isoLevel, points[1], points[2]);
					      triangles[1].vertices[2] = triangles[0].vertices[1];
					      nTriangle++;
					      break;
					   case 0x09:
					   case 0x06:
					      triangles[0].vertices[0] = LinearInterpolation(isoLevel, points[0], points[1]);
					      triangles[0].vertices[1] = LinearInterpolation(isoLevel, points[1], points[3]);
					      triangles[0].vertices[2] = LinearInterpolation(isoLevel, points[2], points[3]);
					      nTriangle++;
					      triangles[1].vertices[0] = triangles[0].vertices[0];
					      triangles[1].vertices[1] = LinearInterpolation(isoLevel, points[0], points[2]);
					      triangles[1].vertices[2] = triangles[0].vertices[2];
					      nTriangle++;
					      break;
					   case 0x07:
					   case 0x08:
					      triangles[0].vertices[0] = LinearInterpolation(isoLevel, points[3], points[0]);
					      triangles[0].vertices[1] = LinearInterpolation(isoLevel, points[3], points[2]);
					      triangles[0].vertices[2] = LinearInterpolation(isoLevel, points[3], points[1]);
					      nTriangle++;
					      break;
					}

				    v2f test = (v2f)0;
				    float c = triIndex / 15.0;
				    test.color = float4(c,c,c,c);
//		               for (int i=0; i<nTriangle; i++) {
//		            		Triangle tri = triangles[i];
//		            		test.vertex = UnityObjectToClipPos(float4(0,1,tetraIndex,1));
//		            		OutputStream.Append (test);
//		            		test.vertex = UnityObjectToClipPos(float4(1,-1,tetraIndex,1));
//		            		OutputStream.Append (test);
//		            		test.vertex = UnityObjectToClipPos(float4(-1,0,tetraIndex,1));
//		            		OutputStream.Append (test);
//		            		OutputStream.RestartStrip();
//		               }
//					for (int i=0; i<nTriangle; i++) {
//						Triangle tri = triangles[i];
//						test.vertex = UnityObjectToClipPos(float4(0,1,tetraIndex,1));
//						OutputStream.Append (test);
//						test.vertex = UnityObjectToClipPos(float4(1,-1,tetraIndex,1));
//						OutputStream.Append (test);
//						test.vertex = UnityObjectToClipPos(float4(-1,0,tetraIndex,1));
//						OutputStream.Append (test);
//						OutputStream.RestartStrip();
//					}
					for (int i=0; i<nTriangle; i++) {
	            		Triangle tri = triangles[i];
	            		test.vertex = UnityObjectToClipPos(tri.vertices[2]);
	            		OutputStream.Append (test);
	            		test.vertex = UnityObjectToClipPos(tri.vertices[1]);
	            		OutputStream.Append (test);
	            		test.vertex = UnityObjectToClipPos(tri.vertices[0]);
	            		OutputStream.Append (test);
	            		OutputStream.RestartStrip();
            		}
//					c = float4(1,1,1,1);
//					test.color = c;
//					for (int i=0; i<1; i++) {
//	            		Triangle tri = triangles[i];
//	            		test.vertex = UnityObjectToClipPos(cubePoints[7].loc);
//	            		OutputStream.Append (test);
//	            		test.vertex = UnityObjectToClipPos(cubePoints[6].loc);
//	            		OutputStream.Append (test);
//	            		test.vertex = UnityObjectToClipPos(cubePoints[3].loc);
//	            		OutputStream.Append (test);
//	            		OutputStream.RestartStrip();

//	            		Triangle tri = triangles[i];
//	            		test.vertex = UnityObjectToClipPos(points[0].loc);
//	            		OutputStream.Append (test);
//	            		test.vertex = UnityObjectToClipPos(points[1].loc);
//	            		OutputStream.Append (test);
//	            		test.vertex = UnityObjectToClipPos(points[2].loc);
//	            		OutputStream.Append (test);
//	            		OutputStream.RestartStrip();

//	            		c = float4(0.5,0.5,0.5,1);
//	            		test.color = c;
//	            		test.vertex = UnityObjectToClipPos(cubePoints[0].loc);
//	            		OutputStream.Append (test);
//	            		test.vertex = UnityObjectToClipPos(cubePoints[5].loc);
//	            		OutputStream.Append (test);
//	            		test.vertex = UnityObjectToClipPos(cubePoints[1].loc);
//	            		OutputStream.Append (test);
//	            		OutputStream.RestartStrip();
//            		}
				}

            	//				    v2f hoge = (v2f)0;
//				    hoge.color = float4(1,1,1,1);
//	                if (tetraIndex == 0) {
//	                	hoge.color = float4(0,0,0,0);
//	                	hoge.vertex = UnityObjectToClipPos(points[0].loc);
//	            		OutputStream.Append (hoge);
//	            		hoge.color = float4(1,1,1,1);
//	            		hoge.vertex = UnityObjectToClipPos(points[1].loc);
//	            		OutputStream.Append (hoge);
//	            		hoge.color = float4(1,1,1,1);
//	            		hoge.vertex = UnityObjectToClipPos(points[2].loc);
//	            		OutputStream.Append (hoge);
//	            		OutputStream.RestartStrip();
//
//	            		hoge.color = float4(0,0,0,0);
//	            		hoge.vertex = UnityObjectToClipPos(points[0].loc);
//	            		OutputStream.Append (hoge);
//	            		hoge.color = float4(1,1,1,1);
//	            		hoge.vertex = UnityObjectToClipPos(points[2].loc);
//	            		OutputStream.Append (hoge);
//	            		hoge.color = float4(1,1,1,1);
//	            		hoge.vertex = UnityObjectToClipPos(points[3].loc);
//	            		OutputStream.Append (hoge);
//	            		OutputStream.RestartStrip();
//
//	            		hoge.color = float4(0,0,0,0);
//	            		hoge.vertex = UnityObjectToClipPos(points[0].loc);
//	            		OutputStream.Append (hoge);
//	            		hoge.color = float4(1,1,1,1);
//	            		hoge.vertex = UnityObjectToClipPos(points[3].loc);
//	            		OutputStream.Append (hoge);
//	            		hoge.color = float4(1,1,1,1);
//	            		hoge.vertex = UnityObjectToClipPos(points[1].loc);
//	            		OutputStream.Append (hoge);
//	            		OutputStream.RestartStrip();
//
//	            		hoge.color = float4(1,1,1,1);
//	            		hoge.vertex = UnityObjectToClipPos(points[1].loc);
//	            		OutputStream.Append (hoge);
//	            		hoge.color = float4(1,1,1,1);
//	            		hoge.vertex = UnityObjectToClipPos(points[2].loc);
//	            		OutputStream.Append (hoge);
//	            		hoge.color = float4(1,1,1,1);
//	            		hoge.vertex = UnityObjectToClipPos(points[3].loc);
//	            		OutputStream.Append (hoge);
//	            		OutputStream.RestartStrip();
//            		}


				
			}
           
            fixed4 frag (v2f i) : SV_Target
            {
//				float4 col = float4(1,1,1,1);
//				return col;
				return i.color;

			}

			ENDCG
        }
    }
}