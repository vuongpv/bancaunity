using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHPlayerHudPanel : MonoBehaviour {
    public UILabel nameLabel;

	public UILabel levelLabel;

    public UILabel timeLabel;

    public UISprite progressBar;

	public FHPlayerController controller;

	void Start()
	{
        UpdateUI();
	}

	void OnClick()
	{
	}

	public void UpdateUI()
	{
        levelLabel.text = controller.level.ToString();
	}

    public void UpdateXPProgress(int current, int total)
    {
        progressBar.fillAmount = (float)current / (float)total;
    }
}