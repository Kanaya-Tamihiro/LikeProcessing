using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoViewPointCamera
{
	public class TwoViewPointCamera : MonoBehaviour
	{

		protected RenderTexture mainColorTexture;
		protected RenderTexture mainDepthTexture;
		protected RenderTexture secondMaskColorTexture;
		protected RenderTexture secondMaskDepthTexture;
		protected RenderTexture secondColorTexture;
		protected RenderTexture secondDepthTexture;

		public Vector3 lookAt = Vector3.zero;

		protected GameObject secondCamerasObj;
		protected GameObject secondMaskCameraObj, secondCameraObj;

		protected Camera mainCamera, secondMaskCamera, secondCamera;

		SecondCamera secondCameraScript;

		public int secondCameraLayer = 8;

		void Start ()
		{
			mainCamera = GetComponent<Camera> ();
			mainCamera.depthTextureMode = DepthTextureMode.Depth;
			mainColorTexture = new RenderTexture (Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
			mainColorTexture.Create ();
			mainDepthTexture = new RenderTexture (Screen.width, Screen.height, 24, RenderTextureFormat.Depth);
			mainDepthTexture.Create ();
			mainCamera.SetTargetBuffers (mainColorTexture.colorBuffer, mainDepthTexture.depthBuffer);

			mainCamera.orthographic = true;
			mainCamera.cullingMask &= ~(1 << secondCameraLayer);

			secondCamerasObj = new GameObject ("SecondCameras");
			secondMaskCameraObj = new GameObject ("SecondMaskCamera");
			secondCameraObj = new GameObject ("SecondCamera");
			secondMaskCameraObj.transform.SetParent (secondCamerasObj.transform);
			secondCameraObj.transform.SetParent (secondCamerasObj.transform);

			secondMaskCamera = secondMaskCameraObj.AddComponent<Camera> ();
			secondMaskCamera.depthTextureMode = DepthTextureMode.Depth;
			secondMaskColorTexture = new RenderTexture (Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
			secondMaskColorTexture.Create ();
			secondMaskDepthTexture = new RenderTexture (Screen.width, Screen.height, 24, RenderTextureFormat.Depth);
			secondMaskDepthTexture.Create ();
			secondMaskCamera.SetTargetBuffers (secondMaskColorTexture.colorBuffer, secondMaskDepthTexture.depthBuffer);

			secondMaskCamera.orthographic = true;
			secondMaskCamera.cullingMask &= ~(1 << secondCameraLayer);
			secondMaskCamera.clearFlags = CameraClearFlags.Depth;

			secondCamera = secondCameraObj.AddComponent<Camera> ();
			secondCamera.depthTextureMode = DepthTextureMode.Depth;
			secondColorTexture = new RenderTexture (Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
			secondColorTexture.Create ();
			secondDepthTexture = new RenderTexture (Screen.width, Screen.height, 24, RenderTextureFormat.Depth);
			secondDepthTexture.Create ();
			secondCamera.SetTargetBuffers (secondColorTexture.colorBuffer, secondDepthTexture.depthBuffer);

			secondCamera.orthographic = true;
			secondCamera.cullingMask = 1 << secondCameraLayer;
			secondCamera.clearFlags = CameraClearFlags.Depth;

			secondCameraScript = secondCameraObj.AddComponent<SecondCamera> ();
			secondCameraScript.mainCameraScript = this;

			UpdateSecondCamera ();
		}

		void UpdateSecondCamera() {
			
			secondMaskCamera.depth = mainCamera.depth + 1;
			secondMaskCamera.orthographicSize = mainCamera.orthographicSize;
			secondMaskCamera.nearClipPlane = mainCamera.nearClipPlane;
			secondMaskCamera.farClipPlane = mainCamera.farClipPlane;

			secondCamera.depth = mainCamera.depth + 2;
			secondCamera.orthographicSize = mainCamera.orthographicSize;
			secondCamera.nearClipPlane = mainCamera.nearClipPlane;
			secondCamera.farClipPlane = mainCamera.farClipPlane;
		}

		void Update ()
		{
			transform.LookAt (lookAt);
			Vector3 mainCameraWorldPosition = transform.position;
			Vector3 reflectionVector = new Vector3(
				-1 * (lookAt.x - mainCameraWorldPosition.x),
				1 * (lookAt.y - mainCameraWorldPosition.y),
				1 * (lookAt.z - mainCameraWorldPosition.z));

			secondCamerasObj.transform.position = lookAt + reflectionVector;
			secondCamerasObj.transform.LookAt(lookAt);
		}

		class SecondCamera : MonoBehaviour {

			public Material postRenderMaterial;
			public TwoViewPointCamera mainCameraScript;

			void Start () {
				postRenderMaterial = new Material(Shader.Find("TwoViewPointCamera/MixShader"));
				postRenderMaterial.SetTexture ("_SecondColorTex", mainCameraScript.secondColorTexture);
				postRenderMaterial.SetTexture ("_SecondDepthTex", mainCameraScript.secondDepthTexture);
				postRenderMaterial.SetTexture ("_SecondMaskDepthTex", mainCameraScript.secondMaskDepthTexture);
			}

			void OnPostRender()
			{
				Graphics.SetRenderTarget(null);
				Graphics.Blit(mainCameraScript.mainColorTexture, postRenderMaterial);
			}
		}
	}

}