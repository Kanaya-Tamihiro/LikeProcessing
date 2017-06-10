using System;
using UnityEngine;
using System.Collections.Generic;

namespace LikeProcessing.PMetaball
{
	public class Edge
	{
		Point[] points = new Point[2];
		PLineSimple pline;
		PGeodesicDomeImproved pGeodesicDome;
		public Vector3 intersection;
		public Vector3 intersectionNormal;
		public bool hasIntersection = false;

		public Edge (Point p1, Point p2)
		{
			points [0] = p1;
			points [1] = p2;
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
			pline = new PLineSimple (points [0].loc, points [1].loc, 0.01f, 3, parent);
		}

		public int[] MeshCounts () {
			return PLineSimple.MeshCounts (3);
		}

		public PMesh Mesh (PMesh pmesh) {
			return PLineSimple.Mesh (points [0].loc, points [1].loc, 0.01f, 3, pmesh);
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
			//					pGeodesicDome.gameObject.transform.localPosition = intersection;
			//					pGeodesicDome.gameObject.SetActive (true);
			//				}
		}

		public void CulcIntersection (float isoLevel)
		{
			if (hasIntersection == true)
				return;
			hasIntersection = true;
			LinearInterpolation (isoLevel);
		}

		void LinearInterpolation (float isoLevel)
		{
			Point p1 = points [0];
			Point p2 = points [1];
			if (lessThan (p2.loc, p1.loc)) {
				Point temp;
				temp = p1;
				p1 = p2;
				p2 = temp;    
			}

			if (Mathf.Abs (p1.isoValue - p2.isoValue) > 0.00001f) {
				intersection = p1.loc + (p2.loc - p1.loc) / (p2.isoValue - p1.isoValue) * (isoLevel - p1.isoValue);
				intersectionNormal = p1.normal + (p2.normal - p1.normal) / (p2.isoValue - p1.isoValue) * (isoLevel - p1.isoValue);
			} else {
				intersection = p1.loc;
				intersectionNormal = p1.normal;
			}
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
}

