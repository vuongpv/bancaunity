using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIRating : UIBaseDialogHandler
{
    public Transform starContainter;
    public UILabel coinValue;

    UISprite[] stars;
    int currentStar;

    public override void OnInit()
    {
        stars = new UISprite[5];
        for (int i = 0; i <5 ;i ++)
            stars[i] = starContainter.FindChild("Star" + (i + 1).ToString()).gameObject.GetComponentInChildren<UISprite>();
    }

    public override void OnBeginShow(object parameter)
    {
        coinValue.text = FHGameConstant.RATING_GOLD_BONUS.ToString();

        for (int i = 0; i < 5; i++)
            SetStarStatus(i, true);
        
        currentStar = 5;
    }

    void OnClick()
    {
        GameObject obj = UICamera.selectedObject;

        switch (obj.name)
        {
            case "RateLater":
                Hide();
                break;

            case "RateNow":
                OpenRatingWindow();               
                Hide();
                break;

            case "Star1":
            case "Star2":
            case "Star3":
            case "Star4":
            case "Star5":
                currentStar = int.Parse(obj.name.Substring(obj.name.Length - 1));
                SetSelectStar(currentStar);
                break;
        }
    }

    void SetStarStatus(int id, bool enable)
    {
        stars[id].spriteName = (enable ? "star_rate" : "star_empty");
        stars[id].MakePixelPerfect();
    }

    void SetSelectStar(int id)
    {
        for (int i = 0; i < 5; i++)
            if (i < id)
                SetStarStatus(i, true);
            else
                SetStarStatus(i, false);
    }

    void OpenRatingWindow()
    {
        FHPlayerProfile.instance.lastTimeShowRating = -1;
        
        FHPlayerProfile.instance.gold += FHGameConstant.RATING_GOLD_BONUS;
        FHPlayerProfile.instance.ForceSave();
        FHGoldHudPanel.instance.UpdateGold();

        FHUtils.OpenAppStore(FHSystem.instance.appIdentifier);
    }

    void Hide()
    {
        GuiManager.HidePanel(GuiManager.instance.guiRating);
    }
}