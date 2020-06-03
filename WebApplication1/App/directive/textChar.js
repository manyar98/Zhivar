define(['angularAMD'], function (app) {
    app.directive('textChar', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function ($scope, $elem, $attrs, ngModel) {
                $elem.on('keypress', function (evt) {
                    if (evt.keyCode && evt.keyCode == 8 && evt.which && evt.which == 8)//Backspace Key
                        return true;
                    if (evt.keyCode && evt.keyCode == 9)//Tab Key
                        return true;
                    else if (evt.keyCode && (evt.keyCode == 39 || evt.keyCode == 37))//Right & Left Arrow Key
                        return true;
                    else if (evt.keyCode && evt.keyCode == 46)//Delete Key
                        return true;

                    else if (evt.keyCode && evt.keyCode == 32)//Delete Key
                        return true;

                    var charCode = (evt.which) ? evt.which : evt.keyCode;
                    if (((charCode >= 0 && charCode < 64) || (charCode > 90 && charCode < 97) || (charCode > 123 && charCode < 127))) {
                        return false;
                    }

                    return true;
                });
            }
        };
    }]);
});