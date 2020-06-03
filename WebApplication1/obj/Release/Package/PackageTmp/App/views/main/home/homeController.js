define(['application'], function (app) {
    app.register.controller('homeController', ['$scope', '$rootScope',
        function ($scope, $rootScope) {

            $rootScope.applicationModule = "خانه";
            $scope.applicationDescription = "";

            $scope.initialize = function () {

            }

        }
    ]);
});


