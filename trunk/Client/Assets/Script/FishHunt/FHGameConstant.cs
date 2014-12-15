using UnityEngine;
using System.Collections;

public class FHGameConstant
{
    public static int[] BET_MULTIPLIERS = {1, 2, 3, 5, 10, 15, 20, 25, 30, 50};
    public const float BULLET_IMPACT_CHECKING_TIME_CYCLE = 0.0f;

    public const int ONLINE_PAYOUT_RATE = 150;
    public const int MULTI_PAYOUT_RATE = 80;

    public const int LIGHTNING_GUN = 101;
    public const int NUKE_GUN = 102;
    public const int POWERUP_GUN = 103;

    public static int[] CASH_VALUE = { 20000, 50000, 100000, 200000, 500000 };
    public static int[] GOLD_BONUS = { 15, 20, 30, 35, 40 };

    public static int[] GOLD_SHARE = { 100, 200, 500, 1000, 2000, 5000 };

    public const float SHOP_REQUEST_TIMEOUT = 60.0f;

    public const int RATING_GOLD_BONUS = 1000;
}