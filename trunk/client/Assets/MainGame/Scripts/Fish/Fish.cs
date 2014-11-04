using UnityEngine;
using System.Collections;

public class Fish : _MyGameObject {

	protected int mId;
	protected int mStatus;
	protected Camera mViewLimit;
	private float leftLimit,rightLimit,topLimit,bottomLimit;
	private float dx,dy;
	private float curren_Angle = 0, next_Angle = 0, offsetAngle, detaAngle;
	protected MyAnimation mAnimation;

	private string[] fr_move,fr_die;



	public void Init(int id)
	{
		mId = id;
		LoadFrame ();

		mAnimation = GetComponent<MyAnimation> ();
		ChangeStatus ((int)FISH_STATUS.ST_NORMAL);
		
		RandomPosition ();

		SetSpeed (Random.Range(0.005f,0.01f));

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

		leftLimit = -Screen.width / 2;
		rightLimit = Screen.width / 2;
		topLimit = Screen.height / 2;
		bottomLimit = -Screen.height / 2;


	}


	
	// Update is called once per frame
	void Update () {
		base.Update ();	

		switch (mStatus) {
		case (int)FISH_STATUS.ST_NORMAL:
			if (curren_Angle != next_Angle) {
				UpdateAngle();
			} else {
				StartCoroutine (WaitingNextAngle ());
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
		else if (transform.localPosition.x >= rightLimit + GetWidthSprite())
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
			x=rightLimit + GetWidthSprite()/2;
			y=Random.Range(bottomLimit,topLimit-GetHeightSprite()+1);
			angle=Random.Range(140,220);
			break;
		}

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
	}

	IEnumerator WaitingNextAngle()
	{
		yield return new WaitForSeconds (1f);
		SetNextAngle (Random.Range(curren_Angle - 90, curren_Angle + 90));
	}

 	private void UpdateAngle()
	{
		detaAngle -= Mathf.Abs(offsetAngle);

		curren_Angle += offsetAngle;
		if ((offsetAngle > 0 && curren_Angle > next_Angle) || (offsetAngle < 0 && curren_Angle < next_Angle))
			curren_Angle = next_Angle;
			
		SetAngleObject(curren_Angle);

		Setdx_dy(curren_Angle);
	}

	private void SetNextAngle(float a) {

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

enum FISH_STATUS{
	ST_NORMAL=1,
	ST_DIE=2,
	ST_CATCH=3,
	ST_STUN=4,
	ST_FINISH=5
};
