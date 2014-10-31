using UnityEngine;
using System.Collections;

public class ControlGun : MonoBehaviour {
	public GameObject leftObject,rightObject;
	private BoxCollider leftBox,rightBox;

	void Start()
	{
		leftBox=leftObject.GetComponent<BoxCollider>();
		rightBox=rightObject.GetComponent<BoxCollider>();
	}

	void OnTap(TapGesture gesture) { 
		if (leftBox.bounds.Contains (gesture.Position) || rightBox.bounds.Contains (gesture.Position))
			Debug.Log ("left right touch");
	}
}
