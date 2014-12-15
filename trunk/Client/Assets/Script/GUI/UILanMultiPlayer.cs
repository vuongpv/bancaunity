using UnityEngine;
using System.Collections;

public class UILanMultiPlayer : UIBaseDialogHandler{

    public class ServerItem
    {
        public GameObject objectControl;
        public UILabel serverName;
        public UILabel serverIP;
        public UILabel serverStatus;
        
        
        public GameObject lockIcon;
        public GameObject btnJoin;
        public GameObject btnJoinDis;
        public GameObject bgFull;
        public ServerItem(GameObject _objectControl)
        {
            objectControl = _objectControl;
            serverName = _objectControl.transform.Find("ServerName").GetComponent<UILabel>();
            serverIP = _objectControl.transform.Find("ServerIP").GetComponent<UILabel>();
            serverStatus = _objectControl.transform.Find("ServerStatus").GetComponent<UILabel>();
            btnJoin = _objectControl.transform.Find("BtnJoin").gameObject;
            btnJoinDis = _objectControl.transform.Find("BtnJoinDisable").gameObject;
            lockIcon = _objectControl.transform.Find("LockIcon").gameObject;
            bgFull = _objectControl.transform.Find("BGInsideFull").gameObject;
        }
        public void Reset()
        {
            objectControl.SetActiveRecursively(false);
        }
        public void FillData(string _serverName, string _serverIP, string _serverStatus, bool _isPassword, bool _isFull, bool _isPlay)
        {
            objectControl.SetActiveRecursively(true);
            serverName.text = _serverName;
            serverIP.text = _serverIP;
            serverStatus.text = _serverStatus;
            if (_isPassword)
            {
                lockIcon.SetActiveRecursively(true);
            }
            else
            {
                lockIcon.SetActiveRecursively(false);
            }
            
            if (!_isFull &&!_isPlay)
            {
                btnJoinDis.SetActiveRecursively(false);
                btnJoin.SetActiveRecursively(true);
                bgFull.SetActiveRecursively(false);
            }
            else
            {
                btnJoinDis.SetActiveRecursively(true);
                btnJoin.SetActiveRecursively(false);
            }
        }
    }

    public UILabel playerName;
    public UILabel messageLabel;
    public UILabel userIP;
    public string currentplayerName;
    public ServerItem[] serverItem = new ServerItem[3];
    private HostData[] hostData = null;
    private HostData currentHostChoose = null;
    public int currentIndex = 0;
    public int maxIndex=0;
    public GameObject objectChooseRoom;
    public GameObject objectLogin;
    public UILabel loginserverName;
    public UILabel loginServerPass;
    public void Awake()
    {
        base.Awake();
    }
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (currentplayerName != playerName.text)
        {
            currentplayerName = playerName.text;
            FHUtils.SetPlayerName(currentplayerName);
        }
	}

    public override void OnBeginShow(object parameter)
    {
        gameObject.SetActiveRecursively(true);
        for (int i = 0; i < 3; i++)
        {
            if (serverItem[i] == null)
            {
                serverItem[i] = new ServerItem(gameObject.transform.Find("UIContent").transform.Find("ChooseRoom").transform.Find("ChooseRoom0" + (i + 1).ToString()).gameObject);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (serverItem[i] != null)
            {
                serverItem[i].Reset();
            }
        }
        playerName.text = FHUtils.GetPlayerName();
        hostData = FHLanNetwork.instance.GetHostServer();
        messageLabel.text = "Please Wait...";
        //RefreshHost();
        StopCoroutine("UpdateRoomRoutine");
        StartCoroutine(UpdateRoomRoutine(2));
        objectLogin.SetActiveRecursively(false);
        userIP.text = Network.player.ipAddress;
    }


    private IEnumerator UpdateRoomRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        RefreshHost();
        if (hostData.Length < 1)
        {
            StartCoroutine(UpdateRoomRoutine(time));
        }
    }
    public void RefreshHost()
    {
        hostData=FHLanNetwork.instance.GetHostServer();
        HostData[] _h = new HostData[10];
        //for (int i = 0; i < _h.Length; i++)
        //{
        //    _h[i] = hostData[0];
        //}
        //hostData = _h;
        //Debug.LogError(hostData.Length);
        currentIndex = 0;
        maxIndex = hostData.Length;
        if (hostData.Length == 0)
        {
            messageLabel.text = "No Host Avalble";
        }
        else
        {
            messageLabel.text = "";
        }
        UpdatePage();

    }
    public void UpdatePage()
    {
        for (int i = 0; i < 3; i++)
        {
            if (serverItem[i] != null)
            {
                serverItem[i].Reset();
            }
        }
        for (int i = currentIndex; i < maxIndex&&i<currentIndex+3; i++)
        {
            int index = i;
            HostData _element=hostData[index];
            if (!(NatTester.filterNATHosts && _element.useNat))
            {
                string _serverName=_element.gameName;
                string _serverIP=_element.ip[0]+":"+_element.port;
                string _serverStatus=_element.connectedPlayers.ToString() + "/" + _element.playerLimit.ToString();
                bool _isFull=false;
                if(_element.connectedPlayers>=_element.playerLimit)
                {
                   _isFull=true;
                }

                serverItem[index-currentIndex].FillData(_serverName, _serverIP, _serverStatus,_element.passwordProtected,_isFull, false);
            }
        }
    }
    void OnClick()
    {
        Debug.Log(UICamera.selectedObject.name);
        switch (UICamera.selectedObject.name)
        {
            case "BtnQuickPlay":
                {
                    OnQuickPlay();
                }
                break;
            case "BtnCreateHost":
                {
                    OnCreateHost();
                }
                break;
            case "BtnRefresh":
                {
                    OnRefreshServerList();
                }
                break;
            case "BtnItem01":
                {
                    OnClickItem01();
                }
                break;
            case "BtnItem02":
                {
                    OnClickItem02();
                }
                break;
            case "BtnItem03":
                {
                    OnClickItem03();
                }
                break;
            case "BtnNext":
                {
                    OnClickNext();
                }
                break;
            case "BtnBack":
                {
                    OnClickBack();
                }
                break;
            case "BtnClose":
                {
                    OnClose();
                }
                break;
            case "BtnCancel":
                {
                    OnJoinCancel();
                }
                break;
            case "BtnJoinLogin":
                {
                    OnJoinLogin();
                }
                break;
        }
    }
    public void OnJoinLogin()
    {
        //todo
        Debug.LogError(loginServerPass.text);
        NetworkConnectionError error = FHLanNetwork.instance.ConnectWithPassword(currentHostChoose.ip, currentHostChoose.port, loginServerPass.text);
        if (error == NetworkConnectionError.NoError)
        {
            GuiManager.HidePanel(GuiManager.instance.guiLanMultiPlayer);
            GuiManager.ShowPanel(GuiManager.instance.guiJoinRoomHandler, new UIJoinRoom.JoinRoomData(UIJoinRoomState.QuestWaitStart, currentHostChoose));
        }
    }
    public void OnJoinCancel()
    {
        objectChooseRoom.SetActiveRecursively(true);
        objectLogin.SetActiveRecursively(false);
        UpdatePage();
    }
    public void OnQuickPlay()
    {
         
    }
    public void OnCreateHost()
    {
        GuiManager.HidePanel(GuiManager.instance.guiLanMultiPlayer);
        GuiManager.ShowPanel(GuiManager.instance.guiJoinRoomHandler,new UIJoinRoom.JoinRoomData(UIJoinRoomState.CreateRoom,null));
    }
    public void OnRefreshServerList()
    {
        Debug.LogError("OnRefreshServerList");
        RefreshHost();
    }
    public void OnClickItem01()
    {
        currentHostChoose = hostData[currentIndex + 0];
        AccessHost();
    }
    public void OnClickItem02()
    {
        currentHostChoose = hostData[currentIndex + 1];
        AccessHost();
    }
    public void OnClickItem03()
    {
        currentHostChoose = hostData[currentIndex + 2];
        AccessHost();
    }
    public void AccessHost()
    {
        if (!currentHostChoose.passwordProtected)
        {
            //todo
            NetworkConnectionError error = FHLanNetwork.instance.Connect(currentHostChoose.ip, currentHostChoose.port);
            if (error == NetworkConnectionError.NoError)
            {
                GuiManager.HidePanel(GuiManager.instance.guiLanMultiPlayer);
                GuiManager.ShowPanel(GuiManager.instance.guiJoinRoomHandler, new UIJoinRoom.JoinRoomData(UIJoinRoomState.QuestWaitStart, currentHostChoose));
            }
        }
        else
        {
            loginserverName.text = currentHostChoose.gameName;
            objectChooseRoom.SetActiveRecursively(false);
            objectLogin.SetActiveRecursively(true);
        }
    }
    public void OnClickNext()
    {
        if (currentIndex <maxIndex-3)
        {
            
            currentIndex++;
            UpdatePage();
        }
        
    }
    public void OnClickBack()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdatePage();
        }
    }
    public void  OnClose()
    {
        GuiManager.HidePanel(GuiManager.instance.guiLanMultiPlayer);
    }
}
