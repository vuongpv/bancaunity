using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class FHLobbyChat : MonoBehaviour
{
    #region Desclare struct
		public class FHLobbyChatEntry
		{
				public string name = "";
				public string text = "";
		}
		public class FHLobbyPlayerNode
		{
				public string playerName;
				public NetworkPlayer networkPlayer;
		}
    #endregion

		//using for event UI class
		public event Action<FHLobbyChatEntry> chatEventAdded=null;

		private string playerName;
		//Server-only player list
		private List<FHLobbyPlayerNode> playerList = new List<FHLobbyPlayerNode> ();
		private List<FHLobbyChatEntry> chatEntries = new List<FHLobbyChatEntry> ();
    
    #region Client function
		void OnConnectedToServer ()
		{
				InitChat ();
				networkView.RPC ("TellServerOurName", RPCMode.Server, playerName);
		}
    #endregion

    #region Server function

		void OnServerInitialized ()
		{
				InitChat ();
				FHLobbyPlayerNode newEntry = new FHLobbyPlayerNode ();
				newEntry.playerName = playerName;
				newEntry.networkPlayer = Network.player;
				playerList.Add (newEntry);
				AddInfoMessage (playerName + " joined the chat");
		}

		void OnPlayerDisconnected (NetworkPlayer player)
		{
				AddInfoMessage ("Player disconnected from: " + player.ipAddress + ":" + player.port);
				//Remove player from the server list
				foreach (FHLobbyPlayerNode entry in playerList) {
						if (entry.networkPlayer == player) {
								playerList.Remove (entry);
								break;
						}
				}
		}

		void OnDisconnectedFromServer ()
		{
				EndChat ();
		}

		void OnPlayerConnected (NetworkPlayer player)
		{
				AddInfoMessage ("Player connected from: " + player.ipAddress + ":" + player.port);
		}
    #endregion

    #region Network function
		[RPC]
		void TellServerOurName (string name, NetworkMessageInfo info) //Sent by newly connected clients, receieved by server
		{
				FHLobbyPlayerNode newEntry = new FHLobbyPlayerNode ();
				newEntry.playerName = name;
				newEntry.networkPlayer = info.sender;
				playerList.Add (newEntry);

				AddInfoMessage (name + " joined the chat");
		}
		[RPC]
		void ApplyGlobalChatText (string name, string msg)
		{
				FHLobbyChatEntry entry = new FHLobbyChatEntry ();
				entry.name = name;
				entry.text = msg;
				if (chatEventAdded != null) {
						chatEventAdded (entry);
				}
				chatEntries.Add (entry);
				//Remove old entries
				if (chatEntries.Count > 6) {
						chatEntries.RemoveAt (0);
				}
		}
		// Add Chat Message
		public void AddInfoMessage (string str)
		{
				ApplyGlobalChatText ("", str);
				if (Network.connections.Length > 0) {
						networkView.RPC ("ApplyGlobalChatText", RPCMode.Others, "", str);
				}
		}
		public void AddChatMessage (string str)
		{
				ApplyGlobalChatText (playerName, str);
				if (Network.connections.Length > 0) {
						networkView.RPC ("ApplyGlobalChatText", RPCMode.Others, playerName, str);
				}
		}
    #endregion

    #region function util
		public void EndChat ()
		{
				chatEntries = new List<FHLobbyChatEntry> ();
		}

		public void InitChat ()
		{
//        playerName = FHUtils.GetPlayerName();
				chatEntries = new List<FHLobbyChatEntry> ();
		}
    #endregion
		public void Start ()
		{

		}
}