using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LikeProcessing;

namespace LikeProcessig.Examples {

	public class ProcessingToUnitySketch : PSketch {

		ProcessingTexture pTexture;

		MyOfApp ofApp;

		// Use this for initialization
		IEnumerator Start () {
			GameObject gobj = GameObject.Find ("Sphere");
			pTexture = new ProcessingTexture ("p2dinunity/Example", 1024, 1024);
			gobj.GetComponent<Renderer> ().material.mainTexture = pTexture.texture;

			ofApp = new MyOfApp ();

			yield return StartPluginRenderLoop ();
		}
		
		// Update is called once per frame
		void Update () {
			pTexture.update ();
		}

		class MyOfApp : OFApp {

			public MyOfApp() {
				OfRenderDelegate renderDelegate = new OfRenderDelegate (this.Render);
				OFApp.RegistRenderFunc (renderDelegate);
			}

			protected void Render() {
				uofDrawBox (100);
			}
		}
	}


}
