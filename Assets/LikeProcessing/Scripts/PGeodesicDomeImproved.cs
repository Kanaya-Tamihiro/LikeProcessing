using UnityEngine;
using System.Collections.Generic;

namespace LikeProcessing
{

	public class PGeodesicDomeImproved
	{
		public GameObject gameObject;
		int detail;
		Vector3[] firstRectVertices = new Vector3[4];
		Vector3[] secondRectVertices = new Vector3[4];
		Vector3[] thirdRectVertices = new Vector3[4];
		Vector3[] icosahedralVertices = new Vector3[12];
		Facet[] facets;
		PVertices pvertices;
		bool isHardEdge = false;
//		Edge[] edges = new Edge[30];

		public PGeodesicDomeImproved (int _detail = 3, bool _isHardEdge = false) {
			detail = Mathf.Max (3, _detail);
			isHardEdge = _isHardEdge;
			gameObject = new GameObject ("PGeodesicDome");
			gameObject.AddComponent<MeshFilter> ();
			gameObject.AddComponent<MeshRenderer> ().material = new Material(Shader.Find("LikeProcessing/VertexColor"));
			SetUp ();
			SetMesh ();
		}

//		class Edge {
//			public Vector3[] points;
//			public int detail;
//
//			public Edge(Vector3 vecA, Vector3 vecB, int _detail) {
//				detail = _detail;
//				Vector3 deltaV = (vecB - vecA) / detail;
//				points = new Vector3[detail + 1];
//				points[0] = vecA;
//				for(int i=1; i<detail; i++) {
//					points[i] = vecA + deltaV * i;
//				}
//				points[detail] = vecB;
//			}
//
//		}

		class Facet {
			public Vector3[] triangleVerteces;

			public Facet (Vector3 vecA, Vector3 vecB, Vector3 vecC, int detail = 3) {
				Vector3[] a2bPoints = PUtil.divideVector3(vecA, vecB, detail);
				Vector3[] a2cPoints = PUtil.divideVector3(vecA, vecC, detail);
				Vector3[] b2cPoints = PUtil.divideVector3(vecB, vecC, detail);
				
				int pointCount = 0;
				for (int i=0; i<=detail; i++) {
					pointCount += (i+1);
				}
				Vector3[] points = new Vector3[pointCount];
				int index = 0;
				// clockwise order vecA, vecB, vecC
				// Assume vecA to C was already normalized
				for(int i=0; i<=detail; i++) {
					Vector3 p, normP;
					p = a2bPoints[i];
					normP = p.normalized * 0.5f;	// mult to sphere sarface
					points[index] = normP;
					index++;
					for(int j=1; j<=i; j++) {
						Vector3 pp, normPP;
						if (i == detail) {
							pp = b2cPoints[j];
						}
						else if (j == i) {
							pp = a2cPoints[j];
						}
						else {
							Vector3 deltaV = (a2cPoints[i] - p) / i;
							pp = p + deltaV * j;
						}
						normPP = pp.normalized * 0.5f;
						points[index] = normPP;
						index++;
					}
				}
				triangleVerteces = new Vector3[detail*detail*3];
				int triangleVertecesIndex = 0;
				int startIndex = 0;
				// first cover upward triangles
				for (int i=0; i<detail; i++) {
					startIndex += i;
					int nextStartIndex = startIndex + i + 1;
					triangleVerteces[triangleVertecesIndex] = points[startIndex];
					triangleVerteces[triangleVertecesIndex+1] = points[nextStartIndex];
					triangleVerteces[triangleVertecesIndex+2] = points[nextStartIndex+1];
					triangleVertecesIndex += 3;
					for (int j=1; j<=i; j++) {
						triangleVerteces[triangleVertecesIndex] = points[startIndex+j];
						triangleVerteces[triangleVertecesIndex+1] = points[nextStartIndex+j];
						triangleVerteces[triangleVertecesIndex+2] = points[nextStartIndex+j+1];
						triangleVertecesIndex += 3;
					}
				}
				// next cover downward triangles
				startIndex = 0;
				for (int i=1; i<detail; i++) {
					startIndex += i;
					int nextStartIndex = startIndex + i + 1 + 1;
					for (int j=0; j<i; j++) {
						triangleVerteces[triangleVertecesIndex] = points[startIndex+j];
						triangleVerteces[triangleVertecesIndex+1] = points[nextStartIndex+j];
						triangleVerteces[triangleVertecesIndex+2] = points[startIndex+j+1];
						triangleVertecesIndex += 3;
					}
				}
			}
		}

		public void SetUp() {
			float root2 = Mathf.Sqrt (2);
			float a = root2;
			float half_a = root2 / 2.0f;
			float b = 1.0f;
			float half_b = b / 2.0f;
			float ratio = 0.5f / (new Vector2 (half_a, half_b)).magnitude;
			half_a *= ratio;
			half_b *= ratio;
			firstRectVertices [0] = new Vector3 (half_a, half_b, 0);
			firstRectVertices [1] = new Vector3 (half_a, -half_b, 0);
			firstRectVertices [2] = new Vector3 (-half_a, -half_b, 0);
			firstRectVertices [3] = new Vector3 (-half_a, half_b, 0);
			Quaternion quatUp = Quaternion.AngleAxis (90, Vector3.up);
			Quaternion quatRight = Quaternion.AngleAxis (90, Vector2.right);
			Quaternion quatRightUp = quatRight * quatUp;
			secondRectVertices [0] = quatRightUp * firstRectVertices [0];
			secondRectVertices [1] = quatRightUp * firstRectVertices [1];
			secondRectVertices [2] = quatRightUp * firstRectVertices [2];
			secondRectVertices [3] = quatRightUp * firstRectVertices [3];
			Quaternion quatUpRight = quatUp * quatRight;
			thirdRectVertices [0] = quatUpRight * firstRectVertices [0];
			thirdRectVertices [1] = quatUpRight * firstRectVertices [1];
			thirdRectVertices [2] = quatUpRight * firstRectVertices [2];
			thirdRectVertices [3] = quatUpRight * firstRectVertices [3];
			firstRectVertices.CopyTo (icosahedralVertices, 0);
			secondRectVertices.CopyTo (icosahedralVertices, 4);
			thirdRectVertices.CopyTo (icosahedralVertices, 8);

			int[] _triangles = {
				1+4, 0+8, 1+8,
				1+4, 0+0, 0+8,
				1+4, 0+4, 0+0,
				1+4, 3+0, 0+4,
				1+4, 1+8, 3+0,

				1+8, 2+4, 2+0,
				0+8, 2+4, 1+8,
				0+8, 1+0, 2+4,
				0+0, 1+0, 0+8,
				0+0, 3+8, 1+0,
				0+4, 3+8, 0+0,
				0+4, 2+8, 3+8,
				3+0, 2+8, 0+4,
				3+0, 2+0, 2+8,
				1+8, 2+0, 3+0,

				3+4, 3+8, 2+8,
				3+4, 2+8, 2+0,
				3+4, 2+0, 2+4,
				3+4, 2+4, 1+0,
				3+4, 1+0, 3+8
			};

			facets = new Facet[_triangles.Length/3];
			for (int i=0; i<facets.Length; i++) {
				facets [i] = new Facet (
					icosahedralVertices[_triangles[i*3]],
					icosahedralVertices[_triangles[i*3+1]],
					icosahedralVertices[_triangles[i*3+2]],
					detail
				);
			}
			pvertices = new PVertices ();
			pvertices.BeginShape (isHardEdge);
			foreach(Facet facet in facets) {
				pvertices.AddVertices (facet.triangleVerteces);
			}
			pvertices.EndShape ();
		}

		public void SetMesh() {
			Mesh mesh = gameObject.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
			mesh.vertices = pvertices.Vertices ();
			mesh.triangles = pvertices.Triangles ();
			mesh.RecalculateNormals ();

//			Color[] colors = new Color[mesh.vertices.Length];
//			for (int i=0; i<colors.Length; i=i+3) {
//				Color color = Color.HSVToRGB (Random.value, 0.5f, 1.0f);
//				colors [i] = color;
//				colors [i+1] = color;
//				colors [i+2] = color;
//			}
//			mesh.colors = colors;
		}

	}
}

