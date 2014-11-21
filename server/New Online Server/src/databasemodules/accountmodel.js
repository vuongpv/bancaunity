/**
 * Created by kakapro on 12/17/13.
 */

"use strict";

var uuid = require('node-uuid');
var db = require("./../databasemodules/dbmanager");
var logger = require('log4js').getLogger("ACCOUNT");
logger.setLevel("DEBUG");
var defineResult = require("./../defines").ResultCode;
var utils = require("./../utils");


module.exports = AccountModel;

function AccountModel() {
    logger.debug("Constructor AccountModel");
    this.type = "account";
    this.uid = "";
    this.userName = "";
    this.isInited = false;
    this.isLocked = false;
}


AccountModel.create = function (username) {
    var a = new AccountModel();
    a.uid = uuid.v1();
    a.userName = username;
    return a;
};

AccountModel.updateInitStatus = function (username, callback) {
    load(username, function (err, result) {
        if (err) {
            callback(err);
        }
        else {
            result.isInited = true;
            save(username, result, function (err) {
                if (err) {
                    callback(err);
                }
                else {
                    callback();
                }
            });
        }
    });
};

var save = AccountModel.save = function (username, data, callback) {
    if (data == null || username == null) {
        logger.error("data NULL roi ne");
        callback("Params null");
        return;
    }

    var docName = "account-" + username;
    db.setKey(docName, data, function (err) {
        if (err) {
            logger.error("save account with error" + err);
            callback(err);
        }
        else {
            callback();
        }
    });
};

var load = AccountModel.load = function (username, callback) {
    var docName = "account-" + username;
    //logger.debug("Load account for " + username);
    db.getKey(docName, function (err, result) {
        if (err) {
            if (err == db.DBError.KEY_NOT_FOUND)
                callback(null, null);
            else {
                logger.error("load account with error " + err);
                callback(err);
            }
            return;
        }
       // console.log(result);
        callback(null, result.value);
    });
};