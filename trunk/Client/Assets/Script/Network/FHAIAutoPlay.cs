using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHAIAutoPlay : SingletonMono<FHAIAutoPlay>
{
    public FHPlayerOnline playerOnline;
    [HideInInspector]
    public bool isActiveAuto=false;

    private float lastTimeUpdate = 0;
    private float timeUpdateAI=0.35f;//0.7s

    private List<int> bigFishID = new List<int> { 11, 3};
    private List<int> medFishID = new List<int> { 15, 10, 9, 4, 13, 14 };//remain is low

    private List<int> bigBullet = new List<int> { 10, 8 };
    private List<int> medBullet = new List<int> { 5, 4 };
    private List<int> lowBullet = new List<int> { 3, 2, 1 };

    int oldFishID=0;
    int countShot = 0;
    int maxCountforFish = 6;
    bool first = false;
	void Start () {
        first = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!isActiveAuto)
        {
            return;
        }
        float now = Time.realtimeSinceStartup;
        if (now - lastTimeUpdate > timeUpdateAI)
        {
            
            if (now - lastTimeUpdate > 3)
            {
                if (!first)
                {
                    first = true;
                }
                else
                {
                    playerOnline.ProcessLanChangeGold(playerOnline.gold + 15 * (int)(now - lastTimeUpdate));
                }
            }
            lastTimeUpdate = now;
            AutoAI();
           
        }
	}

    public IEnumerator SetDelayActive(float _time)
    {
        yield return new WaitForSeconds(_time);
        isActiveAuto = true;
    }
    public void AutoAI()
    {
        HashSet<FHFish> activeFishes = FHFishManager.instance.GetActiveFishes();
        foreach(FHFish item in activeFishes)
        {
            if (item != null && item.state != FHFishState.Dead && item.fishIdentify == oldFishID)// ban lai con ca lan ban truoc
            {
                int canReshot = FHUtils.rand.Next(100);
                if (canReshot > 70)// lock
                    return;
                float rotate=playerOnline.GetAngleFromTarget(item.transform.position);
                float distance = playerOnline.DistanceFromTarget(item.transform.position);
                   
                float fixAngle = -8;
                if (distance > 10)
                {
                    fixAngle = -3;
                }

                if (item.transform.right.x < 0)
                {
                    rotate -= fixAngle;
                }
                else
                {
                    rotate += fixAngle;
                }
                if (playerOnline.gold > playerOnline.currentGun.id)
                {
                    playerOnline.ProcessLanShot(0,0,rotate);
                    playerOnline.ProcessLanChangeGold(playerOnline.gold - playerOnline.currentGun.id);
                }
                countShot++;
                if(countShot>=maxCountforFish)
                {
                    countShot=0;
                    oldFishID=-1;
                }
                return;
            }
        }
        // shot new fish
        List<FHFish> fishResult = new List<FHFish>();
        int rad = FHUtils.rand.Next(100);
        int bulletID = 1;
        if(rad<40)// shot big fish
        {
            foreach (FHFish item in activeFishes)
            {
                if (item != null && item.state != FHFishState.Dead && item.routeFactor > 0.4 && item.routeFactor<0.8)
                {
                    if (bigFishID.Contains(item.configFish.id))
                    {
                        fishResult.Add(item);
                        bulletID = bigBullet[FHUtils.rand.Next(bigBullet.Count)];
                    }
                }
            }
        }
        else if (rad < 75)// shot med fish
        {
            foreach (FHFish item in activeFishes)
            {
                if (item != null && item.state != FHFishState.Dead && item.routeFactor > 0.4 && item.routeFactor < 0.8)
                {
                    if (medFishID.Contains(item.configFish.id))
                    {
                        fishResult.Add(item);
                        bulletID = medBullet[FHUtils.rand.Next(medBullet.Count)];
                    }
                }
            }
        }
        else// shot small fish
        {
            foreach (FHFish item in activeFishes)
            {
                if (item != null && item.state != FHFishState.Dead && item.routeFactor > 0.4 && item.routeFactor < 0.8)
                {
                    if (medFishID.Contains(item.configFish.id))
                    {
                        fishResult.Add(item);
                        bulletID = lowBullet[FHUtils.rand.Next(lowBullet.Count)];
                    }
                }
            }
        }
        if(fishResult.Count<1)
        {
            foreach (FHFish item in activeFishes)
            {
                if (item != null && item.active == true)
                {
                    fishResult.Add(item);
                    bulletID = lowBullet[FHUtils.rand.Next(lowBullet.Count)];
                }
            }
        }
        int changeBullet = FHUtils.rand.Next(100);
        int changerate = 50;
        if (playerOnline.currentGun.id == 10)// sung 10
        {
            changerate = 80;
        }
        if (changeBullet < changerate)
        {
            playerOnline.ProcessLanCurrentGun(bulletID);
        }
        int canShot = FHUtils.rand.Next(100);

        if(canShot<70&&fishResult.Count>0)
        {
            for (int i = 0; i < 5; i++)
            {
                FHFish fish = fishResult[FHUtils.rand.Next(fishResult.Count)];
                if (playerOnline.gold > bulletID)
                {
                    float rotate = playerOnline.GetAngleFromTarget(fish.transform.position);
                    float distance = playerOnline.DistanceFromTarget(fish.transform.position);
                    float fixAngle = -8;
                    if (distance > 10)
                    {
                        fixAngle = -3;
                    }

                    if (fish.transform.right.x < 0)
                    {
                        rotate -= fixAngle;
                    }
                    else
                    {
                        rotate += fixAngle;
                    }
                    if (rotate > 180)
                    {
                        rotate = -360 + rotate;
                    }
                    if (rotate < -180)
                    {
                        rotate = 360 + rotate;
                    }

                    //Debug.LogError("AAAAAAAAAAAAAAAA:" + rotate+":"+distance);
                    if (rotate < -90 || rotate > 90)
                    {
                        continue;
                    }
                    playerOnline.ProcessLanShot(0, 0, rotate);
                    playerOnline.ProcessLanChangeGold(playerOnline.gold - bulletID );
                    countShot = 0;
                    oldFishID = fish.fishIdentify;
                    break;
                }
            }
        }
      
    }
}
