using System;
using System.Collections.Generic;
using UnityEngine;

public enum FHAudioType
{
    Sound = 0,
    Music = 1,
}

public class FHAudioManager : SingletonMono<FHAudioManager>
{
    const string AUDIO_PREFAB_PATH = "Prefabs/Audio/";

    public const string MUSIC_MAIN = "music_main";
    public const string SOUND_SWITCH = "sound_switch";
    public const string SOUND_SWITCH_SPECIAL = "sound_switch_special";
    public const string SOUND_COIN01 = "sound_coin01";
    public const string SOUND_COIN02 = "sound_coin02";
    public const string SOUND_FIRE01 = "sound_fire01";
    public const string SOUND_FIRE02 = "sound_fire02";
    public const string SOUND_FIRE03 = "sound_fire03";
    public const string SOUND_FIRE_LASER = "sound_fire_laser";
    public const string SOUND_FIRE_NUKE = "sound_fire_nuke";
    public const string SOUND_FIRE_POWERUP = "sound_fire_powerup";
    public const string SOUND_HIT01 = "sound_hit01";
    public const string SOUND_HIT_NUKE = "sound_hit_nuke";
    public const string SOUND_ENABLE_POWERUP = "sound_enable_powerup";
    public const string SOUND_LEVELUP = "sound_levelup";

    Dictionary<string, AudioSource> soundPool = new Dictionary<string, AudioSource>();
    Dictionary<string, AudioSource> musicPool = new Dictionary<string, AudioSource>();

    public void LoadMusic()
    {
        Load(FHAudioType.Music, MUSIC_MAIN);
    }

    public void LoadSound()
    {
        Load(FHAudioType.Sound, SOUND_SWITCH);
        Load(FHAudioType.Sound, SOUND_SWITCH_SPECIAL);
        Load(FHAudioType.Sound, SOUND_COIN01);
        Load(FHAudioType.Sound, SOUND_COIN02);
        Load(FHAudioType.Sound, SOUND_FIRE01);
        Load(FHAudioType.Sound, SOUND_FIRE02);
        Load(FHAudioType.Sound, SOUND_FIRE03);
        Load(FHAudioType.Sound, SOUND_FIRE_LASER);
        Load(FHAudioType.Sound, SOUND_FIRE_NUKE);
        Load(FHAudioType.Sound, SOUND_FIRE_POWERUP);
        Load(FHAudioType.Sound, SOUND_HIT01);
        Load(FHAudioType.Sound, SOUND_HIT_NUKE);
        Load(FHAudioType.Sound, SOUND_ENABLE_POWERUP);
        Load(FHAudioType.Sound, SOUND_LEVELUP);
    }

    void Load(FHAudioType type, string audioName)
    {
        AudioSource source = ((GameObject)GameObject.Instantiate(Resources.Load(AUDIO_PREFAB_PATH + audioName, typeof(GameObject)))).GetComponent<AudioSource>();
        source.gameObject.transform.parent = gameObject.transform;

        if (type == FHAudioType.Sound)
            soundPool[audioName] = source;
        else
            musicPool[audioName] = source;
    }

    public void PlaySound(string audioName)
    {
        if (!FHPlayerProfile.instance.sound)
            return;
        AudioSource audio = null;
        if (soundPool.TryGetValue(audioName,out audio))
        {
            audio.PlayOneShot(soundPool[audioName].clip);
        }
    }

    public void PlayMusic(string audioName)
    {
        PlayMusic(audioName, true);
    }

    public void PlayMusic(string audioName, bool loop)
    {
        if (!FHPlayerProfile.instance.music)
            return;

        if (musicPool[audioName].isPlaying)
            musicPool[audioName].Stop();

        musicPool[audioName].loop = loop;
        musicPool[audioName].Play();
    }

    public void StopMusic(string audioName)
    {
        if (musicPool[audioName].isPlaying)
            musicPool[audioName].Stop();
    }

    public void StopMusic()
    {
        foreach (KeyValuePair<string, AudioSource> source in musicPool)
            StopMusic(source.Key);
    }

    public void PlaySoundCoin()
    {
        if (!FHPlayerProfile.instance.sound)
            return;

        AudioSource source = null;

        if (!soundPool[SOUND_COIN01].isPlaying)
            source = soundPool[SOUND_COIN01];
        else
        if (!soundPool[SOUND_COIN02].isPlaying)
            source = soundPool[SOUND_COIN02];

        if (source != null)
            source.Play();
    }
}