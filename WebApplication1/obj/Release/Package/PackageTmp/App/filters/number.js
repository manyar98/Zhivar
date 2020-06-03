define(['angularAMD'], function (app) {
    app.filter('number', function () {
        return function (value) {
            		return Hesabfa.farsiDigit(value);
        };
    })
});

