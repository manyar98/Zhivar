define(['angularAMD'], function (app) {
    app.directive('textTime', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            link: function ($scope, $elem) {
                $elem.on('keypress', function (evt) {
                    var charCode = (evt.which) ? evt.which : evt.keyCode;
                    if (!(charCode > 47 && charCode < 59)) {
                        return false;
                    }

                    return true;

                });

            }
        };
    }]);
});