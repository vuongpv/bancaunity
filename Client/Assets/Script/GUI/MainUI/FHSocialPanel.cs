using UnityEngine;
using System.Collections;

public class FHSocialPanel : MonoBehaviour {

	void OnClick()
	{
		switch (UICamera.selectedObject.name)
		{
			case "FacebookBtn":
				//FacebookBinding.PostNewFeed("name ne", "caption ne", "description ne", "http://www.friendsmash.com/images/logo_large.jpg", "http://google.com");
				//FacebookBinding.SendAppRequest("Choi voi minh nha");
				break;
		}
	}
}
