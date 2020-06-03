define(['application', 'momentJalaali'], function (app) {
    app.register.service('dateService', [function () {
        return {
            toJalaaliDate: function (inputDate, format) {
                var date = moment(inputDate);
                return date.format(format);
            },
            toJalaaliDate: function (inputDate) {
                var date = moment(inputDate);
                return date.format('jYYYY/jMM/jDD');
            },
            toJalaaliDateTime: function (inputDate) {
                var date = moment(inputDate);
                return date.format('jYYYY/jMM/jDD hh:mm');
            },
            toJalaaliDateNow: function (format) {
                var date = moment(new Date());
                return date.format(format);
            },
            toJalaaliDateNow: function () {
                var now = new Date();
                var date = moment(now);
                return date.format("jYYYY/jMM/jDD hh:mm");
            },
        }
    }])
});


