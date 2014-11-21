/**
 * Created by kakapro on 19/11/2013.
 */
//cac ham ho tro

"use strict";
var defines = require('./defines');
var _ = require('underscore');
var configManager=require("./dataconfigmodules/configmanager");


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

/**
 * @return {boolean}
 */
exports.AddExp=function(client, point)
{
    try
    {
        var me = client.userData;
        var proper=me.properties;
        var exp=parseInt(proper.exp);
        var level=parseInt(proper.level);

        var nextLevel=level+1;
        if(nextLevel>defines.LIMIT_USER_LEVEL)
        {
            nextLevel=defines.LIMIT_USER_LEVEL;
        }
        exp+=point;
        var levelItem=GetLevelItem(nextLevel);
        var expNeed=parseInt(levelItem["xpPerLevel"]);

        if(exp>expNeed)//len level
        {
            exp-=expNeed;
            level=nextLevel;
        }
        proper.level=level;
        proper.exp=exp;
        return true;
    }
    catch(err)
    {
        console.log("Add Level Error:"+JSON.stringify(err));
        return false;
    }
};
/**
 * @return {boolean}
 */
exports.AddFishExp=function(client,point)
{
    try
    {
        var me = client.userData;
        var fishRod=me.inventory.fishItem.fishRod;
        var fishExp=parseInt(fishRod.exp);
        var fishLevel=parseInt(fishRod.level);

        var nextFishLevel=fishLevel+1;
        if(nextFishLevel>defines.LIMIT_USER_FISH_LEVEL)
        {
            nextFishLevel=defines.LIMIT_USER_FISH_LEVEL;
        }

        fishExp+=point;
        var levelItem=GetFishLevelItem(nextFishLevel);
        var expNeed=parseInt(levelItem["xpPerLevel"]);
        if(fishExp>expNeed)//len level
        {
            fishExp-=expNeed;
            fishLevel=nextFishLevel;
        }
        fishRod.level=fishLevel;
        fishRod.exp=fishExp;
        return true;
    }
    catch(err)
    {
        console.log("Add Level Error:"+JSON.stringify(err));
        return false;
    }
};

/**
 * @return {boolean}
 */
exports.addMint=function(client,goldAdd)
{
    try
    {
        var gold=parseInt(client.userData.properties.mint);
        if(gold<0)
        {
            gold=0;
        }
        if(goldAdd<0)
        {
            goldAdd=0;
        }
        gold+=goldAdd;
        client.userData.properties.mint=gold;
        return true;
    }
    catch(err)
    {
        console.log("Add Gold Error:"+JSON.stringify(err));
        return false;
    }
};
/**
 * @return {boolean}
 */
exports.subMint=function(client,goldSub)
{
    try
    {
        var gold=parseInt(client.userData.properties.mint);

        if(gold < goldSub)
            return false;

        if(gold<0)
        {
            gold=0;
        }
        if(goldSub<0)
        {
            goldSub=0;
        }
        gold-=goldSub;
        client.userData.properties.mint=gold;
        return true;
    }
    catch(err)
    {
        console.log("Sub gold Error:"+JSON.stringify(err));
        return false;
    }
};

/**
 * @return {boolean}
 */
exports.addDole=function(client,cashAdd)
{
    try
    {
        var cash=parseInt(client.userData.billing.dole);
        if(cash<0)
        {
            cash=0;
        }
        if(cashAdd<0)
        {
            cashAdd=0;
        }
        cash+=cashAdd;
        client.userData.billing.dole=cash;
        return true;
    }
    catch(err)
    {
        console.log("Add Cash Error:"+JSON.stringify(err));
        return false;
    }
};

/**
 * @return {boolean}
 */
exports.subDole=function(client,cashSub)
{
    try
    {
        var cash=parseInt(client.userData.billing.dole);

        if(cash < cashSub)
            return false;

        if(cash<0)
        {
            cash=0;
        }
        if(cashSub<0)
        {
            cashSub=0;
        }
        cash-=cashSub;
        client.userData.billing.dole=cash;
        return true;
    }
    catch(err)
    {
        console.log("SUB Cash Error:"+JSON.stringify(err));
        return false;
    }
};

var GetLevelItem=exports.GetLevelItem=function(level)
{
    return configManager.getData(configManager.TableIndexs.LEVEL,level);
};
var GetFishLevelItem=exports.GetFishLevelItem=function(level)
{
    return configManager.getData(configManager.TableIndexs.FISH_LEVEL,level);
};

