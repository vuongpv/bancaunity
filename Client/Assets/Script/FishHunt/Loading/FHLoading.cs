using UnityEngine;
using System.Collections;

public class FHLoading
{
    private FHLoadingManager manager;

	public int currentStep { get; private set; }

    public int numberLoadingSteps { get; protected set; }

    public FHLoading(FHLoadingManager _manager)
    {
        manager = _manager;
    }

    public virtual void Update(int step)
    {
		currentStep = step;
    }


}