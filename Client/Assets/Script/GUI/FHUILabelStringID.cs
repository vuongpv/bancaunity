using UnityEngine;
using System.Collections;

public class FHUILabelStringID : MonoBehaviour {

	public int stringID;

	// Use this for initialization
	void OnEnable () {
		if( FHLocalization.instance == null )
			return;

		UILabel label = GetComponent<UILabel>();
		if( label != null )
			label.text = FHLocalization.instance.GetString(stringID);
	}
	
}
