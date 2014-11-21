/**
 * Created by kakapro on 10/24/13.
 */

//define cac kieu du lieu, enum ...
"use strict";

exports.DEFAULT_STAMINA = 20;
exports.MAX_HISTORY_COUNT = 100;
exports.LIMIT_USER_LEVEL=300;
exports.LIMIT_USER_FISH_LEVEL=300;

exports.MINT_BONUS_ENTER_INVITE=300;

exports.LIMIT_MINIMUM_FISH_PARTY_PERSON=2;
exports.LIMIT_MAXIMUM_FISH_PARTY_PERSON=15;

exports.PARTY_FISH_ROOM_NAME="FISH_PARTY_";

exports.FISHPARTY_ROOMTYPE={
    AUHORITY:1,
    SECRET  :2,
    PUBLIC  :3
};
exports.PRICE_TYPE=
{
    MINT:0,
    DOLE:1
};

exports.INVITE_CODE_STATUS=
{
    OPEN:0,
    EXPIRED:1
};

exports.PriceTypeString=
{
    Mint:"Mint",
    Dole:"Dole"
};

exports.EVENT_USER_MESSAGE =// custom event send auto to user
{
    NONE: 0,
    USER_HAVE_NICED: 1,// a person nice you
    USER_HAVE_BELLED: 2,// a person bell you
    USER_HAVE_FRIEND_REQUEST: 3,// a person make friend with you
    USER_HAVE_FRIEND_CONFIRMED: 4,// a person confirmed friend with yo
    USER_HAVE_FEVERED:5// a person help you to fish
};
exports.FRIEND_TYPE =
{
    NONE: 0,
    QUEST: 1,
    FRIEND_WAITING: 2,
    FRIEND_NEED_CONFIRM: 3,
    FRIEND: 4,
    FRIEND_SOCIAL: 5
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
    OtherInteractWithObject  : 9,

    HasError            : 10,
    UpdateTaskList		: 11,

    SyncPlayerData      : 12,
    SyncFishState		: 13,

    EventUserMessage    : 14,// message anounce for user know who nice/like, add friend...

    EventFishParty_APersonJoin : 15,
    EventFishParty_APersonLeave : 16,
    EventFishParty_AcceptJoin : 17,
    EventFishParty_OwnerClose : 18,
    EventFishParty_ChangeOwner : 19,
    EventFishParty_ChangeComment:20,
    EventFishParty_ARequireJoinRoom:21,
    EventFishParty_GetOutUser:22,

    EventUserGetFishReward:23,

    OtherChangeClothes:24,//thong bao co thang thay doi quan ao
    OtherPlayAnimation:25,


    //Chat message
    Chat                : 1000,
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

exports.TypeLike = {
    NICE: 1,
    BELL: 2
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
    DATABASE_ERROR: 7,
    WRONG_PASSWORD: 8,
    ALREADY_LIKE: 9,
    NOT_ENOUGH_STAMINA: 10,
    ROOM_FULL: 11,
    ALREADY_FRIEND: 12,
    ALREADY_REQUEST: 13,
    ALREADY_NICE: 14,
    ALREADY_BELL: 15,
    EMPTY_HISTORY: 16,
    VERSION_NOT_MATCH: 17,
    ALREADY_LOGIN: 18,
    ALREADY_FEVER:19,
    NOT_HAVE_ROD:20,
    NOT_HAVE_BAIT:21,
    ROOM_IS_LOCKED:22,
    ROOM_NOT_FOR_FISH:23,
    USER_HAVE_LOGOUT:24,
    WAIT_OWNER_CONFIRM:25,
    FISH_PARTY_ROOM_FULL:26,
    USER_JOIN_ANOTHER_PARTY_ROOM:27,
    ALREADY_JOIN_IN_A_ROOM:28,
    NOT_ENOUGH_CASH:29,
    NOT_ENOUGH_GOLD:30,
    ALREADY_ENTER_INVITE_CODE:31,
    WRONG_INVITE_CODE:32
};

exports.TestFuncType =
{
    GetUID: 1,
    GetAllRoomUIDRoom: 2,
    GetUserDB: 3,
    GetCurrentCloth: 4,
    GetFriendDB: 5,
    GetClientID: 6,
    GetFriendByUID: 7
};

exports.TaskStatus =
{
    NotAvailable: 0,
    Current : 1,
    Completed : 2
};

