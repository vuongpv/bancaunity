using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FHLobbyGame : MonoBehaviour
{
    public class PlayerInfo
    {
        public string username="";
        public NetworkPlayer player;
    }
    
    public bool lobbyGameActive;
    // event register for view class
    public event Action<bool> checkDisconnectHost = null;
    public event Action<bool> checkedUpdate = null;
    public event Action<bool> checkedLoadGame = null;

    public event Action<bool> checkedReadyIngame = null;
    public bool isHostActive = false;

    public float lastRegTime = -60;

    private bool launchingGame = false;

    public List<PlayerInfo> playerList = new List<PlayerInfo>();
    

    [HideInInspector]
    public string playerName = "";
    [HideInInspector]
    public int serverMaxPlayers = 2;
    [HideInInspector]
    public string serverName = "Server unname";
    [HideInInspector]
    public bool serverPasswordProtected = false;

    public int GetCurrentCountPlayer()
    {
        return playerList.Count;
    }


    public void EnableLobby()
    {
        playerName = FHUtils.GetPlayerName();
        lastRegTime = Time.time - 3600;

        launchingGame = false;

        FHLobbyChat chat = FHLanNetwork.instance.fhLobbyChat;
        chat.InitChat();
        isHostActive = false;
        lobbyGameActive = true;
        Debug.LogWarning("Init Lobby Game");
    }

    public void Update()
    {
        if (Network.isServer && lastRegTime < Time.time - 20)
        {
            lastRegTime = Time.time;
            MasterServer.RegisterHost(FHLanNetwork.GAME_NAME_INLAN, serverName, "No description");
        }

        //Debug.LogWarning(Network.isServer + "," + Network.isClient);

        if (isHostActive)
        {
            if (!Network.isServer && !Network.isClient)
            {
                isHostActive = false;
                if (lobbyGameActive)
                {
                    if (checkDisconnectHost != null)
                    {
                        checkDisconnectHost(true);
                    }
                }
                
            }
        }
        else
        {
            if (Network.isServer || Network.isClient)
            {
                isHostActive = true;
            }
           
            
        }
        string str = "";
        for (int i = 0; i < playerList.Count; i++)
        {
            str += playerList[i].username +","+ playerList[i].player.ipAddress;
            str += "\n";
        }
        //if (str.Length > 0)
         //   Debug.LogWarning(str);
       
    }

    public void LeaveLobbyGame()
    {
        lobbyGameActive = false;
        StartCoroutine(leaveLobby());
    }

    public IEnumerator leaveLobby()
    {
        Debug.LogWarning("Disconnect from host, or shutdown host");
        if (Network.isServer || Network.isClient)
        {
            if (Network.isServer)
            {
                MasterServer.UnregisterHost();
            }
            Network.Disconnect();
            yield return new WaitForSeconds(0.3f);
        }
        FHLobbyChat chat = FHLanNetwork.instance.fhLobbyChat;
        chat.EndChat();
        Debug.LogWarning("Disconnect");
    }

    public void StartHost(string _password, int _numberplayers, string _serverName)
    {
        Debug.LogWarning(_password + "," + _numberplayers + "," + _serverName);
        if (_numberplayers < 1)
        {
            _numberplayers = 1;
        }
        if (_numberplayers >= 32)
        {
            _numberplayers = 32;
        }
        if (_password != "")
        {
            serverPasswordProtected = true;
            Network.incomingPassword = _password;
        }
        else
        {
            serverPasswordProtected = false;
            Network.incomingPassword = "";
        }

        serverName = _serverName;
        EnableLobby();
        Network.InitializeSecurity();
        //bool nat = !Network.HavePublicAddress();
        Network.InitializeServer((_numberplayers - 1),FHLanNetwork.GAME_SERVER_PORT);
       
        checkedUpdate(true);
    }

    public void OnConnectedToServer()
    {
        //Called on client
        //Send everyone this clients data
        playerList = new List<PlayerInfo>();
        playerName = FHUtils.GetPlayerName();
        networkView.RPC("addPlayer", RPCMode.AllBuffered, Network.player, playerName);
    }

    public void OnServerInitialized()
    {
        //Called on host
        //Add hosts own data to the playerlist	
        Debug.LogWarning("OnServerInitialized:" + playerName + "," + serverName);
        playerList = new List<PlayerInfo>();
        networkView.RPC("addPlayer", RPCMode.AllBuffered, Network.player, playerName);

        bool pProtected = false;
        if (Network.incomingPassword!=null && Network.incomingPassword != "")
        {
            pProtected = true;
        }
        int maxPlayers = Network.maxConnections + 1;

        networkView.RPC("setServerSettings", RPCMode.AllBuffered, pProtected, maxPlayers, serverName);

    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        //Called on host
        //Remove player information from playerlist
        networkView.RPC("playerLeft", RPCMode.All, player);

        FHLobbyChat chat = GetComponent<FHLobbyChat>();
        chat.AddChatMessage("A player left the lobby");
    }

    // Start Game
    public void HostLaunchGame()
    {
        if (!Network.isServer)
        {
            return;
        }

        // Don't allow any more players
        Network.maxConnections = -1;
        MasterServer.UnregisterHost();

        networkView.RPC("launchGame", RPCMode.All);
        if (checkedUpdate!=null)
        {
            checkedUpdate(true);
        }
    }

    #region Network view
    [RPC]
    void setServerSettings(bool password, int maxPlayers, string newSrverTitle)
    {
        serverMaxPlayers = maxPlayers;
        serverName = newSrverTitle;
        serverPasswordProtected = password;
    }

    [RPC]
    void addPlayer(NetworkPlayer player, string username)
    {
        Debug.Log("got addplayer:" + username);

        PlayerInfo playerInstance = new PlayerInfo();
        playerInstance.player = player;
        playerInstance.username = username;
        playerList.Add(playerInstance);
        if (checkedUpdate != null)
        {
            checkedUpdate(true);
        }
    }


    [RPC]
    void playerLeft(NetworkPlayer player)
    {

        PlayerInfo deletePlayer=null;

        foreach (PlayerInfo playerInstance in playerList)
        {
            if (player == playerInstance.player)
            {
                deletePlayer = playerInstance;
            }
        }
        if (deletePlayer != null)
        {
            playerList.Remove(deletePlayer);
        }
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
        if (checkedUpdate != null)
        {
            checkedUpdate(true);
        }
    }


    [RPC]
    void launchGame()
    {
        //Network.isMessageQueueRunning = false;
        launchingGame = true;
        if (checkedLoadGame != null)
        {
            checkedLoadGame(true);
        }
        if (checkedUpdate != null)
        {
            checkedUpdate(true);
        }
    }
    [RPC]
    void playerReadyToPlay()
    {
        if (checkedReadyIngame != null)
        {
            checkedReadyIngame(true);
        }
    }
    public void ReadyToPlay()
    {
        Debug.LogWarning(Network.connections.Length);
        networkView.RPC("playerReadyToPlay", RPCMode.All);
    }
    #endregion


}