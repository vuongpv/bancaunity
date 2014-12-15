using UnityEngine;
using System.Collections;
using System;

public class FHActiveGoldController : MonoBehaviour {

	// Remain countdown time
	private TimeSpan remainTime;

	// Sum the time to 1 second elapse
	private float sumTime;

	public UILabel label;
	public UISprite bg;

	void Awake()
	{
		if (label == null)
			label = GetComponent<UILabel>();
	}

	// Use this for initialization
	void Start () {
		remainTime = new TimeSpan(0, 1, 0);
	}

	void Update()
	{
        if (bg == null)
        {
            return; 
        }
		if (FHPlayerProfile.instance.gold >= 200)
		{
			bg.enabled = false;
			label.enabled = false;
			return;
		}
		else if( label.enabled == false )
		{
			remainTime = new TimeSpan(0, 1, 0);
			label.enabled = true;
			bg.enabled = true;
		}
		
		if (Time.deltaTime / Time.timeScale > 1f)
		{
			sumTime += Time.deltaTime;
		}
		else
		{
			sumTime += Time.deltaTime / Time.timeScale;
		}
		if (sumTime > 1)
		{
			remainTime = remainTime.Subtract(new TimeSpan(0, 0, 1));
			if (remainTime.TotalSeconds <= 0)
			{
				FHGuiCollectibleManager.instance.SpawnUICoinText(transform.position + new Vector3(0, 0.01f, 0), FHDefines.ACTIVE_GOLD_PER_MINUTE);

				FHPlayerProfile.instance.gold += FHDefines.ACTIVE_GOLD_PER_MINUTE;
				FHPlayerProfile.instance.ForceSave();

                if (FHGoldHudPanel.instance != null)
                    FHGoldHudPanel.instance.UpdateGold();

				remainTime = new TimeSpan(0, 1, 0);
			}

			UpdateText();
			sumTime = 0;
		}
	}

	void UpdateText()
	{
		label.text = string.Format("{1:00}", remainTime.Minutes, remainTime.Seconds);
	}
}
