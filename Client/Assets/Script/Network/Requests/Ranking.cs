using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Request_Ranking : BaseRequest
{
	public string SID;
	public Request_Ranking()
	{
		type = (int)RequestType.Ranking;
	}
}
public class Response_Ranking:BaseResponse
{
	public List<RankingModel> dailyRanking;
}
public class RankingModel:MonoBehaviour
{
	public string uid;
	public string name;
	public int gold;
	public int diamond;
	public int score;
}