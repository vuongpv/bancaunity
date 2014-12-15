using UnityEngine;
using System.Collections;
using GFramework;
using System.Linq;
public class UIOnlineFinalScore : MonoBehaviour {
    public enum MatchOnlineResult
    {
        WIN=0,
        DEUCE=1,
        LOSE=2,
    }
    public GameObject winObj;
    public GameObject loseObj;
    public GameObject deuceObj;
    
    public UILabel scoreLbl;
    public UILabel loseLbl;
    private bool isCanClose;
    public FHSceneOnlineController sceneOnline;

	// Use this for initialization
	void Start () {
        isCanClose = false;
        gameObject.SetActiveRecursively(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Show(MatchOnlineResult matchResult, int score)
    {
        isCanClose = false;
        gameObject.SetActiveRecursively(true);
        
        loseObj.transform.localScale = deuceObj.transform.localScale = winObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        if (matchResult==MatchOnlineResult.WIN)
        {
            loseObj.SetActiveRecursively(false);
            deuceObj.SetActiveRecursively(false);
            scoreLbl.text = score.ToString();
            DoTween("win",winObj);
        }
        else if (matchResult == MatchOnlineResult.DEUCE)
        {
            winObj.SetActiveRecursively(false);
            loseObj.SetActiveRecursively(false);
            DoTween("lose", deuceObj);
        }
        else
        {
            winObj.SetActiveRecursively(false);
            deuceObj.SetActiveRecursively(false);
            loseLbl.text = score.ToString();
            DoTween("lose", loseObj);
        }
    }
    public void OnShowWinFinish()
    {
        isCanClose = true;
        GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Effect/id_eff_magic_spoof_online")) as GameObject;
    }

    public void OnShowLoseFinish()
    {
        isCanClose = true;
    }
    public void OnClose()
    {
        if (isCanClose)
        {
            sceneOnline.UserClose(FH.MessageBox.DialogResult.Yes);
        }
    }
    /// <summary>
    /// Does the tween.
    /// </summary>
    protected float DoTween(string tweenName, GameObject obj)
    {
        if (string.IsNullOrEmpty(tweenName))
            return 0;

        iTweenEvent tween = gameObject.GetComponents<iTweenEvent>().Where(tw => tw.tweenName == tweenName).FirstOrDefault();
        if (tween != null)
        {
            tween.SetObjectTarget(obj);
            tween.Play();
            if (!tween.Values.ContainsKey("time"))
                return 2f;

            return (float)tween.Values["time"];
        }
        return 0;
    }
}
