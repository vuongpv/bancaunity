/**
 * Created by hiepdhd on 11/20/14.
 */

"use strict";

var utils = require("./../utils/utils"),
    defines = require("./../defines"),
    resultCode = defines.ResultCode,
    logger = require('log4js').getLogger("USER"),
    async = require("async");

var accountModel = require('../databasemodules/models/accountmodel');
var propertiesModel = require('../databasemodules/models/propertiesmodel');

module.exports = User;

function User(client) {
    this.client = client;
    this.uid = client.uid;

    this.properties = propertiesModel.createDefault();

    //assign uid
    this.properties.uid = this.uid;
}

User.prototype.loadDataFromDB = function (callbackFunc) {
    var uid = this.uid;
    var self = this;

    // logger.info("START load DB for " + self.client.userName);

    async.parallel([
        function (callback) {
            propertiesModel.load(uid, function(err,result){
                if(err)
                {
                    callback(err);//has error
                    return;
                }

                if(result != null){
                    utils.copyModelToObject(result, self.properties);
                }
                else
                    logger.error("ko tim thay properties data cua " + uid);

                callback();//ok
            });
        }
        //,

    ], function (err) {
        if (err) {
            logger.error("loadDataFromDB with error:" + err);
            callbackFunc(err);
            return;
        }

        logger.info("LOAD DB of " + self.client.userName + " OK");

        callbackFunc(resultCode.OK);
    });
};

User.prototype.saveDataToDB = function (callbackFunc) {

    var self = this;
    var uid = self.uid;

    async.parallel([
        function (callback) {
            propertiesModel.save(uid, self.properties, function(err){
                if(err)
                    callback(err);
            });
        }

    ], function (err) {
        if (err) {
            logger.error("saveDataToDB with error :" + err);
            return;
        }
        callbackFunc();
    });
};
