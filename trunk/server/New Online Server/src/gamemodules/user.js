/**
 * Created by ThuanTQ on 11/27/13.
 * module lien quan den quan ly du lieu user
 */

"use strict";

var utils = require("./../utils"),
     defines = require("./../defines"),
     logger = require('log4js').getLogger("USER"),
     async = require("async");

var accountModel = require('../databasemodules/accountmodel');

module.exports = User;

function User(client) {
    this.client = client;
    this.uid = client.uid;

    //this.account = accountModel.createDefault();

}