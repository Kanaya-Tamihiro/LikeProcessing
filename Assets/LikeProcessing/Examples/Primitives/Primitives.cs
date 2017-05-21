using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LikeProcessing;

namespace LikeProcessig.Examples {
	
	public class Primitives : PSketch {

		PGeodesicDome pgeodesicDome;

		void Start () {
			lightObj.GetComponent<Light> ().intensity = 0.8f;
			pgeodesicDome = new PGeodesicDome(3);
			pgeodesicDome.gameObject.transform.position += new Vector3 (0, 1, 0);				
		}

		void Update () {

		}
	}

}
