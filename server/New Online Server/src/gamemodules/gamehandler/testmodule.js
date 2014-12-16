/**
 * Created by hiepdhd on 11/21/14.
 */

var defines = require('../../defines');
var resultCode = defines.ResultCode;
var User = require("./../user");
var account = require('../../databasemodules/models/accountmodel');

exports.Login = function(client, data, resp) {
    var res = {
    };
    res.retCode = resultCode.Ok;

    var username = data.username;
    //check username existed
    if (username == null || username.length < 1) {
        console.log("Name = null");
        resp({retCode:resultCode.Ok});
        return;
    }


    username = username.replace(/-/g,'');//loai bo ky tu "-"
    username = username.toLowerCase();

    account.mongoModel.findOne({username: username}, function(err, res){
        if (err) return console.error(err);

        if(!res){
            console.log("chua co account nay -> tao moi");

            var newAcc = account.createDefault(username);

            account.save(newAcc.uid, newAcc, function(err) {
                if (err) return console.error(err);
                console.log("tao moi account thanh cong!");
                console.dir(newAcc);

                client.uid = newAcc.uid;
                client.userName = newAcc.username;
                client.userData = new User(client);
                client.userData.saveDataToDB(function(err){
                    if (err != resultCode.OK) {
                        console.log("saveDataToDB for " + newAcc.uid + " with error " + err);
                        resp({retCode:resultCode.DATABASE_ERROR});
                        return;
                    }
                })
            });
        }
        else{
            console.log("da co account nay roi ne");
            console.dir(res);

            client.uid = res.uid;
            client.userName = res.username;
            client.userData = new User(client);

            client.userData.loadDataFromDB(function (err) {
                if (err != resultCode.OK) {
                    console.log("loadDataFromDB for " + res.uid + " with error " + err);
                    resp({retCode:resultCode.DATABASE_ERROR});
                    return;
                }

//                console.log("=========================");
//                console.dir(client.userData.properties);
//                console.log("=========================");
            });
        }

    });



    resp(res);
}