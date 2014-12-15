using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FHGamblingLogic : SingletonMono<FHGamblingLogic>
{
    // Constants
    const int MULTIPLIER_ROUND_NUMBER = 100000;

    System.Random randomGenerator = new System.Random((int)DateTime.Now.Ticks & 0x0000FFFF);

    // Methods
    public bool FishWillBeDie(FHGun gun, FHFish fish, ConfigLevelRecord levelRecord, out int powerupID)
    {
        powerupID = -1;

        if (levelRecord == null)
            return false;

        return FishWillBeDie(gun, fish, levelRecord.payoutRate, 1, out powerupID);
    }

    public bool FishWillBeDie(FHGun gun, FHFish fish, int payoutRate, float scalar, out int powerupID)
    {
        powerupID = -1;

        if (gun == null || gun.configGun == null)
            return false;

        if (fish == null || fish.configFish == null)
            return false;

        // Calculate hit rate, rate needs be normalized to float: 0.0f <= rate <= 1.0f
        float hitRate = fish.configFish.rate * ((float)payoutRate / 100.0f) * gun.configGun.hitRateMultiplier * scalar;

        if (hitRate < 0.0f)
            hitRate = 0.0f;

        if (hitRate > 1.0f)
            hitRate = 1.0f;

        // Randomize for ultimate result
        int rand = (randomGenerator.Next() % MULTIPLIER_ROUND_NUMBER) + 1;
        bool willBeDie = (rand <= (hitRate * MULTIPLIER_ROUND_NUMBER));

        if (willBeDie && gun.configGun.id < 100)
        {
            powerupID = CalculatePowerupsDrop(fish.configFish.id);
            //    Debug.LogError(fish.configFish.name + " rand =  " + rand + " after " + fish.hitTimes + " / " + (1.0f / hitRate) + " hits");
        }

        return willBeDie;
    }

    int CalculatePowerupsDrop(int fishID)
    {
        Dictionary<int, float> rates = ConfigManager.configFish.GetPowerupRates(fishID);
        if (rates == null)
            return -1;

        int rand = (randomGenerator.Next() % MULTIPLIER_ROUND_NUMBER) + 1;
        bool willDropPowerup = (rand <= (rates[FHGameConstant.POWERUP_GUN] * MULTIPLIER_ROUND_NUMBER));

        if (!willDropPowerup)
            return -1;

        rand = (randomGenerator.Next() % MULTIPLIER_ROUND_NUMBER) + 1;
        if (rand <= (rates[FHGameConstant.LIGHTNING_GUN] * MULTIPLIER_ROUND_NUMBER))
            return FHGameConstant.LIGHTNING_GUN;
        else
            return FHGameConstant.NUKE_GUN;
    }
}
