using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIRootBase : MonoBehaviour {

	// Use this for initialization
    void Awake()
    {
        Vector3 vecScale=GuiManager.instance.gameObject.transform.localScale;
        gameObject.transform.localScale = vecScale;
        UIAnchor[] anchors = gameObject.GetComponentsInChildren<UIAnchor>();
        for (int i = 0; i < anchors.Length; i++)
        {
            anchors[i].uiCamera = GuiManager.instance.uiCamera;
        }
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
