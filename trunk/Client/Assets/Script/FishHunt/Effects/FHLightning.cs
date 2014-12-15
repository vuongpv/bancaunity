using UnityEngine;
using System.Collections;

public class FHLightning : MonoBehaviour {
    float lifeTime = 0.5f;
    FHGun gun;

	public void Setup(FHGun _gun)
    {
        gun = _gun;
        StartCoroutine(AutoDestroy());
	}
	
    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(lifeTime);

        gun.DespawnImpactEffect(gameObject.transform);
    }
}
