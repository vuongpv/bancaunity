using UnityEngine;
using System.Collections;

public class ExternalServices : SingletonMono<ExternalServices>
{
	// FACEBOOK
	public const string FACEBOOK_ID = "154016334800539"; // "154016334800539";
	public const string FACEBOOK_DISPLAYNAME = "Fish Hunt HD"; // "154016334800539";
	public const string FACEBOOK_PERMISSION = "";

	// FLURRY
#if UNITY_ANDROID
	public const string FLURRY_APIKEY = "8H3KP5H26K698MD8V73X";
#elif UNITY_IPHONE
	public const string FLURRY_APIKEY = "SD39Y82RJCH5QSRCX8RV";
#else
	public const string FLURRY_APIKEY = "";
#endif

	// SO6 CARD PAYMENT
	public const string SO6PAYMENT_HOST = "cardpay.g6-mobile.zing.vn";
	public const int SO6PAYMENT_PORT = 443;

	public void Init()
	{
		FacebookBinding.Init(FACEBOOK_ID, FACEBOOK_PERMISSION);
		FacebookBinding.PublishInstallApp();

	   // InAppBillingBinding.Init();

		SO6PaymentBinding.Init(SO6PAYMENT_HOST, SO6PAYMENT_PORT);
		GoogleAnalyticsBinding.Init();

		FlurryBinding.Init(FLURRY_APIKEY);
	}

	void OnApplicationPause(bool pause)
	{
		//Debug.LogError("Application pause " + pause);
		if (pause)
		{
			// Pause
			FlurryBinding.Shutdown();
		}
		else
		{
			// Unpause
			FlurryBinding.Init(FLURRY_APIKEY);

			if (!FacebookBinding.IsInit )
				FacebookBinding.Init(FACEBOOK_ID, FACEBOOK_PERMISSION);
			FacebookBinding.PublishInstallApp();
		}
	}

	void OnDestroy()
	{
		//Debug.LogError("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
		//FlurryBinding.Shutdown();
	}
		
	public static string SMSServiceNumberFromPrice(int price)
	{
		switch(price)
		{
			case 5000:
				return "6569";

			case 10000:
				return "6669";

			case 15000:
				return "6769";
		}

		return "";
	}
}
