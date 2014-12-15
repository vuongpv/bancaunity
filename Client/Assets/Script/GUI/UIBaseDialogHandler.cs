using UnityEngine;
using System.Collections;

public class UIBaseDialogHandler : MonoBehaviour {

    [HideInInspector]
    public UIPanel uiPanel;
    public void Initial()
    {
        UIAnchor[] anchors = gameObject.GetComponentsInChildren<UIAnchor>();
        for (int i = 0; i < anchors.Length; i++)
        {
            anchors[i].uiCamera = GuiManager.instance.GetUICamera();
        }
        if (uiPanel == null)
        {
            uiPanel = gameObject.GetComponent<UIPanel>();
        }
    }

	public void Awake()
    {
        Initial();

		OnInit();
    }

	public virtual void OnInit()
	{

	}

    public virtual void OnBeginShow(object parameter)
    {

    }

    public virtual void OnBeginHide(object parameter)
    {

    }
}
