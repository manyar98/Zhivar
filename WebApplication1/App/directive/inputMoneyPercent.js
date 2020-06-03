define(['application'], function (app) {
    app.register.directive('inputMoneyPercent',[ function () {
	return {
		restrict: 'E',
		transclude: true,
		template: function () {
			if (Hesabfa.isDecimalCurrency())
				return '<input display-numbers keyboard-filter="floatpercent" digit-group type="text"/>';
			return '<input display-numbers decimal-point-zero keyboard-filter="integerpercent" digit-group type="text"/>';
		},
		replace: true
	};
}]);
});