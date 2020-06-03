define(['angularAMD', 'messageService'], function (app) {
    app.directive('textEmail', ['$parse', 'messageService', function ($parse, messageService) {
        var REGEXP = /^[_a-z0-9A-Z]+(\.[_a-z0-9A-Z]+)*@[a-z0-9-A-Z]+(\.[a-z0-9-A-Z]+)*(\.[a-zA-Z]{2,3})$/;
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function ($scope, $elem, $attrs, ngModel) {
                $elem.on("keyup", function () {
                    var isMatchRegex = REGEXP.test($elem.val());
                    if (isMatchRegex && $elem.hasClass('warning') || $elem.val() == '') {
                        $elem.removeClass('warning');
                    } else if (isMatchRegex == false && !$elem.hasClass('warning')) {
                        $elem.addClass('warning');
                    }
                });
                $elem.on("blur", function (e) {
                    if ($elem.hasClass('warning')) {
                        e.stopPropagation();
                        e.preventDefault();
                       // messageService.info("ایمیل را با فرمت  درست وارد کنید","");
                    }
                });
            }
        };
    }]);
});