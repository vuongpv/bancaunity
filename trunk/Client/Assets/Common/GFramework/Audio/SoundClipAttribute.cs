using UnityEngine;
using System.Collections;
using System;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class SoundClipAttribute : Attribute {

	public string name { get; set; }

	public SoundClipAttribute(string name)
	{
		this.name = name;
	}
}
