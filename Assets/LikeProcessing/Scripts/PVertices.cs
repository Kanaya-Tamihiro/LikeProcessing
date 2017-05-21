using UnityEngine;
using System;
using System.Collections.Generic;


namespace LikeProcessing
{
	public class PVertices
	{
		Dictionary<Vector3, int> vertexDict = new Dictionary<Vector3, int> ();
		List<Vector3> hardEdgeVertexList = new List<Vector3> ();
		List<int> triangleIndexList = new List<int>();
		int[] triangleIndecis = new int[0];
		Vector3[] vertices = new Vector3[0];
		bool isShapeEnded = true;
		bool isHardEdge = false;

		public PVertices ()
		{
		}

		public int AddVertex (Vector3 vertex) {
			if (isShapeEnded)
				throw new Exception ("should call AddVertex() after StartShape()");
			if (isHardEdge) {
				hardEdgeVertexList.Add (vertex);
				int index = triangleIndexList.Count;
				triangleIndexList.Add (index);
				return index;
			}
			bool isContain = vertexDict.ContainsKey (vertex);
			if (isContain) {
				int index = vertexDict [vertex];
				triangleIndexList.Add (index);
				return index;
			} else {
				int index = vertexDict.Count;
				vertexDict.Add (vertex, index);
				triangleIndexList.Add (index);
				return index;
			}
		}

		public void AddVertices(Vector3[] vertices) {
			foreach (Vector3 v in vertices)
				AddVertex (v);
		}

		public Vector3[] Vertices() {
			if (!isShapeEnded)
				throw new Exception ("should call Vertices() after EndShape()");
			return vertices;
		}

		public int[] Triangles() {
			if (!isShapeEnded)
				throw new Exception ("should call Triangles() after EndShape()");
			return triangleIndecis;
		}

		public void BeginShape(bool _isHardEdge = false) {
			isShapeEnded = false;
			vertexDict.Clear ();
			triangleIndexList.Clear ();
			hardEdgeVertexList.Clear ();
			isHardEdge = _isHardEdge;
		}

		public void EndShape() {
			isShapeEnded = true;
			triangleIndecis = triangleIndexList.ToArray ();
			if (!isHardEdge) {
				vertices = new Vector3[vertexDict.Count];
				foreach (KeyValuePair<Vector3, int> entry in vertexDict)
					vertices [entry.Value] = entry.Key;
			} else {
				vertices = hardEdgeVertexList.ToArray ();
			}
			isHardEdge = false;
		}
	}
}

