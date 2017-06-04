﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LikeProcessing;
using LikeProcessing.PMetaball;

namespace LikeProcessig.Examples {
	
	public class Primitives : PSketch {

		PGeodesicDomeImproved pgeodesicDomeImproved;
		PGeodesicDome pgeodesicDome;
		PLineSimple plineSimple;
		PMetaball pmetaball;

		float deltaTime = 0f;

		PMetaball.Core core1, core2;

		void Start () {
			lightObj.GetComponent<Light> ().intensity = 0.8f;

//			pgeodesicDomeImproved = new PGeodesicDomeImproved(3, false);
//			pgeodesicDomeImproved.gameObject.transform.position += new Vector3 (-2, 1, 0);
//			pgeodesicDomeImproved.gameObject.GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Standard"));
//
//			pgeodesicDome = new PGeodesicDome(3);
//			pgeodesicDome.gameObject.transform.position += new Vector3 (-2, 3, 0);
//			pgeodesicDome.gameObject.GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Standard"));
//
//			plineSimple = new PLineSimple(new Vector3(0, 0, 0), new Vector3(1, 2, 0));
//			plineSimple.gameObject.GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Standard"));

			pmetaball = new PMetaball();
			pmetaball.detail = 30;
			pmetaball.size = 5.0f;
			pmetaball.isoPower = .7f;
			pmetaball.isoValuesAddictive = true;
			pmetaball.isHardEdge = true;
			pmetaball.gameObject.transform.position = new Vector3 (0,2,0);
			pmetaball.SetUpLattice ();
			core1 = new PMetaball.Core (new Vector3 (1, 0, 0));
			pmetaball.AddCore (core1);
			core2 = new PMetaball.Core (new Vector3 (1, 0, 0));
			pmetaball.AddCore (core2);
//			pmetaball.AddCore (new PMetaball.CoreLine(new Vector3 (-1.0f,-1.0f,0), new Vector3 (1.0f,1.0f,0)));
//			pmetaball.AddMetaball (new Vector3 (0.25f,1,0));
			pmetaball.Update ();
		}

		void Update () {
			deltaTime += Time.deltaTime;
//			if (deltaTime >= 1.0f/10.0f)
//			{
//				deltaTime = 0;
//				pmetaball.isoPower -= 0.001f;
//				pmetaball.Update ();
//			}
			float angle = Mathf.PI/2 * Time.time;
			core1.position = (new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), Mathf.Cos (angle))) * 2.5f;;
			core2.position = (new Vector3 (Mathf.Sin(angle), Mathf.Cos(angle), Mathf.Cos(angle) * Mathf.Sin(angle))) * 1.5f;
			pmetaball.Update ();
		}
	}

}
