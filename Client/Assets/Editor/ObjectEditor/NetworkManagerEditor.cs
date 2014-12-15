using UnityEditor;
using UnityEngine;
using System.Collections;
[CustomEditor(typeof(FHNetworkManager))]
public class NetworkManagerEditor : Editor
{
	
		public override void OnInspectorGUI ()
		{
	
				base.OnInspectorGUI ();

				FHNetworkManager myTarget = target as FHNetworkManager;

				myTarget.webServerUrl = "127.0.0.1:3000";
				myTarget.assetServerUrl = "203.162.76.28:8888";

				switch (myTarget.serverType) {
				case FHNetworkManager.ServerType.LOCAL:
						myTarget.gameServerUrl = "127.0.0.1:3000";
						myTarget.webServerUrl = "127.0.0.1";
						break;
//		case AvNetworkManager.ServerType.DEV_JP_60:
//			myTarget.gameServerUrl = "203.162.76.60:7075";
//			myTarget.webServerUrl = "203.162.76.60:82/avatar_backend";
//			myTarget.assetServerUrl = "203.162.76.60:8888";
//			break;
				case FHNetworkManager.ServerType.HIEP:
						myTarget.gameServerUrl = "192.168.1.212:3000";
						myTarget.assetServerUrl = "203.162.76.28:8888";
						myTarget.webServerUrl = "192.168.1.212";
						break;

				case FHNetworkManager.ServerType.DIEU:
						myTarget.gameServerUrl = "192.168.1.215:3000";
						myTarget.assetServerUrl = "203.162.76.28:8888";
						myTarget.webServerUrl = "192.168.1.212";
						break;
	
				}
		}
}