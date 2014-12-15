using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHelpHandler : UIBaseDialogHandler
{
    public UIHelpNavigator navigator;

    private string cheatCode = "";
    private int enableCheatCode = 0;

    bool startSeasonAfterClose;

    public override void OnInit()
    {
    }

    public override void OnBeginShow(object parameter)
    {
        if (navigator != null)
            navigator.Setup();

        cheatCode = "";
        enableCheatCode = 0;
        startSeasonAfterClose = false;
        navigator.ResetOnOpen();
        if (parameter != null)
            startSeasonAfterClose = (bool)parameter;
    }

    void OnClick()
    {
        GameObject obj = UICamera.selectedObject;

        int code = -1;
        switch (obj.name)
        {
            case "EnableCheat":
                enableCheatCode += 1;
                break;

            case "BtnClose":
                OnBtnClose();
                break;

            case "ArrowLeft":
                navigator.MoveLeft();
                break;

            case "ArrowRight":
                navigator.MoveRight();
                break;

            case "icon_fish_mermaid":
                code = 0;
                break;

            case "icon_fish_hammershark":
                code = 1;
                break;

            case "icon_fish_rayfish":
                code = 2;
                break;

            case "icon_fish_angelshark":
                code = 3;
                break;

            case "icon_fish_lionfish":
                code = 4;
                break;

            case "icon_fish_turtle":
                code = 5;
                break;

            case "icon_fish_lobster":
                code = 6;
                break;
            case "icon_fish_pufferfish":
                code = 7;
                break;

            case "icon_fish_squid":
                code = 8;
                break;

            case "icon_fish_jellyfish":
                code = 9;
                break;

            case "EnableFPS":
                HUDFPS hudFPS = GameObject.FindObjectOfType(typeof(HUDFPS)) as HUDFPS;
                if (hudFPS != null)
                    hudFPS.render = !hudFPS.render;
                break;

            case "ResetCheatCode":
                cheatCode = "";
                break;
        }

        if (enableCheatCode == 5)
        {
            navigator.gameObject.collider.enabled = !navigator.gameObject.collider.enabled;
            enableCheatCode = 0;
        }
        
        if (code != -1)
        {
            cheatCode += code.ToString();
            ProcessCheat();
        }
    }

    void OnBtnClose()
    {
        GuiManager.HidePanel(GuiManager.instance.guiHelp);
        if (startSeasonAfterClose)
            FHFishSeasonManager.instance.canStart = true;
    }

    void ProcessCheat()
    {
        if (!FHSystem.instance.IsEnableCheat())
            return;


        FHPlayerController controller = GameObject.FindObjectOfType(typeof(FHPlayerController)) as FHPlayerController;

        bool cheatActivated = false;

        switch (cheatCode)
        {
            case "1441":
                if (controller != null)
                {
                    controller.gold += 10000;
                    controller.goldHudPanel.UpdateGold();
                }
                else
                {
                    FHPlayerProfile.instance.gold += 10000;
                    FHGoldHudPanel.instance.UpdateGold();
                }
                cheatActivated = true;
                break;

            case "1442":
                Debug.LogError("1442");
                FHHttpClient.CheatDiamond(1000,(code, json) =>
                {
                    if (code == FHResultCode.OK)
                    {
                        Debug.LogError("FH Diamond Server:" + json.ToString());
                        int diamond = int.Parse((string)json["diamond"]);
                        FHPlayerProfile.instance.diamond = diamond;
                        FHDiamondHudPanel.instance.UpdateDiamond();
                    }
                    else
                    {
                        Debug.LogError("AAAAAA");
                    }
                });
                
                cheatActivated = true;
                break;

            case "911":
                if (controller != null)
                {
                    controller.lightning += 1;
                    controller.UpdatePowerupIcons();
                }
                cheatActivated = true;
                break;

            case "113":
                if (controller != null)
                {
                    controller.nuke += 1;
                    controller.UpdatePowerupIcons();
                }
                cheatActivated = true;
                break;

            case "6996":
                FHPlayerProfile.instance.level += 1;
                if (controller != null)
                    controller.playerHudPanel.UpdateUI();
                cheatActivated = true;
                break;
        }

        if (cheatActivated)
            cheatCode = "";
    }
}