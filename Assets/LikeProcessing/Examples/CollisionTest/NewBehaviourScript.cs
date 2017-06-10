using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float speed = 0.1f;
		if (Input.GetKey (KeyCode.RightArrow)) {
			transform.Translate (speed, 0, 0);
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.Translate (speed * -1, 0, 0);
		}
	}

	void OnCollisionEnter(Collision collision){
		Debug.Log ("OnCollisionEnter" + gameObject.name);
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("OnTriggerEnter" + gameObject.name);
	}
}
