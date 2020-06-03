define(['application'], function (app) {
    app.register.controller('financialReportController', ['$scope', '$state', function ($scope, $state) {
       
        $scope.redirect = function (url,id)
        {
            $state.go(url, { id: id });
        
        }
    }])
}); 
