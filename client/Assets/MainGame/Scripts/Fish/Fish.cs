using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using GFramework;

public enum FHFishState
{
		Swim = 0,
		Dying,
		Dead,
}

public enum FHFishViewType
{
		None,
		Sprite2D,
		Model3D,
}

[Serializable]
public enum FishSpeed
{
		PARENT,// van toc theo group chua no
		MINIMUM,// van toc toi thieu
		NORMAL,// van toc trung binh
		MAXIMUM,// van toc toi da
}

public class Fish : _MyGameObject
{

		protected int mId;
		protected int mStatus;
		protected Camera mViewLimit;
		protected float leftLimit, rightLimit, topLimit, bottomLimit;
		protected float dx, dy;
		protected float curren_Angle = 0, next_Angle = 0, offsetAngle, detaAngle;
		protected MyAnimation mAnimation;
		protected float price = 0;
		private FHFishManager manager;
		public FHRoute route;
		[HideInInspector]
		public FHFishSeason
				season;
		[HideInInspector]
		public float
				routeFactor = 0f;

		public ConfigFishRecord configFish { get; private set; }

		[HideInInspector]
		public int
				fishIdentify = 0;
		public int groupID = -1;
		private float fadeOutTime;
		public const float FADE_TIME = 1f;
		protected string[] fr_move, fr_die;

		void Awake ()
		{
		
				SetSpeed (0.05f);
				mAnimation = GetComponent<MyAnimation> ();
				

		}

		public void SetManager (FHFishManager manager)
		{
				this.manager = manager;
		}

		public void Setup (ConfigFishRecord configFish, FHFishSeason season, FHRoute route)
		{
				this.configFish = configFish;
				this.season = season;
				this.route = route;

				Debug.Log ("======== Setup: " + configFish.id);
		
				this.routeFactor = 0;
		
				transform.position = new Vector3 (0, 1000, 0);
				
				transform.localScale = Vector3.one;
				Init (configFish.id);
		}

		public void SetupInGroup (ConfigFishRecord configFish, FHFishSeason season, FHRoute route, int fishGroupID, FGCustomInfo fishGroupInfo, int index)
		{
				Setup (configFish, season, route);
		
//				if (fishGroupInfo == null || fishGroupInfo.GetNodes ().Length <= index)
//						return;
//		
//				groupID = fishGroupID;
//		
//				FGCustomNode _element = fishGroupInfo.GetNodes () [index];
//				if (_element.fishSpeed == FishSpeed.PARENT)
//						internalSpeed = desSpeedDelay = GetSpeed (fishGroupInfo.baseSpeed);
//				else
//						internalSpeed = desSpeedDelay = GetSpeed (fishGroupInfo.GetNodes () [index].fishSpeed);
//		
//				posLocal = _element.GetPosition ();
//				loopSin = _element.loopSin;
//				heightSin = _element.heightSin;
//		
//				listEvent.Clear ();
//				for (int i = 0; i < fishGroupInfo.customEvent.Count; i++) {
//						FGCustomEvent _evt = fishGroupInfo.customEvent [i];
//						if (_evt != null)
//								listEvent [_evt] = false;
//				}
//		
//				for (int i = 0; i < _element.customEvent.Count; i++) {
//						FGCustomEvent _evt = _element.customEvent [i];
//						if (_evt != null)
//								listEvent [_evt] = false;
//				}
//		
//				saveSpeed = internalSpeed;
		}

		public virtual void Init (int id)
		{
				mId = id;
				LoadFrame ();

	
				ChangeStatus ((int)FISH_STATUS.ST_NORMAL);
		
				RandomPosition ();


				name = "f_" + mId;
				switch (mId) {
				case 7:
						mAnimation.SetDelay (2);
						SetSpeed (0.03f);
						break;
				case 6:
				case 10:
				case 14:
						mAnimation.SetDelay (4);
						SetSpeed (0.02f);
						break;
				case 11:
				case 12:
						mAnimation.SetDelay (5);
						SetSpeed (0.01f);
						break;
				case 13:
				case 15:
				case 16:
				case 17:
						mAnimation.SetDelay (3);
						SetSpeed (0.03f);
						break;

//		default:00
//			mAnimation.SetDelay(1);
//			SetSpeed(0.001f);
//			break;
				}

		}

		public float GetPrice ()
		{
				return price;
		}

		public void SetPrice (float p)
		{
				price = p;
		}

		protected virtual void LoadFrame ()
		{
				switch (mId) {
//				case 1:
//				case 2:
//				case 3:
//				case 4:
//				case 5:
//				case 8:
//				case 9:
//						fr_move = new string[] {
//				"f2000" + mId + "_m_01",
//				"f2000" + mId + "_m_02",
//				"f2000" + mId + "_m_03",
//				"f2000" + mId + "_m_04",
//				"f2000" + mId + "_m_05",
//				"f2000" + mId + "_m_06",
//				"f2000" + mId + "_m_07",
//				"f2000" + mId + "_m_08"
//			};
//						fr_die = new string[] {
//				"f2000" + mId + "_d_01",
//				"f2000" + mId + "_d_01",
//				"f2000" + mId + "_d_01",
//				"f2000" + mId + "_d_02",
//				"f2000" + mId + "_d_02",
//				"f2000" + mId + "_d_02",
//				"f2000" + mId + "_d_03",
//				"f2000" + mId + "_d_03",
//				"f2000" + mId + "_d_03"
//			};		
//						break;
		
//				case 6:
//						fr_move = new string[]{
//				"f2000" + mId + "_m_01",
//				"f2000" + mId + "_m_02",
//				"f2000" + mId + "_m_03",
//				"f2000" + mId + "_m_04",
//				"f2000" + mId + "_m_05",
//				"f2000" + mId + "_m_06",
//				"f2000" + mId + "_m_06",
//				"f2000" + mId + "_m_06",
//				"f2000" + mId + "_m_07",
//				"f2000" + mId + "_m_07",
//				"f2000" + mId + "_m_07",
//				"f2000" + mId + "_m_08",
//				"f2000" + mId + "_m_08",
//				"f2000" + mId + "_m_08"
//			};
//						fr_die = new string[]{
//				"f2000" + mId + "_d_01",
//				"f2000" + mId + "_d_01",
//				"f2000" + mId + "_d_01",
//				"f2000" + mId + "_d_02",
//				"f2000" + mId + "_d_02",
//				"f2000" + mId + "_d_02",
//				"f2000" + mId + "_d_03",
//				"f2000" + mId + "_d_03",
//				"f2000" + mId + "_d_03"};
//						break;
				case 7:
						fr_move = new string[] {
								"f2000" + mId + "_m_01",
								"f2000" + mId + "_m_02",
								"f2000" + mId + "_m_03",
								"f2000" + mId + "_m_04",
								"f2000" + mId + "_m_05",
								"f2000" + mId + "_m_06"
						};
						fr_die = new string[] {
								"f2000" + mId + "_d_01",
								"f2000" + mId + "_d_01",
								"f2000" + mId + "_d_01",
								"f2000" + mId + "_d_02",
								"f2000" + mId + "_d_02",
								"f2000" + mId + "_d_02",
								"f2000" + mId + "_d_03",
								"f2000" + mId + "_d_03",
								"f2000" + mId + "_d_03"
						};
				
						break;
				default:
						fr_move = new string[] {
				"f2000" + mId + "_m_01",
				"f2000" + mId + "_m_02",
				"f2000" + mId + "_m_03",
				"f2000" + mId + "_m_04",
				"f2000" + mId + "_m_05",
				"f2000" + mId + "_m_06",
				"f2000" + mId + "_m_07",
				"f2000" + mId + "_m_08"
			};
						fr_die = new string[] {
				"f2000" + mId + "_d_01",
				"f2000" + mId + "_d_01",
				"f2000" + mId + "_d_01",
				"f2000" + mId + "_d_02",
				"f2000" + mId + "_d_02",
				"f2000" + mId + "_d_02",
				"f2000" + mId + "_d_03",
				"f2000" + mId + "_d_03",
				"f2000" + mId + "_d_03"
			};		
						break;
				case 10:
						fr_move = new string[]{
				"f200" + mId + "_m_01",
				"f200" + mId + "_m_02",
				"f200" + mId + "_m_03",
				"f200" + mId + "_m_04",
				"f200" + mId + "_m_05",
				"f200" + mId + "_m_06",

				"f200" + mId + "_m_07",

				"f200" + mId + "_m_08"
			};
						fr_die = new string[]{
				"f200" + mId + "_d_01",
				"f200" + mId + "_d_01",
				"f200" + mId + "_d_01",
				"f200" + mId + "_d_02",
				"f200" + mId + "_d_02",
				"f200" + mId + "_d_02",
				"f200" + mId + "_d_03",
				"f200" + mId + "_d_03",
				"f200" + mId + "_d_03"};
						break;

				case 11:
				case 12:
						fr_move = new string[] {
				"f200" + mId + "_m_01",
				"f200" + mId + "_m_02",
				"f200" + mId + "_m_02",
				"f200" + mId + "_m_03",
				"f200" + mId + "_m_03",
				"f200" + mId + "_m_03",
				"f200" + mId + "_m_04",
				"f200" + mId + "_m_05",
				"f200" + mId + "_m_06",
				"f200" + mId + "_m_07",
				"f200" + mId + "_m_08"
			};
						fr_die = new string[] {
				"f200" + mId + "_d_01",
				"f200" + mId + "_d_01",
				"f200" + mId + "_d_01",
				"f200" + mId + "_d_02",
				"f200" + mId + "_d_02",
				"f200" + mId + "_d_02",
				"f200" + mId + "_d_03",
				"f200" + mId + "_d_03",
				"f200" + mId + "_d_03"
			};		
						break;
		
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
						fr_move = new string[] {
				"f200" + mId + "_m_01",
				"f200" + mId + "_m_02",
				"f200" + mId + "_m_03",
				"f200" + mId + "_m_04",
				"f200" + mId + "_m_05",
				"f200" + mId + "_m_06",
				"f200" + mId + "_m_07",
				"f200" + mId + "_m_08"
			};
						fr_die = new string[] {
				"f200" + mId + "_d_01",
				"f200" + mId + "_d_01",
				"f200" + mId + "_d_01",
				"f200" + mId + "_d_02",
				"f200" + mId + "_d_02",
				"f200" + mId + "_d_02",
				"f200" + mId + "_d_03",
				"f200" + mId + "_d_03",
				"f200" + mId + "_d_03"
			};		
						break;
				}


		}

		public void SetCamera (Camera c)
		{
				mViewLimit = c;

				leftLimit = -Screen.width / 2;
				rightLimit = Screen.width / 2;
				topLimit = Screen.height / 2;
				bottomLimit = -Screen.height / 2;


		}


	
		// Update is called once per frame
		public void Update ()
		{
//				if (!mAnimation.IsUpdate ()) {
//						Debug.LogError ("=================================== bug here update");
//						return;
//				}

				if (route == null) {
						return;
				}
			
		
//				if (state == FHFishState.Swim) {
//					
//						routeFactor += Time.deltaTime * GetSpeed ();
//			
//						routeFactor = Mathf.Clamp01 (routeFactor);
//			
//						
//						UpdateFishAlone ();
//				
//						if (routeFactor >= 1f)
//								Despawn ();
//				} else if (state == FHFishState.Dying) {
//						fadeOutTime += Time.deltaTime;
//						if (fadeOutTime > FADE_TIME) {
//								state = FHFishState.Dead;
//								Despawn ();
//						}
//				}

				base.Update ();	
				switch (mStatus) {
				case (int)FISH_STATUS.ST_NORMAL:
//						if (curren_Angle != next_Angle) {
//								UpdateAngle ();
//						} else {
//								StartCoroutine (WaitingNextAngle ());
//						}
//
//
//						Move (dx, dy);
//						if (!CheckLimit ()) {
//								RandomPosition ();		
//						}

						routeFactor += Time.deltaTime * GetSpeed ();
			
						routeFactor = Mathf.Clamp01 (routeFactor);
			
			
						transform.position = route.GetPositionOnRoute (routeFactor);
						transform.right = -MathfEx.GetForwardVector (route.GetOrientationOnRoute (routeFactor));
			
						if (transform.right.x < 0) {
								transform.localEulerAngles = new Vector3 (180.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
								mAnimation.transform.localEulerAngles = new Vector3 (180.0f, mAnimation.transform.localEulerAngles.y, mAnimation.transform.localEulerAngles.z);
				
						}
			
						if (routeFactor >= 1f)
								Despawn ();

						break;
				case (int)FISH_STATUS.ST_DIE:
//						if (mAnimation.EndFrame ()) {
////								gameObject.SetActive (false);
////								RandomPosition ();
//								Despawn ();
//				
//				
//						}

						fadeOutTime += Time.deltaTime;
						if (fadeOutTime > FADE_TIME) {
								Despawn ();
						}

						break;
				}
		}

		public void Despawn ()
		{
				this.manager.CollectFish (this);
		}

		

		public void ChangeStatus (int newStatus)
		{
				mStatus = newStatus;
				switch (mStatus) {
				case (int) FISH_STATUS.ST_NORMAL:
						mAnimation.SetFrame (fr_move);
//						switch (mId) {
//						case 7:
//								mAnimation.SetDelay (2);
//								break;
//						case 6:
//						case 10:
//								mAnimation.SetDelay (4);
//								break;
//						case 11:
//						case 12:
//								mAnimation.SetDelay (5);
//								break;
//						case 13:
//						case 14:
//						case 15:
//						case 16:
//						case 17:
//								mAnimation.SetDelay (3);
//								break;
//						}
						break;
				case (int)FISH_STATUS.ST_DIE:
//						mAnimation.SetDelay (1);
						mAnimation.SetFrame (fr_die);

						break;
				}
		}

		public int GetStatus ()
		{
				return mStatus;
		}

		public virtual bool CheckLimit ()
		{
	
				if (transform.localPosition.x < leftLimit - 2 * GetWidthSprite ())
						return false;
				else if (transform.localPosition.x >= rightLimit + 2 * GetWidthSprite ())
						return false;
				if (transform.localPosition.y < bottomLimit)
						return false;
				else if (transform.localPosition.y > topLimit)
						return false;
				return true;
		}

		public virtual void RandomPosition ()
		{
				ChangeStatus ((int)FISH_STATUS.ST_NORMAL);
				int r = UnityEngine.Random.Range (0, 10);
				float x, y, angle = 0;
				switch (r) {
				case 0:
				case 1:
				case 2:
				case 3:
						x = leftLimit;
						y = UnityEngine.Random.Range (bottomLimit, topLimit - GetHeightSprite ()) + 1;
						angle = UnityEngine.Random.Range (-50, 50);
						break;

				default:
						x = rightLimit + GetWidthSprite () / 2;
						y = UnityEngine.Random.Range (bottomLimit, topLimit - GetHeightSprite () + 1);
						angle = UnityEngine.Random.Range (140, 220);
						break;
				}

				transform.localPosition = new Vector2 (x, y);

				SetAngleObject (angle);
				Setdx_dy (angle);
				gameObject.SetActive (true);
		}

		protected virtual void Setdx_dy (float angle)
		{
				float radi = Mathf.Deg2Rad * angle;
				dx = Mathf.Cos (radi) * GetSpeed ();
				dy = Mathf.Sin (radi) * GetSpeed ();
		}

		IEnumerator WaitingNextAngle ()
		{
				yield return new WaitForSeconds (1f);
				SetNextAngle (UnityEngine.Random.Range (curren_Angle - 90, curren_Angle + 90));
		}

		private void UpdateAngle ()
		{
				detaAngle -= Mathf.Abs (offsetAngle);

				curren_Angle += offsetAngle;
				if ((offsetAngle > 0 && curren_Angle > next_Angle) || (offsetAngle < 0 && curren_Angle < next_Angle))
						curren_Angle = next_Angle;
			
				SetAngleObject (curren_Angle);

				Setdx_dy (curren_Angle);
		}

		public virtual void SetNextAngle (float a)
		{

				next_Angle = a;
		
				if (next_Angle > 360)
						next_Angle -= 360;
		
				offsetAngle = 1;
		
				if (next_Angle > curren_Angle) {
						detaAngle = next_Angle - curren_Angle;
				} else if (next_Angle < curren_Angle) {
						offsetAngle = -offsetAngle;
						detaAngle = curren_Angle - next_Angle;
				} else {
						offsetAngle = 0;
				}
		}

}

enum FISH_STATUS
{
		ST_NORMAL=1,
		ST_DIE=2,
		ST_CATCH=3,
		ST_STUN=4,
		ST_FINISH=5
}
;
