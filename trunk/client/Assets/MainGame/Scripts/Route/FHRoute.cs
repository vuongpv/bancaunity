using UnityEngine;
using System.Collections;
using GFramework;
using System;

[RequireComponent(typeof(Spline))]
public class FHRoute : MonoBehaviour {

	public Spline spline;

	public int numSegments = 32;

	private Vector3[] positions;
	private Quaternion[] orientations;


	public bool isValid {
		get {
			return positions != null;
		}
	}

	void Awake()
	{
		if (spline == null)
			spline = GetComponent<Spline>();
	}

	public void CalculateSegments()
	{
        positions = new Vector3[numSegments + 1];
        orientations = new Quaternion[numSegments + 1];
        
        for (int i = 0; i <= numSegments; i++)
		{
			float t = (float)i / numSegments;
			positions[i] = spline.GetPositionOnSpline(t);
			orientations[i] = spline.GetOrientationOnSpline(t);
		}
	}

	public Vector3 GetPositionOnRoute(float t)
	{
		if (t >= 1)
			return positions[positions.Length - 1];

		float fidx = t * numSegments;
		int idx = (int) fidx;
		return Vector3.Lerp(positions[idx], positions[idx + 1], fidx - idx);
	}

	public Quaternion GetOrientationOnRoute(float t)
	{
		if (t >= 1)
			return orientations[orientations.Length - 1];

		float fidx = t * numSegments;
		int idx = (int)fidx;
		return Quaternion.Lerp(orientations[idx], orientations[idx + 1], fidx - idx);
	}

    public float GetLength()
    {
        return spline.splineLength;
    }

	
}