using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;

public static class FHCardExchangeErrorCode
{
}

public delegate void FHCardExchangeCallback(int code, ConfigCardRecord card);

public class FHCardExchange : SingletonMono<FHCardExchange>
{
	private enum TransactionState
	{
		None = 0,
		Request = 1,
		SendInfo = 2,
		Poll = 3,
		Close = 4,
	}

	private TransactionState state;
    private FHCardExchangeCallback exchangeCallback = null;
    private ConfigCardRecord card;
    private string email, phone;

	private float requestStartTime;
	private float requestElapsedTime;

	void Reset()
	{
		state = TransactionState.None;
        exchangeCallback = null;
		card = null;
	}

	void Update()
	{
		if (state == TransactionState.None)
			return;

		if (state == TransactionState.Request || state == TransactionState.Close || state == TransactionState.SendInfo)
		{
			if (Time.time - requestStartTime > FHGameConstant.SHOP_REQUEST_TIMEOUT)
				CompleteTransaction(FHResultCode.TIME_OUT);

			return;
		}
	}

    public void RequestExchange(ConfigCardRecord _card, string _email, string _phone, FHCardExchangeCallback callback)
	{
		if (!FHHttpClient.isInternetReachable)
		{
            exchangeCallback = callback;
			CompleteTransaction(FHResultCode.NOT_CONNECT);
			return;
		}

		// Reset transaction info
		Reset();

		state = TransactionState.Request;

        exchangeCallback = callback;
        card = _card;
		email = _email;
		phone = _phone;

        requestStartTime = Time.time;

        SendInfo();
	}

    void SendInfo()
    {
        if (!UpdateDiamond())
        {
            CompleteTransaction(FHResultCode.FAILED);
            return;
        }

        state = TransactionState.Close;
        CloseTransaction();
    }

    void CloseTransaction()
    {
        CompleteTransaction(FHResultCode.OK);
    }

	void CompleteTransaction(int code)
	{
        if (exchangeCallback != null)
            exchangeCallback(code, card);

		Reset();
	}

	bool UpdateDiamond()
	{
		if (card == null)
			return false;

        FHHttpClient.ExchangeDiamond(card.diamondValue,(code, json) =>
        {
            if (code == FHResultCode.OK)
            {
                Debug.LogError(json);
                int diamond = int.Parse((string)json["diamond"]);
                FHPlayerProfile.instance.diamond = diamond;
                FHDiamondHudPanel.instance.UpdateDiamond();
            }
            else
            {
                Debug.LogError("Error ExchangeDiamond:"+json);
            }
        });

		return true;
	}
}