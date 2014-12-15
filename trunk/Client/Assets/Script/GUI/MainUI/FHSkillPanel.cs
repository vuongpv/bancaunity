using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public class FHSkillPanel : MonoBehaviour, ICollectibleTarget
{
	public FHPlayerController controller;

    public GameObject skillBtn;

    public GameObject lightning;
    public UILabel lightningCounter;

    public GameObject nuke;
    public UILabel nukeCounter;

    public GameObject powerup;

	private Tweener lightningOverTween;
	private Tweener nukeOverTween;
    private Tweener skillOverTween;
	private TweenParms skillOverTweenParam;

	void Awake()
	{
		skillOverTweenParam = new TweenParms()
			.Prop("localScale", new Vector3(1f, 1f, 1f))
			.OnComplete(() =>
			{
			
			});
	}

    void OnClick()
	{
        bool choosed = false;

		switch (UICamera.selectedObject.name)
		{
			case "Powerup":
                controller.SetCurrentGun(FHGameConstant.POWERUP_GUN);
                choosed = true;
				break;

			case "Nuke":
                controller.SetCurrentGun(FHGameConstant.NUKE_GUN);
                choosed = true;
				break;

            case "Lightning":
                controller.SetCurrentGun(FHGameConstant.LIGHTNING_GUN);
                choosed = true;
                break;
        }

        if (choosed)
        {
            controller.gunHudPanel.DisableBet();
            if (controller is FHPlayerMultiController)
                ((FHPlayerMultiController)controller).circleMenu.Toggle();
        }
	}
    
    public void SetPowerupGun(bool enable )
    {
        if (enable)
            UIHelper.EnableWidget(powerup);
        else
            UIHelper.DisableWidget(powerup);
    }

    public void SetLightningGun(int counter)
    {
        if (counter == 0)
            UIHelper.DisableWidget(lightning);
        else
        {
            UIHelper.EnableWidget(lightning);
            lightningCounter.text = counter.ToString();
        }
    }

    public void SetNukeGun(int counter)
    {
        if (counter == 0)
            UIHelper.DisableWidget(nuke);
        else
        {
            UIHelper.EnableWidget(nuke);
            nukeCounter.text = counter.ToString();
        }
    }

	public Vector3 GetTargetPos(string type)
	{
        if (FHSystem.instance.GetCurrentPlayerMode() == FHPlayerMode.Multi && !((FHPlayerMultiController)controller).circleMenu.IsShowed())
            return skillBtn.transform.position;

		if (type == "lightning")
			return lightning.transform.position;

		if (type == "nuke")
			return nuke.transform.position;

		return transform.position;
	}

	public void OnReachTarget(string type)
	{
        if (FHSystem.instance.GetCurrentPlayerMode() == FHPlayerMode.Multi && !((FHPlayerMultiController)controller).circleMenu.IsShowed())
        {
            if (skillOverTween != null)
                HOTween.Kill(skillOverTween);

            skillBtn.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            skillOverTween = HOTween.Punch(skillBtn.transform, 0.5f, skillOverTweenParam, 0.5f, 0.2f);

            if (type == "lightning")
                SetLightningGun(controller.lightning);
            else
            if (type == "nuke")
                SetNukeGun(controller.nuke);

            return;
        }

		if (type == "lightning")
		{
			if (lightningOverTween != null)
				HOTween.Kill(lightningOverTween);

			lightning.transform.localScale = new Vector3(0.8f, 0.8f, 1);
			lightningOverTween = HOTween.Punch(lightning.transform, 0.5f, skillOverTweenParam, 0.5f, 0.2f);

            SetLightningGun(controller.lightning);
		}
		else if (type == "nuke")
		{
			if (nukeOverTween != null)
				HOTween.Kill(nukeOverTween);

			nuke.transform.localScale = new Vector3(0.8f, 0.8f, 1);
			nukeOverTween = HOTween.Punch(nuke.transform, 0.5f, skillOverTweenParam, 0.5f, 0.2f);

            SetNukeGun(controller.nuke);
		}
	}

    public void EnableSkillButton(bool enable)
    {
        skillBtn.collider.enabled = enable;
        skillBtn.GetComponentInChildren<UISprite>().spriteName = (enable ? "multi_showskill_btn" : "multi_showskill_btn_dis");
    }
}