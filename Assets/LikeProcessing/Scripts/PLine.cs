using UnityEngine;

namespace LikeProcessing
{
	public class PLine
	{
		float weight;

		public GameObject line, cylinder, sphereFrom, sphereTo;

		public PLine (GameObject parent) : this (parent, Vector3.zero, Vector3.zero) {}

		public PLine(GameObject parent, float weight) : this (parent, Vector3.zero, Vector3.zero, weight) {}

		public PLine (GameObject parent, Vector3 from, Vector3 to, float weight = 1.0f)
		{
			line = new GameObject();
			line.name = "line";
			line.transform.SetParent (parent.transform);
			// We make a offset gameobject to counteract the default cylindermesh pivot/origin being in the middle
			cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			cylinder.name = "line.cylinder";
			cylinder.transform.SetParent(line.transform);
			// Offset the cylinder so that the pivot/origin is at the bottom in relation to the outer ring gameobject.
			cylinder.transform.localPosition = new Vector3(0f, 1f, 0f);
			cylinder.transform.localScale = new Vector3(weight, 1f, weight);
			this.weight = weight;
			MeshRenderer meshRenderer = cylinder.GetComponent<MeshRenderer>();
			Material material = new Material(Shader.Find("Standard"));
			material.color = new Color(0.6F, 1.0F, 0.2F);
			meshRenderer.material = material;

			sphereFrom = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			sphereFrom.name = "sphere.from";
			sphereFrom.transform.SetParent (line.transform);

			sphereTo = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			sphereTo.name = "sphere.to";
			sphereTo.transform.SetParent (line.transform);

			update (from, to);
		}

		public void setWeight(float weight) {
			cylinder.transform.localScale = new Vector3(weight, 1f, weight);
		}

		public void setColor(Color color) {
			cylinder.GetComponent<Renderer> ().material.color = color;
		}

		public void update(Vector3 from, Vector3 to) {
			line.transform.localPosition = from;

			float cylinderDistance = 0.5f * Vector3.Distance(from, to);
			line.transform.localScale = new Vector3(line.transform.localScale.x, cylinderDistance, line.transform.localScale.z);
			sphereFrom.transform.localScale = new Vector3 (weight, weight/cylinderDistance, weight);
			sphereTo.transform.localPosition = new Vector3 (0, 2, 0);
			sphereTo.transform.localScale = new Vector3 (weight, weight/cylinderDistance, weight);
			// Make the cylinder look at the main point.
			// Since the cylinder is pointing up(y) and the forward is z, we need to offset by 90 degrees.
//			line.transform.LookAt(line.transform.TransformPoint(to), line.transform.TransformPoint(Vector3.up));
			line.transform.rotation = Quaternion.LookRotation(to-from, line.transform.up);
			line.transform.localRotation *= Quaternion.Euler(90, 0, 0);
		}

		public void destory() {
			Object.Destroy (this.line);
		}
	}
}

