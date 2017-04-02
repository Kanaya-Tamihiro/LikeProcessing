using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LikeProcessing;

namespace Koch {

	public class Koch {

		List<KochLine> lines;
		PLine[] plines;
		GameObject koch;

		public Koch(Vector3 start, Vector3 end) {
			koch = new GameObject ("koch");
			lines = new List<KochLine>();
			lines.Add(new KochLine(start, end));
		}

		public void generate(int cycle) {
			for (int i = 0; i < cycle; i++) {
				List<KochLine> next = new List<KochLine>();
				foreach (KochLine l in lines) {
					Vector3 a = l.kochA();
					Vector3 b = l.kochB();
					Vector3 c = l.kochC();
					Vector3 d = l.kochD();
					Vector3 e = l.kochE();
					next.Add(new KochLine(a, b));
					next.Add(new KochLine(b, c));
					next.Add(new KochLine(c, d));
					next.Add(new KochLine(d, e));
				}
				lines = next;
			}
		}

		public void drawLine() {
			if (plines != null) {
				foreach (PLine pline in plines) {
					pline.destory ();
				}
			}
			plines = new PLine[lines.Count];
			for (int i=0; i<lines.Count; i++) {
				KochLine k = lines [i];
				plines [i] = new PLine (koch, k.start, k.end);
			}
		}
	}
	
	public class KochLine {
		public Vector3 start;
		public Vector3 end;

		public KochLine(Vector3 a, Vector3 b) {
			start = a;
			end = b;
//			pline = new PLine (parent, start, end, 3);
		}

		public Vector3 kochA() {
			return start;
		}

		public Vector3 kochE() {
			return end;
		}

		public Vector3 kochB() {
			Vector3 v = end - start;
			v *= 0.33333f;
			v = Quaternion.AngleAxis (30 , Vector3.forward) * v;
			v += start;
			return v;
		}

		public Vector3 kochD() {
			Vector3 v = end - start;
			v *= 0.66666f;
			v = Quaternion.AngleAxis (-30 , Vector3.forward) * v;
			v += start;
			return v;
		}

		public Vector3 kochC() {
			Vector3 a = start;
			Vector3 v = end - start;
			v *= 0.33333f;
			a += v;
			v = Quaternion.AngleAxis (-20 , Vector3.forward) * v;
			a += v;
			return a;
		}

//		public void destroy() {
//			pline.destory();
//		}
	}

}
