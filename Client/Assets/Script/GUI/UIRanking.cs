using UnityEngine;
using System.Collections;

public class UIRanking : UIBaseDialogHandler {
	public UIGrid grid;
	private int index = 1;
	public GameObject itemPrefab;
	public GameObject loading;
	public override void OnInit()
	{
		FHNetworkManager.SendRequestToServer (new Request_Ranking(), typeof(Response_Ranking), respRanking => {
			Response_Ranking resRanking = respRanking as Response_Ranking;	
			if (resRanking.retCode == (int)ResultCode.OK) {
				foreach(RankingModel item in resRanking.dailyRanking)
				{
					GameObject go = Instantiate(itemPrefab) as GameObject;
					go.GetComponent<RankingItem>().Init(item, index);
					go.transform.parent = grid.transform;
					go.transform.localScale = Vector3.one;
					grid.Reposition();
					index++;
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
		GuiManager.HidePanel(GuiManager.instance.guiRanking);
	}
}

