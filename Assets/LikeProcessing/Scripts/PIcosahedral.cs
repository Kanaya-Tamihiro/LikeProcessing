using UnityEngine;
using System.Collections.Generic;

namespace LikeProcessing
{

	public class PIcosahedral
	{
		public GameObject gameObject;
		Vector3[] firstRectVertices = new Vector3[4];
		Vector3[] secondRectVertices = new Vector3[4];
		Vector3[] thirdRectVertices = new Vector3[4];
		Vector3[] vertices = new Vector3[12];
		Vector3[] verticesHardEdge = new Vector3[60];
		int[] triangles;
		int[] trianglesHardEdge;

		public PIcosahedral () {
			gameObject = new GameObject ("PIcosahedral");
			gameObject.AddComponent<MeshFilter> ();
			gameObject.AddComponent<MeshRenderer> ().material = new Material(Shader.Find("LikeProcessing/VertexColor"));
			SetUp ();
			SetMeshHardEdge ();
		}

		void SetUp() {
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

			firstRectVertices.CopyTo (vertices, 0);
			secondRectVertices.CopyTo (vertices, 4);
			thirdRectVertices.CopyTo (vertices, 8);

			int[] _triangles = {
				//				0,1,2,2,3,0, 4,5,6,6,7,4, 8,9,10,10,11,8,
				1+4, 0+8, 1+8,
				1+4, 1+8, 3+0,
				1+4, 3+0, 0+4,
				1+4, 0+4, 0+0,
				1+4, 0+0, 0+8,

				0+8, 2+4, 1+8,
				0+8, 1+0, 2+4,
				0+0, 1+0, 0+8,
				0+0, 3+8, 1+0,
				0+4, 3+8, 0+0,
				0+4, 2+8, 3+8,
				3+0, 2+8, 0+4,
				3+0, 2+0, 2+8,
				1+8, 2+0, 3+0,
				1+8, 2+4, 2+0,

				3+4, 3+8, 2+8,
				3+4, 2+8, 2+0,
				3+4, 2+0, 2+4,
				3+4, 2+4, 1+0,
				3+4, 1+0, 3+8
			};
			triangles = _triangles;

			trianglesHardEdge = new int[60];
			for (int i=0; i<triangles.Length; i++) {
				int triangleIndex = triangles[i];
				Vector3 vertex = vertices [triangleIndex];
				verticesHardEdge [i] = vertex;
				trianglesHardEdge [i] = i;
			}
		}

		void SetMesh() {
			Mesh mesh = gameObject.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.RecalculateNormals ();

			Color[] colors = new Color[12];
			for (int i=0; i<12; i++) {
				colors [i] = Color.HSVToRGB (Random.value, 1.0f, 1.0f);
			}
			mesh.colors = colors;
		}

		void SetMeshHardEdge() {
			Mesh mesh = gameObject.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
			mesh.vertices = verticesHardEdge;

			mesh.triangles = trianglesHardEdge;
			mesh.RecalculateNormals ();

			Color[] colors = new Color[60];
			for (int i=0; i<60; i=i+3) {
				Color color = Color.HSVToRGB (Random.value, 1.0f, 1.0f);
				colors [i] = color;
				colors [i+1] = color;
				colors [i+2] = color;
			}
			mesh.colors = colors;
		}

	}
}

