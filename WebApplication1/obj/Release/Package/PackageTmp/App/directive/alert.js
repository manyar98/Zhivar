define(['application'], function (app) {

    app.register.directive('alert',[ function () {
    return {
        restrict: 'E',
        templateUrl: '/App/template/alert.html',
        scope: {
            message: '=',
            messages: '=?',
            alertType: '='
        }
    
	};
}]);
});