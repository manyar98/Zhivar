define(['application', 'digitGroup', 'number', 'dx', 'roweditor', 'helper'], function (app) {
    app.register.controller('closeFinanYearController', ['$scope', '$rootScope',
        function ($scope, $rootScope) {

    $scope.init = function () {
        $rootScope.pageTitle("بستن دوره مالی");
        $('#businessNav').show();

        //$scope.closingDate = $scope.currentFinanYear.DisplayEndDate;
        if ($scope.currentFinanYear && $scope.currentFinanYear.Closed) {
            $scope.closed = true;
            $scope.alert = true;
            $scope.alertType = "info";
            $scope.alertMessage = "دوره مالی کنونی بسته شده است";
            $scope.$apply();
            $scope.checkPermission();
            return;
        }

        $scope.incomeTaxRate = 0;
        $scope.profitDivisionRate = 100;
        $scope.incomeTaxAmount = 0;
        $scope.profitDivisionAmount = 0;
        $scope.retainedEarningsAmount = 0;
        $scope.getData();
    };
    $scope.getData = function () {
        $scope.loading = true;
        $.ajax({
            type:"POST",
            url: "/app/api/FinanYear/GetRequiredDataToClosingFinanYear",
            contentType: "application/json"
        }).done(function (res) {
            var result = res.data;
                $scope.loading = false;
                $scope.newFinanYear = result.newFinanYear;
                $scope.newFinanYear.Name = "سال مالی منتهی به " + $scope.newFinanYear.DisplayEndDate;
                $scope.netIncome = result.netIncome;
                $scope.netIncomeAfterTax = result.netIncome;
                $scope.shareholders = result.shareholders;
                for (var i = 0; i < $scope.shareholders.length; i++)
                    $scope.shareholders[i].profitAmount = 0;
                if (result.closingDate && result.closingDate !== "")
                    $scope.closingDate = result.closingDate;
                $scope.$apply();
                $scope.profitDivisionChange();
            }).fail(function (error) {
                $scope.loading = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.checkPermission = function () {
        callws(DefaultUrl.MainWebService + 'IsBussinessHavePermissionToCloseFinanYearAgain', {})
            .success(function (havePermission) {
                if (havePermission) $scope.showCloseFinanYearAgainBtn = true;
                $scope.$apply();
            })
            .fail(function (error) {
                alertbox({ content: error });
            })
            .loginFail(function () {
                window.location = DefaultUrl.login;
            });
    };
    $scope.letsCloseFinanYearAgain = function () {
        $scope.showCloseFinanYearAgainBtn = false;
        $scope.alert = false;
        $scope.closed = false;
        $scope.incomeTaxRate = 0;
        $scope.profitDivisionRate = 100;
        $scope.incomeTaxAmount = 0;
        $scope.profitDivisionAmount = 0;
        $scope.retainedEarningsAmount = 0;
        $scope.getData();

        $scope.$apply();
    };
    $scope.incomeTaxChange = function () {
        if ($scope.incomeTaxRate < 0 || $scope.incomeTaxRate > 100) {
            alertbox({ content: "نرخ مالیات بر درآمد باید بین 0 تا 100 باشد" });
            return;
        }
        $scope.incomeTaxAmount = ($scope.netIncome * $scope.incomeTaxRate) / 100;
        $scope.incomeTaxAmount = Math.floor($scope.incomeTaxAmount);
        $scope.netIncomeAfterTax = $scope.netIncome - $scope.incomeTaxAmount;
        $scope.$apply();
        $scope.profitDivisionChange();
    };
    $scope.incomeTaxAmountChange = function () {
        $scope.incomeTaxRate = ($scope.incomeTaxAmount * 100) / $scope.netIncome;
        $scope.incomeTaxRate = Math.round($scope.incomeTaxRate * 100) / 100;
        $scope.netIncomeAfterTax = $scope.netIncome - $scope.incomeTaxAmount;
        $scope.$apply();
        $scope.profitDivisionChange();
    };
    $scope.profitDivisionChange = function () {
        if ($scope.profitDivisionRate < 0 || $scope.profitDivisionRate > 100) {
            alertbox({ content: "نرخ تقسیم سود باید بین 0 تا 100 باشد" });
            return;
        }
        $scope.saveProfitePercent = 100 - $scope.profitDivisionRate;
        $scope.profitDivisionAmount = ($scope.netIncomeAfterTax * $scope.profitDivisionRate) / 100;
        if ($rootScope.isDecimalCurrency)
            $scope.profitDivisionAmount = Math.round($scope.profitDivisionAmount * 100) / 100;
        else
            $scope.profitDivisionAmount = Math.floor($scope.profitDivisionAmount);
        $scope.retainedEarningsAmount = $scope.netIncomeAfterTax - $scope.profitDivisionAmount;
        $scope.calculateShareholdersProfit();
        $scope.$apply();
    };
    $scope.profitDivisionAmountChange = function () {
        $scope.profitDivisionRate = ($scope.profitDivisionAmount * 100) / $scope.netIncomeAfterTax;
        $scope.profitDivisionRate = Math.round($scope.profitDivisionRate * 100) / 100;
        if ($rootScope.isDecimalCurrency)
            $scope.profitDivisionAmount = Math.round($scope.profitDivisionAmount * 100) / 100;
        else
            $scope.profitDivisionAmount = Math.floor($scope.profitDivisionAmount);
        $scope.retainedEarningsAmount = $scope.netIncomeAfterTax - $scope.profitDivisionAmount;
        $scope.calculateShareholdersProfit();
        $scope.$apply();
    };
    $scope.calculateShareholdersProfit = function () {
        var shareholders = $scope.shareholders;
        for (var i = 0; i < shareholders.length; i++) {
            shareholders[i].profitAmount = ($scope.profitDivisionAmount * shareholders[i].SharePercent) / 100;
            if ($rootScope.isDecimalCurrency)
                shareholders[i].profitAmount = Math.round(shareholders[i].profitAmount * 100) / 100;
            else
                shareholders[i].profitAmount = Math.floor(shareholders[i].profitAmount);
        }
    };
    $scope.closeFinanYear = function () {
        questionbox({
            content: "با بستن سال مالی کنونی، دیگر امکان ویرایش اسناد مالی وجود نخواهد داشت.<br/>آیا از بستن سال مالی مطمئن هستید؟",
            onBtn1Click: function () {
                questionbox({
                    content: "این عمل غیرقابل برگشت است. آیا مطمئن هستید؟",
                    onBtn1Click: function () {
                        if ($scope.calling) return;
                        $scope.calling = true;
                        callws(DefaultUrl.MainWebService + 'CloseFinanYear', {
                            closeDate: $scope.closingDate,
                            newFinanYear: $scope.newFinanYear,
                            incomeTax: $scope.incomeTaxAmount,
                            profitDivision: $scope.profitDivisionAmount
                        })
							.success(function (newFinanYearToken) {
							    $scope.calling = false;
							    $scope.alert = true;
							    $scope.alertType = "success";
							    $scope.alertMessage = "دوره مالی جاری با موفقیت بسته و دوره مالی جدید ایجاد شد";
							    $scope.newFinanYear.token = newFinanYearToken;
							    $scope.finanYears.push($scope.newFinanYear);
							    $scope.closed = true;
							    $scope.currentFinanYear.Closed = true;
							    $scope.$apply();
							}).fail(function (error) {
							    $scope.calling = false;
							    applyScope($scope);
							    if ($scope.accessError(error)) return;
							    alertbox({ content: error, type: "error", title: "توجه" });
							    applyScope($scope);
							}).loginFail(function () {
							    window.location = DefaultUrl.login;
							});
                    }
                });
            }
        });
    };
    $scope.cancel = function () {
        window.history.back();
    };
    $scope.goToNewFinanYear = function () {
        if ($scope.newFinanYear) {
            $scope.switchFinanYear($scope.newFinanYear);
            return;
        }
        var finanYears = $scope.finanYears;
        for (var i = 0; i < finanYears.length; i++) {
            var fYear = finanYears[i];
            if (fYear.DisplayStartDate === $scope.currentFinanYear.DisplayStartDate)
                continue;
            if (fYear.DisplayEndDate === $scope.currentFinanYear.DisplayEndDate)
                continue;
            $scope.switchFinanYear(fYear);
            break;
        }
        //		window.location.href = "/dashboard/";
    };
    //
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});