using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FHQuestType : int
{
    HuntFish = 0,
    UseGunCollectCoin = 1,
    CollectCoinWithBet = 2,
}

public enum FHQuestState : int
{
    InProcess = 0,
    Finish = 1,
    Expire = 2,
}

public enum FHQuestProperty : int
{
    Fish = 0,
    Bullet = 1,
    Coin = 2,
}

public enum FHQuestParam
{
    GunID = 0,
    NumberCoins = 1,
    BetMultiplier = 2,
}

public class FHQuest
{
    // Serialize members
    protected const string TYPE = "type";
    protected const string STATE = "state";
    protected const string EXPIRE_TIME = "expireTime";
    protected const string ELAPSED_TIME = "elapsedTime";
    protected const string AWARD = "award";
    protected const string CONFIG_ID = "configID";

    // Properties
    public FHQuestType type;
    public FHQuestState state;
    public float expireTime;
    public float elapsedTime;
    public int award;
    public int configID;

    protected Dictionary<string, object> jsonDic;

    public FHQuest()
    {
        elapsedTime = 0.0f;

        jsonDic = new Dictionary<string, object>();
    }

    public FHQuest(Dictionary<string, object> _jsonDic)
    {
        jsonDic = _jsonDic;

        type = (FHQuestType)((long)jsonDic[TYPE]);
        state = (FHQuestState)((long)jsonDic[STATE]);
        expireTime = (float)((double)jsonDic[EXPIRE_TIME]);
        elapsedTime = (float)((double)jsonDic[ELAPSED_TIME]);
        award = (int)((long)jsonDic[AWARD]);
        configID = (int)((long)jsonDic[CONFIG_ID]);
    }

    public virtual void Serialize()
    {
        jsonDic.Clear();

        jsonDic[TYPE] = (int)type;
        jsonDic[STATE] = (int)state;
        jsonDic[EXPIRE_TIME] = (float)expireTime;
        jsonDic[ELAPSED_TIME] = (float)elapsedTime;
        jsonDic[AWARD] = (int)award;
        jsonDic[CONFIG_ID] = (int)configID;
    }

    public string GetJSON()
    {
        Serialize();
        return MiniJSON.Json.Serialize(jsonDic);
    }

    public void UpdateProperties(List<KeyValuePair<FHQuestProperty, object>> props)
    {
        if (state != FHQuestState.InProcess)
            return;

        if (props != null)
        {
            foreach (KeyValuePair<FHQuestProperty, object> prop in props)
                UpdateProperty(prop);
        }
    }

    public void IntervalUpdate(float deltaTime)
    {
        if (state != FHQuestState.InProcess)
            return;

        elapsedTime += deltaTime;

        UpdateState();
    }

    public virtual void UpdateState()
    {
    }

    public virtual void UpdateProperty(KeyValuePair<FHQuestProperty, object> prop)
    {
    }

    public virtual string GetStatus()
    {
        return "";
    }

    public float GetRemainTime()
    {
        return (expireTime - elapsedTime);
    }
}

public class FHQuest_HuntFish: FHQuest
{
    // Serialize members
    protected const string FISH_ID = "fishID";
    protected const string NUMBER_FISHES = "numberFishes";
    protected const string FISH_COUNTER = "fishCounter";

    // Properties
    public int fishID;
    public int numberFishes;
    public int fishCounter;

    public FHQuest_HuntFish(ConfigQuestRecord config)
        : base()
    {
        type = FHQuestType.HuntFish;
        expireTime = config.time;
        numberFishes = config.param1;
        fishID = config.param2;
        award = config.award;
        configID = config.id;

        fishCounter = 0;
    }

    public FHQuest_HuntFish(Dictionary<string, object> _jsonDic)
        : base(_jsonDic)
    {
        fishID = (int)((long)jsonDic[FISH_ID]);
        numberFishes = (int)((long)jsonDic[NUMBER_FISHES]);
        fishCounter = (int)((long)jsonDic[FISH_COUNTER]);
    }

    public override void Serialize()
    {
        base.Serialize();

        jsonDic[FISH_ID] = fishID;
        jsonDic[NUMBER_FISHES] = numberFishes;
        jsonDic[FISH_COUNTER] = fishCounter;
    }

    public override void UpdateProperty(KeyValuePair<FHQuestProperty, object> prop)
    {
        switch (prop.Key)
        {
            case FHQuestProperty.Fish:
                int _fishID = (int)prop.Value;
                if (_fishID == fishID)
                    fishCounter++;
                break;
        }
    }

    public override void UpdateState()
    {
        if (elapsedTime <= expireTime && fishCounter >= numberFishes)
            state = FHQuestState.Finish;
        else
        if (elapsedTime > expireTime)
            state = FHQuestState.Expire;
    }

    public override string GetStatus()
    {
        return (fishCounter + "/" + numberFishes);
    }
}

public class FHQuest_UseGunCollectCoin : FHQuest
{
    // Serialize members
    protected const string GUN_ID = "gunID";
    protected const string NUMBER_COINS = "numberCoins";
    protected const string COIN_COUNTER = "coinCounter";

    // Properties
    public int gunID;
    public int numberCoins;
    public int coinCounter;

    public FHQuest_UseGunCollectCoin(ConfigQuestRecord config)
        : base()
    {
        type = FHQuestType.UseGunCollectCoin;
        expireTime = config.time;
        numberCoins = config.param1;
        gunID = config.param2;
        award = config.award;
        configID = config.id;

        coinCounter = 0;
    }

    public FHQuest_UseGunCollectCoin(Dictionary<string, object> _jsonDic)
        : base(_jsonDic)
    {
        gunID = (int)((long)jsonDic[GUN_ID]);
        numberCoins = (int)((long)jsonDic[NUMBER_COINS]);
        coinCounter = (int)((long)jsonDic[COIN_COUNTER]);
    }

    public override void Serialize()
    {
        base.Serialize();

        jsonDic[GUN_ID] = gunID;
        jsonDic[NUMBER_COINS] = numberCoins;
        jsonDic[COIN_COUNTER] = coinCounter;
    }

    public override void UpdateProperty(KeyValuePair<FHQuestProperty, object> prop)
    {
        switch (prop.Key)
        {
            case FHQuestProperty.Coin:
                Dictionary<FHQuestParam, object> @params = (Dictionary<FHQuestParam, object>)prop.Value;
                if ((int)@params[FHQuestParam.GunID] == gunID)
                    coinCounter += (int)@params[FHQuestParam.NumberCoins];
                break;
        }
    }

    public override void UpdateState()
    {
        if (elapsedTime <= expireTime && coinCounter >= numberCoins)
            state = FHQuestState.Finish;
        else
        if (elapsedTime > expireTime)
            state = FHQuestState.Expire;
    }

    public override string GetStatus()
    {
        return (coinCounter + "/" + numberCoins);
    }
}

public class FHQuest_CollectCoinWithBet : FHQuest
{
    // Serialize members
    protected const string NUMBER_COINS = "numberCoins";
    protected const string BET_MULTIPLIER = "betMultiplier";
    protected const string COIN_COUNTER = "coinCounter";

    // Properties
    public int numberCoins;
    public int betMultiplier;
    public int coinCounter;

    public FHQuest_CollectCoinWithBet(ConfigQuestRecord config)
        : base()
    {
        type = FHQuestType.CollectCoinWithBet;
        expireTime = config.time;
        numberCoins = config.param1;
        betMultiplier = config.param2;
        award = config.award;
        configID = config.id;

        coinCounter = 0;
    }

    public FHQuest_CollectCoinWithBet(Dictionary<string, object> _jsonDic)
        : base(_jsonDic)
    {
        numberCoins = (int)((long)jsonDic[NUMBER_COINS]);
        betMultiplier = (int)((long)jsonDic[BET_MULTIPLIER]);
        coinCounter = (int)((long)jsonDic[COIN_COUNTER]);
    }

    public override void Serialize()
    {
        base.Serialize();

        jsonDic[NUMBER_COINS] = numberCoins;
        jsonDic[BET_MULTIPLIER] = betMultiplier;
        jsonDic[COIN_COUNTER] = coinCounter;
    }

    public override void UpdateProperty(KeyValuePair<FHQuestProperty, object> prop)
    {
        switch (prop.Key)
        {
            case FHQuestProperty.Coin:
                Dictionary<FHQuestParam, object> @params = (Dictionary<FHQuestParam, object>)prop.Value;
                if ((int)@params[FHQuestParam.BetMultiplier] == betMultiplier)
                    coinCounter += (int)@params[FHQuestParam.NumberCoins];
                break;
        }
    }

    public override void UpdateState()
    {
        if (elapsedTime <= expireTime && coinCounter >= numberCoins)
            state = FHQuestState.Finish;
        else
        if (elapsedTime > expireTime)
            state = FHQuestState.Expire;
    }
    
    public override string GetStatus()
    {
        return (coinCounter + "/" + numberCoins);
    }
}