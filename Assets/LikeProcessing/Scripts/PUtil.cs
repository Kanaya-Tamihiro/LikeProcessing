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
	}
}

