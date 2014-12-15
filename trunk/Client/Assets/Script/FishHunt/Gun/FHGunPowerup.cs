using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

[SoundClip("fire")]
[SoundClip("hit")]
public class FHGunPowerup : FHGun
{
    protected override void OnFireBullet()
    {
        // Store current bet multiplier
        int betMultiplier = controller.GetCurrentBetMultiplier();

        // Spawn bullet
        FHBullet bullet = bulletPool.Spawn(bulletPrefab.transform).GetComponent<FHBullet>();
        bullet.Setup(this, configGun.bulletSpeed, configGun.bulletAoe, betMultiplier);

        // Play fire anim & sound
        FHAudioManager.instance.PlaySound(FHAudioManager.SOUND_FIRE_POWERUP);
        barrelSprite.PlayAnim(0);

        // Reset
        controller.ResetPowerup(betMultiplier);
    }
}
