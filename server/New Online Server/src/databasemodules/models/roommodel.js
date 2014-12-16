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

module.exports = Room;

function Room() {
    this.id="";
    this.name = "defaultRoom";
    this.price=100;
    this.skintype="0";//1 auto - 0 player
    this.roomType=0;//0 - gold, 1 - diamond
    this.maxUser=3;
}

Room.createDefault = function (id) {
    var r = new Room();
    r.id=id;
    r.name="Room "+id;
    return r;
};

var RoomSchema = new Schema({

    id: { type: String, default: '' },
    name:{type:String,default:''},
    price:{type:Number,default:100},
    skintype:{type:Number,default:0},//1 auto - 0 player
    roomType:{type:Number,default:0},//0 - gold, 1 - diamond
    maxUser:{type:Number,default:3}

});

Room.mongoModel = mongoose.model('room', RoomSchema);

Room.save = function (data, callback) {
    if (data == null) {
        logger.error("data nullllllll");
        callback("Params null");
        return;
    }

    var roomModel = new Room.mongoModel();
    utils.copyObjectToModel(data, roomModel);

    roomModel.save(function(err, roomModel) {
        if (err) return console.error(err);
        console.dir("Room save ne: " + roomModel);
        callback();
    });
}
