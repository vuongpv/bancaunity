using UnityEngine;
using System.Collections;

public class FHTextFadeOut : MonoBehaviour
{
    public float totalTime;
    public float stepTime;

	public UILabel label;
	public UISprite sprite;

    int numberSteps;
    float stepAlpha;

	public void StartEffect()
    {
        label.alpha = 1f;
        sprite.alpha = 1f;

        numberSteps = (int)(totalTime / stepTime);
        stepAlpha = 1.0f / (float)numberSteps;

		StartCoroutine(FadeOut());
	}

    IEnumerator FadeOut()
	{
        for (int step = 0; step < numberSteps; step++)
        {
            yield return new WaitForSeconds(stepTime);

            label.alpha = 1 - step * stepAlpha;
            sprite.alpha = 1 - step * stepAlpha;
        }

        gameObject.SetActiveRecursively(false);
	}
}
