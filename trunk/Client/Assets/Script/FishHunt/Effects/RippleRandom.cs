using UnityEngine;
using System.Collections;
using System;

public class RippleRandom : MonoBehaviour
{
	public RippleMesh rippleMesh;

	public float intervalMin = 0.5f;
	public float intervalMax = 2f;

	public int splashPerInterval = 1;

	private float timeRemain;
	
	void Awake()
	{
		if (rippleMesh == null)
		{
			rippleMesh = GetComponent<RippleMesh>();
		}
	}

	void Start()
	{
		timeRemain = UnityEngine.Random.Range(intervalMin, intervalMax);
	}

	void Update()
	{
		timeRemain -= Time.deltaTime;
		if (timeRemain <= 0)
		{
			timeRemain = UnityEngine.Random.Range(intervalMin, intervalMax);

			for (int i = 0; i < splashPerInterval; i++)
			{
				int col = UnityEngine.Random.Range(0, rippleMesh.cols);
				int row = UnityEngine.Random.Range(0, rippleMesh.rows);
				rippleMesh.SplashAtGridPoint(col, row);
			}
		}
	}
}

