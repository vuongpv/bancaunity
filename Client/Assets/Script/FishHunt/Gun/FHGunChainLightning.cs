using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

public class FHFishPoint : GFramework.Point
{
    public FHFish fish;

    public FHFishPoint(FHFish _fish, float _x, float _y)
    {
        fish = _fish;
        x = _x;
        y = _y;
    }
}

public class FHGunChainLightning : FHGun
{
    const float FISH_FLICK_TIME = 1.0f;
    const float LIGHTNING_LIFE_TIME = 0.8f;

    Transform gunLightningEffect = null;

    public override void Setup(FHPlayerController controller, ConfigGunRecord configGun)
    {
        base.Setup(controller, configGun);
    }

    protected override void OnFireBullet()
    {
        if (controller.lightning <= 0)
            return;

        // Store current bet multiplier
        int betMultiplier = controller.GetCurrentBetMultiplier();

        // Spawn bullet
        FHBullet bullet = bulletPool.Spawn(bulletPrefab.transform).GetComponent<FHBullet>();
        bullet.Setup(this, configGun.bulletSpeed, configGun.bulletAoe, 1);

        GameObject lightning = (GameObject)GameObject.Instantiate((GameObject)impactEffectPrefab);
        lightning.transform.parent = bullet.transform;
        lightning.transform.localPosition = Vector3.zero;

        FHLightningBolt bolt = lightning.GetComponent<FHLightningBolt>();
        bolt.target = barrelAnchor;
        bolt.lifeTime = 100.0f;
        bolt.Init();

        bullet.particle = lightning;

        // Play fire anim & sound
        FHAudioManager.instance.PlaySound(FHAudioManager.SOUND_FIRE_LASER);
        barrelSprite.PlayAnim(0);

        // Update counter
        controller.DecreasePowerup(configGun.id);
    }

    protected override void SpawnImpactEffects(Vector3 impactPosition)
    {
    }

    protected override List<FHFish> CheckExplodeHitTargets(Vector3 bulletPosition, FHFish impactTarget)
	{
        HashSet<FHFish> activeFishes = FHFishManager.instance.GetActiveFishes();

        // Find convex hull
        List<Point> fishPoints = new List<Point>();
        foreach (var fish in activeFishes)
        {
            if (fish.state == FHFishState.Dead || fish.state == FHFishState.Dying)
                continue;

            if (fish.IsVisible())
                fishPoints.Add(new FHFishPoint(fish, fish._transform.position.x, fish._transform.position.z));
        }

        List<Point> convexHullPoints = GFramework.ConvexHull.FindConvexPolygon(fishPoints);

        // Get hits
        List<FHFish> hits = new List<FHFish>();

        impactTarget.flickTime = FISH_FLICK_TIME;
        impactTarget.OnBulletHit();
        hits.Add(impactTarget);

        for (int i = 0; i < convexHullPoints.Count; i++)
        {
            FHFish fish = ((FHFishPoint)convexHullPoints[i]).fish;
            fish.flickTime = FISH_FLICK_TIME;
            fish.OnBulletHit();
            hits.Add(fish);
        }

        DrawChainLaser(hits);

        return hits;
    }

    void DrawChainLaser(List<FHFish> fishes)
    {
        for (int i = 1; i < fishes.Count; i++)
        {
            float distance = (fishes[i]._transform.position - fishes[0]._transform.position).magnitude;

            GameObject lightning = (GameObject)GameObject.Instantiate((GameObject)impactEffectPrefab);
            lightning.transform.parent = fishes[i].transform;
            lightning.transform.localPosition = Vector3.zero;

            FHLightningBolt bolt = lightning.GetComponent<FHLightningBolt>();
            bolt.zigs = (int)(distance * 50.0f / 5.0f);
            bolt.target = fishes[0]._transform;
            bolt.lifeTime = LIGHTNING_LIFE_TIME;
            bolt.Init();

            fishes[i].particle = lightning;
        }
    }
}
