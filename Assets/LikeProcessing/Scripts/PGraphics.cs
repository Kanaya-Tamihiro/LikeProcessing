using UnityEngine;

namespace LikeProcessing
{
    public class PGraphics
    {
        public static int nextLayer = 8;
        public int layer;
        public GameObject cameraObj;
        public int width;
        public int height;
        public RenderTexture renderTexture;

        public PGraphics(int width, int height)
        {
            layer = nextLayer;
            nextLayer++;
            this.width = width;
            this.height = height;

            cameraObj = new GameObject("PGraphics Camera " + layer);
            Camera camera = cameraObj.AddComponent<Camera>();
            PSketch.setupCamera(camera, height);
            camera.cullingMask = 1 << layer;
            camera.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.cullingMask &= ~(1 << layer);

            renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            renderTexture.antiAliasing = 4;
            //renderTexture.Create();
            camera.targetTexture = renderTexture;
        }

        public static PGraphics createGraphics(int width, int height)
        {
            return new PGraphics(width, height);
        }

        public void renderTo(GameObject obj)
        {
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            Material material = new Material(Shader.Find("Standard"));
            material.SetTexture("_MainTex", this.renderTexture);
            meshRenderer.material = material;
        }

    }
}