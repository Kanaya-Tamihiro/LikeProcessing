using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevirseCamera : MonoBehaviour {

	GameObject mainCamera;

	// Use this for initialization
	void Start () {
		mainCamera = GameObject.Find ("MainCamera");
		Vector3 position = transform.position;
		transform.position = new Vector3 (0, position.y, position.z);
		transform.LookAt (Vector3.zero);
	}
	
	void Update() {
		Vector3 position = transform.position;
//		float x = position.x + Time.deltaTime * 0.4f;
//		transform.position = new Vector3 (x, position.y, position.z);
		transform.position = new Vector3 (mainCamera.transform.position.x, position.y, position.z);
		transform.LookAt (Vector3.zero);
	}
}
