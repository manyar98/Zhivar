define(['application', 'combo', 'scrollbar', 'helper', 'editCash', 'editBank', 'dx', 'roweditor', 'gridHelper', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker','number'], function (app) {
    app.register.controller('cashesController', ['$scope', '$rootScope','$state',
        function ($scope,$rootScope,$state) {
    $scope.init = function () {
        $rootScope.pageTitle("صندوق و بانک");
        $('#businessNav').show();
        $scope.alert = false;
        $scope.getCashes();
        applyScope($scope);
    };

    $scope.getCashes = function () {
        $scope.alert = false;


        $.ajax({
            type: "POST",
            //data: JSON.stringify(id),
            url: "/app/api/Cashes/GetCashesAndBanks",
            contentType: "application/json"
        }).done(function (res) {
            var data = res.data;
                $scope.alert = false;
                $scope.cashes = data.cashes;
                $scope.banks = data.banks;
                applyScope($scope);
            }).fail(function (error) {
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
                $scope.$apply();
            });
    };
    $scope.addNewCash = function () {
        $scope.cash = null;
        $scope.editCashModal = true;
    };
    $scope.addNewBank = function () {
        $scope.bank = null;
        $scope.editBankModal = true;
    };
    $scope.getEditedCash = function (cash) {
        var finded = findAndReplace($scope.cashes, cash.ID, cash);
        if (!finded) $scope.cashes.push(cash);
        $scope.$apply();
    };
    $scope.getEditedBank = function (bank) {
        var finded = findAndReplace($scope.banks, bank.ID, bank);
        if (!finded) $scope.banks.push(bank);
        $scope.$apply();
    };
    $scope.editCash = function (cash) {
        $scope.cash = cash;
        $scope.editCashModal = true;
    };
    $scope.editBank = function (bank) {
        $scope.bank = bank;
        $scope.editBankModal = true;
    };
    $scope.removeCash = function (cash) {
        questionbox({
            content: "آیا از حذف " + cash.Name + " مطمئن هستید؟",
            onBtn1Click: function () {
                if ($scope.calling) return;
                $scope.calling = true;

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(cash.ID),
                    url: "/app/api/Cash/Delete",
                    contentType: "application/json"
                }).done(function (res) {
                        $scope.calling = false;
                        findAndRemove($scope.cashes, cash);
                        $scope.alert = true;
                        $scope.alertType = "success";
                        $scope.alertMessage = cash.Name + " با موفقیت حذف گردید.";
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        applyScope($scope);
                        if ($scope.accessError(error)) return;
                        $scope.alert = true;
                        $scope.alertType = "danger";
                        $scope.alertMessage = error;
                        applyScope($scope);
                    });
            }
        });
    };
    $scope.removeBank = function (bank) {
        questionbox({
            content: "آیا از حذف " + bank.Name + " مطمئن هستید؟",
            onBtn1Click: function () {
                if ($scope.calling) return;
                $scope.calling = true;

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(bank.ID),
                    url: "/app/api/Bank/Delete",
                    contentType: "application/json"
                }).done(function (res) {
                        $scope.calling = false;
                        findAndRemove($scope.banks, bank);
                        $scope.alert = true;
                        $scope.alertType = "success";
                        $scope.alertMessage = bank.Name + " با موفقیت حذف گردید.";
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        applyScope($scope);
                        if ($scope.accessError(error)) return;
                        $scope.alert = true;
                        $scope.alertType = "danger";
                        $scope.alertMessage = error;
                        applyScope($scope);
                    });
            }
        });
    };
    $scope.redirectMoneyTransfer = function (url, type, origin, destination) {
        switch (type) {
            case 'fromBankToCash':
                $state.go(url, {fromBank:origin,toCash:destination});
                break;
            case 'fromCashToBank':
                $state.go(url, {fromCash:origin,toBank:destination});
                break;
            case 'fromCashToCash':
                $state.go(url, { fromCash: origin, toCash: destination });
                break;
            case 'fromBankToBank':
                $state.go(url, { fromBank: origin, toBank: destination });
                break;
            default:

        }
    };
    $scope.redirect = function(url,type,id)
    {
        switch (type) {
            case 'cash':
                $state.go(url, { cashId: id });
                break;
            case 'bank':
                $state.go(url, { bankId: id });
                break;
          
            default:
        }
    };
    $scope.redirectToTransaction = function(url,id)
    {
        $state.go(url,{id:id});
    }
    //
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});