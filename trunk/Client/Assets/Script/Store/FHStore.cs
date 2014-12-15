using UnityEngine;
using System.Collections;
using com.soomla.unity;

public class FHStore : MonoBehaviour {

	private static FHStoreEventHandler handler;
    /// luu y phai thay doi SoomSec rieng biet cho IOS tuy theo moi apps(neu khong thay xay ra loi cache voi app cu)
    /// bien nam o file nam o file Soomla.soomSec
	
    // Use this for initialization
	void Start()
	{
#if UNITY_IPHONE
		StoreController.Initialize(new FHStoreAssets());
		handler = new FHStoreEventHandler();
#endif
	}
}
