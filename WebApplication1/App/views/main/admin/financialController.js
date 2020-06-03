define(['application'], function (app) {
    app.register.controller('financialController', ['$scope', '$state', function ($scope, $state) {
       
        $scope.redirectInvoices = function (url,id)
        {
            $state.go(url, { show: id });
        
        }
        $scope.redirectInvoice = function (url, id) {
            $state.go(url, { id: id });

        }
        $scope.redirectCost = function (url, id) {
            $state.go(url, { id: id });

        }
        $scope.redirectToReceiveAndPay = function (id) {
            $state.go('receiveAndPay', { isReceive: id });

        }
    }])
}); 
