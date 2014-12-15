using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class SystemModule : GMonoBehaviour
{
	/// <summary>
	/// Get dependencies
	/// </summary>
	/// <returns></returns>
	public abstract System.Type[] GetDependencies();

	/// <summary>
	/// Initialize the module
	/// </summary>
	public abstract void Init();

	/// <summary>
	/// Shutdown the module
	/// </summary>
	public abstract void Shutdown();
}

public abstract class SystemModuleSingleton<T> : SystemModule where T : SystemModule
{
	private static T singleton;

	public static T instance
	{
		get
		{
			if (SystemModuleSingleton<T>.singleton == null)
			{
				SystemModuleSingleton<T>.singleton = (T)FindObjectOfType(typeof(T));
				if (SystemModuleSingleton<T>.singleton == null)
				{
					GameObject obj = new GameObject();
					obj.name = typeof(T).Name;
					SystemModuleSingleton<T>.singleton = obj.AddComponent<T>();
				}
			}

			return SystemModuleSingleton<T>.singleton;
		}
	}
}
