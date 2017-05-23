using UnityEngine;

namespace LikeProcessing
{
	public class PLineSimple
	{
		float weight;
		int detail;
		public GameObject gameObject;
		Vector3 from, to;
		Vector3[] vertices;
		int[] triangles;

		public PLineSimple (Vector3 _from, Vector3 _to, float _weight = 0.1f, int _detail = 6)
		{
			gameObject = new GameObject();
			gameObject.name = "PLineSimple";
			gameObject.AddComponent<MeshFilter> ();
//			gameObject.AddComponent<MeshRenderer> ().material = new Material(Shader.Find("LikeProcessing/VertexColor"));
			gameObject.AddComponent<MeshRenderer> ().material = PSketch.material;
			weight = _weight;
			detail = _detail;
			from = _from;
			to = _to;
			SetMesh ();
		}

		public void SetMesh() {
			Vector3 center = (from + to) / 2;
			gameObject.transform.position = center;
			Vector3 _from = from - center;
			Vector3 _to = to - center;
			Vector3 fromTo = _to - _from;
			fromTo.Normalize ();
			Vector3 randv = PSketch.randomVector ();
			while (Vector3.Angle(fromTo, randv) < 5.0f) {
				randv = PSketch.randomVector();
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
				fromPoints [i] = _from + point;
				toPoints [i] = _to + point;
			}
			triangles = new int[detail * 2 * 3];
			int index = 0;
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
			vertices = new Vector3[detail * 2];
			fromPoints.CopyTo (vertices, 0);
			toPoints.CopyTo (vertices, detail);

			Mesh mesh = gameObject.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
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

