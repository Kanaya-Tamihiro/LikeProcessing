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

		public int CulcIntersections (List<Vector3> vertexList, int triangleIndex, bool isHardEdge)
		{
			int edgeFlags = PMetaball.edgeTable [cubeIndex];
			if ((edgeFlags & 1) > 0) {
				triangleIndex += edges [0].CulcIntersection (vertexList, triangleIndex, isHardEdge);
			}
			if ((edgeFlags & 2) > 0) {
				triangleIndex += edges [1].CulcIntersection (vertexList, triangleIndex, isHardEdge);
			}
			if ((edgeFlags & 4) > 0) {
				triangleIndex += edges [2].CulcIntersection (vertexList, triangleIndex, isHardEdge);
			}
			if ((edgeFlags & 0x8) > 0) {
				triangleIndex += edges [3].CulcIntersection (vertexList, triangleIndex, isHardEdge);
			}
			if ((edgeFlags & 0x10) > 0) {
				triangleIndex += edges [4].CulcIntersection (vertexList, triangleIndex, isHardEdge);
			}
			if ((edgeFlags & 0x20) > 0) {
				triangleIndex += edges [5].CulcIntersection (vertexList, triangleIndex, isHardEdge);
			}
			if ((edgeFlags & 0x40) > 0) {
				triangleIndex += edges [6].CulcIntersection (vertexList, triangleIndex, isHardEdge);
			}
			if ((edgeFlags & 0x80) > 0) {
				triangleIndex += edges [7].CulcIntersection (vertexList, triangleIndex, isHardEdge);
			}
			if ((edgeFlags & 0x100) > 0) {
				triangleIndex += edges [8].CulcIntersection (vertexList, triangleIndex, isHardEdge);
			}
			if ((edgeFlags & 0x200) > 0) {
				triangleIndex += edges [9].CulcIntersection (vertexList, triangleIndex, isHardEdge);
			}
			if ((edgeFlags & 0x400) > 0) {
				triangleIndex += edges [10].CulcIntersection (vertexList, triangleIndex, isHardEdge);
			}
			if ((edgeFlags & 0x800) > 0) {
				triangleIndex += edges [11].CulcIntersection (vertexList, triangleIndex, isHardEdge);
			}
			return triangleIndex;
		}

		public void Vertices (List<int> triangleIndexList)
		{
			int i = 0;
			while (PMetaball.triTable [cubeIndex, i] != -1) {
				triangleIndexList.Add (edges [PMetaball.triTable [cubeIndex, i + 2]].triangleIndex );
				triangleIndexList.Add (edges [PMetaball.triTable [cubeIndex, i + 1]].triangleIndex );
				triangleIndexList.Add (edges [PMetaball.triTable [cubeIndex, i + 0]].triangleIndex );
				i += 3;
			}
		}

		public void VerticesHardEdge (List<Vector3> vertexList, List<int> triangleIndexList)
		{
			int i = 0;
			while (PMetaball.triTable [cubeIndex, i] != -1) {
				vertexList.Add (edges [PMetaball.triTable [cubeIndex, i + 2]].intersection);
				triangleIndexList.Add (triangleIndexList.Count);
				vertexList.Add (edges [PMetaball.triTable [cubeIndex, i + 1]].intersection);
				triangleIndexList.Add (triangleIndexList.Count);
				vertexList.Add (edges [PMetaball.triTable [cubeIndex, i + 0]].intersection);
				triangleIndexList.Add (triangleIndexList.Count);
				i += 3;
			}
		}
	}
}

