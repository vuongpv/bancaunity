using UnityEngine;
using System.Collections;

public enum SceneTransitionType
{
}

public class FishScenes
{
		public const string Intro = "Intro";

		public const string MainMenu = "MenuScreen";

		public const string Single = "FishingOffline";

		public const string Online = "FishingOnline";
}

public class SceneManager : SingletonMono<SceneManager>
{
		public TransitionManager transitionMgr;

		void Start ()
		{
				Application.LoadLevel (FishScenes.MainMenu);
        
		}

		public void LoadScene (string scene)
		{
//				FHAudioManager.instance.StopMusic ();

				Application.LoadLevel (scene);
		}

		public void LoadSceneWithLoading (string scene)
		{
//				FHAudioManager.instance.StopMusic ();
//
//				FHLoadingManager.instance.LoadToScene (scene);
		}

		public string GetCurrentScene ()
		{
				return Application.loadedLevelName;
		}

		public void BackToMM ()
		{
				FHNetworkManager.Instance.ResetSocketClient ();
				SceneManager.instance.LoadSceneWithLoading (FishScenes.MainMenu);
		}
}