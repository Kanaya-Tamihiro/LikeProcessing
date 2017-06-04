using System;
using UnityEngine;

namespace LikeProcessing.PMetaball
{
	public class Point
	{
		public Vector3 loc;
		PGeodesicDomeImproved pGeodesicDome;
		public float isoValue;
		public Vector3 normal;

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
}

