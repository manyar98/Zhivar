define(['application'], function (app) {
    app.register.directive('empty', [function ($compile) {
        return {
            restrict: 'E',
            template: ''
        };
    }]);
});