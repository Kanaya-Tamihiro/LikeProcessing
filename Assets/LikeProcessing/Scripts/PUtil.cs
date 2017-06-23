using System;
using UnityEngine;

namespace LikeProcessing
{
	public class PUtil
	{
		public PUtil ()
		{
		}

		public static string VerticesToString(Vector3[] vertices) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			foreach (Vector3 v in vertices)
			{
				sb.Append(v.x).Append(" ").Append(v.y).Append(" ").Append(v.z).Append("|");
			}
			if (sb.Length > 0) // remove last "|"
				sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}

		public static string VerticesToString(Vector4[] vertices) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			foreach (Vector3 v in vertices)
			{
				sb.Append(v.x).Append(" ").Append(v.y).Append(" ").Append(v.z).Append("|");
			}
			if (sb.Length > 0) // remove last "|"
				sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}

		public static string IndecisToString(int[] indeces) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			foreach (int index in indeces)
			{
				sb.Append(index).Append(",");
			}
			if (sb.Length > 0) // remove last ","
				sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}

		// sort left to right , bottom to up, near to far order
		public static Vector3[] divideVector3 (Vector3 v1, Vector3 v2, int detail) {
			Vector3[] vecs = new Vector3[detail + 1];
			bool invert = false;
			float defference = 0.000001f;
			if ((v1.x - v2.x) > defference) {
				invert = true; 
			}
			else if ((v1.y - v2.y) > defference) {
				invert = true;
			}
			else if ((v1.z - v2.z) > defference) {
				invert = true;
			}
			Vector3 va, vb;
			if (invert) {
				va = v2;
				vb = v1;
			} else {
				va = v1;
				vb = v2;
			}
			Vector3 deltaV = (vb - va) / detail;
			vecs[0] = va;
			for(int i=1; i<detail; i++) {
				vecs [i] = va + deltaV * i;
			}
			vecs [detail] = vb;
			if (invert) {
				Array.Reverse (vecs);
			}
			return vecs;
		}
	}
}

