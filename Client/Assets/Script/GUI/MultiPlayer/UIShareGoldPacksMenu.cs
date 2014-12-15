using UnityEngine;
using System.Collections;

public class UIShareGoldPacksMenu : MonoBehaviour
{
    const int NUMBER_PACKS_PER_PAGE = 3;

    public GameObject btnBuyCoin;

    FHPlayerMultiController player;

    UIGoldPackDragDropItem[] packs;

    void Awake()
    {
        packs = new UIGoldPackDragDropItem[NUMBER_PACKS_PER_PAGE];

        for (int i = 0; i < NUMBER_PACKS_PER_PAGE; i++)
            packs[i] = gameObject.transform.FindChild("Pack" + i.ToString()).gameObject.GetComponent<UIGoldPackDragDropItem>();
    }

    public void Setup(FHPlayerMultiController _player)
    {
        player = _player;
        
        if (!player.isHostingPlayer)
        {
            for (int i = 0; i < NUMBER_PACKS_PER_PAGE; i++)
                packs[i].transform.localPosition = packs[i].simpleModePos;

            btnBuyCoin.SetActiveRecursively(false);
        }

        Reload();
    }

    public void Reload()
    {
        for (int i = 0; i < NUMBER_PACKS_PER_PAGE; i++)
        {
            packs[i].transform.localEulerAngles = Vector3.zero;
            packs[i].Setup(player, this, FHGameConstant.GOLD_SHARE[i]);
        }
    }

    public void DisableAllColliders()
    {
        for (int i = 0; i < NUMBER_PACKS_PER_PAGE; i++)
            packs[i].gameObject.collider.enabled = false;
    }
}