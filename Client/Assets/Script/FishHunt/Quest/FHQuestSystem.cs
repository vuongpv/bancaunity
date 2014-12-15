using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FHQuestSystem : SingletonMono<FHQuestSystem>
{
		private const int QUEST_DAILY_MAX = 3;

//    private const int QUEST_MIN_INTERVAL = 300;
//    private const int QUEST_MAX_INTERVAL = 420;

		private const int QUEST_MIN_INTERVAL = 30;
		private const int QUEST_MAX_INTERVAL = 42;
		private bool initialized = false;

		private float questInterval;

		private FHQuest activeQuest;

		private List<ConfigQuestRecord> questConfigs = new List<ConfigQuestRecord> ();

		private List<KeyValuePair<FHQuestProperty, object>> questProperties;

		private FHPlayerProfile profile;

		private System.Random randomGenerator = new System.Random ((int)DateTime.Now.Ticks & 0x0000FFFF);

		private FHQuestPanel questPanel = null;

		private bool isNoActiveQuestSaved = false;
		private bool isShowingResult = false;

		void Init ()
		{
				if (initialized)
						return;

				profile = FHPlayerProfile.instance;
        
				if (profile.lastQuestFinishedTime > DateTime.Now.Ticks)
						profile.lastQuestFinishedTime = 0;

				if (DateTime.Now.Ticks - profile.lastQuestFinishedTime > TimeSpan.TicksPerDay)
						profile.questDailyCounter = 0;

				questProperties = new List<KeyValuePair<FHQuestProperty, object>> ();

				UpdateQuestConfigs ();

				initialized = true;
		}

		void Reset ()
		{
				activeQuest = null;
				isNoActiveQuestSaved = false;
				isShowingResult = false;
        
				questProperties.Clear ();
		}

		public FHQuest GetActiveQuest ()
		{
				return activeQuest;
		}

		public void StartSystem ()
		{
				if (!initialized)
						Init ();

				if (questPanel == null) {
						questPanel = GameObject.Find ("QuestPanel").GetComponent<FHQuestPanel> ();
				}

				Reset ();

				string json = profile.activeQuest;
				Debug.LogError ("&&&&&&&&&&&&&&&&&&&&&&&&&&&& " + json);
				/**
				 * load Quest from RMS
		 		*/
				if (json != "")
						LoadQuest (json);
				else
						NextQuest ();
		}

		public void Stop ()
		{
				StopAllCoroutines ();
		}

		public void UpdateProperty (FHQuestProperty prop, object value)
		{
				if (activeQuest == null)
						return;

				questProperties.Add (new KeyValuePair<FHQuestProperty, object> (prop, value));
				activeQuest.UpdateProperties (questProperties);
				questProperties.Clear ();

				UpdateUI ();
		}

		public void IntervalUpdate (float deltaTime)
		{
				if (activeQuest != null) {
						if (activeQuest.state == FHQuestState.Expire) {
								if (profile.questDailyCounter == 0)
										profile.lastQuestFinishedTime = 0;

								HideUI ();
								NextQuest ();
						} else
            if (activeQuest.state == FHQuestState.Finish) {
								if (!isShowingResult)
										ShowResult ();
						} else {
								UpdateUI ();
								activeQuest.IntervalUpdate (deltaTime);
						}

						SaveActiveQuest ();
				} else {
						if (!isNoActiveQuestSaved) {
								isNoActiveQuestSaved = true;
								SaveActiveQuest ();
						}

						// Check daily limit
						if (profile != null) {
								if (profile.questDailyCounter >= QUEST_DAILY_MAX) {
										if (DateTime.Now.Ticks - profile.lastQuestFinishedTime > TimeSpan.TicksPerDay) {
												profile.questDailyCounter = 0;

												NextQuest ();
										}
								}
						}
				}
		}

		void NextQuest ()
		{
				Reset ();

				if (questConfigs.Count <= 0 || profile.questDailyCounter >= QUEST_DAILY_MAX) {
						HideUI ();
						return;
				}

				GetInterval ();
				StartCoroutine (_NextQuest ());
		}

		void GetInterval ()
		{
				questInterval = randomGenerator.Next (QUEST_MIN_INTERVAL, QUEST_MAX_INTERVAL + 1);
				Debug.LogError ("+++++++++++++++++++++++++++ wait quest: " + questInterval);
		}

		IEnumerator _NextQuest ()
		{

				yield return new WaitForSeconds (questInterval);

				GetAQuest ();
		}

		void GetAQuest ()
		{
				Debug.LogError ("+++++++++++++++++++++++++++ show quest");
				//random id request
				int index = randomGenerator.Next (0, questConfigs.Count);
				ConfigQuestRecord config = questConfigs [index];
        
				FHQuestType type = (FHQuestType)config.type;
				switch (type) {
				case FHQuestType.HuntFish:
						activeQuest = new FHQuest_HuntFish (config);
						break;
        
				case FHQuestType.UseGunCollectCoin:
						activeQuest = new FHQuest_UseGunCollectCoin (config);
						break;
            
				case FHQuestType.CollectCoinWithBet:
						activeQuest = new FHQuest_CollectCoinWithBet (config);
						break;
				}

				if (activeQuest != null) {
						InitUI ();

						profile.lastQuestFinishedTime = DateTime.Now.Ticks;
						SaveActiveQuest ();
				}
		}
		// using for test Quest
		private int testID = 0;
		public void TestUserQuest ()
		{
				int index = testID;
				if (testID < questConfigs.Count) {
						testID++;
				} else {
						GUIMessageDialog.Show (null, "Test Finish", "Test", FH.MessageBox.MessageBoxButtons.OK);
						testID = 0;
				}
				ConfigQuestRecord config = questConfigs [index];

				FHQuestType type = (FHQuestType)config.type;
				switch (type) {
				case FHQuestType.HuntFish:
						activeQuest = new FHQuest_HuntFish (config);
						break;

				case FHQuestType.UseGunCollectCoin:
						activeQuest = new FHQuest_UseGunCollectCoin (config);
						break;

				case FHQuestType.CollectCoinWithBet:
						activeQuest = new FHQuest_CollectCoinWithBet (config);
						break;
				}

				if (activeQuest != null) {
						InitUI ();

						profile.lastQuestFinishedTime = DateTime.Now.Ticks;
						SaveActiveQuest ();
				}
		}

		void LoadQuest (string json)
		{
				activeQuest = null;
				isNoActiveQuestSaved = false;

				try {
						Dictionary<string, object> jsonDic = MiniJSON.Json.Deserialize (json) as Dictionary<string, object>;
						FHQuestType type = (FHQuestType)((long)jsonDic ["type"]);

						switch (type) {
						case FHQuestType.HuntFish:
								activeQuest = new FHQuest_HuntFish (jsonDic);
								break;

						case FHQuestType.UseGunCollectCoin:
								activeQuest = new FHQuest_UseGunCollectCoin (jsonDic);
								break;

						case FHQuestType.CollectCoinWithBet:
								activeQuest = new FHQuest_CollectCoinWithBet (jsonDic);
								break;
						}
				} catch (System.Exception ex) {
						activeQuest = null;
				}

				if (activeQuest != null) {
						InitUI ();
            
						SaveActiveQuest ();
				} else
						NextQuest ();
		}

		void ShowResult ()
		{
				isShowingResult = true;
				questPanel.ShowResult (activeQuest.award);
		}

		void InitUI ()
		{
				questPanel.ShowContent ();

				questPanel.UpdateMessage (activeQuest.GetStatus ());
				questPanel.UpdateTime (activeQuest.GetRemainTime ());

				switch (activeQuest.type) {
				case FHQuestType.HuntFish:
						questPanel.ShowFish ((activeQuest as FHQuest_HuntFish).fishID);
						break;

				case FHQuestType.UseGunCollectCoin:
						questPanel.ShowGun ((activeQuest as FHQuest_UseGunCollectCoin).gunID);
						break;

				case FHQuestType.CollectCoinWithBet:
						questPanel.ShowMultiplier ((activeQuest as FHQuest_CollectCoinWithBet).betMultiplier);
						break;
				}
		}

		void UpdateUI ()
		{
				questPanel.UpdateMessage (activeQuest.GetStatus ());
				questPanel.UpdateTime (activeQuest.GetRemainTime ());
		}

		void HideUI ()
		{
				UIHelper.DisableWidget (questPanel.gameObject);

				GuiManager.HidePanel (GuiManager.instance.guiQuestDetail);
		}

		void SaveActiveQuest ()
		{
				if (profile == null)
						return;

				if (activeQuest != null)
						profile.activeQuest = activeQuest.GetJSON ();
				else
						profile.activeQuest = "";
		}

		public void UpdateQuestConfigs ()
		{
				questConfigs.Clear ();

				foreach (ConfigQuestRecord record in ConfigManager.configQuest.records) {
						if (record.type == (int)FHQuestType.CollectCoinWithBet) {
								ConfigLevelRecord levelConfig = ConfigManager.configLevel.GetLevel (profile.level);
								if (levelConfig != null && levelConfig.maxBet >= record.param2)
										questConfigs.Add (record);
						} else
								questConfigs.Add (record);
				}
		}

		public void ReceiveAward ()
		{
				questPanel.controller.SpawnCollectGold (activeQuest.award);

				profile.questDailyCounter++;
				NextQuest ();

				questPanel.HideResult ();
		}
}