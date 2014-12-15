using UnityEngine;
using System.Collections;

public class UIMission : UIBaseDialogHandler {
	public UIGrid grid;
	public GameObject itemPrefab;
	public GameObject loading;
	public override void OnInit()
	{
		FHNetworkManager.SendRequestToServer (new Request_Mission(), typeof(Response_Mission), respMission => {
			Response_Mission resMission = respMission as Response_Mission;	
			if (resMission.retCode == (int)ResultCode.OK) {
				foreach(MissionModel item in resMission.missionList)
				{
					GameObject go = Instantiate(itemPrefab) as GameObject;
					go.GetComponent<MissionItem>().Init(item);
					go.transform.parent = grid.transform;
					go.transform.localScale = Vector3.one;
					grid.Reposition();
				}
				loading.SetActive(false);
			}
		});
	}
	public override void OnBeginShow(object parameter)
	{
	}
	public void OnClose()
	{
		GuiManager.HidePanel(GuiManager.instance.guiMission);
	}
}

