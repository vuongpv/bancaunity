using UnityEngine;
using System.Collections;

public class FHCircleMenuManager : SingletonMono<FHCircleMenuManager>
{
    public FHCircleMenu[] circleMenus;

    public void Init()
    {
        circleMenus = (FHCircleMenu[])GameObject.FindSceneObjectsOfType(typeof(FHCircleMenu));
    }

    public void HideAllGoldPacksMenusExcept(FHCircleMenu menu)
    {
        for (int i = 0; i < circleMenus.Length; i++)
        {
            if (circleMenus[i] == menu)
                continue;

            if ((circleMenus[i].IsShowed() && circleMenus[i].GetCurrentMenuType() == FHCircleMenuType.GoldPacks)
                || (circleMenus[i].GetCurrentRotatingType() == FHCircleMenuRotatingType.Show)
            )
                circleMenus[i].Toggle(FHCircleMenuType.GoldPacks);
        }
    }
}