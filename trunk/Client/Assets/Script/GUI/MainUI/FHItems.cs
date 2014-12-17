using UnityEngine;
using System.Collections;

public class FHItems : MonoBehaviour
{
		public UILabel numberBoom, numberIce;
		private int indexITem = -1;
		public GameObject boomObject, boomIcon, iceObject, iceIcon;
		private BoxCollider boxBoom, boxIce;
		public Camera camera;
		UISprite sprite;

		void Start ()
		{
				boxBoom = boomIcon.GetComponent<BoxCollider> ();
				boxIce = iceIcon.GetComponent<BoxCollider> ();

				sprite = boomIcon.GetComponent<UISprite> ();
		}

		void Update ()
		{
			
		}

		public static Vector3 convertPositionToCamera (Vector3 v, Camera c)
		{
				return c.ScreenToWorldPoint (v);
		}
		
		public bool OnFingerDown (Vector3 position)
		{
//				Vector2 p = new Vector2 (position.x, position.z);
				Debug.Log ("====  position: " + position.ToString () + ",m  " + boxBoom.bounds.ToString ());		
				position = convertPositionToCamera (position, camera);
//				Debug.Log ("==== boxBoom: " + boxBoom.s.center.x + "," + boxBoom.bounds.center.y + "," + boxBoom.bounds.size.x + "," + boxBoom.bounds.size.y + ", boxIce: " + boxIce.bounds.ToString () + ",position: " + position.x + ", " + position.y + ", " + position.z);
				Debug.Log ("==== boxBoom: " + boxBoom.size.ToString () + boomObject.transform.position.ToString () + ", position: " + position.ToString ());		
				if (boxBoom.bounds.Contains (position)) {
						return true;
				} else if (boxIce.bounds.Contains (position))
						return true;
				Debug.Log ("****************************** false false false");
				return false;

//				switch (UICamera.selectedObject.name) {
//				case "Boom":
//						indexITem = 0;
//						boomObject.SetActive (true);
//						return true;
//				case "Ice":
//						indexITem = 1;
//						iceObject.SetActive (true);
//						return true;
//				}
//				return false;

//				Debug.LogError ("==============  indexITem: " + indexITem);
//				Bounds b = new Bounds (new Vector3 (-1, -1, 0), new Vector3 (sprite.width, sprite.height, 0));
//
//				if (b.Contains (position)) {
//						indexITem = 0;
//				} else if (boxIce.bounds.Contains (position)) {
//						indexITem = 1;
//				}
		}

		public bool OnFingerMove (Vector3 position)
		{
//				Debug.LogError ("OnFingerMove:  " + position.x + "," + position.y + ", " + position.z);
//				position = convertPositionToCamera (new Vector2 (position.x, position.z), camera);
//				position = new Vector2 (position.x, position.z);

//				Vector3 pos = Camera.main.ScreenToWorldPoint (position);
				switch (indexITem) {
				case 0:
						boomObject.transform.position = position;
						return true;
				case 1:
						iceObject.transform.position = position;
						return true;
				}
				return false;

		}

		public bool OnFingerUp ()
		{
				if (indexITem >= 0) {
						indexITem = -1;
						return true;
				}
				return false;
		}

}
