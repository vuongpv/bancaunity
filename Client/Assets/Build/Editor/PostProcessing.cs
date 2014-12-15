using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;

public class PostProcessing {
	
#if UNITY_IPHONE
	// Update is called once per frame
	[PostProcessBuild]
	public static void EditXCodeProject ( BuildTarget target, string pathToBuiltProject) 
	{
		// Edit XCodeProject
		XCProject currentProject = new XCProject(pathToBuiltProject);
		currentProject.ApplyMod("Assets/Build/ios_config.projmods" );
		currentProject.Save ();

		// Edit Info.plist
		Hashtable plist = new Hashtable();
		PListManager.ParsePListFile(pathToBuiltProject + "/Info.plist", ref plist);
		plist.Add("FacebookAppID", ExternalServices.FACEBOOK_ID);
		plist.Add("FacebookDisplayName", ExternalServices.FACEBOOK_DISPLAYNAME);

		string fbScheme = "fb" + ExternalServices.FACEBOOK_ID;
		plist.Add("CFBundleURLTypes", new ArrayList() { new Hashtable() { {"CFBundleURLSchemes", new ArrayList() { fbScheme } } } } );
		PListManager.SavePlistToFile(pathToBuiltProject + "/Info.plist", plist);

		// Copy files to target
		string[] files = new string[]{
			"Icon.png",
			"Icon-72.png",
			"Icon-144.png",
			"Icon@2x.png",
			"Default.png",
			"Default@2x.png",
			"Default-568h@2x.png",
			"Default-LandScape.png",
			"Default-LandScape@2x.png"
		};

		foreach(var file in files)
		{
			FileUtil.ReplaceFile(Application.dataPath + "/Build/iOS/Images/" + file, pathToBuiltProject + "/" + file);
		}
	}
#endif

}
