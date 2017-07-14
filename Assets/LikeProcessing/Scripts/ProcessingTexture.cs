using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace LikeProcessing
{

	public class ProcessingTexture
	{
		public Texture2D texture;
		IntPtr processingTexturePtr;
		string fullyQualifiedClassName;

//		[DllImport ("RenderingPlugin")]
		private static extern IntPtr CreateProcessingTexture (IntPtr texturePtr, string fullyQualifiedClassName, int width, int height);

//		[DllImport ("RenderingPlugin")]
		private static extern void ProcessingTextureUpdate (IntPtr processingTexturePtr);


		public ProcessingTexture (string _fullyQualifiedClassName, int width, int height)
		{
			texture = new Texture2D (width, height, TextureFormat.ARGB32, false);
			texture.filterMode = FilterMode.Trilinear;
			texture.Apply ();

			fullyQualifiedClassName = _fullyQualifiedClassName;

			processingTexturePtr = CreateProcessingTexture (texture.GetNativeTexturePtr (), fullyQualifiedClassName, texture.width, texture.height);
			if (processingTexturePtr == IntPtr.Zero)
				Debug.LogError (fullyQualifiedClassName + " not found");
		}


		public void update()
		{
			if (processingTexturePtr != IntPtr.Zero) {
				ProcessingTextureUpdate (processingTexturePtr);
			}
		}
	}
}
