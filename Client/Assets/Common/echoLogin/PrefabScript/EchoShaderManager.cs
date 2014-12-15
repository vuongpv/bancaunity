using UnityEngine;
using System;

[ExecuteInEditMode()] 
public class EchoShaderManager : MonoBehaviour 
{
	public bool PointLight   = false;
	public bool DirectionalLight = true;
	
	//============================================================
	virtual public void OnDestroy()
	{
		EchoGameObject._initFlag = false;
	}

	//============================================================
	public void SetShaders()
	{
		if ( PointLight && DirectionalLight )
		{
			Shader.EnableKeyword ("ECHO_POINTANDDIRECTIONAL");
			Shader.DisableKeyword ("ECHO_POINT");
			Shader.DisableKeyword ("ECHO_DIRECTIONAL");
		}
		else if ( PointLight )
		{
			Shader.DisableKeyword ("ECHO_POINTANDDIRECTIONAL");
			Shader.EnableKeyword ("ECHO_POINT");
			Shader.DisableKeyword ("ECHO_DIRECTIONAL");
		}
		else 
		{
			Shader.DisableKeyword ("ECHO_POINTANDDIRECTIONAL");
			Shader.DisableKeyword ("ECHO_POINT");
			Shader.EnableKeyword ("ECHO_DIRECTIONAL");
		}
	}

	//============================================================
	void Start()
	{
		SetShaders();
	}

	//============================================================
	void Awake()
	{
		SetShaders();
	}
}