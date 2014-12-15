using UnityEngine;
using System.Collections;

public class UIDailyGiftItem : MonoBehaviour {

	public UISprite dayLabelBg;
	public UISprite checker;


	public void Enable(bool enable)
	{
		if( enable)
			UIHelper.EnableWidget(gameObject, 1f);
		else
			UIHelper.DisableWidget(gameObject, 0.5f);
	}

	public void SetCurrent(bool current)
	{
		dayLabelBg.enabled = current;
	}

	public void SetCollected(bool collected)
	{
		checker.enabled = collected;
	}



}
