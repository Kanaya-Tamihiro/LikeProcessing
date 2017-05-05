using UnityEngine;

/// <summary>
/// GameビューにてSceneビューのようなカメラの動きをマウス操作によって実現する
/// </summary>
namespace LikeProcessing
{
[RequireComponent(typeof(Camera))]
public class PCamera : MonoBehaviour
{
	[SerializeField, Range(0.1f, 10f)]
	private float wheelSpeed = 1f;

	[SerializeField, Range(0.1f, 10f)]
	private float moveSpeed = 0.3f;

	[SerializeField, Range(0.1f, 10f)]
	private float rotateSpeed = 0.3f;

	[SerializeField, Range(-1.0f, 1.0f)]
	private float horizonOblique = 0.0f;

	[SerializeField, Range(-1.0f, 1.0f)]
	private float verticalOblique = 0.0f;

	private Vector3 preMousePos;

	private void Update()
	{
		MouseUpdate();
		ObliqueUpdate ();
		return;
	}

	private void ObliqueUpdate() {
		Matrix4x4 mat  = Camera.main.projectionMatrix;
		mat[0, 2] = horizonOblique;
		mat[1, 2] = verticalOblique;
		Camera.main.projectionMatrix = mat;
	}

	private void MouseUpdate()
	{
		float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
		if(scrollWheel != 0.0f)
			MouseWheel(scrollWheel);

		if(Input.GetMouseButtonDown(0) ||
			Input.GetMouseButtonDown(1) ||
			Input.GetMouseButtonDown(2))
			preMousePos = Input.mousePosition;

		MouseDrag(Input.mousePosition);
	}

	private void MouseWheel(float delta)
	{
		transform.position += transform.forward * delta * wheelSpeed;
		return;
	}

	private void MouseDrag(Vector3 mousePos)
	{
		Vector3 diff = mousePos - preMousePos;

		if(diff.magnitude < Vector3.kEpsilon)
			return;

		if(Input.GetMouseButton(2))
			transform.Translate(-diff * Time.deltaTime * moveSpeed);
		else if(Input.GetMouseButton(1))
			CameraRotate(new Vector2(-diff.y, diff.x) * rotateSpeed);

		preMousePos = mousePos;
	}

	public void CameraRotate(Vector2 angle)
	{
		transform.RotateAround(transform.position, transform.right, angle.x);
		transform.RotateAround(transform.position, Vector3.up, angle.y);
	}
}
}