

exports.RequestType = {
    Login               : 1,
    CreateUserInfo		: 2,
    GetUserInfo			: 3,

    GetPublicRoomList   : 4,
    GetInfor:11,
    RoomPrices:12,
    JoinWaittingRoom:20,

    Ranking:100,
    Mission:101,

    TestDB              : 1000
};


exports.MessageType = {

    None: 0,
    // function for register socket.io
    Subscribe: 1,        // dang ky mot slot(dung cho socket.io)
    SubscribeResult: 2,  // result subscribe from server socket.io
    UnSubscribe: 3,      // huy dang ky (dung cho socket.io)
    RoomReady: 4,        // Room have already to play, broadcast information room to all room's clients
    SycnReady: 5,        // sync ready (dung cho socket.io)
    ClientLogic:6,      // log event client to broadcast to room
    FinalResult :7,		// sync final result
    BenchMark:8,
    PlayAuto:9,
    ReConnect:10,
    SysPosition:20,//thong bao co su thay doi trong phong
    ResultJoinRoom:21,//Vi tri chon khong thah cong
    UpdateProperties:22,//update properties of account
    MeLeaveRoom:23,
    MeLeaveWaitingRoom:24,
    UserLeaveRoom:25,
    MeJoinRoom_Ok:26,


    // logicGame
    FigureDown: 101,
    FigureUp: 102,
    FigureMove: 103,
    Shot: 104,
    HitFish: 105,
    UpgradeGun: 106,
    ChangeMoney: 107,
    ChangeLevel: 108,
    StopShot: 109,
    RecivePresent: 110
};
