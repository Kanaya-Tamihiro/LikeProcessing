using UnityEngine;
using System.Collections.Generic;
using LikeProcessing;

public class TroidSketch : PSketch {

    //public MeshFilter meshFilter;
    //public MeshRenderer meshRenderer;

    int pointCount = 40;
    float radius = 0.5F;
    float latheRadius = 0.8F;
    int segmentCount = 50;

    GameObject troid;
    // Use this for initialization
    void Start()
    {
        troid = new GameObject("troid");
        troid.transform.localScale *= 100;
        MeshFilter meshFilter = troid.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = troid.AddComponent<MeshRenderer>();

        Mesh mesh = meshFilter.mesh;
        List<Vector3> edge = points();
        List<Vector3> vertices = new List<Vector3>();
        vertices.AddRange(edge);
        List<int> triangles = new List<int>();
        for (int seg = 0; seg < segmentCount; seg++)
        {
            edge = rotatedPoints(edge);
            vertices.AddRange(edge);
            for (int p = 0; p < pointCount; p++)
            {
                int s = p + seg * (pointCount + 1);
                int t = s + 1;
                int u = t + pointCount;
                triangles.Add(s);
                triangles.Add(u + 1);
                triangles.Add(t);
                triangles.Add(s);
                triangles.Add(u);
                triangles.Add(u + 1);
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(0.6F, 1.0F, 0.2F);
        meshRenderer.sharedMaterial = material;
    }

    // Update is called once per frame
    void Update()
    {
        troid.transform.Rotate(Vector3.up * 2.0F);
        troid.transform.Rotate(Vector3.left * 3.0F);
    }

    List<Vector3> points()
    {
        float theta = 2 * Mathf.PI / pointCount;
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < pointCount + 1; i++)
        {
            points.Add(new Vector3(Mathf.Cos(theta * i) * radius + latheRadius, 0, Mathf.Sin(theta * i) * radius));
        }
        return points;
    }

    List<Vector3> rotatedPoints(List<Vector3> points)
    {
        float theta = 2 * Mathf.PI / segmentCount;
        List<Vector3> rotatedPoints = new List<Vector3>();
        foreach (Vector3 p in points)
        {
            rotatedPoints.Add(Quaternion.Euler(0, 0, Mathf.Rad2Deg * theta) * p);
        }
        return rotatedPoints;
    }
}
