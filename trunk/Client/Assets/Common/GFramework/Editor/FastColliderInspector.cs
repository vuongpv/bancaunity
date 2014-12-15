using UnityEditor;
using UnityEngine;
using System.Collections;
using GFramework;
using System.Collections.Generic;

[CustomEditor(typeof(FastCollider))]
public class FastColliderInspector : Editor
{
    private const float BUTTON_HEIGHT = 30f;

    GUIStyle textStyle;

    GameObject testModel;
    bool editStand = false;
    bool editSeat = false;

    int adjustingAxis = 1; // y

    int currentPreview = -1;
    int currentPosIdx = 0;

	List<FastColliderNode> nodes;
	Transform targetTransform;

    public void OnEnable()
    {
		FastCollider obj = target as FastCollider;
		if (obj == null)
			return;

		targetTransform = obj.transform;

		nodes = obj.colliderNodes;
		if (nodes == null)
		{
			nodes = new List<FastColliderNode>();
			obj.colliderNodes = nodes;
		}

		foreach (var node in nodes)
		{
			if (node.transform == null)
				node.transform = targetTransform;
		}
    }

    public override void OnInspectorGUI()
	{
		EditorGUIUtility.LookLikeControls(90f);

		int removeItemIdx = -1;
		for(int i = 0; i < nodes.Count; i++ )
		{
			bool removeItem = false;
			DrawNodeItem(nodes[i], ref removeItem);
			if (removeItem)
				removeItemIdx = i;
		}

		if (removeItemIdx >= 0)
			nodes.RemoveAt(removeItemIdx);

		EditorGUILayout.Separator();

		GUILayout.BeginHorizontal();
		{
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add", GUILayout.Width(180), GUILayout.Height(32)))
			{
				var newNode = new FastColliderNode();
				newNode.Init("Node " + nodes.Count, targetTransform);
				nodes.Add(newNode);
			}
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();

		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}

	/// <summary>
	/// Helper function that draws a field of 3 floats.
	/// </summary>
	private Vector3 DrawVector3(string label, Vector3 value)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(82f));

		GUILayoutOption opt = GUILayout.MinWidth(28f);
		GUILayout.Label("X");
		value.x = EditorGUILayout.FloatField(value.x, opt);
		GUILayout.Label("Y");
		value.y = EditorGUILayout.FloatField(value.y, opt);
		GUILayout.Label("Z");
		value.z = EditorGUILayout.FloatField(value.z, opt);
		GUILayout.EndHorizontal();
		return value;
	}

	private void DrawNodeItem(FastColliderNode node, ref bool needRemove)
	{
		GUILayout.BeginVertical(GUI.skin.box);
		{
			GUILayout.BeginHorizontal();
			{
				node.name = EditorGUILayout.TextField("Name", node.name);
				GUI.color = Color.red;
				if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
					needRemove = true;
				GUI.color = Color.white;
			}
			GUILayout.EndHorizontal();

			node.firstPt = DrawVector3("First Pt", node.firstPt);
			node.lastPt = DrawVector3("Last Pt", node.lastPt);
			node.radius = EditorGUILayout.FloatField("Radius", node.radius);
		}
		GUILayout.EndVertical();
	}

	private void DrawOnSceneNodeItem(FastColliderNode node)
	{
		Vector3 wFirstPt = node.GetWorldFirstPt();
		Vector3 wLastPt = node.GetWorldLastPt();
		float wRadius = node.GetWorldRadius();

		Vector3 diff = wLastPt - wFirstPt;
		Vector3 right = Vector3.Cross(diff, Vector3.up).normalized;

		Handles.color = Color.green;
		Quaternion rotation = (diff.sqrMagnitude > 0 ? Quaternion.LookRotation(diff, Vector3.up) : Quaternion.identity);
		//Vector3 localPosition = node.firstPt + (dist * 0.5f);
		//Handles.CylinderCap(0, wFirstPt + (wLastPt - wFirstPt) * 0.5f, rotation, node.GetWorldRadius(targetTransform));
		Handles.RadiusHandle(rotation, wFirstPt, wRadius);
		Handles.RadiusHandle(rotation, wLastPt, wRadius);
		Handles.DrawLine(wFirstPt + (Vector3.up * wRadius), wLastPt + (Vector3.up * wRadius));
		Handles.DrawLine(wFirstPt - (Vector3.up * wRadius), wLastPt - (Vector3.up * wRadius));
		Handles.DrawLine(wFirstPt + (right * wRadius), wLastPt + (right * wRadius));
		Handles.DrawLine(wFirstPt - (right * wRadius), wLastPt - (right * wRadius));

		Vector3 newFirstPos = Handles.PositionHandle(wFirstPt, rotation);
		node.firstPt = targetTransform.InverseTransformPoint(newFirstPos);

		Vector3 newLastPos = Handles.PositionHandle(wLastPt, rotation);
		node.lastPt = targetTransform.InverseTransformPoint(newLastPos);
	}

    public void OnSceneGUI()
    {
		foreach (var node in nodes)
		{
			DrawOnSceneNodeItem(node);
		}
		
       /* Chair obj = target as Chair;
        if (obj == null)
            return;

        if (listActionPos.Count == 0)
            return;

        Transform actionPos = listActionPos[currentPosIdx];

        // Ray casr
        Vector2 guiPos = HandleUtility.WorldToGUIPoint(actionPos.position);
        guiPos.x += 40;
        guiPos.y += 40;

        Handles.color = Color.white;
        Handles.DrawLine(actionPos.position, HandleUtility.GUIPointToWorldRay(guiPos).origin);

        Rect rect = new Rect(guiPos.x, guiPos.y, 150, 60);

        Rect wndRect = rect;
        wndRect.xMin -= 5;
        wndRect.yMin -= 5;

        //GUI.Window(0, wndRect, DoMyWindow, "Window");

        // Show stand handle
        if (editStand)
        {
            Vector3 newPos = Handles.PositionHandle(actionPos.position, actionPos.rotation);
            if (!MathfEx.Approx(newPos, actionPos.position, Mathf.Epsilon))
            {
                actionPos.position = newPos;
                if (testModel != null)
                    PreviewModel(obj, actionPos, 0);
            }
        }
        else
        {
            Handles.color = Color.green;
			Handles.CubeCap(0, actionPos.position, actionPos.rotation, 0.1f);
        }

        // Show seat handle
        if (editSeat)
        {
            Vector3 newPos = Handles.PositionHandle(actionPos.TransformPoint(obj.seatOffset), actionPos.rotation);
            newPos = actionPos.InverseTransformPoint(newPos);
            if (!MathfEx.Approx(newPos, obj.seatOffset, Mathf.Epsilon))
            {
                obj.seatOffset = newPos;
                if (testModel != null)
                    PreviewModel(obj, actionPos, 1);
            }
        }
        else
        {
            Handles.color = Color.blue;
			Handles.CubeCap(0, actionPos.TransformPoint(obj.seatOffset), actionPos.rotation, 0.05f);
        }

        //Handles.Label(actionPos.position + Vector3.up * 2, new GUIContent("Action Point"), textStyle);



        // Button test
        Handles.BeginGUI();

        //GUI.enabled = false;
        GUI.Box(wndRect, "");
        //GUI.enabled = true;

        GUILayout.BeginArea(rect);


        // Isolate mode
        if (editStand)
        {
            if (GUILayout.Button("< Back", GUILayout.Width(60), GUILayout.Height(BUTTON_HEIGHT)))
                editStand = false;

            GUILayout.EndArea();
            Handles.EndGUI();
            return;
        }

        if (editSeat)
        {
            if (GUILayout.Button("< Back", GUILayout.Width(60), GUILayout.Height(BUTTON_HEIGHT)))
                editSeat = false;

            // Adjusment
            if (testModel != null && editSeat)
            {
                GUILayout.BeginHorizontal();
                float abjustValue = 0.0f;
                if (GUILayout.Button("+"))
                {
                    abjustValue = 0.02f;
                }
                if (GUILayout.Button("-"))
                {
                    abjustValue = -0.02f;
                }

                adjustingAxis = GUILayout.SelectionGrid(adjustingAxis, new string[] { "x", "y", "z" }, 3);
                if (abjustValue != 0.0f)
                {
                    switch (adjustingAxis)
                    {
                        case 0:
                            obj.seatOffset = obj.seatOffset + new Vector3(abjustValue, 0, 0);
                            break;

                        case 1:
                            obj.seatOffset = obj.seatOffset + new Vector3(0, abjustValue, 0);
                            break;

                        case 2:
                            obj.seatOffset = obj.seatOffset + new Vector3(0, 0, abjustValue);
                            break;
                    }

                    PreviewModel(obj, actionPos, 1);
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndArea();
            Handles.EndGUI();
            return;
        }

        // Line by line
        GUILayout.BeginVertical();

        // First line
        GUILayout.BeginHorizontal();
        GUILayout.Label("Action Point", textStyle);
        GUILayout.EndHorizontal();

        // Second line
        GUILayout.BeginHorizontal();
        if (testModel == null)
        {
            if (GUILayout.Button("Attach", GUILayout.Width(50), GUILayout.Height(BUTTON_HEIGHT)))
            {
                testModel = GameObject.Find("TestMaleModel");
                if (testModel == null)
                {
					testModel = GameObject.Find("FemaleCharSample");

					if (testModel == null)
					{
						if (EditorUtility.DisplayDialog("Test model", "Do you want to test Male or Female?", "Male", "Female"))
						{
							testModel = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath("Assets/Art/CharacterSample/FemaleCharSample.prefab", typeof(GameObject)));
							testModel.name = "FemaleCharSample";
						}
						else
						{
							testModel = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath("Assets/Art/CharacterSample/MaleCharSample.prefab", typeof(GameObject)));
							testModel.name = "TestMaleModel";
						}

						if( testModel == null )
							Debug.LogWarning("Can't found \"TestMaleModel\" or \"TestFemaleModel\" in \"Art/CharacterSample/\". Please add manually");
					}
                }

                if (testModel != null)
                {
                    currentPreview = 0;
                    PreviewModel(obj, actionPos, currentPreview);
                }
            }

            if (GUILayout.Button("Next Seat", GUILayout.Width(70), GUILayout.Height(BUTTON_HEIGHT)))
            {

                currentPosIdx++;
                if (currentPosIdx >= listActionPos.Count)
                    currentPosIdx = 0;

                //Debug.LogWarning(currentPosIdx + " " + listActionPos.Length);
            }
        }
        else
        {
            if (GUILayout.Button("Test", GUILayout.Width(40), GUILayout.Height(BUTTON_HEIGHT)))
            {
                if (currentPreview == 0)
                    currentPreview = 1;
                else
                    currentPreview = 0;

                PreviewModel(obj, actionPos, currentPreview);
            }

            if (GUILayout.Button("Stand", GUILayout.Width(50), GUILayout.Height(BUTTON_HEIGHT)))
            {
                editStand = true;
                if (testModel != null)
                    PreviewModel(obj, actionPos, 0);
            }

            if (GUILayout.Button("Seat", GUILayout.Width(40), GUILayout.Height(BUTTON_HEIGHT)))
            {
                editSeat = true;
                if (testModel != null)
                    PreviewModel(obj, actionPos, 1);
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        // End
        GUILayout.EndArea();
        Handles.EndGUI();

        if (rect.Contains(Event.current.mousePosition))
        {


            if (Event.current.type == EventType.MouseDown ||
                Event.current.type == EventType.MouseUp)
            {
                //Debug.LogWarning("Consume");
                Event.current.Use();
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }*/
    }

    
}

