define(['application'], function (app) {
    app.register.directive('inputMoney', [function () {
        return {
            restrict: 'E',
            transclude: true,
            template: function () {
                if (Hesabfa.isDecimalCurrency())
                    return '<input display-numbers keyboard-filter="float" digit-group type="text"/>';
                return '<input display-numbers decimal-point-zero keyboard-filter="integer" digit-group type="text"/>';
            },
            replace: true
        };
    }]);
});