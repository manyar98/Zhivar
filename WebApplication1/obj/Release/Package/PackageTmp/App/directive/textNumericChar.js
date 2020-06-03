define(['angularAMD'], function (app) {
    app.directive('textNumericChar', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function ($scope, $elem, $attrs, ngModel) {
                $elem.on('keypress', function (evt) {
                    var hybridKey = evt.shiftKey || evt.ctrlKey || evt.altKey;
                    var someSymbol = [126, 33, 64, 35, 36, 37, 94, 38, 42, 40, 41, 63, 95]; //~!@#$%^&*()?_

                    var isSymbol = $.grep(someSymbol, function (item) { return item == evt.keyCode || item == evt.which }).length > 0;

                    if(evt.shiftKey && isSymbol)
                        return true;
                    else if (!hybridKey && evt.keyCode && evt.keyCode == 8 && evt.which && evt.which == 8)//Backspace Key
                        return true;
                    else if (evt.keyCode && evt.keyCode == 9)//Tab Key, Shift + Tab Keies
                        return true;
                    else if (!hybridKey && evt.keyCode && (evt.keyCode == 39 || evt.keyCode == 37))//Right & Left Arrow Key
                        return true;
                    else if (!hybridKey && evt.keyCode && evt.keyCode == 46)//Delete Key
                        return true;

                    var charCode = (evt.which) ? evt.which : evt.keyCode;

                    if (!((charCode > 47 && charCode < 58) || (charCode > 64 && charCode < 91) || (charCode > 96 && charCode < 123))) {
                        return false;
                    }

                    return true;
                });
            }
        };
    }]);
});