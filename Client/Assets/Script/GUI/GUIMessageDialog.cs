using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FH.MessageBox;


public class GUIMessageDialog : GUIDialogBase
{
    #region Dialog Button Control
    public class DialogBtnControl
    {
        public Transform tranformBtn;
        public UILabel text;
        public DialogResult result;
        public DialogBtnControl(Transform _tran, UILabel _labelText)
        {
            tranformBtn = _tran;
            text = _labelText;
        }
        public void Reset()
        {
            result = DialogResult.None;
            text.text = "";
            tranformBtn.gameObject.SetActiveRecursively(false);
        }
        public void SetInfomation(DialogResult _result, string _text, Vector3 _location)
        {
            tranformBtn.gameObject.SetActiveRecursively(true);
            result = _result;
            text.text = _text;
            tranformBtn.localPosition = _location;
        }
    }
    #endregion

    // content
    private UILabel contentMessage;
    private UILabel captionText;

    // image title 
    private UISprite imageTitle;

    // location btn first save
    public Vector3 location1Btn;
    public Vector3 location2Btn;
    public Vector3 location3Btn;

    public float distance = 125.0f;
    // current Result for three button
    private DialogBtnControl[] btnDialog = new DialogBtnControl[3];

    // control static
    private static List<MessageItem> items = new List<MessageItem>();
    private static GUIMessageDialog messageHandler;
    public void Awake()
    {
        messageHandler = this;
    }


    //********************* Overide *********************//
    public override GameObject OnInit()
    {
        //Debug.Log("dialog messagebox on init");
        GameObject obj = GameObject.Instantiate(Resources.Load(GUI_PATH_PREFAB + "UIMessageDialog")) as GameObject;
        obj.name = "UIMessageDialog";
        obj.transform.parent = gameObject.transform;
        obj.transform.localScale = new Vector3(1, 1, 1);
        guiControlLocation = obj.transform.Find("UIContent").gameObject;
        // find object
        contentMessage = guiControlLocation.transform.Find("messageText").GetComponent<UILabel>();
        captionText = guiControlLocation.transform.Find("captionText").GetComponent<UILabel>();
        for (int i = 0; i < 3; i++)
        {
            Transform _tran = guiControlLocation.transform.Find("Btn0" + (i + 1).ToString());
            _tran.gameObject.GetComponent<UIForwardEvents>().target = gameObject;
            UILabel _label = _tran.Find("staticText").GetComponent<UILabel>();
            btnDialog[i] = new DialogBtnControl(_tran, _label);
            btnDialog[i].Reset();
        }
        layer =50;

        UIWidget[] wiget = gameObject.GetComponentsInChildren<UIWidget>();
        //Debug.LogError(wiget.Length);
        for (int i = 0; i < wiget.Length; i++)
        {
            wiget[i].depth += (layer +50);
        }
        return obj;
    }

    protected override float OnBeginShow(object parameter)
    {
        // to do
        guiControlDlg.SetActiveRecursively(true);

        if (parameter != null)
        {
            MessageItem item = (MessageItem)parameter;
            SetupDisplayButtons(item);
            contentMessage.text = item.message;
            captionText.text = item.caption;
            guiControlLocation.transform.localPosition = new Vector3(0, 0, -100.0f);
        }
        else
        {
            Debug.LogError("Method to open Message Dialog is not exactly");
        }
        return base.OnBeginShow(parameter);
    }

    protected override float OnBeginHide(object parameter)
    {
        return base.OnBeginHide(parameter);
    }

    protected override void OnEndHide(bool isDestroy)
    {
    }

    protected override void OnEndShow()
    {
    }

    //********************  End override ****************//
    void OnClick()
    {
        switch (UICamera.selectedObject.name)
        {
            case "Btn01":
                {
                    OnBtnClick(0);
                }
                break;

            case "Btn02":
                {
                    OnBtnClick(1);
                }
                break;

            case "Btn03":
                {
                    OnBtnClick(2);
                }
                break;
        }
    }

    public void OnBtnClick(int i)
    {
        bool close = true;

        if (items[items.Count - 1].callback != null)
            close = items[items.Count - 1].callback(btnDialog[i].result);

        if (!close)
            return;

        items.RemoveAt(items.Count - 1);

        if (close && !CheckShowMessageDialog())
            GuiManager.HidePanel(GuiManager.instance.guiMessageDialogHandler);
    }

    public bool CheckShowMessageDialog()
    {
        if (items.Count > 0)
        {
            GuiManager.ShowPanel(GuiManager.instance.guiMessageDialogHandler, items[items.Count - 1]);
            return true;
        }

        return false;
    }


    #region simulator message same with .Net
    private void ResetMessageState()
    {
        for (int i = 0; i < 3; i++)
        {
            btnDialog[i].Reset();
        }
    }

    private void SetupDisplayButtons(MessageItem item)
    {
        ResetMessageState();
        switch (item.buttons)
        {
            case MessageBoxButtons.OK:
                btnDialog[0].SetInfomation(DialogResult.Ok, FHLocalization.instance.GetString(FHStringConst.LABEL_BTN_OK), location1Btn);
                break;

            case MessageBoxButtons.OKCancel:
				btnDialog[0].SetInfomation(DialogResult.Ok, FHLocalization.instance.GetString(FHStringConst.LABEL_BTN_OK), location2Btn);
				btnDialog[1].SetInfomation(DialogResult.Cancel, FHLocalization.instance.GetString(FHStringConst.LABEL_BTN_CANCEL), new Vector3(location2Btn.x + distance, location2Btn.y, location2Btn.z));
                break;

            case MessageBoxButtons.AbortRetryIgnore:
                btnDialog[0].SetInfomation(DialogResult.Abort, "About", location3Btn);
                btnDialog[1].SetInfomation(DialogResult.Retry, "Retry", new Vector3(location3Btn.x + distance, location2Btn.y, location2Btn.z));
                btnDialog[2].SetInfomation(DialogResult.Ignore, "Ignore", new Vector3(location3Btn.x + distance * 2, location2Btn.y, location2Btn.z));
                break;

            case MessageBoxButtons.YesNoCancel:
                btnDialog[0].SetInfomation(DialogResult.Yes, "YES", location3Btn);
                btnDialog[1].SetInfomation(DialogResult.No, "NO", new Vector3(location3Btn.x + distance, location2Btn.y, location2Btn.z));
                btnDialog[2].SetInfomation(DialogResult.Cancel, "CANCEL", new Vector3(location3Btn.x + distance * 2, location2Btn.y, location2Btn.z));
                break;

            case MessageBoxButtons.YesNo:
				btnDialog[0].SetInfomation(DialogResult.Yes, FHLocalization.instance.GetString(FHStringConst.LABEL_BTN_OK), location2Btn);
				btnDialog[1].SetInfomation(DialogResult.No, FHLocalization.instance.GetString(FHStringConst.LABEL_BTN_CANCEL), new Vector3(location2Btn.x + distance, location2Btn.y, location2Btn.z));
                break;

            case MessageBoxButtons.RetryCancel:
                btnDialog[0].SetInfomation(DialogResult.Retry, "RETRY", location2Btn);
                btnDialog[1].SetInfomation(DialogResult.Cancel, "CANCEL", new Vector3(location2Btn.x + distance, location2Btn.y, location2Btn.z));
                break;

            default:
                btnDialog[0].SetInfomation(DialogResult.None, "CLOSE", location1Btn);
                break;
        }
    }

    public static void Show(MessageCallback callback, string message)
    {
        Show(callback, message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
    }

    public static void Show(MessageCallback callback, string message, string caption)
    {
        Show(callback, message, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
    }

    public static void Show(MessageCallback callback, string message, string caption, MessageBoxButtons buttons)
    {
        Show(callback, message, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
    }

    public static void Show(MessageCallback callback, string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        Show(callback, message, caption, buttons, icon, MessageBoxDefaultButton.Button1);
    }

    public static void Show(MessageCallback callback, string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
    {
        MessageItem item = new MessageItem
        {
            caption = caption,
            buttons = buttons,
            defaultButton = defaultButton,
            callback = callback
        };
        item.message = message;
        switch (icon)
        {
            case MessageBoxIcon.Hand:
            case MessageBoxIcon.Stop:
            case MessageBoxIcon.Error:
                //item.message.image = messageHandler.error;
                break;

            case MessageBoxIcon.Exclamation:
            case MessageBoxIcon.Warning:
                //item.message.image = messageHandler.warning;
                break;

            case MessageBoxIcon.Asterisk:
            case MessageBoxIcon.Information:
                //item.message.image = messageHandler.info;
                break;
        }
        if (items.Count > 2)
        {
            items.RemoveAt(0);
        }
        items.Add(item);

        GuiManager.ShowPanel(GuiManager.instance.guiMessageDialogHandler, item);
    }
    #endregion


}
