define(['application'], function (app) {
    app.register.controller('financialListController', ['$scope', '$state', function ($scope, $state) {

        $scope.redirect = function (url, id) {
            $state.go(url, { show: id });

        }

    

        $scope.redirectToCheques = function (id) {
            $state.go('cheques', { show: id });

        }
        
    }])
});
