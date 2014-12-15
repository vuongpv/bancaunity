using UnityEngine;
using System.Collections;

public class NativeDialogBinding
{
#if UNITY_IPHONE

#elif UNITY_ANDROID

	private static AndroidJavaClass obj_nativeDialog;

#endif

	private static void Init()
	{
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		
#elif UNITY_ANDROID
		AndroidJNI.AttachCurrentThread();
		using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				obj_nativeDialog = new AndroidJavaClass("com.ap.api.NativeDialog");
				obj_nativeDialog.CallStatic("init");
			}
		}
#endif
	}

	/// <summary>
	/// Show alert dialog with a single button
	/// </summary>
	/// <param name="tilte"></param>
	/// <param name="message"></param>
	/// <param name="cancelable"></param>
	/// <param name="positiveLabel"></param>
	/// <param name="positiveEventName"></param>
	/// <param name="eventReceiver"></param>
	public static void ShowAlertDialog1(string tilte, string message, bool cancelable, string positiveLabel, string positiveEventName, string eventReceiver)
	{
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		
#elif UNITY_ANDROID
		if (obj_nativeDialog == null)
			Init();
		obj_nativeDialog.CallStatic("showAlertDialog1", tilte, message, cancelable, positiveLabel, positiveEventName, eventReceiver);
#endif
	}

	/// <summary>
	/// Show alert dialog with two buttons
	/// </summary>
	/// <param name="tilte"></param>
	/// <param name="message"></param>
	/// <param name="cancelable"></param>
	/// <param name="positiveLabel"></param>
	/// <param name="positiveEventName"></param>
	/// <param name="negativeLabel"></param>
	/// <param name="negativeEventName"></param>
	/// <param name="eventReceiver"></param>
	public static void ShowAlertDialog2(string tilte, string message, bool cancelable, string positiveLabel, string positiveEventName, string negativeLabel, string negativeEventName, string eventReceiver)
	{
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		
#elif UNITY_ANDROID
		if (obj_nativeDialog == null)
			Init();
		obj_nativeDialog.CallStatic("showAlertDialog2", tilte, message, cancelable, positiveLabel, positiveEventName, negativeLabel, negativeEventName, eventReceiver);
#endif
	}

	public static void ShowProgressDialog(string tilte, string message)
	{
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		
#elif UNITY_ANDROID
		if (obj_nativeDialog == null)
			Init();
		obj_nativeDialog.CallStatic("showProgressDialog", tilte, message);
#endif
	}

	public static void HideProgressDialog()
	{
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		
#elif UNITY_ANDROID
		if (obj_nativeDialog == null)
			Init();
		obj_nativeDialog.CallStatic("hideProgressDialog");
#endif
	}
}