using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;


public class InAppBillingBinding : MonoBehaviour
{
    private const string SKU_BUY_TYPE_01 = "type01";
    private const string SKU_BUY_TYPE_02 = "type02";
    private const string SKU_BUY_TYPE_03 = "type03";
    private const string SKU_BUY_TYPE_04 = "type04";
    private const string SKU_BUY_TYPE_05 = "type05";
    private const string SKU_BUY_TYPE_06 = "type06";
    private static string base64EncodedPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAr6/rwAb3bMlaT9Xh3dVDr9S8rO+EvhoTL8WiKXmJI37yJ96faO09X1zJHKyM8TJUS0EUan3HbitwfQs2mQyzE92A83tlhx6x222reQ4tjkI8SK/c7zUeLleMBCTceTzS1fVPiow3U9DXdrZ2cG9/bL0MAmx5u5EBvAJM5/e63GlB1cTlHfnUFA1kfQ/uPLUZFMxs9Vtdgd7ihBmDscdlvkJ2qphG0obu6IXQHJ0uuXDXyOWaM+Msm3MYV9Ya0aGC3xH+plDutO+QiYsQFjQzb2ZNBbTYxb74zjyiGNqV7tc1m4+pb15HzAotGIse590QYIc9DBUuZt/ejeNI4pUuPwIDAQAB";
#if UNITY_ANDROID
    private static AndroidJavaObject obj_Activity;
    
#endif
    private static bool isInit = false;
    private static bool isSuccessInit = true;

    public static void Init()
    {
        base64EncodedPublicKey = FHSystem.instance.KeyGameID;
        Debug.LogError("FH key:" + base64EncodedPublicKey);
        if (isInit)
        {
            return;
        }
        isInit = true;
        if (UnityEngine.Object.FindObjectOfType(typeof(InAppBillingBinding)) == null)
        {
            GameObject go = new GameObject("InAppBillingBinding");
            go.AddComponent<InAppBillingBinding>();
            DontDestroyOnLoad(go);
        }

#if UNITY_EDITOR
        Debug.LogWarning("In app Billing can not run for editor and stand alone");
		return;
#endif
#if UNITY_ANDROID
        Debug.LogError("FH In app Billing in Unity Android Initial");
		AndroidJNI.AttachCurrentThread(); 
		using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
		    obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            obj_Activity.Call("InitPayment", base64EncodedPublicKey);
            Debug.LogError("FH Unity :"+obj_Activity.GetRawClass().ToString()+","+obj_Activity.GetRawObject().ToString());
		}
#endif
    }

    /// <summary>
    /// finish purchase and release memory 
    /// </summary>
    public static void Destroy()
    {
        #if UNITY_ANDROID
        //if(obj_Activity!=null)
        //{
        //    Debug.LogError("FH In app Billing destroy payment");
        //    obj_InAppBilling.Call("OnDestroy");
        //}
        #endif
    }


    public static void BuyItem(string itemID)
    {		
#if UNITY_ANDROID
        Debug.LogError("FH In app Billing Call Buy Item");

        if (!isInit)
        {
            Init();
        }
        if(obj_Activity==null)
        {
            Debug.LogError("FH obj_Activity InAppBilling null");
            
            return;
        }
        else
        {
            Debug.LogError("FH obj_InAppBilling ok !"+obj_Activity.ToString());
        }

        if (!isSuccessInit)
        {
            GUIMessageDialog.Show(null, "You must add account for your device first", "In app purcharse");  
        }
        else
        {
            obj_Activity.Call("BuyItem", itemID);
        }
        
#endif
    }


    #region [ Event callbacks ]

    void InAppPurcharseFinish(string purchaseID)
    {
        int coinAdd = 0;
        ConfigGoldPackRecord _record=ConfigManager.configGoldPack.GetPackByID(purchaseID);
        if (_record != null)
        {
            coinAdd = _record.goldValue + _record.goldBonus;
        }
        else
        {
            coinAdd += 2000;// co the item moi chua co trong config
        }
        Debug.Log("FH Unity Finish In app Purchase:" + purchaseID+", coid add="+coinAdd);

        if (FHSystem.instance.GetCurrentPlayerMode() == FHPlayerMode.Multi)
            FHMultiPlayerManager.instance.GetMainPlayer().AddCoin(coinAdd);
        else
            FHPlayerProfile.instance.gold += coinAdd;
        
        FHPlayerProfile.instance.ForceSave();            

        if (FHGoldHudPanel.instance != null)
            FHGoldHudPanel.instance.UpdateGold();        

        FHPlayStorePayment.instance.RequestPayment(_record, null);
        FHPlayStorePayment.instance.RequestClosePayment();
        
    }
    void InAppInitFinish(string status)
    {
        if (status.Contains("-101"))
        {
            isSuccessInit = false;
            GUIMessageDialog.Show(null, "You must add account for your device first", "In app purcharse");    
        }
        else
        {
            isSuccessInit = true;
            Debug.LogError("FH Unity Init In App billing OK");
        }
    }

    #endregion
}