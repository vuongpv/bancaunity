using UnityEngine;
using System.Collections;

public class AndroidKeyHandler : MonoBehaviour {

	bool isShowingExitConfirmDialog;

	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Escape))
		{
			OnBackButton();
		}
		else if (Input.GetKey(KeyCode.Menu))
		{
			OnMenuButton();
		}
	}

	protected virtual void OnBackButton()
	{
		if (Application.loadedLevelName == FHScenes.MainMenu)
		{
			if (!isShowingExitConfirmDialog)
			{
				NativeDialogBinding.ShowAlertDialog2("Exit confirm", "Do you want to exit?", false, "Exit", "OnExitButton", "Cancel", "OnCancelButton", gameObject.name);
				isShowingExitConfirmDialog = true;
			}
		}
		else
		{
			FHSettingPanel.instance.OnPressBackBtn(true);
		}
	}

	void OnExitButton()
	{
		Application.Quit();
	}

	void OnCancelButton()
	{
		isShowingExitConfirmDialog = false;
	}

	protected virtual void OnMenuButton()
	{

	}
}
