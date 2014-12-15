using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Request_Rooms:BaseRequest
{
		public string UID;
		public Request_Rooms (string uid)
		{
				type = (int)RequestType.Rooms;
				UID = uid;
		}
}

public class Response_Rooms:BaseResponse
{
		public List<PublicRoomData> rooms;
		public string positions;
}

public class Request_JoinWaittingRoom:BaseRequest
{
		public string UID;
		public Request_JoinWaittingRoom (string uid)
		{
				type = (int)RequestType.JoinWaittingRoom;
				UID = uid;
		}
}

public class Response_JoinWaittingRoom:BaseResponse
{
		public bool result;
}