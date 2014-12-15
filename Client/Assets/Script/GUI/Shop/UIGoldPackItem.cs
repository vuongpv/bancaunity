using UnityEngine;
using System.Collections;

public enum UIGoldPackItemState
{
    Normal = 0,
    Confirm = 1,
}

public class UIGoldPackItem : MonoBehaviour
{
    const float ROTATING_SPEED = 600.0f;

    public GameObject normalContainer;
    public GameObject confirmContainer;

    public UILabel goldValue;

    public UILabel cashValue;

    public UISprite tag;

    public UISprite packIcon;

    public UISprite cashIcon;

    public UISprite confirmText;

    public UIGoldPackItemState currentState;
    UIGoldPackItemState targetState;
    GameObject rotatingContainer = null;
    GameObject targetContainer;
    Vector3 rotatingAngle;

    UISelectGoldPack manager = null;
    ConfigGoldPackRecord pack = null;

    TweenScale tweenScale = null;

    public void Disable()
    {
        gameObject.SetActiveRecursively(false);
    }

    public void Setup(UISelectGoldPack _manager, ConfigGoldPackRecord _pack)
    {
        manager = _manager;
        pack = _pack;

        gameObject.SetActiveRecursively(true);

        UIHelper.EnableWidget(normalContainer);
        normalContainer.transform.localEulerAngles = Vector3.zero;
        SetupGUI_Normal();

        UIHelper.EnableWidget(confirmContainer);
        confirmContainer.transform.localEulerAngles = Vector3.zero;
        confirmContainer.SetActiveRecursively(false);

        rotatingContainer = null;
        currentState = UIGoldPackItemState.Normal;
        SwitchState(UIGoldPackItemState.Normal);

        manager.DisableNote();
    }

    public void SwitchState(UIGoldPackItemState _state)
    {
        targetState = _state;
        if (targetState == currentState)
            return;

        switch (currentState)
        {
            case UIGoldPackItemState.Normal:
                rotatingContainer = normalContainer;
                targetContainer = confirmContainer;
                
                if ((FHPayPortIndex)pack.payPortID == FHPayPortIndex.SMS)
                    manager.EnableNote(pack.cashValue);
                break;

            case UIGoldPackItemState.Confirm:
                rotatingContainer = confirmContainer;
                targetContainer = normalContainer;
                break;
        }

        // Init GUI before rotating
        UIHelper.DisableCollider(rotatingContainer);

        targetContainer.SetActiveRecursively(true);
        targetContainer.transform.localEulerAngles = new Vector3(0.0f, 270.0f, 0.0f);
        UIHelper.DisableCollider(targetContainer);

        rotatingAngle = Vector3.zero;
    }
                  
    void Update()
    {
        if (rotatingContainer == null)
            return;

        rotatingAngle.y += ROTATING_SPEED * Time.deltaTime;

        if (rotatingContainer != targetContainer)
        {
            if (rotatingAngle.y >= 90.0f)
            {
                rotatingContainer.SetActiveRecursively(false);
                SetupGUI(targetState);

                rotatingContainer = targetContainer;
                rotatingAngle = new Vector3(0.0f, 270.0f, 0.0f);
            }
            else
                rotatingContainer.transform.localEulerAngles = rotatingAngle;
        }
        else
        {
            if (rotatingAngle.y >= 360.0f)
            {
                targetContainer.transform.localEulerAngles = Vector3.zero;
                UIHelper.EnableCollider(targetContainer);

                rotatingContainer = null;
                rotatingAngle = Vector3.zero;

                currentState = targetState;
            }
            else
                rotatingContainer.transform.localEulerAngles = rotatingAngle;
        }
    }

    void SetupGUI(UIGoldPackItemState state)
    {
        switch (state)
        {
            case UIGoldPackItemState.Normal:
                SetupGUI_Normal();
                break;

            case UIGoldPackItemState.Confirm:
                SetupGUI_Confirm();
                break;
        }
    }

    void SetupGUI_Confirm()
    {
        gameObject.transform.localScale = Vector3.one;
        if (tweenScale != null)
            tweenScale.enabled = false;

        FHPayPortIndex payPortID = (FHPayPortIndex)pack.payPortID;

        switch (payPortID)
        {
            case FHPayPortIndex.Diamond:
                confirmText.spriteName = "shop_text_EXCHANGE";
                break;

            case FHPayPortIndex.SMS:
                confirmText.spriteName = "shop_text_SENDSMS";
                break;

            default:
                confirmText.spriteName = "shop_text_SENDSMS";
                break;
        }

        confirmText.MakePixelPerfect();
    }

    void SetupGUI_Normal()
    {
        goldValue.text = pack.goldValue.ToString("0,0") + "\n" + (pack.goldBonus > 0 ? ("+" + pack.goldBonus.ToString("0,0")) : "");

        string cashText = Reduce(string.Format("{0:N}", pack.cashValue));
        cashIcon.gameObject.SetActiveRecursively(false);
        if (pack.cashType == "Diamond")
        {
            cashText = cashText;
            cashIcon.gameObject.SetActiveRecursively(true);
        }
        else
        if (pack.cashType == "$")
            cashText = pack.cashType + cashText;
        else
            cashText = cashText + " " + pack.cashType;
        cashValue.text = cashText;

        tag.GetComponent<UIShopTag>().Setup(pack.tag);

        packIcon.spriteName = (pack.packType == (int)FHShopPackType.Gold ? "coin" : "kimcuong_icon");
        packIcon.MakePixelPerfect();

        gameObject.transform.localScale = Vector3.one;
        if ((pack.tag.Contains("hot") || pack.tag.Contains("best")))
        {
            if (tweenScale == null)
            {
                tweenScale = TweenScale.Begin(gameObject, 0.5f, new Vector3(1.05f, 1.05f, 1.05f));
                tweenScale.method = UITweener.Method.EaseInOut;
                tweenScale.style = UITweener.Style.PingPong;
            }
            else
                tweenScale.enabled = true;
        }
        else
        {
            if (tweenScale != null)
                tweenScale.enabled = false;
        }
    }

    void OnClick()
    {
        if (pack == null)
            return;

        if (manager.IsNeedEnterCode())
        {
            manager.NotifySwitchContent(pack);
            return;
        }

        if (!manager.IsNeedConfirm())
        {
            OnClickConfirm();
            return;
        }

        GameObject obj = UICamera.selectedObject;

        switch (obj.name)
        {
            case "PackNormal":
                OnClickNormal();
                break;

            case "PackConfirm":
                OnClickConfirm();
                break;
        }
    }

    void OnClickNormal()
    {
        manager.Reset();

        SwitchState(UIGoldPackItemState.Confirm);
    }

    void OnClickConfirm()
    {
        FHPayPortIndex payPortID = (FHPayPortIndex)pack.payPortID;

        switch (payPortID)
        {
            case FHPayPortIndex.SMS:
                manager.RequestSMSPayment(pack);
                break;

            case FHPayPortIndex.PlayStore:
                manager.RequestPlayStorePayment(pack);
                break;

            case FHPayPortIndex.PlayStore_HD:
                manager.RequestPlayStorePayment(pack);
                break;

			case FHPayPortIndex.AppleStore:
				manager.RequestAppleStorePayment(pack);
                break;

            case FHPayPortIndex.ZingCard:
                break;

            case FHPayPortIndex.Diamond:
                manager.RequestDiamondExchange(pack);
                break;
        }
    }
    
    string Reduce(string str)
    {
        while (str.Length > 0 && str[str.Length - 1] == '0')
            str = str.Remove(str.Length - 1);

        if (str.Length > 0 && str[str.Length - 1] == '.')
            str = str.Remove(str.Length - 1);

        return str;
    }
}