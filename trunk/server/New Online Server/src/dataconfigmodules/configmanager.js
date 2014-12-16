/**
 * Created by Leticia on 11/29/13.
 */
"use strict";

var logger = require('log4js').getLogger("CONFIG");
var utils = require("../utils/utils");
var csv = require('ya-csv'),
    _ = require('underscore'),
    async = require('async'),
    defineDataConfig = require('./definedataconfig');

var mapData = [];
var TableIndexs = exports.TableIndexs = {
    "MISSION": 1,
    "ROOM":2
};

exports.loadData = function (onFinish) {

    async.parallel([
        function(callback)
        {
            loadConfigData("../config/ConfigMission.csv", "MISSION", defineDataConfig.missionDataColumns, "ID", true, false, callback);
        },
        function(callback)
        {
            loadConfigData("../config/ConfigRoom.csv", "ROOM", defineDataConfig.roomDataColumns, "ID", true, false, callback);
        }

    ],
    function(err){
        if(err)
        {
            logger.error("Error khi load config");
        }
        else
        {
            //logger.info("Load config finish");
            onFinish();
            Test();
        }
    });
};

var loadRoomData = function(callback)
{

};

exports.loadDynamicConfig = function(){
    logger.info("Load dynamic data");
//    loadConfigData("../config/server/ConfigRoom.csv","ROOMS",defineDataConfig.roomDataColumn,"Idx",true, false,function(){
//        require("../gamemodules/gamehandler/roommodule").initRoomData();
//    });

//    loadConfigData("../config/share/ConfigFish.csv", "FISH", defineDataConfig.fishDataColums, "id", true, false, function(){
//        logger.info("Finish load fish info");
//    });

};

exports.loadDynamicRoomConfig = function(){
    logger.info("Load dynamic Room data");

};

var Test = function () {
    //logger.debug("TEST READ CONFIG: ");

  // var present = getNicknameFilter();
    //console.log("Present");
   // console.log(present);

  //var str ="いつも";
  // console.log(str);
   // str = utf8_decode(str);
    //console.log(str);

};

exports.getNicknameFilter = function()
{
    var filter = getTable(TableIndexs.NICKNAMEFILTER);

    var res = [];
    _.each(filter, function(val){
        res.push(val.Value);
    });

    return res;
};

var getTableLength = exports.getTableLength = function(tableIndex) {
    if ( tableIndex < 0) {
        logger.error("cannot find table : ", tableIndex);
        return 0;
    }
    var tempTable = mapData[tableIndex];
    if (tempTable == null || typeof(tempTable) == "undefined") {
        logger.error("Cannot find table:", tableIndex);
        return 0;
    }
    return _.size(tempTable);
};

var getTable = exports.getTable = function (tableIndex) {
    //logger.debug("get table : ", tableIndex);

    if ( tableIndex < 0) {
        logger.error("cannot find table : ", tableIndex);
    }
    return mapData[tableIndex];
};

var getData = exports.getData = function (tableIndex, dataIndex) {
    //logger.debug("getData  index:", dataIndex, "table:", tableIndex);
    var tempTable = getTable(tableIndex);
    if (tempTable == null || typeof(tempTable) == "undefined") {
        logger.error("Cannot find table:", tableIndex);
        return null;
    }
    if (typeof (tempTable[dataIndex]) == "undefined") {
        logger.error("Cannot find data: " + dataIndex);
        return null;
    }
    return tempTable[dataIndex];
};

var getString = exports.getString = function( dataIndex)
{
    var row = getData(TableIndexs.STRINGS, dataIndex);
    if(row && row["JP"])
        return getData(TableIndexs.STRINGS, dataIndex)["JP"];//lay tieng nhat
    else
        return "" + dataIndex;
};


function loadConfigData(path, tabIndex, columnDefine, indexKey, keyIsNumber, isAppend, callback) {
    //logger.info("Start Load Config Data:" + tabIndex);
    var reader = csv.createCsvFileReader(path, {
        separator: '\t',
        quote: '"',
        escape: '"',
        comment: '',
        columnsFromHeader: true,
        encoding:'utf16le'
    });

    var tempData = {};

    reader.setColumnNames(columnDefine);

    var autoIndex = 0;
    reader.addListener('data', function (data) {
        if(!indexKey)//ko co key thi auto index
        {
            tempData[autoIndex++] = data;
        }
        else
        {
            if (keyIsNumber)
            {
                var _temp = parseInt(data[indexKey]);
                if (!isNaN(_temp)) {
                    tempData[data[indexKey]] = data;
                }
            }
            else
            {
                tempData[data[indexKey]] = data;
            }
        }
    });


    reader.addListener('end', function (data) {
        if(!isAppend)
        {
            mapData[TableIndexs[tabIndex]] = tempData;
            //logger.info("No append :" + tabIndex + ":" + _.size(mapData[TableIndexs[tabIndex]]));
        }
        else
        {
           // logger.info("Old append :" + tabIndex + ":" + _.size(mapData[TableIndexs[tabIndex]]));
          //  logger.info("Finish append :" + tabIndex + ":" + _.size(tempData));
            _.extend(mapData[TableIndexs[tabIndex]], tempData);
        }
        logger.debug("Finish get data of :" + tabIndex + ":" + _.size(tempData));
        if(callback)
            callback();
    });
}
