using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;

[Serializable]
public class ServerHost
{
	public string host;
	public string port;
}

public class FHHttpClient : MonoBehaviour {

	public static FHHttpClient instance;

	public ServerHost productionServer;
	public ServerHost developmentServer;

	private static String uniqueDeviceID;

	public static bool isInternetReachable
	{
		get {
			return Application.internetReachability != NetworkReachability.NotReachable;
		}
	}

	void Awake()
	{
		if( instance == null )
			instance = this;

		uniqueDeviceID = SystemHelper.deviceUniqueID;
		Debug.Log("Unique Device ID: " + uniqueDeviceID);
	}

	void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	string GetRequestAPI(string api)
	{
#if UNITY_EDITOR
		return "http://" + developmentServer.host + ":" + developmentServer.port + "/" + api;
#else
		return "http://" + productionServer.host + ":" + productionServer.port + "/" + api;
#endif
    }

    #region GET request


    void Request(string api, Action<int, JSONNode> response)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			if (response != null)
				response(FHResultCode.NOT_CONNECT, null);
			return;
		}
		StartCoroutine(_Request(api, response));
	}

    IEnumerator _Request(string api, Action<int, JSONNode> response)
	{
		WWW www = new WWW(GetRequestAPI(api));

		while (!www.isDone && www.error == null)
		{
			yield return null;
		}

		if (www.error != null)
		{
            Debug.Log("Request Error. Url: " + www.url+","+ api + ", Error: " + www.error);
			if (response != null)
				response(FHResultCode.HTTP_ERROR, null);
			yield break;
		}

		Debug.Log("Request response. Url: " + www.url+", "+ api + ", Response: " + www.text);
		if (response != null)
		{
			JSONNode json = JSON.Parse(www.text);
			response(json["code"].AsInt, json);
		}

		www.Dispose();
    }
    #endregion

    #region POST request
    void RequestPOST(string api, Dictionary<string, object> parameters, Action<int, JSONNode> response)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (response != null)
                response(FHResultCode.NOT_CONNECT, null);
            return;
        }

        StartCoroutine(_RequestPOST(api, parameters, response));
    }

    IEnumerator _RequestPOST(string api, Dictionary<string, object> parameters, Action<int, JSONNode> response)
    {
        WWWForm form = new WWWForm();

        foreach (KeyValuePair<string, object> pair in parameters)
            form.AddField(pair.Key, pair.Value.ToString());

        WWW www = new WWW(GetRequestAPI(api), form);

		while (!www.isDone && www.error == null)
		{
			yield return null;
		}

        if (www.error != null)
        {
			Debug.LogError("Request Error. Url: " + api + ", Error: " + www.error);

			if (response != null)
				response(FHResultCode.HTTP_ERROR, null);
            yield break;
        }

		Debug.Log("Request response. Url: " + api + ", Response: " + www.text);

        if (response != null)
        {
            JSONNode json = JSON.Parse(www.text);
            response(json["code"].AsInt, json);
        }

        www.Dispose();
    }

    void RequestDataPOST(string api, Hashtable header, string body, Action<int, JSONNode> response)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (response != null)
                response(FHResultCode.NOT_CONNECT, null);
            return;
        }

        StartCoroutine(_RequestDataPOST(api, header, body, response));
    }

    IEnumerator _RequestDataPOST(string api, Hashtable header, string body, Action<int, JSONNode> response)
    {
        UTF8Encoding encoding = new System.Text.UTF8Encoding();

        WWW www = new WWW(GetRequestAPI(api), encoding.GetBytes(body), header);

		while (!www.isDone && www.error == null)
		{
			yield return null;
		}

        if (www.error != null)
        {
            if (response != null)
                response(FHResultCode.HTTP_ERROR, null);
            yield break;
        }

        Debug.LogError(www.text);

        if (response != null)
        {
            JSONNode json = JSON.Parse(www.text);
            response(FHResultCode.OK, json);
        }

        www.Dispose();
    }
    #endregion

    #region [ FishHunt API - Hourly gold, daily gift]

    // Get hourly gold
	public static void PeakHourlyGold(Action<int, JSONNode> response)
	{
		if (instance == null) return;
		instance.Request("naprofile/" + uniqueDeviceID + "/hourlygold/peak", response);
	}

	// Collect hourly gold
	public static void CollectHourlyGold(Action<int, JSONNode> response)
	{
		if (instance == null) return;
		instance.Request("naprofile/" + uniqueDeviceID + "/hourlygold/do", response);
	}

	// Get daily gift
	public static void PeakDailyGift(Action<int, JSONNode> response)
	{
		if( instance == null ) return;
		instance.Request("naprofile/" + uniqueDeviceID + "/dailygift/peak", response);
	}

	public static void CollectDailyGift(Action<int, JSONNode> response)
	{
		if (instance == null) return;
		instance.Request("naprofile/" + uniqueDeviceID + "/dailygift/do", response);
	}

    // Get current time server
    public static void GetCurrentTimeServer(Action<int, JSONNode> response)
    {
        if (instance == null) return;
        instance.Request("naprofile/" + uniqueDeviceID + "/getServerTime", response);
    }
    public static void GetCurrentDiamond(Action<int, JSONNode> response)
    {
        if (instance == null) return;
        instance.Request("naprofile/" + uniqueDeviceID + "/getCurrentDiamond", response);
        Debug.LogError(uniqueDeviceID);
    }

    public static void ExchangeDiamond(int _value,Action<int, JSONNode> response)
    {
        if (instance == null) return;
        instance.Request("naprofile/" + uniqueDeviceID + "/exchangeDiamond/"+_value, response);
    }

    public static void CheatDiamond(int _value, Action<int, JSONNode> response)
    {
        if (instance == null) return;
        instance.Request("naprofile/" + uniqueDeviceID + "/cheatDiamond/" + _value, response);
    }

    public static void StoreProfile(int gold, int level, Action<int, JSONNode> response)
    {
        if (instance == null) return;
        instance.Request("naprofile/" + uniqueDeviceID + "/storeProfile/"+ gold + "/" + level, response);
    }

    public static void CheckProfile(Action<int, JSONNode> response)
    {
        if (instance == null) return;
        instance.Request("naprofile/" + uniqueDeviceID + "/checkProfile", response);
    }

    public static void RestoreProfile(Action<int, JSONNode> response)
    {
        if (instance == null) return;
        instance.Request("naprofile/" + uniqueDeviceID + "/restoreProfile", response);
    }
	#endregion

    #region [ FishHunt API - Shop]

	// Request pay ID
	public static void IsPaymentEnable(Action<bool> callback)
	{
		if (instance == null) return;
		instance.Request("so6payment/isenable", (result, json) =>
		{
			if (callback != null)
				callback(result == FHResultCode.OK);
		});
	}

    // Request pay ID
    public static void RequestTransaction(Dictionary<string, object> @params, string type, Action<int, JSONNode> response)
    {
        if (instance == null) return;
		instance.RequestPOST("so6payment/" + type + "/request", @params, response);
    }

    // Poll transaction
    public static void PollTranstaction(string payID, Action<int, JSONNode> response)
    {
        if (instance == null) return;
        instance.Request("so6payment/" + payID + "/poll", response);
    }

    // Close transaction
    public static void CloseTranstaction(string payID, Action<int, JSONNode> response)
    {
        if (instance == null) return;
        instance.Request("so6payment/" + payID + "/close", response);
    }

    // Fake result
    public static void FakeResult(Hashtable header, string body, Action<int, JSONNode> response)
    {
        if (instance == null) return;
        instance.RequestDataPOST("so6payment/sms/result", header, body, response);
    }

	// Close transaction
	public static void GetPaidTranstactions( Action<int, JSONNode> response)
	{
		if (instance == null) return;
		instance.Request("so6payment/paid/" + uniqueDeviceID, response);
	}

    #endregion

	#region [ System ]

	public static void GetClientUpdate(string appId, Action<int, JSONNode> response)
	{
		if (instance == null) return;
		instance.Request("client/checkupdate?appid=" + appId, response);
	}

	public static void GetShopConfig(string appId, Action<int, JSONNode> response)
	{
		if (instance == null) return;
		instance.Request("client/getshopconfig?appid=" + appId, response);
	}
	#endregion

	#region [ Action Codes ]

	public static void DoActionCode(string code, Action<int, JSONNode> response)
	{
		if (instance == null) return;
		instance.Request("actioncodes/" + code + "/" + uniqueDeviceID, response);
        
	}

	#endregion
}