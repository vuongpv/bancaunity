using UnityEngine;
using System.Collections;

public class ServerInfo
{
		public int code = 0;
		public string ip = "";
		public string mess = "";
}

public enum ChatChannel
{
		System = 0,
		All = 1,
		Room = 2,
		Party = 3,
		Private = 4
}

public enum ResultCode
{
		OK          = 0,
		FAILED      = 1,
		NOT_FOUND   = 2,
		PENDING     = 3,
		CLOSED      = 4,
		TIME_OUT    = 5,
		EXPIRED     = 6,
}

public class OtherPlayerNetworkData
{
		public string nickname;
		public string clientid;
		public string uid;
		public int actpos;
		public bool isPlaying;
}

public enum Skin_Room
{
		AI=1,
		Player=2
}

public class PublicRoomData
{
		public int id;
		public string name;
		public int price;
		public int skintype;
		public int	roomType;
		public int maxUser;
		public bool status = true;
}

