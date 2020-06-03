define(['application'], function (app) {
    app.register.directive('integerPositive', [function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attr, ngModel) {
                element.on("keydown", function (e) {
                    if (!e.char) {
                        if (e.keyCode === 46 || e.keyCode === 190)
                            e.char = ".";
                        if (e.keyCode === 189 || e.keyCode === 173)
                            e.char = "-";
                        if (e.keyCode === 187 || e.keyCode === 61)
                            e.char = "+";
                    }
                    // Allow: backspace, delete, tab, escape, enter and F5.
                    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 116]) !== -1 ||
                        // Allow: Ctrl+A, C, V, X
                        ((e.keyCode === 65 || e.keyCode === 88 || e.keyCode === 67 || e.keyCode === 86) && e.ctrlKey === true) ||
                        // Allow: home, end, left, right, down, up
                        (e.keyCode >= 35 && e.keyCode <= 40)) {
                        // let it happen, don't do anything
                        return;
                    }
                    if (e.char === ".") {
                        e.preventDefault();
                        return;
                    }

                    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                        e.preventDefault();
                    }

                });
                var beforePasteValue;
                element.on("paste", function () {
                    beforePasteValue = element.val();
                    setTimeout(function () {
                        var val = (element.val() + "").replace(/\,/g, '').replace(/\،/g, '');
                        var b = isNumber(val) && Number(val) >= 0 || isNaN(parseInt(val));
                        if (!b) {
                            element.val(beforePasteValue);
                        }
                    }, 10);
                });
                element.on("focus", function () {
                    element.select();
                });

                function convertToNumber(x) {
                    if (typeof x == "undefined" || x == null)
                        return 0;
                    x = (x + "").replace(/\,/g, '').replace(/\،/g, '');
                    x = Hesabfa.englishDigit(x);
                    x = parseInt(x);
                    if (isNaN(x))
                        return 0;
                    return x;
                }

                function formatToFarsi(x) {
                    if (typeof x == "undefined" || x == null)
                        return "0";
                    x = parseInt(x);
                    var s = x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    s = Hesabfa.farsiDigit(s);
                    return s;
                }


                ngModel.$parsers.push(function (x) {
                    var number = convertToNumber(x);
                    var view = formatToFarsi(number);

                    element.val(view);
                    ngModel.$setViewValue(number + "");

                    return number;
                });
                ngModel.$formatters.push(function (x) {
                    var number = convertToNumber(x);
                    return formatToFarsi(number);
                });
            }
        };
    }]);
});