using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMultiSummary : UIBaseDialogHandler
{
    public UILabel coin;
    public UILabel nuke;
    public UILabel lightning;
    public UILabel powerup;

    SpawnPool effectPool;
    GameObject magicSpoofPrefab;

    public override void OnInit()
    {
        effectPool = PoolManager.Pools["effects"];
        magicSpoofPrefab = (GameObject)Resources.Load("Prefabs/Effect/id_eff_magic_spoof", typeof(GameObject));
    }

    public override void OnBeginShow(object parameter)
    {
        coin.text = FHPlayerProfile.instance.gold.ToString();
        nuke.text = FHPlayerProfile.instance.nuke.ToString();
        lightning.text = FHPlayerProfile.instance.lightning.ToString();
        powerup.text = GetNumberPowerupGuns().ToString();

        //StartCoroutine(SpawnEffect());
        Transform effect = effectPool.Spawn(magicSpoofPrefab.transform);
        effect.localPosition = new UnityEngine.Vector3(0, -15.0f, 0.0f);
        effect.GetComponent<FHSpoof>().Setup(effectPool);
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

    void OnBtnOK()
    {
        GuiManager.HidePanel(GuiManager.instance.guiMultiSummary);
        SceneManager.instance.BackToMM();
    }

    int GetNumberPowerupGuns()
    {
        Dictionary<string, object> powerups = FHPlayerProfile.instance.powerups;
        if (powerups == null)
            return 0;

        int count = 0;

        for (int i = 0; i < FHGameConstant.BET_MULTIPLIERS.Length; i++)
        {
            string multiplier = FHGameConstant.BET_MULTIPLIERS[i].ToString();

            if (!powerups.ContainsKey(multiplier))
                continue;

            if ((long)powerups[multiplier] >= ConfigManager.configPowerup.GetGoldLimitForMultiplier(FHGameConstant.BET_MULTIPLIERS[i]))
                count++;
        }

        return count;
    }
}