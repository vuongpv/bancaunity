using UnityEngine;
using System.Collections.Generic;

public class FHLanguageSelection : MonoBehaviour
{
    public UISprite iconLanguage;
    public float containerStartY;
    public float containerFinishY;
    public float containerScrollingSpeed;

    public Transform container;
    public Transform arrow;
    public GameObject outerArea;

    Vector3 direction = Vector3.zero;
    bool isScrolling = false;
    bool isShowing = false;

    Dictionary<FHLocalization.Language, string> mapLanquage=new Dictionary<FHLocalization.Language, string>();
    Dictionary<FHLocalization.Language, int> saveLanquage = new Dictionary<FHLocalization.Language, int>();
    public UISprite[] languageNew=new UISprite[2];
    private FHLocalization.Language[] chooseLanquage=new FHLocalization.Language[2];
    void Start()
    {
        mapLanquage[FHLocalization.Language.Vietnamese] = "menu_vietnamese";
        mapLanquage[FHLocalization.Language.Chinese] = "menu_chinese";
        mapLanquage[FHLocalization.Language.English] = "menu_english";
        saveLanquage[FHLocalization.Language.Vietnamese] = 1;
        saveLanquage[FHLocalization.Language.Chinese] = 2;
        saveLanquage[FHLocalization.Language.English] = 0;
        Initial();
    }
    public void Initial()
    {
        int check=PlayerPrefs.GetInt(FHUtils.NAME_LANGUAGESAVE);
        switch (check)
        {
            case 1: FHLocalization.instance.currLang = FHLocalization.Language.Vietnamese;
                break;
            case 2: FHLocalization.instance.currLang = FHLocalization.Language.Chinese;
                break;
            default: FHLocalization.instance.currLang = FHLocalization.Language.English;
                break;
        }
        FHLocalization.instance.SetLocale(FHLocalization.instance.currLang);
        if (FHLocalization.instance.currLang == FHLocalization.Language.Chinese)
        {
             iconLanguage.spriteName = "menu_chinese";
             chooseLanquage[0] = FHLocalization.Language.English;
             chooseLanquage[1] = FHLocalization.Language.Vietnamese;
        }
        else if (FHLocalization.instance.currLang == FHLocalization.Language.Vietnamese)
        {
              iconLanguage.spriteName = "menu_vietnamese";
              chooseLanquage[0] = FHLocalization.Language.English;
              chooseLanquage[1] = FHLocalization.Language.Chinese;
        }
        else
        {
             iconLanguage.spriteName = "menu_english";
             chooseLanquage[0] = FHLocalization.Language.Vietnamese;
             chooseLanquage[1] = FHLocalization.Language.Chinese;
        }
        for (int i = 0; i < 2; i++)
        {
            languageNew[i].spriteName = mapLanquage[chooseLanquage[i]];
        }
    }
    void Update()
    {
        if (!isScrolling)
            return;

        container.localPosition += direction * containerScrollingSpeed * Time.deltaTime;

        if (container.localPosition.y >= containerFinishY)
        {
            container.localPosition = new Vector3(container.localPosition.x, containerFinishY, container.localPosition.z);

            isShowing = true;
            isScrolling = false;
            direction = Vector3.zero;
            UIHelper.EnableWidget(outerArea);
        }
        else
            if (container.localPosition.y <= containerStartY)
            {
                container.localPosition = new Vector3(container.localPosition.x, containerStartY, container.localPosition.z);

                isShowing = false;
                isScrolling = false;
                direction = Vector3.zero;
                UIHelper.DisableWidget(outerArea);
            }
    }

	void OnLanguageSelection(string language)
	{
    }
    public void Show()
    {
        arrow.eulerAngles = new Vector3(arrow.eulerAngles.x, arrow.eulerAngles.y, 180.0f - arrow.eulerAngles.z);

        if (!isScrolling)
        {
            if (isShowing)
                direction = -Vector3.up;
            else
                direction = Vector3.up;

            isScrolling = true;
        }
        else
            direction = -direction;
    }
    public void OnChangeLanquage()
    {
        Show();
    }
    public void OnLanquage1()
    {
        Debug.LogError(chooseLanquage[0]);
        iconLanguage.spriteName = mapLanquage[chooseLanquage[0]];
        FHLocalization.instance.currLang = chooseLanquage[0];
        int checkSave = saveLanquage[chooseLanquage[0]];
        PlayerPrefs.SetInt(FHUtils.NAME_LANGUAGESAVE, checkSave);
        Initial();
        Show();
        //SceneManager.instance.LoadSceneWithLoading(FHScenes.MainMenu);
    }

    public void OnLanquage2()
    {
        iconLanguage.spriteName = mapLanquage[chooseLanquage[1]];
        FHLocalization.instance.currLang = chooseLanquage[1];
        int checkSave = saveLanquage[chooseLanquage[1]];
        PlayerPrefs.SetInt(FHUtils.NAME_LANGUAGESAVE, checkSave);
        Initial();
        Show();
    }
}
