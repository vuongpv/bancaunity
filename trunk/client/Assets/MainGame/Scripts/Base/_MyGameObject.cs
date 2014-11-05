using UnityEngine;
using System.Collections;

public class _MyGameObject : MonoBehaviour {
	private	float speed=0.0f;
	private float xTarget,yTarget;
	private bool isTarget;
	private int signSin, signCos;
	

	public void SetPosition(float xx,float yy)
	{
		SetX (xx);
		SetY (yy);
	}
	
	public void Update()
	{
		if (isTarget) {
			if (MoveToXY()) {
				AtTheTargetPosition();
			} else {
				NotAtTheTargetPosition();
			}
		}
	}
	
	public float GetWidthSprite()
	{
		return gameObject.GetComponent<UISprite> ().width;
	}
	
	public float GetHeightSprite()
	{
		return gameObject.GetComponent<UISprite> ().height;
	}
	
	public float GetWidthCollider()
	{
		return gameObject.renderer.bounds.size.x;
	}
	
	public float GetHeightCollider()
	{
		return gameObject.renderer.bounds.size.y;
	}
	
	public Bounds GetBoundSprite()
	{
		return gameObject.renderer.bounds;
	}
	
	public Bounds GetBoundCollider()
	{
		return collider.bounds;
	}
	
	public float GetX()
	{
//		if(transform.parent!=null)
//			return transform.localPosition.x;
		return transform.position.x;
	}
	
	public float GetY()
	{
//		if(transform.parent!=null)
//			return transform.localPosition.y;
		return transform.position.y;
	}
	
	public void SetX(float xx)
	{
//		if(transform.parent==null)
			transform.position = new Vector2 (xx,GetY());
//		else
//			transform.localPosition = new Vector2 (xx,GetY());
	}
	
	public void SetY(float yy)
	{
//		if(transform.parent==null)
			transform.position = new Vector2 (GetX(),yy);
//		else
//			transform.localPosition=new Vector2 (GetX(),yy);
		
	}
	
	public float GetSpeed()
	{
		return speed;
	}

	public void SetSpeed(float sp)
	{
		speed = sp;
	}
	

	
	public void Move(float dx, float dy) {
//		SetPosition (GetX () + dx, GetY () + dy);
		SetX (GetX() + dx);
		SetY (GetY()+dy);


	}
	
	public  bool MoveToX(int desX) {
		bool b = (GetX() > desX);
		if (b) {
			SetX(GetX() -speed);
		} else {
			SetX(GetX() + speed);
		}
		bool a = (GetX() > desX);
		if (b != a) {
			SetX(desX);
		}
		if (GetX() == desX) {
			return true;
		}
		return false;
	}
	
	public bool MoveToY(int desY) {
		bool b = (GetY() > desY);
		if (b) {
			SetY(GetY() -speed);
		} else {
			SetY(GetY() + speed);
		}
		bool a = (GetY() > desY);
		if (b != a) {
			SetY(desY) ;
		}
		if (GetY() == desY) {
			return true;
		}
		return false;
	}
	
	public bool MoveToXY() {
		
		float dx = GetX() - xTarget;
		float dy = GetY() - yTarget;
		
		float delta =  Mathf.Sqrt(dx * dx + dy * dy);
		
		if (delta > 0) {
			
			float sin = (dy ) / delta;
			float cos = (dx ) / delta;
			
			if (signSin * sin >= 0 && signCos * cos >= 0) {
				
				Move(-(speed * cos) , -(speed * sin) );
				
			} else {
				//
				isTarget = false;
				SetPosition(xTarget, yTarget);
				return true;
			}
			
		} else {
			//
			isTarget = false;
			SetPosition(xTarget, yTarget);
			return true;
		}
		
		return false;
	}
	
	public void MoveTo(float xTarget, float yTarget) {
		
		isTarget = true;
		this.xTarget = xTarget;
		this.yTarget = yTarget;
		
		float dx = GetX() - xTarget;
		float dy = GetY() - yTarget;
		
		float delta =  Mathf.Sqrt(dx * dx + dy * dy);
		if (delta <= 0) {
			return;
		}
		
		float sin = (dy ) / delta;
		float cos = (dx) / delta;
		if (sin >= 0) {
			signSin = 1;
		} else {
			signSin = -1;
		}
		
		if (cos >= 0) {
			signCos = 1;
		} else {
			signCos = -1;
		}
		
	}

	public void SetAngleObject(float fAngle)
	{
		transform.rotation =Quaternion.Euler(0.0f, 0.0f, fAngle); 
	}

	public void SetAngleObject(Quaternion rotation)
	{
		transform.rotation = rotation; 
	}
	
	
	
	public virtual void AtTheTargetPosition() {
		Debug.Log ("GameObject AtTheTargetPosition");
	}
	
	protected virtual void NotAtTheTargetPosition() {
		
	}

}
