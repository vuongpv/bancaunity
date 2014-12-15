#pragma strict

public var prefab:GameObject;

function Start () 
{
	var loopx:int;
	var loopy:int;
	var go:GameObject;
	
	for ( loopy = 0; loopy < 8; loopy++ )
	{
		for ( loopx = 0; loopx < 12; loopx ++ )
		{
			go	= UnityEngine.Object.Instantiate ( prefab ) as GameObject;
			go.renderer.sharedMaterial = prefab.renderer.sharedMaterial;
			
#if UNITY_4_0
				go.SetActive ( true );
#else
				go.active = true;
#endif
			
			go.transform.localPosition = Vector3 ( ( loopx-5.5 ) * 70.0, ( loopy-3.5 ) * 64.0, prefab.transform.localPosition.z );
		}
	}


}

