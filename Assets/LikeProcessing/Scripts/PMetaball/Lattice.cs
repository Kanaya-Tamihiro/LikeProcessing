using System;
using UnityEngine;
using System.Collections.Generic;

namespace LikeProcessing.PMetaball
{
	public class Lattice
	{
		public GameObject gameObject, blob;
		public LatticeMono latticeMono;
		PMetaball metaball;

		Point[,,] points;
		Edge[,,] edgeHorizon, edgeVertical, edgeDepth;
		Cube[,,] cubes;

		Vector3[] vertices;
		int[] triangleIndeces;
		Vector3[] normals;
		int lastUpdatedTriangleIndexCount = 0;

		public class LatticeMono : MonoBehaviour {
			public Lattice lattice;
			public HashSet<Core> affectedCores = new HashSet<Core>();
			void OnTriggerEnter(Collider other) {
				if (other.gameObject.tag == PMetaball.CoreTag) {
					Debug.Log ("Lattice enter other");
					Core.CoreMono coreMono = other.gameObject.GetComponent<Core.CoreMono> ();
					affectedCores.Add (coreMono.core);
					coreMono.core.affectLattices.Add (lattice);
					lattice.metaball.shouldUpdateLattices.Add (lattice);
				}
			}
			void OnTriggerExit(Collider other)
			{
				Debug.Log ("Lattice exit other");
				Core.CoreMono coreMono = other.gameObject.GetComponent<Core.CoreMono> ();
				affectedCores.Remove (coreMono.core);
				coreMono.core.affectLattices.Remove (lattice);
				lattice.metaball.shouldUpdateLattices.Add (lattice);
			}
		}

		public Lattice (PMetaball metaball, IntVector location)
		{
			this.metaball = metaball;
			gameObject = new GameObject ();
			blob = new GameObject ();
			gameObject.name = String.Format("Lattice ({0}, {1}, {2})", location.x, location.y, location.z);
			blob.name = "blob";
			gameObject.transform.SetParent (metaball.gameObject.transform);
			blob.transform.SetParent (metaball.gameObject.transform);
			float size = metaball.size;
			gameObject.transform.localPosition = new Vector3 (
				size + location.x * 2 * size,
				size + location.y * 2 * size,
				size + location.z * 2 * size
			);

			gameObject.AddComponent<MeshFilter> ();//.mesh.MarkDynamic ();
			blob.AddComponent<MeshFilter> ().mesh.MarkDynamic ();
			// gameObject.AddComponent<MeshRenderer> ().material = new Material(Shader.Find("LikeProcessing/VertexColor"));
			gameObject.AddComponent<MeshRenderer> ().material = PSketch.material;
			blob.AddComponent<MeshRenderer> ().material = PSketch.material;
			Mesh mesh = blob.GetComponent<MeshFilter> ().mesh;

			vertices = new Vector3[PMetaball.maxVertexCount];
			triangleIndeces = new int[PMetaball.maxVertexCount];
			normals = new Vector3[PMetaball.maxVertexCount];

			mesh.Clear ();
			mesh.vertices = vertices;
			mesh.triangles = triangleIndeces;
			mesh.normals = normals;

			BoxCollider collider = gameObject.AddComponent<BoxCollider> ();
			collider.center = Vector3.zero;
			collider.size = new Vector3 (size * 2, size * 2, size * 2);
			collider.isTrigger = true;
			latticeMono = gameObject.AddComponent<LatticeMono> ();
			latticeMono.lattice = this;
		}

		public void SetUpLattice () {
			SetPoint ();
			SetEdge ();
			SetCube ();
//			DrawPoints ();
//			DrawEdges ();
//			DrawEdge2();
//			DrawCubes ();
		}


		public void Update ()
		{
//			Debug.Log (gameObject.name + " Update called");
			if (metaball.isoValuesAddictive == true)
				CulcIsoValuesAdd ();
			else
				CulcIsoValuesMax ();
			//			ClearEdges ();
			CulcCubeVertices ();
			//			DrawIntersectionPoints ();
			SetMesh ();
		}

		public void SetMesh ()
		{
			Mesh mesh = blob.GetComponent<MeshFilter> ().mesh;
			mesh.vertices = vertices;
			mesh.triangles = triangleIndeces;
			mesh.normals = normals;
			if (metaball.isHardEdge) {
				//				Mesh mesh = gameObject.GetComponent<MeshFilter> ().mesh;
				mesh.RecalculateNormals ();
			}
		}

		void CulcIsoValuesAdd ()
		{
			int detail = metaball.detail;
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						Point p = points [iz, iy, ix];
						p.isoValue = 0;
						foreach (Core core in latticeMono.affectedCores) {
							float[] valueAndNormal = core.CulcIsoValueAndNormal (p, metaball.isoPower);
							p.isoValue += valueAndNormal [0];
							p.normal += new Vector3 (valueAndNormal[1], valueAndNormal[2], valueAndNormal[3]);
						}
						//						p.normal.Normalize ();
					}
				}
			}
		}

		void CulcIsoValuesMax ()
		{
			int detail = metaball.detail;
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						Point p = points [iz, iy, ix];
						p.isoValue = 0;
						foreach (Core core in latticeMono.affectedCores) {
							float[] valueAndNormal = core.CulcIsoValueAndNormal (p, metaball.isoPower);
							if (p.isoValue < valueAndNormal[0]) {
								p.isoValue = valueAndNormal[0];
								p.normal = new Vector3 (valueAndNormal[1], valueAndNormal[2], valueAndNormal[3]);
							}
						}
						//						p.normal.Normalize ();
					}
				}
			}
		}

		//		void ClearEdges () {
		//			for (int iz = 0; iz < detail + 1; iz++) {
		//				for (int iy = 0; iy < detail + 1; iy++) {
		//					for (int ix = 0; ix < detail; ix++) {
		//						edgeHorizon [iz, iy, ix].hasIntersection = false;
		//					}
		//				}
		//			}
		//			for (int iz = 0; iz < detail + 1; iz++) {
		//				for (int iy = 0; iy < detail; iy++) {
		//					for (int ix = 0; ix < detail + 1; ix++) {
		//						edgeVertical [iz, iy, ix].hasIntersection = false;
		//					}
		//				}
		//			}
		//			for (int iz = 0; iz < detail; iz++) {
		//				for (int iy = 0; iy < detail + 1; iy++) {
		//					for (int ix = 0; ix < detail + 1; ix++) {
		//						edgeDepth [iz, iy, ix].hasIntersection = false;
		//					}
		//				}
		//			}
		//		}

		void CulcCubeVertices ()
		{
			int detail = metaball.detail;
			float isoLevel = metaball.isoLevel;
			int triangleIndexCount = 0;
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
						cube.CulcIntersections (isoLevel);
						triangleIndexCount += cube.Vertices (vertices, normals, triangleIndexCount);
					}
				}
			}

			for (int i=lastUpdatedTriangleIndexCount; i<triangleIndexCount; i++) {
				triangleIndeces [i] = i;
			}
			for (int i=triangleIndexCount; i<lastUpdatedTriangleIndexCount; i++) {
				triangleIndeces [i] = 0;
				vertices [i] = Vector3.zero;
				normals [i] = Vector3.zero;
			}
			lastUpdatedTriangleIndexCount = triangleIndexCount;
		}

		void SetPoint ()
		{
			int detail = metaball.detail;
			float size = metaball.size;
			Vector3 position = gameObject.transform.localPosition;
			points = new Point[detail + 1, detail + 1, detail + 1];
			float x = -size + position.x;
			float y = -size + position.y;
			float z = -size + position.z;
			float delta = (size * 2) / detail;
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						Point point = new Point (x, y, z);
						points [iz, iy, ix] = point;
						x += delta;
					}
					x = -size + position.x;
					y += delta;
				}
				y = -size + position.y;
				z += delta;
			}
		}

		void SetEdge ()
		{
			int detail = metaball.detail;
			float size = metaball.size;
			edgeHorizon = new Edge[detail + 1, detail + 1, detail];
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail; ix++) {
						Point p1 = points [iz, iy, ix];
						Point p2 = points [iz, iy, ix + 1];
						Edge edge = new Edge (p1, p2);
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
						Edge edge = new Edge (p1, p2);
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
						Edge edge = new Edge (p1, p2);
						edgeDepth [iz, iy, ix] = edge;
					}
				}
			}
		}

		void SetCube ()
		{
			int detail = metaball.detail;
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
						Cube cube = new Cube (_points, _edges);
						cubes [iz, iy, ix] = cube;
					}
				}
			}
		}

		void DrawPoints ()
		{
			int detail = metaball.detail;
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
			int vertexCount = 0;
			int triangleCount = 0;
			int detail = metaball.detail;
			int edgeCount = (detail + 1) * (detail + 1) * detail * 3;
			int[] counts = edgeHorizon [0, 0, 0].MeshCounts ();
			vertexCount = counts [0] * edgeCount;
			triangleCount = counts [1] * edgeCount;
			PMesh pmesh = new PMesh (vertexCount, triangleCount);

			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail; ix++) {
						edgeHorizon [iz, iy, ix].Mesh (pmesh);
					}
				}
			}
			for (int iz = 0; iz < detail + 1; iz++) {
				for (int iy = 0; iy < detail; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						edgeVertical [iz, iy, ix].Mesh (pmesh);
					}
				}
			}
			for (int iz = 0; iz < detail; iz++) {
				for (int iy = 0; iy < detail + 1; iy++) {
					for (int ix = 0; ix < detail + 1; ix++) {
						edgeDepth [iz, iy, ix].Mesh (pmesh);
					}
				}
			}
			Mesh mesh = gameObject.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
//			Debug.Log (PUtil.IndecisToString(pmesh.triangles));
			mesh.vertices = pmesh.vertices;
			mesh.triangles = pmesh.triangles;
			mesh.RecalculateNormals ();
		}

		void DrawEdge2() {
			int detail = metaball.detail;
			Point p1 = points [0, 0, 0];
			Point p2 = points [0, 0, detail];
			Point p3 = points [0, detail, 0];
			Point p4 = points [0, detail, detail];
			Point p11 = points [detail, 0, 0];
			Point p22 = points [detail, 0, detail];
			Point p33 = points [detail, detail, 0];
			Point p44 = points [detail, detail, detail];
			int lineDetail = 3;
			float lineWeight = 0.05f;
			int[] lineMeshCounts = PLineSimple.MeshCounts (lineDetail);
			PMesh pmesh = new PMesh (lineMeshCounts[0] * 12, lineMeshCounts[1] * 12);
			Vector3 origin = gameObject.transform.localPosition;
			PLineSimple.Mesh (p1.loc - origin, p2.loc - origin, lineWeight, lineDetail, pmesh);
			PLineSimple.Mesh (p1.loc - origin, p3.loc - origin, lineWeight, lineDetail, pmesh);
			PLineSimple.Mesh (p3.loc - origin, p4.loc - origin, lineWeight, lineDetail, pmesh);
			PLineSimple.Mesh (p4.loc - origin, p2.loc - origin, lineWeight, lineDetail, pmesh);
			PLineSimple.Mesh (p11.loc - origin, p22.loc - origin, lineWeight, lineDetail, pmesh);
			PLineSimple.Mesh (p11.loc - origin, p33.loc - origin, lineWeight, lineDetail, pmesh);
			PLineSimple.Mesh (p33.loc - origin, p44.loc - origin, lineWeight, lineDetail, pmesh);
			PLineSimple.Mesh (p44.loc - origin, p22.loc - origin, lineWeight, lineDetail, pmesh);
			PLineSimple.Mesh (p1.loc - origin, p11.loc - origin, lineWeight, lineDetail, pmesh);
			PLineSimple.Mesh (p2.loc - origin, p22.loc - origin, lineWeight, lineDetail, pmesh);
			PLineSimple.Mesh (p3.loc - origin, p33.loc - origin, lineWeight, lineDetail, pmesh);
			PLineSimple.Mesh (p4.loc - origin, p44.loc - origin, lineWeight, lineDetail, pmesh);
			Mesh mesh = gameObject.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
			mesh.vertices = pmesh.vertices;
			mesh.triangles = pmesh.triangles;
			mesh.RecalculateNormals ();
		}

		public void DrawIntersectionPoints ()
		{
			int detail = metaball.detail;
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
			int detail = metaball.detail;
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
	}
}

