define(['angularAMD'], function (app) {
    app.directive('textNumericEslash', ['$parse', function ($parse) {
        return {
            restrict: 'A',
          
            link: function ($scope, $elem, $attrs) {
                $elem.on('keypress', function (evt) {
                    var charCode = (evt.which) ? evt.which : evt.keyCode;
                    if (!((charCode > 46 && charCode < 58) || (charCode == 157 || charCode == 8))) {
                        return false;
                    }

                    return true;

                });

            }
        };
    }]);
});