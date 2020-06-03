define(['application', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper'], function (app) {
    app.register.controller('chartOfAccountsController', ['$scope','$rootScope', '$location', '$stateParams', '$window','$state',
        function ($scope, $rootScope, $location, $stateParams, $window,$state) {

	$scope.init = function () {
		$rootScope.pageTitle("جدول حساب ها");
		$scope.accCodeHelp = true;
		if ($stateParams.id && $stateParams.id === "trialBalance") {
			$scope.param = false;
			$scope.showTrialBalance = true;     // نمایش تراز آزمایشی
		}
		$('#businessNav').show();
		$scope.grid = new gridHelper({ scope: $scope, rpp: 100, showContextMenu: false });
		if ($scope.showTrialBalance)
			$scope.getTrialBalance();
		else
			$scope.getFinanAccounts();

		$('#tabs a').click(function (e) {
			e.preventDefault();
			$(this).tab('show');
			$('#addAccountBtn').focus();
		});
	};
	$scope.getFinanAccounts = function (resort) {
		if ($scope.loading) return;
		$scope.loading = true;
		$scope.calling = true;
           $.ajax({
            type:"POST",
            //data: JSON.stringify({ start: "", end: "" }),
            url:"/app/api/Accounting/GetFinanAccounts",
            contentType: "application/json"
        }).done(function(res){
                var result = res.data;
            	$scope.loading = false;
            	$scope.calling = false;
            	var data = result.finanAccounts;
            	$scope.systemAccountTypes = result.systemAccountTypes;
            	//				alert("[$scope.newAccount = data.pop();] removed.");
            	//$scope.newAccount = data.pop();
            	$scope.grid.data = data;
            	$scope.grid.init();
            	if (!resort) $scope.grid.sort('Coding');
            	$scope.$apply();

            	$scope.sumDebitBalance = 0;
            	$scope.sumCreditBalance = 0;

            	if ($scope.showTrialBalance) {
            		$rootScope.pageTitle("تراز حساب ها");
            		var l = data.length;
            		for (var i = 0; i < l; i++) {
            			var acc = data[i];
            			if (acc.Level !== 3) continue;
            			if (acc.GroupCode === 9) continue;
            			if (acc.BalanceType === 0) $scope.sumDebitBalance += acc.Balance;
            			else $scope.sumCreditBalance += acc.Balance;
            		}
            	}
            }).fail(function (error) {
            	$scope.calling = false;
            	$scope.loading = false;
            	$scope.$apply();
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
	};
	$scope.getTrialBalance = function () {
		if ($scope.loading) return;
		$scope.loading = true;
		$scope.calling = true;
		if (!$scope.fromDate) $scope.fromDate = "";
		if (!$scope.toDate) $scope.toDate = "";
		$.ajax({
		    type:"POST",
		    //data: JSON.stringify({ start: "", end: "" }),
		    url:"/app/api/Accounting/GetFinanAccountsBalance",
		    contentType: "application/json"
		}).done(function (res) {

            var result = res.data;
            	$scope.loading = false;
            	$scope.calling = false;
            	var data = result.finanAccounts;
            	$scope.systemAccountTypes = result.systemAccountTypes;
            	$scope.grid.data = data;
            	$scope.grid.init();
            	//            	if (!resort) $scope.grid.sort('Coding');
            	$scope.$apply();

            	$scope.sumDebitBalance = 0;
            	$scope.sumCreditBalance = 0;

            	if ($scope.showTrialBalance) {
            		$rootScope.pageTitle("تراز حساب ها");
            		var l = data.length;
            		for (var i = 0; i < l; i++) {
            			var acc = data[i];
            			if (acc.Level !== 3) continue;
            			if (acc.GroupCode === 9) continue;
            			if (acc.BalanceType === 0) $scope.sumDebitBalance += acc.Balance;
            			else $scope.sumCreditBalance += acc.Balance;
            		}
            	}
            }).fail(function (error) {
            	$scope.calling = false;
            	$scope.loading = false;
            	$scope.$apply();
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
	};
	$scope.classByLevel = function (level) {
		if (level === 1)
			return 'account-level1';
		else if (level === 2)
			return 'account-level2';
		else if (level === 3)
			return 'account-level3';
		return null;
	};
	$scope.search = function () {
		var key = $scope.searchKey;
		if (!key || key === ' ') {
			var activeTab = $("ul#tabs li.active").index() - 1;
			if (activeTab === -1) {
				$scope.grid.removeFilter();
				$scope.$apply();
			}
			else $scope.grid.applyFilter('AccountType.Category', activeTab);
			return;
		}
		if (!isNaN(parseInt(key, 10)))
			$scope.grid.applyFilter('Code', key, true, $scope, true);
		else
			$scope.grid.applyFilter('Name', key, true, $scope, true);
	};
	$scope.addAccount = function () {
		$scope.alert = false;
		$scope.account = null;
		$scope.editAccountModal = true;
		applyScope($scope);
	};
	$scope.closeEditAccount = function () {
		$('#panelEditAccount').slideUp();
		$('#inputAccCode').popover('hide');
	};
	$scope.editAccount = function (account) {
		if ($scope.showTrialBalance) {
		    if (account.Level === 1) return;
		    $state.go('ledgerReport', { accountId:account.Id})
			//$window.open("#ledgerReport/" + account.Id, '_blank');
			//            $location.path('/ledgerReport/' + account.Id);
			return;
		}
		if (account.Level === 1) return;
		//        if (account.SystemAccount > 1) {
		//            alertbox({ content: 'حساب های سیستمی قابل حذف یا ویرایش نمی باشند.' });
		//            return;
		//        }
		$scope.alert = false;
		$scope.account = account;
		$scope.editAccountModal = true;
		applyScope($scope);
	};
	$scope.submitAccount = function () {
		if ($scope.calling) return;
		var acc = $scope.acc;
		acc.AccountType = findById($scope.accTypes, $scope.accountTypeId);
		$scope.calling = true;
		callws(DefaultUrl.MainWebService + 'SaveAccount', { account: acc, selectedSysAcc: acc.SystemAccountName })
            .success(function (account) {
            	$scope.calling = false;
            	$scope.alertBoxVisible = false;
            	var finded = findAndReplace($scope.grid.data, account.Id, account);
            	if (!finded) $scope.grid.addItem(account);
            	$scope.grid.sort('Coding', false);
            	applyScope($scope);
            	$('#panelEditAccount').slideUp();
            }).fail(function (error) {
            	$scope.calling = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	$scope.alertBoxVisible = true;
            	$scope.alertMessage = error;
            	applyScope($scope);
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
	$scope.deleteAccountAlert = function () {
		$scope.deleteAlertQuestion = true;
	};
	$scope.cancelDelete = function () {
		$scope.deleteAlertQuestion = false;
	};
	$scope.deleteAccount = function () {
		if ($scope.calling) return;
		var selectedItems = $scope.grid.getSelectedItems();
		if (!selectedItems || selectedItems.length === 0)
			return;
		$scope.calling = true;
		callws(DefaultUrl.MainWebService + 'DeleteAccounts', { accounts: selectedItems })
    .success(function () {
    	$scope.calling = false;
    	$scope.deleteAlertQuestion = false;
    	$scope.grid.removeItems(selectedItems);
    	applyScope($scope);
    }).fail(function (error) {
    	$scope.calling = false;
    	applyScope($scope);
    	if ($scope.accessError(error)) return;
    	$scope.alertBoxVisible = true;
    	$scope.alertMessage = error;
    	applyScope($scope);
    }).loginFail(function () {
    	window.location = DefaultUrl.login;
    });
	};
	$scope.updateAccountsBalance = function () {
		if ($scope.updatingBalances) return;
		$scope.updatingBalances = true;
		callws(DefaultUrl.MainWebService + 'UpdateAccountsBalance', {})
            .success(function () {
            	$scope.updatingBalances = false;
            	$scope.getFinanAccounts(true);
            	alertbox({ content: "تراز حساب ها بروزرسانی شد" });
            }).fail(function (error) {
            	$scope.updatingBalances = false;
            	$scope.$apply();
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error });
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
	$scope.getEditedAccount = function (account) {
		if (!account) return;
		var finded = findAndReplace($scope.grid.data, account.Id, account);
		if (finded) {
			findAndReplace($scope.grid.gridData, account.Id, account);
			findAndReplace($scope.grid.pageData, account.Id, account);
		}
		if (!finded) $scope.grid.addItem(account);
		$scope.editAccountModal = false;
		$scope.$apply();
	};
	$scope.exportTrialBalanceToExcel = function () {
		if ($scope.callingImport) return;
		$scope.callingImport = true;
		$('#loadingModal').modal('show');
		callws(DefaultUrl.MainWebService + 'ExportTrialBalanceToExcel', {})
            .success(function (data) {
            	$scope.callingImport = false;
            	$('#loadingModal').modal('hide');
            	$scope.$apply();
            	var fileName = "TrialBalance";

            	var contentType = 'application/vnd.ms-excel';
            	var blob = b64toBlob(data, contentType);
            	if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            		window.navigator.msSaveOrOpenBlob(blob, fileName + ".xlsx");
            	}
            	else {
            		var objectUrl = URL.createObjectURL(blob);
            		//                    window.open(objectUrl);
            		//                    var encodedUri = "data:application/vnd.ms-excel," + encodeURI(data);
            		var link = document.createElement("a");
            		link.href = objectUrl;
            		link.download = fileName + ".xlsx";
            		link.target = '_blank';
            		document.body.appendChild(link);
            		link.click();
            	}

            }).fail(function (error) {
            	$scope.callingImport = false;
                $scope.$apply();

                if ($scope.accessError(error)) return;

                alertbox({content: error});

            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
    //
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});