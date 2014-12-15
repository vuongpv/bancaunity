using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Holoville.HOTween;

/// <summary>
/// Progressable status
/// </summary>
public enum ProgressableStatus
{
	Pending,
	Running,
	Finished,
}

/// <summary>
/// Interface
/// </summary>
public interface IProgressable
{
	float GetProgress();

	ProgressableStatus GetProgressStatus();
}

public class UIProgressPanelController : MonoBehaviour
{
	public UISlider progress;
	public UILabel percent;

	private IProgressable progressableObject;

	private ProgressableStatus curStatus;

	public Action onDownloadFinished;

	public void SetObject(IProgressable obj)
	{
		progress.sliderValue = 0;

		progressableObject = obj;
		curStatus = ProgressableStatus.Pending;

		UpdateProgress();
	}

	public void UpdateProgress()
	{
		if (progress != null && progressableObject != null)
		{
			float value = progressableObject.GetProgress();
			if (progress.sliderValue != value || curStatus != progressableObject.GetProgressStatus())
			{
				progress.sliderValue = value;
				curStatus = progressableObject.GetProgressStatus();

				percent.text = (int)((progress.sliderValue) * 100f) + "%";
				if (curStatus == ProgressableStatus.Finished)
				{
					if (onDownloadFinished != null)
						onDownloadFinished();
				}
			}
		}
	}

	public void UpdateAll()
	{
		UpdateProgress();
	}

}
