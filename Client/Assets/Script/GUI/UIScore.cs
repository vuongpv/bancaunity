using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIScore : UIBaseDialogHandler
{
    public UILabel score;

    SpawnPool effectPool;
    GameObject magicSpoofPrefab;

    public override void OnInit()
    {
        effectPool = PoolManager.Pools["effects"];
        magicSpoofPrefab = (GameObject)Resources.Load("Prefabs/Effect/id_eff_magic_spoof_yellow", typeof(GameObject));
    }

    public override void OnBeginShow(object parameter)
    {
        score.text = ((int)parameter).ToString();

        GuiManager.HidePanelAfterTime(GuiManager.instance.guiScore, 1.5f);

        Transform effect = effectPool.Spawn(magicSpoofPrefab.transform);
        effect.localPosition = new UnityEngine.Vector3(0, -15.0f, 0.0f);
        effect.GetComponent<FHSpoof>().Setup(effectPool);
    }
}