function convertNumbersToAdaptive(x, digitGroup) {
    if (typeof x == "undefined" || x == null)
        x = "";
    x = x + "";
    if (digitGroup)
        x = x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return Hesabfa.farsiDigit(x);
}
function convertNumbersToActual(x, digitGroup) {
    if (typeof x == "undefined" || x == null)
        x = "";
    x = x + "";
    if (digitGroup)
        x = x.replace(/\,/g, '').replace(/\،/g, '');
    return Hesabfa.englishDigit(x);
}
// keyboard numbers to display numbers
define(['application'], function (app) {
    app.register.directive('displayNumbers', [function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attr, ngModel) {
                var isDigitGroup = typeof attr["digitGroup"] != "undefined";
                element.on("input change keyup", function (e) {
                    var val = element[0].value;
                    var beforeVal = val;
                    if (typeof val == "undefined" || val == null)
                        val = "";
                    val = val + "";

                    var main = convertNumbersToActual(val, isDigitGroup);
                    ngModel.$setViewValue(main);

                    var ss = element[0].selectionStart;
                    var se = element[0].selectionEnd;
                    var length = val.length;

                    var display = convertNumbersToAdaptive(main, isDigitGroup);
                    if (display !== beforeVal) {
                        element[0].value = display;
                        element[0].selectionStart = ss + (display.length - length);
                        element[0].selectionEnd = se + (display.length - length);
                    }

                });
                ngModel.$formatters.push(function (x) {
                    if (typeof x == "undefined" || x == null)
                        x = "";
                    x = x + "";
                    return convertNumbersToAdaptive(x, isDigitGroup);
                });
            }
        };
    }]);
});