define(['application'], function (app) {
    app.register.directive('inputCode', [function () {
        return {
            restrict: 'E',
            transclude: true,
            template: '<input display-numbers keyboard-filter="code" type="text"/>',
            replace: true
        };
    }]);
});