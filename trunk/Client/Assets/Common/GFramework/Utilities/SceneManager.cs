using UnityEngine;
using System.Collections;

public enum SceneTransitionType
{
}

public class FHScenes
{
		public const string Intro = "Intro";

		public const string MainMenu = "MainMenu";

		public const string Single = "FishHuntSingle";

		public const string Multi = "FishHuntMulti";

		public const string Lan = "FishHuntLan";

		public const string Online = "FishHuntOnline";

		public const string Tables = "TablesRoom";
}

public class SceneManager : SingletonMono<SceneManager>
{
		public TransitionManager transitionMgr;

		void Start ()
		{
				FHLoadingManager.instance.LoadToScene (FHScenes.MainMenu);
		}

		public void LoadScene (string scene)
		{
				FHAudioManager.instance.StopMusic ();

				Application.LoadLevel (scene);
		}

		public void LoadSceneWithLoading (string scene)
		{
				FHAudioManager.instance.StopMusic ();

				FHLoadingManager.instance.LoadToScene (scene);
		}

		public string GetCurrentScene ()
		{
				return Application.loadedLevelName;
		}

		public void BackToMM ()
		{
				FHNetworkManager.Instance.ResetSocketClient ();
				SceneManager.instance.LoadSceneWithLoading (FHScenes.MainMenu);
		}
}