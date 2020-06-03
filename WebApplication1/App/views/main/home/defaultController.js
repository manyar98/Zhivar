define(['application'], function (app) {
    app.register.controller('defaultController', ['$scope', '$state', function ($scope, $state) {

        $scope.test = "test defualt";
    }])
});
