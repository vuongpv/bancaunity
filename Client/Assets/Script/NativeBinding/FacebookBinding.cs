using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;


public class FacebookBinding : MonoBehaviour
{

#if UNITY_IPHONE

	[DllImport("__Internal")]
	private static extern void facebook_init( string appID, string permissions );

	[DllImport("__Internal")]
	private static extern string facebook_accessToken();

	[DllImport("__Internal")]
	private static extern void facebook_login();

	[DllImport("__Internal")]
	private static extern void facebook_logout();

	[DllImport("__Internal")]
	private static extern void facebook_destroy();

	[DllImport("__Internal")]
	private static extern void facebook_postNewFeed( string name, string caption, string description, string imgurl, string link );

	[DllImport("__Internal")]
	private static extern void facebook_publishAppInstall();

#elif UNITY_ANDROID

	private static AndroidJavaObject obj_facebook;

#endif

	// Listeners
	public static Action<string> e_facebook_login;

	public static Action<bool> e_facebook_webdialog;

	public static bool IsInit = false;

	public static void Init(string appID, string permissions)
	{
		IsInit = true;

		if (UnityEngine.Object.FindObjectOfType(typeof(FacebookBinding)) == null)
		{
			GameObject go = new GameObject("FacebookBinding");
			go.AddComponent<FacebookBinding>();
			DontDestroyOnLoad(go);
		}

#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		facebook_init(appID, permissions);
#elif UNITY_ANDROID
		AndroidJNI.AttachCurrentThread(); 
		using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
				obj_facebook = new AndroidJavaObject("com.ap.api.Facebook");
				obj_facebook.CallStatic("init", appID, permissions);
			}
		}
#endif
	}

	/// <summary>
	/// finish purchase and release memory 
	/// </summary>
	public static void Destroy()
	{
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		facebook_destroy(  );
#endif
	}


	public static void Login()
	{
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		facebook_login();		
#elif UNITY_ANDROID
		obj_facebook.CallStatic("login");
#endif
	}

	public static string AccessToken()
	{
#if UNITY_EDITOR
		return "";
#elif UNITY_IPHONE
		return facebook_accessToken();		
#elif UNITY_ANDROID
		return obj_facebook.CallStatic<string>("accessToken");
#else
		return "";
#endif
	}

	public static void PostNewFeed(string name, string caption, string description, string imgurl, string link)
	{
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		facebook_postNewFeed( name, caption, description, imgurl, link );		
#elif UNITY_ANDROID
		obj_facebook.CallStatic("postNewFeed", name, caption, description, imgurl, link);
#endif
	}

	public static void SendAppRequest(string message)
	{
#if UNITY_IPHONE
		//facebook_sendAppRequest( message );		
#elif UNITY_ANDROID
		obj_facebook.CallStatic("sendAppRequest", message);
#endif
	}

	public static void PublishInstallApp()
	{
#if UNITY_EDITOR
        return;
#elif UNITY_IPHONE
		facebook_publishAppInstall();		
#elif UNITY_ANDROID
        obj_facebook.CallStatic("publishInstallApp");
#endif
	}


	#region [ Event callbacks ]

	void facebook_login(string status)
	{
		string accessToken = null;

		if (status == UnityEventStatus.OK)
		{
			accessToken = AccessToken();
			Debug.Log("Facebook login succeeded, " + accessToken);
		}

		if (e_facebook_login != null)
			e_facebook_login(status);
	}

	void facebook_webdialog(string status)
	{
		string accessToken = null;

		if (e_facebook_webdialog != null)
			e_facebook_webdialog(status == UnityEventStatus.OK);
	}

	#endregion
}