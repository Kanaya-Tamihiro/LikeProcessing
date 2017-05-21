using UnityEngine;
using System;
using System.Collections.Generic;


namespace LikeProcessing
{
	public class PVertices
	{
		struct IntVector {
			public int x, y, z;
			public Vector3 vector;
			public IntVector(Vector3 v) {
				int precision = 10000;
				x = Mathf.RoundToInt(v.x * precision);
				y = Mathf.RoundToInt(v.y * precision);
				z = Mathf.RoundToInt(v.z * precision);
				float reversePrecision = 1.0f / precision;
				vector = new Vector3(
					x * reversePrecision,
					y * reversePrecision,
					z * reversePrecision
				);
			}
			public override int GetHashCode()
			{
				int hash = 17;
				hash = hash * 23 + x;
				hash = hash * 23 + y;
				hash = hash * 23 + z;
				return hash;
			}
		}

		Dictionary<IntVector, int> vertexDict = new Dictionary<IntVector, int> ();
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
			IntVector intVector = new IntVector (vertex);
			bool isContain = vertexDict.ContainsKey (intVector);
			if (isContain) {
				int index = vertexDict [intVector];
				triangleIndexList.Add (index);
				return index;
			} else {
				int index = vertexDict.Count;
				vertexDict.Add (intVector, index);
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
				foreach (KeyValuePair<IntVector, int> entry in vertexDict)
					vertices [entry.Value] = entry.Key.vector;
			} else {
				vertices = hardEdgeVertexList.ToArray ();
			}
			isHardEdge = false;
		}
	}
}

