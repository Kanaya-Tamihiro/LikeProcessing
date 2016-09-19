using UnityEngine;
using System.Collections;
using LikeProcessing;
using UnityStandardAssets.ImageEffects;

public class Sketch : PSketch {

    PGraphics graphics;
    GameObject cube;
    GameObject sphere;
    GameObject line;
    GameObject plane;

    Material rtMterial;

	// Use this for initialization
	void Start () {
        graphics = PGraphics.createGraphics(512,512);
        BlurOptimized blurComp = graphics.cameraObj.AddComponent<BlurOptimized>();
        blurComp.blurShader = Shader.Find("Hidden/FastBlur");

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "cube";
        cube.transform.localScale *= 100;
        cube.layer = graphics.layer;

        //sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //sphere.transform.localScale *= 300;
        //graphics.renderTo(sphere);

        line = new GameObject("line");
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.SetWidth(100f,100f);
        lr.SetVertexCount(3);
        lr.SetPosition(0, new Vector3(0,-200,0));
        lr.SetPosition(1, new Vector3(100, 0, 0));
        lr.SetPosition(2, new Vector3(0,200,0));
        //graphics.renderTo(line);

        //Shader shader = Shader.Find("Unlit/Texture");
        Shader shader = Shader.Find("Sprites/Default");
        rtMterial = new Material(shader);
        rtMterial.name = "New Render Texture";
        //graphics.renderTexture.Create();
        //RenderTexture.active = graphics.renderTexture;
        //rtMterial.mainTexture = graphics.renderTexture;
        rtMterial.SetTexture("_MainTex", graphics.renderTexture);
        lr.material = rtMterial;

        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "plane";
        plane.transform.localScale *= 100;
        plane.transform.Rotate(90,0,0);
        plane.transform.Translate(-100,0,0);
        //graphics.renderTo(plane);
        plane.GetComponent<MeshRenderer>().material = rtMterial;
    }
	
	// Update is called once per frame
	void Update () {
        cube.transform.Rotate(1,2,0);
	}
}
