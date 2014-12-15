using UnityEngine;
using System.Collections;

public class UIAchievesHandler : UIBaseDialogHandler
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnClick()
    {
        switch (UICamera.selectedObject.name)
        {
            case "BtnOK":
                {
                    OnBtnOK();
                }
                break;
        }
    }
    public void OnBtnOK()
    {
        GuiManager.HidePanel(GuiManager.instance.guiAchievesHandler);
    }
}
