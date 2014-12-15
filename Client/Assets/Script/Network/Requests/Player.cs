using UnityEngine;
using System.Collections;

public class Request_MyInformation : BaseRequest
{
		public string SID;
		public Request_MyInformation (string clientID)
		{
				type = (int)RequestType.MyInformation;
				SID = clientID;
		}
}
public class Response_MyInformation:BaseResponse
{
		public string value;
}

public class Request_Properties : BaseRequest
{
		public string uid;
		public Request_Properties (string _uid)
		{
				type = (int)RequestType.MyInformation;
				uid = _uid;
		}
}
public class Response_Properties:BaseResponse
{
		public string properties;
}