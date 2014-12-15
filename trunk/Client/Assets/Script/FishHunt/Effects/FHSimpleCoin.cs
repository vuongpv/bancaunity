using UnityEngine;
using System.Collections;
using System;

public class FHSimpleCoin : MonoBehaviour
{
    const float FLY_SPEED = 2f;

    Transform _transform;
    Vector3 direction;
    bool canFly = false;

	Vector3 originScale;

	public string type;

	ICollectibleTarget target;

	void Awake()
	{
		_transform = gameObject.transform;
		originScale = _transform.localScale;
	}

	public void Setup(ICollectibleTarget target)
    {
		_transform.localScale = originScale;

		this.target = target;
		direction = (target.GetTargetPos(type) - _transform.position).normalized;

        canFly = true;
    }

    void Update()
    {
        if (!canFly)
            return;

		_transform.position += direction * FLY_SPEED * Time.deltaTime;
		if (Vector3.Dot(target.GetTargetPos(type) - _transform.position, direction) < 0)
		{
			FHGuiCollectibleManager.instance.Collect(_transform);
			target.OnReachTarget(type);
		}
    }

	void OnDrawGizmos()
	{
		if( Application.isPlaying )
		{
			Gizmos.DrawLine(_transform.position, _transform.position + direction * 100f);
		}
	}
}
