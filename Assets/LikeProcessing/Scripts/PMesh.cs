using System;
using UnityEngine;

namespace LikeProcessing
{
	public class PMesh
	{
		public Vector3[] vertices;
		public int[] triangles;
		public Vector3[] normals;
		public int currentVertexIndex;
		public int currentTriangleIndex;

		public PMesh(int vertexCount, int triangleCount, int normalCount = 0) {
			vertices = new Vector3[vertexCount];
			triangles = new int[triangleCount];
			normals = new Vector3[normalCount];
			currentVertexIndex = 0;
			currentTriangleIndex = 0;
		}

		public void Add(Vector3[] _vertices, int[] _triangles, Vector3[] _normals = null) {
			Array.Copy (_vertices, 0, vertices, currentVertexIndex, _vertices.Length);
//			Array.Copy (_triangles, 0, triangles, currentTriangleIndex, _triangles.Length);
			for (int i=0; i<_triangles.Length; i++) {
				triangles [currentTriangleIndex + i] = currentVertexIndex + _triangles[i];
			}
			if (_normals != null) Array.Copy (_normals, 0, normals, currentVertexIndex, _normals.Length);
			currentVertexIndex += _vertices.Length;
			currentTriangleIndex += _triangles.Length;
		}
	}
}

