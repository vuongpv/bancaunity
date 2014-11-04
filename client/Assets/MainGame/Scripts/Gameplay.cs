using UnityEngine;
using System.Collections;
using System.Linq; 

public class Gameplay : GameBoard {
	public	Camera mView;
	private int level=2;
	public TextAsset fishLevel;
	public TextAsset totalInfor;
	
	private string[][] infor_fishLevel,infor_Fish;

	void Start () {
		infor_fishLevel= CSVReader.GetData (fishLevel.text);

		LoadFish (level);
	}

	public void ReadFile(string fileName)
	{
		string line;
		System.IO.StreamReader file = new System.IO.StreamReader(fileName);
		while ((line = file.ReadLine()) != null) {
			Debug.Log(line);
		}
	}

	private void LoadFish(int level)
	{
		string[] data=infor_fishLevel[level];
		for (int i=(int)INDEX_FISH_LEVEL.F1; i<data.Length; i++) {
			if(data[i]==null)
				continue;

			for(int j=0;j<int.Parse(data[i]);j++)
			{
				CreateFish(i-3);
			}
		}
	}


	private Fish CreateFish(int id)
	{
		Fish f = null;

		switch (id) {
		case 7:
			f=(Instantiate (Resources.Load (Constant.pathPrefabs+"Fish07")) as GameObject).GetComponent<Fish07>();
			f.SetCamera( mView);
			f.transform.parent =transform.FindChild("Object").transform.FindChild("Fishs").transform;
			f.Init(id);
			break;
		case 10:
		case 6:
			f=(Instantiate (Resources.Load (Constant.pathPrefabs+"Jellyfish")) as GameObject).GetComponent<Jellyfish>();
			f.SetCamera( mView);
			f.transform.parent =transform.FindChild("Object").transform.FindChild("Fishs").transform;
			f.Init(id);
			break;
		case 15:
		case 16:
		case 17:
			f=(Instantiate (Resources.Load (Constant.pathPrefabs+"Mermaid")) as GameObject).GetComponent<Mermaid>();
			f.SetCamera( mView);
			f.transform.parent =transform.FindChild("Object").transform.FindChild("Fishs").transform;
			f.Init(id);
			break;
		default:
			f=(Instantiate (Resources.Load (Constant.pathPrefabs+"Fish")) as GameObject).GetComponent<Fish>();
			f.SetCamera( mView);
			f.transform.parent =transform.FindChild("Object").transform.FindChild("Fishs").transform;
			f.Init(id);
			break;
		}


		return f;
	}




	public void OnSettingClick()
	{
		Debug.Log ("OnSettingClick");
		ShowDialog (Constant.pathPrefabs,Constant.DefaultDialog);
	}

	public void OnCaptureClick()
	{
		Debug.Log ("OnCaptureClick");

	}

	public void OnShopClick()
	{
		Debug.Log ("OnShopClick");

	}
	

}

/**
 *index trong data 
 */
enum INDEX_FISH_LEVEL{
	LEVEL=0,
	TOTAL_GOLD=1,
	REWARD_GOLD=2,
	TOTAL_FISH=3,
	F1=4,
	F2=5,
	F3=6,
	F4=7,
	F5=8,
	F6=9,
	F7=10,
	F8=11,
	F9=12,
	F10=13,
	F11=14,
	F12=15,
	F13=16,
	F14=17,
	F15=18,
	F16=19,
	F17=20,
};
