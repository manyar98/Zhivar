// modals for edit objects
define(['application', 'dataService'], function (app) {
    app.register.directive('finanAccountSelect', [function () {
        return {
            restrict: 'A',
            templateUrl: '/dashboard/widgets/finan-account-select.html?ver=1.2.9.1',
            transclude: true
        };
    }]);
});