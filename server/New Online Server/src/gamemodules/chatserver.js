"use strict";
var  _ = require('underscore');
var defines = require('../defines');
var logger = require('log4js').getLogger("CLIENT");
var chatMessageFromClient =
{
    ConnectChat: 100,
    ChatPrivate: 101,
    ChatPublic: 102,
    JoinChatRoom: 103,
    LeaveChatRoom: 104,
    JoinChatParty: 105,
    LeaveChatParty: 106,
    ChatAction: 107
};
exports.start = function(io)
{
    var chat = io.of('/chatserver');
    chat.on('connection', function (socket) {
        socket.room = null;
        socket.on('message', function (data) {
            chatHandler(data);
        });
        socket.on('disconnect', function () {
            console.log("On disconnect: " + socket.room);
            socket.leave(socket.room);
        });
        var chatHandler = function (data) {
            var type = parseInt(data.type);
            logger.debug("handle chat message:type=" + type + " data=" + JSON.stringify(data));

            var handler = handlers[type];
            if (handler != null) {
                try
                {
                    handler(data);
                }
                catch(e)
                {
                    logger.error("handler chat " + type + " with error " + e + "\n" + e.stack);
                }
            }
            else {
                logger.error("unprocessed chat message " + type);
            }
        };
        var OnJoinRoom = function(data)
        {
            console.log("On join chat room: " + data.roomName);
            if(socket.room != null)
                socket.leave(socket.room);
            socket.room = data.roomName;
            socket.join(data.roomName);
        }
        var OnChatRoom = function(data)
        {
             var dataChat =
             {
                message: data["message"]
             };
             socket.broadcast.to(socket.room).emit(defines.MessageToClient.ChatPublic, dataChat);
        }
        var handlers = {};
        handlers[chatMessageFromClient.ChatPublic] = OnChatRoom;
        handlers[chatMessageFromClient.JoinChatRoom] = OnJoinRoom;
    });
};