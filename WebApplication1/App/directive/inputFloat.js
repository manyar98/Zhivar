define(['application'], function (app) {
    app.register.directive('inputFloat', [function () {
        return {
            restrict: 'E',
            transclude: true,
            template: '<input display-numbers keyboard-filter="float" type="text"/>',
            replace: true
        };
    }]);
});