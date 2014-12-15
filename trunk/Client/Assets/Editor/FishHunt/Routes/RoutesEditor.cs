using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class RoutesEditor : EditorWindow
{
		const string ROUTES_ASSET_RESOURCES_DIR = "Assets/Resources/";
		const string ROUTES_ASSET_DIR = "ScriptableObjects/Routes/";

		int seasonID = 0;
		string[] seasonNames;
		string objectName = "";

		[MenuItem("FishHunt/Open the Routes Editor")]
		static public void OpenRoutesEditor ()
		{
				EditorWindow.GetWindow<RoutesEditor> (false, "Routes Editor", true);
		}

    #region GUI
		void OnGUI ()
		{
				bool select;
				bool load;
				bool save;
				bool customSave;

				LoadSeasonNames ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Season:", EditorStyles.label, GUILayout.Width (64f));        
				if (Selection.activeObject != null)
						objectName = Selection.activeObject.name;
				objectName = GUILayout.TextField (objectName, EditorStyles.textField);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginVertical ();
				customSave = GUILayout.Button ("Custom save", GUILayout.MinWidth (100f));
				EditorGUILayout.EndVertical ();

				if (customSave)
						ExportToAssetFile (objectName); 
        
				if (seasonNames == null || seasonNames.Length <= 0)
						return;

				EditorGUILayout.Separator ();
				EditorGUILayout.Separator ();
				EditorGUILayout.Separator ();

				EditorGUILayout.BeginVertical ();
				seasonID = EditorGUILayout.Popup ("Season:", seasonID, seasonNames, EditorStyles.popup);
				EditorGUILayout.EndVertical ();

				EditorGUILayout.BeginHorizontal ();
				select = GUILayout.Button ("Select", GUILayout.MinWidth (64f));
				EditorGUILayout.Space ();

				load = GUILayout.Button ("Load", GUILayout.MinWidth (64f));
				EditorGUILayout.Space ();

				save = GUILayout.Button ("Save", GUILayout.MinWidth (64f));
				EditorGUILayout.EndHorizontal ();

				if (select)
						SelectAsset ();

				if (load)
						LoadFromAssetFile ();

				if (save)
						ExportToAssetFile ();
		}

		void LoadSeasonNames ()
		{
				seasonNames = null;

				DirectoryInfo dir = new DirectoryInfo (ROUTES_ASSET_RESOURCES_DIR + ROUTES_ASSET_DIR);
				FileInfo[] info = dir.GetFiles ("*.asset");

				seasonNames = new string[info.Length];

				int index = 0;
				foreach (FileInfo f in info)
						seasonNames [index++] = Path.GetFileNameWithoutExtension (f.Name);
		}
    #endregion

    #region Select asset
		void SelectAsset ()
		{
				//string assetFileName = seasonNames[seasonID];
				//Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(ROUTES_ASSET_RESOURCES_DIR + ROUTES_ASSET_DIR + assetFileName + ".asset");
				//EditorUtility.FocusProjectWindow();
				//EditorGUIUtility.ExitGUI();

				string assetFileName = seasonNames [seasonID];
				GameObject selected = GameObject.Find (assetFileName);
				if (selected != null)
						Selection.activeObject = selected;
		}
    #endregion

    #region Load from asset file
		void LoadFromAssetFile ()
		{
				ConfigManager.instance.LoadFishConfigs (true);

				string assetFileName = seasonNames [seasonID];
				GameObject selected = GameObject.Find (assetFileName);
				if (selected != null) {
						Debug.LogError ("Routes Editor: duplicate " + assetFileName + " object!");
						//DestroyObject(selected);
						return;
				}
        
				FHRoutesAsset asset = Resources.Load (ROUTES_ASSET_DIR + assetFileName) as FHRoutesAsset;

				GameObject routeRoot = new GameObject ();
				routeRoot.name = assetFileName;
				routeRoot.transform.position = Vector3.zero;

				for (int i = 0; i < asset.routes.Length; i++) {
						if (asset.routes [i] == null)
								continue;

						GameObject go = new GameObject ();

						string name = "";
						if (asset.routes [i].isGroup) {
								ConfigFishGroupRecord group = ConfigManager.configFishGroup.GetFishGroupByID (asset.routes [i].fishGroupID);
								if (group != null)
										name = group.name;
								else
										asset.routes [i].isGroup = false;
						}

						if (!asset.routes [i].isGroup)
								name = ConfigManager.configFish.GetFishByID (asset.routes [i].fishID).name;

						go.name = WrapRouteID (asset.routes [i].id) + "_" + name;
						go.transform.parent = routeRoot.transform;
						go.transform.position = asset.routes [i].splinePos;

						FHFishData data = go.AddComponent<FHFishData> ();
						data.isGroup = asset.routes [i].isGroup;
						data.fishID = asset.routes [i].fishID;
						data.fishGroupID = 0;
						data.fishGroupID = asset.routes [i].fishGroupID;

						Spline spline = go.AddComponent<Spline> ();
						spline.interpolationMode = Spline.InterpolationMode.BSpline;

						FHRoute route = go.AddComponent<FHRoute> ();

						spline.AddSplineNodes (asset.routes [i].splineNodes);
				}
		}
    #endregion

    #region Export to asset file
		void ExportToAssetFile ()
		{
				string assetFileName = seasonNames [seasonID];
				ExportToAssetFile (assetFileName);
		}

		void ExportToAssetFile (string assetFileName)
		{
				GameObject selected = GameObject.Find (assetFileName);
				if (selected == null) {
						Debug.LogError ("Routes Editor: " + assetFileName + " object not found!");
						return;
				}

				GameObject go = selected as GameObject;
				Spline[] splines = go.GetComponentsInChildren<Spline> ();
				if (splines == null || splines.Length <= 0)
						return;

				FHRoutesAsset asset = ScriptableObject.CreateInstance<FHRoutesAsset> ();
				asset.routes = new FHRouteAsset[splines.Length];

				for (int i = 0; i < splines.Length; i++)
						ExportARoute (asset, i, splines [i]);

				AssetDatabase.CreateAsset (asset, ROUTES_ASSET_RESOURCES_DIR + ROUTES_ASSET_DIR + assetFileName + ".asset");
				AssetDatabase.SaveAssets ();

				Selection.activeObject = asset;

				AssetDatabase.Refresh ();
		}

		void ExportARoute (FHRoutesAsset asset, int index, Spline spline)
		{
				GameObject routeObj = spline.gameObject;

				int routeID = GetRouteID (routeObj.name);
				if (routeID < 0)
						return;

				spline.UpdateSplineNodes ();

				asset.routes [index] = new FHRouteAsset ();

				asset.routes [index].id = routeID;

				FHFishData fishData = routeObj.GetComponent<FHFishData> ();
				asset.routes [index].isGroup = fishData.isGroup;
				asset.routes [index].fishID = fishData.fishID;
				asset.routes [index].fishGroupID = fishData.fishGroupID;

				asset.routes [index].splinePos = spline.transform.position;

				asset.routes [index].splineNodes = new Vector3[spline.SplineNodes.Length];
				for (int i = 0; i < spline.SplineNodes.Length; i++)
						asset.routes [index].splineNodes [i] = spline.SplineNodes [i].Position;
		}
    #endregion

    #region Utilities
		int GetRouteID (string routeName)
		{
				int routeID = -1;
				string routeIDStr = "";

				try {
						while (routeName != "" && routeName[0] != '_') {
								routeIDStr = routeIDStr + routeName [0];
								routeName = routeName.Substring (1, routeName.Length - 1);
						}
				} catch (System.Exception ex) {
						Debug.LogError ("Error! Please check " + ex.Message);
						return routeID;
				}

				int.TryParse (routeIDStr, out routeID);
				return routeID;
		}

		string WrapRouteID (int routeID)
		{
				string result = routeID.ToString ();

				while (result.Length < 3)
						result = "0" + result;

				return result;
		}
    #endregion
}
