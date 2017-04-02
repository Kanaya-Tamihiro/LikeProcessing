using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LikeProcessing;

namespace N20170311.Bezier {


	public class Script : PSketch {

		Movas movas;

		// Use this for initialization
		void Start () {
			background(Color.black);
//			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
//			obj.transform.localScale = new Vector3(1000, 1000, 1);
//			obj.transform.position = new Vector3 (0,0,300);
//			obj.transform.rotation *= Quaternion.Euler (90,45,0);
			movas = new Movas(30);
		}

		// Update is called once per frame
		void Update () {
			movas.update ();
		}
	}
}


