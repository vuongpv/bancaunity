"use strict";

var _ = require("underscore");
var logger = require('log4js').getLogger("GAMESERVER");
logger.setLevel("DEBUG");
var utils = require("../utils/utils");
var client = require("./client");
var gamedefines = require("./gamedefines");

var mapClientsByClientID = exports.mapClientsByClientID = {};

var mapClientsByUid = exports.mapClientsByUid = {};//danh sach client dang online
//var loginUserIn24H = exports.loginUserIn24H = {count:0, avglv:0, listuser:{}};//danh sach nguoi choi login tu 24h truoc -> for random visit

//var TIME_CHECK_REMOVE_LOGINUSER = 1 * 3600;//interval time de check remove user da login trong 24h (seconds)
//var TIME_SAVE_LOGINUSER_IN_LIST = 24 * 3600;//thoi gian user dc save trong list (seconds)

exports.start = function (io) {

    var server = io.of('/gameserver');

    server.on('connection', function (socket) {
        //save in socket
        new client.Client(socket, server);
    });



    //check thoi gian de remove user trong list userlogin 24h
    //setInterval(checkToRemoveFromLoginList, TIME_CHECK_REMOVE_LOGINUSER * 1000);//1h check 1 lan

    return server;
};

//var saveToLoginMap = exports.saveToLoginMap = function(client)
//{
//    //save to map 24h
//    //check privacy
//    if(client.userData.properties.enableAccessHome == 0)//ko add con nay vao map
//        return;
//
//    if(loginUserIn24H.listuser.hasOwnProperty(client.uid))//co roi thi update
//    {
//        logger.warn("login roi, login lai thi update avg level " + client.uid);
//        var oldLv = loginUserIn24H.listuser[client.uid].level;
//
//        logger.debug("Da login: before" + loginUserIn24H.avglv);
//        var newLv = 1;//level mac dinh cua ng moi
//        if(client.userData != null)
//            newLv = client.userData.properties.level;//update level
//
//        var total = loginUserIn24H.avglv * loginUserIn24H.count - oldLv + newLv;
//
//        loginUserIn24H.avglv = (loginUserIn24H.count != 0) ?  total/ loginUserIn24H.count : 0;//update avg
//
//        loginUserIn24H.listuser[client.uid].level = newLv;//update level
//
//        logger.debug("Da login: after" + loginUserIn24H.avglv);
//    }
//    else//chua co thi them vao
//    {
//        logger.debug("Login moi: before" + loginUserIn24H.avglv);
//        var newLv = 1;
//        if(client.userData != null)
//            newLv = client.userData.properties.level;
//
//        loginUserIn24H.listuser[client.uid] = {uid:client.uid, time:utils.getCurrentTimeInSecond(), level:newLv, isFriend:false, isVisited:false};
//
//        var total = loginUserIn24H.avglv * loginUserIn24H.count + client.userData.properties.level;//tinh lai total
//        loginUserIn24H.count ++;// tang count
//        loginUserIn24H.avglv = (loginUserIn24H.count != 0) ?  total/ loginUserIn24H.count : 0;//update avg
//
//        logger.debug("Login moi: after" + loginUserIn24H.avglv);
//    }
//};

//var removeFromLoginMap = exports.removeFromLoginMap = function(client)
//{
//    if(loginUserIn24H.listuser.hasOwnProperty(client.uid))//co roi thi update)
//    {
//        //logger.debug("Remove: before: avg=" + loginUserIn24H.avglv + " total=" + (loginUserIn24H.avglv * loginUserIn24H.count) + "count=" + loginUserIn24H.count);
//
//        var total = loginUserIn24H.avglv * loginUserIn24H.count - client.level;//tinh lai total
//        loginUserIn24H.count --;// giam count
//        loginUserIn24H.avglv = (loginUserIn24H.count != 0) ?  total/ loginUserIn24H.count : 0;//update avg
//
//        //logger.debug("Remove: after: avg=" + loginUserIn24H.avglv + " total=" + total + "count=" + loginUserIn24H.count);
//
//        loginUserIn24H.listuser[client.uid] = null;
//        delete  loginUserIn24H.listuser[client.uid];
//    }
//};

//var checkToRemoveFromLoginList = function()
//{
    //var numOfLoginUser = _.size(loginUserIn24H.listuser);
    //logger.debug("check login user:" + numOfLoginUser);

//    logger.debug("Avg Lv:" + loginUserIn24H.avglv);
//
//    _.each(loginUserIn24H.listuser, function(loginUser, key){
//        var curTime = utils.getCurrentTimeInSecond();
//        //logger.debug("check " + loginUser.level);
//        if( curTime > loginUser.time + TIME_SAVE_LOGINUSER_IN_LIST)
//        {
//            removeFromLoginMap(loginUser);
//        }
//    });
//}



