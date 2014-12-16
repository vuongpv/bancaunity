/**
 * Created by hiepdhd on 11/20/14.
 */

"use strict";

var utils = require('../../utils/utils');
var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var logger = require('log4js').getLogger("PROPERTIES");
logger.setLevel("DEBUG");

module.exports = Properties;

function Properties() {
    this.uid = '';
    this.name='demo';
    this.gold = 1000;
    this.diamond=0;
    this.score = 0;
}

Properties.createDefault = function () {
    return new Properties();
};

var PropertiesSchema = new Schema({
    uid: { type: String, default: '' },
    name: { type: String, default: 'demo' },
    gold: { type: Number, default: 1000 },
    diamond:{type: Number,default :0},
    score: { type: Number, default: 0 }
});

Properties.mongoModel = mongoose.model('properties', PropertiesSchema);

Properties.load = function (_uid, callback) {
    this.mongoModel.findOne({uid: _uid}, function(err, res){
        if (err) {
            logger.error("load properties with error" + err);
            callback(err);
            return;
        }
        callback(null, res);
    });
};
Properties.onLoadAll = function(callback)
{
    this.mongoModel.find({}, function(err, res){
        if (err) {
            logger.error("load properties with error" + err);
            callback(err);
            return;
        }
        callback(res);
    });
};
Properties.save = function (uid, data, callback) {
    if (data == null || uid == null) {
        logger.error("NULL PROPERTIES");
        callback("Params null");
        return;
    }

    var propModel = new Properties.mongoModel();
    utils.copyObjectToModel(data, propModel);

    propModel.save(function(err, propModel) {
        if (err) return console.error(err);
        console.dir("Properties save ne: " + propModel);
        callback();
    });
}

Properties.update=function(data,callback)
{
    if(data==null || data.uid==null)
    {   logger.error("NULL PROPERTIES");
        callback("Params null");
        return;
    }

    this.mongoModel.update({uid:data.uid},{score:data.score,diamond:data.diamond,gold:data.gold,name:data.name},function(err, proModel) {
        if (err) return console.error(err);
        console.dir("properties save ne: " + proModel);
    });
}

