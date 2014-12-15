using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHProfileGroup
{
		public const string BASIC = "basic";
		public const string SETTINGS = "settings";
		public const string QUEST = "quest";
		public const string ITEM = "item";
		public const string STATISTIC = "statistic";
}

public class FHProfileProperty
{
		// Basic
		public const string GOLD = "gold";
		public const string DIAMOND = "diamond";
		public const string LEVEL = "level";
		public const string XP = "xp";
		public const string LAST_TIME_SHOW_RATING = "rating";
		public const string TARGET_APP = "targetApp";
		public const string DATA_RESTORED = "dataRestored";

		// Settings
		public const string MUSIC = "music";
		public const string SOUND = "sound";
		public const string LANGUAGE = "lang";

		// Quest
		public const string LAST_QUEST_FINISHED_TIME = "lastQuestFinishedTime";
		public const string QUEST_DAILY_COUNTER = "questDailyCounter";
		public const string ACTIVE_QUEST = "activeQuest";

		// Item
		public const string POWER_UP = "powerUp";
		public const string LIGHTNING = "lightning";
		public const string NUKE = "nuke";

		// Statistic
		public const string STAT_FIRST_PAY = "firstPay";
		public const string LAST_TIME_SHOW_MULTI_TUT = "multiTut";
		public const string INSTALL_SOURCE_SENT = "installSourceSent";
}

public class FHPlayerProfile : SingletonMono<FHPlayerProfile>
{
		const long XOR_KEY = unchecked((long)0xDF154CADDAC451FD);

		// Default values
		const long DEFAULT_GOLD = 1000 ^ XOR_KEY;//1000 de test, default la 50
		const long DEFAULT_DIAMOND = 0 ^ XOR_KEY;
		const long DEFAULT_LEVEL = 1 ^ XOR_KEY;
		const long DEFAULT_XP = 0 ^ XOR_KEY;
		const string DEFAULT_ACTIVE_QUEST = "";

		// Properties
    #region Basic
		public int gold {
				get { 
						long val = (long)GetProperty (FHProfileProperty.GOLD, DEFAULT_GOLD);
						return (int)(val ^ XOR_KEY); 
				}

				set {
						if (value < 0)
								value = 0;

						SetProperty (FHProfileProperty.GOLD, ((long)value) ^ XOR_KEY);
				}
		}

		public int diamond {
				get {
						long val = (long)GetProperty (FHProfileProperty.DIAMOND, DEFAULT_DIAMOND);
						return (int)(val ^ XOR_KEY);
				}

				set {
						if (value < 0)
								value = 0;

						SetProperty (FHProfileProperty.DIAMOND, ((long)value) ^ XOR_KEY);
				}
		}

		public int level {
				get { 
						long val = (long)GetProperty (FHProfileProperty.LEVEL, DEFAULT_LEVEL);
						return (int)(val ^ XOR_KEY); 
				}

				set { SetProperty (FHProfileProperty.LEVEL, ((long)value) ^ XOR_KEY); }
		}

		public int xp {
				get {
						long val = (long)GetProperty (FHProfileProperty.XP, DEFAULT_XP);
						return (int)(val ^ XOR_KEY); 
				}

				set { SetProperty (FHProfileProperty.XP, ((long)value) ^ XOR_KEY); }
		}

		public long lastTimeShowRating {
				get { return (long)GetProperty (FHProfileProperty.LAST_TIME_SHOW_RATING, (long)0); }

				set { SetProperty (FHProfileProperty.LAST_TIME_SHOW_RATING, value); }
		}

		public string targetApp {
				get { return (string)GetProperty (FHProfileProperty.TARGET_APP, ""); }

				set { SetProperty (FHProfileProperty.TARGET_APP, value); }
		}

		public bool dataRestored {
				get { return (bool)GetProperty (FHProfileProperty.DATA_RESTORED, false); }

				set { SetProperty (FHProfileProperty.DATA_RESTORED, (bool)value); }
		}
    #endregion

    #region Settings
		public bool music {
				get { return (bool)GetProperty (FHProfileProperty.MUSIC, true); }

				set { SetProperty (FHProfileProperty.MUSIC, (bool)value); }
		}

		public bool sound {
				get { return (bool)GetProperty (FHProfileProperty.SOUND, true); }

				set { SetProperty (FHProfileProperty.SOUND, (bool)value); }
		}

		public string language {
				get { return (string)GetProperty (FHProfileProperty.LANGUAGE, "English"); }

				set { SetProperty (FHProfileProperty.LANGUAGE, (string)value); }
		}
	#endregion

    #region Quest
		public string activeQuest {
				get { return (string)(GetProperty (FHProfileProperty.ACTIVE_QUEST, DEFAULT_ACTIVE_QUEST)); }

				set { SetProperty (FHProfileProperty.ACTIVE_QUEST, value); }
		}

		public long lastQuestFinishedTime {
				get { return (long)GetProperty (FHProfileProperty.LAST_QUEST_FINISHED_TIME, (long)0); }

				set { SetProperty (FHProfileProperty.LAST_QUEST_FINISHED_TIME, value); }
		}

		public int questDailyCounter {
				get { return (int)((long)GetProperty (FHProfileProperty.QUEST_DAILY_COUNTER, (long)0)); }

				set { SetProperty (FHProfileProperty.QUEST_DAILY_COUNTER, (long)value); }
		}
    #endregion

    #region Item
		public Dictionary<string, object> powerups {
				get { return (GetProperty (FHProfileProperty.POWER_UP, null) as Dictionary<string, object>); }
		}

		public int lightning {
				get { return (int)((long)GetProperty (FHProfileProperty.LIGHTNING, (long)0)); }

				set { SetProperty (FHProfileProperty.LIGHTNING, (long)value); }
		}

		public int nuke {
				get { return (int)((long)GetProperty (FHProfileProperty.NUKE, (long)0)); }

				set { SetProperty (FHProfileProperty.NUKE, (long)value); }
		}
    #endregion

	#region Statistic
		public bool stat_firstPay {
				get { return ((long)GetProperty (FHProfileProperty.STAT_FIRST_PAY, (long)0)) != 0; }

				set { SetProperty (FHProfileProperty.STAT_FIRST_PAY, value ? 1L : 0L); }
		}

		public long lastTimeShowMultiTut {
				get { return (long)GetProperty (FHProfileProperty.LAST_TIME_SHOW_MULTI_TUT, (long)0); }

				set { SetProperty (FHProfileProperty.LAST_TIME_SHOW_MULTI_TUT, value); }
		}

		public bool installSourceSent {
				get { return (bool)GetProperty (FHProfileProperty.INSTALL_SOURCE_SENT, false); }

				set { SetProperty (FHProfileProperty.INSTALL_SOURCE_SENT, value); }
		}
	#endregion

		// Variables
		Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>> ();
		Dictionary<string, string> propGroup = new Dictionary<string, string> ();

		FHProfileDataStream dataStream;

		public void Load (FHProfileDataStream _dataStream)
		{
				dataStream = _dataStream;

				InitGroupInfo ();

				GetGroup (FHProfileGroup.BASIC);
				GetGroup (FHProfileGroup.SETTINGS);
				GetGroup (FHProfileGroup.QUEST);
				GetGroup (FHProfileGroup.ITEM);
				GetGroup (FHProfileGroup.STATISTIC);

				if (dataStream.saveInterval > 0)
						StartCoroutine (OnSaveInterval ());
		}

		public void ForceSave ()
		{
				dataStream.Flush ();
		}

		IEnumerator OnSaveInterval ()
		{
				yield return new WaitForSeconds (dataStream.saveInterval);

				dataStream.Flush ();

				StartCoroutine (OnSaveInterval ());
		}
    
		void InitGroupInfo ()
		{
				propGroup [FHProfileProperty.GOLD] = FHProfileGroup.BASIC;
				propGroup [FHProfileProperty.DIAMOND] = FHProfileGroup.BASIC;
				propGroup [FHProfileProperty.LEVEL] = FHProfileGroup.BASIC;
				propGroup [FHProfileProperty.XP] = FHProfileGroup.BASIC;
				propGroup [FHProfileProperty.LAST_TIME_SHOW_RATING] = FHProfileGroup.BASIC;
				propGroup [FHProfileProperty.TARGET_APP] = FHProfileGroup.BASIC;
				propGroup [FHProfileProperty.DATA_RESTORED] = FHProfileGroup.BASIC;

				propGroup [FHProfileProperty.MUSIC] = FHProfileGroup.SETTINGS;
				propGroup [FHProfileProperty.SOUND] = FHProfileGroup.SETTINGS;
				propGroup [FHProfileProperty.LANGUAGE] = FHProfileGroup.SETTINGS;

				propGroup [FHProfileProperty.LAST_QUEST_FINISHED_TIME] = FHProfileGroup.QUEST;
				propGroup [FHProfileProperty.QUEST_DAILY_COUNTER] = FHProfileGroup.QUEST;
				propGroup [FHProfileProperty.ACTIVE_QUEST] = FHProfileGroup.QUEST;

				propGroup [FHProfileProperty.POWER_UP] = FHProfileGroup.ITEM;
				propGroup [FHProfileProperty.LIGHTNING] = FHProfileGroup.ITEM;
				propGroup [FHProfileProperty.NUKE] = FHProfileGroup.ITEM;

				propGroup [FHProfileProperty.STAT_FIRST_PAY] = FHProfileGroup.STATISTIC;
				propGroup [FHProfileProperty.LAST_TIME_SHOW_MULTI_TUT] = FHProfileGroup.STATISTIC;
				propGroup [FHProfileProperty.INSTALL_SOURCE_SENT] = FHProfileGroup.STATISTIC;
		}

		void GetGroup (string group)
		{
				string json = dataStream.Get (group, "");
				if (json == "") {
						data [group] = new Dictionary<string, object> ();
						return;
				}

				try {
						data [group] = MiniJSON.Json.Deserialize (json) as Dictionary<string, object>;
				} catch (System.Exception ex) {
				}

				if (data [group] == null)
						data [group] = new Dictionary<string, object> ();
		}

		void SetGroup (string group)
		{
				string json = MiniJSON.Json.Serialize (data [group]);

				dataStream.Set (group, json);
		}

		public object GetProperty (string prop, object defaultValue)
		{
				string group = "";
				if (!propGroup.TryGetValue (prop, out group))
						return defaultValue;

				if (!data [group].ContainsKey (prop))
						return defaultValue;

				return data [group] [prop];
		}

		public void SetProperty (string prop, object value)
		{
				if (!propGroup.ContainsKey (prop)) {
						Debug.LogError ("[Profile] " + prop + " isn't in any group!!!");
						return;
				}

				string group = propGroup [prop];

				data [group] [prop] = value;

				SetGroup (group);
		}
}