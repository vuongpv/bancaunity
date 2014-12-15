using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public delegate void FHPlayStorePaymentCallback(int code, ConfigGoldPackRecord goldpack);

public class FHPlayStorePayment : SingletonMono<FHPlayStorePayment>
{
    private enum TransactionState
    {
        None = 0,
        Request = 1,
        Close = 2,
    }

    private const float REQUEST_TIME_OUT = 60.0f;

    private TransactionState state;
    private string payID = "";
    private FHPlayStorePaymentCallback paymentCallback = null;
    private ConfigGoldPackRecord pack;
    
    private float requestStartTime;
    private float requestElapsedTime;

    void Reset()
    {
        state = TransactionState.None;
        payID = "";
        paymentCallback = null;
        pack = null;
    }

    void Update()
    {
        if (state == TransactionState.None)
            return;

        if (state == TransactionState.Request || state == TransactionState.Close)
        {
            if (Time.time - requestStartTime > REQUEST_TIME_OUT)
                CompleteTransaction(FHResultCode.TIME_OUT);

            return;
        }
    }

    public void RequestPayment(ConfigGoldPackRecord _pack, FHPlayStorePaymentCallback callback)
    {
        if (!FHHttpClient.isInternetReachable)
        {
            paymentCallback = callback;
            CompleteTransaction(FHResultCode.NOT_CONNECT);
            return;
        }

        // Reset transaction info
        Reset();

        state = TransactionState.Request;

        paymentCallback = callback;
        pack = _pack;

        // Make request params
		Dictionary<string, object> @params = new Dictionary<string, object>();
		@params["deviceID"] = SystemHelper.deviceUniqueID;
		@params["product"] = pack.id;
		@params["money"] = pack.cashValue;

        requestStartTime = Time.time;
        FHHttpClient.RequestTransaction(@params, "googleplay", (code, json) =>
        {
            OnReceivedPayID(code, json);

			switch ((int)pack.cashValue)
			{
				case 1: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "1.99"); break;
				case 2: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "2_99"); break;
				case 6: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "6_99"); break;
				case 12: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "12_99"); break;
				case 23: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "23_99"); break;
				case 39: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "39_99"); break;
			}
        });
    }

    void OnReceivedPayID(int code, JSONNode json)
    {
        if (code != FHResultCode.OK)
        {
            CompleteTransaction(code);
            return;
        }

        try
        {
            payID = (string)json["payID"];
        }
        catch (System.Exception ex)
        {
            CompleteTransaction(FHResultCode.FAILED);
            return;
        }

        Debug.LogError("[PlayStore] PayID = " + payID);

        state = TransactionState.None;
    }

    public void RequestClosePayment()
    {
        state = TransactionState.Close;
        requestStartTime = Time.time;

        Debug.LogError("[PlayStore] Request close PayID = " + payID);

        FHHttpClient.CloseTranstaction(payID, (code, json) =>
        {
            OnReceivedCloseTransaction(code, json);
        });
    }
    
    void OnReceivedCloseTransaction(int code, JSONNode json)
    {
        CompleteTransaction(code);
    }

    void CompleteTransaction(int code)
    {
        if (paymentCallback != null)
            paymentCallback(code, pack);

        Reset();
    }
}