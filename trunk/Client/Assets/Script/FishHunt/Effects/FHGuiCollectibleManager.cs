using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public interface ICollectibleTarget
{
	Vector3 GetTargetPos(string type);
	void OnReachTarget(string type);
}

public class FHGuiCollectibleManager : MonoBehaviour {

	SpawnPool guiEffectPool;
	GameObject coinPrefab, coinTextPrefab, lightningPrefab, nukePrefab, diamondPrefab;

	public static FHGuiCollectibleManager instance { get; private set; }

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	// Use this for initialization
	void Start()
	{
		guiEffectPool = PoolManager.Pools["guieffects"];

		coinPrefab = (GameObject)Resources.Load("Prefabs/Effect/id_eff_coin", typeof(GameObject));
		coinTextPrefab = (GameObject)Resources.Load("Prefabs/Effect/id_eff_coin_text", typeof(GameObject));
		lightningPrefab = (GameObject)Resources.Load("Prefabs/Effect/id_eff_lightning", typeof(GameObject));
		nukePrefab = (GameObject)Resources.Load("Prefabs/Effect/id_eff_nuke", typeof(GameObject));
        diamondPrefab = (GameObject)Resources.Load("Prefabs/Effect/id_eff_diamond", typeof(GameObject));
	}

	Vector3 GetUIPos(Vector3 worldPos)
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
		screenPos.z = 0;
		return UICamera.currentCamera.ScreenToWorldPoint(screenPos);
	}

	public void SpawnWorldGold(Vector3 worldPos, ICollectibleTarget target, int value)
	{
		Vector3 uiPos = GetUIPos(worldPos);

		// Coin
		SpawnUICoin(uiPos, target, value);

		// Text
		SpawnUICoinText(uiPos, value);
	}

    public void SpawnUICoinText(Vector3 uiPos, int value)
	{
		// Text
		Transform obj = guiEffectPool.Spawn(coinTextPrefab.transform, uiPos, Quaternion.identity);
		FHCoinText text = obj.GetComponent<FHCoinText>();
		text.Setup(value);
	}

	public void SpawnUICoin(Vector3 uiPos, ICollectibleTarget target, int value)
	{
		// Text
		Transform obj = guiEffectPool.Spawn(coinPrefab.transform, uiPos, Quaternion.identity);
		FHSimpleCoin collectible = obj.GetComponent<FHSimpleCoin>();
		collectible.Setup(target);
	}

	public void SpawnWorldPwrUp(int pwrUpID, Vector3 worldPos, ICollectibleTarget target)
	{
		Vector3 uiPos = GetUIPos(worldPos);

		// Text
		Transform obj = null;
		
		if( pwrUpID == FHGameConstant.LIGHTNING_GUN )
			obj = guiEffectPool.Spawn(lightningPrefab.transform, uiPos, Quaternion.identity);
		else if (pwrUpID == FHGameConstant.NUKE_GUN)
			obj = guiEffectPool.Spawn(nukePrefab.transform, uiPos, Quaternion.identity);
		FHSimpleCoin collectible = obj.GetComponent<FHSimpleCoin>();
		collectible.Setup(target);
	}

	public void Collect(Transform collectible)
	{
		guiEffectPool.Despawn(collectible);
	}

    public void SpawnWorldGoldMulti(Transform coinTextPos, Vector3 worldPos, ICollectibleTarget target, int value)
    {
        Vector3 uiPos = GetUIPos(worldPos);

        // Coin
        SpawnUICoin(uiPos, target, value);

        // Text
        SpawnUICoinTextMulti(coinTextPos, value);
    }

    public void SpawnUICoinTextMulti(Transform uiPos, int value)
    {
        // Text
        Transform obj = guiEffectPool.Spawn(coinTextPrefab.transform, uiPos.position, Quaternion.identity);
        obj.up = uiPos.up;
        
        Vector3 angle = obj.localEulerAngles;
        if (obj.up.y < 0)
            angle.y = 0;
        obj.localEulerAngles = angle;

        FHCoinText text = obj.GetComponent<FHCoinText>();
        text.Setup(value);
    }

    public void SpawnWorldDiamond(Vector3 worldPos, ICollectibleTarget target, int value)
    {
        Vector3 uiPos = GetUIPos(worldPos);

        // Coin
        SpawnUIDiamond(uiPos, target, value);

        // Text
        SpawnUICoinText(uiPos, value);
    }

    public void SpawnUIDiamond(Vector3 uiPos, ICollectibleTarget target, int value)
    {
        // Text
        Transform obj = guiEffectPool.Spawn(diamondPrefab.transform, uiPos, Quaternion.identity);
        FHSimpleCoin collectible = obj.GetComponent<FHSimpleCoin>();
        collectible.Setup(target);
    }
}