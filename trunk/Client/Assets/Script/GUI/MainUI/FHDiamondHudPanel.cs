using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public class FHDiamondHudPanel : MonoBehaviour
{
    public static FHDiamondHudPanel instance;

    public UILabel diamondLbl;
    
    public int currentDiamond;

    void Awake()
	{
		if (instance == null)
			instance = this;

        currentDiamond = 0;
    }

    void Start()
    {
        UpdateDiamond();
    }

	void OnDestroy()
	{
		if( instance = this )
			instance = null;
	}

	void OnClick()
	{
		switch (UICamera.selectedObject.name)
		{
            case "AddDiamond":
                GuiManager.ShowPanel(GuiManager.instance.guiShopHandler, FHShopPackType.Diamond);
                break;

            case "Exchange":
                GuiManager.ShowPanel(GuiManager.instance.guiCardExchange);
                break;
        }
	}

    public void SetDiamond(int diamond)
    {
        diamondLbl.text = diamond.ToString("0,0");
    }

    public void UpdateDiamond()
    {
        int targetDiamond =  FHPlayerProfile.instance.diamond;

        HOTween.To(this, 1.5f, new TweenParms()
            .Prop("currentDiamond", targetDiamond)
            .Delay(0f)
            .Ease(EaseType.Linear)
            .OnUpdate(() => { SetDiamond(currentDiamond); })
        );
    }

    public void SyncDiamond()
    {
        // Connect to server to synchronize diamond value

        UpdateDiamond();
    }
}