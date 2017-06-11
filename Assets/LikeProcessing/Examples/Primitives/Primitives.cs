using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LikeProcessing;
using LikeProcessing.PMetaball;

namespace LikeProcessig.Examples {
	
	public class Primitives : PSketch {

		PGeodesicDomeImproved pgeodesicDomeImproved;
		PGeodesicDome pgeodesicDome;
		PLineSimple plineSimple;
		PMetaball pmetaball;

		float deltaTime = 0f;

		Core core1, core2;

		void Start () {
			lightObj.GetComponent<Light> ().intensity = 0.8f;

//			pgeodesicDomeImproved = new PGeodesicDomeImproved(3, false);
//			pgeodesicDomeImproved.gameObject.transform.position += new Vector3 (-2, 1, 0);
//			pgeodesicDomeImproved.gameObject.GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Standard"));
//
//			pgeodesicDome = new PGeodesicDome(3);
//			pgeodesicDome.gameObject.transform.position += new Vector3 (-2, 3, 0);
//			pgeodesicDome.gameObject.GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Standard"));
//
//			plineSimple = new PLineSimple(new Vector3(0, 0, 0), new Vector3(1, 2, 0));
//			plineSimple.gameObject.GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Standard"));

			pmetaball = new PMetaball();
			pmetaball.detail = 20;
			pmetaball.size = 3.0f;
			pmetaball.isoPower = .3f;
			pmetaball.isoValuesAddictive = true;
			pmetaball.isHardEdge = false;
			pmetaball.gameObject.transform.position = new Vector3 (0,0,0);
//			pmetaball.SetUpLattice ();
//			core1 = new Core (pmetaball, new Vector3 (3, 3, 3));
//			pmetaball.AddCore (core1);
//			core2 = new Core (pmetaball, new Vector3 (3, 0, 3));
//			pmetaball.AddCore (core2);
//			pmetaball.AddCore (new PMetaball.CoreLine(new Vector3 (-1.0f,-1.0f,0), new Vector3 (1.0f,1.0f,0)));
//			pmetaball.AddMetaball (new Vector3 (0.25f,1,0));
//			pmetaball.Update ();
		}

//		void Update () {
//			deltaTime += Time.deltaTime;
////			if (deltaTime >= 1.0f/10.0f)
////			{
////				deltaTime = 0;
////				pmetaball.isoPower -= 0.001f;
////				pmetaball.Update ();
////			}
//			float angle = Mathf.PI/4 * Time.time;
//			if (Input.GetKey (KeyCode.RightArrow)) {
//				pmetaball.MoveCore(core1, (new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), Mathf.Cos (angle))) * 2.5f);
//				pmetaball.MoveCore(core2, (new Vector3 (Mathf.Sin(angle), Mathf.Cos(angle), Mathf.Cos(angle) * Mathf.Sin(angle))) * 1.5f);
//				pmetaball.Update ();
//			}
//
//		}

		void Update() {
			if (Input.GetMouseButtonDown (0)) {
				Vector3 v3 = Input.mousePosition;
				v3.z = 5.0f;
				v3 = Camera.main.ScreenToWorldPoint(v3);
				pmetaball.AddCore (new Core(pmetaball, v3));
				pmetaball.Update ();
			}

		}
	}

}
