using UnityEngine;
using System.Collections;
using System;

public class RippleInput : MonoBehaviour
{
	public RippleMesh rippleMesh;
	public float minInputInterval = 0.3f;
	private float lastInputTime;

	// Cache
	private Collider _collider;

	void Awake()
	{
		_collider = collider;

		if (rippleMesh == null)
		{
			rippleMesh = GetComponent<RippleMesh>();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			if (Time.realtimeSinceStartup - lastInputTime < minInputInterval)
				return;

			lastInputTime = Time.realtimeSinceStartup;

			RaycastHit hit;
			if (_collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue))
			{
				rippleMesh.SplashAtTexCoordPoint(hit.textureCoord);
			}
		}

	}
}

