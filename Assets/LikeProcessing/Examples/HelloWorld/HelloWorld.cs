using UnityEngine;

namespace LikeProcessing.Examples
{
	
    public class HelloWorld : PSketch
    {
		public int hoge;
        // Use this for initialization
        void Start()
        {
            background(Color.black);
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.localScale = new Vector3(1, 1, 1);
			obj.transform.position = new Vector3 (0,1,0);
            //            blur();
            bloom();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log(111);
                this.record();
            }
        }
    }
}