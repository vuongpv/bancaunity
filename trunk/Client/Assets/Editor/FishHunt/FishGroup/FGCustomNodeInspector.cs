using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FGCustomNode))]
public class FGCustomNodeInspector : Editor
{
    private FGCustomNode fGCustomNode;
    private int countCustomEvent = -1;

    public override void OnInspectorGUI()
    {
        fGCustomNode = (FGCustomNode)target;

        fGCustomNode.heightSin = EditorGUILayout.FloatField("HeightSin", fGCustomNode.heightSin);
        fGCustomNode.loopSin = EditorGUILayout.IntField("Loop Sin", fGCustomNode.loopSin);
        fGCustomNode.timeAppear = EditorGUILayout.FloatField("Time Appear", fGCustomNode.timeAppear);
        fGCustomNode.fishSpeed = (FishSpeed)EditorGUILayout.EnumPopup("Fish speed", fGCustomNode.fishSpeed);
        EditorGUILayout.Space();
        fGCustomNode.countCustomEvent = EditorGUILayout.IntField("Event Count", fGCustomNode.countCustomEvent);
        if (fGCustomNode.countCustomEvent < 0)
        {
            fGCustomNode.countCustomEvent = 0;
        }
        if (fGCustomNode.countCustomEvent != countCustomEvent)
        {
            // to do
            if (countCustomEvent < fGCustomNode.countCustomEvent)
            {
                //Debug.LogError("Count editor:"+countCustomEvent);
                countCustomEvent = fGCustomNode.countCustomEvent;
                List<FGCustomEvent> list = new List<FGCustomEvent>();
                for (int i = 0; i < countCustomEvent; i++)
                {
                    if (i < fGCustomNode.customEvent.Count)
                        list.Add(fGCustomNode.customEvent[i]);
                    else
                        list.Add(new FGCustomEvent());
                }
                // change
                fGCustomNode.customEvent = list;
            }
            else
            {
                countCustomEvent = fGCustomNode.countCustomEvent;

                List<FGCustomEvent> list = new List<FGCustomEvent>();
                for (int i = 0; i < countCustomEvent; i++)
                {
                    if (i < fGCustomNode.customEvent.Count)
                    {
                        list.Add(fGCustomNode.customEvent[i]);
                    }
                    else
                    {
                        list.Add(new FGCustomEvent());
                    }
                }
                // change
                fGCustomNode.customEvent = list;
            }
        }
        if (fGCustomNode.countCustomEvent > 0)
        {
            for (int i = 0; i < fGCustomNode.customEvent.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();

                GUILayout.Label(i.ToString(), EditorStyles.largeLabel);

                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                FGCustomEvent _event = fGCustomNode.customEvent[i];
                _event.fGKindEvent = (FGKindEvent)EditorGUILayout.EnumPopup("type Event", _event.fGKindEvent);
                _event.distanceEvent = EditorGUILayout.FloatField("distance appear (0->1)", _event.distanceEvent);
                if (_event.fGKindEvent == FGKindEvent.VELOCITY)
                {
                    _event.velocityChange = EditorGUILayout.FloatField("velocity)", _event.velocityChange);
                }
                else if (_event.fGKindEvent == FGKindEvent.CURVE)
                {
                    _event.heightSin = EditorGUILayout.FloatField("Height Sin)", _event.heightSin);
                    _event.loopSin = EditorGUILayout.IntField("Loop Sin)", _event.loopSin);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
