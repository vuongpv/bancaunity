using UnityEngine;
using System.Collections;

public class FishSpriteSetup : MonoBehaviour {
    public GameObject background;

    Rect btnSwitchStateRect = new Rect(5, 5, 65, 30);
    Rect btnSwitchBackgroundRect = new Rect(75, 5, 105, 30);
    
    int state = 0;

    void Start()
    {
        Play();
    }

    void OnGUI()
    {
        if (GUI.Button(btnSwitchStateRect, "Anim"))
        {
            state = 1 - state;
            Play();
        }

        if (GUI.Button(btnSwitchBackgroundRect, "Background"))
        {
            background.SetActiveRecursively(!background.active);
        }
    }

    void Play()
    {
        object[] objs = GameObject.FindSceneObjectsOfType(typeof(PackedSprite));
        
        for (int i = 0; i < objs.Length; i++)
        {
            PackedSprite fish = objs[i] as PackedSprite;
            fish.PlayAnim(state);
        }
    }
}