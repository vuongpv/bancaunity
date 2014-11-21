"use strict";

Number.prototype.toBase = function (base) {
    var symbols = 
    "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".split("");
    var decimal = this;
    var conversion = "";

    if (base > symbols.length || base <= 1) {
        return false;
    }

    while (decimal >= 1) {
        conversion = symbols[(decimal - (base * Math.floor(decimal / base)))] + 
                     conversion;
        decimal = Math.floor(decimal / base);
    }

    return (base < 11) ? parseInt(conversion) : conversion;
}
