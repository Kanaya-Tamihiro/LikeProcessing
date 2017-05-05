using UnityEngine;
using System.Collections.Generic;

namespace LikeProcessing
{

	public class PBezierLine
	{
		public GameObject bezierObj;
		Vector3 x1, x2, x3, x4;
		public float weight;
		public int detail;
		List<List<Vector3>> points, allPoints;
		int angle_count;
		float i_inc;

		public PBezierLine (GameObject parent) {
			bezierObj = new GameObject ("PBezierLine");
			bezierObj.transform.SetParent (parent.transform);
			bezierObj.AddComponent<MeshFilter> ();
			PSketch.fill (bezierObj, new Color(0.6F, 1.0F, 0.2F));
		}

		public void setup (Vector3 x1, Vector3 x2, Vector3 x3, Vector3 x4, float weight = .04f, int detail = 6)
		{
			this.x1 = x1; this.x2 = x2; this.x3 = x3; this.x4 = x4;
			this.weight = Mathf.Max(weight, 0.01f); this.detail = detail;

			Vector3 _z = new Vector3(x2[0]-x1[0],x2[1]-x1[1],x2[2]-x1[2]);
			Vector3 _zz = new Vector3(x3[0]-x2[0],x3[1]-x2[1],x3[2]-x2[2]);
			Vector3 _zzz = new Vector3(x4[0]-x3[0],x4[1]-x3[1],x4[2]-x3[2]);

			float approx_length = _z.magnitude + _zz.magnitude + _zzz.magnitude;
			float _seg = approx_length/20.0f;
			int segment_count = Mathf.CeilToInt(Mathf.Sqrt(_seg * _seg * 0.6f + 225.0f));
//			Debug.Log("segment_count %d\n", segment_count);
			i_inc = 1.0f/segment_count;

			points = new List<List<Vector3>>();
			angle_count = Mathf.Max(detail, 3);

			allPoints = new List<List<Vector3>>();
			for (float i = 0.0f; i <= 1.0f + i_inc/2.0f; i = i+i_inc) {
				if (i > 1.0f) {
					i = 1.0f;
				}
				allPoints.Add(bezierPointsAt(i));
			}
			updateFromTo (0.0f, 1.0f);
		}

		public void updateFromTo(float from, float to) {
			points.Clear ();
//			List<float> iList = new List<float> ();
//			iList.Add (from);
			points.Add(bezierPointsAt(from));
			float iStart = 0f;
			int index = 0;
			while (iStart <= from) {
				iStart += i_inc;
				index++;
			}
			for (float i = iStart; i < to; i = i + i_inc) {
				points.Add (allPoints[index]);
				index++;
			}
//			iList.Add (to);
//			foreach(float i in iList) {
//				points.Add(bezierPointsAt(i));
//			}
			points.Add(bezierPointsAt(to));

			List<Vector3> verteces = new List<Vector3> ();
			List<int> triangles = new List<int> ();

			for (int i = 0; i < points.Count -1; i++) {
				List<Vector3> edge1 = points[i];
				List<Vector3> edge2 = points[i+1];

				for (int j = 0; j < angle_count; j++) {
					int next = j == angle_count -1 ? 0 : j+1;
					Vector3 p11 = edge1[j];
					Vector3 p12 = edge1[next];
					Vector3 p21 = edge2[j];
					Vector3 p22 = edge2[next];
					verteces.Add(p11);
					triangles.Add (verteces.Count -1);
					verteces.Add(p12);
					triangles.Add (verteces.Count -1);
					verteces.Add(p22);
					triangles.Add (verteces.Count -1);
					verteces.Add(p22);
					triangles.Add (verteces.Count -1);
					verteces.Add(p21);
					triangles.Add (verteces.Count -1);
					verteces.Add(p11);
					triangles.Add (verteces.Count -1);
				}
			}
			Mesh mesh = bezierObj.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
			mesh.vertices = verteces.ToArray ();
			mesh.triangles = triangles.ToArray ();
			mesh.RecalculateNormals();
		}

		List<Vector3> bezierPointsAt(float i) {
			float[] q, tan;
			f(i, out q, out tan);
			Vector3 point = new Vector3(q[0], q[1], q[2]);
			Vector3 axis = new Vector3(tan[0], tan[1], tan[2]);
			Vector3 normal = new Vector3(tan[1], -1*tan[0], tan[2]);
			normal.Normalize ();
			normal = normal * weight;

			float angle = Mathf.PI*2/angle_count;
			List<Vector3> vertices = new List<Vector3>();
			for (int j = 0; j < angle_count; j++) {
				Vector3 v = Quaternion.AngleAxis (Mathf.Rad2Deg * (angle * j), axis) * normal;
				vertices.Add(v + point);
			}
			return vertices;
		}

		void f(float t, out float[] q, out float[] tan) {
			q = new float[3];
			tan = new float[3];
			float[] z1 = new float[3];
			float[] z2 = new float[3];
			float[] z3 = new float[3];
			float[] w1 = new float[3];
			float[] w2 = new float[3];
			z1[0] = (x2[0] - x1[0])*t + x1[0];
			z1[1] = (x2[1] - x1[1])*t + x1[1];
			z1[2] = (x2[2] - x1[2])*t + x1[2];
			z2[0] = (x3[0] - x2[0])*t + x2[0];
			z2[1] = (x3[1] - x2[1])*t + x2[1];
			z2[2] = (x3[2] - x2[2])*t + x2[2];
			z3[0] = (x4[0] - x3[0])*t + x3[0];
			z3[1] = (x4[1] - x3[1])*t + x3[1];
			z3[2] = (x4[2] - x3[2])*t + x3[2];
			w1[0] = (z2[0] - z1[0])*t + z1[0];
			w1[1] = (z2[1] - z1[1])*t + z1[1];
			w1[2] = (z2[2] - z1[2])*t + z1[2];
			w2[0] = (z3[0] - z2[0])*t + z2[0];
			w2[1] = (z3[1] - z2[1])*t + z2[1];
			w2[2] = (z3[2] - z2[2])*t + z2[2];
			float dx = w2[0] - w1[0];
			float dy = w2[1] - w1[1];
			float dz = w2[2] - w1[2];
			tan[0] = dx;
			tan[1] = dy;
			tan[2] = dz;
			q[0] = (w2[0] - w1[0])*t + w1[0];
			q[1] = (w2[1] - w1[1])*t + w1[1];
			q[2] = (w2[2] - w1[2])*t + w1[2];
		}

		public void setColor(Color color) {
			bezierObj.GetComponent<Renderer> ().material.color = color;
		}

		public void destory() {
			Object.Destroy (bezierObj);
		}

	}
}

