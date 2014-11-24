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
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Messages;


public class M_Chat_Connect:M_Chat_Base
{
	public string name;
	public string uid;
	public M_Chat_Connect (string name_,string uid_)
	{
		type = (int)ChatMessageToServer.ConnectChat;
		name = name_;
		uid = uid_;
	}
	
}

public class M_Chat_JoinRoom:M_Chat_Base
{
	public string room;
	
	public M_Chat_JoinRoom (string r)
	{
		type = (int)ChatMessageToServer.JoinChatRoom;
		room = r;
	}
	
}

public class M_Chat_LeaveRoom:M_Chat_Base
{
	public M_Chat_LeaveRoom ()
	{
		type = (int)ChatMessageToServer.LeaveChatRoom;
	}
}

public class M_Chat_Public:M_Chat_Base
{
	public int channel;
	public string mess;
	
	public M_Chat_Public (ChatChannel chan, string m)
	{
		type = (int)ChatMessageToServer.ChatPublic;
		mess = m;
		channel = (int)chan;
	}
	
}

public class M_Chat_Private:M_Chat_Base
{
	public string mess;
	public string uid;
	public M_Chat_Private (string m,string uid)
	{
		type = (int)ChatMessageToServer.ChatPrivate;
		mess = m;
		this.uid = uid;
	}
	
}

public class M_Chat_Action:M_Chat_Base
{
	public string id;
	public M_Chat_Action(string _id)
	{
		type = (int) ChatMessageToServer.eventType;
		id = _id;
	}
}

public class M_Chat_JoinParty :M_Chat_Base
{
	public string p;
	public M_Chat_JoinParty(string party)
	{
		p = party;
		type =(int) ChatMessageToServer.JoinChatParty;
	}
}
public class M_Chat_LeaveParty : M_Chat_Base
{
	public M_Chat_LeaveParty()
	{
		type = (int)ChatMessageToServer.LeaveChatParty;
	}
}



