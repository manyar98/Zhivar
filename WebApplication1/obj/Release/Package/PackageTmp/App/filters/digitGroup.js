define(['angularAMD'], function (app) {
    app.filter('digitGroup', function () {
        return function (value) {
            return Hesabfa.money(value);
        };
    })
});

