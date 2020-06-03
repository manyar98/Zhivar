define(['application', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper'], function (app) {
    app.register.controller('exploreAccountsController', ['$scope','$rootScope',
        function ($scope, $rootScope) {

	var grid;
	$scope.init = function () {
		$('#businessNav').show();
		$scope.fromDate = $scope.currentFinanYear.DisplayStartDate;
		$scope.toDate = $scope.currentFinanYear.DisplayEndDate;
        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
			elementId: 'gridContainer',
			layoutKeyName: 'grid-report-exploreAccounts',
			height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 100),
			columns: [
				{ caption: 'کد حساب', dataField: 'Coding', width: 100, printWidth: 2 },
				{ caption: 'نام حساب', dataField: 'Name', printWidth: 5 },
				{ caption: 'گردش بدهکار', dataField: 'SumDebit', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'گردش بستانکار', dataField: 'SumCredit', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'مانده بدهکار', dataField: 'Debit', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'مانده بستانکار', dataField: 'Credit', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" },
				{
					caption: '#', width: 100, printVisible: false, cellTemplate: function (ce, ci) {
						var o;
						if ($scope.level === 0) {
							o = createLedgerReportQueryString(ci.data.Id, 0, $scope.fromDate, $scope.toDate);
							ce.html("<a href='#/ledgerReport/" + o + "' target='_blank'>گردش حساب</a>");
						}
						if ($scope.level === 1) {
							o = createLedgerReportQueryString(ci.data.Id, 0, $scope.fromDate, $scope.toDate);
							ce.html("<a href='#/ledgerReport/" + o + "' target='_blank'>گردش حساب</a>");
						}
						if ($scope.level === 2) {
							o = createLedgerReportQueryString($scope.account.Id, ci.data.Id, $scope.fromDate, $scope.toDate);
							ce.html("<a href='#/ledgerReport/" + o + "' target='_blank'>گردش حساب</a>");
						}
					}
				}
			]
            , print: {
            	fileName: "exploreAccounts.pdf",
            	rowColumnWidth: 1.5,
            	page: {
            		landscape: false
            	},
            	header: {
            		title1: $rootScope.currentBusiness.Name,
            		title2: "مرور حساب ها",
            		left: $scope.currentFinanYear.Name,
            		center: function () {
            			return $scope.selectedDateRange;
            		},
            		right: function () {
            			var str = $scope.level > 0 ? $scope.totalAccount.Name : "حساب های کل";
            			str += $scope.level === 2 ? "/" + $scope.account.Name : "";
            			return str;
            		}
            	}
            }
		});
		$scope.totalAccount = null;
		$scope.account = null;
		$scope.level = 0;

		grid.on("cellClick", function (e) {
			if (e.column.caption === '#' || !e.data)
				return;
			if ($scope.level >= 2)
				return;
			if ($scope.level === 0)
				$scope.totalAccount = e.data;
			if ($scope.level === 1)
				$scope.account = e.data;
			$scope.level++;
			$scope.getAccountsToExplore();
			$scope.$apply();
		});

		$scope.getAccountsToExplore();
		applyScope($scope);
	};
	$scope.requestLevel = function (l) {
		if (l === $scope.level)
			return;
		if (l === 0 || (l === 1 && $scope.totalAccount) || (l === 2 && $scope.totalAccount && $scope.account)) {
			$scope.level = l;
			$scope.getAccountsToExplore();
		}
	}
	$scope.getAccountsToExplore = function () {

	    $rootScope.pageTitle("بارگیری گزارش...");
	    //var data = [{}];
	    //grid.fill(data);
		//grid.beginCustomLoading();
		$scope.loading = true;
		var account = null;
		if ($scope.level === 1)
			account = $scope.totalAccount;
		if ($scope.level === 2)
			account = $scope.account;
            
        var model ={
            account: account, 
            start: $scope.fromDate, 
            end: $scope.toDate
            };

		$.ajax({
		    type: "POST",
		    data: JSON.stringify(model),
		    url:"/app/api/Accounting/GetAccountsToExplore",
		    contentType: "application/json"
		}).done(function(res)
        {
        var data = res.data;
           	$scope.loading = false;
            	grid.fill(data);
            	grid.endCustomLoading();
            	$rootScope.pageTitle("گزارش مرور حساب ها");
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.loading = false;
            	$rootScope.pageTitle("گزارش مرور حساب ها");
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
	};
	$scope.print = function () {
		grid.print();
	};
	$scope.pdf = function () {
		grid.pdf();
	};
	$scope.excel = function () {
		grid.exportToExcel(false);
	};
	function createLedgerReportQueryString(accountId, detailAccountId, startDate, endDate) {
	    var o = { accountId: accountId || 0, detailAccountId: detailAccountId || 0, startDate: startDate || '', endDate: endDate || '' };
	    return btoa(JSON.stringify(o));
	}
    
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);

}])
});