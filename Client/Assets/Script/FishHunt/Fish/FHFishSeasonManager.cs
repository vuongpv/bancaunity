using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHFishSeasonManager : SingletonMono<FHFishSeasonManager>
{
		public bool canStart = false;

		List<ConfigSeasonRecord> listSeason;
		List<int> listNormalSeasonID, listSpecialSeasonID;

		private FHFishSeason currentSeason;

		private int currentSeasonID;

		private string seasonOnlineName = "";
		public string SeasonOnlineName {
				get { return seasonOnlineName; }
				set { seasonOnlineName = value; }
		}

		private Dictionary<string, FHFishSeason> seasons = new Dictionary<string, FHFishSeason> ();

		void Start ()
		{
				foreach (FHFishSeason child in GetComponentsInChildren<FHFishSeason>(false))
						GameObject.Destroy (child.gameObject);

				listSeason = ConfigManager.configSeason.GetAllSeason ();
				listNormalSeasonID = new List<int> ();
				listSpecialSeasonID = new List<int> ();

				if (listSeason != null) {
						for (int i = 0; i < listSeason.Count; i++) {
								if (!listSeason [i].scenes.Contains (Application.loadedLevelName))
										continue;

								if (IsSpecialSeason (listSeason [i].id)) {
										listSpecialSeasonID.Add (i);
								} else
										listNormalSeasonID.Add (i);

								string prefabName = listSeason [i].name;
								GameObject obj = GameObject.Instantiate (Resources.Load ("Prefabs/Others/season")) as GameObject;
								obj.transform.parent = this.transform;
								obj.name = prefabName;

								FHFishSeason season = obj.GetComponent<FHFishSeason> ();
								seasons [prefabName] = season;

								season.Setup (this, listSeason [i]);
						}
				}

				StartCoroutine (CheckStartSeason ());
				// tam thoi khong choi lan
				//if (!FHLanNetwork.instance.isConnect())
				//{
				//    StartCoroutine(CheckStartSeason());
				//}
		}

		IEnumerator CheckStartSeason ()
		{
				while (!canStart) {
						yield return new WaitForSeconds (0.1f);
				}

				SetSeasonStart ();
		}

		public void SetSeasonStart ()
		{
				FHAudioManager.instance.PlayMusic (FHAudioManager.MUSIC_MAIN);

//				if (FHSystem.instance.GetCurrentPlayerMode () == FHPlayerMode.Single)
//						FHQuestSystem.instance.StartSystem ();

				currentSeasonID = -1;
				currentSeason = null;

				currentSeasonID = NextSeason ();
				SetSeason (listSeason [currentSeasonID].name);
		}

		void Update ()
		{
				if (currentSeason == null)
						return;


				if (currentSeason.finished) {
						currentSeasonID = NextSeason ();

						SetSeason (listSeason [currentSeasonID].name);
				} else
						currentSeason.OnSeasonUpdate ();
		}

		void SetSeason (string name)
		{
				if (currentSeason != null)
						currentSeason.OnSeasonEnd ();

				if (!seasons.ContainsKey (name))
						return;

				currentSeason = seasons [name];

				if (currentSeason != null)
						currentSeason.OnSeasonStart ();
		}

		void SetSeason (FHFishSeason season)
		{
				if (currentSeason != null)
						currentSeason.OnSeasonEnd ();

				currentSeason = season;

				if (currentSeason != null)
						currentSeason.OnSeasonStart ();
		}
    
		public FHFishSeason GetCurrentSeason ()
		{
				return currentSeason;
		}

		int NextSeason ()
		{
				List<int> listSeasonID = null;

				if (Application.loadedLevelName == FHScenes.Online)
						listSeasonID = listNormalSeasonID;
				else {
						if (currentSeasonID == -1 || IsSpecialSeason (listSeason [currentSeasonID].id) || listSpecialSeasonID.Count <= 0) {
								listSeasonID = listNormalSeasonID;
						} else {
								listSeasonID = listSpecialSeasonID;
								
						}
				}

				if (listSeasonID.Count <= 0)
						return 0;
				else
						return listSeasonID [FHSystem.instance.randomGenerator.Next (listSeasonID.Count)];
		}

		public IEnumerator PlayFireWorkFishs (float time)
		{
				yield return new WaitForSeconds (time);
				currentSeasonID = 2;
				SetSeason (listSeason [currentSeasonID].name);
		}

		bool IsSpecialSeason (int seasonID)
		{
				return (seasonID > 100);
		}
}