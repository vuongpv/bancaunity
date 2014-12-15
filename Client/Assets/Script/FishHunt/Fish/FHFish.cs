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

public class FHFish : MonoBehaviour
{
		public const float FADE_TIME = 1f;
		public const float FLICK_TIME = 0.24f;

		public ConfigFishRecord configFish { get; private set; }

		public FHFishViewType viewType;
		public FHRoute route;
		[HideInInspector]
		public FHFishSeason
				season;
		private FHFishManager manager;
		public Color hitColor = Color.red;
		public float speed = 1f;

		public FastCollider fastCollider { get; private set; }

		[NonSerialized]
		public FHFishState
				state;
		[HideInInspector]
		public float
				routeFactor = 0f;
		private PackedSprite sprite;

		public Transform _transform { get; private set; }

		private Renderer _renderer;
		private Material _material;
		public GameObject particle;
		private float desSpeedDelay = 0.1f;
		public int hitTimes = 0;
    
		//thuantq : add fish ID on single and Network static
		public static Dictionary<int,int> FISH_ID = new Dictionary<int,int> ();

		public static int GetGenerateFishID (int fishType)
		{
				if (!FISH_ID.ContainsKey (fishType)) {
						FISH_ID [fishType] = (fishType % 1000) * 1000;
				} else {
						FISH_ID [fishType]++;
				}
				return FISH_ID [fishType];
		}

		[HideInInspector]
		public int
				fishIdentify = 0;
		private float fadeOutTime;

    #region Effects
		public float posX {
				get { return _transform.position.x; }
				set { _transform.position = new Vector3 (value, _transform.position.y, _transform.position.z); }
		}
    
		public float posZ {
				get { return _transform.position.z; }
				set { _transform.position = new Vector3 (_transform.position.x, _transform.position.y, value); }
		}
    #endregion

    #region Fish in group
		public int groupID = -1;
		private Vector3 posLocal = new Vector3 (0, 0, 0);// x : toa do ngang, z: toa do do, y toa do canh theo line (di truoc)
    
		private float heightSin = 0;
		private int loopSin = 0;// maximum 5 time loop sin circle in a line
    
		private float currentSin = 0;
		private Dictionary<FGCustomEvent, bool> listEvent = new Dictionary<FGCustomEvent, bool> ();
		private float saveSpeed = 0.1f;
		private float internalSpeed = 0.1f;
		private bool isStop = false;
		public float flickTime;
		private float eventFactor = 0;
    #endregion

		public bool testHit;
		private bool isFireWorks = false;
		private float dx, dz;
    
		public void SetManager (FHFishManager manager)
		{
				this.manager = manager;
		}

		void Awake ()
		{
				_transform = transform;
				_renderer = GetComponentInChildren<Renderer> ();

				DetectViewType ();

				if (viewType == FHFishViewType.Model3D) {
						_material = _renderer.material;
				} else if (viewType == FHFishViewType.Sprite2D) {
						sprite = GetComponentInChildren<PackedSprite> ();
				}

				fastCollider = GetComponent<FastCollider> ();
		}

		void OnDestroy ()
		{
				if (_material)
						Destroy (_material);
		}

		void OnDrawGizmos ()
		{
				//Gizmos.matrix = transform.localToWorldMatrix;
				//Gizmos.DrawWireCube(Vector3.zero, new Vector3(boundRadius * 2,  boundRadius * 2, boundLength));
		}

		public void DetectViewType ()
		{
				if (GetComponentInChildren<PackedSprite> () != null)
						viewType = FHFishViewType.Sprite2D;
				else
						viewType = FHFishViewType.Model3D;
		}

		public void Setup (ConfigFishRecord configFish, FHFishSeason season, FHRoute route)
		{
				if (particle != null)
						Destroy (particle);
        
				this.configFish = configFish;
				this.fishIdentify = GetGenerateFishID (configFish.id);// generate id sync on network
				this.season = season;
				this.route = route;
		
				this.routeFactor = 0;
				this.state = FHFishState.Swim;

				this.flickTime = FLICK_TIME;

				this.groupID = -1;

				_transform.position = new Vector3 (0, 1000, 0);

				if (viewType == FHFishViewType.Model3D) {
						_material.shader = Shader.Find ("Gfw/RimLight Lite");
						_material.color = Color.white;
						_material.SetColor ("_OutlineColor", Color.white);
				} else
        		if (viewType == FHFishViewType.Sprite2D) {
						sprite.PlayAnim ("swim", UnityEngine.Random.Range (0, sprite.GetAnim ("swim").GetFrameCount ()));
						if (sprite != null) {
								sprite.SetColor (Color.white);
						}
				}
		}

		public void Setup (ConfigFishRecord configFish, FHFishSeason season, FHRoute route, float deg)
		{
				if (particle != null)
						Destroy (particle);
		
				this.configFish = configFish;
				this.fishIdentify = GetGenerateFishID (configFish.id);// generate id sync on network
				this.season = season;
				this.route = route;
				this.isFireWorks = true;
		
				this.routeFactor = 0;
				this.state = FHFishState.Swim;
		
				this.flickTime = FLICK_TIME;
		
				this.groupID = -1;
		
				_transform.position = new Vector3 (0, 0, 0);

				_transform.rotation = Quaternion.Euler (0, deg, 0);

				float radi = Mathf.Deg2Rad * deg;
				dx = Mathf.Cos (radi) * GetSpeed (FishSpeed.MINIMUM);
				dz = Mathf.Sin (radi) * GetSpeed (FishSpeed.MINIMUM);
		
				if (viewType == FHFishViewType.Model3D) {
						_material.shader = Shader.Find ("Gfw/RimLight Lite");
						_material.color = Color.white;
						_material.SetColor ("_OutlineColor", Color.white);
				} else
			if (viewType == FHFishViewType.Sprite2D) {
						
						sprite.PlayAnim ("swim", UnityEngine.Random.Range (0, sprite.GetAnim ("swim").GetFrameCount ()));
						if (sprite != null) {
								sprite.SetColor (Color.white);
						}
				}
		}

		void Update ()
		{
#if UNITY_EDITOR
				if (testHit) {
						StartCoroutine ("FlickColor");
						testHit = false;
				}
#endif

				if (route == null) {
						
						return;
				}

				if (state == FHFishState.Swim) {
						if (groupID != -1) {
								UpdateInGroup ();
								return;
						}

						routeFactor += Time.deltaTime * speed;

						routeFactor = Mathf.Clamp01 (routeFactor);

						if (isFireWorks) {
								UpdateFireWork ();
						} else {
								_transform.position = route.GetPositionOnRoute (routeFactor);
								_transform.right = -MathfEx.GetForwardVector (route.GetOrientationOnRoute (routeFactor));
				
								if (_transform.right.x < 0)
										_transform.localEulerAngles = new Vector3 (180.0f, _transform.localEulerAngles.y, _transform.localEulerAngles.z);
				
						}
						if (routeFactor >= 1f)
								Despawn ();
				} else if (state == FHFishState.Dying) {
						fadeOutTime += Time.deltaTime;
						if (fadeOutTime > FADE_TIME) {
								state = FHFishState.Dead;
								Despawn ();
						} else
								sprite.Color = new Color (1, 1, 1, 1f - (fadeOutTime / FADE_TIME));
				}


		}

		public void Die (Action onComplete)
		{
				state = FHFishState.Dying;
				fadeOutTime = 0;

				if (onComplete != null) 
						onComplete ();

				sprite.PlayAnim ("dead");
        
				//_collider.enabled = false;

				/*if (viewType == FHFishViewType.Model3D)
		{
			_material.shader = Shader.Find("Gfw/RimLight Trans Lite");

			_material.color = Color.white;
			HOTween.To(_material, 1f, new TweenParms()
				.Prop("color", new Color(1, 1, 1, 0))
				.Delay(0.5f)
				.OnStart(() => { if (onComplete != null) onComplete(); })
				.OnComplete(() => { Despawn(); }));
		}
		else if (viewType == FHFishViewType.Sprite2D)
		{
			sprite.PlayAnim("dead");

			HOTween.To(sprite, 1f, new TweenParms()
				.Prop("Color", new Color(1, 1, 1, 0))
				.Delay(0.5f)
				.OnStart(() => { if (onComplete != null) onComplete(); })
				.OnComplete(() => { Despawn(); }));
		}*/
		}

		public void Despawn ()
		{
				this.manager.CollectFish (this);
		}

		public void OnBulletHit ()
		{
				StopCoroutine ("FlickColor");
				StartCoroutine ("FlickColor");
		}

		public IEnumerator FlickColor ()
		{
				if (viewType == FHFishViewType.Model3D) {
						_material.SetColor ("_OutlineColor", Color.red);
						yield return new WaitForSeconds (flickTime);
						_material.SetColor ("_OutlineColor", Color.white);
				} else if (viewType == FHFishViewType.Sprite2D) {
						sprite.SetColor (Color.Lerp (Color.white, hitColor, 0.5f));
						yield return new WaitForSeconds (flickTime / 3);
						sprite.SetColor (hitColor);
						yield return new WaitForSeconds (flickTime / 3);
						sprite.SetColor (Color.Lerp (Color.white, hitColor, 0.5f));
						yield return new WaitForSeconds (flickTime / 3);
						sprite.SetColor (Color.white);

						flickTime = FLICK_TIME;
				}
		}

		public bool IsVisible ()
		{
				return (_transform.position.x >= FHSystem.instance.boundLeft
						&& _transform.position.x <= FHSystem.instance.boundRight
						&& _transform.position.z <= FHSystem.instance.boundTop
						&& _transform.position.z >= FHSystem.instance.boundBottom);
		}

    #region Fish in group
		public void SetupInGroup (ConfigFishRecord configFish, FHFishSeason season, FHRoute route, int fishGroupID, FGCustomInfo fishGroupInfo, int index)
		{
				Setup (configFish, season, route);

				if (fishGroupInfo == null || fishGroupInfo.GetNodes ().Length <= index)
						return;

				groupID = fishGroupID;

				FGCustomNode _element = fishGroupInfo.GetNodes () [index];
				if (_element.fishSpeed == FishSpeed.PARENT)
						internalSpeed = desSpeedDelay = GetSpeed (fishGroupInfo.baseSpeed);
				else
						internalSpeed = desSpeedDelay = GetSpeed (fishGroupInfo.GetNodes () [index].fishSpeed);

				posLocal = _element.GetPosition ();
				loopSin = _element.loopSin;
				heightSin = _element.heightSin;

				listEvent.Clear ();
				for (int i = 0; i < fishGroupInfo.customEvent.Count; i++) {
						FGCustomEvent _evt = fishGroupInfo.customEvent [i];
						if (_evt != null)
								listEvent [_evt] = false;
				}

				for (int i = 0; i < _element.customEvent.Count; i++) {
						FGCustomEvent _evt = _element.customEvent [i];
						if (_evt != null)
								listEvent [_evt] = false;
				}

				saveSpeed = internalSpeed;
		}

		Vector3 ComputeRoutePosition (float routeFactor)
		{
				float angleSin = (routeFactor - eventFactor) * loopSin * MathfEx.TWO_PI;
				float rightOffset = posLocal.x + heightSin * SinCosTable.Sin (angleSin);

				return route.GetPositionOnRoute (routeFactor) + MathfEx.GetRightVector (route.GetOrientationOnRoute (routeFactor)) * rightOffset;
		}

		void UpdateFireWork ()
		{
				_transform.position -= new Vector3 (dx, 0, -dz);
		}

		void UpdateFishAlone ()
		{
				_transform.position = route.GetPositionOnRoute (routeFactor);
				_transform.right = -MathfEx.GetForwardVector (route.GetOrientationOnRoute (routeFactor));
		
				if (_transform.right.x < 0)
						_transform.localEulerAngles = new Vector3 (180.0f, _transform.localEulerAngles.y, _transform.localEulerAngles.z);
		}

		void UpdateInGroup ()
		{
				if (route == null)
						return;

				if (state == FHFishState.Swim) {
						float _dist = Mathf.Abs (internalSpeed - desSpeedDelay);
						if (_dist > 0.02f) {// thay doi van toc tu tu
								if (_dist > 0.5) {
										if (internalSpeed > desSpeedDelay) {
												internalSpeed = desSpeedDelay + 0.3f;
										} else {
												internalSpeed = desSpeedDelay - 0.3f;
										}

								} else {
										internalSpeed += (desSpeedDelay - internalSpeed) * 0.03f;
								}
								if (Mathf.Abs (internalSpeed - desSpeedDelay) < 0.03f) {
										internalSpeed = desSpeedDelay;
								}
						}
						routeFactor += Time.deltaTime * internalSpeed;
						CheckEvent ();
						if (isStop) {
								return;
						}
						routeFactor = Mathf.Clamp01 (routeFactor);

						float _routeValue = Mathf.Clamp01 (posLocal.y / route.GetLength () + routeFactor);
						// calculate location for line ray

						_transform.position = ComputeRoutePosition (_routeValue);
						Vector3 direction = ComputeRoutePosition (_routeValue + 0.05f) - _transform.position;
						if (Vector3.SqrMagnitude (direction) > 0) {
								if (_transform.right.x < 0)
										_transform.localEulerAngles = new Vector3 (180.0f, _transform.localEulerAngles.y, _transform.localEulerAngles.z);
								Quaternion desiredOrient = Quaternion.LookRotation (direction) * Quaternion.AngleAxis (90f, Vector3.up);
								_transform.rotation = Quaternion.RotateTowards (_transform.rotation, desiredOrient, 45f);

								if (_transform.right.x < 0)
										_transform.localEulerAngles = new Vector3 (180.0f, _transform.localEulerAngles.y, _transform.localEulerAngles.z);
						}

						if (routeFactor >= 1f)
								Despawn ();
				}
		}

		bool CheckEvent ()
		{
				if (listEvent.Count > 0) {
						FGCustomEvent _evt = null;
						foreach (KeyValuePair<FGCustomEvent, bool> pair in listEvent) {
								if (pair.Key.distanceEvent < routeFactor) {
										_evt = pair.Key;
										ChangeEvent (_evt);
										break;
								}
						}
						if (_evt != null) {
								listEvent.Remove (_evt);
								//Debug.LogError("Check OK has Event");
								return true;
						}
				}
				return false;
		}

		public void ChangeEvent (FGCustomEvent _evt)
		{
				switch (_evt.fGKindEvent) {
				case FGKindEvent.VELOCITY:
						if (routeFactor < 0.1f) {
								internalSpeed = desSpeedDelay = saveSpeed * _evt.velocityChange;
						} else {
								desSpeedDelay = saveSpeed * _evt.velocityChange;
						}
						break;
				case FGKindEvent.START:
						isStop = false;
						break;
				case FGKindEvent.STOP:
						isStop = true;
						break;
				case FGKindEvent.LOSE:
						routeFactor = 1;
						break;
				case FGKindEvent.CURVE:
						if (heightSin < 0.1f || loopSin == 0) {
								heightSin = _evt.heightSin;
								loopSin = _evt.loopSin;
								eventFactor = routeFactor;
						}
						break;
				}
		}

		public float GetSpeed (FishSpeed fishSpeed)
		{
				switch (fishSpeed) {
				case FishSpeed.MINIMUM:
						return speed * 0.5f;
				case FishSpeed.NORMAL:
						return speed * 1.0f;
				case FishSpeed.MAXIMUM:
						return speed * 1.5f;
				default:
						return speed * 1.0f;
				}
		}
    #endregion
}