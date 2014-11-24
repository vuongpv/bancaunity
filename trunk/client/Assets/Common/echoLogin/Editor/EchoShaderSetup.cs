using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(EchoShaderManager))]
public class EchoShaderSetup : Editor 
{
		private SerializedObject echoShader;
		private SerializedProperty
		pointLight,
		dirLight;
	
	//============================================================
	void OnEnable () 
	{
		echoShader = new SerializedObject(target);
		pointLight = echoShader.FindProperty("PointLight");
		dirLight = echoShader.FindProperty("DirectionalLight");
	}
	
	//============================================================
	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("Select light type to use for lit echoLogin Shaders.");
		
		EditorGUILayout.PropertyField ( pointLight );
		EditorGUILayout.PropertyField ( dirLight );
		
		echoShader.ApplyModifiedProperties();
		SetShaders();
	}

	//============================================================
	public void SetShaders()
	{
		if ( pointLight.boolValue && dirLight.boolValue )
		{
			Shader.EnableKeyword ("ECHO_POINTANDDIRECTIONAL");
			Shader.DisableKeyword ("ECHO_POINT");
			Shader.DisableKeyword ("ECHO_DIRECTIONAL");
		}
		else if ( pointLight.boolValue )
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
};