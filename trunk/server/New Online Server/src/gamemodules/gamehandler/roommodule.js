/**
 * Created by hiepdhd on 11/12/14.
 */
"use strict";

var gamedefines = require("../gamedefines");
var io = require("../gameserver").io;

// global room ID identifier
var roomFH =
    [
        0,  // type bet default : ignore
        0,  // type bet 50
        0,  // type bet 100
        0,  // type bet 200
        0,  // type bet 500
        0,  // type bet 1000
        0   // type bet 5000
    ];
// constant varibale
var roomFHName =
    [
        "RoomType0_",   // prefix name room type01
        "RoomType1_",   // prefix name room type01
        "RoomType2_",
        "RoomType3_",
        "RoomType4_",
        "RoomType5_",
        "RoomType6_"
    ];
var roomAutoPlay =
    [
        0,  // type bet default : ignore
        0,  // type bet 50
        0,  // type bet 100
        0,  // type bet 200
        0,  // type bet 500
        0,  // type bet 1000
        0   // type bet 5000
    ];
var roomFHAutoPlayName =
    [
        "RoomAuto0_",   // prefix name room type01
        "RoomAuto1_",   // prefix name room type01
        "RoomAuto2_",
        "RoomAuto3_",
        "RoomAuto4_",
        "RoomAuto5_",
        "RoomAuto6_"
    ];

var MAX_TYPE_ROOM = 7; //0-6
var MAX_CLIENT_IN_ROOM = 2;
var TAX_PLAY_NORMAL = 5;// 5%
var TIME_PLAY_IN_MATCH=100;
var MAX_PLAYER_REALTIME_IN_SERVER = 5000;// Max user play
var MIN_TIME_UPDATE_CLIENT_LOGIC=0.04;
var FHCount_CCU = 0;
var FHCount_BenchMark = 0;

var FHNetEvent_SubcribeResult={
    eventName : gamedefines.MessageType.Subscribe,
    roomName:'',
    roomType:1,
    result:true
};
var FHNetEvent_RoomInfoReady=
{
    eventName : gamedefines.MessageType.RoomReady,
    roomType:1,
    routeID:0,
    timePlay:50,
    timeUpdate:0.1,
    RoomName:'',
    kindPlay:0,// 0 : play with another, 1 : play with AI
    player_Names:'',
    player_SIDs:'',
    player_locations:''// location socket
};

var FHNetEvent_PlayAuto={
    eventName : gamedefines.MessageType.PlayAuto,
};

var FHNetEvent_SycnReady={
    eventName : gamedefines.MessageType.SycnReady,
    timeServer:0,
    SID:'',
    CountReady:0
};
var FHNetEvent_ClientLogLogic={
    eventName : gamedefines.MessageType.ClientLogic,
    timeServer:0,
    SID:'',
    listEvents:''
};
var FHNetEvent_ClientFinalResult={
    SID:'',
    score:0
};
var FHNetEvent_BenchMark =
{
    CCU: 0,
    CountConnect: 0
};

var FHNetEvent_ReConnect={
    roomName:''
};

exports.OnUserSubscribe = function OnUserSubscribe(client, subscribeData)
{
    console.log("OnUserSubscribe")
    var _userName= subscribeData.userName;
    var _roomType= subscribeData.roomType;
    // check logic
    if(_userName == null || _roomType == null)
    {
        console.log("subscribe message error");
        return;
    }
    if(_roomType < 1 || _roomType >= MAX_TYPE_ROOM)
    {
        console.log("room type is not define on server");
        return;
    }
    client.userName = _userName;// set socket User Name

    var _roomName = GetRoomName(_roomType);

    var _roomClients = client.server.clients(_roomName);// get all client in Room
    var _numberClient = _roomClients.length;
    if(FHCount_CCU>MAX_PLAYER_REALTIME_IN_SERVER)// MAX USER CCU ON SERVER
    {
        FHNetEvent_SubcribeResult.result = false;
        client.socket.emit(gamedefines.MessageType.SubscribeResult + "",FHNetEvent_SubcribeResult);
        return;
    }
    FHNetEvent_SubcribeResult.result = true;

    if(_numberClient < MAX_CLIENT_IN_ROOM)
    {
        FHNetEvent_SubcribeResult.roomName = _roomName;
        FHNetEvent_SubcribeResult.roomType = _roomType;
        client.socket.emit(gamedefines.MessageType.SubscribeResult + "",FHNetEvent_SubcribeResult);
        //console.log(FHNetEvent_SubcribeResult);
        client.socket.location = _numberClient;// set location Socket
        client.socket.join(_roomName);
    }

    _roomClients = client.server.clients(_roomName);// get all client in Room after connect new
    var _numberClientNow = _roomClients.length;
    if (_numberClientNow == MAX_CLIENT_IN_ROOM)// enough player to play game
    {
        //console.log("AAAAAAAAAAAAAAAAAA");
        // to access game
        FHNetEvent_RoomInfoReady.roomType = _roomType,
        FHNetEvent_RoomInfoReady.routeID = Math.floor(Math.random()*10);
        FHNetEvent_RoomInfoReady.RoomName = _roomName;
        FHNetEvent_RoomInfoReady.timeUpdate=MIN_TIME_UPDATE_CLIENT_LOGIC;
        FHNetEvent_RoomInfoReady.timePlay=TIME_PLAY_IN_MATCH;
        FHNetEvent_RoomInfoReady.player_Names = '';
        FHNetEvent_RoomInfoReady.player_SIDs = '';
        FHNetEvent_RoomInfoReady.player_locations = '';
        FHNetEvent_RoomInfoReady.kindPlay=0;
        for (var i = 0; i < _numberClientNow && i<MAX_CLIENT_IN_ROOM; i += 1) {
            _roomClients[i].RoomName=_roomName;
            _roomClients[i].CountUserReady=0;
            FHNetEvent_RoomInfoReady.player_Names += _roomClients[i].userName + ' $$';
            FHNetEvent_RoomInfoReady.player_SIDs += _roomClients[i].id + '$$';
            FHNetEvent_RoomInfoReady.player_locations += i + '$$';
            //console.log(_roomClients[i].id);
            //console.log(_roomClients[i].userName);
        }
        for (var i = 0; i < _numberClientNow&&i<MAX_CLIENT_IN_ROOM; i += 1) {
            if(_roomClients[i].isReady==null||_roomClients[i].isReady==false)
            {
                _roomClients[i].emit(gamedefines.MessageType.RoomReady+'', FHNetEvent_RoomInfoReady);
                _roomClients[i].isReady=true;
            }
        }
        roomFH[_roomType]+=1;
        //io.sockets.in (_roomName).emit(gamedefines.MessageType.RoomReady+'', FHNetEvent_RoomInfoReady);
        console.log("Room online dual:"+_roomType);
    }

    //test
    console.log("number of client:"+_numberClient+","+_numberClientNow);

    console.log(_userName+"|"+_roomName);

}
exports.OnUserPlayAuto = function OnUserPlayAuto(client, userData)
{
    var _roomTypeAI = userData.roomType;
    var _roomBefore = userData.userName;//old room

    client.socket.leave(_roomBefore);
    var _roomNameAI = GetRoomPlayAuto(_roomTypeAI);//userName la roomName (temp:chua fix lai client)
    client.socket.join(_roomNameAI);
    client.socket.RoomName = _roomNameAI;
    console.log("play auto:"+_roomTypeAI+", "+_roomNameAI);

    var _roomClientsAI = client.server.clients(_roomNameAI);// get all client in Room after connect new
    var _numberClientNowAI = _roomClientsAI.length;
    if (_numberClientNowAI >= MAX_CLIENT_IN_ROOM)
    {
        return; // have full play with another user
    }

    // play auto when time wait too long
    FHNetEvent_RoomInfoReady.roomType = _roomTypeAI,
    FHNetEvent_RoomInfoReady.routeID = Math.floor(Math.random()*10);
    FHNetEvent_RoomInfoReady.RoomName = _roomNameAI;
    FHNetEvent_RoomInfoReady.timeUpdate=MIN_TIME_UPDATE_CLIENT_LOGIC+_roomTypeAI*0.01;
    FHNetEvent_RoomInfoReady.timePlay=TIME_PLAY_IN_MATCH;
    FHNetEvent_RoomInfoReady.player_Names = '';
    FHNetEvent_RoomInfoReady.player_SIDs = '';
    FHNetEvent_RoomInfoReady.player_locations = '';
    FHNetEvent_RoomInfoReady.kindPlay=1;

    for (var i = 0; i < _numberClientNowAI && i < MAX_CLIENT_IN_ROOM; i += 1) {
        _roomClientsAI[i].RoomName=_roomNameAI;
        _roomClientsAI[i].CountUserReady=0;
        FHNetEvent_RoomInfoReady.player_Names += _roomClientsAI[i].userName + ' $$';
        FHNetEvent_RoomInfoReady.player_SIDs += _roomClientsAI[i].id + '$$';
        FHNetEvent_RoomInfoReady.player_locations += i + '$$';
    }
    for (var i = 0; i < _numberClientNowAI && i < MAX_CLIENT_IN_ROOM; i += 1) {
        if(_roomClientsAI[i].isReady==null||_roomClientsAI[i].isReady==false)
        {
            _roomClientsAI[i].emit(gamedefines.MessageType.RoomReady + '', FHNetEvent_RoomInfoReady);
            _roomClientsAI[i].isReady=true;
        }
    }
    roomAutoPlay[_roomTypeAI]+=1;
    roomFH[_roomTypeAI]+=10;// trach loi room AI va Multi

    console.log("RoomtypeAI:"+_roomTypeAI+". old room is :"+_roomBefore);
}

///
/// function with Room
///
exports.OnUserReady = function OnUserReady(client, sysReadyData)
{
    //console.log("OnUserReady");
    // check logic....etc
    FHNetEvent_SycnReady.SID = sysReadyData.SID;
    var now=new Date();
    FHNetEvent_SycnReady.timeServer=now.getTime();

    var _roomNameReady = client.socket.RoomName;
    var _roomClients = client.server.clients(_roomNameReady);// get all client in Room after connect new

    var _numberClientNow = _roomClients.length;

    for (var i = 0; i < _numberClientNow; i ++) {
        _roomClients[i].CountUserReady++;
    }
    FHNetEvent_SycnReady.CountReady = client.socket.CountUserReady;
    for (var i = 0; i < _numberClientNow; i ++) {
        _roomClients[i].emit(gamedefines.MessageType.SycnReady+'', FHNetEvent_SycnReady);
    }

    //io.sockets.in (_roomNameReady).emit(gamedefines.MessageType.SycnReady+'', FHNetEvent_SycnReady);
    console.log('Room name Ready is:'+_roomNameReady + "," + FHNetEvent_SycnReady.CountReady);
    //console.log('Count Client :'+_numberClientNow);
}

exports.OnUserLogClientLogic = function OnUserLogClientLogic(client, sysReadyData)
{
    //console.log("OnUserLogClientLogic:"+ socket.userName);

    // check logic....etc
    var _roomNameLogic = client.socket.RoomName;
    FHNetEvent_ClientLogLogic.SID = sysReadyData.SID;
    FHNetEvent_ClientLogLogic.listEvents = sysReadyData.listEvent ;
    var now=new Date();
    FHNetEvent_ClientLogLogic.timeServer=now.getTime();

    var _roomClients = client.server.clients(_roomNameLogic);// get all client in Room after connect new
    var _numberClientNow = _roomClients.length;

    // send for another user in room
    for (var i = 0; i < _numberClientNow; i++) {
        if(_roomClients[i].id != client.socket.id)// not sent for owner
        {
            //var _id=_roomClients[i].id;
            //console.log("OnUserLogClientLogic:"+_id);
            _roomClients[i].emit(gamedefines.MessageType.ClientLogic + '', FHNetEvent_ClientLogLogic);
        }
    }

    // send for all (test)
    //io.sockets.sockets.in (_roomNameLogic).emit(gamedefines.MessageType.ClientLogic+'', FHNetEvent_ClientLogLogic);
}

exports.OnUserFinalResult = function OnUserFinalResult(socket, sysReadyData)
{
    var _roomNameFinal = socket.RoomName;
    console.log('Room name Final is:'+_roomNameFinal);
    FHNetEvent_ClientFinalResult.SID = sysReadyData.SID;
    FHNetEvent_ClientFinalResult.score = sysReadyData.score;
    io.sockets.in(_roomNameFinal).emit(gamedefines.MessageType.FinalResult+'', FHNetEvent_ClientFinalResult);
}
exports.OnBenchMark = function OnBenchMark(socket, sysReadyData)
{
    FHNetEvent_BenchMark.CCU = Count_CCU;
    FHNetEvent_BenchMark.CountConnect = FHCount_BenchMark;
    socket.emit(gamedefines.MessageType.BenchMark + "",FHNetEvent_BenchMark);
}
exports.OnUserReconnect = function OnUserReconnect(socket, userReconnect)
{
    console.log("OnUserReconnect:" + userReconnect.roomName);
    var _roomReconnect = userReconnect.roomName;
    var _roomClientRe = io.sockets.clients(_roomReconnect);// get all client in Room after connect new
    var _numberRe = _roomClientRe.length;
    var _isExists=false;
    if(_numberRe >= MAX_CLIENT_IN_ROOM)
    {
        return;
    }
    for (i = 0; i < _numberRe; i++) {
        if(_roomClientRe[i].id == socket.id)// not sent for owner
        {
            _isExists=true;
        }
    }
    if(_isExists==false)
    {
        FHNetEvent_ReConnect.roomName = _roomReconnect;
        socket.join(_roomReconnect);
        socket.emit(gamedefines.MessageType.ReConnect + "", FHNetEvent_ReConnect);
    }
}


function GetRoomName(roomType)
{
    if(roomType<1 || roomType>=MAX_TYPE_ROOM)
    {
        roomType=1;
    }
    return roomFHName[roomType] + roomFH[roomType];
}
function GetRoomPlayAuto(roomType)
{
    if(roomType<1 || roomType >= MAX_TYPE_ROOM)
    {
        roomType=1;
    }
    return roomFHAutoPlayName[roomType] + roomAutoPlay[roomType];

}