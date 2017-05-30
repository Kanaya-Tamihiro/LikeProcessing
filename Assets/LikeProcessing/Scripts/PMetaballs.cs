﻿using UnityEngine;
using System.Collections.Generic;

namespace LikeProcessing
{
	public class PMetaball
	{
		
		public GameObject gameObject;
		public float size = 3.0f;
		public int detail = 20;
		public float isoLevel = 0.5f;
		public bool isHardEdge = false;

		Point[,,] points;
		Edge[,,] edgeHorizon, edgeVertical, edgeDepth;
		Cube[,,] cubes;

		public List<Core> cores = new List<Core> ();

		//		Vector3[] vercites = new Vector3[30000];
		//		int[] triangleIndeces = new int[30000];
		List<Vector3> vertexList = new List<Vector3> ();
		List<int> triangleIndexList = new List<int> ();


		public PMetaball ()
		{
			gameObject = new GameObject ();
			gameObject.name = "PMetaballs";
			gameObject.AddComponent<MeshFilter> ().mesh.MarkDynamic ();
			//			gameObject.AddComponent<MeshRenderer> ().material = new Material(Shader.Find("LikeProcessing/VertexColor"));
			gameObject.AddComponent<MeshRenderer> ().material = PSketch.material;

			SetPoint ();
			SetEdge ();
			SetCube ();
//							DrawPoints ();
//							DrawEdges ();
//							DrawCubes ();
		}

		public void AddCore (Core core)
		{
//			core.gameObject.transform.SetParent (gameObject.transform);
			cores.Add (core);
		}

		public void Update ()
		{
			CulcIsoValues ();
			ClearEdges ();
			CulcCubeVertices ();
//			DrawIntersectionPoints ();
			SetMesh ();
		}

		public void SetMesh ()
		{
			Mesh mesh = gameObject.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
			mesh.vertices = vertexList.ToArray ();
			mesh.triangles = triangleIndexList.ToArray ();
			mesh.RecalculateNormals ();
		}

			
		void SetPoint ()
		{
			points = new Point[detail + 1, detail + 1, detail + 1];
			float x = -size;
			float y = -size;
			float z = -size;
			float delta = (size * 2) / detail;
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						Point point = new Point (x, y, z);
						points [iz, iy, ix] = point;
						x += delta;
					}
					x = -size;
					y += delta;
				}
				y = -size;
				z += delta;
			}
		}

		void SetEdge ()
		{
			edgeHorizon = new Edge[detail + 1, detail + 1, detail];
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail; ix++) {
						Point p1 = points [iz, iy, ix];
						Point p2 = points [iz, iy, ix + 1];
						Edge edge = new Edge (p1, p2, isoLevel);
						edgeHorizon [iz, iy, ix] = edge;
					}
				}
			}

			edgeVertical = new Edge[detail + 1, detail, detail + 1];
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						Point p1 = points [iz, iy, ix];
						Point p2 = points [iz, iy + 1, ix];
						Edge edge = new Edge (p1, p2, isoLevel);
						edgeVertical [iz, iy, ix] = edge;
					}
				}
			}

			edgeDepth = new Edge[detail, detail + 1, detail + 1];
			for (int iz = 0; iz < detail; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						Point p1 = points [iz, iy, ix];
						Point p2 = points [iz + 1, iy, ix];
						Edge edge = new Edge (p1, p2, isoLevel);
						edgeDepth [iz, iy, ix] = edge;
					}
				}
			}
		}

		void SetCube ()
		{
			cubes = new Cube[detail, detail, detail];
			for (int iz = 0; iz < detail; iz++) {
				for (int iy = 0; iy < detail; iy++) {
					for (int ix = 0; ix < detail; ix++) {
						Point[] _points = {
							points [iz + 1, iy, ix],	//left back down
							points [iz + 1, iy, ix + 1], //right back down
							points [iz, iy, ix + 1],	//right front down
							points [iz, iy, ix],		//left front down
							points [iz + 1, iy + 1, ix],	//left back up
							points [iz + 1, iy + 1, ix + 1], //right back up
							points [iz, iy + 1, ix + 1],	//right front up
							points [iz, iy + 1, ix],		//left front up
						};
						Edge[] _edges = {
							edgeHorizon [iz + 1, iy, ix], //back down
							edgeDepth [iz, iy, ix + 1],   //right down
							edgeHorizon [iz, iy, ix],   //front down
							edgeDepth [iz, iy, ix],     //left down
							edgeHorizon [iz + 1, iy + 1, ix], //back up
							edgeDepth [iz, iy + 1, ix + 1],   //right up
							edgeHorizon [iz, iy + 1, ix],   //front up
							edgeDepth [iz, iy + 1, ix],     //left up
							edgeVertical [iz + 1, iy, ix],	  //left back side
							edgeVertical [iz + 1, iy, ix + 1], //right back side
							edgeVertical [iz, iy, ix + 1],	  //right front side
							edgeVertical [iz, iy, ix],	  //left front side
						};
						Cube cube = new Cube (_points, _edges, isoLevel);
						cubes [iz, iy, ix] = cube;
					}
				}
			}
		}

		void DrawPoints ()
		{
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						Point p = points [iz, iy, ix];
						p.draw (gameObject);
					}
				}
			}
		}

		void DrawEdges ()
		{
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail; ix++) {
						edgeHorizon [iz, iy, ix].draw (gameObject);
					}
				}
			}
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						edgeVertical [iz, iy, ix].draw (gameObject);
					}
				}
			}
			for (int iz = 0; iz < detail; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						edgeDepth [iz, iy, ix].draw (gameObject);
					}
				}
			}
		}

		public void DrawIntersectionPoints ()
		{
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail; ix++) {
						edgeHorizon [iz, iy, ix].DrawIntersection (gameObject);
					}
				}
			}
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						edgeVertical [iz, iy, ix].DrawIntersection (gameObject);
					}
				}
			}
			for (int iz = 0; iz < detail; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						edgeDepth [iz, iy, ix].DrawIntersection (gameObject);
					}
				}
			}
		}

		void DrawCubes ()
		{
			for (int iz = 0; iz < detail; iz++) {
				if (iz % 2 == 0)
					continue;
				for (int iy = 0; iy < detail; iy++) {
					if (iy % 2 == 0)
						continue;
					for (int ix = 0; ix < detail; ix++) {
						if (ix % 2 == 0)
							continue;
						cubes [iz, iy, ix].draw (gameObject);
					}
				}
			}
		}

		void CulcIsoValues ()
		{
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						Point p = points [iz, iy, ix];
						p.isoValue = 0;
						foreach (Core core in cores) {
							p.isoValue += core.CulcIsoValue (p, isoLevel);
						}
					}
				}
			}
		}

		void ClearEdges () {
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail; ix++) {
						edgeHorizon [iz, iy, ix].hasIntersection = false;
					}
				}
			}
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						edgeVertical [iz, iy, ix].hasIntersection = false;
					}
				}
			}
			for (int iz = 0; iz < detail; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						edgeDepth [iz, iy, ix].hasIntersection = false;
					}
				}
			}
		}

		void CulcCubeVertices ()
		{
			vertexList.Clear ();
			triangleIndexList.Clear ();
			int triangleIndex = 0;
			for (int iz = 0; iz < detail; iz++) {
				for (int iy = 0; iy < detail; iy++) {
					for (int ix = 0; ix < detail; ix++) {
						Cube cube = cubes [iz, iy, ix];
						int cubeIndex = 0;
						if (cube.points [0].isoValue > isoLevel)
							cubeIndex |= 1;
						if (cube.points [1].isoValue > isoLevel)
							cubeIndex |= 2;
						if (cube.points [2].isoValue > isoLevel)
							cubeIndex |= 4;
						if (cube.points [3].isoValue > isoLevel)
							cubeIndex |= 8;
						if (cube.points [4].isoValue > isoLevel)
							cubeIndex |= 16;
						if (cube.points [5].isoValue > isoLevel)
							cubeIndex |= 32;
						if (cube.points [6].isoValue > isoLevel)
							cubeIndex |= 64;
						if (cube.points [7].isoValue > isoLevel)
							cubeIndex |= 128;
						cube.cubeIndex = cubeIndex;
						triangleIndex = cube.CulcIntersections (vertexList, triangleIndex, isHardEdge);
//						if (isHardEdge == true) {
//							cube.VerticesHardEdge (vertexList, triangleIndexList);
//						} else {
						cube.Vertices (triangleIndexList);
//						}
					}
				}
			}
		}

		public void destory ()
		{
			Object.Destroy (this.gameObject);
		}

		public class Point
		{
			public Vector3 loc;
			PGeodesicDomeImproved pGeodesicDome;
			public float isoValue;

			public Point (float x, float y, float z)
			{
				loc = new Vector3 (x, y, z);
			}

			public void draw (GameObject parent)
			{
				if (pGeodesicDome != null)
					return;
				pGeodesicDome = new PGeodesicDomeImproved ();
				pGeodesicDome.gameObject.transform.SetParent (parent.transform);
				pGeodesicDome.gameObject.transform.position = loc;
				pGeodesicDome.gameObject.transform.localScale *= 0.02f;
			}
		}

		public class Edge
		{
			Point[] points = new Point[2];
			PLineSimple pline;
			PGeodesicDomeImproved pGeodesicDome;
			public Vector3 intersection;
			public bool hasIntersection = false;
			public int triangleIndex = -1;
			public float isoLevel;

			public Edge (Point p1, Point p2, float _isoLevel)
			{
				points [0] = p1;
				points [1] = p2;
				isoLevel = _isoLevel;
			}

//			public Vector3 GetIntersection ()
//			{
////				hasIntersection = false;
//				return intersection;
//			}
//
//			public int getTriangleIndex ()
//			{
////				hasIntersection = false;
//				return triangleIndex;
//			}

			public void draw (GameObject parent)
			{
				if (pline != null)
					return;
				pline = new PLineSimple (points [0].loc, points [1].loc, 0.005f, 3);
				pline.gameObject.transform.SetParent (parent.transform);
			}

			public void DrawIntersection (GameObject parent)
			{
//				if (pGeodesicDome == null) {
//					pGeodesicDome = new PGeodesicDomeImproved ();
//					pGeodesicDome.gameObject.transform.SetParent (parent.transform);
//					pGeodesicDome.gameObject.transform.localScale *= 0.02f;	
//				}
//				if (!hasIntersection) {
//					pGeodesicDome.gameObject.SetActive (false);
//					pGeodesicDome.gameObject.transform.localScale = 0;
//				} else {
//					pGeodesicDome.gameObject.transform.position = intersection;
//					pGeodesicDome.gameObject.SetActive (true);
//				}
			}

			public int CulcIntersection (List<Vector3> vertexList, int _triangleIndex, bool isHardEdge)
			{
				if (hasIntersection == true) {
					if (isHardEdge) {
						vertexList.Add (intersection);
						triangleIndex = _triangleIndex;
						return 1;
					} else
						return 0;
				}
				hasIntersection = true;
				intersection = LinearInterpolation ();
				vertexList.Add (intersection);
				triangleIndex = _triangleIndex;
				return 1;
			}

			Vector3 LinearInterpolation ()
			{
				Point p1 = points [0];
				Point p2 = points [1];
				if (lessThan (p2.loc, p1.loc)) {
					Point temp;
					temp = p1;
					p1 = p2;
					p2 = temp;    
				}

				Vector3 result;
				if (Mathf.Abs (p1.isoValue - p2.isoValue) > 0.00001f)
					result = p1.loc + (p2.loc - p1.loc) / (p2.isoValue - p1.isoValue) * (isoLevel - p1.isoValue);
				else
					result = p1.loc;
				return result;
			}

			bool lessThan (Vector3 left, Vector3 right)
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
		}

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
				int edgeFlags = edgeTable [cubeIndex];
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
				while (triTable [cubeIndex, i] != -1) {
					triangleIndexList.Add (edges [triTable [cubeIndex, i + 2]].triangleIndex );
					triangleIndexList.Add (edges [triTable [cubeIndex, i + 1]].triangleIndex );
					triangleIndexList.Add (edges [triTable [cubeIndex, i + 0]].triangleIndex );
					i += 3;
				}
			}

			public void VerticesHardEdge (List<Vector3> vertexList, List<int> triangleIndexList)
			{
				int i = 0;
				while (triTable [cubeIndex, i] != -1) {
					vertexList.Add (edges [triTable [cubeIndex, i + 2]].intersection);
					triangleIndexList.Add (triangleIndexList.Count);
					vertexList.Add (edges [triTable [cubeIndex, i + 1]].intersection);
					triangleIndexList.Add (triangleIndexList.Count);
					vertexList.Add (edges [triTable [cubeIndex, i + 0]].intersection);
					triangleIndexList.Add (triangleIndexList.Count);
					i += 3;
				}
			}
		}

		public class Core {

			Vector3 position;

			public Core () {}

			public Core(Vector3 _position) {
				position = _position;
			}

			virtual public float CulcIsoValue(Point p, float isoLevel) {
				float distance = Vector3.Distance (p.loc, position);
				return 0.7f / (1.0f + distance * distance);	
			}
		}

		public class CoreLine : Core
		{
			Vector3 p_org, p_end;
			Vector3 delta;
			float width;

			public CoreLine (Vector3 from, Vector3 to, float _width) : base()
			{
				p_org = from;
				p_end = to;
				delta = to - from;
				width = _width;
			}

			override public float CulcIsoValue(Point p, float isoLevel) {
				Vector3 q = p.loc - p_org;
				float t = Vector3.Dot(delta, q) / delta.magnitude;
//				Debug.Log (t);
				float distance;
				if (t > 1) {
					distance = (p.loc - p_end).magnitude;
//					distance = 100;
				} else if (t < 0) {
					distance = (p.loc - p_org).magnitude;
//					distance = 100;
				} else {
					distance = (q - (p_org + t * delta)).magnitude;
//					Debug.Log (distance);
//					distance = 100;
				}
				return .7f / (1.0f + distance * distance);	
			}
		}

		public static int[] edgeTable = {
			0x0, 0x109, 0x203, 0x30a, 0x406, 0x50f, 0x605, 0x70c,
			0x80c, 0x905, 0xa0f, 0xb06, 0xc0a, 0xd03, 0xe09, 0xf00,
			0x190, 0x99, 0x393, 0x29a, 0x596, 0x49f, 0x795, 0x69c,
			0x99c, 0x895, 0xb9f, 0xa96, 0xd9a, 0xc93, 0xf99, 0xe90,
			0x230, 0x339, 0x33, 0x13a, 0x636, 0x73f, 0x435, 0x53c,
			0xa3c, 0xb35, 0x83f, 0x936, 0xe3a, 0xf33, 0xc39, 0xd30,
			0x3a0, 0x2a9, 0x1a3, 0xaa, 0x7a6, 0x6af, 0x5a5, 0x4ac,
			0xbac, 0xaa5, 0x9af, 0x8a6, 0xfaa, 0xea3, 0xda9, 0xca0,
			0x460, 0x569, 0x663, 0x76a, 0x66, 0x16f, 0x265, 0x36c,
			0xc6c, 0xd65, 0xe6f, 0xf66, 0x86a, 0x963, 0xa69, 0xb60,
			0x5f0, 0x4f9, 0x7f3, 0x6fa, 0x1f6, 0xff, 0x3f5, 0x2fc,
			0xdfc, 0xcf5, 0xfff, 0xef6, 0x9fa, 0x8f3, 0xbf9, 0xaf0,
			0x650, 0x759, 0x453, 0x55a, 0x256, 0x35f, 0x55, 0x15c,
			0xe5c, 0xf55, 0xc5f, 0xd56, 0xa5a, 0xb53, 0x859, 0x950,
			0x7c0, 0x6c9, 0x5c3, 0x4ca, 0x3c6, 0x2cf, 0x1c5, 0xcc,
			0xfcc, 0xec5, 0xdcf, 0xcc6, 0xbca, 0xac3, 0x9c9, 0x8c0,
			0x8c0, 0x9c9, 0xac3, 0xbca, 0xcc6, 0xdcf, 0xec5, 0xfcc,
			0xcc, 0x1c5, 0x2cf, 0x3c6, 0x4ca, 0x5c3, 0x6c9, 0x7c0,
			0x950, 0x859, 0xb53, 0xa5a, 0xd56, 0xc5f, 0xf55, 0xe5c,
			0x15c, 0x55, 0x35f, 0x256, 0x55a, 0x453, 0x759, 0x650,
			0xaf0, 0xbf9, 0x8f3, 0x9fa, 0xef6, 0xfff, 0xcf5, 0xdfc,
			0x2fc, 0x3f5, 0xff, 0x1f6, 0x6fa, 0x7f3, 0x4f9, 0x5f0,
			0xb60, 0xa69, 0x963, 0x86a, 0xf66, 0xe6f, 0xd65, 0xc6c,
			0x36c, 0x265, 0x16f, 0x66, 0x76a, 0x663, 0x569, 0x460,
			0xca0, 0xda9, 0xea3, 0xfaa, 0x8a6, 0x9af, 0xaa5, 0xbac,
			0x4ac, 0x5a5, 0x6af, 0x7a6, 0xaa, 0x1a3, 0x2a9, 0x3a0,
			0xd30, 0xc39, 0xf33, 0xe3a, 0x936, 0x83f, 0xb35, 0xa3c,
			0x53c, 0x435, 0x73f, 0x636, 0x13a, 0x33, 0x339, 0x230,
			0xe90, 0xf99, 0xc93, 0xd9a, 0xa96, 0xb9f, 0x895, 0x99c,
			0x69c, 0x795, 0x49f, 0x596, 0x29a, 0x393, 0x99, 0x190,
			0xf00, 0xe09, 0xd03, 0xc0a, 0xb06, 0xa0f, 0x905, 0x80c,
			0x70c, 0x605, 0x50f, 0x406, 0x30a, 0x203, 0x109, 0x0
		};
			
		public static int[,] triTable = {{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1 },
			{ 2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1 },
			{ 8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1 },
			{ 3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1 },
			{ 4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1 },
			{ 4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1 },
			{ 5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1 },
			{ 2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1 },
			{ 9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1 },
			{ 2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1 },
			{ 10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1 },
			{ 5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1 },
			{ 5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1 },
			{ 10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1 },
			{ 8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1 },
			{ 2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1 },
			{ 7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1 },
			{ 2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1 },
			{ 11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1 },
			{ 5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1 },
			{ 11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1 },
			{ 11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1 },
			{ 5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1 },
			{ 2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1 },
			{ 5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1 },
			{ 6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1 },
			{ 3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1 },
			{ 6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1 },
			{ 5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1 },
			{ 10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1 },
			{ 6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1 },
			{ 8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1 },
			{ 7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1 },
			{ 3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1 },
			{ 5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1 },
			{ 0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1 },
			{ 9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1 },
			{ 8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1 },
			{ 5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1 },
			{ 0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1 },
			{ 6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1 },
			{ 10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1 },
			{ 10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1 },
			{ 8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1 },
			{ 1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1 },
			{ 0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1 },
			{ 10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1 },
			{ 3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1 },
			{ 6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1 },
			{ 9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1 },
			{ 8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1 },
			{ 3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1 },
			{ 6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1 },
			{ 10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1 },
			{ 10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1 },
			{ 2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1 },
			{ 7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1 },
			{ 7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1 },
			{ 2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1 },
			{ 1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1 },
			{ 11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1 },
			{ 8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1 },
			{ 0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1 },
			{ 7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1 },
			{ 10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1 },
			{ 2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1 },
			{ 6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1 },
			{ 7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1 },
			{ 2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1 },
			{ 10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1 },
			{ 10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1 },
			{ 0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1 },
			{ 7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1 },
			{ 6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1 },
			{ 8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1 },
			{ 6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1 },
			{ 4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1 },
			{ 10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1 },
			{ 8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1 },
			{ 1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1 },
			{ 8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1 },
			{ 10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1 },
			{ 10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1 },
			{ 5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1 },
			{ 11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1 },
			{ 9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1 },
			{ 6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1 },
			{ 7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1 },
			{ 3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1 },
			{ 7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1 },
			{ 3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1 },
			{ 6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1 },
			{ 9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1 },
			{ 1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1 },
			{ 4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1 },
			{ 7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1 },
			{ 6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1 },
			{ 0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1 },
			{ 6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1 },
			{ 0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1 },
			{ 11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1 },
			{ 6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1 },
			{ 5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1 },
			{ 9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1 },
			{ 1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1 },
			{ 10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1 },
			{ 0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1 },
			{ 5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1 },
			{ 10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1 },
			{ 11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1 },
			{ 9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1 },
			{ 7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1 },
			{ 2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1 },
			{ 8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1 },
			{ 9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1 },
			{ 9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1 },
			{ 1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1 },
			{ 5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1 },
			{ 0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1 },
			{ 10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1 },
			{ 2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1 },
			{ 0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1 },
			{ 0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1 },
			{ 9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1 },
			{ 5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1 },
			{ 5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1 },
			{ 8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1 },
			{ 9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1 },
			{ 1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1 },
			{ 3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1 },
			{ 4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1 },
			{ 9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1 },
			{ 11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1 },
			{ 11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1 },
			{ 2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1 },
			{ 9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1 },
			{ 3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1 },
			{ 1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1 },
			{ 4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1 },
			{ 0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1 },
			{ 9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1 },
			{ 1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ 0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
			{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }
		};

	}
}

