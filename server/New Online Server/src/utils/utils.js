"use strict";
var _ = require('underscore');

exports.getCurrentDayOfYear = function ()// for average, not need exactly day of year, just use for compare
{
    var now = new Date();
    return now.getMonth() * 30 + now.getDay();
};

var getCurrentTimeInMs = exports.getCurrentTimeInMs = function ()// by mini_second
{
    var now = new Date();
    return now.getTime();
};

exports.getCurrentTimeInSecond = function ()// by second
{
    return Math.floor(getCurrentTimeInMs()/1000);
};


exports.cloneObjectData = function (obj) {
    return JSON.parse(JSON.stringify(obj));
};

//copy data obj1 -> obj2 (chi copy key co trong obj2, ko tinh obj con): loop cho obj ben trong neu co
//giu reference, ko clone
var copyObjectData =  function(obj1, obj2)
{
    //console.log("START COPY" + JSON.stringify(obj1) + "\n" + JSON.stringify(obj2));

    _.each(obj1, function(v,k){
        if(obj2.hasOwnProperty(k))
        {
            obj2[k] = v;
        }
    });

    //console.log("END COPY" + JSON.stringify(obj2));
};
exports.copyObjectData = copyObjectData;

//random so int, bao gom ca max
exports.randomInt = function(min, max)
{
    return Math.floor(Math.random() * (max-min+1)) + min;
};

//copy model trong mongoose -> object
exports.copyModelToObject = function(model, desObj) {
    //can phai chuyen ve dang object truoc
    var obj1 = model.toObject();

    _.each(obj1, function(v,k){
        if(desObj.hasOwnProperty(k))
        {
            desObj[k] = v;
        }
    });
}

exports.copyObjectToModel = function(obj, model) {
    _.each(obj, function(v, k){
        model[k] = v;
    });
}
exports.getLenghtDayOfYear = function (date)
{
    var now = new Date();
    var start = new Date(now.getFullYear(), 0, 0);
    // console.log("@@@@@@ " + now);
    // console.log("######## " + date);
    // console.log("%%%%%%%%%" + start);
    var diff = now - date;
    var oneDay = 1000 * 60 * 60 * 24;
    var day = Math.floor(diff / oneDay);
    // console.log("%%%%%%%%%" + day);

    return day;
}