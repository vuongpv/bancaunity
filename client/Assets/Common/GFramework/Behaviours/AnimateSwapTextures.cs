using UnityEngine;
using System.Collections;

public class AnimateSwapTextures : MonoBehaviour
{
	public Texture2D[] textures;
	public string textureName;

	public int fps;

	private int currentFrame;

	private float currentFrameTime;

	// Cache
	public Material material;

	void Awake()
	{
		if (string.IsNullOrEmpty(textureName))
			textureName = "_MainTex";
		if( material == null )
			material = renderer.sharedMaterial;
	}

	void Start()
	{
		material.mainTexture = textures[currentFrame];
	}

	// Update is called once per frame
	void Update () {

		currentFrameTime += Time.deltaTime;
		if (currentFrameTime >= (1f / fps))
		{
			AdvanceFrame();
			material.SetTexture(textureName, textures[currentFrame]);

			currentFrameTime = 0;
		}
	}

	void AdvanceFrame()
	{
		currentFrame++;
		if (currentFrame >= textures.Length)
			currentFrame = 0;
	}
}
