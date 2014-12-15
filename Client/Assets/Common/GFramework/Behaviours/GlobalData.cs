using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("GFramework/Global Data")]
public class GlobalData : GMonoBehaviour
{
	private static Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();

	// Use this for initialization
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		if (cache.ContainsKey(name))
		{
			Debug.LogWarning("Object [" + name + "] exists. Destroy new one");
			Object.DestroyImmediate(this.gameObject);
		}
		else
			cache[name] = gameObject;
	}
    void Start()
    {

    }
}
