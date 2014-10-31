using UnityEngine;
using System.Collections;

public class BackgroudGun : MonoBehaviour {
	public Camera mView;
	// Use this for initialization
	void Start () {
		BoxCollider box = gameObject.GetComponent<BoxCollider> ();
		transform.localPosition = new Vector2 (0,mView.transform.position.y-mView.pixelHeight/2 +box.size.y/2);
	}
	

}
