using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LikeProcessing
{
	[RequireComponent (typeof(Camera))]
	public class OrthoCameraMouseControl : MonoBehaviour
	{
		private Vector3 preMousePos;

		[SerializeField, Range (0.1f, 10f)]
		private float wheelSpeed = 1f;

		[SerializeField, Range (0.1f, 10f)]
		private float moveSpeed = 0.3f;

		[SerializeField, Range (0.1f, 10f)]
		private float rotateSpeed = 0.3f;

		private Camera camera;

		public Vector3 center = Vector3.zero;

		void Start ()
		{
			camera = GetComponent<Camera> ();
		}

		private void Update ()
		{
			MouseUpdate ();
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
			camera.orthographicSize += delta * wheelSpeed;
		}

		private void MouseDrag (Vector3 mousePos)
		{
			Vector3 diff = mousePos - preMousePos;

			if (diff.magnitude < Vector3.kEpsilon)
				return;

			if (Input.GetMouseButton (2)) {
				transform.localPosition = Quaternion.AngleAxis (diff.x * moveSpeed, Vector3.up) * transform.localPosition;
				transform.localPosition = Quaternion.AngleAxis (diff.y * moveSpeed, Vector3.right) * transform.localPosition;
			} else if (Input.GetMouseButton (1))
				CameraRotate (new Vector2 (-diff.y, diff.x) * rotateSpeed);

			preMousePos = mousePos;
			transform.LookAt (center);
		}

		public void CameraRotate (Vector2 angle)
		{
			transform.RotateAround (transform.position, transform.right, angle.x);
			transform.RotateAround (transform.position, Vector3.up, angle.y);
		}
	}
}
