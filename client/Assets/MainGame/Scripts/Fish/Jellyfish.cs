using UnityEngine;
using System.Collections;

public class Jellyfish : Fish {


	public override void RandomPosition()
	{
		ChangeStatus ((int)FISH_STATUS.ST_NORMAL);
		int r = Random.Range (0,10);
		float x, y,angle=0;
		switch (r) {
		case 0:
		case 1:
			x=leftLimit;
			y=Random.Range(bottomLimit,topLimit-GetHeightSprite())+1;
			angle=Random.Range(10,50);
			break;
		case 2:
		case 3:
			x=rightLimit + GetWidthSprite()/2;
			y=Random.Range(bottomLimit,topLimit-GetHeightSprite()+1);
			angle=Random.Range(-50,-10);
			break;
			
		default:
			x=Random.Range(leftLimit, rightLimit);
			y=bottomLimit;
			angle=Random.Range(-50,50);
			break;
		}
		
		transform.localPosition=new Vector2(x,y);
		
		SetAngleObject (angle);
		Setdx_dy (angle);
		gameObject.SetActive (true);
	}

	public override void SetNextAngle(float a) {
		

	}




}
