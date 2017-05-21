using UnityEngine;
using LikeProcessing.Lsystem;

namespace LikeProcessing.Examples
{
	public class Lsystem : PSketch
	{
		PLsystem lsystem;
		Turtle turtle;

		void Start() {
			StartMeanderingSnake ();
			this.lightObj.GetComponent<Light> ().intensity = 0.8f;
		}

		void StartA()
		{
			//background(Color.black);
			Rule[] ruleset = new Rule[1];
			ruleset[0] = new Rule('F', "FF+[+F-F-F]-[-F+F+F]");
			lsystem = new PLsystem("F", ruleset);
			lsystem.generate(4);
			turtle = new Turtle(lsystem.getSentence(), 0.15f, Mathf.Deg2Rad*25, Vector3.down*0);
			turtle.render();
		}

		void StartAA()
		{
			//background(Color.black);
			Rule[] ruleset = new Rule[1];
//			ruleset[0] = new Rule('F', "FF[+F][-F][*F][/F]");
//			ruleset[0] = new Rule('F', "FF+[+F-F-F]-[-F+F+F]*[*F/F/F]/");
			ruleset[0] = new Rule('F', "FF+[*+F-F-F]-[/-F+F+F]");
			lsystem = new PLsystem("F", ruleset);
			lsystem.generate(3);
			turtle = new Turtle(lsystem.getSentence(), 0.15f, Mathf.Deg2Rad*110, Vector3.down*0);
			turtle.render();
		}

		void StartB() {
			background(Color.cyan);
			Rule[] ruleset = new Rule[5];
			ruleset[0] = new Rule('F', "F+F-F");
			ruleset[1] = new Rule('W', "YF++ZF4-XF[-YF4-WF]++");
			ruleset[2] = new Rule('X', "+YF--ZF[3-WF--XF]+");
			ruleset[3] = new Rule('Y', "-WF++XF[+++YF++ZF]-");
			ruleset[4] = new Rule('Z', "--YF++++WF[+ZF++++");
			lsystem = new PLsystem("[X]++[X]++[X]++[X]++[X]", ruleset);
			lsystem.generate(2);
			turtle = new Turtle(lsystem.getSentence(), 0.15f, Mathf.Deg2Rad*36, Vector3.down*2);
			turtle.render();
		}

		void StartKoch() {
			background(Color.black);
			Rule[] ruleset = new Rule[1];
			ruleset[0] = new Rule('F', "F+F-F-F+F");
			lsystem = new PLsystem("F", ruleset);
			lsystem.generate(5);
			turtle = new Turtle(lsystem.getSentence(), 0.15f, Mathf.Deg2Rad*90, Vector3.down*2);
			turtle.render();
		}

		void StartMeanderingSnake() {
			background(Color.black);
			Rule[] ruleset = new Rule[1];
			ruleset[0] = new Rule('F', "F-F+F");
			lsystem = new PLsystem("F", ruleset);
			lsystem.generate(3);
			turtle = new Turtle(lsystem.getSentence(), 0.15f, Mathf.Deg2Rad*90, Vector3.down*00);
			turtle.render();
		}

		void StartMizukusa() {
			background(Color.black);
			Rule[] ruleset = new Rule[1];
			ruleset[0] = new Rule('F', "FF[+FF][-F]F[-F]");
			lsystem = new PLsystem("F", ruleset);
			lsystem.generate(2);
			turtle = new Turtle(lsystem.getSentence(), 0.15f, Mathf.Deg2Rad*30, Vector3.down*2);
			turtle.render();
		}

		void StartHorsetailGrass() {
			background(Color.black);
			Rule[] ruleset = new Rule[1];
			ruleset[0] = new Rule('F', "FF-[-F+F+F]+[+F-F-F]");
			lsystem = new PLsystem("++F", ruleset);
			lsystem.generate(3);
			turtle = new Turtle(lsystem.getSentence(), 0.15f, Mathf.Deg2Rad*16, Vector3.down*2);
			turtle.render();
		}

		void StartSparklingFirework() {
			background(Color.black);
			Rule[] ruleset = new Rule[3];
			ruleset[0] = new Rule('F', "OA++PA----FA[-OA----MA]++");
			ruleset[1] = new Rule('N', "+OA--PA[---MA--NA]+");
			ruleset[2] = new Rule('P', "--OA+++++MA[+PA++++NA]--NA");
			lsystem = new PLsystem("[F]++[F]++[F]++[F]++[F]", ruleset);
			lsystem.generate(3);
			turtle = new Turtle(lsystem.getSentence(), 0.15f, Mathf.Deg2Rad*36, Vector3.down*2);
			turtle.render();
		}

		void Update()
		{
            //cameraRotateWithMouse();
		}
	}
}