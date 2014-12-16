/**
 * Created by kakapro on 10/24/13.
 */

//define cac kieu du lieu, enum ...
"use strict";

exports.FRIEND_TYPE =
{
    NONE: 0,
    FRIEND_WAITING: 1,
    FRIEND_NEED_CONFIRM: 2,
    FRIEND: 3,
    FRIEND_SOCIAL: 4
};


exports.MessageToClient =
{
    //game message
    Connected           : 1,
    SystemLog           : 2,
    JoinedRoom          : 3,
    JoinedHome  		: 4,
    NewPlayer           : 5,
    OtherMoveTo              : 6,
    LeftRoom            : 7,
    OtherLeftRoom      : 8,

    //Chat message
    ChatPublic          : 1000,
    PrivateChat         : 1001,


    //Test message
    TestFunc            : 10001,
    TestPacket            : 10002
};

exports.ChatChannel = {

    Sys: 0,
    All: 1,
    Room: 2,
    Party: 3
};

exports.ResultCode =
{
    OK: 0,
    FAILED: 1,
    NOT_FOUND: 2,
    PENDING: 3,
    CLOSED: 4,
    TIME_OUT: 5,
    EXPIRED: 6,
    DATABASE_ERROR: 7
};
