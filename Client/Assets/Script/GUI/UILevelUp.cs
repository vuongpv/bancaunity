using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILevelUpParams
{
    public int level;
    public int coin;

    public UILevelUpParams(int _level, int _coin)
    {
        level = _level;
        coin = _coin;
    }
}

public class UILevelUp : UIBaseDialogHandler
{
    public UILabel level;
    public UILabel coin;

    SpawnPool effectPool;
    GameObject magicSpoofPrefab;

    public override void OnInit()
    {
        effectPool = PoolManager.Pools["effects"];
        magicSpoofPrefab = (GameObject)Resources.Load("Prefabs/Effect/id_eff_magic_spoof", typeof(GameObject));
    }

    public override void OnBeginShow(object parameter)
    {
        UILevelUpParams @params = (UILevelUpParams)parameter;
        level.text = @params.level.ToString();
        coin.text = @params.coin.ToString();

        StartCoroutine(SpawnEffect());

        GuiManager.HidePanelAfterTime(GuiManager.instance.guiLevelUp, 3.0f);
    }

    IEnumerator SpawnEffect()
    {
        Transform effect = effectPool.Spawn(magicSpoofPrefab.transform);
        effect.localPosition = new UnityEngine.Vector3(0, -15.0f, 0.0f);
        effect.GetComponent<FHSpoof>().Setup(effectPool);

        yield return new WaitForSeconds(0.5f);

        effect = effectPool.Spawn(magicSpoofPrefab.transform);
        effect.localPosition = new UnityEngine.Vector3(-5.0f, -15.0f, 2.0f);
        effect.GetComponent<FHSpoof>().Setup(effectPool);

        yield return new WaitForSeconds(0.5f);

        FHAudioManager.instance.PlaySound(FHAudioManager.SOUND_LEVELUP);

        effect = effectPool.Spawn(magicSpoofPrefab.transform);
        effect.localPosition = new UnityEngine.Vector3(3.0f, -15.0f, -2.0f);
        effect.GetComponent<FHSpoof>().Setup(effectPool);
    }

    void OnClick()
    {
        GameObject obj = UICamera.selectedObject;

        switch (obj.name)
        {
            case "BtnOK":
                OnBtnOK();
                break;
        }
    }

    public void OnBtnOK()
    {
        GuiManager.HidePanel(GuiManager.instance.guiLevelUp);
    }
}