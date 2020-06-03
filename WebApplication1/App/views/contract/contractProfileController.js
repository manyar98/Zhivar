define(['application', 'dataService', 'digitGroup', 'number', 'dx', 'roweditor', 'helper', 'gridHelper', 'messageService'], function (app) {
    app.register.controller('contractProfileController', ['$scope', 'dataService', '$location', '$stateParams', '$compile', '$state', '$rootScope', 'messageService',
        function ($scope, dataService, $location, $stateParams, $compile, $state, $rootScope, messageService) {


            $rootScope.applicationModule = "نمایش قرارداد";
            $scope.applicationDescription = "در این صفحه امکان نمایش قرارداد وجود دارد.";


            $scope.init = function () {
                $('#businessNav').show();
                $("#progressBarDiv").hide();

                $scope.getInformation();
            };

            $scope.getInformation = function () {
                if ($scope.pageLoading) return;
                $scope.pageLoading = true;

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(30),
                    url: "/app/api/Contract/GetInfoMediaByID",
                    contentType: "application/json"
                }).done(function (res) {

                    var result = res.data;
                    $scope.pageLoading = false;

                    $scope.contractInfo = result;

                    $scope.Week1 = result.Week1;
                    $scope.Week2 = result.Week2;
                    $scope.Week3 = result.Week3;
                    $scope.Week4 = result.Week4;
                    $scope.Week5 = result.Week5;

                    $('#fileShow').prop("src", $scope.contractInfo.Image);
                 
                }).fail(function (error) {
                    $scope.pageLoading = false;
                 
            
                    if ($scope.accessError(error)) return;
                    alertbox({ content: error, type: "error" });
                });
            };

            $scope.redirectToEditInvoice = function (id) {
                $state.go("newContract", { id: id });

            };

            $scope.previousMonth = function () {

                if ($scope.pageLoading) return;
                $scope.pageLoading = true;

                var month = 0;
                var year = 0;
                if ($scope.contractInfo.NumberOfMonth > 1)
                {
                    month = $scope.contractInfo.NumberOfMonth - 1;
                    year = $scope.contractInfo.NumberOfYear;
                }
             
                else {
                    month = 12;
                    year = $scope.contractInfo.NumberOfYear - 1;
                }

                var model = {
                    month: month,
                    year: year
                };

          
                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Contract/GetCalender",
                    contentType: "application/json"
                }).done(function (res) {

                    var result = res.data;
                    $scope.pageLoading = false;

                    $scope.contractInfo = result;

                    $scope.Week1 = result.Week1;
                    $scope.Week2 = result.Week2;
                    $scope.Week3 = result.Week3;
                    $scope.Week4 = result.Week4;
                    $scope.Week5 = result.Week5;

                }).fail(function (error) {
                    $scope.pageLoading = false;


                    if ($scope.accessError(error)) return;
                    alertbox({ content: error, type: "error" });
                });
            };

            $scope.nextMonth = function () {

                if ($scope.pageLoading) return;
                $scope.pageLoading = true;

                var month = 0;
                var year = 0;
                if ($scope.contractInfo.NumberOfMonth < 12)
                {
                    month = $scope.contractInfo.NumberOfMonth + 1;
                    year = $scope.contractInfo.NumberOfYear;
                }
               
                else
                {
                    month = 1;
                    year = $scope.contractInfo.NumberOfYear + 1;
                }

                var model = {
                    month: month,
                    year: year
                };
                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Contract/GetCalender",
                    contentType: "application/json"
                }).done(function (res) {

                    var result = res.data;
                    $scope.pageLoading = false;

                    $scope.contractInfo = result;

                    $scope.Week1 = result.Week1;
                    $scope.Week2 = result.Week2;
                    $scope.Week3 = result.Week3;
                    $scope.Week4 = result.Week4;
                    $scope.Week5 = result.Week5;

                }).fail(function (error) {
                    $scope.pageLoading = false;


                    if ($scope.accessError(error)) return;
                    alertbox({ content: error, type: "error" });
                });
            };
            
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);



        }]);
});