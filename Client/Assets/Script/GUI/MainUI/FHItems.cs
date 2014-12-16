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

		
	#region [ Input events ]
		public void OnFingerDown (Vector3 position)
		{
				switch (UICamera.selectedObject.name) {
				case "Boom":
						indexITem = 0;
						boomObject.SetActive (true);
						break;
				case "Ice":
						indexITem = 1;
						iceObject.SetActive (true);
						break;
				}

//				Debug.LogError ("==============  indexITem: " + indexITem);
//				Bounds b = new Bounds (new Vector3 (-1, -1, 0), new Vector3 (sprite.width, sprite.height, 0));
//
//				if (b.Contains (position)) {
//						indexITem = 0;
//				} else if (boxIce.bounds.Contains (position)) {
//						indexITem = 1;
//				}
		}

		public void OnFingerMove (Vector3 position)
		{
//				Debug.LogError ("OnFingerMove:  " + position.x + "," + position.y + ", " + position.z);
				position = convertPositionToCamera (new Vector2 (position.x, position.z), camera);
//				position = new Vector2 (position.x, position.z);

//				Vector3 pos = Camera.main.ScreenToWorldPoint (position);
				switch (indexITem) {
				case 0:
//						boomObject.transform.position = new Vector3 (boomObject.transform.position.x + position.x, boomObject.transform.position.y + position.y, boomObject.transform.position.z + position.z);
//						boomObject.transform.position = Vector3.Lerp (boomObject.transform.position, position, 0.001f);
//						boomObject.transform.forward = (position - boomObject.transform.position).normalized;
						boomObject.transform.position = position;
						break;
				case 1:
						
						break;
				}

		}
		public void OnFingerUp ()
		{
//				Debug.LogError ("===================== reset item");
//				indexITem = -1;
		}
	#endregion

}
