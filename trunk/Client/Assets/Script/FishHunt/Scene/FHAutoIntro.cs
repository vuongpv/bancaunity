using UnityEngine;
using System.Collections;

public class FHAutoIntro : MonoBehaviour
{
	public bool autoIntro = true;

	void Awake()
    {
        if (GameObject.Find("SceneManager") == null)
        {
			Debug.Log("FHAutoIntro = " + autoIntro);
			if(autoIntro)
            	Application.LoadLevel(FHScenes.Intro);
        }
	}
}
