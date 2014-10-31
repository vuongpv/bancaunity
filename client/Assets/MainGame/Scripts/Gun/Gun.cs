using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : _MyGameObject {
	public Camera mView;

	private readonly int MAX_GUN = 12;

	private int mId = 1;
	private int mStatus;
	private string[] fr_normal, fr_shot;
	private MyAnimation mAnimation;
	private BoxCollider mBox;


	public GameObject leftObject,rightObject;
	private BoxCollider leftBox,rightBox;

	private List<Bullet> bullets;


	// Use this for initialization
	void Start () {
		mBox=gameObject.GetComponent<BoxCollider>();
		leftBox=leftObject.GetComponent<BoxCollider>();
		rightBox=rightObject.GetComponent<BoxCollider>();


		mId = 12;
		ReLoad ();

		BoxCollider box = gameObject.GetComponent<BoxCollider> ();
		transform.localPosition = new Vector2 (0,mView.transform.position.y-mView.pixelHeight/2 +box.size.y/2);

		LoadBullets ();
	}

	private void LoadBullets()
	{
		bullets = new List<Bullet> ();
		for (int i=0; i<20; i++) {
			Bullet b = (Instantiate (Resources.Load ("Prefabs/Bullet")) as GameObject).GetComponent<Bullet>();
			b.transform.parent = GameObject.Find("Bullets").transform;
			b.Init (mId);
			b.gameObject.SetActive(false);
			bullets.Add(b);

		}
	}

	private void ReLoad()
	{
		Debug.Log ("======reload   "+mId);

		LoadFrame (mId);
		transform.transform.localScale = Vector3.one;
		mAnimation=GetComponent<MyAnimation>();
		
		ChangeStatus ((int)GUN_STATUS.ST_NORMAL);
		mAnimation.loop = true;
		SetSpeed (0);
	}

	protected void LoadFrame(int id)
	{
		fr_normal = new string[]{"g"+id+"_01","g"+id+"_01","g"+id+"_01"};
		fr_shot = new string[]{"g"+id+"_02","g"+id+"_02","g"+id+"_02","g"+id+"_03","g"+id+"_03","g"+id+"_03"};

	}

	// Update is called once per frame
	void Update () {
		base.Update ();
		switch (mStatus) {
		case (int)GUN_STATUS.ST_NORMAL:

			break;
		case (int)GUN_STATUS.ST_SHOT:
		
			if(mAnimation.EndFrame())
				ChangeStatus((int)GUN_STATUS.ST_NORMAL);
			break;
		}
	
	}

	public void ChangeStatus(int newStatus)
	{
		mStatus = newStatus;
		switch (mStatus) {
		case  (int)GUN_STATUS.ST_NORMAL:
			mAnimation.SetFrame(fr_normal);

			break;
		case (int)GUN_STATUS.ST_SHOT:
			mAnimation.SetFrame(fr_shot);
			break;
		}
	}
	void OnTap(TapGesture gesture) { 
		Vector3 target = Util_Funtion.convertPositionToCamera (gesture.Position,mView);

		if (mBox.bounds.Contains(target))
						return;

		if (leftBox.bounds.Contains (target)) {
			if(mId>1)
				mId-=1;
			else
				mId=12;
			ReLoad();
			return;
		}
		else
		if (rightBox.bounds.Contains (target)) {
			if(mId<MAX_GUN-1)
				mId+=1;
			else
				mId=1;
			ReLoad();
			return;
		}

		ChangeStatus ((int)GUN_STATUS.ST_SHOT);
		float deg=(((float) Mathf.Atan2(target.x - (GetX() ), (GetY() ) - target.y)) * Mathf.Rad2Deg) + 180f;

		SetAngleObject (deg);

		CreateBullet (deg,target.x,target.y);

	 }


	public void CreateBullet(float deg,float xTouch,float yTouch)
	{
		for (int i=0; i<bullets.Count; i++) {
			Bullet b = bullets [i];
			if (!b.gameObject.activeSelf)
			{
				if (b.GetId () != mId) {
					b.SetId (mId);
					b.LoadFrame (mId);
				}
				b.transform.localScale = Vector3.one;
				b.ChangeStatus ((int)BULLET_STATUS.MOVE);
				b.transform.localPosition = transform.localPosition;
				b.SetAngleObject (transform.rotation);
				b.setDec (xTouch, yTouch);
				b.gameObject.SetActive (true);
				return;
			}
		}


		Debug.Log ("new bullet");


		Bullet b1 = (Instantiate (Resources.Load ("Prefabs/Bullet")) as GameObject).GetComponent<Bullet>();
		b1.transform.parent = GameObject.Find("Bullets").transform;
		b1.Init (mId);
		b1.transform.localPosition = transform.localPosition;
		b1.SetAngleObject (transform.rotation);
		b1.transform.localScale = Vector3.one;
		b1.setDec (xTouch,yTouch);
		bullets.Add(b1);
	}
}

enum GUN_STATUS{
	ST_NORMAL = 1, ST_SHOT = 2, ST_CHANGE = 3, ST_NEW = 4
};
