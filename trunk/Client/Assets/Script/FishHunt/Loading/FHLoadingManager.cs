using UnityEngine;
using System.Collections;

public class FHLoadingManager : SingletonMono<FHLoadingManager>, TransitionActionDelegate, IProgressable
{
		private FHLoading currentLoading = null;

		private int step = -1;

		private string loadScene;
		private bool endLoading;

		private float stepInterval;

		public UIProgressPanelController progressPanel;

		void OnLevelWasLoaded (int level)
		{
				Debug.Log ("Loaded level " + level);
		}

		public void LoadToScene (string scene)
		{
				loadScene = scene;

				endLoading = false;
				TransitionManager.Instance.DoAction (this, true);
		}

		private void _LoadToScene ()
		{
				currentLoading = null;

				switch (loadScene) {
				case FHScenes.MainMenu:
						if (Application.loadedLevelName == FHScenes.Intro)
								currentLoading = new FHLoading_SplashToMM (this);
						else
								currentLoading = new FHLoading_GPToMM (this);
						break;

				case FHScenes.Single:
						currentLoading = new FHLoading_MMToSingle (this);
						break;

				case FHScenes.Multi:
						currentLoading = new FHLoading_MMToMulti (this);
						break;

				case FHScenes.Online:
						currentLoading = new FHLoading_MMToOnline (this);
						break;
				case FHScenes.Tables:
						currentLoading = new FHLoading_Table (this);
						break;
				}

				// Start loading
				if (currentLoading != null) {
						step = 0;
						progressPanel.UpdateAll ();	
				}
		}
    
		void ResetLoading ()
		{
				currentLoading = null;
				step = -1;
		}

		void Update ()
		{
				if (currentLoading == null)
						return;

				if (step < 0)
						return;

				if (step >= currentLoading.numberLoadingSteps) {
						ResetLoading ();
						endLoading = true;
						loadScene = null;
						return;
				}

				if (currentLoading.currentStep < step)
						currentLoading.Update (step);

				step++;

				progressPanel.UpdateAll ();
		}

		void OnGUI ()
		{
				if (currentLoading == null)
						return;

				if (step < 0 || step >= currentLoading.numberLoadingSteps)
						return;

				int percent = (step + 1) * 100 / currentLoading.numberLoadingSteps;

				/*GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
        GUILayout.Label("Loading " + " " + percent.ToString() + "%");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();*/
		}


	#region Interface TransitionActionDelegate

		public void Begin ()
		{
				Debug.Log (">>> Load scene action: " + loadScene);
				if (!string.IsNullOrEmpty (loadScene)) {
						if (progressPanel != null) {
								progressPanel.gameObject.SetActiveRecursively (true);
								progressPanel.SetObject (this);
						}
						_LoadToScene ();
				}
		}

		public bool IsFinished ()
		{
				if (string.IsNullOrEmpty (loadScene))
						return true;

				return endLoading;
		
		}

		public void End ()
		{
				if (progressPanel) {
						progressPanel.SetObject (null);
						progressPanel.gameObject.SetActiveRecursively (false);
				}


		}

	#endregion

	#region Interface IProgressable

		public float GetProgress ()
		{
				if (step < 0)
						return 0;

				if (step >= currentLoading.numberLoadingSteps)
						return 1;

				return (step + 1) / (float)currentLoading.numberLoadingSteps;
		}

		public ProgressableStatus GetProgressStatus ()
		{
				if (step < 0)
						return ProgressableStatus.Pending;

				if (step >= currentLoading.numberLoadingSteps)
						return ProgressableStatus.Finished;

				return ProgressableStatus.Running;
		}

	#endregion
}
