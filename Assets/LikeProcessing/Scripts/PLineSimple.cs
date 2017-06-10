using UnityEngine;

namespace LikeProcessing
{
	public class PLineSimple
	{
		float weight;
		int detail;
		public GameObject gameObject;
		Vector3 from, to;
//		Vector3[] vertices;
//		int[] triangles;

		public PLineSimple (Vector3 _from, Vector3 _to, float _weight = 0.1f, int _detail = 6, GameObject parent = null)
		{
			gameObject = new GameObject();
			gameObject.name = "PLineSimple";
			gameObject.AddComponent<MeshFilter> ();
//			gameObject.AddComponent<MeshRenderer> ().material = new Material(Shader.Find("LikeProcessing/VertexColor"));
			gameObject.AddComponent<MeshRenderer> ().material = PSketch.material;
			if (parent != null) {
				gameObject.transform.SetParent (parent.transform);
			}
			weight = _weight;
			detail = _detail;
			from = _from;
			to = _to;
			SetMesh ();
		}

		public static int[] MeshCounts (int detail) {
			int[] counts = { detail * 2, detail * 2 * 3 };
			return counts;
		}

		public static PMesh Mesh(Vector3 from, Vector3 to, float weight, int detail, PMesh pmesh = null) {
			if (pmesh == null) {
				pmesh = new PMesh (detail * 2, detail * 2 * 3);
			}
//			Vector3 center = (from + to) / 2;
//			Vector3 _from = from - center;
//			Vector3 _to = to - center;
//			Vector3 fromTo = _to - _from;
			Vector3 fromTo = to - from;
			fromTo.Normalize ();
			Vector3 randv = Vector3.right;
			if (Vector3.Angle(fromTo, randv) < 5.0f) {
				randv = Vector3.up;
			}
			Vector3 v1 = Vector3.Cross (fromTo, randv);
			v1.Normalize ();
			Vector3 v2 = Vector3.Cross (fromTo, v1);
			v2.Normalize ();
			v1 = v1 * weight;
			v2 = v2 * weight * -1;
			Vector3[] fromPoints = new Vector3[detail];
			Vector3[] toPoints = new Vector3[detail];
			float theta = 2f * Mathf.PI / (float) detail;
			for(int i=0; i<detail; i++) {
				Vector3 point = v1 * Mathf.Cos (theta * i) + v2 * Mathf.Sin (theta * i);
				fromPoints [i] = from + point;
				toPoints [i] = to + point;
			}

			int index = 0;
			int[] triangles = new int[detail * 2 * 3];
			Vector3[] vertices = new Vector3[detail * 2];
			for (int i = 0; i < detail; i++) {
				if (i == detail - 1) {
					triangles [index] = detail + i;
					triangles [index + 1] = 0;
					triangles [index + 2] = i;
					triangles [index + 3] = detail;
					triangles [index + 4] = 0;
					triangles [index + 5] = detail + i;
				} else {
					triangles [index] = detail + i;
					triangles [index + 1] = i + 1;
					triangles [index + 2] = i;
					triangles [index + 3] = detail + i + 1;
					triangles [index + 4] = i + 1;
					triangles [index + 5] = detail + i;
				}
				index += 6;
			}

			fromPoints.CopyTo (vertices, 0);
			toPoints.CopyTo (vertices, detail);
			pmesh.Add (vertices, triangles);
			return pmesh;
		}

		public void SetMesh() {
//			Vector3 center = (from + to) / 2;
//			gameObject.transform.localPosition = center;
			PMesh pmesh = PLineSimple.Mesh (from, to, weight, detail);
			Mesh mesh = gameObject.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
			mesh.vertices = pmesh.vertices;
			mesh.triangles = pmesh.triangles;
			mesh.RecalculateNormals ();

			Color[] colors = new Color[mesh.vertices.Length];
			for (int i=0; i<colors.Length; i=i+1) {
				Color color = Color.HSVToRGB (Random.value, 0.5f, 1.0f);
				colors [i] = color;
			}
			mesh.colors = colors;
		}

		public void destory() {
			Object.Destroy (this.gameObject);
		}
	}
}

