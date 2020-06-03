// keyboard filter for numbers, including + - . 0-9
define(['application'], function (app) {
    app.register.directive('keyboardFilter', [function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attr, ngModel) {
                var type = attr["keyboardFilter"];
                var isPercent = type.indexOf("percent") >= 0;
                type = type.replace("percent", "");
                if ($.inArray(type, ['code', 'integer', 'signed-integer', 'signed-float', 'float']) === -1)
                    alert("invalid keyboard-filter type: [" + type + "]");
                var isCode = type === "code";
                var isFloat = type === "float" || type === "signed-float";
                var isInteger = type === "integer" || type === "signed-integer";
                var isSigned = type === "signed-integer" || type === "signed-float";


                function getTypedValue() {
                    return convertNumbersToActual(element.val(), true);
                }
                function getValue() {
                    var val = getTypedValue();
                    var hasPercent = isPercent && val.indexOf("%") >= 0;

                    var ret = "";
                    if (isCode) {
                        for (var i = 0; i < val.length; i++) {
                            if (val[i] >= '0' && val[i] <= '9')
                                ret += val[i];
                        }
                        return ret;
                    }
                    if (isFloat) {
                        ret = parseFloat(val);
                        if (!isNumber(ret))
                            return "0";
                        if (!isSigned && ret < 0)
                            val *= -1;
                        if (hasPercent)
                            return val + "%";
                        return val + "";
                    }
                    if (isInteger) {
                        ret = parseInt(val);
                        if (!isNumber(ret))
                            return "0";
                        if (!isSigned && ret < 0)
                            ret *= -1;
                        if (hasPercent)
                            return ret + "%";
                        return ret + "";
                    }
                    return "";
                }

                //element.css('direction', 'ltr');
                element.on("keydown", function (e) {
                    // Allow: backspace, delete, tab, escape, enter and F5.
                    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 116]) !== -1 ||
                        // Allow: Ctrl+A, C, V, X
                        ((e.keyCode === 65 || e.keyCode === 88 || e.keyCode === 67 || e.keyCode === 86) && e.ctrlKey === true) ||
                        // Allow: home, end, left, right, down, up
                        (e.keyCode >= 35 && e.keyCode <= 40)) {
                        // let it happen, don't do anything
                        return;
                    }
                    if (e.shiftKey && e.keyCode >= 35 && e.keyCode <= 40)
                        return;
                    var val;

                    if (isFloat) {
                        if (e.char === "." || e.keyCode === 110 || e.keyCode === 190) {
                            val = element.val() + "";
                            if (val.indexOf('.') !== -1)
                                e.preventDefault();
                            return;
                        }
                    }
                    if (isPercent) {
                        if (e.char === '%') {
                            val = element.val() + "";
                            if (val.indexOf('%') !== -1)
                                e.preventDefault();
                            return;
                        }
                    }
                    if (isSigned) {
                        if (e.char === '-' || e.keyCode === 109 || e.keyCode === 189) {
                            val = element.val() + "";
                            if (val.indexOf('-') !== -1) {
                                val = val.replace(/-/ig, '');
                                element.val(val);
                                ngModel.$setViewValue(getValue());
                                e.preventDefault();
                            } else {
                                val = "-" + val;
                                element.val(val);
                                ngModel.$setViewValue(getValue());
                                e.preventDefault();
                            }
                            return;
                        }
                        if (e.char === '+' || e.keyCode === 107 || (e.shiftKey && e.keyCode === 187)) {
                            val = element.val() + "";
                            if (val.indexOf('-') !== -1) {
                                val = val.replace(/-/ig, '');
                                element.val(val);
                                ngModel.$setViewValue(getValue());
                            }
                            e.preventDefault();
                            return;
                        }
                    }
                    if ((e.keyCode < 48 || e.keyCode > 57) && (e.keyCode < 96 || e.keyCode > 105)) {
                        e.preventDefault();
                    }

                });

                var beforePasteValue;
                element.on("paste", function () {
                    beforePasteValue = element.val();
                    setTimeout(function () {
                        var val = getValue();
                        var b = isNumber(val);
                        if (!b) {
                            element.val(beforePasteValue);
                        }
                    }, 0);
                });
                element.on("blur", function () {
                });
                element.on("focus", function () {
                    element.select();
                });
                ngModel.$parsers.push(function (x) {
                    var typed = getTypedValue();
                    var val = getValue();
                    if (val !== typed && (!isFloat || val + '.' !== typed)) {
                        element.val(val);
                    }
                    ngModel.$setViewValue(val);
                    return val;
                });
            }
        };
    }]);
});