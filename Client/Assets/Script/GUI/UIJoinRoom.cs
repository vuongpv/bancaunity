using UnityEngine;
using System;
using System.Collections;

public enum UIJoinRoomState
{
    CreateRoom=0,
    OwnerWaitStart=1,
    QuestWaitStart=2,
}
public class UIJoinRoom : UIBaseDialogHandler {

	// Use this for initialization
    public class JoinRoomData
    {
        public UIJoinRoomState uiJoinRoomState;
        public HostData hostData;
        public JoinRoomData(UIJoinRoomState _state,HostData _host)
        {
            hostData = _host;
            uiJoinRoomState = _state;
        }
    }
    private System.Random rand = new System.Random();
    public UILabel serverName;
    public UILabel serverPass;
    public UILabel titleName;
    public UILabel currentNumber;
    public UILabel passProtected;
    public UILabel chatInput;
    public UITextList chatContent;
    public GameObject objectCreate;
    public GameObject objectJoin;
    public GameObject btnAccessGame;
   
    private JoinRoomData joinRoomData=null;


    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
    public override void OnBeginShow(object parameter)
    {
        if (parameter != null)
        {
            joinRoomData = (JoinRoomData)parameter;
            if (joinRoomData == null)
            {
                GUIMessageDialog.Show(OnErrorHappen, " There are something wrong, please try again !", "Notion", FH.MessageBox.MessageBoxButtons.OK);
            }
            switch (joinRoomData.uiJoinRoomState)
            {
                case UIJoinRoomState.QuestWaitStart:
                    OnQuestJoinRoom(joinRoomData.hostData);
                    break;
                case UIJoinRoomState.OwnerWaitStart:
                    OnOwnerJoinRoom();
                    break;
                case UIJoinRoomState.CreateRoom:
                    OnCreateRoom();
                    break;
            }
        }
        else
        {
            GUIMessageDialog.Show(OnErrorHappen, " There are something wrong, please try again !", "Notion", FH.MessageBox.MessageBoxButtons.OK);
        }
        FHLanNetwork.instance.fhLobbyChat.chatEventAdded += OnChatAdd;
        FHLanNetwork.instance.fhLobbyGame.checkedUpdate += OnUpdateUIAdd;
        FHLanNetwork.instance.fhLobbyGame.checkedLoadGame += OnCheckedLoadGame;
        chatContent.textLabel.text = "";
        chatContent.Clear();
        
    }
    public override void OnBeginHide(object parameter)
    {
        FHLanNetwork.instance.fhLobbyChat.chatEventAdded -= OnChatAdd;
        FHLanNetwork.instance.fhLobbyGame.checkedUpdate -= OnUpdateUIAdd;
        FHLanNetwork.instance.fhLobbyGame.checkedLoadGame -= OnCheckedLoadGame;
    }
    void OnClick()
    {
        switch (UICamera.selectedObject.name)
        {
            case "BtnCreateHost":
                {
                    OnCreateHost();
                }
                break;
            case "BtnClose":
                {
                    OnClose();
                }
                break;
            case "BtnStartGame":
                {
                    OnStartGame();
                }
                break;
            case "BtnSend":
                {
                    OnSendChat();
                }
                break;
                

        }
    }
    public void OnQuestJoinRoom(HostData _hostData)
    {
        objectCreate.SetActiveRecursively(false);
        objectJoin.SetActiveRecursively(true);
        btnAccessGame.SetActiveRecursively(false);
        titleName.text = _hostData.gameName;

    }
    public void OnOwnerJoinRoom()
    {
        objectCreate.SetActiveRecursively(false);
        objectJoin.SetActiveRecursively(true);
        btnAccessGame.SetActiveRecursively(true);
    }
    public void OnCreateRoom()
    {
        objectCreate.SetActiveRecursively(true);
        objectJoin.SetActiveRecursively(false);
        if (serverName.text.Length < 1)
        {
            serverName.text = "Server " + (rand.Next(1000)).ToString();
        }

    }

    public void OnSendChat()
    {
        if (chatInput.text.Length > 0)
        {
            FHLanNetwork.instance.fhLobbyChat.AddChatMessage(chatInput.text);
            chatInput.text = "";
        }
    }
    public void OnStartGame()
    {
        Debug.Log("ONStart");
        if (FHLanNetwork.instance.fhLobbyGame.GetCurrentCountPlayer() >= FHLanNetwork.MINIMUM_PLAYER_TOPLAY)
        {
            FHLanNetwork.instance.fhLobbyGame.HostLaunchGame();
        }
        else
        {
            GUIMessageDialog.Show(OnInfo, "FISH HURT Multiplay just start when have at least 2 Fisher", "Information", FH.MessageBox.MessageBoxButtons.OK);
        }
    }
    public bool OnInfo(FH.MessageBox.DialogResult result)
    {
        if (result == FH.MessageBox.DialogResult.Ok)
        {

        }

		return true;
    }
    public void OnCheckedLoadGame(bool result)
    {
        Debug.Log("OnCheckedLoadGame");
        SceneManager.instance.LoadScene("FishHuntOnline");
        GuiManager.HidePanel(GuiManager.instance.guiJoinRoomHandler);
    }
    public void OnChatAdd(FHLobbyChat.FHLobbyChatEntry entry)
    {
        if (entry != null)
        {
            if(entry.name.Length>0)
                chatContent.Add("[" + entry.name + "]: " + entry.text);
            else
                chatContent.Add("[System]: " + entry.text);
        }
    }

    public void OnUpdateUIAdd(bool result)
    {
        currentNumber.text = FHLanNetwork.instance.fhLobbyGame.GetCurrentCountPlayer().ToString() + "/" + FHLanNetwork.instance.fhLobbyGame.serverMaxPlayers.ToString();
        if (FHLanNetwork.instance.fhLobbyGame.serverPasswordProtected)
        {
            passProtected.text = "YES";
        }
        else
        {
            passProtected.text = "NO";
        }
    }



    public void OnCreateHost()
    {
        Debug.LogWarning("OnCreateHost");
        FHLobbyGame lobbyGame = FHLanNetwork.instance.fhLobbyGame;
        FHLanNetwork.instance.Reset();
        lobbyGame.StartHost(serverPass.text,2, serverName.text);
        lobbyGame.EnableLobby();
        OnOwnerJoinRoom();
        titleName.text = serverName.text;

        
    }
    public bool OnErrorHappen(FH.MessageBox.DialogResult result)
    {
        if (result == FH.MessageBox.DialogResult.Ok)
        {
            OnClose();
        }
		return true;
    }
    public void OnClose()
    {
        FHLanNetwork.instance.fhLobbyGame.LeaveLobbyGame();
        GuiManager.ShowPanel(GuiManager.instance.guiLanMultiPlayer);
        GuiManager.HidePanel(GuiManager.instance.guiJoinRoomHandler);
    }
}
