using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;


public class Cgal : MonoBehaviour {

	[DllImport ("sample")]
	private static extern int Aaaa (int width);

	[DllImport ("sample")]
	private static extern int Bbbb (ref IntPtr ptrResultVerts, ref int resultVertLength);

	// Use this for initialization
	void Start () {
		Debug.Log(Aaaa (400));

		IntPtr ptrResultVerts = IntPtr.Zero;
		int resultVertLength = 0;
		Bbbb (ref ptrResultVerts, ref resultVertLength);
		Debug.Log (resultVertLength);
		float[] resultVertices = null;

		// Load the results into a managed array.
		resultVertices = new float[resultVertLength];
		Marshal.Copy(ptrResultVerts
			, resultVertices
			, 0
			, resultVertLength);

		Debug.Log(resultVertices[0]);
		/*
	      * WARNING!!!! IMPORTANT!!!
	      * In this example the plugin created an array allocated
	      * in unmanged memory.  The plugin will need to provide a
	      * means to free the memory.
	      */

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
