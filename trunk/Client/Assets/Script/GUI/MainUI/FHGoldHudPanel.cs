using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using FH.MessageBox;

public class FHGoldHudPanel : MonoBehaviour, ICollectibleTarget
{
    public FHPlayerController controller;

    public UILabel goldLabel;
    public UILabel playerName;

    public UISprite coinOver;
    public float coinWidth;
    public float coinHeight;

    private Tweener coinOverTween;
    private TweenParms coinOverTweenParam;

    public Transform coinAnchor;

    public GameObject outOfCoin;

    public static FHGoldHudPanel instance;

    private int uiTargetGold = -1;
    public int uiCurGold;

    private int minCoin = 10;

    void Awake()
    {
        if (instance == null)
            instance = this;

        outOfCoin.SetActiveRecursively(false);
        coinOver.gameObject.active = false;

        coinOverTweenParam = new TweenParms()
            .Prop("localScale", new Vector3(coinWidth, coinHeight, 1f))
            .OnComplete(() =>
            {
                coinOver.gameObject.active = false;
                coinOverTween = null;
            });

        UpdateGold();
    }

    void Start()
    {
    }

    void OnDestroy()
    {
        if (instance = this)
            instance = null;
    }

    void OnClick()
    {
        switch (UICamera.selectedObject.name)
        {
            case "AddGoldBtn":
				ShowShop();
                break;

            case "DailyGiftBtn":
                GuiManager.ShowPanel(GuiManager.instance.guiDailyGift);
                break;

            case "ShareCoinBtn":
                if (FHMultiPlayerManager.instance.sharingCoinObj == null)
                {
                    FHCircleMenuManager.instance.HideAllGoldPacksMenusExcept(((FHPlayerMultiController)controller).circleMenu);
                    ((FHPlayerMultiController)controller).ToggleCircleMenu(FHCircleMenuType.GoldPacks);
                }
                break;
        }
    }

    public void SetGold(int gold)
    {
        goldLabel.text = gold.ToString("0,0");
    }

    public void StartOutOfCoinNotify()
    {
        if (uiTargetGold < minCoin)
        {
            outOfCoin.SetActiveRecursively(true);
            StopCoroutine("WaitForOutOfCoin");
            StartCoroutine("WaitForOutOfCoin");
        }
    }

    public void SetMinCoin(int minCoin)
    {
        this.minCoin = minCoin;
        /*if (uiTargetGold < minCoin)
            outOfCoin.SetActiveRecursively(true);
        else if (outOfCoin.gameObject.active == true)
            outOfCoin.SetActiveRecursively(false);*/
    }

    public void UpdateGold()
    {
        if ((controller != null && uiTargetGold == controller.gold) || (controller == null && uiTargetGold == FHPlayerProfile.instance.gold))
            return;

        if (controller != null)
            uiTargetGold = controller.gold;
        else
            uiTargetGold = FHPlayerProfile.instance.gold;

        if (uiTargetGold > minCoin && outOfCoin.gameObject.active == true)
            outOfCoin.SetActiveRecursively(false);

        // Gold
        HOTween.To(this, 1.5f, new TweenParms()
            .Prop("uiCurGold", uiTargetGold)
            .Delay(0f)
            .Ease(EaseType.Linear)
            .OnUpdate(() => { SetGold(uiCurGold); })
        );
    }
    public void UpdateUI(int gold, string name)
    {
        // just using for online: gold online not using FHPlayerProfile
        if (playerName != null)
        {
            playerName.text = name;
        }
        if (uiTargetGold == gold)
        {
            return;
        }
        uiTargetGold = gold;

        // Gold
        HOTween.To(this, 1.5f, new TweenParms()
            .Prop("uiCurGold", uiTargetGold)
            .Delay(0f)
            .Ease(EaseType.Linear)
            .OnUpdate(() => { SetGold(uiCurGold); })
        );
    }

    public Vector3 GetTargetPos(string type)
    {
        if (coinAnchor != null)
            return coinAnchor.position;
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    public void OnReachTarget(string type)
    {
        if (coinOverTween != null)
            HOTween.Kill(coinOverTween);

        coinOver.gameObject.active = true;
        coinOver.transform.localScale = new Vector3(0.2f, 0.2f, 1);
        coinOverTween = HOTween.Punch(coinOver.transform, 0.5f, coinOverTweenParam, 0.5f, 0.2f);
    }


    IEnumerator WaitForOutOfCoin()
    {
        yield return new WaitForSeconds(3);

        outOfCoin.SetActiveRecursively(false);
    }

    void EnablePaymentResult(bool result)
    {
        if (result)
            GuiManager.ShowPanel(GuiManager.instance.guiShopHandler);
        else
            GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.PAYMENT_DISABLED),
                FHLocalization.instance.GetString(FHStringConst.PAYMENT_DIALOG_TITLE),
                FH.MessageBox.MessageBoxButtons.OK);
    }

	void ShowShop()
	{
		FHHttpClient.GetShopConfig(FHSystem.instance.GetFullAppID(),
			(code, json) =>
			{
				if (code == FHResultCode.OK)
				{
					FHSystem.instance.shopConfig = json;
					UIShopHandler.Show();
				}
				else
				{
					GUIMessageDialog.Show((result) =>
					{
						if (result == DialogResult.Ok)
							ShowShop();

						return true;
					},
						FHLocalization.instance.GetString(FHStringConst.ENABLE_NET),
						FHLocalization.instance.GetString(FHStringConst.PAYMENT_DIALOG_TITLE),
						FH.MessageBox.MessageBoxButtons.OKCancel
					);
				}
			}
		);
	}
}