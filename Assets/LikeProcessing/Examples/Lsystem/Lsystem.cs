using UnityEngine;
using LikeProcessing.Lsystem;

namespace LikeProcessing.Examples
{
	public class Lsystem : PSketch
	{
		PLsystem lsystem;
		Turtle turtle;

		void Start()
		{
			background(Color.black);
			Rule[] ruleset = new Rule[1];
			ruleset[0] = new Rule('F', "FF+[+F-F-F]-[-F+F+F]");
			lsystem = new PLsystem("F", ruleset);
			lsystem.generate();
			lsystem.generate();
			lsystem.generate();
			turtle = new Turtle(lsystem.getSentence(), 15, Mathf.Deg2Rad*25, Vector3.down*200);
			turtle.render();

		}

		// Update is called once per frame
		void Update()
		{
//			cameraRotateWithMouse();
		}
	}
}