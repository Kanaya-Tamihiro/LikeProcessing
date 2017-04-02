using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LikeProcessing;

namespace Koch {
	public class Script : PSketch {

		Koch koch;

		void Start () {
			background(Color.black);
			Vector3 start = new Vector3(-Screen.width/2, 0, 0);
			Vector3 end   = new Vector3(Screen.width/2, 0, 0);
			koch = new Koch (start, end);
			koch.generate (5);
			koch.drawLine ();
		}

		void Update () {
//			foreach (KochLine l in lines) {
//				l.display();
//			}
			cameraRotateWithMouse();
		}


	}
}
