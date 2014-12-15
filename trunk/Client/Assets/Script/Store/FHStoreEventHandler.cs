using UnityEngine;
using System.Collections;
using com.soomla.unity;
using System.Collections.Generic;

public class FHStoreEventHandler {

	public FHStoreEventHandler ()
	{
		Events.OnMarketPurchase += onMarketPurchase;
		Events.OnMarketRefund += onMarketRefund;
		Events.OnItemPurchased += onItemPurchased;
		Events.OnGoodEquipped += onGoodEquipped;
		Events.OnGoodUnEquipped += onGoodUnequipped;
		Events.OnGoodUpgrade += onGoodUpgrade;
		Events.OnBillingSupported += onBillingSupported;
		Events.OnBillingNotSupported += onBillingNotSupported;
		Events.OnMarketPurchaseStarted += onMarketPurchaseStarted;
		Events.OnItemPurchaseStarted += onItemPurchaseStarted;
		Events.OnClosingStore += onClosingStore;
		Events.OnOpeningStore += onOpeningStore;
		Events.OnUnexpectedErrorInStore += onUnexpectedErrorInStore;
		Events.OnCurrencyBalanceChanged += onCurrencyBalanceChanged;
		Events.OnGoodBalanceChanged += onGoodBalanceChanged;
		Events.OnMarketPurchaseCancelled += onMarketPurchaseCancelled;
		Events.OnRestoreTransactionsStarted += onRestoreTransactionsStarted;
		Events.OnRestoreTransactions += onRestoreTransactions;
		Events.OnStoreControllerInitialized += onStoreControllerInitialized;
	}
		
	public void onMarketPurchase(PurchasableVirtualItem pvi) {
		if (UIShopHandler.instance != null)
			UIShopHandler.instance.ChangeStatus (false);
	}
		
	public void onMarketRefund(PurchasableVirtualItem pvi) {

	}
		
	public void onItemPurchased(PurchasableVirtualItem pvi) {
		Debug.LogError ("On Item Purchased " + pvi.ItemId);
		ConfigGoldPackRecord goldPack = ConfigManager.configGoldPack.GetPackByID(pvi.ItemId);
		if (goldPack != null)
		{

            if (FHSystem.instance.GetCurrentPlayerMode() == FHPlayerMode.Multi)
            {
                FHMultiPlayerManager.instance.GetMainPlayer().AddCoin(goldPack.goldValue + goldPack.goldBonus);
            }
            else
            {
                FHPlayerProfile.instance.gold += goldPack.goldValue + goldPack.goldBonus;
                if (FHGoldHudPanel.instance != null)
                    FHGoldHudPanel.instance.UpdateGold();

                RequestTrackPayment(goldPack);
            }
            FHPlayerProfile.instance.ForceSave();
		}

		if (UIShopHandler.instance != null)
			UIShopHandler.instance.ChangeStatus (false);
	}
		
	public void onGoodEquipped(EquippableVG good) {
			
	}
		
	public void onGoodUnequipped(EquippableVG good) {
			
	}
		
	public void onGoodUpgrade(VirtualGood good, UpgradeVG currentUpgrade) {
			
	}
		
	public void onBillingSupported() {
			
	}
		
	public void onBillingNotSupported() {
			
	}
		
	public void onMarketPurchaseStarted(PurchasableVirtualItem pvi) {
			
	}
		
	public void onItemPurchaseStarted(PurchasableVirtualItem pvi) {
			
	}
		
	public void onMarketPurchaseCancelled(PurchasableVirtualItem pvi) {
		if (UIShopHandler.instance != null)
			UIShopHandler.instance.ChangeStatus (false);
	}
		
	public void onClosingStore() {
			
	}
		
	public void onUnexpectedErrorInStore() {
		if (UIShopHandler.instance != null)
			UIShopHandler.instance.ChangeStatus (false);
	}
		
	public void onOpeningStore() {

	}
		
	public void onCurrencyBalanceChanged(VirtualCurrency virtualCurrency, int balance, int amountAdded) {
				
	}
		
	public void onGoodBalanceChanged(VirtualGood good, int balance, int amountAdded) {
	}
		
	public void onRestoreTransactionsStarted() {
			
	}
		
	public void onRestoreTransactions(bool success) {
			
	}
		
	public void onStoreControllerInitialized() {
			
	}

	public void RequestTrackPayment(ConfigGoldPackRecord pack)
	{
		if (pack == null)
			return;

		// Make request params
		Dictionary<string, object> @params = new Dictionary<string, object>();
		@params["deviceID"] = SystemHelper.deviceUniqueID;
		@params["product"] = pack.id;
		@params["money"] = pack.cashValue;

		FHHttpClient.RequestTransaction(@params, "applestore", (code, json) =>
		{
			switch ((int)pack.cashValue)
			{
				case 1: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "1.99"); break;
				case 2: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "2_99"); break;
				case 6: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "6_99"); break;
				case 12: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "12_99"); break;
				case 23: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "23_99"); break;
				case 39: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "39_99"); break;
			}
		});
	}
}
