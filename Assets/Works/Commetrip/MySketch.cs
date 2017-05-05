using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LikeProcessing;

public class MySketch : PSketch {

    public string myName;
    CommetsGraphics graphics;
    LineGraphics lineGraphics;
    static Texture2D lineTexturex;

    void Start () {
        setupCamera();
        background(Color.black);
        
        //lineGraphics = new LineGraphics(300,300);
        //lineGraphics.Start();

        //RenderTexture.active = lineGraphics.renderTexture;
        //Texture2D texture = new Texture2D(300, 300, TextureFormat.ARGB32, false, false);
        //texture.ReadPixels(new Rect(0, 0, 300, 300), 0, 0);
        //texture.Apply();
        //lineTexturex = texture;

        graphics = new CommetsGraphics(6, 6, lineGraphics);
        graphics.Start();

        //GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //obj.name = "hogehoge";
        //obj.transform.localScale = new Vector3(300, 300, 300);
        //graphics.renderTo(obj);
        //lineGraphics.renderTo(obj);
        //blur();
        
    }
	
	void Update () {
        //cameraRotateWithMouse();
        graphics.Update();    
    }

    class LineGraphics : PGraphics
    {
        GameObject line;

        public LineGraphics(int width, int height) : base(width, height)
        {

        }

        public void Start()
        {
            blur();
            line = new GameObject("Line");
            line.layer = this.layer;
            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.SetColors(Color.white, Color.white);
            lineRenderer.SetVertexCount(2);
            lineRenderer.SetPosition(0, new Vector3(0, -1.2f, 0));
            lineRenderer.SetPosition(1, new Vector3(0, 1.2f, 0));
            lineRenderer.SetWidth(1f, 1f);
        }
    }

    class CommetsGraphics : PGraphics
    {
        int commetcnt = 500;
        Commet[] commets;
        int commetTrackLength = 30;

        int starcnt = 4;
        Vector3[] stars;
        int activeStarIdx = 0;
        Vector3 activeStar;

        float maxCommetSpeed = 0.2f;

        LineGraphics lineGraphics;

        public CommetsGraphics(int width, int height, LineGraphics lineGraphics) : base(width, height)
        {
            this.lineGraphics = lineGraphics;
        }

        public void Start()
        {
            //setupCamera(this.cameraObj.GetComponent<Camera>(), this.height, true);
            commets = new Commet[commetcnt];
            for (int i = 0; i < commetcnt; i++)
            {
                Commet commet = new Commet(this, i);
                commets[i] = commet;
            }

            stars = new Vector3[starcnt];
            float range = Screen.width / 200.0f;
            for (int i = 0; i < starcnt; i++)
            {
                Vector3 v = new Vector3(Random.Range(-range, range),
                                        Random.Range(-range, range),
                                        Random.Range(-range, range));
                stars[i] = v;
            }
            activeStar = stars[activeStarIdx];
        }

        public void Update()
        {
            //cameraRotateWithMouse();
            foreach (Commet c in commets) c.move();
            foreach (Commet c in commets) c.draw();

            if (Time.frameCount % 20 == 0)
            {
                activeStarIdx = (activeStarIdx + 1) % starcnt;
                activeStar = stars[activeStarIdx];
            }
        }

        class Commet
        {
            CommetsGraphics graphics;
            GameObject obj;
            Vector3 dir = new Vector3(0, 0, 0);
            Vector3 loc;
            List<Vector3> preLocs = new List<Vector3>();

            public Commet(CommetsGraphics graphics, int index)
            {
                this.graphics = graphics;
                obj = new GameObject("commet" + index);
                //obj.layer = graphics.layer;
                LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
                //lineRenderer.material = new Material(Shader.Find("Standard"));
                Material lineTexture = Resources.Load("LazerTexture", typeof(Material)) as Material;
                //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
                lineRenderer.material = lineTexture;
                //lineRenderer.material.mainTexture = graphics.lineGraphics.renderTexture;
                //lineRenderer.material = new Material(Shader.Find("Standard"));
                float hue = Random.Range(0f, 1f);
                //lineRenderer.SetColors(Color.HSVToRGB(hue,1f,1f), Color.HSVToRGB(hue,1f,0.2f));
                //lineRenderer.SetColors(Color.white, Color.red);
                //lineRenderer.SetVertexCount(2);
                //lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
                //lineRenderer.SetPosition(1, new Vector3(-200, 200, 0));
                lineRenderer.SetWidth(0.1f, 0.1f);
				loc = new Vector3(Random.Range(-Screen.width / 100.0f, Screen.width / 100.0f),
					Random.Range(-Screen.height / 100.0f, Screen.height / 100.0f),
					Random.Range(-Screen.height / 100.0f, Screen.height / 100.0f));
                //float scale = Random.Range(3, 10);
                //obj.transform.localScale = new Vector3(scale, scale, scale);
            }

            public void move()
            {
                Vector3 gravity = (graphics.activeStar - loc).normalized;
                Vector3 newDir = Vector3.ClampMagnitude(dir + gravity, graphics.maxCommetSpeed);
                dir = newDir;
                preLocs.Insert(0, loc);
                loc = loc + newDir;
                if (preLocs.Count > graphics.commetTrackLength)
                {
                    preLocs.RemoveAt(graphics.commetTrackLength);
                }
            }

            public void draw()
            {
                if (preLocs.Count <= 1) return;
                LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();
                lineRenderer.SetVertexCount(preLocs.Count);
                lineRenderer.SetPositions(preLocs.ToArray());
            }
        }
    }

    
}
