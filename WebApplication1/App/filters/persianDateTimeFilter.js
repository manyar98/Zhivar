define(['angularAMD', 'momentJalaali'], function (app) {
    app.filter('persianDate', function () {
        return function (date, format) {
            date = date || new Date();
            format = format || 'jYYYY/jMM/jDD';
            return moment(date).format(format);
        };
    }).filter('persianDateTime', function () {
        return function (date, format) {
            date = date || new Date();
            format = format || 'jYYYY/jMM/jDD - HH:mm';
            return moment(date).format(format);
        };
    })
});