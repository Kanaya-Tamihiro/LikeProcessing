using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LikeProcessing;

public class RibbonsSketch : PSketch
{

    int ribbonCnt = 3;
    int stickLen = Screen.width / 4;

    Ribbon[] ribbons;

    GameObject ctrlRight;

    void Start()
    {
        //setupCamera (true);
        background(Color.black);
        //Application.targetFrameRate = 20;

        Vector3 stickHead = getStickHead();
        ribbons = new Ribbon[ribbonCnt];
        for (int i = 0; i < ribbonCnt; i++)
        {
            Ribbon ribbon = new Ribbon(stickHead);
            ribbons[i] = ribbon;
        }

        //		sphere = new GameObject ();
        ////		sphere.transform.localScale *= 100;
        //		sphere.AddComponent<MeshFilter>();
        //		sphere.AddComponent<MeshRenderer>();
        //		Mesh mesh = sphere.GetComponent<MeshFilter>().mesh;
        //		mesh.Clear();
        //		mesh.vertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 100, 0), new Vector3(100, 100, 0)};
        //		mesh.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};
        //		mesh.triangles = new int[] {0, 1, 2};
    }

    //GameObject sphere;

    Vector3 getStickHead()
    {
        float alpha = Mathf.PI - (1.0f * Input.mousePosition.x / Screen.width) * Mathf.PI;
        float beta = (1.0f * Input.mousePosition.y / Screen.height) * Mathf.PI;
        Vector3 stickHead = new Vector3(
            Mathf.Cos(alpha) * stickLen,
            Mathf.Sin(alpha) * Mathf.Sin(beta - Mathf.PI / 2) * stickLen,
            1.0f * Mathf.Sin(alpha) * Mathf.Sin(beta) * stickLen
        );
        return stickHead;
        //if (ctrlRight == null)
        //    ctrlRight = GameObject.Find("Controller (right)");
        //if (ctrlRight == null)
        //    return Vector3.zero;
        //else
        //{
        //    Vector3 head = new Vector3(0, 0, stickLen);
        //    //Quaternion quat = new Quaternion(ctrlRight.transform.rotation.x, ctrlRight.transform.rotation.y, 0, ctrlRight.transform.rotation.w);
        //    return ctrlRight.transform.rotation * head;
        //}
    }

    void Update()
    {
        Vector3 stickHead = getStickHead();
        foreach (Ribbon ribbon in ribbons)
        {
            ribbon.update(stickHead);
        }
        //		sphere.transform.position = stickHead;

    }

    public class Ribbon
    {
        public int particleCnt = 60;
        public Color color = Color.HSVToRGB(Random.value, 1, 1);
        public float rightDownRandomness = 0.2f;
        public float radiusMax = 12;   // maximum width of ribbon
        public float radiusDivide = 10;   // distance between current and next point / this = radius for first half of the ribbon
        public Vector3 gravity = new Vector3(0, .01f, 0); // gravity applied to each particle
        public float friction = 1.1f; // friction applied to the gravity of each particle
        public int maxDistance = 40;   // if the distance between particles is larger than this the drag comes into effect
        public float drag = 2.5f;  // if distance goes above maxDistance - the points begin to grag. high numbers = less drag
        public float dragFlare = .015f;  // degree to which the drag makes the ribbon flare out
        Particle[] particles;

        float deltaTime = 0f;
        float frameRateTime = 1.0f / 20;

        GameObject lineObj;

        public Ribbon(Vector3 stickHead)
        {
            particles = new Particle[particleCnt];
            for (int i = 0; i < particleCnt; i++)
            {
                Particle p = new Particle(this, stickHead);
                particles[i] = p;
            }

            lineObj = new GameObject("ribbon");
            //			lineObj.AddComponent<LineRenderer>();
            lineObj.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = lineObj.AddComponent<MeshRenderer>();
            //			Material material = new Material(Shader.Find("Standard"));
            //			material.EnableKeyword("_EMISSION");
            //			material.SetColor("_EmissionColor", color);
            Material material = new Material(Shader.Find("Particles/Additive"));
            material.SetColor("_TintColor", color);
            meshRenderer.sharedMaterial = material;
        }

        public void update(Vector3 stickHead)
        {
            deltaTime += Time.deltaTime;
            if (deltaTime >= frameRateTime)
            {
                deltaTime = 0;
                move(stickHead);
                draw();
            }
        }

        void move(Vector3 stickHead)
        {
            Particle newParticle = particles[0];
            newParticle.reset(stickHead);
            for (int i = 1; i < particleCnt; i++)
            {
                particles[i - 1] = particles[i];
            }
            particles[particleCnt - 1] = newParticle;

            for (int i = 1; i < particleCnt - 1; i++)
            {
                Particle p = particles[i];
                Particle back = particles[i - 1];
                Particle head = particles[i + 1];
                p.move(back, head, i);
            }
        }

        void draw()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            Mesh mesh = lineObj.GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            //			lineObj.vertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 100, 0), new Vector3(100, 100, 0)};
            //lineObj.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};

            for (int i = particleCnt - 1; i > 1; i--)
            {
                Particle p = particles[i];
                Particle pback = particles[i - 1];
                //				if (i == particleCnt - 1)
                //					vertices.Add (p.loc);
                //				vertices.Add (pback.loc);
                int len = vertices.Count;
                /*
				vertices.Add (p.rightHead);
				vertices.Add (p.leftHead);
				vertices.Add (pback.leftHead);
				vertices.Add (pback.rightHead);
				int[] indeces = { len, len + 1, len + 2, len + 2, len + 3, len };
				triangles.AddRange (indeces);
				*/

                List<Vector3> leftBezierPoints = bezierPoints(p.leftHead, p.left, pback.leftHead, pback.leftHead);
                List<Vector3> rightBezierPoints = bezierPoints(p.rightHead, p.right, pback.rightHead, pback.rightHead);

                vertices.AddRange(rightBezierPoints);
                vertices.AddRange(leftBezierPoints);
                int detail = leftBezierPoints.Count;
                for (int j = 0; j < leftBezierPoints.Count - 1; j++)
                {
                    int[] indeces = { len, len + detail, len + 1, len + detail, len + detail + 1, len + 1 };
                    triangles.AddRange(indeces);
                    len++;
                }

            }
            //			lineObj.triangles = new int[] {0, 1, 2};
            //			LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer> ();
            //			lineRenderer.SetWidth (20,20);
            //			lineRenderer.SetVertexCount (vertices.Count);
            ////			lineRenderer.SetPosition (0, new Vector3(0,0,0));
            ////			lineRenderer.SetPosition (1, new Vector3(0,200,0));
            //			lineRenderer.SetPositions(vertices.ToArray());
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
        }

        List<Vector3> bezierPoints(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            float x1 = v1.x, y1 = v1.y, z1 = v1.z;
            float x2 = v2.x, y2 = v2.y, z2 = v2.z;
            float x3 = v3.x, y3 = v3.y, z3 = v3.z;
            float x4 = v4.x, y4 = v4.y, z4 = v4.z;

            int bezierDetail = 20;
            float f = 1.0f / bezierDetail;
            float ff = f * f;
            float fff = ff * f;

            Matrix4x4 bezierMatrix = Matrix4x4.identity;
            bezierMatrix.SetRow(0, new Vector4(0, 0, 0, 1));
            bezierMatrix.SetRow(1, new Vector4(fff, ff, f, 0));
            bezierMatrix.SetRow(2, new Vector4(6 * fff, 2 * ff, 0, 0));
            bezierMatrix.SetRow(3, new Vector4(6 * fff, 0, 0, 0));

            Matrix4x4 bezierBasicMatrix = Matrix4x4.identity;
            bezierBasicMatrix.SetRow(0, new Vector4(-1, 3, -3, 1));
            bezierBasicMatrix.SetRow(1, new Vector4(3, -6, 3, 0));
            bezierBasicMatrix.SetRow(2, new Vector4(-3, 3, 0, 0));
            bezierBasicMatrix.SetRow(3, new Vector4(1, 0, 0, 0));

            Matrix4x4 draw = bezierMatrix * bezierBasicMatrix;

            float xplot1 = draw[1, 0] * x1 + draw[1, 1] * x2 + draw[1, 2] * x3 + draw[1, 3] * x4;
            float xplot2 = draw[2, 0] * x1 + draw[2, 1] * x2 + draw[2, 2] * x3 + draw[2, 3] * x4;
            float xplot3 = draw[3, 0] * x1 + draw[3, 1] * x2 + draw[3, 2] * x3 + draw[3, 3] * x4;

            float yplot1 = draw[1, 0] * y1 + draw[1, 1] * y2 + draw[1, 2] * y3 + draw[1, 3] * y4;
            float yplot2 = draw[2, 0] * y1 + draw[2, 1] * y2 + draw[2, 2] * y3 + draw[2, 3] * y4;
            float yplot3 = draw[3, 0] * y1 + draw[3, 1] * y2 + draw[3, 2] * y3 + draw[3, 3] * y4;

            float zplot1 = draw[1, 0] * z1 + draw[1, 1] * z2 + draw[1, 2] * z3 + draw[1, 3] * z4;
            float zplot2 = draw[2, 0] * z1 + draw[2, 1] * z2 + draw[2, 2] * z3 + draw[2, 3] * z4;
            float zplot3 = draw[3, 0] * z1 + draw[3, 1] * z2 + draw[3, 2] * z3 + draw[3, 3] * z4;

            List<Vector3> vertices = new List<Vector3>();
            vertices.Add(new Vector3(x1, y1, z1));

            for (int j = 0; j < bezierDetail; j++)
            {
                x1 += xplot1; xplot1 += xplot2; xplot2 += xplot3;
                y1 += yplot1; yplot1 += yplot2; yplot2 += yplot3;
                z1 += zplot1; zplot1 += zplot2; zplot2 += zplot3;
                vertices.Add(new Vector3(x1, y1, z1));
            }

            return vertices;
        }
    }

    public class Particle
    {
        public float randomness = .1f;
        public Vector3 loc = Vector3.zero;
        public Vector3 speed = Vector3.zero;
        public Vector3 left = Vector3.zero;
        public Vector3 right = Vector3.zero;
        public Vector3 leftHead = Vector3.zero;
        public Vector3 rightHead = Vector3.zero;
        public float radius;
        Ribbon ribbon;

        //GameObject obj, obj2;

        public Particle(Ribbon ribbon, Vector3 loc)
        {
            this.ribbon = ribbon;

            //obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //obj.transform.localScale *= 10;
            //obj2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //obj2.transform.localScale *= 10;
            reset(loc);
        }

        public void reset(Vector3 head)
        {
            loc = left = right = leftHead = rightHead = head;
            //			loc = head;
            //			left = head;
            //			right = head;
            //			leftHead = head;
            //			rightHead = head;
            speed = Vector3.zero;
            //obj.transform.position = left;
            //obj2.transform.position = right;
        }

        public void move(Particle pback, Particle phead, int index)
        {
            Vector3 direction = (phead.loc + loc) / 2 - (pback.loc + loc) / 2;
            float radian = Mathf.Atan2(direction.y, direction.x);
            //			if (index == ribbon.particleCnt - 3) {
            //				Debug.Log (direction);
            //			}
            float distance = direction.magnitude;
            if (distance > ribbon.maxDistance)
            {
                Vector3 preloc = loc;
                direction = direction.normalized * (ribbon.maxDistance / ribbon.drag);
                loc += direction;
                speed += (loc - preloc) * ribbon.dragFlare;
            }
            speed -= ribbon.gravity;
            speed *= ribbon.friction;
            loc += speed;
            Vector3 randomnessv = new Vector3(
                Random.value * ribbon.rightDownRandomness,
                Random.value * ribbon.rightDownRandomness,
                0
            );
            loc += randomnessv;
            Vector3 randomnessv2 = new Vector3(
                (randomness / 2 - Random.value * randomness) * distance,
                (randomness / 2 - Random.value * randomness) * distance,
                0
            );
            loc += randomnessv2;
            if (index > ribbon.particleCnt / 2)
            {
                radius = distance / ribbon.radiusDivide;
            }
            else
            {
                radius = phead.radius * .9f;
            }

            radius = Mathf.Min(radius, ribbon.radiusMax);

            if (index == ribbon.particleCnt - 2 || index == 1)
            {
                if (radius > 1)
                    radius = 1;
            }
            //			radius = 50;
            left = loc + new Vector3(
                Mathf.Cos(radian + (Mathf.PI / 2 * 3)) * radius,
                Mathf.Sin(radian + (Mathf.PI / 2 * 3)) * radius,
                0
            );
            right = loc + new Vector3(
                Mathf.Cos(radian + (Mathf.PI / 2)) * radius,
                Mathf.Sin(radian + (Mathf.PI / 2)) * radius,
                0
            );
            leftHead = (phead.left + left) / 2;
            rightHead = (phead.right + right) / 2;

            //obj.transform.position = leftHead;
            //obj2.transform.position = rightHead;
            return;
        }
    }


}
