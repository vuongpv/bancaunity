using UnityEngine;
using System.Collections;

public class FHMultiOutOfCoinFlick : MonoBehaviour {

	public UILabel label;

	// Use this for initialization
	void OnEnable () {
		StartCoroutine(Flick());
	}

	IEnumerator Flick()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.1f);
			label.alpha = 0.7f;
			yield return new WaitForSeconds(0.1f);
			label.alpha = 0.5f;
			yield return new WaitForSeconds(0.1f);
			label.alpha = 0.2f;
			yield return new WaitForSeconds(0.1f);
			label.alpha = 0.5f;
			yield return new WaitForSeconds(0.1f);
			label.alpha = 0.7f;
			yield return new WaitForSeconds(0.1f);
			label.alpha = 1f;
		}
	}
}
