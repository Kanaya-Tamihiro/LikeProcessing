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

		public int CulcIntersection (List<Vector3> vertexList, List<Vector3> normalList, int _triangleIndex, bool isHardEdge)
		{
			if (hasIntersection == true) {
				if (isHardEdge) {
					vertexList.Add (intersection);
					normalList.Add (intersectionNormal);
					triangleIndex = _triangleIndex;
					return 1;
				} else
					return 0;
			}
			hasIntersection = true;
			LinearInterpolation ();
			vertexList.Add (intersection);
			normalList.Add (intersectionNormal);
			triangleIndex = _triangleIndex;
			return 1;
		}

		void LinearInterpolation ()
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

