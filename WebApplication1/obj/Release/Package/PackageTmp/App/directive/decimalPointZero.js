// replaces . to 000
define(['application'], function (app) {
    app.register.directive('decimalPointZero', [function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attr, ngModel) {
                element.on("keydown", function (e) {
                    if (e.char === "." || e.keyCode === 110 || e.keyCode === 190) {
                        var ss = element[0].selectionStart;
                        var val = element[0].value;

                        val = val.slice(0, ss) + "000" + val.slice(ss);
                        element.val(val);

                        element[0].selectionStart = ss + 3;
                        element[0].selectionEnd = ss + 3;

                        e.preventDefault();
                        element.trigger('change');
                        return;
                    }
                });
            }
        };
    }]);
});