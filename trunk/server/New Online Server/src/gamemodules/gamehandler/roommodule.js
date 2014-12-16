/**
 * Created by kakapro on 11/28/13.
 */
"use strict";

var defines = require('../../defines');
var resultCode = defines.ResultCode;
var gamedefines = require("../gamedefines");
var waitingRoom="waitingroom";

//var room = require('../../databasemodules/models/roommodel');
var configManager = require("../../dataconfigmodules/configmanager");
var _ = require('underscore');
var listRoom=[];
var position=[];
var _maxUser;
var currentUserInRoom = [];// so nguoi hien tai trong room
exports.listRoom = listRoom;
//var app = require("./../../app");
var FHCount_CCU = 0;
var TIME_PLAY_IN_MATCH=100;
var MAX_PLAYER_REALTIME_IN_SERVER = 50;// Max user play
var MIN_TIME_UPDATE_CLIENT_LOGIC=0.04;
var FHNetEvent_SubcribeResult={
    eventName : gamedefines.MessageType.Subscribe,
    roomName:'',
    roomType:1,//
    result:true
};
var FHNetEvent_RoomInfoReady=
{
    eventName : gamedefines.MessageType.RoomReady,
    roomType:false,//true - diamand    false- gold
    routeID:0,
    timePlay:0,
    timeUpdate:0.1,
    RoomName:'',
    kindPlay:1,// 0 : play with another, 1 : play with AI
    player_Names:'',
    player_UIDs:'',
    player_locations:'',// location socket
    price:0,
    status:false//true: can't join- false:
};


var FHNetEvent_SycnReady={
    eventName : gamedefines.MessageType.SycnReady,
    timeServer:0,
    UID:'',
    CountReady:0
};
var FHNetEvent_ClientLogLogic={
    eventName : gamedefines.MessageType.ClientLogic,
    timeServer:0,
    UID:'',
    listEvents:''
};
var FHNetEvent_ClientFinalResult={
    UID:'',
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

//var CreateListRoomDefault= function()
//{
//    for(var i=0;i<6;i++)
//    {
//        console.log("========createroom: "+i);
//        var newRoom =room.createDefault(i);
//
//        room.save( newRoom, function(err) {
//            if (err) return console.error(err);
//
//        });
//    }
//}
//exports.CreateRooms =CreateListRoomDefault;
//exports.InitRooms = function()
//{
//    room.mongoModel.find(function(err,resFind)
//    {
//        if (err) return console.error(err);
//        if(!resFind ||resFind.length<=0)
//        {
//            CreateListRoomDefault();
//            console.log("Reset data again")
//            return;
//        }
//        for(var i=0;i<resFind.length;i++)
//        {
//            var r = room.createDefault(0);
//            r.id=resFind[i].id;
//            r.name=resFind[i].name;
//            r.price=resFind[i].price;
//            r.skintype=resFind[i].skintype;
//            r.maxUser=resFind[i].maxUser;
//            r.isDiamond=resFind[i].isDiamond;
//
//            listRoom.push(r);
//            currentUserInRoom.push(0)
//            var p= [0, 0, 0];
//            position.push(p);
//        }
//
//    });
//}
exports.InitRooms=function()
{
    var detailConfigData = configManager.getTable(configManager.TableIndexs.ROOM);
    _.each(detailConfigData,function(val)
    {
        var room={
            id: val.ID,
            name:val.Name,
            price:val.Price,
            skintype:val.Skintype,
            roomType:val.RoomType,
            maxUser:val.MaxUser,
            roomStatus:true
        };
        listRoom.push(room);
        currentUserInRoom.push(0)
        var p= [0, 0, 0];
        position.push(p);
    });
}

var GetRooms =function(client,data,resp)
{
    var res={};
    res.retCode=resultCode.OK;
    for(var i=0;i<listRoom.length;i++)
    {
        var _clientInRoom=client.server.clients(listRoom[i].name);
        if(_clientInRoom.length==0)
            listRoom[i].roomStatus=true;
    }
    res.rooms=listRoom;
    client.rooms=listRoom;

    res.positions=OnCurrentPosition(client);
    resp(res);
}
exports.GetListRoom=GetRooms;

exports.OnJoinWaitingRoom=function OnJoinWaitingRoom(client,data,resp)
{
    var res={};
    try{
        client.socket.join(waitingRoom);
        res.result=true;
    }catch (e){
        res.result=false;
    }
    resp(res);
}

function OnLeaveWaitingRoom(client)
{
    client.socket.leave(waitingRoom);
    console.log("Leave waiting room");
}

exports.OnUserJoinRoom=function OnUserJoinRoom(client,dataJoinRoom)
{
    client.positions=-1;
    client.indexroom=-1;
    var _userName= dataJoinRoom.userName;
    var _roomId= dataJoinRoom.roomId;
    var _position=dataJoinRoom.position;
    var indexroom=GetIndexRoom(_roomId,client);

    if(_userName == null || _roomId == null)
    {
        console.log("subscribe message error");
        return;
    }
    if(indexroom < 0 || indexroom >= listRoom.length)
    {
        console.log("room type is not define on server: "+indexroom+",  "+listRoom.length);
        return;
    }
    client.userName = _userName;// set socket User Name
    var roomjoin=GetOneRoom(_roomId,client);
    client.indexroom=indexroom;
    client.positions=_position;
    client.roomName=roomjoin.name;
    var  skindRoom=roomjoin.skintype;
    //check position in room.
    if(position[indexroom][client.positions]==1)
    {
        var rs="false";
        client.socket.emit(gamedefines.MessageType.ResultJoinRoom+"", {result:rs});
        return;
    }
    position[indexroom][client.positions]=1;
    OnRefeshPosition(client);
    if(skindRoom==1)//Ai
    {
        OnUserPlayAuto(client,roomjoin,indexroom);
    }else//Player
    {
        OnUserSubscribe(client,roomjoin,indexroom);
    }
}

function OnUserSubscribe(client, roomjoin,indexroom)
{
//    console.log("OnUserSubscribe: "+roomjoin.name);
    var _roomName = roomjoin.name;
    _maxUser=roomjoin.maxUser;
    var _roomClients = client.server.clients(_roomName);// get all client in Room
    var _numberClient = _roomClients.length;

    if(FHCount_CCU>_maxUser)// MAX USER CCU ON SERVER
    {
        FHNetEvent_SubcribeResult.result = false;
        client.socket.emit(gamedefines.MessageType.SubscribeResult + "",FHNetEvent_SubcribeResult);
        return;
    }
    FHNetEvent_SubcribeResult.result = true;

    if(_numberClient < _maxUser)
    {
        FHNetEvent_SubcribeResult.roomName = _roomName;
        FHNetEvent_SubcribeResult.roomType = roomjoin.roomType;
        client.socket.emit(gamedefines.MessageType.SubscribeResult + "",FHNetEvent_SubcribeResult);
        client.socket.join(_roomName);
        var rs="true";
        client.socket.emit(gamedefines.MessageType.ResultJoinRoom+"", {result:rs});
    }

    _roomClients = client.server.clients(_roomName);// get all client in Room after connect new
    var _numberClientNow = _roomClients.length;

    if (_numberClientNow == _maxUser)// enough player to play game
    {
        // to access game
        FHNetEvent_RoomInfoReady.roomType = roomjoin.roomType;
        FHNetEvent_RoomInfoReady.routeID = Math.floor(Math.random()*10);
        FHNetEvent_RoomInfoReady.RoomName = _roomName;
        FHNetEvent_RoomInfoReady.timeUpdate=MIN_TIME_UPDATE_CLIENT_LOGIC;
        FHNetEvent_RoomInfoReady.timePlay=TIME_PLAY_IN_MATCH;
        FHNetEvent_RoomInfoReady.player_Names = '';
        FHNetEvent_RoomInfoReady.player_UIDs = '';
        FHNetEvent_RoomInfoReady.player_locations = '';
        FHNetEvent_RoomInfoReady.kindPlay=0;
        FHNetEvent_RoomInfoReady.price=roomjoin.price;
        for (var i = 0; i<_maxUser; i += 1) {
            _roomClients[i].roomName=_roomName;
            _roomClients[i].CountUserReady=0;
            FHNetEvent_RoomInfoReady.player_Names += _roomClients[i].client.userName + ' $$';
            FHNetEvent_RoomInfoReady.player_UIDs += _roomClients[i].client.uid + '$$';
            FHNetEvent_RoomInfoReady.player_locations += _roomClients[i].client.positions + '$$';
        }
        for (var i = 0; i < _numberClientNow&&i<_maxUser; i ++) {
            if(_roomClients[i].isReady==null||_roomClients[i].isReady==false)
            {
                _roomClients[i].emit(gamedefines.MessageType.RoomReady+'', FHNetEvent_RoomInfoReady);
                _roomClients[i].isReady=true;
            }
        }
        listRoom[indexroom].roomStatus=false;
    }
    console.log("number of client:"+_numberClient+","+_numberClientNow);
}
//get all position in waiting room, the first time login
function OnCurrentPosition(client)
{
    var positions="";
    for(var i=0;i<position.length;i++)
    {
        for(var j=0;j<position[i].length;j++)
        {
            positions+=position[i][j]+"$$";
        }
    }
    return positions;
}

function OnRefeshPosition(client)
{
    var _roomClients = client.server.clients(waitingRoom);// get all client in waitting Room
    var _numberClientNow = _roomClients.length;
    var positions="";
    for(var i=0;i<position.length;i++)
    {
        for(var j=0;j<position[i].length;j++)
        {
            positions+=position[i][j]+"$$";
        }
    }
    for (var i = 0; i < _numberClientNow; i += 1) {
        _roomClients[i].emit(gamedefines.MessageType.SysPosition+'', {positions: positions});
    }
}

function OnUserPlayAuto(client, roomJoin,indexroom)
{
    console.log("OnUserPlayAuto");

    _maxUser=roomJoin.maxUser;
    var _roomName=roomJoin.name;
    var _roomNameAI = _roomName;//userName la roomName (temp:chua fix lai client)
    client.socket.join(_roomNameAI);
    client.roomName = _roomNameAI;

    var _roomClientsAI = client.server.clients(_roomNameAI);// get all client in Room after connect new
    var _numberClientNowAI = _roomClientsAI.length;
    if (_numberClientNowAI >= _maxUser)
    {
        return; // have full play with another user
    }
    // play auto when time wait too long
    FHNetEvent_RoomInfoReady.roomType = roomJoin.roomType;
    FHNetEvent_RoomInfoReady.routeID = Math.floor(Math.random()*10);
    FHNetEvent_RoomInfoReady.RoomName = _roomNameAI;
    FHNetEvent_RoomInfoReady.timeUpdate=MIN_TIME_UPDATE_CLIENT_LOGIC;
    FHNetEvent_RoomInfoReady.timePlay=TIME_PLAY_IN_MATCH;
    FHNetEvent_RoomInfoReady.player_Names = '';
    FHNetEvent_RoomInfoReady.player_UIDs = '';
    FHNetEvent_RoomInfoReady.player_locations = '';
    FHNetEvent_RoomInfoReady.kindPlay=1;

    for (var i = 0; i < _numberClientNowAI && i < _maxUser; i += 1) {
        _roomClientsAI[i].roomName=_roomNameAI;
        _roomClientsAI[i].CountUserReady=0;
        FHNetEvent_RoomInfoReady.player_Names += _roomClientsAI[i].userName + ' $$';
        FHNetEvent_RoomInfoReady.player_UIDs += _roomClientsAI[i].uid + '$$';
        FHNetEvent_RoomInfoReady.player_locations += position[indexroom][i] + '$$';
    }
    for (var i = 0; i < _numberClientNowAI && i < _maxUser; i += 1) {
        if(_roomClientsAI[i].isReady==null||_roomClientsAI[i].isReady==false)
        {
            _roomClientsAI[i].emit(gamedefines.MessageType.RoomReady + '', FHNetEvent_RoomInfoReady);
            _roomClientsAI[i].isReady=true;
        }
    }
}

function GetOneRoom(_roomId,client)
{
    for(var i=0;i<client.rooms.length;i++)
    {
        if(listRoom[i].id==_roomId)
            return listRoom[i];
    }
}
//lay index trong danh sach room
function GetIndexRoom(_roomId,client)
{
    for(var i=0;i<listRoom.length;i++)
    {
        if(listRoom[i].id==_roomId)
            return i;
    }
}

function OnLeaveRoom (client,roomname)
{
    if(client.indexroom<0 || client.positions<0)
        return;
    try{
        position[client.indexroom][client.positions]=0;
        client.socket.leave(roomname);
        var _roomClients = client.server.clients(client.roomName);
        if(_roomClients.length==0)
        {
            listRoom[client.indexroom].roomStatus=true;
        }
        var now=new Date();
        var inforLeaveRoom=client.roomName+"$$"+client.userName+"$$"+now.getTime();
        for(var i=0;i<_roomClients.length;i++)
        {
            _roomClients[i].emit(gamedefines.MessageType.UserLeaveRoom+'', {inforLeaveRoom: inforLeaveRoom});
        }
    }catch(e)
    {
        console.log("===============error=======leave room: "+e);
        return;
    }
    OnRefeshPosition(client);
}

exports.OnLeaveWaitingRoom=function(client)
{
    OnLeaveWaitingRoom(client);
}

exports.OnLeaCurrentRoom=function(client)
{
    if(client==null || client.roomName==null)
    return;
    OnLeaveRoom(client,client.roomName);
    client.roomName=null;
}

exports.OnDestroyMe=function (client)
{
    OnLeaveWaitingRoom(client);
    OnLeaveRoom(client,client.roomName);
}

exports.OnUserReady = function OnUserReady(client, sysReadyData)
{
    console.log("OnUserReady");
    // check logic....etc
    FHNetEvent_SycnReady.UID = sysReadyData.UID;
    var now=new Date();
    FHNetEvent_SycnReady.timeServer=now.getTime();

    var _roomNameReady = client.roomName;
    var _roomClients = client.server.clients(_roomNameReady);// get all client in Room after connect new
    var _numberClientNow = _roomClients.length;

    for (var i = 0; i < _numberClientNow; i ++) {
        _roomClients[i].CountUserReady++;
    }
    FHNetEvent_SycnReady.CountReady = client.socket.CountUserReady;
    for (var i = 0; i < _numberClientNow; i ++) {
        _roomClients[i].emit(gamedefines.MessageType.SycnReady+'', FHNetEvent_SycnReady);
    }
}

exports.OnUserLogClientLogic = function OnUserLogClientLogic(client, sysReadyData)
{
    // check logic....etc
    var _roomNameLogic = client.socket.RoomName;
    FHNetEvent_ClientLogLogic.UID = sysReadyData.UID;
    FHNetEvent_ClientLogLogic.listEvents = sysReadyData.listEvent ;
    var now=new Date();
    FHNetEvent_ClientLogLogic.timeServer=now.getTime();

    var _roomClients = client.server.clients(_roomNameLogic);// get all client in Room after connect new
    var _numberClientNow = _roomClients.length;

    // send for another user in room
    for (var i = 0; i < _numberClientNow; i++) {
        if(_roomClients[i].client.uid != client.uid)// not sent for owner
        {
            _roomClients[i].emit(gamedefines.MessageType.ClientLogic + '', FHNetEvent_ClientLogLogic);
        }
    }
}

//exports.OnUserFinalResult = function OnUserFinalResult(socket, sysReadyData)
//{
//    var _roomNameFinal = socket.RoomName;
//    console.log('Room name Final is:'+_roomNameFinal);
//    FHNetEvent_ClientFinalResult.UID = sysReadyData.UID;
//    FHNetEvent_ClientFinalResult.score = sysReadyData.score;
//    io.sockets.in(_roomNameFinal).emit(gamedefines.MessageType.FinalResult+'', FHNetEvent_ClientFinalResult);
//}
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

    if(_numberRe >= _maxUser)
    {
        return;
    }
    for (i = 0; i < _numberRe; i++) {
        if(_roomClientRe[i].uid == socket.uid)// not sent for owner
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


