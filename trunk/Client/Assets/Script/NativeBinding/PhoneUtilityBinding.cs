using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class PhoneUtilityBinding : MonoBehaviour {

	public static PhoneUtilityBinding instance;

	// Listeners
	public static Action<string> e_phone_sendsms;

	private static bool initialised = false;

#if UNITY_IPHONE

	[DllImport("__Internal")]
	private static extern void phone_init();

	[DllImport("__Internal")]
	private static extern void phone_sendsms( string number, string message );

#elif UNITY_ANDROID

	private static AndroidJavaClass obj_phoneUtility;

#endif

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	private static void Init()
	{
		initialised = true;

		if (instance == null)
		{
			GameObject go = new GameObject("PhoneUtilityBinding");
			go.AddComponent<PhoneUtilityBinding>();
			DontDestroyOnLoad(go);
		}

#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		phone_init();
		
#elif UNITY_ANDROID
		AndroidJNI.AttachCurrentThread();
		using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				obj_phoneUtility = new AndroidJavaClass("com.ap.api.PhoneUtility");
				obj_phoneUtility.CallStatic("init");
			}
		}
#endif
	}

	public static void SendSMSWithMessageApp(string number, string message)
	{
		if( !initialised )
			Init();

#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		phone_sendsms(number, message);
#elif UNITY_ANDROID
		obj_phoneUtility.CallStatic("sendSMSWithMessageApp", number, message);
#endif
	}

	public static void SendSMS(string number, string message)
	{
		if( !initialised )
			Init();

#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		phone_sendsms(number, message);
#elif UNITY_ANDROID
		obj_phoneUtility.CallStatic("sendSMS", number, message);
#endif
	}

	#region [ Event callbacks ]

	void phone_sms(string status)
	{
		Debug.LogError("SMS return code " + status);
		if (e_phone_sendsms != null)
			e_phone_sendsms(status);
	}


	#endregion

	
	
}
