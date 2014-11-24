//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FHUsersManager:Singleton<FHUsersManager>
{
	private FHUserMe userMe;

	public FHUserMe UserMe {
		get {
			return userMe;
		}
	}

	private Dictionary<string, FHUser> allUsers = new Dictionary<string, FHUser>();

	public Dictionary<string, FHUser> AllUsers {
		get {
			return allUsers;
		}
	}

	public void CreatePlayerMe(string id)
	{
		allUsers.Clear();

		userMe = new FHUserMe(id);
		allUsers[id] = userMe;

		Debug.LogWarning("Create me " + id);
	}

	public void AddPlayer(FHUser p)
	{
		if(allUsers.ContainsKey(p.ClientId))
		{
			Debug.LogError("Existed player?????? - " + p.ClientId);
			return;
		}

		allUsers.Add(p.ClientId, p);
	}

	public void RemovePlayer(string id)
	{
		allUsers.Remove(id);
	}

	public void ClearOthersPlayer()
	{
		List<FHUser> temp = new List<FHUser>(allUsers.Values);
		foreach(var p in temp)
		{
			if(!p.IsPlayerMe)
				allUsers.Remove(p.ClientId);
		}
	}

	public int GetNumOfOtherPlayer()
	{
		List<FHUser> temp = new List<FHUser>(allUsers.Values);
		int count = 0;
		foreach(var p in temp)
		{
			if(!p.IsPlayerMe)
				count++;
		}
		return count;
	}

//	public FHPlayerOnline FindPlayerByName(string name)
//	{
//		try{
//		FHPlayerOnline ret= allPlayers.Values.First<FHPlayerOnline>( a => a.NickName == name); 
//		return ret;
//		}catch(Exception e)
//		{
//			Debug.Log(e.Message);
//		}
//		return null;
//	}

//	public GameObject FindPlayerObjectByName(string name)
//	{
//		FHPlayerOnline p = FindPlayerByName(name);
//		if(p!=null &&p.OwnerObject !=null)
//			return p.OwnerObject;
//		Debug.LogError("Cannot find player with name " + name + " in " + allPlayers.Count);
//		return null;
//	}

	public FHUser FindPlayerById(string id)
	{
		if(id == null)
		{
			Debug.LogWarning("WTF");
			return null;
		}

		FHUser ret;
		if(allUsers.TryGetValue(id, out ret))
		{
			return ret;
		}
		Debug.LogWarning("Cannot find player with id " + id);

		return null;
	}

//	public GameObject FindPlayerObjectById(string id)
//	{
//		FHPlayerOnline p = FindPlayerById(id);
//		if(p != null && p.OwnerObject != null)
//			return p.OwnerObject;
//
//		return null;
//	}


	public GameObject CreatePlayerObject(FHUser data)
	{
		Debug.LogWarning("CreatePlayerObject - " + data.ClientId);
		
//		GameObject playerGO = AvGameObjectUtils.LoadGameObject( "Prefabs/Character/Char");
		GameObject playerGO = new GameObject();
		
		return playerGO;
	}

//	public bool IsLocalPlayer(GameObject go)
//	{
//		return playerMe != null && playerMe.OwnerObject == go;
//	}

//	public void SetShowAllPlayerObject(bool show)
//	{
//		List<FHPlayer> temp = new List<FHPlayer>(allPlayers.Values);
//		foreach(var p in temp)
//		{
//			p.OwnerObject.SetActive(show);
//		}
//	}
}

