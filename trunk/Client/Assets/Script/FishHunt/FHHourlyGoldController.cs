using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;

public class FHHourlyGoldController : MonoBehaviour {

	// Result code
	private int code;

	// Remain countdown time
	private TimeSpan remainTime;

	// Sum the time to 1 second elapse
	private float sumTime;

	// Text label
	public UILabel label;
	public UISprite button;

	public FHPlayerController controller;

	private bool isConnectFailed;
	private float reconnectTimeRemain;
	
 	void Start()
	{
		button.gameObject.active = false;

		// Init the data
		FHHttpClient.PeakHourlyGold((code, json) => {
			Refresh (code, json);
		});
	}

	void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			// Init the data
			FHHttpClient.PeakHourlyGold((code, json) =>
			{
				Refresh(code, json);
			}); 
		}
	}

	void Update()
	{
		// Reconnect
		if (isConnectFailed && FHHttpClient.isInternetReachable)
		{
			reconnectTimeRemain -= Time.deltaTime;
			if (reconnectTimeRemain < 0)
			{
				isConnectFailed = false;
				// Init the data
				FHHttpClient.PeakHourlyGold((code, json) =>
				{
					Refresh(code, json);
				});
			}
		}

		if (remainTime == null || remainTime.TotalSeconds == 0)
			return;
		
		if (Time.deltaTime / Time.timeScale > 1f)
		{
			sumTime += Time.deltaTime;
		}
		else
		{
			sumTime += Time.deltaTime / Time.timeScale;
		}
		if (sumTime > 1)
		{
			remainTime = remainTime.Subtract(new TimeSpan(0, 0, 1));
			if (remainTime.TotalSeconds <= 0)
				code = FHResultCode.OK;
			
			UpdateText();
			
			sumTime = 0;
		}
	}

	public void Refresh(int code, JSONNode json)
	{
		int remainMillisec = 0;
		if( json != null )
			remainMillisec = json["remainMillisec"].AsInt;
		Refresh(code, remainMillisec);

	}
	
	public void Refresh(int code, long remainMillisec)
	{
		this.code = code;
		this.remainTime = new TimeSpan(remainMillisec * 10000L); // 10000 ticks in a milliseconds
		UpdateText();
	}

	void UpdateText()
	{
		switch(code)
		{
		case FHResultCode.NOT_CONNECT:
			label.color = Color.red;
			label.text = FHLocalization.instance.GetString(FHStringConst.LABEL_NETWORK_ERROR);
			button.gameObject.active = false;
			isConnectFailed = true;
			reconnectTimeRemain = 5;
			break;

		case FHResultCode.HTTP_ERROR:
			label.color = Color.red;
			label.text = FHLocalization.instance.GetString(FHStringConst.LABEL_NETWORK_ERROR);
            if (button != null)
            {
                button.gameObject.active = false;
            }
            isConnectFailed = true;
			reconnectTimeRemain = 5;
			break;

		case FHResultCode.OK:
		case FHResultCode.CANNOT_DO_ACTION:
			if (remainTime.TotalSeconds <= 0)
			{
				label.color = Color.green;
				label.text = "";
				button.gameObject.active = true;
			}
			else
			{
				label.color = Color.white;
				label.text = string.Format("{0:0}:{1:00}:{2:00}", remainTime.Hours, remainTime.Minutes, remainTime.Seconds);
				button.gameObject.active = false;
			}
			break;

		case FHResultCode.REACH_DAILY_LIMIT:
			label.color = Color.yellow;
			label.text = FHLocalization.instance.GetString(FHStringConst.LABEL_REACH_DAY_LIMIT);
			label.gameObject.active = false;
			button.gameObject.active = false;
			break;
		}

	}

	void CollectGold()
	{
		if (remainTime.TotalSeconds <= 0)
		{
			FHHttpClient.CollectHourlyGold((code, json) =>
			{
				if (code == FHResultCode.OK)
				{
					int gold = json["amount"].AsInt;
					FHGuiCollectibleManager.instance.SpawnUICoinText(transform.position + new Vector3(0, 0.01f, 0), gold);
					FHPlayerProfile.instance.gold += gold;
					FHPlayerProfile.instance.ForceSave();

					if (FHGoldHudPanel.instance != null)
						FHGoldHudPanel.instance.UpdateGold();
				}
				Refresh(code, json);
			});
		}
	}
}
