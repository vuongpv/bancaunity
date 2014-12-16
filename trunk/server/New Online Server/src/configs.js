/**
 * Created by kakapro on 11/26/13.
 */

//Chua cac constant dung cho server

"use strict";

exports.CLIENT_VERSION = "0.1000";
exports.DEBUG_MODE = false;
exports.TIME_AUTO_SAVE = 60 * 10;//10 phut save 1 lan

//Moi truong chay game: can thay doi khi deploy len cac server tes va release
exports.RUN_ENV = 0;//0: DEV, 1:REAL: