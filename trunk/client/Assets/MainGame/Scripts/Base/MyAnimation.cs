using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MyAnimation : UISpriteAnimation
{



		protected bool endFrame = false;
		protected float timeDelay = 1;
		private bool isUpdate = true;

		void Awake ()
		{
				RebuildSpriteList ();
		transform.localRotation =Quaternion.Euler(0.0f, 0.0f, 180f)
		}



		protected override void  Start ()
		{
		}
		/// <summary>
		/// Advance the sprite animation process.
		/// </summary>

		public void SetDelay (float delay)
		{
				timeDelay = delay;
		}
		public override void Update ()
		{
				if (mIndex == 0)
						endFrame = false;
				if (mActive && mSpriteNames.Count > 1 && Application.isPlaying && mFPS > 0) {
						if (isUpdate)
								isUpdate = false;
						mDelta += RealTime.deltaTime;
						float rate = timeDelay / mFPS;
			
						if (rate < mDelta) {
								isUpdate = true;
								mDelta = (rate > 0f) ? mDelta - rate : 0f;
								++mIndex;
								if (mIndex >= mSpriteNames.Count) {
										endFrame = true;

										mIndex = 0;
										mActive = mLoop;
								}
				
								if (mActive) {
										mSprite.spriteName = mSpriteNames [mIndex];

										if (mSnap)
												mSprite.MakePixelPerfect ();
								}
						}
				}
		}


		public bool IsUpdate ()
		{
				return isUpdate;
		}

		public void SetFrame (string[] names)
		{
				if (null == names || names.Length <= 0)
						return;
				mSpriteNames.Clear ();

				for (int i=0; i<names.Length; i++) {
						mSpriteNames.Add (names [i]);
				}
				endFrame = false;
				mIndex = 0;
				mSprite.spriteName = mSpriteNames [mIndex];
		}


		public override void RebuildSpriteList ()
		{
//		Debug.Log ("RebuildSpriteList");
				if (mSprite == null)
						mSprite = GetComponent<UISprite> ();

				if (null == mSpriteNames)
						mSpriteNames = new List<string> ();
		}

		public void SetIndexFrame (int index)
		{
				mIndex = index;
		}
		public int GetIndexFrame ()
		{
				return mIndex;
		}

		public bool EndFrame ()
		{
				return endFrame;
		}


		public string GetNameCurrentFrame ()
		{
				return 		mSprite.spriteName;
		}
	
}
