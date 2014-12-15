using UnityEngine;
using System.Collections;
using System;


public class SO6PaymentCard
{
	public const string MOBIFONE = "VMS";
	public const string VINAFONE = "VNP";
	public const string VIETTEL = "VTT";
	public const string ZINGCARD = "ZC";
}

public class SO6PaymentBinding : ManualSingletonMono<SO6PaymentBinding>
{
	// Listeners
	public static Action<int> e_card_result;

#if UNITY_IPHONE

#elif UNITY_ANDROID

	private static AndroidJavaClass obj_so6Payment;

#endif

	public static void Init(string host, int port)
	{
		if (instance == null)
		{
			GameObject go = new GameObject("SO6PaymentBinding");
			go.AddComponent<SO6PaymentBinding>();
			DontDestroyOnLoad(go);
		}
		
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		
#elif UNITY_ANDROID
		AndroidJNI.AttachCurrentThread();
		using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				obj_so6Payment = new AndroidJavaClass("com.ap.api.SO6Payment");
				obj_so6Payment.CallStatic("init", host, port);
			}
		}
#endif
	}

	public static void SendCard(string userID, string gameID, int cardType, string cardSerialNo, string cardCode, string info)
	{
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		
#elif UNITY_ANDROID
		obj_so6Payment.CallStatic("send", userID, gameID, cardType, cardSerialNo, cardCode, info);
#endif
	}

	#region [ Event callbacks ]

	void so6payment__card_result(string status)
	{
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		
#elif UNITY_ANDROID
		int result = obj_so6Payment.CallStatic<int>("getLastResult");
		Debug.LogError("Card return code " + result);
		if (e_card_result != null)
			e_card_result(result);
#endif
	}


	#endregion
	
}
