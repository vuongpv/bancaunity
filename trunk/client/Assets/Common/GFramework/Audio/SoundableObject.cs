using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class SoundName
{
	public string name;
	public AudioClip clip;
}

[ExecuteInEditMode]
[RequireComponent(typeof(AudioSource))]
public class SoundableObject : MonoBehaviour {

	public List<SoundName> sounds;

	public AudioSource _audio { get; private set; }

	// One shot setting
	public float oneShotCoolDown = 0.1f;
	private float lastOneShotTime;

	public int maxOneShotsPerSecond = 5;
	private float lastOneShotCountTime;
	private float lastOneShotCount;

	void Awake()
	{
		if( sounds == null )
			sounds = new List<SoundName>();

		_audio = audio;
		if (_audio == null)
		{
			_audio = gameObject.AddComponent<AudioSource>();
			_audio.playOnAwake = false;
		}

#if UNITY_EDITOR
		foreach (var component in gameObject.GetComponents<Component>())
		{
			object[] attributes = component.GetType().GetCustomAttributes(true);
			foreach (var attr in attributes)
			{
				if (attr is SoundClipAttribute == false) 
					continue;

				string name = ((SoundClipAttribute) attr).name;
				SoundName sound = sounds.Find(s => s.name == name);
				if (sound == null)
				{
					sound = new SoundName();
					sound.name = name;
					sounds.Add(sound);
				}

			}
		}
#endif
	}

	public AudioClip GetAudioClip(string name)
	{
		SoundName sound = sounds.Find(s => s.name == name);
		if (sound == null)
			return null;

		return sound.clip;
	}

	/// <summary>
	/// Play one shot with limit options
	/// </summary>
	/// <param name="name"></param>
	/// <param name="ignorable"></param>
	public void PlayOneShot(string name, bool ignorable)
	{
		float curTime = Time.realtimeSinceStartup;
		if (ignorable)
		{
			if (curTime - lastOneShotTime < oneShotCoolDown )
				return;

			if ((int)lastOneShotCountTime == (int)curTime &&
				lastOneShotCount >= maxOneShotsPerSecond)
				return;
		}

		AudioClip clip = GetAudioClip(name);
		if( clip == null ) return;
		_audio.PlayOneShot(clip);

		if ((int)lastOneShotCountTime == (int)curTime)
		{
			lastOneShotCount++;
		}
		else
		{
			lastOneShotCountTime = curTime;
			lastOneShotCount = 1;
		}

		lastOneShotTime = curTime;

	}

	public void Play(string name)
	{
		_audio.loop = false;
		_audio.clip = GetAudioClip(name);
		_audio.Play();
	}

	public void PlayLoopSound(string name)
	{
		_audio.loop = true;
		_audio.clip = GetAudioClip(name);
		_audio.Play();
	}
}
