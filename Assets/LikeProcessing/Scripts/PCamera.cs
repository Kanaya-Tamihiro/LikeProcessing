using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// GameビューにてSceneビューのようなカメラの動きをマウス操作によって実現する
/// </summary>
namespace LikeProcessing
{
	[RequireComponent (typeof(Camera))]
	public class PCamera : MonoBehaviour
	{
		[SerializeField, Range (0.1f, 10f)]
		private float wheelSpeed = 1f;

		[SerializeField, Range (0.1f, 10f)]
		private float moveSpeed = 0.3f;

		[SerializeField, Range (0.1f, 10f)]
		private float rotateSpeed = 0.3f;

		[SerializeField, Range (-1.0f, 1.0f)]
		private float horizonOblique = 0.0f;

		[SerializeField, Range (-1.0f, 1.0f)]
		private float verticalOblique = 0.0f;

		private Vector3 preMousePos;

		[DllImport ("RenderingPlugin")]
		private static extern void UpdateCameraPosition (float posX, float posY, float posZ);

		[DllImport ("RenderingPlugin")]
		private static extern void UpdateCameraRotMat (float m00, float m01, float m02, float m03,
		                                                 float m10, float m11, float m12, float m13,
		                                                 float m20, float m21, float m22, float m23,
		                                                 float m30, float m31, float m32, float m33);

		[DllImport ("RenderingPlugin")]
		private static extern void UpdateCameraRotQuat (float qx, float qy, float qz, float qw);

		private void Update ()
		{
			MouseUpdate ();
			ObliqueUpdate ();
			return;
		}

		private void ObliqueUpdate ()
		{
			Matrix4x4 mat = Camera.main.projectionMatrix;
			mat [0, 2] = horizonOblique;
			mat [1, 2] = verticalOblique;
			Camera.main.projectionMatrix = mat;
		}

		private void MouseUpdate ()
		{
			float scrollWheel = Input.GetAxis ("Mouse ScrollWheel");
			if (scrollWheel != 0.0f)
				MouseWheel (scrollWheel);

			if (Input.GetMouseButtonDown (0) ||
			    Input.GetMouseButtonDown (1) ||
			    Input.GetMouseButtonDown (2))
				preMousePos = Input.mousePosition;

			MouseDrag (Input.mousePosition);
		}

		private void MouseWheel (float delta)
		{
			transform.position += transform.forward * delta * wheelSpeed;
			UpdatePluginCamera ();
		}

		private void MouseDrag (Vector3 mousePos)
		{
			Vector3 diff = mousePos - preMousePos;

			if (diff.magnitude < Vector3.kEpsilon)
				return;

			if (Input.GetMouseButton (2))
				transform.Translate (-diff * Time.deltaTime * moveSpeed);
			else if (Input.GetMouseButton (1))
				CameraRotate (new Vector2 (-diff.y, diff.x) * rotateSpeed);

			preMousePos = mousePos;
			UpdatePluginCamera ();
		}

		public void CameraRotate (Vector2 angle)
		{
			transform.RotateAround (transform.position, transform.right, angle.x);
			transform.RotateAround (transform.position, Vector3.up, angle.y);
		}

		private void UpdatePluginCamera ()
		{
			Vector3 position = transform.position;
			Quaternion quat = transform.rotation;
			UpdateCameraPosition (position.x, position.y, position.z);

			Matrix4x4 m = Matrix4x4.TRS (Vector3.zero, transform.rotation, Vector3.one);
//			m = m.transpose;
//			UpdateCameraRotMat (m [0], m [1], m [2], m [3], m [4], m [5], m [6], m [7],
//				m [8], m [9], m [10], m [11], m [12], m [13], m [14], m [15]);
			UpdateCameraRotQuat(quat.x, quat.y, quat.z, quat.w);

//			Debug.Log (m.ToString());
		}
	}
}