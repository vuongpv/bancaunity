using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
[CustomEditor(typeof(FGCustomInfo))]
public class FGCustomInfoInspector : Editor
{
    private FGCustomInfo fgCustomInfo;
    private bool oldSimulation = false;
    private int countCustomEvent = -1;
    private static string[] fishOnConfig = null;
    private FGFishCollect[] fishCollect = null;

    public override void OnInspectorGUI()
    {
        fgCustomInfo = (FGCustomInfo)target;
        EditorGUILayout.Separator();
        fgCustomInfo.fgType = (FGType)EditorGUILayout.EnumPopup("Type Fish Group", fgCustomInfo.fgType);
        if (fgCustomInfo.fgType == FGType.GROUP_LOOP)
        {
            GUILayout.Label("All member in sub Node will be respawn again", EditorStyles.label);
            fgCustomInfo.loopCountRespawn = EditorGUILayout.IntField("Loop Count Respawn ", fgCustomInfo.loopCountRespawn);
            fgCustomInfo.timeRespawn = EditorGUILayout.FloatField("Time Respawn ", fgCustomInfo.timeRespawn);
        }

        EditorGUILayout.Space();
        fgCustomInfo.baseSpeed = (FishSpeed)EditorGUILayout.EnumPopup("Fish speed", fgCustomInfo.baseSpeed);
        EditorGUILayout.Space();
        GUILayout.Label("Custom Editor", EditorStyles.boldLabel);
        fgCustomInfo.sizeGizmos = EditorGUILayout.FloatField("gizmos radius", fgCustomInfo.sizeGizmos);
        fgCustomInfo.colorGizmos = EditorGUILayout.ColorField("gizmos color", fgCustomInfo.colorGizmos);
        fgCustomInfo.colorLine = EditorGUILayout.ColorField("Line color", fgCustomInfo.colorLine);

        fgCustomInfo.isSimulation = EditorGUILayout.Toggle("simulation (preview)", fgCustomInfo.isSimulation);
        EditorGUILayout.Space();
        fgCustomInfo.countCustomEvent = EditorGUILayout.IntField("Event Count", fgCustomInfo.countCustomEvent);
        if (fgCustomInfo.countCustomEvent < 0)
        {
            fgCustomInfo.countCustomEvent = 0;
        }
        if (fgCustomInfo.countCustomEvent != countCustomEvent)
        {
            // to do
            if (countCustomEvent < fgCustomInfo.countCustomEvent)
            {
                //Debug.LogError("Count editor:"+countCustomEvent);
                countCustomEvent = fgCustomInfo.countCustomEvent;
                List<FGCustomEvent> list = new List<FGCustomEvent>();
                for (int i = 0; i < countCustomEvent; i++)
                {
                    if (i < fgCustomInfo.customEvent.Count)
                        list.Add(fgCustomInfo.customEvent[i]);
                    else
                        list.Add(new FGCustomEvent());
                }
                // change
                fgCustomInfo.customEvent = list;
            }
            else
            {
                countCustomEvent = fgCustomInfo.countCustomEvent;

                List<FGCustomEvent> list = new List<FGCustomEvent>();
                for (int i = 0; i < countCustomEvent; i++)
                {
                    if (i < fgCustomInfo.customEvent.Count)
                    {
                        list.Add(fgCustomInfo.customEvent[i]);
                    }
                    else
                    {
                        list.Add(new FGCustomEvent());
                    }
                }
                // change
                fgCustomInfo.customEvent = list;
            }
        }
        if (fgCustomInfo.countCustomEvent > 0)
        {

            for (int i = 0; i < fgCustomInfo.customEvent.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();

                GUILayout.Label(i.ToString(), EditorStyles.largeLabel);

                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();

                FGCustomEvent _event = fgCustomInfo.customEvent[i];
                _event.fGKindEvent = (FGKindEvent)EditorGUILayout.EnumPopup("Type Event", _event.fGKindEvent);
                _event.distanceEvent = EditorGUILayout.FloatField("Distance Appear (0->1)", _event.distanceEvent);
                if (_event.fGKindEvent == FGKindEvent.VELOCITY)
                {
                    _event.velocityChange = EditorGUILayout.FloatField("Velocity)", _event.velocityChange);
                }
                else if (_event.fGKindEvent == FGKindEvent.CURVE)
                {
                    _event.heightSin = EditorGUILayout.FloatField("Height Sin)", _event.heightSin);
                    _event.loopSin = EditorGUILayout.IntField("Loop Sin)", _event.loopSin);
                }
                fgCustomInfo.customEvent[i] = _event;
                //Debug.LogError(fgCustomInfo.customEvent.Count);
                //Debug.LogError(_event.fGKindEvent);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

            }

        }
        if (oldSimulation != fgCustomInfo.isSimulation)
        {
            //fgCustomInfo.OnDrawGizmos();
            SceneView.RepaintAll();

            oldSimulation = fgCustomInfo.isSimulation;
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

 
}
