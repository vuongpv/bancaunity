using UnityEngine;
using System.Collections;

public class FHUser {

	protected string uid = "";
	
	public string Uid {
		get {
			return uid;
		}
		set{
			uid = value;
		}
	}
	
	protected string clientId = "";
	
	public string ClientId {
		get {
			return clientId;
		}
	}
	
	protected bool isPlayerMe = false;
	
	public bool IsPlayerMe {
		get {
			return isPlayerMe;
		}
	}
	
	public FHUser(string clientId)
	{
		this.clientId = clientId;
	}
}
