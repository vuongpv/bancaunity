using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RawMessageOnlinePlay
{
		public string msgName;
		public string jsonData;
		public RawMessageOnlinePlay (string _msgName, string _jsonData)
		{
				msgName = _msgName;
				jsonData = _jsonData;
		}
}
public class FHUserOnlinePlay
{
		public string uid = "";
		public string userName = "player";
		public int location = 0;
		public float gold = 0;
		public float diamond = 0;
		public FHUserOnlinePlay (string _name, string _sid, int _location)
		{
				userName = _name;
				uid = _sid;
				location = _location;
		}

		public FHUserOnlinePlay (string _name, string _sid, float _gold, float _diamond)
		{
				userName = _name;
				uid = _sid;
				gold = _gold;
				diamond = _diamond;
		}
}
public class FHRoomOnlinePlay
{
		public string roomName;
		public int roomType;
		public int routeID;
		public int timePlay = 100;
		public float timeSequenceUpdate = 0.1f;
		public List<FHUserOnlinePlay> listPlayer = new List<FHUserOnlinePlay> ();
		public bool isroomReady;
		public bool isAutoPlay;
		public bool isDiamondRoom = false;
		public float taxPercent = 1;
		public float price;
		public FHRoomOnlinePlay ()
		{

		}
		public bool Init (string _roomName, int _roomType, int _routeID, int _timePlay, float _timeSequenceUpdate, int kindPlay, string _playerNames, string _UIDs, string _locations, float _taxPercent, float _price)
		{
				isDiamondRoom = false;
				roomName = _roomName;
				roomType = _roomType;
				taxPercent = _taxPercent;
				price = _price;
				if (roomType >= (int)SocketJoinRoomType.roomTypeDiamond && roomType <= (int)SocketJoinRoomType.roomTypeDiamond) {
						isDiamondRoom = true;
				}
				routeID = _routeID;
				timePlay = _timePlay;
				timeSequenceUpdate = _timeSequenceUpdate;
				
				if (kindPlay == 1) {
						isAutoPlay = true;
				}
				Debug.Log ("Time sequence update: " + timeSequenceUpdate);
				string[] subString = new string[] { "$$" };
				string[] subPlayers = _playerNames.Split (subString, StringSplitOptions.RemoveEmptyEntries);
				string[] subSIDs = _UIDs.Split (subString, StringSplitOptions.RemoveEmptyEntries);
				string[] subLocations = _locations.Split (subString, StringSplitOptions.RemoveEmptyEntries);
				listPlayer.Clear ();
				if (subPlayers.Length == subSIDs.Length && subSIDs.Length == subLocations.Length) {
						try {
								for (int i = 0; i < subPlayers.Length; i++) {
										int local = int.Parse (subLocations [i].Trim ());
										FHUserOnlinePlay FHUserOnlinePlay = new FHUserOnlinePlay (subPlayers [i], subSIDs [i], local);
										listPlayer.Add (FHUserOnlinePlay);
										Debug.LogWarning ("**********************uid [" + i + "]=  " + FHUserOnlinePlay.uid);
								}
								return true;
						} catch (System.Exception ex) {
								Debug.LogError ("Parse Room Info Error:" + ex.Message);
								return false;
						}
				}
				Debug.LogError ("Parse Room Info Error");
				return false;

		}

		public void SetInforToPlayer (string _playerName, string _SID, float _gold, float _diamond)
		{
				FHUserOnlinePlay FHUserOnlinePlay = new FHUserOnlinePlay (_playerName, _SID, _gold, _diamond);
		}
}