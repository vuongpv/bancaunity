using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FHLocalization : ManualSingletonMono<FHLocalization>
{	
	public enum Language
	{
		English = 0,
		Vietnamese,
		Chinese
	}

	public static Language[] languages=new Language[]{Language.English,Language.Chinese,Language.Vietnamese};

	public const string FONT_PREFAB_PATH = "Prefabs/Font/";

	public Language currLang;
	private ConfigString configString;

	private Dictionary<string, UIFont> fonts = new Dictionary<string, UIFont>();
	private Dictionary<string, UIFont> origFontRefs = new Dictionary<string, UIFont>();
	
	protected override void Awake()
	{
		base.Awake();

		LoadDataString();
	}

	public string GetString(int id)
	{
		if (configString == null || !configString.isLoaded)
			return "@error: config not loaded@";

		ConfigStringItem item = configString.GetStringItem(id);
		if (item == null)
			return string.Format("({0})", id);
		string reString = item.en;
		switch (currLang)
		{
			case Language.English:
				reString = item.en;
				break;
			case Language.Vietnamese:
				reString = item.vn;
				break;
			case Language.Chinese:
				reString = item.chn;
				break;
		}


		string[] regex = { @"\n" };
		string[] temp2 = reString.Split(regex, System.StringSplitOptions.RemoveEmptyEntries);
		reString = "";
		for (int i = 0; i < temp2.Length; i++)
		{
			if (i == temp2.Length - 1)
			{
				reString += temp2[i];
				break;
			}
			reString += temp2[i] + "\n";
		}
		return reString;
	}
	
	private void LoadDataString()
	{
		try
		{
			if (configString == null)
			{
				configString = new ConfigString();

				configString.BeginLoadAppend();
				configString.LoadFromAssetPath("Config/ConfigString");
				configString.EndLoadAppend();

				Debug.Log("Config [" + configString.GetName() + "] loaded");
			}
		}
		catch (System.Exception ex)
		{
			Debug.Log("Load Config [" + configString.GetName() + "] Error = " + ex.ToString());
		}
	}

	public static string GetName(Language lang)
	{
		switch (lang)
		{
			case Language.Vietnamese:
				return "Tiếng Việt";
	
			case Language.Chinese:
				return "汉语";

			default:
				return "English";
		}
	}

	Language GetLanguage(string lang)
	{
		switch (lang)
		{
			case "Tiếng Việt":
				return Language.Vietnamese;

			case "汉语":
				return Language.Chinese;

			default:
				return Language.English;
		}
	}

	public void LoadFonts()
	{
		UIFont[] loadedFonts = Resources.FindObjectsOfTypeAll(typeof(UIFont)) as UIFont[];
		for (int i = 0; i < loadedFonts.Length; i++)
			fonts[loadedFonts[i].name] = loadedFonts[i];

		foreach (ConfigFontItem fontItem in ConfigManager.configFont.records)
			LoadFontItem(fontItem);

#if UNITY_EDITOR
		foreach (KeyValuePair<string, UIFont> font in fonts)
			if (font.Value.replacement != null)
			{
				origFontRefs[font.Key] = font.Value.replacement;
			}
#endif
	}

	void LoadFontItem(ConfigFontItem item)
	{
		LoadFont(item.alias);
		LoadFont(item.en);
		LoadFont(item.vn);
		LoadFont(item.chn);
	}

	void LoadFont(string fontName)
	{
		if (fonts.ContainsKey(fontName))
			return;

		UIFont font = Resources.Load(FONT_PREFAB_PATH + fontName, typeof(UIFont)) as UIFont;
		fonts[fontName] = font;
	}

	public void SetLocale(string lang)
	{
		SetLocale(GetLanguage(lang));
	}

	public void SetLocale(Language lang)
	{
		FHPlayerProfile.instance.language = GetName(lang);

		currLang = lang;

		// Switch font
		foreach (ConfigFontItem fontItem in ConfigManager.configFont.records)
			SwitchFont(fontItem);

		// Apply new message to all UILabel
		UILabel[] labels = NGUITools.FindActive<UILabel>();

		for (int i = 0, imax = labels.Length; i < imax; ++i)
		{
			FHUILabelStringID stringID = labels[i].GetComponent<FHUILabelStringID>();
			if (stringID == null)
				continue;

			labels[i].text = GetString(stringID.stringID);
		}
	}

	void SwitchFont(ConfigFontItem item)
	{
		UIFont aliasFont = fonts[item.alias];
		UIFont targetFont = null;

		switch (currLang)
		{
			case Language.English:
				targetFont = fonts[item.en];
				break;

			case Language.Vietnamese:
				targetFont = fonts[item.vn];
				break;

			case Language.Chinese:
				targetFont = fonts[item.chn];
				break;
		}

		if (targetFont != null)
			aliasFont.replacement = targetFont;
	}

	public void RestoreFontReferences()
	{
		foreach (KeyValuePair<string, UIFont> font in origFontRefs)
			fonts[font.Key].replacement = font.Value;
	}

	void OnApplicationQuit()
	{
#if UNITY_EDITOR
		RestoreFontReferences();
#endif
	}
}