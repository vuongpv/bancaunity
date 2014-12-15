using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Request_Mission : BaseRequest
{
	public Request_Mission()
	{
		type = (int)RequestType.Mission;
	}
}
public class Response_Mission:BaseResponse
{
	public List<MissionModel> missionList;
}
public class MissionModel:MonoBehaviour
{
	public int id;
	public string name;
	public int gold;
	public int diamond;
}