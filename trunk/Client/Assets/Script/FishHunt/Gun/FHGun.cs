using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GFramework;

public enum GunState
{
		Idle = 0, 
		Firing,
}

public class FHGun : MonoBehaviour
{
		public FHPlayerController controller;

		public Transform barrelAnchor;
		public SpriteBase barrelSprite;

		public float angleRotate = 0;
		// State
		public GunState state;

		public ConfigGunRecord configGun { get; private set; }

		private float lastTimeFiring;

		public GameObject bulletPrefab;
		public GameObject impactEffectPrefab;

		protected SpawnPool bulletPool;
		protected SpawnPool effectPool;
		protected SpawnPool aoePool;

		// Cache
		public Transform _transform { get; private set; }
		public SoundableObject _soundable { get; private set; }

		public int id { get { return configGun.id; } }

		protected FHGunManager manager;

		Transform gunAnchorTrans;
		Vector3 currentAngle;

		public void SetManager (FHGunManager _manager)
		{
				manager = _manager;
		}
	
		public virtual void Setup (FHPlayerController controller, ConfigGunRecord configGun)
		{
				_transform = transform;
				gunAnchorTrans = controller.gunAnchor.transform;

				_soundable = GetComponent<SoundableObject> ();

				bulletPool = PoolManager.Pools ["bullets"];
				effectPool = PoolManager.Pools ["effects"];
				aoePool = PoolManager.Pools ["aoe"];

				lastTimeFiring = -configGun.cooldown;

				this.controller = controller;
				this.configGun = configGun;

				bulletPrefab = (GameObject)Resources.Load ("Prefabs/Gun/" + configGun.bulletName, typeof(GameObject));
				impactEffectPrefab = (GameObject)Resources.Load ("Prefabs/Effect/" + configGun.impactEffectName, typeof(GameObject));
		}

		public virtual void SetTarget (Vector3 target)
		{
				target.y = gunAnchorTrans.position.y;

				gunAnchorTrans.forward = (target - gunAnchorTrans.position).normalized;

				currentAngle = gunAnchorTrans.localEulerAngles;
				if (Vector3.Dot (gunAnchorTrans.forward, gunAnchorTrans.parent.right) >= 0) {
						if (currentAngle.y >= 100.0f)
								currentAngle.y = 100.0f;
				} else {
						if (currentAngle.y <= 260.0f)
								currentAngle.y = 260.0f;
				}
        
				gunAnchorTrans.localEulerAngles = currentAngle;

				angleRotate = gunAnchorTrans.localRotation.eulerAngles.y;
				Quaternion quatenion = Quaternion.identity;
				quatenion.SetAxisAngle (new Vector3 (0, 1, 0), (angleRotate) * Mathf.PI / 180.0f);
				gunAnchorTrans.localRotation = quatenion;
		}

		public float GetAngleRotateFromTarget (Vector3 target)
		{
				Quaternion old = controller.gunAnchor.transform.localRotation;
				float _angle;
        
				target.y = controller.gunAnchor.transform.position.y;

				controller.gunAnchor.transform.forward = (target - controller.gunAnchor.transform.position).normalized;
				_angle = controller.gunAnchor.transform.localRotation.eulerAngles.y;
				Quaternion quatenion = Quaternion.identity;
				quatenion.SetAxisAngle (new Vector3 (0, 1, 0), (_angle) * Mathf.PI / 180.0f);
				controller.gunAnchor.transform.localRotation = old;
				return _angle;
		}
    
		public void PullTrigger ()
		{
				SetState (GunState.Firing);
		}

		public void ReleaseTrigger ()
		{
				SetState (GunState.Idle);
		}

		void Update ()
		{
				if (state == GunState.Firing) {
						FireBullet ();
				}
		}
	
		public void FireBullet ()
		{
				if (Time.realtimeSinceStartup - lastTimeFiring > configGun.cooldown) {
						OnFireBullet ();
						lastTimeFiring = Time.realtimeSinceStartup;
				}
		}

		protected virtual void OnFireBullet ()
		{
				// Store current bet multiplier
				int betMultiplier = controller.GetCurrentBetMultiplier ();

				if (controller.gold < betMultiplier * configGun.bulletPrice)
						return;

				// Spawn bullet
				FHBullet bullet = bulletPool.Spawn (bulletPrefab.transform).GetComponent<FHBullet> ();
				bullet.Setup (this, configGun.bulletSpeed, configGun.bulletAoe, betMultiplier);

				// Play fire anim & sound
				if (configGun.id < 4)
						FHAudioManager.instance.PlaySound (FHAudioManager.SOUND_FIRE01);
				else
        if (configGun.id < 8)
						FHAudioManager.instance.PlaySound (FHAudioManager.SOUND_FIRE02);
				else
						FHAudioManager.instance.PlaySound (FHAudioManager.SOUND_FIRE03);
				barrelSprite.PlayAnim (0);

				// Update coin, xp, quest
				controller.OnFireBullet (configGun.id, betMultiplier, configGun.bulletPrice);
		}
   

		public void CollectBullet (Transform bullet)
		{
				if (bullet.gameObject.active)
						bulletPool.Despawn (bullet);
		}

		public void SetState (GunState state)
		{

				if (this.state == state)
						return;

				switch (state) {
				case GunState.Firing:
						
						break;
				}

				this.state = state;
		}

		public bool CheckBulletImpact (FHBullet bullet)
		{
				FHFish impactTarget = null;

				Vector3 impactPosition = Vector3.zero;

				impactTarget = GetBulletHitTarget (bullet._prevPos, bullet.headPosition, out impactPosition);
				if (impactTarget == null)
						return false;

				// Play hit sound
				if (this is FHGunNuke)
						FHAudioManager.instance.PlaySound (FHAudioManager.SOUND_HIT_NUKE);
				else
						FHAudioManager.instance.PlaySound (FHAudioManager.SOUND_HIT01);

				// Store impact info
				int impactBetMultiplier = bullet.betMultiplier;

				// Collect bullet
				CollectBullet (bullet._transform);

				// Spawn effect
				SpawnImpactEffects (impactPosition);

				// Explode
				List<FHFish> fishHits = CheckExplodeHitTargets (impactPosition, impactTarget);
				controller.CalculateHitRate (this, fishHits, impactBetMultiplier);

				return true;
		}

		public virtual FHFish GetBulletHitTarget (Vector3 bulletPosStart, Vector3 bulletPosEnd, out Vector3 hitPt)
		{
				HashSet<FHFish> activeFishes = FHFishManager.instance.GetActiveFishes ();
				foreach (FHFish fish in activeFishes) {
						if (fish.state == FHFishState.Dead || fish.state == FHFishState.Dying)
								continue;

						if (fish.fastCollider.IsLineSegmentIntersect (bulletPosStart, bulletPosEnd, out hitPt))
								return fish;
				}

				hitPt = Vector3.zero;
				return null;
		}

		protected virtual List<FHFish> CheckExplodeHitTargets (Vector3 impactPosition, FHFish impactTarget)
		{
				HashSet<FHFish> activeFishes = FHFishManager.instance.GetActiveFishes ();
				List<FHFish> hits = new List<FHFish> ();

				// Impact first
				impactTarget.OnBulletHit ();
				hits.Add (impactTarget);

				// Add others
				if (hits.Count < configGun.maxHits) {
						foreach (var fish in activeFishes) {
								if (fish.state == FHFishState.Dead || fish.state == FHFishState.Dying)
										continue;
                
								if (fish.fastCollider.IsSphereOverlapped (impactPosition, configGun.bulletAoe)) {
										fish.OnBulletHit ();
										hits.Add (fish);
								}

								if (hits.Count >= configGun.maxHits)
										break;
						}
				}

				return hits;
		}

		protected virtual void SpawnImpactEffects (Vector3 impactPosition)
		{
				Transform effect = effectPool.Spawn (impactEffectPrefab.transform);
				effect.position = impactPosition;

				FHSimpleImpact impact = effect.gameObject.GetComponent<FHSimpleImpact> ();
				impact.Setup (this);
		}

		public void DespawnImpactEffect (Transform effect)
		{
				effectPool.Despawn (effect);
		}

		public void DespawnAOEEffect (Transform effect)
		{
				aoePool.Despawn (effect);
		}

    #region Control Delay & smooth for lan and online network
		private int idSmooth = 0;
		public void SetTargetAndShotDelay (float _angleRotate, bool isShot)
		{
				idSmooth++;
				if (angleRotate > 180) {
						angleRotate = -360 + angleRotate;
				}
				if (_angleRotate > 180) {
						_angleRotate = -360 + _angleRotate;
				}
				StartCoroutine (SmoothRotateAndShot (angleRotate, _angleRotate, idSmooth, 0, isShot));
		}
		private IEnumerator SmoothRotateAndShot (float _oldAngle, float _angleRotate, int _idSmooth, float _percent, bool isShot)
		{
				yield return new WaitForSeconds (0.03f);//30fps
				if (idSmooth != _idSmooth) {
						if (isShot == true) {
								ProcessLanFireBullet (_angleRotate);
						}
						yield break;
				}
				if (_percent < 0.6) {
						_percent += 0.25f;
				} else if (_percent < 0.8) {
						_percent += 0.2f;
				} else {
						_percent += 0.1f;
				}
				angleRotate = _oldAngle + (_angleRotate - _oldAngle) * _percent;
				Quaternion quatenion = Quaternion.identity;
				quatenion.SetAxisAngle (new Vector3 (0, 1, 0), (angleRotate) * Mathf.PI / 180.0f);
				controller.gunAnchor.transform.localRotation = quatenion;
				if (_percent > 0.99) {
						if (isShot == true) {
								ProcessLanFireBullet (_angleRotate);
						}
						angleRotate = _angleRotate;
						quatenion = Quaternion.identity;
						quatenion.SetAxisAngle (new Vector3 (0, 1, 0), (angleRotate) * Mathf.PI / 180.0f);
						controller.gunAnchor.transform.localRotation = quatenion;
						yield break;
				} else {
						StartCoroutine (SmoothRotateAndShot (_oldAngle, _angleRotate, _idSmooth, _percent, isShot));
				}
		}
		private void ProcessLanFireBullet (float _Angle)
		{
				float saveAngle = angleRotate;
				angleRotate = _Angle;
				Quaternion quatenion = Quaternion.identity;
				quatenion.SetAxisAngle (new Vector3 (0, 1, 0), (angleRotate) * Mathf.PI / 180.0f);
				controller.gunAnchor.transform.localRotation = quatenion;
				OnFireBullet ();
				angleRotate = saveAngle;
				quatenion = Quaternion.identity;
				quatenion.SetAxisAngle (new Vector3 (0, 1, 0), (angleRotate) * Mathf.PI / 180.0f);
				controller.gunAnchor.transform.localRotation = quatenion;

		}
    #endregion
}