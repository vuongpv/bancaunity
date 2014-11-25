using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : _MyGameObject
{
		private int mId;
		private float xDesc, yDesc;
		private float[] zoom = new float[] {
				0.3f,
				0.5f,
				0.5f,
				0.8f,
				1f,
				1.2f,
				1.2f,
				1f,
				1f
		};
		private float mAngle;
		protected MyAnimation mAnimation;
		private int mStatus;
		private string[] fr_move, fr_Net;
		[HideInInspector]
		public static List<Gold>
				golds;

		public void Init (int id)
		{
				SetSpeed (0.1f);
				this.mId = id;
				LoadFrame (id);
				mAnimation = GetComponent<MyAnimation> ();
				name = "b_" + id;
				transform.localScale = Vector3.one;
				ChangeStatus ((int)BULLET_STATUS.MOVE);
				transform.localScale = Vector3.one;

				golds = new List<Gold> ();
				LoadGolds ();

		}

		public void LoadFrame (int id)
		{
				fr_move = new string[]{"b" + id,"b" + id,"b" + id};
				fr_Net = new string[] {
						"net0" + id,
						"net0" + id,
						"net0" + id,
						"net0" + id,
						"net0" + id,
						"net0" + id,
						"net0" + id,
						"net0" + id,
						"net0" + id
				};

		}

		public int  GetId ()
		{
				return mId;
		}

		public void SetId (int id)
		{
				mId = id;
		}


	
		// Update is called once per frame
		void Update ()
		{
				base.Update ();
				switch (mStatus) {
				case (int)BULLET_STATUS.MOVE:
						break;
				case (int)BULLET_STATUS.AT_DESC:
						transform.localScale = new Vector3 (zoom [mAnimation.GetIndexFrame ()], zoom [mAnimation.GetIndexFrame ()], zoom [mAnimation.GetIndexFrame ()]);
						if (mAnimation.EndFrame ()) {
								ChangeStatus ((int)BULLET_STATUS.MOVE);
								transform.localScale = Vector3.one;
								gameObject.SetActive (false);
						}
						break;
				}
		}

		public MyAnimation getanimation ()
		{
				return mAnimation;
		}

		public void setDec (float xD, float yD)
		{
				xDesc = xD;
				yDesc = yD;

				MoveTo (xDesc, yDesc);
		}

		public void ChangeStatus (int newStatus)
		{
				mStatus = newStatus;
				BoxCollider box = gameObject.GetComponent<BoxCollider> ();
				switch (mStatus) {
				case (int) BULLET_STATUS.MOVE:
						mAnimation.SetFrame (fr_move);
						box.size = Vector2.one;
						break;
				case (int) BULLET_STATUS.AT_DESC:
						mAnimation.SetFrame (fr_Net);
						box.size = new Vector2 (GetWidthSprite (), GetHeightSprite ());
						break;
				}
		}

		public override  void AtTheTargetPosition ()
		{
				ChangeStatus ((int)BULLET_STATUS.AT_DESC);
		}

		void OnCollisionEnter (Collision collision)
		{
				Debug.LogError ("OnCollisionEnter");
		}

		void OnTriggerEnter (Collider other)
		{

				if (mStatus != (int)BULLET_STATUS.AT_DESC) {
						return;
				}

				Fish f = other.gameObject.GetComponent<Fish> ();
				if (null != f) {
						if (f.GetStatus () == (int)FISH_STATUS.ST_DIE)
								return;
						f.ChangeStatus ((int)FISH_STATUS.ST_DIE);
						CreateGold (f.GetPrice (), f.transform.localPosition);
				}

		}

		public static void CreateGold (float price, Vector2 position)
		{
				for (int i=0; i<golds.Count; i++) {
						Gold g = golds [i];
						if (g.gameObject.activeSelf)
								continue;
						g.gameObject.SetActive (true);
						g.transform.localPosition = position;
						g.transform.localScale = Vector3.one;
						g.SetPrice (price);
						g.Action (true);
						return;
				}
		}

		private void LoadGolds ()
		{
				golds = new List<Gold> ();
				for (int i=0; i<20; i++) {
						Gold g = (Instantiate (Resources.Load ("Prefabs/Button/Gold")) as GameObject).GetComponent<Gold> ();
						g.transform.parent = GameObject.Find ("Golds").transform;
						g.Init ();
						g.gameObject.SetActive (false);
						golds.Add (g);
				}
		}
}

enum BULLET_STATUS
{
		MOVE=0,
		AT_DESC=1,
};
