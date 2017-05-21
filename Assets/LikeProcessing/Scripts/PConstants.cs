using System;
using UnityEngine;

namespace LikeProcessing
{
	public class PConstants
	{
		public static readonly float EPSILON = 0.0001f;

		public static Material material001;

		public static void Init() {
			material001 = Material.Instantiate(Resources.Load("Materials/Material001", typeof(Material)) as Material);
		}
	}
}

