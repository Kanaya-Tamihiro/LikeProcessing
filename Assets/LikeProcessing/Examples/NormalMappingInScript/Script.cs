using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LikeProcessing;

public class Script : MonoBehaviour {

	Texture2D texture;
	int texSize = 256;

	// Use this for initialization
	void Start () {
		// Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
		texture = new Texture2D(texSize, texSize, TextureFormat.ARGB32, false);
//		for (int i = 0; i < texSize; i++) {
//			for (int j = 0; j < texSize; j++) {
////				float noise = (Mathf.PerlinNoise ((i+Time.time)*0.01f, (j+Time.time)*0.01f) -0.5f) *2;
//				//				Vector3 normal = new Vector3 (0, 0, 1);
//				//				Quaternion quat = Quaternion.Euler (0, 90*noise, 90*noise);
//				//				Vector3 v = quat * normal;
//				Vector3 v = PSketch.randomVector();
//				v = (v + new Vector3 (1, 1, 1)) / 2.0f;
//				texture.SetPixel (i, j, new Color(v.x, v.y, v.z));
//			}
//		}
		texture.Apply ();
		GetComponent<Renderer> ().material.SetTexture ("_BumpMap", texture);
		GetComponent<Renderer> ().material.EnableKeyword("_NORMALMAP");
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < texSize; i++) {
			for (int j = 0; j < texSize; j++) {
				float noise1 = (Mathf.PerlinNoise ((i+Time.time)*0.02f, (j+Time.time)*0.01f) -0.5f) *2;
				float noise2 = (Mathf.PerlinNoise ((i+Time.time+1)*0.02f, (j+Time.time+2)*0.03f) -0.5f) *2;
				Vector3 v = new Vector3 (0, 0, 1);
//				Quaternion quat = Quaternion.Euler (90, 90*noise, 90*noise);
				v = Quaternion.AngleAxis(10 * noise1, Vector3.right) * v;
				v = Quaternion.AngleAxis (10 * noise2, Vector3.up) * v;
//				Vector3 v = quat * normal;
//				Vector3 v = normal;
//				Vector3 v = PSketch.randomVector(noise1, noise2);
				v = (v + new Vector3 (1, 1, 1)) / 2.0f;
				texture.SetPixel (i, j, new Color(v.x, v.y, v.z));
			}
		}
		texture.Apply();
	}
}
