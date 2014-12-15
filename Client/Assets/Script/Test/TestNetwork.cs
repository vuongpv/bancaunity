using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TestNetwork : MonoBehaviour
{

		public UIPopupList popupList;

		// Use this for initialization
		IEnumerator Start ()
		{
				yield return 0;
				FHNetworkManager.Connect (obj => {
						MC_S_Connected result = obj as MC_S_Connected;
						FHNetworkManager.SendRequestToServer (new RequestLogin (SystemInfo.deviceUniqueIdentifier), typeof(ResponseLogin), resp => {
								ResponseLogin res = resp as ResponseLogin;	
								if (res.retCode == (int)ResultCode.OK) {
										string uid = res.uid;
										Debug.LogError("is Mission: " + res.isMission);
										FHUsersManager.instance.CreatePlayerMe (uid);
								}});
				});
		
				List<string> listRequest = Enum.GetValues (typeof(RequestType)).Cast<RequestType> ().Select (v => v.ToString ()).ToList ();
		
				for (int i = 0; i < listRequest.Count; i++) {
						popupList.AddItem (listRequest [i]);
				}
		
				popupList.value = "Login";
		}
	
		public void TestRequest ()
		{
				Debug.Log ("click button");
				switch ((RequestType)Enum.Parse (typeof(RequestType), popupList.value)) {
		
				case RequestType.Login:
//						break;
//		
//				case RequestType.TestDB:
						FHNetworkManager.SendRequestToServer (new RequestLogin (SystemInfo.deviceUniqueIdentifier), typeof(ResponseLogin), resp => {
								ResponseLogin res = resp as ResponseLogin;	
								if (res.retCode == (int)ResultCode.OK) {
										string uid = res.uid;
										Debug.Log ("Login === OK: " + uid);	
										Debug.LogError("is Mission: " + res.isMission);
										FHNetworkManager.SendRequestToServer (new Request_Properties (uid), typeof(Response_Properties), respProperties => {
												Response_Properties resProperties = respProperties as Response_Properties;	
												if (resProperties.retCode == (int)ResultCode.OK) {
														Debug.Log ("================== get propetties is ss");
														Debug.Log ("============= gold: " + resProperties.properties);

												}
										});
								}
						});
						break;
			
				default:
						break;
				}
	
		
		}
}
