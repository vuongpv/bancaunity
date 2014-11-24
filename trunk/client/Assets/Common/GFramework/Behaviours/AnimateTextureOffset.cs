using UnityEngine;
using System.Collections;

public class AnimateTextureOffset : MonoBehaviour {

	public Vector2 speed;

	// Cache
	public Material _material { get; private set; }

	void Awake()
	{
		_material = renderer.sharedMaterial;
	}

	// Update is called once per frame
	void Update () {
		Vector2 offset = _material.GetTextureOffset("_MainTex");
		offset += speed * Time.deltaTime;
		_material.SetTextureOffset("_MainTex", offset);
	}
}
