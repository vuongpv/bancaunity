using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FHFishData))]
public class FishDataInspector : Editor
{
		private FHFishData fishData;
	
		private bool isGroup;
	
		private int fishNameID;
	
		private int fishGroupNameID;
	
		private string[] fishNames;
	
		private string[] fishgGroupNames;

		public override void OnInspectorGUI ()
		{        
				LoadData ();
		
				EditorGUILayout.BeginVertical ();
				fishNameID = EditorGUILayout.Popup ("Fish name:", fishNameID, fishNames, EditorStyles.popup);
				EditorGUILayout.EndVertical ();
		
				if (fishgGroupNames != null && fishgGroupNames.Length > 0) {
						EditorGUILayout.BeginVertical ();
						isGroup = EditorGUILayout.Toggle ("Is group:", isGroup, EditorStyles.toggle);
						EditorGUILayout.EndVertical ();
				} else
						isGroup = false;
		
				if (isGroup) {
						LoadFishGroupNames ();
			
						if (fishgGroupNames != null) {
								EditorGUILayout.BeginVertical ();
								fishGroupNameID = EditorGUILayout.Popup ("Group name:", fishGroupNameID, fishgGroupNames, EditorStyles.popup);
								EditorGUILayout.EndVertical ();
						}
				}
		
				EditorGUILayout.BeginVertical ();
				bool reload = GUILayout.Button ("Reload Configs", GUILayout.Width (100f));
				EditorGUILayout.EndVertical ();
		
				if (GUI.changed) {
						SaveData ();
						EditorUtility.SetDirty (target);
				}
		
				if (reload)
						ConfigManager.instance.LoadFishConfigs (true);
		}
	
		void LoadData ()
		{
				fishData = target as FHFishData;
		
				isGroup = fishData.isGroup;
				fishNameID = 0;
				fishGroupNameID = 0;
		
				fishNames = null;
				fishgGroupNames = null;
		
				ConfigManager.instance.LoadFishConfigs (false);
		
				LoadFishNames ();
				LoadFishGroupNames ();
		}
	
		void LoadFishNames ()
		{
				int index = 0;
				fishNames = new string[ConfigManager.configFish.records.Count];
				foreach (ConfigFishRecord fish in ConfigManager.configFish.records) {
						if (fish.id == fishData.fishID || fishData.fishID == -1) {
								fishNameID = index;
								fishData.fishID = fish.id;
						}
			
						fishNames [index++] = fish.name;
				}
		}
	
		void LoadFishGroupNames ()
		{
				List<ConfigFishGroupRecord> fishGroups = ConfigManager.configFishGroup.GetFishGroupsByFishID (fishData.fishID);
				if (fishGroups == null || fishGroups.Count <= 0)
						return;
		
				int index = 0;
				fishgGroupNames = new string[fishGroups.Count];
				foreach (ConfigFishGroupRecord fishGroup in fishGroups) {
						if (fishGroup.id == fishData.fishGroupID)
								fishGroupNameID = index;
			
						fishgGroupNames [index++] = fishGroup.name;
				}
		}
	
		void SaveData ()
		{
				fishData.isGroup = isGroup;
		
				foreach (ConfigFishRecord fish in ConfigManager.configFish.records) {
						if (fish.name == fishNames [fishNameID]) {
								fishData.fishID = fish.id;
								break;
						}
				}
		
				List<ConfigFishGroupRecord> fishGroups = ConfigManager.configFishGroup.GetFishGroupsByFishID (fishData.fishID);
				if (fishGroups == null || fishGroups.Count <= 0 || fishgGroupNames == null || fishGroupNameID >= fishgGroupNames.Length)
						return;
		
				foreach (ConfigFishGroupRecord fishGroup in fishGroups) {
						if (fishGroup.name == fishgGroupNames [fishGroupNameID]) {
								fishData.fishGroupID = fishGroup.id;
								break;
						}
				}
		}

}
