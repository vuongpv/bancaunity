/**
 * Created by hiepdhd on 11/19/14.
 */
"use strict";

var uuid = require('node-uuid');
var mongoose = require('mongoose');

var logger = require('log4js').getLogger("DATABASE");

//var async = require("async");
var config = require("./../configs");

var utils = require("./../utils/utils");
var _ = require('underscore');

// configuration Database
var LOCAL_HOST = "localhost:27017";
//var DEV_HOST = "192.168.1.212:27017";//HIEP
var DEV_HOST = "192.168.1.215:27017";//DIEU
var REAL_HOST = "";
var room = require('../databasemodules/models/roommodel');

//thay doi de dung ke DB cua server khac (Chi dung trong moi truong DEV)
var DB_MODE = 1;//0: LOCAL, 1: DEV, 2: REAL

var DBType =
{
    SET: 1,
    GET: 2,
    REMOVE :3
};

var DBError = exports.DBError = {
    UNKNOWN: 1,
    KEY_ALREADY_EXIST: 2,
    KEY_NOT_FOUND: 3
};



exports.initDB = function (callback) {

    var dbConfig ={};
    dbConfig.hostname = LOCAL_HOST;
    dbConfig.username = "default";
    dbConfig.password = "";
    dbConfig.dbname = "WOF";

    switch (config.RUN_ENV)
    {
        case 0://chay tren local
            switch (DB_MODE)
            {
                case 0://xai DB localhost.
                    dbConfig.hostname = LOCAL_HOST;
                    dbConfig.username = "default";
                    dbConfig.password = "";
                    break;
                case 1://xai DB localhost.
                    dbConfig.hostname = DEV_HOST;
                    dbConfig.username = "default";
                    dbConfig.password = "";
                    break;
                case 2://xai DB server real
                    dbConfig.hostname = REAL_HOST;
                    dbConfig.username = "default";
                    dbConfig.password = "";
                    break;
            }

            break;

        case 1://config chay tren server real
            dbConfig.hostname = REAL_HOST;
            dbConfig.username = "default";
            dbConfig.password = "";
            break;
    }

    logger.debug("Database info:");
    console.log(dbConfig);

    //connect db
    var dbURI = "mongodb://" + dbConfig.username + ":" + dbConfig.password + "@" + dbConfig.hostname + "/" + dbConfig.dbname;
    mongoose.connect(dbURI);
    mongoose.connection.on('connected', function () {
        callback();
    });

    mongoose.connection.on('error',function (err) {
        callback('Mongoose default connection error: ' + err)
    });

    mongoose.connection.on('disconnected', function () {
        callback('Mongoose default connection disconnected')
    });
};