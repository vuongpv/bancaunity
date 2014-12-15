using UnityEngine;
using System.Collections;

public class RankingItem : MonoBehaviour {
	public UILabel name;
	public UILabel score;
	public UILabel index;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Init(RankingModel item, int _index)
	{
		name.text = item.name;
		score.text = item.score.ToString();
		index.text = _index.ToString();
	}
}
