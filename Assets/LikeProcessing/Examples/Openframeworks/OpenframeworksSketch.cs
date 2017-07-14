using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LikeProcessing;

namespace LikeProcessig.Examples
{
	
	public class OpenframeworksSketch : PSketch
	{

		MyOfApp ofApp;

		IEnumerator Start () {
			ofApp = new MyOfApp ();
			yield return StartPluginRenderLoop ();
		}

		void Update () {
		}

		class MyOfApp : OFApp {

			public MyOfApp() {
				OfRenderDelegate renderDelegate = new OfRenderDelegate (this.Render);
				OFApp.RegistRenderFunc (renderDelegate);
			}

			protected void Render() {
				uofDrawBox (100);
				uofNoFill ();
				uofSetLineWidth (2);
				uofSetColor (252, 97, 97, 255);
//				uofDrawLine (Random.value * -200, Random.value * -200, Random.value * -200, Random.value * 200, Random.value * 200, Random.value * 200);

			}
		}
	}


}
