using UnityEngine;
using System.Collections;

public class MissionItem : MonoBehaviour {
	public UILabel name;
	public UILabel amount;
	public int id;
	public void Init(MissionModel item)
	{
		id = item.id;
		name.text = item.name;
		amount.text = item.gold.ToString();
	}
	public void OnRecive()
	{
		FHNetworkManager.SendMessageToServer(new M_C_RecivePresent(id));
	}
}
