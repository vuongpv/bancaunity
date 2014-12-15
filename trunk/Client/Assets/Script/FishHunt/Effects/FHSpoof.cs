using UnityEngine;
using System.Collections;

public class FHSpoof : MonoBehaviour
{
    public float lifeTime = 1.50f;

    SpawnPool pool;

    public void Setup(SpawnPool _pool)
    {
        pool = _pool;

        StartCoroutine(Despawn());
    }
	
    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(lifeTime);

        pool.Despawn(gameObject.transform);
    }
}
