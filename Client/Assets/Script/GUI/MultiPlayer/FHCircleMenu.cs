using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FH.MessageBox;
public enum FHCircleMenuType
{
    None = -1,
    GoldPacks = 0,
    Skills = 1,
}

public enum FHCircleMenuRotatingType
{
    None = -1,
    Show = 0,
    Hide = 1,
}

public class FHCircleMenu : MonoBehaviour
{
    public FHPlayerMultiController player;

    public GameObject goldPacksMenu;
    public GameObject skillsMenu;

    public float startAngleShowCW = 160.0f;
    public float endAngleShowCW = 0.0f;

    public float startAngleHideCW = 0.0f;
    public float endAngleHideCW = -160.0f;

    float rotatingSpeed = 500.0f;

    public float direction;
    float startAngle, endAngle, angle;

    FHCircleMenuType currentMenuType;
    bool isShowed = false;
    FHCircleMenuRotatingType rotatingType;

    Transform _transform;
    Vector3 initPos;

    void Start()
    {
        _transform = gameObject.transform;

        initPos = new Vector3(0.0f, -1000.0f, 0.0f);
        _transform.localEulerAngles = new Vector3(0.0f, 0.0f, startAngleShowCW);
        _transform.localPosition = initPos;

        currentMenuType = FHCircleMenuType.None;

        isShowed = false;
        rotatingType = FHCircleMenuRotatingType.None;
    }

    public bool IsShowed()
    {
        return isShowed;
    }

    public FHCircleMenuType GetCurrentMenuType()
    {
        return currentMenuType;
    }

    public FHCircleMenuRotatingType GetCurrentRotatingType()
    {
        return rotatingType;
    }

    public void Toggle()
    {
        if (currentMenuType != FHCircleMenuType.None)
            Toggle(currentMenuType);
    }

    public void Toggle(FHCircleMenuType menuType)
    {
        if (FHMultiPlayerManager.instance.sharingCoinObj != null)
            return;

        if (currentMenuType != menuType)
            Show(menuType);
        else
        {
            if (rotatingType == FHCircleMenuRotatingType.None)
            {
                if (isShowed)
                    rotatingType = FHCircleMenuRotatingType.Hide;
                else
                    rotatingType = FHCircleMenuRotatingType.Show;
            }
            else
            {
                if (rotatingType == FHCircleMenuRotatingType.Show)
                    rotatingType = FHCircleMenuRotatingType.Hide;
                else
                    rotatingType = FHCircleMenuRotatingType.Show;
            }

            direction = -direction;
            SetEndAngle();
        }
    }

    void Show(FHCircleMenuType menuType)
    {
        HideAllMenus();

        rotatingType = FHCircleMenuRotatingType.Show;

        GameObject menu = null;
        switch (menuType)
        {
            case FHCircleMenuType.GoldPacks:
                menu = goldPacksMenu;
                direction = -1.0f;
                menu.GetComponent<UIShareGoldPacksMenu>().Setup(player);
                break;

            case FHCircleMenuType.Skills:
                menu = skillsMenu;
                direction = 1.0f;
                break;
        }

        menu.SetActiveRecursively(true);
        currentMenuType = menuType;

        SetupGUI(menuType, menu);

        SetStartAngle();
        SetEndAngle();

        _transform.localPosition = Vector3.zero;
    }

    void SetupGUI(FHCircleMenuType menuType, GameObject menu)
    {
        switch (menuType)
        {
            case FHCircleMenuType.GoldPacks:
                menu.GetComponent<UIShareGoldPacksMenu>().Setup(player);
                break;

            case FHCircleMenuType.Skills:
                player.UpdatePowerupIcons();
                break;
        }
    }

    void Hide()
    {
        rotatingType = FHCircleMenuRotatingType.Hide;
        direction = -direction;
        SetEndAngle();
    }

    void HideAllMenus()
    {
        goldPacksMenu.SetActiveRecursively(false);
        skillsMenu.SetActiveRecursively(false);
    }

    void SetStartAngle()
    {
        if (rotatingType == FHCircleMenuRotatingType.Show)
        {
            if (direction < 0) // Clockwise
                angle = startAngleShowCW;
            else
                angle = -startAngleShowCW;
        }
        else
            angle = startAngleHideCW;
    }

    void SetEndAngle()
    {
        if (rotatingType == FHCircleMenuRotatingType.Show)
            endAngle = endAngleShowCW;
        else
        {
            if (direction < 0) // Clockwise
                endAngle = endAngleHideCW;
            else
                endAngle = -endAngleHideCW;
        }
    }

    void Update()
    {
        if (rotatingType == FHCircleMenuRotatingType.None)
            return;

        angle += direction * rotatingSpeed * Time.deltaTime;

        if ((direction > 0  && angle >= endAngle) || (direction < 0  && angle <= endAngle))
        {
            angle = endAngle;

            if (rotatingType == FHCircleMenuRotatingType.Show)
            {
                isShowed = true;
                rotatingType = FHCircleMenuRotatingType.None;
            }
            else
            if (rotatingType == FHCircleMenuRotatingType.Hide)
            {
                isShowed = false;
                rotatingType = FHCircleMenuRotatingType.None;

                _transform.localPosition = initPos;
                currentMenuType = FHCircleMenuType.None;
            }
        }

        _transform.localEulerAngles = new Vector3(_transform.localEulerAngles.x, _transform.localEulerAngles.y, angle);
    }

    void OnClick()
    {
        switch (UICamera.selectedObject.name)
        {
            case "BuyCoin":
                BuyShop();
                break;
        }
    }
    void BuyShop()
    {
        FHHttpClient.GetShopConfig(FHSystem.instance.GetFullAppID(),
            (code, json) =>
            {
                if (code == FHResultCode.OK)
                {
                    FHSystem.instance.shopConfig = json;
                    GuiManager.ShowPanel(GuiManager.instance.guiShopHandler);
                }
                else
                {
                    GUIMessageDialog.Show((result) =>
                    {
                        if (result == DialogResult.Ok)
                            BuyShop();

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