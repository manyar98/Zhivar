"use strict";
define(['application', 'dataService', 'messageService', 'kendoUi', 'kendoGridFa', 'cartableVerificationForm'], function (app) {
    app.register.controller('verificationFormController', ['$scope', '$rootScope', '$stateParams', 'dataService', 'messageService', '$state',
        function ($scope, $rootScope, $stateParams, dataService, messageService, $state) {

            $rootScope.applicationModule = "کارتابل اصلی";

            $scope.option = $stateParams.message;
            
        }
    ]);
});