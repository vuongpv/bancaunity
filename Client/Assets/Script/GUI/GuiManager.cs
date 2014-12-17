//#define QUESTJOB
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework;

public class GuiManager : SingletonMono<GuiManager>
{
		private GameObject guiSystem;
		public Camera uiCamera;

		// list GUI Dialog
		public GUIDialogBase guiAchievesHandler = null;
		public GUIMessageDialog guiMessageDialogHandler = null;
		public GUIDialogBase guiOptionDialogHandler = null;
		public GUIDialogBase guiMainSingleHandler = null;
		public GUIDialogBase guiJoinRoomHandler = null;
		public GUIDialogBase guiLanMultiPlayer = null;
		public GUIDialogBase guiShopHandler = null;
		public GUIDialogBase guiGiftCode = null;
		public GUIDialogBase guiDailyGift = null;
		public GUIDialogBase guiOnlinePlay = null;
		public GUIDialogBase guiHelp = null;
		public GUIDialogBase guiLevelUp = null;
		public GUIDialogBase guiScore = null;
		public GUIDialogBase guiQuestDetail = null;
		public GUIDialogBase guiMultiSummary = null;
		public GUIDialogBase guiRating = null;
		public GUIDialogBase guiCardExchange = null;
		public GUIDialogBase guiInforFishs = null;
		public GUIDialogBase guiRanking = null;
		public GUIDialogBase guiChat = null;
		public GUIDialogBase guiMission = null;
		public GUIDialogBase guiShop = null;


		public GameObject blackBorder;


		public Camera GetUICamera ()
		{
				return uiCamera;
		}

		// Use this for initialization
		void Start ()
		{
		}

		// Update is called once per frame
		void Update ()
		{
		}

		public void CloseAllWindow ()
		{
        
		}

		public void InitMainGui (bool isSingle)
		{
				if (isSingle) {
						GuiManager.ShowPanel (GuiManager.instance.guiMainSingleHandler);
				}

		}

		public static void ShowPanel (GUIDialogBase panelController)
		{
//				blackBorder.SetActiveRecursively (true);
				ShowPanel (panelController, null);
		}

		public static void ShowPanel (GUIDialogBase panelController, object parameter)
		{
				if (!panelController.TryShow (parameter)) {
						//Debug.LogError("Error:" + panelController.name);
				}
		}

		public static void HidePanelAfterTime (GUIDialogBase panelController, float _time)
		{
				GuiManager.instance.StartCoroutine (GuiManager.instance.CoroutineHidePanel (panelController, _time));
		}
		private IEnumerator CoroutineHidePanel (GUIDialogBase panelController, float _time)
		{
				yield return new WaitForSeconds (_time);
				HidePanel (panelController);
		}
		public static void HidePanel (GUIDialogBase panelController)
		{
				HidePanel (panelController, null);
		}

		public static void HidePanel (GUIDialogBase panelController, object parameter)
		{
//				blackBorder.SetActiveRecursively (false);
				panelController.Hide (parameter);
		}

		public void CheckShowBorder ()
		{
				GUIDialogBase topShow = null;
				GUIDialogBase[] controlers = gameObject.GetComponentsInChildren<GUIDialogBase> ();

				for (int i = 0; i < controlers.Length; i++) {
						if (controlers [i].useBlackBolder) {
								if (controlers [i].status == GUIDialogBase.GUIPanelStatus.Showed
										|| controlers [i].status == GUIDialogBase.GUIPanelStatus.Showing) {
										if (topShow == null) {
												topShow = controlers [i];
										} else {
												if (topShow.guiControlLocation != null && controlers [i].guiControlLocation != null) {
														if (topShow.guiControlLocation.transform.position.z > controlers [i].guiControlLocation.transform.position.z) {
																topShow = controlers [i];
														}
												}
										}
								}
						}
				}
				if (topShow != null && topShow.guiControlLocation != null) {
						Vector3 pos = topShow.guiControlLocation.transform.position;
						blackBorder.SetActiveRecursively (true);
						Vector3 pos2 = blackBorder.transform.position;
						pos2.z = pos.z;
						blackBorder.transform.position = pos2;
						pos2 = blackBorder.transform.localPosition;
						pos2.z += 2;
						blackBorder.transform.localPosition = pos2;
						blackBorder.GetComponent<UIWidget> ().depth = topShow.layer + 50;
				} else {
						blackBorder.SetActiveRecursively (false);
				}
		}
}