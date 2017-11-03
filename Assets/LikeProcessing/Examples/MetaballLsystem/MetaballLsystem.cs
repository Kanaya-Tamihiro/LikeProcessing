using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LikeProcessing;
using LikeProcessing.PMetaball;
using LikeProcessing.Lsystem;

namespace LikeProcessing.Examples
{

	public class MetaballLsystem : PSketch
	{
		PLsystem lsystem;
		MetaballRule rule;

		// Use this for initialization

		void Start () {
			StartA ();
		}

		void StartA ()
		{
			rule = new MetaballRule ();
			rule.AddRule('F', "FF+[+F-F-F]-[-F+F+F]");
			lsystem = new PLsystem("F", rule);
			lsystem.Generate(3);
			rule.len = .5f;
			rule.theta = Mathf.Deg2Rad * 25;
			lsystem.gameObject.transform.position = Vector3.down*0;
			lsystem.Render();
		}


		void StartB ()
		{
			rule = new MetaballRule ();
			rule.AddRule('F', "FF+[*+F-F-F]-[/-F+F+F]");
			lsystem = new PLsystem("F", rule);
			lsystem.Generate(3);
			rule.len = 1.3f;
			rule.theta = Mathf.Deg2Rad * 110;
			lsystem.gameObject.transform.position = Vector3.down*0;
			lsystem.Render();
		}
	
		// Update is called once per frame
		void Update ()
		{
			rule.metaball.Update ();
		}

		public class MetaballRule : PRule {
			public PMetaball.PMetaball metaball;

			public MetaballRule () {
				metaball = new PMetaball.PMetaball();
				metaball.isHardEdge = true;
				metaball.isoValuesAddictive = true;
//				metaball.isoLevel = 3.2f;
//				Core core1 = new Core (metaball, new Vector3 (3, 3, 3));
//				metaball.AddCore (core1);
			}

			public override void Render(GameObject gameObject, char c) {
				if (c == 'F') {
					Vector3 from = matrix.m.MultiplyPoint3x4 (Vector3.zero);
//					Vector3 to = matrix.m.MultiplyPoint3x4 (Vector3.up * len);
//					PLineSimple line = new PLineSimple (from, to, 0.02f, 12);
					Core core1 = new Core (metaball, from);
					metaball.AddCore (core1);
					matrix.translate (Vector3.up * len);
				} else if (c == '+') {
					matrix.rotateZ (-theta);
				} else if (c == '-') {
					matrix.rotateZ (theta);
				} else if (c == '*') {
					matrix.rotateX (-theta);
				} else if (c == '/') {
					matrix.rotateX (theta);
				} else if (c == '[') {
					matrixes.Push (matrix);
					matrix = matrix.copy ();
				} else if (c == ']') {
					matrix = matrixes.Pop ();
				}
			}


		}
	}
}