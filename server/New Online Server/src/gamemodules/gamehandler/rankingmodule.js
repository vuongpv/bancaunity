"use strict";

var utils = require("../../utils/utils"),
    defines = require("../../defines"),
    _ = require('underscore'),
    resultCode = defines.ResultCode,
    logger = require('log4js').getLogger("USER"),
    async = require("async");
var propertiesModel = require('../../databasemodules/models/propertiesmodel');


var DailyRanking = function(client,data,resp)
{
    var res={};
    res.retCode=resultCode.OK;
    var rankingList = [];
    propertiesModel.onLoadAll(function(callback){
       _.each(callback, function(item)
        {
            var it =
            {
                uid : item.uid,
                name : item.name,
                gold : item.gold,
                diamond : item.diamond,
                score : item.score
            };
            rankingList.push(it);
        });
        rankingList = _.sortBy(rankingList, function(item)
        {
            return -item.score;
        });
        res.dailyRanking=rankingList;
        resp(res);
    });
}
exports.GetDailyRanking=DailyRanking;