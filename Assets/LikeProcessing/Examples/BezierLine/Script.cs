using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LikeProcessing.Examples
{
	public class Script : PSketch
	{
		PBezierLine bezierLine;

		// Use this for initialization
		void Start()
		{
			Vector3 x1 = new Vector3(-3,-2,2);
			Vector3 x2 = new Vector3(-1,3,1);
			Vector3 x3 = new Vector3(2,-4,0);
			Vector3 x4 = new Vector3(3,2,-2);
			background(Color.black);
			bezierLine = new PBezierLine (this.gameObject);
			bezierLine.setup(x1,x2,x3,x4,0.1f, 10);
		}

		float t = 0;

		void Update()
		{
//			cameraRotateWithMouse ();
//			bezierLine.updateFromTo (t, 1.0f);
//			t += 0.01f;
//			if (t >= 1.0f)
//				t = 0f;
		}
	}
}

