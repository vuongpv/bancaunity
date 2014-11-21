/**
 * Created by kakapro on 11/28/13.
 */
"use strict";

var logger = require('log4js').getLogger("ACTION");
logger.setLevel("DEBUG");
var defines = require('../../defines');
//var app = require("./../../app");
exports.onPlayerMoveTo = function (client, data) {
    client.playerDataView.pos = data.pos;
    if(client.roomName)
        client.socket.broadcast.to(client.roomName).emit(defines.MessageToClient.OtherMoveTo, {'id': client.clientId, 'pos': data.pos});
};

exports.onPlayerInteractObject = function (client, data) {
    client.playerDataView.actpos = data.actposid;
    if(client.roomName)
        client.socket.broadcast.to(client.roomName).emit(defines.MessageToClient.OtherInteractWithObject, {'id': client.clientId, 'actpos': data.actposid});
};
exports.onPlayerPlayAnim = function (client, data) {

    if(client.roomName)
        client.socket.broadcast.to(client.roomName).emit(defines.MessageToClient.OtherPlayAnimation, {'id': client.clientId, 'anim': data.anim});
};
exports.onPlayerFishingState = function (client, data) {
    if(data==null)
    {
        return;
    }
    var res={
        clientId:client.clientId,
        pos:data["pos"],
        stateId:data["stateId"],
        fishId:data["fishId"]
    };
    //client.playerDataView.pos = res.pos;
    //console.log("Fishstate:"+JSON.stringify(res));
    //app.gameServerIO.in(client.roomName).emit(defines.MessageToClient.SyncFishState, res);

    if(client.roomName)
        client.socket.broadcast.to(client.roomName).emit(defines.MessageToClient.SyncFishState, res);
};