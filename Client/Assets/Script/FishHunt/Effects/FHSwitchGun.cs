using UnityEngine;
using System.Collections;
using System;

public class FHSwitchGun : MonoBehaviour
{
    public float lifeTime = 0.25f;

    FHGunHudPanel manager;

    public void Setup(FHGunHudPanel _manager)
    {
        manager = _manager;

        Vector3 pos = manager.controller.gunAnchor.transform.position;
        pos.y = 0.1f;
        pos.z += 1.5f;
        transform.position = pos;

        SpriteBase sprite = gameObject.GetComponent<SpriteBase>();
        if (sprite != null)
            sprite.PlayAnim(0);

        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(lifeTime);

        manager.DespawnSwitchGunEffect(gameObject.transform);
    }
}
