using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class FHLoading_GPToMM : FHLoading
{
    public FHLoading_GPToMM(FHLoadingManager _manager)
        : base(_manager)
    {
        numberLoadingSteps = 3;
    }

    public override void Update(int step)
    {
		base.Update(step);

        switch (step)
        {
            case 1:
                LoadingStep_1();
                break;
        }
    }

    void LoadingStep_1()
    {
        Application.LoadLevel(FHScenes.MainMenu);
    }
}