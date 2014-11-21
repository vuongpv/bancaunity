"use strict";

var moment = require('moment')
	, fs = require('fs')
	, path = require('path')

/**
 * Count properties of an object
 */
Object.defineProperty(Object.prototype, 'countAttrs', {
	set: function(){},
	get: function(){
		var count = 0;
		for(var prop in this) {
			if(this.hasOwnProperty(prop))
				count = count + 1;
		}
		return count;
	},
	configurable: true
})

// Time helper
module.exports.time = {

	/**
	 * USE FOR TESTING PURPOSE ONLY
	 * Offset for the now function 
	 */
	_nowOffset: moment.duration(0),

	/**
	 * Get current time with global time offset
	 */
	now: function() {
		return moment().add(this._nowOffset)
	}
}

// Response helper
module.exports.response = {
	/**
	 * Send json reponse
	 */
	sendJson: function(res, json) {
		res.writeHead(200, { 'Content-Type': 'application/json' })
		res.write(JSON.stringify(json))
		res.end()
	}
}

function httpPostRequest(contentType, data, host, port, path, cb) {
	var http = require('http');

	//The url we want is `www.nodejitsu.com:1337/`
	var options = {
		host: host,
		path: path,
		//since we are listening on a custom port, we need to specify it by hand
		port: port,
		//This is what changes the request to a POST request
		method: 'POST',
		headers: {
			'Content-Type': contentType,
			'Content-Length': data.length
		}
	};

	var callback = function(response) {
		var str = ''
		response.on('data', function (chunk) {
			str += chunk;
		});

		response.on('end', function (err) {
			if( cb )
				cb(err, str)
		});

		response.on('error', function(err) {
			if( cb )
				cb(err, str)
		});
	}


	var req = http.request(options, callback);
	//This is the data we are posting, it needs to be a string or a buffer
	req.write(data);
	req.end();
}

// Request helper
module.exports.request = {

	// POST FORM
	postRaw: function (data, host, port, path, cb) {
		httpPostRequest('application/x-www-form-urlencoded', data, host, port, path, cb);
	},

	// POST JSON
	postJson: function (data, host, port, path, cb) {
		httpPostRequest('application/json', data.toString(), host, port, path, cb);
	}
}


module.exports.filesys = {
	mkdirParent: function(dirPath, mode, callback) {
		//Call the standard fs.mkdir
		fs.mkdir(dirPath, mode, function(error) {
			//When it fail in this way, do the custom steps
			if (error && error.errno === 34) {
				//Create all the parents recursively
				fs.mkdirParent(path.dirname(dirPath), mode, callback);
				//And then the directory
				fs.mkdirParent(dirPath, mode, callback);
			}
			//Manually run the callback since we used our own callback to do all these
			callback && callback(error);
		});
	}
};

		