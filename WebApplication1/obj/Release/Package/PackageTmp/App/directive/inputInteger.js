define(['application'], function (app) {
    app.register.directive('inputInteger', [function () {
        return {
            restrict: 'E',
            transclude: true,
            template: '<input display-numbers keyboard-filter="integer" type="text"/>',
            replace: true
        };
    }]);
});