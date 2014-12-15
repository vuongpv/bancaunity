using UnityEngine;
using System.Collections;


//$-----------------------------------------------------------------------------
//@ Attach this to the game objects or derive from this, in place of MonoBehaviour,
//@ to add the Core Framework functionality to GameObjects.
//@
//@ Options:
//@
//# Active At Start   	- Sets gameObject.active to this value at runtime
//# Renderer Enabled  	- Sets renderer.enabled to this value at runtime
//# Fix Scale 			- If true, will set the objects localScale to 1,1,1 and if needed scale mesh
//# Add Children      	- Adds EchoComponent to children that do not have one
//&-----------------------------------------------------------------------------
public class EchoComponent : EchoGameObject 
{
	public static int  		echoOverride 		= 0;
	[HideInInspector]
	public EchoComponent    echoListNext;
	[HideInInspector]
	public EchoComponent    echoListPrev;
	[HideInInspector]
	public EchoPoolList   	echoPool			= null;
	public bool             activeAtStart     	= true;     
	public bool             rendererEnabled     = true;     
	public bool 			fixScale 			= true; 	
	public bool             addChildren    		= true;
	private bool            _ecInitFlag           = false;          
	
//$===========================================================================
//@ If you override this from the extended class, you must call base.OnDestroy first
//@ so the framework can properly remove objects from the master list.
//&===========================================================================
	virtual public void OnDestroy()
	{
		if ( _ecInitFlag )
		{
			EchoGameObject.ListRemoveObject ( this );
		}
	}

//$===========================================================================
//@ If you override this from the extended class, you must call base.OnDisable first
//@ so the framework can properly remove objects from the pool.
//&===========================================================================
	virtual public void OnDisable()
	{
		if ( echoPool != null )
		{
			echoPool.Active2Inactive ( this );
		}
	}

//$===========================================================================
//@ If you override this from the extended class, you must call base.Awake first
//@ so EchoGameObject's can be initialized by the system when needed.
//&===========================================================================
	virtual public void Awake()
	{
		if ( echoOverride == 0 && _ecInitFlag == false )
		{
			EchoManualInit ( fixScale, false, addChildren, activeAtStart, rendererEnabled );
		}
	}

//$===========================================================================
//@ This is for the scripts that extend from EchoComponent that use Object Pools.
//@ If you override this method, it will be called on object creation at startup.
//&===========================================================================
	virtual public void EchoPoolObjectInit()
	{
	}
	
	//============================================================
	// this is called by framework, no user need to call this
	//============================================================
	public void EchoPoolInit ( EchoPoolList ipool = null )
	{
		echoPool	= ipool;

		if ( echoPool != null )
		{
			EchoActive ( false );

			echoPool.AddNewObject ( this );
		}
	}

	//============================================================
	// this is called by framework, no user need to call this
	//============================================================
	public void EchoManualInit ( bool iscale, bool iburnmeshscale, bool iaddchildren, bool iactive, bool irendererenabled )
	{
		if ( _ecInitFlag == false )
		{
			_ecInitFlag 	= true;
			addChildren		= iaddchildren;
			fixScale		= iscale;
			activeAtStart   = iactive;
			rendererEnabled = irendererenabled;

			Init ( transform.gameObject, fixScale, iburnmeshscale, addChildren, activeAtStart, rendererEnabled );
		}
	}
}

