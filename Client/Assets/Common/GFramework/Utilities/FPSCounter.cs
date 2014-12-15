using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour {

	public float frequency = 0.5f;

	public int fps;

	// Use this for initialization
	void Start () {
		StartCoroutine(CountFPS());
	}

	private IEnumerator CountFPS()
	{
		for (; ; )
		{
			int lastFrameCount = Time.frameCount;
			float lastTime = Time.realtimeSinceStartup;
			yield return new WaitForSeconds(frequency);
			float timeSpan = Time.realtimeSinceStartup - lastTime;
			int frameCount = Time.frameCount - lastFrameCount;

			fps = Mathf.RoundToInt(frameCount / timeSpan);
		}
	}
}
