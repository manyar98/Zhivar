define(['application'], function (app) {
    app.register.directive('inputText', [function () {
        return {
            restrict: 'E',
            transclude: true,
            template: '<input display-numbers type="text">',
            replace: true
        };
    }]);
});