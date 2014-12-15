using UnityEngine;
using System.Collections;

public class FHAOE : MonoBehaviour
{
    float lifeTime = 0.5f;

    FHGun gun;

    public void Setup(FHGun _gun)
    {
        gun = _gun;
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(lifeTime);

        gun.DespawnAOEEffect(gameObject.transform);
    }
}
