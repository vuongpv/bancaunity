"use strict";


var logger = require('log4js').getLogger("APP");
logger.setLevel("DEBUG");
var uuid = require('node-uuid');

//console.log(uuid.v1());
//console.log(uuid.v1());
//console.log(uuid.v1());
//console.log(uuid.v1());
//console.log(uuid.v1());
//console.log(uuid.v1());
//console.log(uuid.v1());
//return;
var io = require('socket.io').listen(3000);
/////////// START CONFIG SOCKET ///////////////////////////////////////////////////////////////
io.set('close timeout', 60);//default la 60. Thoi gian de client reconnect neu bi close.
io.set('heartbeat timeout', 60);//default la 60. thoi gian timeout client phai gui heartbeat cho server
io.set('heartbeat interval', 25);//default la 25, thoi gian timeout server gui heart beat cho client
io.set('log level', 2);//log level 0:error, 1 warn, 2 info, 3 debug
//////////////////////////////////////////////////////////////////////////////////////

//create game server
exports.gameServerIO = require('./gamemodules/gameserver').start(io);

logger.debug("server is running");


