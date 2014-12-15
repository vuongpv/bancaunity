using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;

public delegate void FHCoinExchangeCallback(int code, ConfigGoldPackRecord goldpack);

public class FHCoinExchange : SingletonMono<FHCoinExchange>
{
    private enum TransactionState
    {
        None = 0,
        Request = 1,
        SendSMS = 2,
        Poll = 3,
        Close = 4,
    }

    private TransactionState state;
    private FHCoinExchangeCallback paymentCallback = null;
    private ConfigGoldPackRecord goldPack;

    void Reset()
    {
        state = TransactionState.None;
        paymentCallback = null;
        goldPack = null;
    }

    public void RequestPayment(ConfigGoldPackRecord pack, FHCoinExchangeCallback callback)
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
        goldPack = pack;

        CoinExchange();
    }

    void CoinExchange()
    {
        if (!UpdateGold())
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
        if (paymentCallback != null)
            paymentCallback(code, goldPack);

        Reset();
    }

    bool UpdateGold()
    {
        if (goldPack == null)
            return false;

        if (FHPlayerProfile.instance.diamond < goldPack.cashValue)
            return false;

        FHPlayerProfile.instance.diamond -= (int)goldPack.cashValue;

        int goldTotal = goldPack.goldValue + goldPack.goldBonus;

        if (FHSystem.instance.GetCurrentPlayerMode() == FHPlayerMode.Multi)
            FHMultiPlayerManager.instance.GetMainPlayer().AddCoin(goldTotal);
        else
            FHPlayerProfile.instance.gold += goldTotal;

        FHPlayerProfile.instance.ForceSave();

        return true;
    }
}