using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;

public class PrefabToFile : EditorWindow
{
    #region Initinate all routes to config text file
    public GameObject prefab=null;
    public UnityEngine.Object fileText=null;
    [MenuItem("FishHunt/PrefabToFile")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PrefabToFile));
    }
    

    public void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Prefab:");
            prefab = (GameObject)EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Save to text file"))
            {
                ModifiedPrefabToFile();
            }
            EditorGUILayout.Separator();
            EditorGUILayout.PrefixLabel("_______________________________________________________________________");
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Text:");
            fileText = (UnityEngine.Object)EditorGUILayout.ObjectField(fileText, typeof(UnityEngine.Object), false);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Load from text"))
            {
                LoadToScene();
            }
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndVertical();
    }
    private void ModifiedPrefabToFile()
    {
        //Debug.LogError("ModifiedPrefabToFile()");
        TextData = "";
        if (prefab != null)
            ToText(prefab);
        //Debug.LogError("textdata=" + TextData);
        string savefile = Application.dataPath + @"/Temp/" + prefab.name+".txt";
        Debug.LogError("OKIE:"+savefile);
        File.WriteAllText(savefile, TextData);
        
    }
    private void LoadToScene()
    {
        //Debug.LogError("LoadToScene");
        if (fileText != null)
        {
            TextAsset _dataText = (TextAsset)fileText;
            RestorePrefab(null, _dataText.text);
            Debug.LogError("Finish");
        }
       
    }
    public string TextData = "";
    public void ToText(GameObject tranform)
    {
        TextData+="{";
        GameObject mainObject = tranform;
        Vector3 location = mainObject.transform.localPosition;
        Vector3 rotation = mainObject.transform.localRotation.eulerAngles;
        Vector3 scale = mainObject.transform.localScale;
        TextData += mainObject.name.Replace("$$", "") + "$$";
        TextData += location.x + "," + location.y +","+ location.z + "$$";
        TextData += rotation.x + "," + rotation.y +","+ rotation.z + "$$";
        TextData += scale.x + "," + scale.y + "," + scale.z + "$$";
        Component[] components = mainObject.GetComponents(typeof(Component));
        for (int i = 1; i < components.Length; i++)
        {
            TextData += components[i].GetType().Name+",";
        }
        TextData+="$$";
        TextData += "\n";
        foreach (Transform child in mainObject.transform) 
        {
            ToText(child.gameObject);
        }
        

        TextData+="}";

    }
    String[] splitBig = new String[] { "$$" };
    String[] splitElement = new String[] { "," };
    public void RestorePrefab(Transform parrent,string _text)
    {
      
        if (_text.Length < 10)
            return;
        _text = _text.Substring(1, _text.Length - 2);
        //Debug.LogError(_text);
        int _index=_text.IndexOf("\n");
        string sub = _text.Substring(0, _index);
        //Debug.LogError(sub);
        string[] info=sub.Split(splitBig,StringSplitOptions.None);
        
        GameObject _object=new GameObject();
        if (parrent != null)
        {
            _object.transform.parent = parrent;
        }
        _object.name = info[0];
        string[] info1=info[1].Split(splitElement,StringSplitOptions.RemoveEmptyEntries);
        _object.transform.localPosition=new Vector3(float.Parse(info1[0]),float.Parse(info1[1]),float.Parse(info1[2]));
        string[] info2 = info[2].Split(splitElement, StringSplitOptions.RemoveEmptyEntries);
        //_object.transform.localRotation = new Vector3(float.Parse(info2[0]), float.Parse(info2[1]), float.Parse(info2[2]));
        string[] info3 = info[3].Split(splitElement, StringSplitOptions.RemoveEmptyEntries);
        _object.transform.localScale = new Vector3(float.Parse(info3[0]), float.Parse(info3[1]), float.Parse(info3[2]));
        string[] info4= info[4].Split(splitElement, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < info4.Length; i++)
        {
            _object.AddComponent(info4[i]);
        }
        //Debug.LogError(_index + "," +_text.Length + "," + _text);
        _text = _text.Substring(_index+1, _text.Length-_index-1);
        //Debug.LogError(_text);
        while (true)
        {
            int startChildIndex = _text.IndexOf("{");
            if (startChildIndex != -1)
            {
                int indexStop = 0;
                int count = 1;
                for (int i = startChildIndex+1; i < _text.Length; i++)
                {
                    if (_text[i] == '{')
                    {
                        count++;
                    }
                    if (_text[i] == '}')
                    {
                        count--;
                    }
                    if (count == 0)
                    {
                        indexStop = i;
                        break;
                    }
                }
                string textData = _text.Substring(startChildIndex, indexStop - startChildIndex+1);
                _text = _text.Substring(indexStop+ 1, _text.Length-1-indexStop);
                ////Debug.LogError("AAA:" + textData);
                RestorePrefab(_object.transform, textData);
            }
            else
            {
                break;
            }
        }

    }
    #endregion
}
