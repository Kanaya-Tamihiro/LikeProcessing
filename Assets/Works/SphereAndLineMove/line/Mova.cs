using UnityEngine;
using LikeProcessing;

namespace N20170311.Line {

	enum Mode {FORWARD, MOVE, REMOVE, STAY};

	public class Movas {
		Mova[] movas;
		GameObject parent;

		public Movas(int count) {
			parent = new GameObject ("Movas");
			movas = new Mova[count];
			for (int i=0; i<movas.Length; i++) {
				movas[i] = new Mova(parent);
			}
		}

		public void update() {
			foreach(Mova mova in movas) {
				mova.update();
				mova.draw ();
			}
		}
	}

	public class Mova
	{
		Mode mode;
		float pa_amount;
		float pb_amount;
		float t, tt;
		float dt;
		float da;
		Vector3 p1, p2, p1p2, p, pp;
		Color stroke_color;
		Color fill_color;
		float line_weight;
		GameObject sphereFrom, sphereTo;
		PLine pline;

		public Mova (GameObject parent)
		{
			sphereFrom = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			sphereTo = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			pline = new PLine (parent);
			sphereFrom.transform.SetParent (parent.transform);
			sphereTo.transform.SetParent (parent.transform);
			reset(false);
		}

		void reset(bool pre) {
			pa_amount = Random.Range(500.0f,500.0f);
			pb_amount = 0;
			t = 0;
			tt = 0;
			dt = Random.Range(.01f,.01f);
			da = pa_amount * dt / 2;
			mode = Mode.FORWARD;
			float r = 250.0f;	//random(100,300);
			p1 = pre ? p2 : PSketch.randomVector() * r;
			p2 = PSketch.randomVector() * r;
//			p1p2 = (p1 + p1) * -.05f;
			p1p2 = p2 - p1;
			float hue = Random.value;
			stroke_color = Color.HSVToRGB(hue, .2f, 1.0f);
			pline.setColor(stroke_color);
			fill_color = Color.HSVToRGB(hue, .5f, 1.0f);
			sphereFrom.GetComponent<Renderer> ().material.color = fill_color;
			sphereTo.GetComponent<Renderer> ().material.color = fill_color;
			line_weight = 1.0f;
			pline.setWeight (line_weight);
		}

		public void update() {
			if (mode == Mode.FORWARD) updateForward();
			else if(mode == Mode.MOVE) updateMove();
			else if(mode == Mode.REMOVE) updateRemove();
		}

		public void draw() {
			if (mode == Mode.FORWARD) drawForward();
			else if(mode == Mode.MOVE) drawMove();
			else if(mode == Mode.REMOVE) drawRemove();
		}

		void updateForward() {
			if (t >= 1.0f) {
				mode = Mode.MOVE;
				updateMove();
				return;
			}
			t += dt;
			pa_amount -= da;
			p = p1 + p1p2 * t;
		}

		void updateMove() {
			if (pa_amount <= 0) {
				mode = Mode.REMOVE;
				updateRemove();
				return;
			}
			pa_amount -= da;
			pb_amount += da;
		}

		void updateRemove() {
			if (tt >= 1.0f) {
				reset(true);
			}
			pb_amount += da;
			tt += dt;
			pp = p1 + p1p2 * tt;
		}

		void drawForward() {
//			noFill();
//			stroke(stroke_color);
//			strokeWeight(line_weight);
			pline.update(p1, p);
//			if (t < 1) {
//				float x = p1.x, y = p1.y, z = p1.z;
//				for(float i=0f; i<t; i+=dt*.1f) {
//					float xx = bezierPoint(p1.x, p1p2.x, p1p2.x, p2.x, i);
//					float yy = bezierPoint(p1.y, p1p2.y, p1p2.y, p2.y, i);
//					float zz = bezierPoint(p1.z, p1p2.z, p1p2.z, p2.z, i);
//					pg.line(x,y,z,xx,yy,zz);
//					x = xx; y = yy; z = zz;
//				}
//			}
//			else {
//				pg.bezier(p1.x, p1.y, p1.z, p1p2.x, p1p2.y, p1p2.z, p1p2.x, p1p2.y, p1p2.z, p2.x, p2.y, p2.z);
//			}
//			pg.fill(fill_color);
//			pg.noStroke();
			if (pa_amount > 0) {
				float r = 2 * Mathf.Sqrt (pa_amount / Mathf.PI);
				sphereFrom.SetActive (true);
				sphereFrom.transform.position = p1;
				sphereFrom.transform.localScale = new Vector3 (1, 1, 1) * r;
			} else {
				sphereFrom.SetActive (false);
			}
//			pg.pushMatrix();
//			pg.translate(p1.x,p1.y,p1.z);
//			pg.sphere(r);
//			pg.popMatrix();
		}
		void drawMove() {
			drawForward();
			float r = 2 * Mathf.Sqrt(pb_amount / Mathf.PI);
//			pg.pushMatrix();
//			pg.translate(p2.x,p2.y,p2.z);
//			pg.sphere(r);
			if (pb_amount > 0) {
				sphereTo.SetActive (true);
				sphereTo.transform.position = p2;
				sphereTo.transform.localScale = new Vector3 (1, 1, 1) * r;
			} else {
				sphereTo.SetActive (false);
			}
//			pg.popMatrix();
		}
		void drawRemove() {
//			pg.noFill();
//			pg.stroke(stroke_color);
//			pg.strokeWeight(line_weight);
			pline.update(pp, p2);
//			float x = bezierPoint(p1.x, p1p2.x, p1p2.x, p2.x, tt);
//			float y = bezierPoint(p1.y, p1p2.y, p1p2.y, p2.y, tt);
//			float z = bezierPoint(p1.z, p1p2.z, p1p2.z, p2.z, tt);
//			for(float i=tt+dt*.1; i<=1; i+=dt*.1) {
//				float xx = bezierPoint(p1.x, p1p2.x, p1p2.x, p2.x, i);
//				float yy = bezierPoint(p1.y, p1p2.y, p1p2.y, p2.y, i);
//				float zz = bezierPoint(p1.z, p1p2.z, p1p2.z, p2.z, i);
//				pg.line(x,y,z,xx,yy,zz);
//				x = xx; y = yy; z = zz;
//			}
//			pg.fill(fill_color);
//			pg.noStroke();
			float r = 2 * Mathf.Sqrt(pb_amount / Mathf.PI);
//			pg.pushMatrix();
//			pg.translate(p2.x,p2.y,p2.z);
//			pg.sphere(r);
			if (pb_amount > 0) {
				sphereTo.SetActive (true);
				sphereTo.transform.position = p2;
				sphereTo.transform.localScale = new Vector3 (1, 1, 1) * r;
			} else {
				sphereTo.SetActive (false);
			}
//			pg.popMatrix();
		}
	}

}
