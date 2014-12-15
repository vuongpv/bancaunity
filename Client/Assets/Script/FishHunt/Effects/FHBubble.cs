using UnityEngine;
using System.Collections;

public class FHBubble : MonoBehaviour {

	public float minDelay = 0.5f;
	public float maxDelay = 2f;

	private float curDelay;

	public ParticleSystem particle;

	// Use this for initialization
	void Awake () {
		curDelay = Random.Range(minDelay, maxDelay);

		if (particle == null)
			particle = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
	
		curDelay -= Time.deltaTime;
		if (curDelay < 0)
		{
			particle.Play();
			curDelay = Random.Range(minDelay, maxDelay);
		}
	}
}
