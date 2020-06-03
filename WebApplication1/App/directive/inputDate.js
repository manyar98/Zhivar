
define(['application'], function (app) {
    app.register.directive('inputDate', [function () {
        return {
            restrict: 'E',
            transclude: true,
            template: '<input date-picker display-numbers type="text" />',
            replace: true
        };
    }]);
});