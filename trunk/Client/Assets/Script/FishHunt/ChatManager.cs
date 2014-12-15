using UnityEngine;
using System.Collections;

public class ChatManager : SingletonMono<ChatManager>{

	// Use this for initialization
	public UIInput input;
	public UITextList textList;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void OnClickSend()
	{
		string mes = NGUIText.StripSymbols(input.value);
		if (!string.IsNullOrEmpty(mes))
		{
			textList.Add(mes);
			input.value = "";
			input.isSelected = false;
		}
		M_Chat_Base c = new M_C_Chat(FHNetworkManager.Client().roomType, FHNetworkManager.Client().roomName, mes);
		FHNetworkManager.SendChatToServer(c);
	}
	public void UpdateMessage(string mes)
	{
		Debug.LogError("mes: " + mes);
		textList.Add(mes);
	}
	public void OnClose()
	{
		GuiManager.HidePanel(GuiManager.instance.guiChat);
	}
}
