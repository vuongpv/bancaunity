/**
 * Created by hiepdhd on 11/20/14.
 */

"use strict";

var utils = require('../../utils/utils');
var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var uuid = require('node-uuid');
var logger = require('log4js').getLogger("ACCOUNT");
logger.setLevel("DEBUG");

module.exports = Account;

function Account() {
    this.uid = "";
    this.username = "";
    this.createdate = new Date();
}

Account.createDefault = function (username) {
    var a = new Account();
    a.uid = uuid.v1();
    a.username = username;
    a.createdate = Date.now();
    return a;
};

var AccountSchema = new Schema({
    uid: { type: String, default: '' },
    username: { type: String, default: '' },
    createdate: { type: Date, default: Date.now()}
});

Account.mongoModel = mongoose.model('account', AccountSchema);

Account.save = function (uid, data, callback) {
    if (data == null || uid == null) {
        logger.error("NULL ACCOUNT");
        callback("Params null");
        return;
    }

    var accModel = new Account.mongoModel();
    utils.copyObjectToModel(data, accModel);

    accModel.save(function(err, accModel) {
        if (err) return console.error(err);
        console.dir("Account save ne: " + accModel);
        callback();
    });
}
