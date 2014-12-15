using UnityEngine;
using System.Collections;
using com.soomla.unity;
using System.Collections.Generic;

public class FHStoreAssets : IStoreAssets
{

	/** Static Final members **/

	public const string FISHHUNT_CURRENCY_ITEM_ID = "fishhunt_gold";

    public const string GOLDPACK1_PRODUCT_ID = "vn.com.gss.fh.hd.it.apple_item001";
    public const string GOLDPACK2_PRODUCT_ID = "vn.com.gss.fh.hd.it.apple_item002";
    public const string GOLDPACK3_PRODUCT_ID = "vn.com.gss.fh.hd.it.apple_item003";
    public const string GOLDPACK4_PRODUCT_ID = "vn.com.gss.fh.hd.it.apple_item004";
    public const string GOLDPACK5_PRODUCT_ID = "vn.com.gss.fh.hd.it.apple_item005";
    public const string GOLDPACK6_PRODUCT_ID = "vn.com.gss.fh.hd.it.apple_item006";


	/** Virtual Currency Packs **/
	public static VirtualCurrencyPack GOLDPACK1 = new VirtualCurrencyPack(
			"Buy 2,000 coins",                                    // name
            "Buy 2,000 coins",									// description
            "vn.com.gss.fh.hd.it.apple_item001",                                // item id
			2000,											// number of currencies in the pack
			FISHHUNT_CURRENCY_ITEM_ID,						// the currency associated with this pack
			new PurchaseWithMarket(GOLDPACK1_PRODUCT_ID, 0.99)
	);

	public static VirtualCurrencyPack GOLDPACK2 = new VirtualCurrencyPack(
            "Buy 6,000 coins",                                    // name
            "Buy 6,000 coins",									// description
            "vn.com.gss.fh.hd.it.apple_item002",                                // item id
			9000,											// number of currencies in the pack
			FISHHUNT_CURRENCY_ITEM_ID,						// the currency associated with this pack
			new PurchaseWithMarket(GOLDPACK2_PRODUCT_ID, 2.99)
	);

	public static VirtualCurrencyPack GOLDPACK3 = new VirtualCurrencyPack(
            "Buy 20,000 coins",                                   // name
            "Buy 10,000 coins",									// description
            "vn.com.gss.fh.hd.it.apple_item003",                                // item id
			10000,											// number of currencies in the pack
			FISHHUNT_CURRENCY_ITEM_ID,						// the currency associated with this pack
			new PurchaseWithMarket(GOLDPACK3_PRODUCT_ID, 4.99)
	);

	public static VirtualCurrencyPack GOLDPACK4 = new VirtualCurrencyPack(
            "Buy 20,000 coins",                                   // name
            "Buy 20,000 coins",									// description
            "vn.com.gss.fh.hd.it.apple_item004",                                // item id
			20000,											// number of currencies in the pack
			FISHHUNT_CURRENCY_ITEM_ID,						// the currency associated with this pack
			new PurchaseWithMarket(GOLDPACK4_PRODUCT_ID, 9.99)
	);

	public static VirtualCurrencyPack GOLDPACK5 = new VirtualCurrencyPack(
            "Buy 50,000 coins",                                   // name
            "Buy 50,000 coins",									// description
            "vn.com.gss.fh.hd.it.apple_item005",                                // item id
			50000,											// number of currencies in the pack
			FISHHUNT_CURRENCY_ITEM_ID,						// the currency associated with this pack
			new PurchaseWithMarket(GOLDPACK5_PRODUCT_ID, 24.99)
	);

	public static VirtualCurrencyPack GOLDPACK6 = new VirtualCurrencyPack(
            "Buy 100,000 coins",                                  // name
            "Buy 100,000 coins",									// description
            "vn.com.gss.fh.hd.it.apple_item006",                                // item id
			100000,											// number of currencies in the pack
			FISHHUNT_CURRENCY_ITEM_ID,						// the currency associated with this pack
			new PurchaseWithMarket(GOLDPACK6_PRODUCT_ID, 49.99)
	);

	/** Virtual Currencies **/
	public static VirtualCurrency FISHHUNT_CURRENCY = new VirtualCurrency(
			"FishHunt Gold",
			"",
			FISHHUNT_CURRENCY_ITEM_ID
	);
	

	public int GetVersion()
	{
		return 0;
	}

	
	public VirtualCurrencyPack[] GetCurrencyPacks()
	{
		return new VirtualCurrencyPack[] { GOLDPACK1, GOLDPACK2, GOLDPACK3, GOLDPACK4, GOLDPACK5, GOLDPACK6 };
	}

	public VirtualCurrency[] GetCurrencies()
	{
		return new VirtualCurrency[] { FISHHUNT_CURRENCY };
	}


	public VirtualGood[] GetGoods()
	{
		return new VirtualGood[] { };
	}

	public VirtualCategory[] GetCategories()
	{
		return new VirtualCategory[] { };
	}

	public NonConsumableItem[] GetNonConsumableItems()
	{
		return new NonConsumableItem[] { };
	}
}