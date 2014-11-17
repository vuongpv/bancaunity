using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FishForum : Fish {

	List<FishOne> fishs;
	int size;
	// Update is called once per frame
	public override void Init(int id)
	{
		Debug.Log ("init fish forum");

		mId=id;
		base.Init (id);
		this.size = Random.Range(3,5);
		fishs = new List<FishOne> ();
		for (int i=0; i<size; i++) {
			FishOne f=(Instantiate (Resources.Load (Constant.pathPrefabs+"Fishs/FishOne")) as GameObject).GetComponent<FishOne>();
			f.SetCamera(mViewLimit);
			f.transform.localScale=Vector3.one;
			f.transform.parent =GameObject.Find("Fishs").transform;
			f.Init(id);		
			f.name="fish_forum:"+i;

			fishs.Add(f);
			f.SetSpeed(GetSpeed());
			f.SetAngleObject(curren_Angle);
		}

		RandomPosition ();


	}



	public override bool CheckLimit()
	{
		if(null==fishs)
			return base.CheckLimit ();	
		for (int i=0; i<fishs.Count; i++) {
			if(fishs[i]==null ||!fishs[i].gameObject.activeSelf)
				continue;
		if(fishs[i].CheckLimit())
				return true;
		}

		return base.CheckLimit ();
	}

	public override void RandomPosition()
	{
		base.RandomPosition ();

		if (null == fishs)
			return;

		for (int i = 0; i < fishs.Count; i++) {
			FishOne f = fishs[i];
			if (null == f)
				continue;
		
			f.transform.localPosition=transform.localPosition;
		}
	}

	public override void SetNextAngle(float a) {
		base.SetNextAngle (a);
		for (int i=0; i<fishs.Count; i++) {
			fishs[i].SetNextAngle(a);		
		}
	}
}
