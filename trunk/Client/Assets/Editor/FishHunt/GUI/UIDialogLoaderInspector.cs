using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
[CustomEditor(typeof(GUIDialogBase))]
public class UIDialogLoaderInspector : Editor
{
    private GUIDialogBase uiDialogLoader;
	// Use this for initialization
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        uiDialogLoader = (GUIDialogBase)target;

        GUILayout.Label("GUI Dialog Custom Editor", EditorStyles.boldLabel);
        uiDialogLoader.dialogPrefab = EditorGUILayout.TextField("Dialog Name", uiDialogLoader.dialogPrefab);
        uiDialogLoader.locationName = EditorGUILayout.TextField("Location Name", uiDialogLoader.locationName);
        uiDialogLoader.layer = EditorGUILayout.IntField("Z-Order(0-15)", uiDialogLoader.layer);
        uiDialogLoader.hideAction = (GUIPanelHideAction)EditorGUILayout.EnumPopup("Hide Action", uiDialogLoader.hideAction);
        uiDialogLoader.destroyTimeout = (float)EditorGUILayout.FloatField("Destroy timeout", uiDialogLoader.destroyTimeout);
        uiDialogLoader.showDelay = (float)EditorGUILayout.FloatField("ShowDelay", uiDialogLoader.showDelay);
        uiDialogLoader.showTweenName = EditorGUILayout.TextField("showTweenName ", uiDialogLoader.showTweenName);
        uiDialogLoader.hideTweenName = EditorGUILayout.TextField("showTweenName ", uiDialogLoader.hideTweenName);
        uiDialogLoader.useBlackBolder = EditorGUILayout.Toggle("Use Black Border ", uiDialogLoader.useBlackBolder);
        uiDialogLoader.isSetupLocation = EditorGUILayout.Toggle("Setup startup position ", uiDialogLoader.isSetupLocation);
        if (uiDialogLoader.isSetupLocation)
        {
            uiDialogLoader.vectorSetupLocation = EditorGUILayout.Vector2Field("position", uiDialogLoader.vectorSetupLocation);
        }


        GUILayout.Label("__________________________________________________________", EditorStyles.largeLabel);
        
        GUILayout.Label("Load GUIDialog on Editor", EditorStyles.largeLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("LOAD", GUILayout.Width(120), GUILayout.Height(60)))
        {
            //Debug.LogError("Load");
            uiDialogLoader.Load();
        }
        if (GUILayout.Button("UNLOAD", GUILayout.Width(120),GUILayout.Height(60)))
        {
            //Debug.LogError("Unload");
            uiDialogLoader.UnLoad();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("__________________________________________________________", EditorStyles.largeLabel);
        EditorGUILayout.Separator();
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
