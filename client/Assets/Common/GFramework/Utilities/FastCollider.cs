using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using GFramework;

[Serializable]
public class FastColliderNode
{
	public Transform transform;

	public string name;
	public Vector3 firstPt;
	public Vector3 lastPt;
	public float radius;

	public void Init(string name, Transform transform)
	{
		this.name = name;
		this.firstPt = Vector3.right;
		this.lastPt = -Vector3.right;
		this.radius = 0.5f;
		this.transform = transform;
	}

	public Vector3 MulVec3(Vector3 v1, Vector3 v2)
	{
		return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
	}

	public Vector3 GetWorldFirstPt()
	{
		return (transform.rotation * MulVec3(firstPt, transform.lossyScale)) + transform.position;
	}

	public Vector3 GetWorldLastPt()
	{
		return (transform.rotation * MulVec3(lastPt, transform.lossyScale)) + transform.position;
	}

	public Vector3 GetWorldCenter()
	{
		Vector3 center = firstPt + (lastPt - firstPt) * 0.5f;
		return (transform.rotation * MulVec3(center, transform.lossyScale)) + transform.position;
	}

	public float GetWorldRadius()
	{
		return radius * transform.lossyScale.x;
	}

	public bool IsLineSegmentIntersect(Vector3 lineStart, Vector3 lineEnd, out Vector3 intersectPt)
	{
		Vector3 wFirstPt = GetWorldFirstPt();
		Vector3 wLastPt = GetWorldLastPt();
		float wRadius = GetWorldRadius();

		Vector3 ptCollider;
		float sqrDistance = MathfEx.SqrDistanceSegmentToSegment(lineStart, lineEnd, wFirstPt, wLastPt, out intersectPt, out ptCollider);
		return sqrDistance < (wRadius * wRadius);
	}

	public bool IsSphereOverlapped(Vector3 origin, float radius)
	{
		Vector3 wFirstPt = GetWorldFirstPt();
		Vector3 wLastPt = GetWorldLastPt();
		float wRadius = GetWorldRadius();

		int closestPoint;
		float sqrDistance = MathfEx.SqrDistancePointToSegment(origin, wFirstPt, wLastPt, out closestPoint);
		return sqrDistance < ((wRadius + radius) * (wRadius + radius));
	}
}


public class FastCollider : MonoBehaviour {

	public List<FastColliderNode> colliderNodes;

	private Transform _transform;


	void Awake()
	{
		_transform = transform;
	}

	//void OnDrawGizmos()
	//{
	//	return;
	//	foreach (var node in colliderNodes)
	//	{
	//		Matrix4x4 localMatrix = new Matrix4x4();
	//		Vector3 diff = node.lastPt - node.firstPt;
	//		float length =  diff.magnitude;
	//		localMatrix.SetTRS(node.firstPt + (diff * 0.5f), diff.sqrMagnitude > 0 ? Quaternion.LookRotation(diff, Vector3.up) : Quaternion.identity, transform.lossyScale);
	//		Gizmos.matrix = transform.localToWorldMatrix * localMatrix;
	//		Gizmos.DrawWireCube(Vector3.zero, new Vector3(node.radius * 2, node.radius * 2, length));
	//		Gizmos.color = Color.red;
	//		Gizmos.DrawCube(Vector3.zero, new Vector3(0.01f, 0.01f, length));
	//	}
	//}

	public bool IsLineSegmentIntersect(Vector3 lineStart, Vector3 lineEnd, out Vector3 intersectPt)
	{
		foreach (var node in colliderNodes)
		{
			if (node.IsLineSegmentIntersect(lineStart, lineEnd, out intersectPt))
				return true;
		}

		intersectPt = Vector3.zero;
		return false;
	}

	public bool IsSphereOverlapped(Vector3 origin, float radius)
	{
		foreach (var node in colliderNodes)
		{
			if (node.IsSphereOverlapped(origin, radius))
				return true;
		}

		return false;
	}
}	
