using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHSettingPanel : MonoBehaviour
{
		public float containerStartY;
		public float containerFinishY;
		public float containerScrollingSpeed;

		public Transform container;
		public Transform arrow;
		public GameObject outerArea;

		Vector3 direction = Vector3.zero;
		bool isScrolling = false;
		bool isShowing = false;
	
		public static FHSettingPanel instance;

		bool enterOnPressBackBtn = false;

		void Awake ()
		{
				if (instance == null)
						instance = this;
		}

		void OnDestroy ()
		{
				if (instance = this)
						instance = null;
		}

		void Start ()
		{
				container.localPosition = new Vector3 (container.localPosition.x, containerStartY, container.localPosition.z);
				UIHelper.DisableWidget (outerArea);
		}
		void OnClick ()
		{
				switch (UICamera.selectedObject.name) {
				case "GiftCodeBtn":
						if (Application.loadedLevelName == FHScenes.Online) {
								GuiManager.ShowPanel (GuiManager.instance.guiMission);
						} else {
								GuiManager.ShowPanel (GuiManager.instance.guiGiftCode);
						}
						Hide ();
						break;

				case "SettingBtn":
						arrow.eulerAngles = new Vector3 (arrow.eulerAngles.x, arrow.eulerAngles.y, 180.0f - arrow.eulerAngles.z);

						if (!isScrolling) {
								if (isShowing)
										direction = -Vector3.up;
								else
										direction = Vector3.up;

								isScrolling = true;
						} else
								direction = -direction;
						break;

				case "BackBtn":
						OnPressBackBtn (false);
						break;

				case "HelpBtn":
						if (Application.loadedLevelName == FHScenes.Online) {
								GuiManager.ShowPanel (GuiManager.instance.guiChat);
						} else
								GuiManager.ShowPanel (GuiManager.instance.guiHelp);
						Hide ();
						break;

				case "OptionBtn":
						if (Application.loadedLevelName == FHScenes.Online) {
								GuiManager.ShowPanel (GuiManager.instance.guiRanking);

						} else {
								GuiManager.ShowPanel (GuiManager.instance.guiOptionDialogHandler);
						}
						Hide ();
						break;

				case "OuterArea":
						Hide ();
						break;
				}
		}

		void Hide ()
		{
				UIHelper.DisableWidget (outerArea);
				arrow.eulerAngles = new Vector3 (arrow.eulerAngles.x, arrow.eulerAngles.y, 180.0f - arrow.eulerAngles.z);

				if (!isScrolling) {
						direction = -Vector3.up;
						isScrolling = true;
				} else
						direction = -Vector3.up;
		}

		void Update ()
		{
				if (!isScrolling)
						return;

				container.localPosition += direction * containerScrollingSpeed * Time.deltaTime;

				if (container.localPosition.y >= containerFinishY) {
						container.localPosition = new Vector3 (container.localPosition.x, containerFinishY, container.localPosition.z);

						isShowing = true;
						isScrolling = false;
						direction = Vector3.zero;
						UIHelper.EnableWidget (outerArea);
				} else
        if (container.localPosition.y <= containerStartY) {
						container.localPosition = new Vector3 (container.localPosition.x, containerStartY, container.localPosition.z);

						isShowing = false;
						isScrolling = false;
						direction = Vector3.zero;
						UIHelper.DisableWidget (outerArea);
				}
		}

		public void OnPressBackBtn (bool prompt)
		{
				if (enterOnPressBackBtn)
						return;

				enterOnPressBackBtn = true;

				if (FHSystem.instance.GetCurrentPlayerMode () == FHPlayerMode.Multi)
						GUIMessageDialog.Show (OnBackToMMResult, FHLocalization.instance.GetString (FHStringConst.CONFIRM_BACK_TO_MM_QUESTION), FHLocalization.instance.GetString (FHStringConst.CONFIRM_BACK_TO_MM_TITLE), FH.MessageBox.MessageBoxButtons.YesNo);
				else {
						if (prompt)
								GUIMessageDialog.Show (OnBackToMMResult, FHLocalization.instance.GetString (FHStringConst.CONFIRM_BACK_TO_MM_QUESTION), "MENU", FH.MessageBox.MessageBoxButtons.YesNo);
						else {
								SceneManager.instance.BackToMM ();
								enterOnPressBackBtn = false;
						}
				}

				Hide ();
		}

		bool OnBackToMMResult (FH.MessageBox.DialogResult result)
		{
				enterOnPressBackBtn = false;

				if (result == FH.MessageBox.DialogResult.Yes || result == FH.MessageBox.DialogResult.Ok) {
						if (FHSystem.instance.GetCurrentPlayerMode () == FHPlayerMode.Multi)
								GuiManager.ShowPanel (GuiManager.instance.guiMultiSummary);
						else
								SceneManager.instance.BackToMM ();
				}

				return true;
		}
		public void TestQuest ()
		{
//				FHQuestSystem.instance.TestUserQuest ();
		}
}