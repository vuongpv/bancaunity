using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class EchoPrefs : EditorWindow
{
	bool fold1 = true;
	bool fold2 = true;
	bool fold3 = true;

	//==========================================================================
	void OnFocus()
	{
		EchoMenuItems.LoadPrefs();
	}

	//==========================================================================
	void OnDestroy()
	{
		EchoMenuItems.SavePrefs();
	}


	//==========================================================================
    public static void Init()
    {
		EchoMenuItems.LoadPrefs();
    	EditorWindow.GetWindow ( typeof ( EchoPrefs ) );
    }

	//==========================================================================
	void OnGUI()
	{

//		EditorGUILayout.LabelField ("EchoComponent Defaults");
		fold1 = EditorGUILayout.Foldout (fold1, "EchoComponent Defaults", EditorStyles.foldout  );
		if ( fold1 )
		{
        	EchoMenuItems.activeAtStart 	= EditorGUILayout.Toggle ("Active At Start", EchoMenuItems.activeAtStart );
        	EchoMenuItems.rendererEnabled 	= EditorGUILayout.Toggle ("Renderer Enabled", EchoMenuItems.rendererEnabled );
	   	    EchoMenuItems.FixScale 		= EditorGUILayout.Toggle ("Fix Scale",  EchoMenuItems.FixScale );
 	        EchoMenuItems.addChildren 		= EditorGUILayout.Toggle ("Add Children", EchoMenuItems.addChildren );
	        EchoMenuItems.smartOptions 		= EditorGUILayout.Toggle ("Smart Options", EchoMenuItems.smartOptions );
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		fold2 = EditorGUILayout.Foldout (fold1, "Music Defaults", EditorStyles.foldout  );
		if ( fold2 )
		{
			EchoMenuItems.threeD1					= EditorGUILayout.Toggle ("3D Sound", EchoMenuItems.threeD1 );
			EchoMenuItems.forceToMono1				= EditorGUILayout.Toggle ("Force To Mono", EchoMenuItems.forceToMono1 );
			EchoMenuItems.loadType1					= (AudioImporterLoadType)EditorGUILayout.EnumPopup("Load Type", EchoMenuItems.loadType1 );
			EchoMenuItems.compressionBitrate1		= (int)EditorGUILayout.Slider ("Compression (kbps)",  (float)EchoMenuItems.compressionBitrate1, 32, 256 );
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		fold3 = EditorGUILayout.Foldout (fold1, "Sound FX Defaults", EditorStyles.foldout  );
		if ( fold3 )
		{
			EchoMenuItems.threeD2					= EditorGUILayout.Toggle ("3D Sound", EchoMenuItems.threeD2 );
			EchoMenuItems.forceToMono2				= EditorGUILayout.Toggle ("Force To Mono", EchoMenuItems.forceToMono2 );
			EchoMenuItems.loadType2					= (AudioImporterLoadType)EditorGUILayout.EnumPopup("Load Type", EchoMenuItems.loadType2 );
			EchoMenuItems.compressionBitrate2		= (int)EditorGUILayout.Slider ("Compression (kbps)", (float)EchoMenuItems.compressionBitrate2, 32, 256 );
		}
		EditorGUILayout.Space();
	}

}