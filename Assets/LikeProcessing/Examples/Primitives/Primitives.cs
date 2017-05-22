using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LikeProcessing;

namespace LikeProcessig.Examples {
	
	public class Primitives : PSketch {

		PGeodesicDomeImproved pgeodesicDome;
		PLineSimple plineSimple;

		void Start () {
			lightObj.GetComponent<Light> ().intensity = 0.8f;

			pgeodesicDome = new PGeodesicDomeImproved(3);
			pgeodesicDome.gameObject.transform.position += new Vector3 (-2, 1, 0);
			pgeodesicDome.gameObject.GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Standard"));

			plineSimple = new PLineSimple(new Vector3(0, 0, 0), new Vector3(1, 2, 0));
			plineSimple.gameObject.GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Standard"));

		}

		void Update () {

		}
	}

}
