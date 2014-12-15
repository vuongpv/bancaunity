using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class FlurryBinding
{

#if UNITY_IPHONE

	[DllImport("__Internal")]
	private static extern void flurry_startsession(string apiKey);

	[DllImport("__Internal")]
	private static extern void flurry_stopsession();

	[DllImport("__Internal")]
	private static extern void flurry_logevent(string eventName);

	[DllImport("__Internal")]
	private static extern void flurry_logevent1(string eventName, string paramKey1, string paramValue1);

	[DllImport("__Internal")]
	private static extern void flurry_logevent2(string eventName, string paramKey1, string paramValue1, string paramKey2, string paramValue2);

	[DllImport("__Internal")]
	private static extern void flurry_logevent3(string eventName, string paramKey1, string paramValue1, string paramKey2, string paramValue2, string paramKey3, string paramValue3);

#elif UNITY_ANDROID

	private static AndroidJavaClass obj_flurry;

#endif

	public static void Init(string apiKey)
	{
	
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		flurry_startsession(apiKey);

#elif UNITY_ANDROID
		AndroidJNI.AttachCurrentThread();
		using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				obj_flurry = new AndroidJavaClass("com.ap.api.Flurry");
				obj_flurry.CallStatic("init", apiKey);
			}
		}
#endif
	}

	public static void Shutdown()
	{
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		
#elif UNITY_ANDROID
		if( obj_flurry != null )
			obj_flurry.CallStatic("shutdown");
#endif
	}

	public static void SendEvent(string eventName, params KeyValuePair<string, string>[] parameters )
	{		
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE

		if( parameters.Length == 0 )
			flurry_logevent(eventName);
		else if( parameters.Length == 1 )
			flurry_logevent1(eventName, parameters[0].Key, parameters[0].Value );
		else if( parameters.Length == 2 )
			flurry_logevent2(eventName, parameters[0].Key, parameters[0].Value, parameters[1].Key, parameters[1].Value );
		else if( parameters.Length == 3 )
			flurry_logevent3(eventName, parameters[0].Key, parameters[0].Value, parameters[1].Key, parameters[1].Value, parameters[2].Key, parameters[2].Value );
		

#elif UNITY_ANDROID
		if( obj_flurry == null )
			return;

		if( parameters.Length == 0 )
			obj_flurry.CallStatic("sendEvent", eventName);
		else if( parameters.Length == 1 )
			obj_flurry.CallStatic("sendEventWithParams1", eventName, parameters[0].Key, parameters[0].Value );
		else if( parameters.Length == 2 )
			obj_flurry.CallStatic("sendEventWithParams2", eventName, parameters[0].Key, parameters[0].Value, parameters[1].Key, parameters[1].Value );
		else if( parameters.Length == 3 )
			obj_flurry.CallStatic("sendEventWithParams3", eventName, parameters[0].Key, parameters[0].Value, parameters[1].Key, parameters[1].Value, parameters[2].Key, parameters[2].Value );
#endif
	}

	public static void SendEvent(string eventName, string key, string value )
	{
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		flurry_logevent1(eventName, key, value);

#elif UNITY_ANDROID
		if( obj_flurry == null )
			return;

		obj_flurry.CallStatic("sendEventWithParams1", eventName, key, value );
#endif
	}
}
