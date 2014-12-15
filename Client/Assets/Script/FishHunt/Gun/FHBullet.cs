using UnityEngine;
using System.Collections;
using GFramework;

public class FHBullet : MonoBehaviour
{
		// Size
		public float size;

		// Speed
		public float speed;

		// Bet
		public int betMultiplier;

		// Previous time
		private float prevTime;
		public float elapsedTime;
		public Vector3 _prevPos { get; private set; }

		// Current gun
		private FHGun gun;

		// Cache
		public Transform _transform { get; private set; }
		private Renderer _renderer;

		public GameObject particle = null;

		public void Setup (FHGun gun, float speed, float aoe, int betMultiplier)
		{
				_transform.position = gun.barrelAnchor.position;
				_transform.forward = gun.barrelAnchor.forward;
				this.speed = speed;
				this.betMultiplier = betMultiplier;
				this.gun = gun;

				if (particle != null)
						Destroy (particle);
		}

		public Vector3 headPosition {
				get {
						return _transform.position + (transform.forward * size);
				}
		}

		void Awake ()
		{
				_transform = transform;
				_renderer = GetComponentInChildren<Renderer> ();
		}

		void OnSpawned ()
		{
				prevTime = Time.realtimeSinceStartup;
				elapsedTime = 0.0f;
		}

		void Update ()
		{
				elapsedTime += Time.deltaTime;

				float curTime = Time.realtimeSinceStartup;

				_prevPos = _transform.position;

				// Calc position_transform.position
				_transform.position += _transform.forward * speed * (curTime - prevTime);

				prevTime = curTime;

				if (elapsedTime < FHGameConstant.BULLET_IMPACT_CHECKING_TIME_CYCLE)
						return;

				this.gun.CheckBulletImpact (this);

				CheckVisible ();
		}

		public void CheckVisible ()
		{
				if (!IsVisible ())
						this.gun.CollectBullet (_transform);
		}

		void OnDrawGizmos ()
		{
				Gizmos.color = Color.red;
				Vector3 vSize = (transform.forward * size);
				Gizmos.DrawLine (transform.position - vSize, transform.position + vSize);
		}

		bool IsVisible ()
		{
				return (_transform.position.x >= FHSystem.instance.boundLeft
						&& _transform.position.x <= FHSystem.instance.boundRight
						&& _transform.position.z <= FHSystem.instance.boundTop
						&& _transform.position.z >= FHSystem.instance.boundBottom);
		}
}

