using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NewBehaviourScriptxx : MonoBehaviour {

//	public Shader shader;
//
//	Material _material;
//	CommandBuffer commandBuffer;

//	void Start () {
//		_material = new Material(shader);
//		commandBuffer = new CommandBuffer ();
//	}

	public ComputeShader shader;

	void Start ()
	{
		shader.Dispatch(0, 1, 1, 1);
	}
		
//	void OnRenderObject() {
//
//		// レンダリングを開始
//		_material.SetPass(0);
//
//		// 1万個のオブジェクトをレンダリング
//		Graphics.DrawProcedural(MeshTopology.Points, 10000, 0);
////		commandBuffer.DrawProcedural(Matrix4x4.identity,//gameObject.transform.localToWorldMatrix,
////			_material,
////			0,
////			MeshTopology.Points,
////			1,
////			100,
////			null
////		);
//	}
}
