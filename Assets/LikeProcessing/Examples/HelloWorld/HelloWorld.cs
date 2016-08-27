using UnityEngine;

namespace LikeProcessing.Examples
{
    public class HelloWorld : PSketch
    {

        // Use this for initialization
        void Start()
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.localScale = new Vector3(200, 200, 200);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}