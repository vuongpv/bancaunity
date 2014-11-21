using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//-------------------------------------------------------------------------------------
public class ELC
{
	public const float CELL256	= 1.0f / 256.0f;
	public const float CELL128	= 1.0f / 128.0f;
	public const float CELL64	= 1.0f / 64.0f;
	public const float CELL32	= 1.0f / 32.0f;
	public const float CELL16	= 1.0f / 16.0f;
	public const float CELL8	= 1.0f / 8.0f;
	public const float CELL4	= 1.0f / 4.0f;
	public const float CELL2	= 1.0f / 2.0f;
};


//$-----------------------------------------------------------------------------
//@ EchoGameObject 	- The core functionality of this framework.
//&-----------------------------------------------------------------------------
public class EchoGameObject : MonoBehaviour
{
	static EchoGameObject 					egoFirst 		= null;
	static EchoGameObject 					egoLast 		= null;
	public static bool                 		_initFlag 		= false;
	public static MaterialPropertyBlock 	matProperties  	= null;
	public static int 						echoRGBA_id;
	public static int 						echoAlpha_id;
	public static int 						echoMix_id;
	public static int 						echoHitMix0_id;
	public static int 						echoHitMix1_id;
	public static int 						echoHitMix2_id;
	public static int 						echoHitMix3_id;
	public static int 						echoUV_id;
	public static int 						echoScale_id;
	public static int 						echoColor_id;
	public static int 						echoHitVector0_id;
	public static int 						echoHitVector1_id;
	public static int 						echoHitVector2_id;
	public static int 						echoHitVector3_id;
	public static int 						echoMainTexST_id;
	private bool							_echoRGBA_flag;
	private bool							_echoAlpha_flag;
	private bool							_echoMix_flag;
	private bool							_echoHitMix0_flag;
	private bool							_echoHitMix1_flag;
	private bool							_echoHitMix2_flag;
	private bool							_echoHitMix3_flag;
	private bool							_echoUV_flag;
	private bool							_echoScale_flag;
	private bool							_echoColor_flag;
	private bool							_echoHitVector0_flag;
	private bool							_echoHitVector1_flag;
	private bool							_echoHitVector2_flag;
	private bool							_echoHitVector3_flag;
	private bool							_echoMainTexST_flag;
	private Vector4							_echoRGBA;
	private Vector4							_echoColor;
	private float							_echoAlpha;
	private float							_echoMix;
	private float						    _echoHitMix0;
	private float							_echoHitMix1;
	private float							_echoHitMix2;
	private float							_echoHitMix3;
	private Vector4							_echoHitVector0;
	private Vector4							_echoHitVector1;
	private Vector4							_echoHitVector2;
	private Vector4							_echoHitVector3;
	private Vector4							_echoUV;
	private Vector4							_echoScale;
	private Vector4							_echoMainTexST;
	private EchoGameObject 					_next;
	private EchoGameObject 					_prev;
	private Vector3                         _originalScale;
	[HideInInspector]
	public  bool                            activeList 	= false; 
	[HideInInspector]
	public Transform 						cachedTransform;
	[HideInInspector]
  	public int 								meshVertCount;
	[HideInInspector]
  	public MeshFilter 						meshFilter;
	[HideInInspector]
  	public Mesh 							mesh;
	[HideInInspector]
	public EchoGameObject					echoFxNext;
	[HideInInspector]
	public bool                     		echoFxFlag;
	[HideInInspector]
	public EchoGameObject[]					children;
	[HideInInspector]
	public int                              childCount;
	[HideInInspector]
	public int                              uvSetCount;
	private List<Vector2[]> uvSet			= new List<Vector2[]>();
	
//---------------------------------------------------------------------------
//  this is automatically called when first EchoGameObject is made
//---------------------------------------------------------------------------
	public static void InitAtStartup()
	{
		_initFlag		= true;
		egoFirst		= new GameObject().AddComponent<EchoGameObject>();
		egoLast			= new GameObject().AddComponent<EchoGameObject>();

		egoFirst._next	= egoLast;
		egoFirst._prev	= null;

		egoLast._next	= null;
		egoLast._prev	= egoFirst;

		matProperties = new MaterialPropertyBlock();

		echoColor_id			= Shader.PropertyToID ("_echoColor");
		echoRGBA_id				= Shader.PropertyToID ("_echoRGBA");
		echoAlpha_id			= Shader.PropertyToID ("_echoAlpha");
		echoMix_id				= Shader.PropertyToID ("_echoMix");
		echoUV_id				= Shader.PropertyToID ("_echoUV");
		echoScale_id			= Shader.PropertyToID ("_echoScale");
		echoHitVector0_id		= Shader.PropertyToID ("_echoHitVector0");
		echoHitVector1_id		= Shader.PropertyToID ("_echoHitVector1");
		echoHitVector2_id		= Shader.PropertyToID ("_echoHitVector2");
		echoHitVector3_id		= Shader.PropertyToID ("_echoHitVector3");
		echoHitMix0_id			= Shader.PropertyToID ("_echoHitMix0");
		echoHitMix1_id			= Shader.PropertyToID ("_echoHitMix1");
		echoHitMix2_id			= Shader.PropertyToID ("_echoHitMix2");
		echoHitMix3_id			= Shader.PropertyToID ("_echoHitMix3");
		echoMainTexST_id		= Shader.PropertyToID ("_MainTex_ST");
	}

//---------------------------------------------------------------------------
// Adds Object to master EchoGameObject list
//---------------------------------------------------------------------------
	public static void ListAddObject ( EchoGameObject iego )
	{
		iego._next				= egoLast;
		iego._prev				= egoLast._prev;

		egoLast._prev._next		= iego;
		egoLast._prev			= iego;
	}

//---------------------------------------------------------------------------
// Removes Object from master EchoGameObject list
//---------------------------------------------------------------------------
	public static void ListRemoveObject ( EchoGameObject iego )
	{
		iego._prev._next			= iego._next;
		iego._next._prev			= iego._prev;
	}

//$-----------------------------------------------------------------------------
//@ Finds any EchoGameObject, even if its inactive
//@
//@ NOTE: All EchoGameObject should be active at start. If you need them inactive,
//@ set the Active At Start EchoCompoent Option to false.
//@
//@ Parameters:
//@
//# igameobjectname - Name of the game object
//&-----------------------------------------------------------------------------
	public static EchoGameObject Find ( string igameobjectname )
	{
		EchoGameObject  ego		= null;

		for ( ego = egoFirst._next; ego != egoLast; ego = ego._next )
		{
			if ( ego.name == igameobjectname )
			{
				break;				
			}
		}

		return ( ego );
	}

//==========================================================================
	public void Init ( GameObject igo = null, bool ifixScale = false, bool iburnmeshscale = false, bool iaddchildren = false, bool iactiveflag = true, bool irendererenabled = true )
	{
		bool burnMeshScale = iburnmeshscale;
		
		if ( igo == null )
			return;

		if ( _initFlag == false )
			InitAtStartup();

		ListAddObject ( this );

//		go              = igo;
#if UNITY_4_0
		gameObject.SetActive ( iactiveflag );
#else
		gameObject.active = iactiveflag;
#endif

		cachedTransform		= gameObject.transform;	// cache transform for speed	
		meshFilter 			= GetComponent< MeshFilter>() as MeshFilter;
	
		echoFxNext			= null;
		echoFxFlag			= false;

		if ( renderer )
			renderer.enabled		= irendererenabled;

		childCount				= 0;

		if ( meshFilter )
        {
            mesh             = meshFilter.mesh;
            meshVertCount    = meshFilter.mesh.vertices.Length;
        }

		_originalScale = cachedTransform.localScale;
		
		if ( ifixScale )
		{
			if ( Mathf.Abs ( 1.0f - cachedTransform.localScale.x ) > 0.0001 || Mathf.Abs ( 1.0f - cachedTransform.localScale.y ) > 0.0001 || Mathf.Abs ( 1.0f - cachedTransform.localScale.z ) > 0.0001 )
			{
				burnMeshScale = true;
			}
		}
		
		if ( burnMeshScale )
			EchoBurnMeshScale();

		if ( ifixScale )
			cachedTransform.localScale = new Vector3 ( 1, 1, 1 );

		InitShaderProperties();

		if ( iaddchildren )
		{
			EchoComponent   ec;

			EchoComponent.echoOverride++;

			children = new EchoGameObject[ cachedTransform.childCount ];

			foreach ( Transform child in cachedTransform )
			{
				if ( burnMeshScale )
				{
					Vector3 scale;
					Vector3 newpos;

					scale.x = child.transform.localScale.x * _originalScale.x;
					scale.y = child.transform.localScale.y * _originalScale.y;
					scale.z = child.transform.localScale.z * _originalScale.z;

					newpos.x = child.transform.localPosition.x * _originalScale.x;
					newpos.y = child.transform.localPosition.y * _originalScale.y;
					newpos.z = child.transform.localPosition.z * _originalScale.z;

					child.transform.localPosition		= newpos;
					child.transform.localScale			= scale;
				}

				ec = child.gameObject.GetComponent<EchoComponent>();

				if ( ec == null )
				{
					ec = child.gameObject.AddComponent<EchoComponent>();
					
					if ( ec != null )
					{
						ec.EchoManualInit ( ifixScale, iburnmeshscale, true, iactiveflag, irendererenabled );
					}
				}

				children[childCount] = ec as EchoGameObject;
				childCount++;
			}
			
			EchoComponent.echoOverride--;
		}
	}

//$-----------------------------------------------------------------------------
//@ Sets EchoGameObject active or inactive 
//@
//@ Parameters:
//@
//# ionoff 			- turn on or off
//# ichildrenflag  	- turn children of this object to ionoff
//&-----------------------------------------------------------------------------
	public void EchoActive ( bool ionoff )
	{
#if UNITY_4_0
		gameObject.SetActive ( ionoff );	
#else
		gameObject.active = ionoff;

		for ( int loop = 0; loop < childCount; loop++ )
		{
			children[loop].EchoActive ( ionoff );
		}
#endif
		
	}

//$-----------------------------------------------------------------------------
//@ Sets EchoGameObject renderer.enabled to value 
//@
//@ Parameters:
//@
//# ionoff 			- turn on or off
//# ichildrenflag  	- turn children of this object to ionoff
//&-----------------------------------------------------------------------------
	public void RendererSet ( bool ionoff, bool ichildrenflag = false )
	{
		renderer.enabled = ionoff;

		if ( ichildrenflag )
		{
			for ( int loop = 0; loop < childCount; loop++ )
			{
				children[loop].RendererSet ( ionoff, ichildrenflag );
			}
		}
	}
	
//===========================================================================
// Scales mesh and colliders so thier localScale can be 1,1,1
//===========================================================================
	public void EchoBurnMeshScale()
	{
		int loop;
		Vector3[] newv = new Vector3[mesh.vertices.Length];
		Vector3 scale;
		Vector3 newpos;

		if ( collider != null )
		{
			if ( collider.GetType() == typeof(BoxCollider) )
			{
				BoxCollider c = collider as BoxCollider;

				scale.x = c.size.x * cachedTransform.localScale.x;
				scale.y = c.size.y * cachedTransform.localScale.y;
				scale.z = c.size.z * cachedTransform.localScale.z;

				newpos.x = c.center.x * cachedTransform.localScale.x;
				newpos.y = c.center.y * cachedTransform.localScale.y;
				newpos.z = c.center.z * cachedTransform.localScale.z;

				c.center 	= newpos;
				c.size 	= scale;
			}
			else if ( collider.GetType() == typeof(SphereCollider) )
			{
				SphereCollider c = collider as SphereCollider;

				scale.x = c.radius * cachedTransform.localScale.x;
				scale.y = c.radius * cachedTransform.localScale.y;
				scale.z = c.radius * cachedTransform.localScale.z;

				newpos.x = c.center.x * cachedTransform.localScale.x;
				newpos.y = c.center.y * cachedTransform.localScale.y;
				newpos.z = c.center.z * cachedTransform.localScale.z;

				c.center 	= newpos;

				if ( scale.x > scale.y && scale.x > scale.z )
					c.radius = c.radius * scale.x;
				else if ( scale.y > scale.x && scale.y > scale.z )
					c.radius = c.radius * scale.y;
				else
					c.radius = c.radius * scale.z;
			}
			else if ( collider.GetType() == typeof(CapsuleCollider) )
			{
				CapsuleCollider c = collider as CapsuleCollider;

				scale.x = cachedTransform.localScale.x;
				scale.y = cachedTransform.localScale.y;
				scale.z = cachedTransform.localScale.z;

				switch ( c.direction )
				{
					case 0:
						c.height = c.height * scale.x;

						if ( scale.y > scale.z )
							c.radius = c.radius * scale.y;
						else 
							c.radius = c.radius * scale.z;
						break;

					case 1:
						c.height = c.height * scale.y;

						if ( scale.x > scale.z )
							c.radius = c.radius * scale.x;
						else 
							c.radius = c.radius * scale.z;
						break;

					case 2:
						c.height = c.height * scale.z;

						if ( scale.x > scale.y )
							c.radius = c.radius * scale.x;
						else 
							c.radius = c.radius * scale.y;
						break;
				}

				newpos.x = c.center.x * cachedTransform.localScale.x;
				newpos.y = c.center.y * cachedTransform.localScale.y;
				newpos.z = c.center.z * cachedTransform.localScale.z;

				c.center 	= newpos;
			}
		}

		for ( loop = 0; loop < mesh.vertices.Length; loop++ )
		{
			newv[loop].x = mesh.vertices[loop].x * cachedTransform.localScale.x;
			newv[loop].y = mesh.vertices[loop].y * cachedTransform.localScale.y;
			newv[loop].z = mesh.vertices[loop].z * cachedTransform.localScale.z;
		}	

		mesh.vertices = newv;
		mesh.RecalculateBounds();

		cachedTransform.localScale = new Vector3 ( 1, 1, 1 );

		if ( collider != null && collider.GetType() == typeof(MeshCollider) )
		{
			MeshCollider c		= collider as MeshCollider;
			c.sharedMesh		= mesh;
		}
	}

//$===========================================================================
//@ Makes a clone of an EchoGameObject
//@
//@ Parameters:
//@
//# iactievflag 	- sets cloned object active state
//&===========================================================================
  	public EchoGameObject Clone ( bool iactiveflag )
	{
	  	GameObject newgo = UnityEngine.Object.Instantiate ( this ) as GameObject;
		EchoGameObject ego;

		ego			= new EchoGameObject();
		
		ego.Init ( newgo );
		
		ego.EchoActive ( iactiveflag );
		
		ego.gameObject.layer					= gameObject.layer;
		ego.gameObject.renderer.sharedMaterial 	= renderer.sharedMaterial;
		
		return ( ego );
	}

//$===========================================================================
//@ This adds a shield sphere object to an existing EchoGameObject.
//@ This should be for testing only.
//@
//@ Parameters:
//@
//# imat  		 	- Material with the echoLogin shield shader on it
//# iscale          - Object scale
//&===========================================================================
	public EchoGameObject AddShield ( Material imat = null, float iscale = 1.2f )
	{
		GameObject 		shield;
		EchoGameObject 	egoShield;
		Vector3 		scale;
		SphereCollider  sc;
		Rigidbody  		rb;

		scale.x = Mathf.Abs ( mesh.bounds.max.x - mesh.bounds.min.x ) * iscale;
		scale.y = Mathf.Abs ( mesh.bounds.max.y - mesh.bounds.min.y ) * iscale;
		scale.z = Mathf.Abs ( mesh.bounds.max.z - mesh.bounds.min.z ) * iscale;

		shield = GameObject.CreatePrimitive ( PrimitiveType.Sphere );			
		shield.transform.localScale		= scale;
		shield.renderer.sharedMaterial	= imat;

		egoShield = new EchoGameObject();
		egoShield.Init ( shield , false, false, true );
		egoShield.EchoBurnMeshScale();

		egoShield.transform.position			= cachedTransform.position;
		egoShield.transform.parent				= cachedTransform;

		sc = shield.GetComponent<SphereCollider>();

		if ( sc == null )
			sc = shield.AddComponent<SphereCollider>();

		sc.isTrigger 	= true;

		rb = shield.AddComponent<Rigidbody>();
		rb.useGravity	= false;
		rb.isKinematic	= true;

		return ( egoShield );
	}

//$===========================================================================
//@ Makes a UV set offset from base UV
//@
//@ Parameters:
//@
//# addu	 	- U offset
//# addv        - V offset
//&===========================================================================
	public void UVCellMake ( float addu, float addv )
	{
		int loop;
		Vector2[] uvs = new Vector2[mesh.vertices.Length];

		for ( loop = 0; loop < mesh.vertices.Length; loop++ )
		{
			uvs[loop] = new Vector2 ( mesh.uv[loop].x + addu, mesh.uv[loop].y - addv );
		}

		uvSet.Add ( uvs );
		uvSetCount++;
	}

//$===========================================================================
//@ Makes a grid of UV sets to be used for cell animation
//@
//@ Parameters:
//@
//# addu	 	- U offset of each cell  
//# addv        - V offset of each cell
//# iwidth      - cellwidth
//# icount      - number of cells
//&===========================================================================
	public void UVSetMake ( float addu, float addv, int iwidth, int icount )
	{
		int loop;
		int x;
		int y;

		for ( loop = 0; loop < icount; loop++ )
		{
			x = loop % iwidth;
			y = loop / iwidth;

			UVCellMake ( x * addu, y * addv );
		}
	}

//$===========================================================================
//@ Sets a new UV set to mesh
//@
//@ NOTE: you must have setup UV sets with UVSetMake before calling this
//@
//@ Parameters:
//@
//# index	 	- index of uv set  
//&===========================================================================
	public void UVSet ( int index )
	{
		mesh.uv = uvSet[index];	
	}

//===========================================================================
// Init all the shader properties for this object  ( no user need to call this )
//===========================================================================
	public void InitShaderProperties()
	{
		if ( renderer == null )
			return;

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoColor") )
		{
			_echoColor = renderer.sharedMaterial.GetVector ("_echoColor");
			_echoColor_flag = true;
		}
		else
		{
			_echoColor_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoRGBA") )
		{
			_echoRGBA = renderer.sharedMaterial.GetVector ("_echoRGBA");
			_echoRGBA_flag = true;
		}
		else
		{
			_echoRGBA_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoAlpha") )
		{
			_echoAlpha = renderer.sharedMaterial.GetFloat ("_echoAlpha");
			_echoAlpha_flag = true;
		}
		else
		{
			_echoAlpha_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoMix") )
		{
			_echoMix = renderer.sharedMaterial.GetFloat ("_echoMix");
			_echoMix_flag = true;
		}
		else
		{
			_echoMix_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoHitMix0") )
		{
			_echoHitMix0 = renderer.sharedMaterial.GetFloat ("_echoHitMix0");
			_echoHitMix0_flag = true;
		}
		else
		{
			_echoHitMix0_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoHitMix1") )
		{
			_echoHitMix1 = renderer.sharedMaterial.GetFloat ("_echoHitMix1");
			_echoHitMix1_flag = true;
		}
		else
		{
			_echoHitMix1_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoHitMix2") )
		{
			_echoHitMix2 = renderer.sharedMaterial.GetFloat ("_echoHitMix2");
			_echoHitMix2_flag = true;
		}
		else
		{
			_echoHitMix2_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoHitMix3") )
		{
			_echoHitMix3 = renderer.sharedMaterial.GetFloat ("_echoHitMix3");
			_echoHitMix3_flag = true;
		}
		else
		{
			_echoHitMix3_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoUV") )
		{
			_echoUV = renderer.sharedMaterial.GetVector ("_echoUV");
			_echoUV_flag = true;
		}
		else
		{
			_echoUV_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoScale") )
		{
			_echoScale = renderer.sharedMaterial.GetVector ("_echoScale");
			_echoScale_flag = true;
		}
		else
		{
			_echoScale_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoHitVector0") )
		{
			_echoHitVector0 = renderer.sharedMaterial.GetVector ("_echoHitVector0");
			_echoHitVector0_flag = true;
		}
		else
		{
			_echoHitVector0_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoHitVector1") )
		{
			_echoHitVector1 = renderer.sharedMaterial.GetVector ("_echoHitVector1");
			_echoHitVector1_flag = true;
		}
		else
		{
			_echoHitVector1_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoHitVector2") )
		{
			_echoHitVector2 = renderer.sharedMaterial.GetVector ("_echoHitVector2");
			_echoHitVector2_flag = true;
		}
		else
		{
			_echoHitVector2_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty("_echoHitVector3") )
		{
			_echoHitVector3 = renderer.sharedMaterial.GetVector ("_echoHitVector3");
			_echoHitVector3_flag = true;
		}
		else
		{
			_echoHitVector3_flag = false;
		}

		//-------
		if ( renderer.sharedMaterial.HasProperty ( "_MainTex_ST" ) )
		{
			_echoMainTexST = renderer.sharedMaterial.GetVector ( "_MainTex_ST" );
			_echoMainTexST_flag = true;
		}
		else
		{
			_echoMainTexST_flag = false;
		}
	}

//$-----------------------------------------------------------------------------
//@ Submits this object's shader properties
//&-----------------------------------------------------------------------------
	public void ShaderPropertiesSubmit()
	{
		matProperties.Clear();

		if ( _echoRGBA_flag )
			matProperties.AddVector ( echoRGBA_id, _echoRGBA );

		if ( _echoColor_flag )
			matProperties.AddVector ( echoRGBA_id, _echoColor );

		if ( _echoAlpha_flag )
			matProperties.AddFloat ( echoAlpha_id, _echoAlpha );

		if ( _echoMix_flag )
			matProperties.AddFloat ( echoMix_id, _echoMix );

		if ( _echoHitMix0_flag )
			matProperties.AddFloat ( echoHitMix0_id, _echoHitMix0 );

		if ( _echoHitMix1_flag )
			matProperties.AddFloat ( echoHitMix1_id, _echoHitMix1 );

		if ( _echoHitMix2_flag )

			matProperties.AddFloat ( echoHitMix2_id, _echoHitMix2 );

		if ( _echoHitMix3_flag )
			matProperties.AddFloat ( echoHitMix3_id, _echoHitMix3 );

		if ( _echoHitVector0_flag )
			matProperties.AddVector ( echoHitVector0_id, _echoHitVector0 );

		if ( _echoHitVector1_flag )
			matProperties.AddVector ( echoHitVector1_id, _echoHitVector1 );

		if ( _echoHitVector2_flag )
			matProperties.AddVector ( echoHitVector2_id, _echoHitVector2 );

		if ( _echoHitVector3_flag )
			matProperties.AddVector ( echoHitVector3_id, _echoHitVector3 );

		if ( _echoUV_flag )
			matProperties.AddVector ( echoUV_id, _echoUV );

		if ( _echoScale_flag )
			matProperties.AddVector ( echoScale_id, _echoScale );

		if ( _echoMainTexST_flag )
			matProperties.AddVector ( echoMainTexST_id, _echoMainTexST );

		renderer.SetPropertyBlock ( matProperties );
	}

//$=============================================================================
//@ Sets the Tiling value of shaders with _MainTexST property
//@
//@ Parameters:
//@
//# itile  	    - Vector2 of the tiling value to set
//&-----------------------------------------------------------------------------
	public void ShaderSetTiling ( Vector2 itile )
	{
		_echoMainTexST.x = itile.x;
		_echoMainTexST.y = itile.y;
	}

//$=============================================================================
//@ Sets the Offset value of shaders with _MainTexST property
//@
//@ Parameters:
//@
//# ioffset  	    - Vector2 of the tiling value to set
//&-----------------------------------------------------------------------------
	public void ShaderSetOffset ( Vector2 ioffset )
	{
		_echoMainTexST.z = ioffset.x;
		_echoMainTexST.w = ioffset.y;
	}

//$=============================================================================
//@ Sets the alpha value of shaders with the _echoAlpha property
//@
//@ Parameters:
//@
//# ialpha      - Float alpha value 0-1
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoAlpha ( float ialpha )
	{
		_echoAlpha = ialpha;
	}

//$=============================================================================
//@ Sets the RGBA value of shaders with the _echoRGBA property
//@
//@ Parameters:
//@
//# irgba  	    - Vector4 of RGBA values 0-2
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoRGBA ( Vector4 irgba )
	{
		_echoRGBA = irgba;
	}

//$=============================================================================
//@ Sets the Color value of shaders with the _echoColor property
//@
//@ Parameters:
//@
//# icolor     - Color in ( r,g,b,a )
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoColor ( Color icolor )
	{
		_echoColor = icolor;
	}

//$=============================================================================
//@ Sets the Scale value of shaders with the _echoScale property
//@
//@ Parameters:
//@
//# iscale     - Scale ( x, y, z, 1 )  
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoScale ( Vector4 iscale )
	{
		_echoScale = iscale;
	}

//$=============================================================================
//@ Sets the UV for shaders that have the _echoUV or MainTex_ST property
//@
//@ Parameters:
//@
//# iuv  		- UV as Vector4 ( u1, v1, u2, v2 )  
//# itype       - 0 = using echoLogin shaders, 1 = use _MainTex_ST for offset
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoUV ( Vector4 iuv, int itype )
	{
		if ( iuv.x > 1.0f )
			iuv.x -= 1.0f;
		if ( iuv.x < 0.0f )
			iuv.x += 1.0f;

		if ( iuv.y > 1.0f )
			iuv.y -= 1.0f;
		if ( iuv.y < 0.0f )
			iuv.y += 1.0f;

		if ( itype == 0 )
		{
			if ( iuv.z > 1.0f )
				iuv.z -= 1.0f;
			if ( iuv.z < 0.0f )
				iuv.z += 1.0f;

			if ( iuv.w > 1.0f )
				iuv.w -= 1.0f;
			if ( iuv.w < 0.0f )
				iuv.w += 1.0f;

			_echoUV = iuv;
		}
		else
		{
			_echoMainTexST.z = iuv.x;
			_echoMainTexST.w = iuv.y;
		}
	}


//$=============================================================================
//@ Sets the UV for shaders that have the _echoUV as cells for cell animation
//@
//@ Parameters:
//@
//# cellnum         - Position in cell grid to show
//# iuvcellwidth    - Width of cell in UV space
//# iuvcellheiught  - Height of cell in UV space
//# icolumns        - Number of columns across in cell grid
//# iuvset          - Which UV set to use (1, 2 or zero for both)    
//&-----------------------------------------------------------------------------
	public void ShaderSetCell_echoUV ( int cellnum, float iuvcellwidth, float iuvcellheight, int icolumns, int iuvset = 1 )
	{
		if ( iuvset == 0 || iuvset == 1 )
		{
			_echoUV.x = ( cellnum % icolumns ) * iuvcellwidth;
			_echoUV.y = 1.0f - ( cellnum / icolumns ) * iuvcellheight;
		}

		if ( iuvset == 0 || iuvset == 2 )
		{
			_echoUV.z = ( cellnum % icolumns ) * iuvcellwidth;
			_echoUV.w = 1.0f - ( cellnum / icolumns ) * iuvcellheight;
		}
	}

//$=============================================================================
//@ Sets the mix value for shaders that have _echoMix property
//@
//@ Parameters:
//@
//# imix        - Mix amount (purpose varys with shader-see shader docs)
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoMix ( float imix )
	{
		_echoMix = imix;
	}

//$=============================================================================
//@ Sets the Hit Mix value for shaders that have _echoHitMix0-3 property
//@
//@ Parameters:
//@
//# ihitid       - Hit number to use (can be 0-3 in shield shader)
//# imix         - Mix amount (purpose varys with shader-see shader docs)
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoHitMix ( int ihitid, float imix )
	{
		switch ( ihitid )
		{
			case 0:
				_echoHitMix0 = imix;
				break;

			case 1:
				_echoHitMix1 = imix;
				break;

			case 2:
				_echoHitMix2 = imix;
				break;

			case 3:
				_echoHitMix3 = imix;
				break;

			default:
				break;
		}
	}

//$=============================================================================
//@ Sets _hitVector0-3 property and turns on Hit Mode
//@
//@ NOTE: This is only used on Shield Shader right now.
//@
//@ Parameters:
//@
//# ivec   	   - Direction of the hit effect
//# ihitnum    - 0-3
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoHitVectorOn ( Vector3 ivec, int ihitnum )
	{
		switch ( ihitnum )
		{
			case 0:
				_echoHitVector0.x = ivec.x; 
				_echoHitVector0.y = ivec.y; 
				_echoHitVector0.z = ivec.z; 
				_echoHitVector0.w = 1.0f; 
				break;

			case 1:
				_echoHitVector1.x = ivec.x; 
				_echoHitVector1.y = ivec.y; 
				_echoHitVector1.z = ivec.z; 
				_echoHitVector1.w = 1.0f; 
				break;

			case 2:
				_echoHitVector2.x = ivec.x; 
				_echoHitVector2.y = ivec.y; 
				_echoHitVector2.z = ivec.z; 
				_echoHitVector2.w = 1.0f; 
				break;

			case 3:
				_echoHitVector3.x = ivec.x; 
				_echoHitVector3.y = ivec.y; 
				_echoHitVector3.z = ivec.z; 
				_echoHitVector3.w = 1.0f; 
				break;

			default:
				break;
		}
	}

//$=============================================================================
//@ Turns off a HitVector
//@
//@ NOTE: This is only used on Shield Shader right now.
//@
//@ Parameters:
//@
//# ihitnum    - 0-3
//&-----------------------------------------------------------------------------
	public void ShaderSet_echoHitVectorOff ( int ihitnum )
	{
		switch ( ihitnum )
		{
			case 0:
				_echoHitVector0.x = 0.0f; 
				_echoHitVector0.y = 0.0f; 
				_echoHitVector0.z = 0.0f; 
				_echoHitVector0.w = 0.0f; 
				break;

			case 1:
				_echoHitVector1.x = 0.0f; 
				_echoHitVector1.y = 0.0f; 
				_echoHitVector1.z = 0.0f; 
				_echoHitVector1.w = 0.0f; 
				break;

			case 2:
				_echoHitVector2.x = 0.0f; 
				_echoHitVector2.y = 0.0f; 
				_echoHitVector2.z = 0.0f; 
				_echoHitVector2.w = 0.0f; 
				break;

			case 3:
				_echoHitVector3.x = 0.0f; 
				_echoHitVector3.y = 0.0f; 
				_echoHitVector3.z = 0.0f; 
				_echoHitVector3.w = 0.0f; 
				break;

			default:
				break;
		}
	}
}

