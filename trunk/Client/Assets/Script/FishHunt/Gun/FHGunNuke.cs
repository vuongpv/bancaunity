using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

[SoundClip("fire")]
[SoundClip("hit")]
public class FHGunNuke : FHGun
{
    protected override void OnFireBullet()
    {
        if (controller.nuke <= 0)
            return;

        // Store current bet multiplier
        int betMultiplier = controller.GetCurrentBetMultiplier();

        // Spawn bullet
        FHBullet bullet = bulletPool.Spawn(bulletPrefab.transform).GetComponent<FHBullet>();
        bullet.Setup(this, configGun.bulletSpeed, configGun.bulletAoe, 1);

        // Play fire anim & sound
        FHAudioManager.instance.PlaySound(FHAudioManager.SOUND_FIRE_NUKE);
        barrelSprite.PlayAnim(0);

        // Update counter
        controller.DecreasePowerup(configGun.id);
    }

    public override FHFish GetBulletHitTarget(Vector3 bulletPosStart, Vector3 bulletPosEnd, out Vector3 hitPt)
    {
        HashSet<FHFish> activeFishes = FHFishManager.instance.GetActiveFishes();
        foreach (FHFish fish in activeFishes)
        {
            if (fish.state == FHFishState.Dead || fish.state == FHFishState.Dying)
                continue;

            if (fish.fastCollider.IsSphereOverlapped(bulletPosStart, 1.0f))
            {
                hitPt = bulletPosEnd;
                return fish;
            }
        }

        hitPt = Vector3.zero;
        return null;
    }

    protected override List<FHFish> CheckExplodeHitTargets(Vector3 impactPosition, FHFish impactTarget)
    {
        FHCameraEffect effect = Camera.main.gameObject.GetComponent<FHCameraEffect>();
        effect.Shake();

        return base.CheckExplodeHitTargets(impactPosition, impactTarget);
    }
}