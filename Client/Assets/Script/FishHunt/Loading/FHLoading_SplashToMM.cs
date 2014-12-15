using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class FHLoading_SplashToMM : FHLoading
{
    public float paramX = 0;

    public FHLoading_SplashToMM(FHLoadingManager _manager)
        : base(_manager)
    {
        numberLoadingSteps = 12;
    }

    public override void Update(int step)
    {
		base.Update(step);

        switch (step)
        {
            case 0:
                LoadingStep_0();
                break;

            case 1:
                LoadingStep_1();
                break;

            case 2:
                LoadingStep_2();
                break;

            case 3:
                LoadingStep_3();
                break;

            case 4:
                LoadingStep_4();
                break;

            case 5:
                LoadingStep_5();
                break;

            case 6:
                LoadingStep_6();
                break;
            
            case 7:
                LoadingStep_7();
                break;

            case 8:
                LoadingStep_8();
                break;

            case 9:
                LoadingStep_9();
                break;

			case 10:
				LoadingStep_10();
				break;
		}
    }

    void LoadingStep_0()
    {
    }

    void LoadingStep_1()
    {
        ConfigManager.instance.Init1();
    }

    void LoadingStep_2()
    {
        ConfigManager.instance.Init2();
    }

    void LoadingStep_3()
    {
        ConfigManager.instance.Init3();
    }

    void LoadingStep_4()
    {
        ConfigManager.instance.Init4();
    }

    void LoadingStep_5()
    {
        // Cache HOTween for avoiding FPS decreasing when call HOTween for the firs time in game play        
        Holoville.HOTween.HOTween.EnableOverwriteManager(false);

        paramX = 0f;
        HOTween.To(this, 0f, new TweenParms()
            .Prop("paramX", 1f));

        // External services
        ExternalServices.instance.Init();

        // Load profile
        FHPlayerProfile.instance.Load(new FHProfileDataStream_Internal(3.0f));

        // Calculate bound
        FHSystem.instance.CalculateBound();

        // Load routes assets
        FHRouteManager.instance.LoadRoutesAssets();
    }

    void LoadingStep_6()
    {
        FHAudioManager.instance.LoadMusic();
    }

    void LoadingStep_7()
    {
        FHAudioManager.instance.LoadSound();
    }

    void LoadingStep_8()
    {
		FHLocalization.instance.LoadFonts();
		FHLocalization.instance.SetLocale(FHPlayerProfile.instance.language);
    }

    void LoadingStep_9()
    {
		Application.LoadLevel(FHScenes.MainMenu);
    }

	void LoadingStep_10()
	{
	}
}