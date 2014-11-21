

exports.RequestType = {
    Login               : 1,
    CreateUserInfo		: 2,
    GetUserInfo			: 3,

    GetPublicRoomList   : 4
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
    // logicGame
    FigureDown: 101,
    FigureUp: 102,
    FigureMove: 103,
    Shot: 104,
    HitFish: 105,
    UpgradeGun: 106,
    ChangeMoney: 107,
    ChangeLevel: 108,
    StopShot: 109
};
