using UnityEngine;
using UnityEngine.VR;

namespace LikeProcessing.Examples
{
	
    public class HelloWorld : PSketch
    {
		public int hoge;
        // Use this for initialization
		PGeodesicDome picosahedral;

        void Start()
        {
            VRSettings.enabled = false;
            background(Color.black);
//            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//            obj.transform.localScale = new Vector3(1, 1, 1);
//			obj.transform.position = new Vector3 (0,1,0);
            //            blur();
			picosahedral = new PGeodesicDome(3);
			picosahedral.gameObject.transform.position += new Vector3 (0, 1, 0);
            bloom();
//			valueTypeArrayTest ();
//			PVertices pvertices = new PVertices();
//			pvertices.BeginShape ();
//			pvertices.AddVertex (new Vector3(1,2,3));
//			pvertices.EndShape ();
//			Vector3[] vertices = pvertices.Vertices ();
        }

        // Update is called once per frame
        void Update()
        {
//            if (Input.GetKeyDown(KeyCode.R))
//            {
//				this.ToggleRecording ();
//            }
//			this.Record ();
        }

		void valueTypeArrayTest() {
			Vector3[] vecs = { new Vector3(0,0,0), new Vector3(1,1,1) };
			Vector3[] vecs2 = vecs;
			vecs2 [0] = new Vector3 (2,2,2);
			Debug.Log (vecs[0]);
			Debug.Log (vecs2[0]);
		}
    }
}