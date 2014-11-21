"use strict";


var logger = require('log4js').getLogger("CLIENT")
    , _ = require('underscore')
    , defines = require('../defines')
    , utils = require('../utils')
    , async = require("async")
    , gameserver = require('./gameserver')
    , uuid = require('node-uuid');

var roomModule = require("./gamehandler/roommodule");

var requestType = require("./gamedefines").RequestType;
var messageType = require("./gamedefines").MessageType;

logger.setLevel("DEBUG");

//handler module
//var loginModule = require("./gamehandler/loginmodule");


module.exports.Client = function (socket, server)
{
    logger.info("client " + socket.id + " connect to server");



    //client members, save for convenient using
    this.clientId = socket.id;
    gameserver.mapClientsByClientID = this.clientId;

    this.uid = null;
    this.userName = null;
    this.nickName = null;
    this.roomName = null;

    //user data
//    this.userData = null;//tham chieu toi data DB

    //view data tinh sau
    this.playerDataView = {
        pos: null,
        actpos: -1
    };
    //end member

    //save parent
    this.socket = socket;//save nguoc lai socket, bi loop du lieu nen ko to JSON string dc
    this.server = server;

    //save client vao socket de search trong room
    socket.client = this;

    //for using in functions
    var self = this;

    var onMessage = function (data) {

        var type = parseInt(data.type);
        logger.info("handle message: type=" + type + " data=" + JSON.stringify(data));

        var handler = messageHandlers[type];
        if (handler != null)
        {
            if(type != messageType.TestPacket)
            {
//                if(self.userData == null)
//                {
//                    logger.error("Chua login thi lam an gi dc. hack ah " + self.clientId);
//                    self.socket.emit(defines.MessageToClient.HasError, {error_code: defines.ResultCode.NOT_FOUND});
//                    return;
//                }
            }

            handler(self, data);

        }
        else
        {
            logger.error("unprocessed message " + type);
        }

    };

    var onRequest = function (data, resp) {

        var type = parseInt(data.type);
        logger.info("handle request: type=" + type + " data=" + JSON.stringify(data));

        var handler = requestHandlers[type];
        if (handler != null)
        {
            if(type != requestType.Login)
            {
//                if(self.userData == null)
//                {
//                    logger.error("Chua login thi lam an gi dc. hack ah " + self.clientId);
//                    self.socket.emit(defines.MessageToClient.HasError, {error_code: defines.ResultCode.NOT_FOUND});
//                    return;
//                }
            }

            handler(self, data, resp);

        }
        else
        {
            logger.error("unprocessed request " + type);
        }

    };

    var onDisconnect = function () {
        console.log("onDisconnect");
        destroyMe();
    };

    var updateClient = function()
    {
        /*if(self!= null && self.userData != null)
        {
            self.userData.checkSyncUpdateData();
        }*/
    };

    var destroyMe = function()
    {
        logger.debug("destroyMe:" + socket.client);

        if(self.uid != null && gameserver.mapClientsByUid.hasOwnProperty(self.uid) && gameserver.mapClientsByUid[self.uid] != null)
            delete gameserver.mapClientsByUid[self.uid];

        if(socket.client != null)
        {
            socket.client = null;
            delete socket.client;
        }

//        clearInterval(updateInteval);
//        updateInteval = 0;

        socket = null;
        self = null;
    };

    //////////////////////////////////////////////////////////////////////////////

    //send connect message to client
    socket.emit(defines.MessageToClient.Connected, {'id': socket.id});

    //receive all message from socket
    socket.on('message',onMessage);

    socket.on('request',onRequest);

    //when disconnected
    socket.on('disconnect',onDisconnect);

//    var updateInteval = setInterval(updateClient, 1000);//update moi 1s

    //////////////////////////////////////////////////////////////////////////////////////

    var requestHandlers = {};

    //login
//    requestHandlers[requestType.Login]          = loginModule.onLogin;
//    requestHandlers[requestType.CreateUserInfo] = loginModule.onCreateUserInfo;


    //register handlers
    var messageHandlers = {};

    //Test function
//    messageHandlers[messageType.TestFunc] = friendModule.onRequestTestFunc;
    messageHandlers[messageType.Subscribe] = roomModule.OnUserSubscribe;
    messageHandlers[messageType.SycnReady] = roomModule.OnUserReady;
    messageHandlers[messageType.ClientLogic] = roomModule.OnUserLogClientLogic;
    messageHandlers[messageType.FinalResult] = roomModule.OnUserFinalResult;
    messageHandlers[messageType.BenchMark] = roomModule.OnBenchMark;
    messageHandlers[messageType.PlayAuto] = roomModule.OnUserPlayAuto;
    messageHandlers[messageType.ReConnect] = roomModule.OnUserReconnect;

};




