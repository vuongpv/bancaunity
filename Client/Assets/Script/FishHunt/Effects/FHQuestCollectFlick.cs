using UnityEngine;
using System.Collections;

public class FHQuestCollectFlick : MonoBehaviour {

	public UILabel label;

	public UISprite sprite;

	// Use this for initialization
	void OnEnable () {
		StartCoroutine(Flick());
	}

	IEnumerator Flick()
	{
		while (true)
		{
            yield return new WaitForSeconds(0.1f);
            label.alpha = 0.9f;
            sprite.alpha = 0.9f;

            yield return new WaitForSeconds(0.1f);
            label.alpha = 0.8f;
            sprite.alpha = 0.8f;

            yield return new WaitForSeconds(0.1f);
            label.alpha = 0.7f;
            sprite.alpha = 0.7f;

            yield return new WaitForSeconds(0.1f);
            label.alpha = 0.6f;
            sprite.alpha = 0.6f;

            yield return new WaitForSeconds(0.1f);
            label.alpha = 0.5f;
            sprite.alpha = 0.5f;

            yield return new WaitForSeconds(0.1f);
            label.alpha = 0.4f;
            sprite.alpha = 0.4f;

            yield return new WaitForSeconds(0.1f);
            label.alpha = 0.3f;
            sprite.alpha = 0.3f;

            yield return new WaitForSeconds(0.1f);
            label.alpha = 0.2f;
            sprite.alpha = 0.2f;

            yield return new WaitForSeconds(0.1f);
            label.alpha = 0.1f;
            sprite.alpha = 0.1f;

            yield return new WaitForSeconds(0.1f);
            label.alpha = 0.0f;
            sprite.alpha = 0.0f;

            yield return new WaitForSeconds(0.1f);
            label.alpha = 1.0f;
            sprite.alpha = 1.0f;
        }
	}
}
