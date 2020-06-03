define(['application','maskedinput'], function (app) {
    app.register.directive('datePicker', [function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attr, ngModel) {
                var datepicker;
                element.css('direction', 'ltr');
                var cal = attr.calendarType;
                element[0].dateChanged = function (e) {
                    var val = element[0].value;
                    var display = convertNumbersToAdaptive(val, false);
                    var main = convertNumbersToActual(val, false);
                    if (cal === "1")
                        display = main;
                    element[0].value = display;
                    ngModel.$setViewValue(main);
                };
                element.on("keydown", function (e) {
                    if (e.which === 9 && datepicker && datepicker.visible)
                        datepicker.showHidePicker();
                });
                function init(persian) {
                    if (persian) {
                        element.mask('۱۹۹۹/۹۹/۹۹');
                        if (element[0].id) {
                            datepicker = new AMIB.persianCalendar(element[0].id, {
                                extraInputID: element[0].id,
                                extraInputFormat: 'yyyy/mm/dd'
                            });
                        }
                    }
                    else {
                        element.mask('99/99/9999');
                        if (element[0].id)
                            $(element).datepicker({ autoclose: true });
                    }
                }

                if (cal === '0' || cal === '1')
                    init(cal === '0');
                else {
                    var int = setInterval(function () {
                        if (typeof window.calendarType == "undefined")
                            return;
                        clearInterval(int);
                        init(window.calendarType === 0 || window.calendarType === '0');
                    }, 100);
                }
            }
        };
    }]);
});