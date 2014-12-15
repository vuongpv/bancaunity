using UnityEngine;
using System.Collections;

public class FHUserMe : FHUser {

	public FHUserMe(string clientId):base(clientId)
	{
		this.isPlayerMe = true;
	}
}
