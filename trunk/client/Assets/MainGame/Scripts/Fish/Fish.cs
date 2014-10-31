using UnityEngine;
using System.Collections;

public class Fish : _MyGameObject {

	protected int mId;
	protected int mStatus;
	protected Camera mViewLimit;
	private float leftLimit,rightLimit,topLimit,bottomLimit;
	private float dx,dy;
	private float curren_Angle = 0, next_Angle = 0, offsetAngle, detaAngle;
	protected int delayCurren;
	protected MyAnimation mAnimation;

	private string[] fr_move,fr_die;



	public void Init(int id)
	{
		mId = id;
		LoadFrame ();

		mAnimation = GetComponent<MyAnimation> ();
		ChangeStatus ((int)FISH_STATUS.ST_NORMAL);
		
		RandomPosition ();

		SetSpeed (0.01f);

		name = "f_" + mId;
	}

	protected void LoadFrame()
	{
		fr_move = new string[]{"f2000"+mId+"_m_01","f2000"+mId+"_m_02","f2000"+mId+"_m_03","f2000"+mId+"_m_04","f2000"+mId+"_m_05","f2000"+mId+"_m_06","f2000"+mId+"_m_07","f2000"+mId+"_m_08"};
		fr_die = new string[]{"f2000"+mId+"_d_01","f2000"+mId+"_d_01","f2000"+mId+"_d_01","f2000"+mId+"_d_02","f2000"+mId+"_d_02","f2000"+mId+"_d_02","f2000"+mId+"_d_03","f2000"+mId+"_d_03","f2000"+mId+"_d_03"};
	}

	public void SetCamera(Camera c)
	{
		mViewLimit = c;

//		leftLimit = mViewLimit.ScreenToWorldPoint (new Vector2(0,0)).x;
//		rightLimit = mViewLimit.ScreenToWorldPoint (new Vector2(Screen.width,0)).x;
//		bottomLimit = mViewLimit.ScreenToWorldPoint (new Vector2(0,0)).y;
//		topLimit = mViewLimit.ScreenToWorldPoint (new Vector2(0,Screen.height)).y;

		leftLimit = -Screen.width / 2;
		rightLimit = Screen.width / 2;
		topLimit = Screen.height / 2;
		bottomLimit = -Screen.height / 2;


	}

//	public Fish(Camera c)
//	{
//		mViewLimit = c;
//		Start ();
//	
//	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();	

		switch (mStatus) {
		case (int)FISH_STATUS.ST_NORMAL:
			if (curren_Angle != next_Angle) {
				updateAngle();
			} else {

				StartCoroutine (test ());
			}


			Move (dx,dy);
			if (!CheckLimit ()) {
				RandomPosition();		
			}



			break;
		case (int)FISH_STATUS.ST_DIE:
			if(mAnimation.EndFrame())
			{
				gameObject.SetActive(false);
				RandomPosition();
			}
			break;
		}
	}

	protected void NewAngle() {
		Debug.Log ("NewAngle");
//		if (haveDes)
//			return;
		SetAngleObject(Random.Range(curren_Angle - 90,curren_Angle + 90));
	}

	protected void updateAngle() {
		detaAngle -= Mathf.Abs(offsetAngle);
		if (delayCurren < 0) {
			if (offsetAngle > 0) {
				offsetAngle += detaAngle;
			} else {
				offsetAngle -= detaAngle;
				
			}
		}
		curren_Angle += offsetAngle;
		
		if ((offsetAngle > 0 && curren_Angle > next_Angle) || (offsetAngle < 0 && curren_Angle < next_Angle))
			curren_Angle = next_Angle;
		
		SetAngleObject( curren_Angle);
		// if (angle >= -90 && angle < 90) {
		// tranform = SpriteBatcher.TRANFORM_NONE;
		// } else {
		// tranform = SpriteBatcher.TRANFORM_90;
		// }
		SetSpeed(curren_Angle);
	}


	public void ChangeStatus(int newStatus)
	{
		mStatus = newStatus;
		switch (mStatus) {
		case (int) FISH_STATUS.ST_NORMAL:
			mAnimation.SetFrame(fr_move);

			break;
		case (int)FISH_STATUS.ST_DIE:

			mAnimation.SetFrame(fr_die);

			break;
		}
	}

	public int GetStatus()
	{
		return mStatus;
	}
	public bool CheckLimit()
	{
				if (transform.localPosition.x < leftLimit-GetWidthSprite())
								return false;
		else if (transform.localPosition.x >= rightLimit)
								return false;
		if (transform.localPosition.y < bottomLimit)
								return false;
						else if (transform.localPosition.y > topLimit)
								return false;
		return true;
	}

	private void RandomPosition()
	{

		ChangeStatus ((int)FISH_STATUS.ST_NORMAL);

//		Debug.Log ("================ RandomPosition");
		int r = Random.Range (0,10);
		float x, y,angle=0;
		switch (r) {
		case 0:
		case 1:
		case 2:
		case 3:
			x=leftLimit;
			y=Random.Range(bottomLimit,topLimit-GetHeightSprite())+1;
			angle=Random.Range(-50,50);
			break;

		default:
			x=rightLimit;
			y=Random.Range(bottomLimit,topLimit-GetHeightSprite()+1);
			angle=Random.Range(140,220);
			break;
		}

//		SetPosition (x,y);

		transform.localPosition=new Vector2(x,y);

		SetAngleObject (angle);
		Setdx_dy (angle);
		gameObject.SetActive (true);
	}

	private void Setdx_dy(float angle)
	{

		float radi = Mathf.Deg2Rad * angle;
		dx = Mathf.Cos (radi)*GetSpeed();
		dy=Mathf.Sin(radi)*GetSpeed();

//		Debug.Log ("dx== "+dx+", dy="+dy+", radi: "+ radi+",angle: "+angle);
	}

	IEnumerator test()
	{
		yield return new WaitForSeconds (1f);
		NewAngle ();
	}

 	protected void UpdateAngle()
	{
			detaAngle -= Mathf.Abs(offsetAngle);
			if (delayCurren < 0) {
				if (offsetAngle > 0) {
					offsetAngle += detaAngle;
				} else {
					offsetAngle -= detaAngle;
					
				}
			}
			curren_Angle += offsetAngle;
			
			if ((offsetAngle > 0 && curren_Angle > next_Angle) || (offsetAngle < 0 && curren_Angle < next_Angle))
				curren_Angle = next_Angle;
			
			SetAngleObject(curren_Angle);

			Setdx_dy(curren_Angle);
	}

}

enum FISH_STATUS{
	ST_NORMAL=1,
	ST_DIE=2,
	ST_CATCH=3,
	ST_STUN=4,
	ST_FINISH=5
};
