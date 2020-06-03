define(['application', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper'], function (app) {
    app.register.controller('finanSettingsController', ['$scope','$rootScope','$stateParams', '$location',
        function ($scope,$rootScope,$stateParams, $location) {
    $scope.addClass = function (step) {
        return step === 2 ? 'btn btn-success btn-sm' : 'btn btn-default btn-smoke btn-sm';
    };

    	var startDateShamsiObj = new AMIB.persianCalendar('startDateShamsi'); 
        	var endDateShamsiObj = new AMIB.persianCalendar('endDateShamsi'); 

    $scope.init = function () {
        $rootScope.pageTitle("تنظیمات مالی");
        $('#businessNav').hide();
        $scope.previousBtnShow = true;
        $scope.saveAndQuitBtnShow = true;
        $scope.nextBtnShow = true;
        $scope.setupMode = $stateParams.setupMode ? true : false;
        if (!$scope.setupMode) $('#businessNav').show();
        $scope.getFirstFinanYear();
        $scope.alertBoxVisible = false;
        $scope.alertMessage = "";
        $scope._business = {
            currency: $rootScope.currentBusiness.Currency,
            calendarType: $rootScope.currentBusiness.CalendarType,
            economicCode: $rootScope.currentBusiness.EconomicCode,
            taxRate: $rootScope.currentBusiness.TaxRate
        }
        if ($scope._business.calendarType === -1)
            $scope._business.calendarType = 0;

        if ($scope._business.calendarType === 0) {
            $("#startDateGregorian").hide();
            $("#endDateGregorian").hide();
        } else {
            $("#startDateShamsi").hide();
            $("#endDateShamsi").hide();
        }

        if ($scope._business && $scope._finanYear) {
            if ($scope._business.calendarType === 0) {
                $scope.startDateShamsi = $scope._finanYear.startDate;
                $scope.endDateShamsi = $scope._finanYear.endDate;
            } else {
                $scope.startDateGregorian = $scope._finanYear.startDate;
                $scope.endDateGregorian = $scope._finanYear.endDate;
            }
        }
        applyScope($scope);
    };
    $scope.convertFinanYearDateToShamsi = function () {
        if (!$scope._finanYear) return;
        if ($scope._finanYear.startDate && $scope._finanYear.startDate !== "") {
            $scope._finanYear.startDate = $scope.ToShamsi($scope.startDateGregorian);
            $scope.startDateShamsi = $scope._finanYear.startDate;
        }
        if ($scope._finanYear.endDate && $scope._finanYear.endDate !== "") {
            $scope._finanYear.endDate = $scope.ToShamsi($scope.endDateGregorian);
            $scope.endDateShamsi = $scope._finanYear.endDate;
        }
    };
    $scope.convertFinanYearDateToMiladi = function () {
        if (!$scope._finanYear) return;
        if ($scope._finanYear.startDate && $scope._finanYear.startDate !== "") {
            $scope._finanYear.startDate = $scope.ToGregorian($scope.startDateShamsi);
            $scope.startDateGregorian = $scope._finanYear.startDate;
        }
        if ($scope._finanYear.endDate && $scope._finanYear.endDate !== "") {
            $scope._finanYear.endDate = $scope.ToGregorian($scope.endDateShamsi);
            $scope.endDateGregorian = $scope._finanYear.endDate;
        }
    };
    $scope.endDateLeave = function () {
        $scope.asignDatesToFinanyear();
        if (!$scope._finanYear.name)
            $scope._finanYear.name = 'سال مالی منتهی به ' + $scope._finanYear.endDate;
    };
    $scope.asignDatesToFinanyear = function () {
        if ($scope._business && $scope._finanYear) {
            if ($scope._business.calendarType === 0) {
                $scope._finanYear.startDate = $scope.startDateShamsi;
                $scope._finanYear.endDate = $scope.endDateShamsi;
            } else {
                $scope._finanYear.startDate = $scope.startDateGregorian;
                $scope._finanYear.endDate = $scope.endDateGregorian;
            }
        }
    };
    $scope.nextSetupStep = function () {
        if ($scope.calling) return;
        $rootScope.currentBusiness.SetupStep++;
        $scope.saveFinanSettings('/users/1');
    };
    $scope.previousSetupStep = function () {
        if ($scope.calling) return;
        $rootScope.currentBusiness.SetupStep--;
        $scope.saveFinanSettings('/orgSettings/1');
    };
    $scope.saveAndQuitSetup = function () {
        if ($scope.calling) return;
        $scope.saveFinanSettings('/dashboard');
    };
    $scope.saveFinanSettings = function (nextPath, method) {
        if ($scope.calling) return;
        $scope.alert = false;
        $scope.calling = true;
        $scope.previousBtnShow = false;
        $scope.saveAndQuitBtnShow = false;
        $scope.nextBtnShow = false;
        $rootScope.currentBusiness.Currency = $scope._business.currency;
        $rootScope.currentBusiness.CalendarType = $scope._business.calendarType;
        $rootScope.currentBusiness.EconomicCode = $scope._business.economicCode + "";
        $rootScope.currentBusiness.TaxRate = $scope._business.taxRate || 0;
        $scope.asignDatesToFinanyear();
        if ($scope._business.calendarType === 0) {
            $scope.currentFinanYear.DisplayStartDate = $scope._finanYear.startDate;
            $scope.currentFinanYear.DisplayEndDate = $scope._finanYear.endDate;
        } else {
            $scope.currentFinanYear.DisplayStartDate = $('#startDateGregorian').val();
            $scope.currentFinanYear.DisplayEndDate = $('#endDateGregorian').val();
        }
        $scope.currentFinanYear.Name = $scope._finanYear.name;

        var model ={
            finanYear: $scope.currentFinanYear, 
            calendar: $scope._business.calendarType 

        };
        
        $.ajax({
            type: "POST",
            data: JSON.stringify(model),
            url: "/app/api/FinanYear/SaveFinanYear",
            contentType: "application/json"
              }).done(function (res) {
                 var finanYear = res.data;
                  $.ajax({
                    type: "POST",
                    data: JSON.stringify($rootScope.currentBusiness),
                    url: "/app/api/Business/SaveBusiness",
                    contentType: "application/json"
                  }).done(function (res) {
                      var business = res.data;
                      if (method) method();
                        $scope.calling = false;
                        $scope.previousBtnShow = true;
                        $scope.saveAndQuitBtnShow = true;
                        $scope.nextBtnShow = true;
                        $scope.$parent.currentBusiness = business;
                        $scope.$parent.currentFinanYear = finanYear;
                        findAndReplace($scope.finanYears, finanYear.Id, finanYear);
                        var businessInList = findBusinessById($scope.businesses, business.Id);
                        $scope.$parent.currentBusiness.token = businessInList.token;
                        findBusinessAndReplace($scope.businesses, $scope.$parent.currentBusiness);
                        if (nextPath && nextPath === "save") {
                            $scope.calling = false;
                            alertbox({
                                content: "تغییرات با موفقیت ذخیره شد. \n" +
				                    "جهت اعمال تغییرات صفحه مجدداً بارگزاری می گردد",
                                onBtn1Click:
				                    function () {
				                        location.reload();
				                    }
                            });
                            return;
                        }
                        applyScope($scope);
                        if (nextPath && nextPath !== "/dashboard")
                            window.location = $scope.getTokenQuerystring() + "#" + nextPath;
                        else if (nextPath)
                            $location.path(nextPath);
                    }).fail(function (error) {
                        $scope.calling = false;
                        applyScope($scope);
                        if ($scope.accessError(error)) return;
                        if ($scope.setupMode) $rootScope.currentBusiness.SetupStep = 2;
                        $scope.previousBtnShow = true;
                        $scope.saveAndQuitBtnShow = true;
                        $scope.nextBtnShow = true;
                        $scope.alertType = "danger";
                        $scope.alert = true;
                        $scope.alertMessage = error;
                        applyScope($scope);
                    });
            }).fail(function (error) {
                if ($scope.accessError(error)) return;
                if ($scope.setupMode)
                    $scope.$parent.currentBusiness.SetupStep = 2;
                $scope.calling = false;
                $scope.previousBtnShow = true;
                $scope.saveAndQuitBtnShow = true;
                $scope.nextBtnShow = true;
                $scope.alertType = "danger";
                $scope.alert = true;
                $scope.alertMessage = error;
                $scope.$apply();
            });
    };
    $scope.calendarSelect = function (index) {
        if ($scope._business.calendarType === index)
            return;
        $scope._business.calendarType = index;
        if (index === 0) {
            $scope.convertFinanYearDateToShamsi();
            $("#startDateShamsi").show();
            $("#endDateShamsi").show();
            $("#startDateShamsi").val($scope._finanYear.startDate);
            $("#endDateShamsi").val($scope._finanYear.endDate);
            $("#startDateGregorian").hide();
            $("#endDateGregorian").hide();
        } else {
            $scope.convertFinanYearDateToMiladi();
            $("#startDateGregorian").show();
            $("#endDateGregorian").show();
            $("#startDateGregorian").val($scope._finanYear.startDate);
            $("#endDateGregorian").val($scope._finanYear.endDate);
            $("#startDateShamsi").hide();
            $("#endDateShamsi").hide();
        }
        applyScope($scope);
    }

    $scope.getFirstFinanYear = function () {
        if ($scope.currentFinanYear) {
            $scope._finanYear = {
                startDate: $scope.$parent.currentFinanYear.DisplayStartDate,
                endDate: $scope.$parent.currentFinanYear.DisplayEndDate,
                name: $scope.$parent.currentFinanYear.Name
            }
            $scope.endDateLeave();
            return;
        }
        $scope.calling = true;
             $.ajax({
            type:"POST",
            //data: JSON.stringify({ start: "", end: "" }),
            url:"/app/api/FinanYear/GetFirstFinanYear",
            contentType: "application/json"
        }).done(function (res) {
            var finanYear = res.data;
        $scope.calling = false;
        $scope.$parent.currentFinanYear = finanYear;
        $scope._finanYear = {
            startDate: $scope.$parent.currentFinanYear.DisplayStartDate,
            endDate: $scope.$parent.currentFinanYear.DisplayEndDate,
            name: $scope.$parent.currentFinanYear.Name
        }
        $scope.endDateLeave();
        $scope.$apply();
    }).fail(function (error) {
        $scope.calling = false;
        $scope.$apply();
        if ($scope.accessError(error)) return;
        alertbox({ content: error, type: "error" });
    });
    };
	//
	$rootScope.loadCurrentBusinessAndBusinesses($scope.init);
}])
});