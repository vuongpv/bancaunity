using UnityEngine;
using System;

public class EchoCoreManager : MonoBehaviour 
{
	public int maxEchoFXEvents 	= 32;
	public bool dynamicAdd 		= true;

//============================================================
	void Awake()
	{
		// pass number of max events you need at one time
		EchoFXEvent.PoolAlloc ( maxEchoFXEvents, dynamicAdd );		
	}

	//============================================================
	void Start()
	{
		Shader.WarmupAllShaders();
	}

	//============================================================
	void LateUpdate () 
	{
		EchoFXEvent.ProcessAllInUpdate();		
	}
}
