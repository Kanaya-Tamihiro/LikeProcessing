using System;
using UnityEngine;
using System.Collections.Generic;

namespace LikeProcessing.PMetaball
{
	public class Cube
	{
		public Point[] points;
		public Edge[] edges;
		public int cubeIndex;
		public float isoLevel;

		public Cube (Point[] _points, Edge[] _edges, float _isoLevel)
		{
			points = _points;
			edges = _edges;
			isoLevel = _isoLevel;
		}

		public void draw (GameObject parent)
		{
			foreach (Point p in points) {
				p.draw (parent);
			}
			foreach (Edge e in edges) {
				e.draw (parent);
			}
		}

		public void CulcIntersections ()
		{
			int edgeFlags = PMetaball.edgeTable [cubeIndex];
			if ((edgeFlags & 1) > 0) {
				edges [0].CulcIntersection ();
			}
			if ((edgeFlags & 2) > 0) {
				edges [1].CulcIntersection ();
			}
			if ((edgeFlags & 4) > 0) {
				edges [2].CulcIntersection ();
			}
			if ((edgeFlags & 0x8) > 0) {
				edges [3].CulcIntersection ();
			}
			if ((edgeFlags & 0x10) > 0) {
				edges [4].CulcIntersection ();
			}
			if ((edgeFlags & 0x20) > 0) {
				edges [5].CulcIntersection ();
			}
			if ((edgeFlags & 0x40) > 0) {
				edges [6].CulcIntersection ();
			}
			if ((edgeFlags & 0x80) > 0) {
				edges [7].CulcIntersection ();
			}
			if ((edgeFlags & 0x100) > 0) {
				edges [8].CulcIntersection ();
			}
			if ((edgeFlags & 0x200) > 0) {
				edges [9].CulcIntersection ();
			}
			if ((edgeFlags & 0x400) > 0) {
				edges [10].CulcIntersection ();
			}
			if ((edgeFlags & 0x800) > 0) {
				edges [11].CulcIntersection ();
			}
		}

		public int Vertices (Vector3[] vertices, Vector3[] normals, int index)
		{
			int i = 0;
			while (PMetaball.triTable [cubeIndex, i] != -1) {
				Edge edge1 = edges [PMetaball.triTable [cubeIndex, i + 2]];
				Edge edge2 = edges [PMetaball.triTable [cubeIndex, i + 1]];
				Edge edge3 = edges [PMetaball.triTable [cubeIndex, i + 0]];
				vertices [index + i] = edge1.intersection;
				vertices [index + i + 1] = edge2.intersection;
				vertices [index + i + 2] = edge3.intersection;
				normals [index + i] = edge1.intersectionNormal;
				normals [index + i + 1] = edge2.intersectionNormal;
				normals [index + i + 2] = edge3.intersectionNormal;
				edge1.hasIntersection = false;
				edge2.hasIntersection = false;
				edge3.hasIntersection = false;
//				triangleIndexList.Add (edges [PMetaball.triTable [cubeIndex, i + 2]].triangleIndex );
//				triangleIndexList.Add (edges [PMetaball.triTable [cubeIndex, i + 1]].triangleIndex );
//				triangleIndexList.Add (edges [PMetaball.triTable [cubeIndex, i + 0]].triangleIndex );
				i += 3;
			}
			return i;
		}

//		public void VerticesHardEdge (List<Vector3> vertexList, List<int> triangleIndexList)
//		{
//			int i = 0;
//			while (PMetaball.triTable [cubeIndex, i] != -1) {
//				vertexList.Add (edges [PMetaball.triTable [cubeIndex, i + 2]].intersection);
//				triangleIndexList.Add (triangleIndexList.Count);
//				vertexList.Add (edges [PMetaball.triTable [cubeIndex, i + 1]].intersection);
//				triangleIndexList.Add (triangleIndexList.Count);
//				vertexList.Add (edges [PMetaball.triTable [cubeIndex, i + 0]].intersection);
//				triangleIndexList.Add (triangleIndexList.Count);
//				i += 3;
//			}
//		}
	}
}

