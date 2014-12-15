using UnityEngine;
using System.Collections;

public class FHSimpleImpact : MonoBehaviour
{
    public float lifeTime = 0.25f;

    FHGun gun;

    public void Setup(FHGun _gun)
    {
        gun = _gun;
        
        SpriteBase sprite = gameObject.GetComponent<SpriteBase>();
        if (sprite != null)
            sprite.PlayAnim(0);

        StartCoroutine(Despawn());
    }
	
    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(lifeTime);

        gun.DespawnImpactEffect(gameObject.transform);
    }
}
