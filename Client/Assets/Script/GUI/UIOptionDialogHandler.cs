using UnityEngine;
using System.Collections;

public class UIOptionDialogHandler : UIBaseDialogHandler
{
    public GameObject soundOn;
    public GameObject soundOff;

    public GameObject musicOn;
    public GameObject musicOff;

    FHPlayerProfile profile;

    public override void OnBeginShow(object parameter)
    {
        profile = FHPlayerProfile.instance;

		if (profile.music)
			EnableMusic(true);
		else
			EnableMusic(false);

		if (profile.sound)
			EnableSound(true);
		else
			EnableSound(false);
    }

	void EnableMusic(bool enable)
	{
		musicOn.SetActiveRecursively(enable);
		musicOff.SetActiveRecursively(!enable);
	}

	void EnableSound(bool enable)
	{
		soundOn.SetActiveRecursively(enable);
		soundOff.SetActiveRecursively(!enable);
	}

	void OnClick()
    {
        switch (UICamera.selectedObject.name)
        {
            case "MusicSwitchOn":
                OnDisableMusic();
                break;

            case "MusicSwitchOff":
                OnEnableMusic();
                break;

            case "SoundSwitchOn":
                OnDisableSound();
                break;

            case "SoundSwitchOff":
                OnEnableSound();
                break;

            case "CloseBtn":
                GuiManager.HidePanel(GuiManager.instance.guiOptionDialogHandler);
                break;
        }
    }

    void OnDisableMusic()
    {
        profile.music = false;
		EnableMusic(false);
        FHAudioManager.instance.StopMusic();
    }

    void OnEnableMusic()
    {
        profile.music = true;
		EnableMusic(true);
        if (Application.loadedLevelName != FHScenes.MainMenu)
            FHAudioManager.instance.PlayMusic(FHAudioManager.MUSIC_MAIN);
    }

    void OnDisableSound()
    {
        profile.sound = false;
        NGUITools.soundVolume = 0.0f;
		EnableSound(false);
	}

    void OnEnableSound()
    {
        profile.sound = true;
        NGUITools.soundVolume = 1.0f;
		EnableSound(true);
    }
}