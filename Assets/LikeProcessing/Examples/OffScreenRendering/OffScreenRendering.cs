using UnityEngine;
using System.Collections;

namespace LikeProcessing.Examples
{
    public class OffScreenRendering : PSketch
    {
        PGraphics graphics;
        GameObject cube, centerCube;
        GameObject screen;

        // Use this for initialization
        void Start()
        {
            graphics = PGraphics.createGraphics(3, 3);
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale *= 1;
            cube.layer = graphics.layer;

            screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
            screen.transform.localScale = new Vector3(3, 3, 3);
            graphics.renderTo(screen);
        }

        // Update is called once per frame
        void Update()
        {
            cube.transform.Rotate(1, 2, 0);
            screen.transform.Rotate(2,1,0);
        }
    }

}