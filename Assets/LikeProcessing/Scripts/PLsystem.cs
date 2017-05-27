using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace LikeProcessing.Lsystem {

	class MatrixEx {
		
	}
	
	public class PLsystem
	{
		public string sentence;
		public PRule rule;
		public int generation;
		public GameObject gameObject;

		public PLsystem (string axiom, PRule _rule) {
			sentence = axiom;
			rule = _rule;
			generation = 0;
			gameObject = new GameObject ("PLsystem");
		}

		public void Generate(int cycle) {
			for(int c=0; c<cycle; c++) {
				StringBuilder nextgen = new StringBuilder();
				for (int i = 0; i < sentence.Length; i++) {
					char curr = sentence[i];
					string replace = rule.Generate(curr);
					nextgen.Append(replace);
				}
				sentence = nextgen.ToString();
				generation++;
			}
		}

		public void Render() {
			//			foreach (PLine line in plines) {
			//				line.destory ();
			//			}
			//			plines.Clear ();

			//			PMatrix matrix = PMatrix.identity;
			////			matrix.translate (gameObj.transform.position);
			//			Stack<PMatrix> matrixes = new Stack<PMatrix> ();

			for (int i = 0; i < sentence.Length; i++) {
				char c = sentence[i];
				rule.Render (gameObject, c);
			}
		}

	}

	public class PRule {
		List<KeyValuePair<char, string>> rules = new List<KeyValuePair<char, string>> ();
		public float len = 0.2f;
		public float theta = Mathf.Deg2Rad*30;
		PMatrix matrix;
		Stack<PMatrix> matrixes;

		public PRule() {
			ResetMatrix ();
		}

		public void AddRule(char c, string s) {
			rules.Add (new KeyValuePair<char, string>(c, s));
		}

		public string Generate(char c) {
			foreach (KeyValuePair<char, string> kv in rules) {
				char a = kv.Key;
				if (a == c) {
					return kv.Value;
				}
			}
			return "" + c;
		}

		public void ResetMatrix() {
			matrix = PMatrix.identity;
			matrixes = new Stack<PMatrix> ();
		}

		public void Render(GameObject gameObject, char c) {
			if (c == 'F') {
				Vector3 from = matrix.m.MultiplyPoint3x4 (Vector3.zero);
				Vector3 to = matrix.m.MultiplyPoint3x4 (Vector3.up * len);
				PLineSimple line = new PLineSimple (from, to, 0.02f, 12);
//				GameObject gobj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
//				gobj.transform.localScale *= 0.05f;
//				gobj.transform.position = (from + to) / 2;
//				plines.Add (line);
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


