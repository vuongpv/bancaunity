using UnityEngine;
using System.Collections;

public class UIGoldPackDropReceiver : MonoBehaviour
{
    public UITouchZone touchArea;

	void OnDrop(GameObject go)
	{
        UIGoldPackDragDropItem ddo = go.GetComponent<UIGoldPackDragDropItem>();
		
		if (ddo != null)
		{
            Debug.LogError(ddo.gameObject.name);
		}
	}
}
