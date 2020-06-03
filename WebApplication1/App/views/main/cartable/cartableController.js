"use strict";
define(['application', 'dataService', 'messageService', 'kendoUi', 'kendoGridFa', 'cartable'], function (app) {
    app.register.controller('cartableController', ['$scope', '$rootScope', '$stateParams', 'dataService', 'messageService', '$state',
        function ($scope, $rootScope, $stateParams, dataService, messageService, $state) {

            $rootScope.applicationModule = "کارتابل اصلی";

            $scope.option = {
                Controller: "dr",
                State: "test",
                Visible:false
            };
            
            $scope.selectedRow = function (e) {
                $state.go("verificationForm", { message: e });
            }
        }
    ]);
});