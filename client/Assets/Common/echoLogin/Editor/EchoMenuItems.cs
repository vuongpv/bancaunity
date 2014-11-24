using UnityEngine;
using UnityEditor;

public class EchoMenuItems : ScriptableObject 
{
	public static bool 					addChildren				= false;
	public static bool 					FixScale				= true;
	public static bool 					activeAtStart			= true;
	public static bool 					rendererEnabled			= true;
	public static bool                  smartOptions            = true;

	public static bool 					forceToMono1			= false;
    public static int 					compressionBitrate1 	= (int)96;
    public static AudioImporterLoadType	loadType1				= AudioImporterLoadType.StreamFromDisc;
    public static bool 					threeD1					= false;
    public static AudioImporterFormat  	format1					= AudioImporterFormat.Compressed;
    public static bool 					hardware1				= true;
    public static bool 					loopable1				= true;

	public static bool 					forceToMono2			= false;
    public static int 					compressionBitrate2 	= (int)96;
    public static AudioImporterLoadType	loadType2				= AudioImporterLoadType.DecompressOnLoad;
    public static bool 					threeD2					= false;
    public static AudioImporterFormat  	format2					= AudioImporterFormat.Compressed;
    public static bool 					hardware2				= false;
    public static bool 					loopable2				= true;

	//==========================================================================
	public static void SavePrefs()
	{
		EditorPrefs.SetBool ( "_echoAddChildren", addChildren );	
		EditorPrefs.SetBool ( "_echoSetScale1", FixScale );	
		EditorPrefs.SetBool ( "_echoActiveAtStart", activeAtStart );
		EditorPrefs.SetBool ( "_echoRendererEnabled", activeAtStart );
		EditorPrefs.SetBool ( "_echoSmartOptions", smartOptions );

		EditorPrefs.SetBool ( "_echoForceToMono1", forceToMono1 );
		EditorPrefs.SetInt ( "_echoBitrate1", compressionBitrate1 );
		EditorPrefs.SetInt ( "_echoLoadType1", (int)loadType1 );
		EditorPrefs.SetBool ( "_echoThreeD1", threeD1 );
		EditorPrefs.SetBool ( "_echoHardware1", hardware1 );
		EditorPrefs.SetBool ( "_echoLoopable1", loopable1 );

		EditorPrefs.SetBool ( "_echoForceToMono2", forceToMono2 );
		EditorPrefs.SetInt ( "_echoBitrate2", compressionBitrate2 );
		EditorPrefs.SetInt ( "_echoLoadType2", (int)loadType2 );
		EditorPrefs.SetBool ( "_echoThreeD2", threeD2 );
		EditorPrefs.SetBool ( "_echoHardware2", hardware2 );
		EditorPrefs.SetBool ( "_echoLoopable2", loopable2 );
	}

	//==========================================================================
	public static void LoadPrefs()
	{
		addChildren				= EditorPrefs.GetBool ( "_echoAddChildren" );	
		FixScale				= EditorPrefs.GetBool ( "_echoSetScale1" );	
		activeAtStart			= EditorPrefs.GetBool ( "_echoActiveAtStart" );
		rendererEnabled			= EditorPrefs.GetBool ( "_echoRendererEnabled" );
		smartOptions			= EditorPrefs.GetBool ( "_echoSmartOptions" );

		forceToMono1			= EditorPrefs.GetBool ( "_echoForceToMono1" );
		compressionBitrate1		= EditorPrefs.GetInt ( "_echoBitrate1" );
		loadType1				= (AudioImporterLoadType)EditorPrefs.GetInt ( "_echoLoadType1" );
		threeD1					= EditorPrefs.GetBool ( "_echoThreeD1" );
		hardware1				= EditorPrefs.GetBool ( "_echoHardware1" );
		loopable1				= EditorPrefs.GetBool ( "_echoLoopable1" );

		forceToMono2			= EditorPrefs.GetBool ( "_echoForceToMono2" );
		compressionBitrate2		= EditorPrefs.GetInt ( "_echoBitrate2" );
		loadType2				= (AudioImporterLoadType)EditorPrefs.GetInt ( "_echoLoadType2" );
		threeD2					= EditorPrefs.GetBool ( "_echoThreeD2" );
		hardware2				= EditorPrefs.GetBool ( "_echoHardware2" );
		loopable2				= EditorPrefs.GetBool ( "_echoLoopable2" );
	}

	//==========================================================================
    [MenuItem ("Component/EchoComponent")]
	static void AddEchoComponent()
	{
        Object[] goes 	= Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
		EchoComponent ec;
		bool addflag 	= addChildren;
		bool scaleflag 	= FixScale;
		
		foreach ( GameObject go in goes )
		{
			ec = go.GetComponent<EchoComponent>();
			
			if ( ec == null )
				ec = go.AddComponent<EchoComponent>();
			
			if ( smartOptions && go.transform )
			{
				scaleflag = false;
		
				if ( go.transform.childCount > 0 )
					addflag = true;
			}
			
			ec.addChildren 		= addflag;
			ec.fixScale 		= scaleflag;

			if ( smartOptions )
				ec.activeAtStart 	= true;
			else
				ec.activeAtStart 	= activeAtStart;
			
			if ( smartOptions )
				ec.rendererEnabled 	= true;
			else
				ec.rendererEnabled 	= rendererEnabled;
		}
	}

	//==========================================================================
    [MenuItem ("Core/PlayerPrefs/Delete")]
    static void PlayerPrefsDelete() 
	{
        PlayerPrefs.DeleteAll();
    }

	//==========================================================================
    [MenuItem ("Core/Sound/Paste Music Settings")]
    static void ApplyMusic() 
	{
        Object[] audioclips = Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);

		Selection.objects = new Object[0];

        foreach ( AudioClip audioclip in audioclips )
		{
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter 		= AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.forceToMono 			= forceToMono1;
            audioImporter.compressionBitrate 	= compressionBitrate1 * 1000;
            audioImporter.loadType 				= loadType1;
            audioImporter.threeD 				= threeD1;
            audioImporter.format 				= format1;
            audioImporter.hardware 				= hardware1;
            audioImporter.loopable 				= loopable1;  

            AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceUpdate);
        }

    }

	//==========================================================================
    [MenuItem ("Core/Sound/Paste Sound FX Settings")]
    static void ApplySoundFX() 
	{
        Object[] audioclips = Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
		
		Selection.objects = new Object[0];
 
        foreach ( AudioClip audioclip in audioclips )
		{
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter 		= AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.forceToMono 			= forceToMono2;
            audioImporter.compressionBitrate 	= compressionBitrate2 * 1000;
            audioImporter.loadType 				= loadType2;
            audioImporter.threeD 				= threeD2;
            audioImporter.format 				= format2;
            audioImporter.hardware 				= hardware2;
            audioImporter.loopable 				= loopable2;  
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate );
        }
    }


	
    [MenuItem ("Core/Preferences")]
	static void InitPrefs()
	{
		EchoPrefs.Init();
	}
}