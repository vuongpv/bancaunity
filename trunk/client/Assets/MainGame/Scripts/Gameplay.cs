using UnityEngine;
using System.Collections;

public class Gameplay : MonoBehaviour {
	public	Camera mView;

	// Use this for initialization
	void Start () {
		LoadFish ();
	}

	private void LoadFish()
	{
		for (int i=0; i<5; i++) {
			Fish f = (Instantiate (Resources.Load ("Prefabs/Fish")) as GameObject).GetComponent<Fish>();
			f.SetCamera( mView);
			f.transform.parent = transform.FindChild ("Fishs").transform;
			f.Init(1);
			
			
		}
		
		for (int i=0; i<3; i++) {
			Fish f = (Instantiate (Resources.Load ("Prefabs/Fish")) as GameObject).GetComponent<Fish>();
			f.SetCamera( mView);
			f.transform.parent = transform.FindChild ("Fishs").transform;
			f.Init(2);
			
		}
		
		for (int i=0; i<3; i++) {
			Fish f = (Instantiate (Resources.Load ("Prefabs/Fish")) as GameObject).GetComponent<Fish>();
			f.SetCamera( mView);
			f.transform.parent = transform.FindChild ("Fishs").transform;
			f.Init(3);
			
		}
		
		
		for (int i=0; i<3; i++) {
			Fish f = (Instantiate (Resources.Load ("Prefabs/Fish")) as GameObject).GetComponent<Fish>();
			f.SetCamera( mView);
			f.transform.parent = transform.FindChild ("Fishs").transform;
			f.Init(5);
			
		}
		
		
		for (int i=0; i<3; i++) {
			Fish f = (Instantiate (Resources.Load ("Prefabs/Fish")) as GameObject).GetComponent<Fish>();
			f.SetCamera( mView);
			f.transform.parent = transform.FindChild ("Fishs").transform;
			f.Init(8);
			
		}
	
	}
	

}
