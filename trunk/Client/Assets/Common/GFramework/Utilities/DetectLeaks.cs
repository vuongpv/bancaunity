using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class LeakCounter
{
	public int allCounters;
	public Dictionary<string, int> objectInstances = new Dictionary<string, int>();
	public List<KeyValuePair<string, int>> sortedObjectInstances; 
}

public class DetectLeaks : MonoBehaviour
{
	private Vector2 scrollPos = Vector2.zero;
	private Vector2 scrollSubPos = Vector2.zero;

	private Dictionary<Type, LeakCounter> objectTypes = new Dictionary<Type, LeakCounter>();
	private List<KeyValuePair<Type, LeakCounter>> sortedObjectTypes;

	private Type currentShowType;

	void Start()
	{
		StartCoroutine(UpdateInfo());
	}

	IEnumerator UpdateInfo()
	{
		while(true)
		{
			// Clear
			foreach (var type in objectTypes)
			{
				type.Value.allCounters = 0;
				if (type.Value.objectInstances != null)
				{
					foreach (var ins in type.Value.objectInstances.Keys.ToArray())
						type.Value.objectInstances[ins] = 0;
				}

				type.Value.sortedObjectInstances = null;
			}


			// Count
			UnityEngine.Object[] objects = FindObjectsOfType(typeof(UnityEngine.Object));
			foreach (UnityEngine.Object obj in objects)
			{
				Type key = obj.GetType();
				LeakCounter counter = null;
				if (!objectTypes.TryGetValue(key, out counter) || counter == null)
				{
					counter = new LeakCounter();
					objectTypes[key] = counter;
				}

				counter.allCounters++;
				if (counter.objectInstances == null)
					counter.objectInstances = new Dictionary<string, int>();

				if (!counter.objectInstances.ContainsKey(obj.name))
					counter.objectInstances.Add(obj.name, 0);

				counter.objectInstances[obj.name]++;
			}

			// Sort type counter
			sortedObjectTypes = new List<KeyValuePair<Type, LeakCounter>>(objectTypes);
			sortedObjectTypes.Sort(
				delegate(KeyValuePair<Type, LeakCounter> firstPair, KeyValuePair<Type, LeakCounter> nextPair)
				{
					return nextPair.Value.allCounters.CompareTo(firstPair.Value.allCounters);
				}
			);

			// Sort instance counter
			foreach (var type in objectTypes)
			{
				type.Value.sortedObjectInstances = new List<KeyValuePair<string, int>>(type.Value.objectInstances);
				type.Value.sortedObjectInstances.Sort(
					delegate(KeyValuePair<string, int> firstPair, KeyValuePair<string, int> nextPair)
					{
						return nextPair.Value.CompareTo(firstPair.Value);
					}
				);
			}


			yield return new WaitForSeconds(1.0f);
		}
	}

    void OnGUI()
    {
		if (sortedObjectTypes == null)
			return;
		GUILayout.BeginHorizontal();


		scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUI.skin.box);
		foreach (KeyValuePair<Type, LeakCounter> entry in sortedObjectTypes)
        {
			GUILayout.BeginHorizontal();
			if (GUILayout.Toggle(currentShowType == entry.Key, "", GUILayout.Width(18)))
				currentShowType = entry.Key;

            GUILayout.Label(entry.Key.ToString() + ": ", GUILayout.Width(200));
			GUILayout.Label("" + entry.Value.allCounters, GUILayout.Width(40));
			GUILayout.EndHorizontal();
        }
		GUILayout.EndScrollView();

		if (currentShowType != null)
		{
			LeakCounter currentCounter = null;
			if (!objectTypes.TryGetValue(currentShowType, out currentCounter) || currentCounter == null || currentCounter.sortedObjectInstances == null)
				return;

			scrollSubPos = GUILayout.BeginScrollView(scrollSubPos, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUI.skin.box);
			foreach (KeyValuePair<string, int> entry2 in currentCounter.sortedObjectInstances)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(entry2.Key + ": ", GUILayout.Width(200));
				GUILayout.Label("" + entry2.Value, GUILayout.Width(40));
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
    }
}