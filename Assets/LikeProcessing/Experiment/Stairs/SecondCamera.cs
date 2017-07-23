using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondCamera : MonoBehaviour {

	private RenderTexture m_colorTex;
	private RenderTexture m_depthTex;
	public Material m_postRenderMat;

	private RenderTexture maskDepthTex;
	private RenderTexture mainColorTex;

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

		m_postRenderMat.SetTexture ("_SecondColorTex", m_colorTex);
		m_postRenderMat.SetTexture ("_SecondDepthTex", m_depthTex);
	}

	void Update() {
		if (maskDepthTex == null) {
			GameObject maskCamera = GameObject.Find ("MaskCamera");
			SecondMaskCamera smc = maskCamera.GetComponent<SecondMaskCamera> ();
			maskDepthTex = smc.m_depthTex;
			m_postRenderMat.SetTexture ("_SecondMaskDepthTex", maskDepthTex);
		}
		if (mainColorTex == null) {
			GameObject mainCamera = GameObject.Find ("MainCamera");
			MainCamera mc = mainCamera.GetComponent<MainCamera> ();
			mainColorTex = mc.m_colorTex;
		}
	}

	void OnPostRender()
	{
		if (maskDepthTex == null || mainColorTex == null)
			return;
		// RenderTarget無し：画面に出力される
		Graphics.SetRenderTarget(null);

		// デプスバッファを描画する(m_postRenderMatはテクスチャ画像をそのまま描画するマテリアル)
		Graphics.Blit(mainColorTex, m_postRenderMat);
	}
}
