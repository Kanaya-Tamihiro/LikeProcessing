using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	public RenderTexture m_colorTex;
	public RenderTexture m_depthTex;
	//	public Material m_postRenderMat;

	void Start ()
	{
		Camera cam = GetComponent<Camera>();
		cam.depthTextureMode = DepthTextureMode.Depth;

		// カラーバッファ用 RenderTexture
		m_colorTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
		m_colorTex.Create();

		// デプスバッファ用 RenderTexture
		m_depthTex = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Depth);
		m_depthTex.Create();

		// cameraにカラーバッファとデプスバッファをセットする
		cam.SetTargetBuffers(m_colorTex.colorBuffer, m_depthTex.depthBuffer);

		Vector3 position = transform.position;
		transform.position = new Vector3 (0, position.y, position.z);
		transform.LookAt (Vector3.zero);
	}

	void Update() {
//		Vector3 position = transform.position;
//		float x = position.x + Time.deltaTime * 0.4f;
//		transform.position = new Vector3 (x, position.y, position.z);
		transform.LookAt (Vector3.zero);
	}
}
