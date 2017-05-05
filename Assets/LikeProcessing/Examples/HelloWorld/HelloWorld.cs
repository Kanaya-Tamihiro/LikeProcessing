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
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}