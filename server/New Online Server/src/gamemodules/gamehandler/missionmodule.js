"use strict";

var utils = require("../../utils/utils"),
    defines = require("../../defines"),
    _ = require('underscore'),
    resultCode = defines.ResultCode,
    logger = require('log4js').getLogger("USER"),
    async = require("async");
var propertiesModel = require('../../databasemodules/models/propertiesmodel');
var configManager = require("../../dataconfigmodules/configmanager");
var list = [];
exports.initMissionDetail = function()
{
    var detailConfigData = configManager.getTable(configManager.TableIndexs.MISSION);
    _.each(detailConfigData, function(val){
        var mission =
        {
            id : val.ID,
            name : val.Name,
            gold : val.Gold,
            diamond : val.Diamond
        };
        list.push(mission);
    });
};
var Mission = function(client,data,resp)
{
    var res={};
    res.retCode=resultCode.OK;
    res.missionList = list;
    resp(res);
}
exports.GetMission=Mission;

var Present = function(client,data,resp)
{
   console.log("nhan qua voi id: " + data.id);
}
exports.GetPresent=Present;
