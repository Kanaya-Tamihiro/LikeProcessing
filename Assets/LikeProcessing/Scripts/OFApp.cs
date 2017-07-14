using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LikeProcessing
{
	public class OFApp
	{
		[DllImport ("RenderingPlugin")]
		private static extern void RegistOfRenderMethod (System.IntPtr ptr);

		[DllImport ("RenderingPlugin")]
		protected static extern void uofDrawBox (float width);

		[DllImport ("RenderingPlugin")]
		protected static extern void uofDrawLine (float x1, float y1, float z1, float x2, float y2, float z2);
		[DllImport ("RenderingPlugin")]
		protected static extern void uofFill ();
		[DllImport ("RenderingPlugin")]
		protected static extern void uofNoFill ();

		[DllImport ("RenderingPlugin")]
		protected static extern void uofSetColor (int r, int g, int b, int a);

		[DllImport ("RenderingPlugin")]
		protected static extern void uofSetLineWidth (float lineWidth);

		protected delegate void OfRenderDelegate ();

		protected static void RegistRenderFunc(OfRenderDelegate renderDelegate) {
			System.IntPtr renderDelegatePtr = Marshal.GetFunctionPointerForDelegate (renderDelegate);
			RegistOfRenderMethod (renderDelegatePtr);
		}

		public OFApp ()
		{
			Setup ();
		}

		protected void Setup() {
		}

		protected void Render() {
		}
	}
}

