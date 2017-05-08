using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace LikeProcessing.Lsystem {

	class MatrixEx {
		
	}
	
	public class PLsystem
	{
		string sentence;
		Rule[] ruleset;
		int generation;

		public PLsystem (string axiom, Rule[] r) {
			sentence = axiom;
			ruleset = r;
			generation = 0;
		}

		public void generate(int cycle) {
			for(int c=0; c<cycle; c++) {
				// An empty StringBuffer that we will fill
				StringBuilder nextgen = new StringBuilder();
				// For every character in the sentence
				for (int i = 0; i < sentence.Length; i++) {
					// What is the character
					char curr = sentence[i];
					// We will replace it with itself unless it matches one of our rules
					string replace = "" + curr;
					// Check every rule
					for (int j = 0; j < ruleset.Length; j++) {
						char a = ruleset[j].getA();
						// if we match the Rule, get the replacement String out of the Rule
						if (a == curr) {
							replace = ruleset[j].getB();
							break; 
						}
					}
					// Append replacement String
					nextgen.Append(replace);
				}
				// Replace sentence
				sentence = nextgen.ToString();
				// Increment generation
				generation++;
			}

		}

		public string getSentence() {
			return sentence; 
		}

		public int getGeneration() {
			return generation; 
		}
	}

	public class Rule {
		char a;
		string b;

		public Rule(char a_, string b_) {
			a = a_;
			b = b_; 
		}

		public char getA() {
			return a;
		}

		public string getB() {
			return b;
		}
	}

	public class Turtle {
		string todo;
		float len;
		float theta;
		List<PLine> plines;
		GameObject gameObj;

		public Turtle(string s, float l, float t, Vector3 position) {
			todo = s;
			len = l; 
			theta = t;
			plines = new List<PLine> ();
			gameObj = new GameObject ("turtle");
			gameObj.transform.position = position;
		} 

		public void render() {
			foreach (PLine line in plines) {
				line.destory ();
			}
			plines.Clear ();

			PMatrix matrix = PMatrix.identity;
//			matrix.translate (gameObj.transform.position);
			Stack<PMatrix> matrixes = new Stack<PMatrix> ();

			for (int i = 0; i < todo.Length; i++) {
				char c = todo[i];
				if (c == 'F' || c == 'G') {
					Vector3 from = matrix.m.MultiplyPoint3x4 (Vector3.zero);
					Vector3 to = matrix.m.MultiplyPoint3x4 (Vector3.up * len);
					PLine line = new PLine (gameObj, from, to, 0.03f);
					plines.Add (line);
					matrix.translate (Vector3.up * len);
				} else if (c == '+') {
					matrix.rotateZ (-theta);
				} else if (c == '-') {
					matrix.rotateZ (theta);
				} else if (c == '*') {
					matrix.rotateX (-theta);
				} else if (c == '%') {
					matrix.rotateX (theta);
				} else if (c == '[') {
					matrixes.Push (matrix);
					matrix = matrix.copy ();
				} else if (c == ']') {
					matrix = matrixes.Pop ();
				}
			}
		}

		public void setLen(float l) {
			len = l;
		} 

		public void changeLen(float percent) {
			len *= percent;
		}

		public void setToDo(string s) {
			todo = s;
		}
	}

}


