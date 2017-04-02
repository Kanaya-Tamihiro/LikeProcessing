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
			Vector3 x1 = new Vector3(-300,-200,200);
			Vector3 x2 = new Vector3(-100,300,100);
			Vector3 x3 = new Vector3(200,-400,0);
			Vector3 x4 = new Vector3(300,200,-200);
			background(Color.black);
			bezierLine = new PBezierLine (this.gameObject);
			bezierLine.setup(x1,x2,x3,x4,10.0f, 30);
		}

		float t = 0;

		void Update()
		{
			cameraRotateWithMouse ();
			bezierLine.updateFromTo (t, 1.0f);
			t += 0.01f;
			if (t >= 1.0f)
				t = 0;
		}
	}
}

