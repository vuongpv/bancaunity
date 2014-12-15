using UnityEngine;
using System.Collections;

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public class GABinding
{
}

public class GoogleAnalyticsBinding : MonoBehaviour {

    private static string tracking_id = "UA-42384695-1";
    private static int dispatchPeriod=100;
    private static bool debug=true;

#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void ga_init( int dispatchPeriod, bool debug);

	[DllImport("__Internal")]
	private static extern void ga_startTracker( string acountID, int dispatchPeriod);

	[DllImport("__Internal")]
	private static extern void ga_sendView(string pageName);

	[DllImport("__Internal")]
	private static extern void ga_sendEvent(string category, string action, string label, long quantity);

	[DllImport("__Internal")]
	private static extern void ga_dispatch();

	[DllImport("__Internal")]
	private static extern void ga_stopTracker();

#elif UNITY_ANDROID
    private static AndroidJavaObject obj_GoogleAnalytic;
#endif
    private static bool isInit = false;
 
    public static void Init()
    {
        if (isInit)
        {
            return;
        }
        isInit = true;
        if (UnityEngine.Object.FindObjectOfType(typeof(GoogleAnalyticsBinding)) == null)
        {
            GameObject go = new GameObject("GoogleAnalyticsBinding");
            go.AddComponent<GoogleAnalyticsBinding>();
            DontDestroyOnLoad(go);
        }

#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		ga_init( dispatchPeriod, debug );
		ga_startTracker( tracking_id, dispatchPeriod );
#elif UNITY_ANDROID
        AndroidJNI.AttachCurrentThread(); 
	    using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
		    using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
			    obj_GoogleAnalytic = new AndroidJavaObject("com.ap.api.GoogleAnalytics");
                obj_GoogleAnalytic.CallStatic("init", obj_Activity,dispatchPeriod, debug);
                obj_GoogleAnalytic.Call("startTracker",tracking_id);
		    }
	    }
#endif
    }
    public static void SendEvent(string category,string action, string label, long value)
    {
		if (!isInit)
		{
			Init();
		}
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		ga_sendEvent( category, action, label, value );
#elif UNITY_ANDROID
        //Debug.LogWarning("FH Google Analytis Send Event To Java");
        if (obj_GoogleAnalytic!=null)
        {
            obj_GoogleAnalytic.Call("sendEvent", category, action, label, value);
        }
#endif
    }
    #region [ Event callbacks ]
    void OnEventSendFinish(string result)
    {
        //Debug.LogWarning("FH Google Analytis Send Event Finish");
    }
    #endregion

    /// <summary>
    /// finish purchase and release memory 
    /// </summary>
    public static void Destroy()
    {
		//Debug.LogError("FH In Google Analytis destroy");
#if UNITY_EDITOR
		return;
#elif UNITY_IPHONE
		ga_stopTracker();
#elif UNITY_ANDROID
        if (obj_GoogleAnalytic != null)
        {
            obj_GoogleAnalytic.Call("stopTracker");
        }
#endif
    }

}
