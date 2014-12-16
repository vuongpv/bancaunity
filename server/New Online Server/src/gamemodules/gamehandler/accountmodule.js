/**
 * Created by kakapro on 11/28/13.
 */
"use strict";

var defines = require('../../defines');
var resultCode = defines.ResultCode;
var User = require("./../user");
var account = require('../../databasemodules/models/accountmodel');
var properties=require('../../databasemodules/models/propertiesmodel');
//var app = require("./../../app");
var utils = require('../../utils/utils');
exports.Login = function(client, data, resp) {
    var resResult = {
    };
    resResult.retCode = resultCode.Ok;

    var username = data.username;
    //check username existed
    if (username == null || username.length < 1) {
        console.log("Name = null");
        resp({retCode:resultCode.Ok});
        return;
    }


    username = username.replace(/-/g,'');//loai bo ky tu "-"
    username = username.toLowerCase();

    account.mongoModel.findOne({username: username}, function(err, res){

        if (err) return console.error(err);

        if(!res){
            console.log("chua co account nay -> tao moi");

            var newAcc = account.createDefault(username);

            account.save(newAcc.uid, newAcc, function(err) {
                if (err) return console.error(err);
                console.log("tao moi account thanh cong!");
                console.dir(newAcc);

                client.uid = newAcc.uid;
                client.userName = newAcc.username;
                resResult.uid=newAcc.uid;
                console.log("===========================new acount: "+newAcc.uid);
                client.userData = new User(client);

                client.userData.saveDataToDB(function(err){
                    if (err != resultCode.OK) {
                        console.log("saveDataToDB for " + newAcc.uid + " with error " + err);
                        resp({retCode:resultCode.DATABASE_ERROR});
//                        return;
                    }
                })

                resp(resResult);
            });
        }
        else{
            console.log("da co account nay roi ne");
            console.dir(res);
            client.uid = res.uid;
            client.userName = res.username;
            client.userData = new User(client);
            resResult.uid=res.uid;
            resResult.isMission = false;
            if(utils.getLenghtDayOfYear(res.createdate) <= 15)
                resResult.isMission = true;
            client.userData.loadDataFromDB(function (err) {
                if (err != resultCode.OK) {
                    console.log("loadDataFromDB for " + res.uid + " with error " + err);
                    resp({retCode:resultCode.DATABASE_ERROR});
//                    return ;
                }
            });
        }
        resp(resResult);
    });
}

exports.OnUpdateProperties= function OnUpdateProperties(client,data,resp)
{
    var _score=data.score;
    var _gold=data.gold;
    var _diamond=data.diamond;

    client.userData.properties.score=_score;
    client.userData.properties.gold=_gold;
    client.userData.properties.diamond=_diamond;

properties.update(client.userData.properties,function(err){
     if (err) return console.error(err);
     console.log("update thanh cong!");
     console.dir(client.userData.properties);
});
}

exports.GetProperties=function(client, data, resp)
{
    if(data.uid==null){
        console.log("======================== uid is null");
        return;
    }

    var  res={};
    res.retCode = resultCode.Ok;
    var uid = data.uid;
    var properties = client.userData.properties;
    var properties_value=properties.uid+"$$"+properties.name+"$$"+properties.gold+"$$"+properties.diamond+"$$"+properties.score;
    res.properties=properties_value;
    resp(res);
}