using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHBackground : MonoBehaviour
{

		public GameObject mBackgroud;
		
		private MeshRenderer mMeshRender;
		private	List<Texture> backgrouds;
		private readonly int maxBackgroud = 3;
		
		private	int mIndexBg;
		private bool isChange = false;
		Rect rec;
		public Camera mCamera;



		// Use this for initialization
		void Start ()
		{
				mMeshRender = mBackgroud.GetComponent<MeshRenderer> ();
				LoadBackgroud ();
				if (mCamera != null)
						rec = mCamera.pixelRect;

			
		}

		
	
		private void LoadBackgroud ()
		{
				backgrouds = new List<Texture> ();
				for (int i=0; i<maxBackgroud; i++) {
						Texture t = Resources.Load ("Sprite/Backgroud/bg_" + (i + 1)) as Texture;
						if (t != null)
								backgrouds.Add (t);
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
//				if (isChange) {
//						ScrollBackgroud ();
//						return;
//				}
//				
//				StartCoroutine (WaitChangeBackgroud (5));
		}

		IEnumerator WaitChangeBackgroud (float timeDelay)
		{
				yield return new WaitForSeconds (timeDelay);
				isChange = true;
				mIndexBg++;
				if (mIndexBg >= maxBackgroud)
						mIndexBg = 0;
		}

		private void ScrollBackgroud ()
		{		
				rec.width = rec.width - 1f;
				mCamera.pixelRect = rec;
				if (mCamera.rect.width <= 0) {
						isChange = false;
						ChangeBackgroud (mIndexBg);
						rec.width = Screen.width;
						mCamera.pixelRect = rec;
				}

		}
	
	


		public void ChangeBackgroud (Texture newBg)
		{
				if (null == newBg) {
						Debug.LogError ("new texture is null");
						return;
				}
				mMeshRender.material.mainTexture = newBg;
		}

		public void ChangeBackgroud (int index)
		{
				if (index < 0 || index >= maxBackgroud) {
						Debug.LogError ("index not avalible");
						return;
				}
				mMeshRender.material.mainTexture = backgrouds [index];
				isChange = false;
		}
}
