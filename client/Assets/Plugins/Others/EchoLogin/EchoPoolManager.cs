using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//$-----------------------------------------------------------------------------
//@ Derive from this to make a pool manager script for an EchoGameObject.
//@
//@ Options:
//@
//# Number Of Pool Objects  - Maximum number of objects in pool
//# Game Object Prefab      - GameObject to instantiate 
//@ 
//@ Example:
//@ 
//%class MissileManager : EchoPoolManager
//%{
//%		static EchoPoolList      mypool;
//%
//@//===========================================================================
//%		void Start()
//%		{
//%				mypool = echoObjectPool;
//%		}
//%
//@//===========================================================================
//%		public static void Launch ( Vector3 istartpos, Vector3 idirection, Transform itarget, float ispeed )
//%		{
//%				MissileBrain brain = mypool.Inactive2Active() as MissileBrain;
//%
//%				if ( brain != null )
//%				{
//%						brain.Launch ( istartpos, idirection, itarget, ispeed );
//%				}
//%		}
//%}
//&-----------------------------------------------------------------------------
public class EchoPoolManager : MonoBehaviour
{
	[HideInInspector]
	public EchoPoolList      echoObjectPool;
	public int               NumberOfPoolObjects		= 2;
	public GameObject        GameObjectPrefab			= null;

//===========================================================================
	void Awake()
	{
		int 			loop;
		GameObject 		go;
		EchoComponent 	ec;

		// new pool object to manage the linked list of objects
		echoObjectPool = new EchoPoolList();

		// makes the new object to be used in pool
		for ( loop = 0; loop < NumberOfPoolObjects; loop++ )
		{
			go	= UnityEngine.Object.Instantiate ( GameObjectPrefab ) as GameObject;

#if UNITY_4_0
				go.SetActive ( true );
#else
				go.active = true;
#endif

			
			foreach ( Transform child in go.transform )
			{
#if UNITY_4_0
				child.gameObject.SetActive ( true );
#else
				child.gameObject.active = true;
#endif
			}
			
			
			// seems if we dont do this, it uses new materials for each new object ( not good )
			go.renderer.sharedMaterial	= GameObjectPrefab.renderer.sharedMaterial;
			ec							= go.GetComponent<EchoComponent>();

			// Adds new object to inactive pool list and deactivates it and all children
			ec.EchoPoolInit( echoObjectPool );
			ec.EchoPoolObjectInit();
		}
	}
}