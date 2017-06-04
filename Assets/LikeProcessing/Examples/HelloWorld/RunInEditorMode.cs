using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RunInEditorMode : MonoBehaviour {

	int n = 0;

	// Use this for initialization
	void Start () {
		Debug.Log (n);
	}
	
	void Awake()
	{
		Debug.Log("Editor causes this Awake");
	}

	void Update()
	{
		this.n++;
		Debug.Log("Editor causes this Update");
	}
}
