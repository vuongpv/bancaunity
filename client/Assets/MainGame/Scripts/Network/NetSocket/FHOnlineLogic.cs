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
		public string sid = "";
		public string userName = "player";
		public int location = 0;
		public FHUserOnlinePlay (string _name, string _sid, int _location)
		{
				userName = _name;
				sid = _sid;
				location = _location;
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
		public FHRoomOnlinePlay ()
		{

		}
		public bool Init (string _roomName, int _roomType, int _routeID, int _timePlay, float _timeSequenceUpdate, int kindPlay, string _playerNames, string _SIDs, string _locations, float _taxPercent)
		{
				isDiamondRoom = false;
				roomName = _roomName;
				roomType = _roomType;
				taxPercent = _taxPercent;
//				if (roomType >= (int)SocketJoinRoomType.roomTypeDiamondStart && roomType <= (int)SocketJoinRoomType.roomTypeDiamondEnd) {
//						isDiamondRoom = true;
//				}
				routeID = _routeID;
				timePlay = _timePlay;
				timeSequenceUpdate = _timeSequenceUpdate;
				Debug.LogError ("=============KindPlay: " + kindPlay);
				if (kindPlay == 1) {
						isAutoPlay = true;
				}
				Debug.LogError (timeSequenceUpdate);
				string[] subString = new string[] { "$$" };
				string[] subPlayers = _playerNames.Split (subString, StringSplitOptions.RemoveEmptyEntries);
				string[] subSIDs = _SIDs.Split (subString, StringSplitOptions.RemoveEmptyEntries);
				string[] subLocations = _locations.Split (subString, StringSplitOptions.RemoveEmptyEntries);
				if (subPlayers.Length == subSIDs.Length && subSIDs.Length == subLocations.Length) {
						try {
								for (int i = 0; i < subPlayers.Length; i++) {
										int local = int.Parse (subLocations [i].Trim ());
										FHUserOnlinePlay FHUserOnlinePlay = new FHUserOnlinePlay (subPlayers [i], subSIDs [i], local);
										listPlayer.Add (FHUserOnlinePlay);
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
}