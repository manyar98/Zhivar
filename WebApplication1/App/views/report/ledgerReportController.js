define(['application', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper'], function (app) {
    app.register.controller('ledgerReportController', ['$scope','$rootScope','$stateParams', '$window',
        function ($scope,$rootScope,$stateParams, $window) {

	var tree;
	var grid;
	//var params;
	//if ($routeParams.params)
	//	params = parseLedgerReportQueryString($routeParams.params);

	$scope.init = function () {
		$rootScope.pageTitle("دفتر کل، معین و تفصیلی");
		$('#businessNav').show();

		$scope.modalAlert = false;
		$scope.fromDate = $stateParams.fromDate ? $stateParams.fromDate : ""; //$scope.currentFinanYear.DisplayStartDate;
		$scope.toDate = $stateParams.toDate ? $stateParams.toDate : ""; //params ? params.endDate : $scope.currentFinanYear.DisplayEndDate;

        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
			elementId: 'gridContainer',
			layoutKeyName: 'grid-report-ledger',
			sorting: { mode: 'none' },
			filterRow: { visible: false },
			headerFilter: { visible: false },
			height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 50),
			columns: [
				{ caption: 'تاریخ', dataField: 'Date', width: 90, printWidth: 80 },
				{
					caption: 'سند', dataField: 'DocNum', width: 60, printWidth: 70, cellTemplate: function (ce, ci) {
						var txt = Hesabfa.farsiDigit(ci.data.DocNum);
						if (ci.data.DocId > 0)
							ce.html("<a href='javascript:void(0);' onclick='angular.element(this).scope().viewDocumentModal(" + ci.data.DocId + ")' class='text-primary txt-bold'>" + txt + "</a>");
						else
							ce.html(txt);
					}
				},
				{ caption: 'کد حساب', dataField: 'Code', width: 80, printWidth: 80 },
				{
					caption: 'نام حساب', dataField: 'Name', printWidth: 200, cellTemplate: function (ce, ci) {
						var a = Hesabfa.farsiDigit(ci.data.Name);
						var d = Hesabfa.farsiDigit(ci.data.DtName);
						var html = '<span class="text-primary">' + a + '</span>';
						if (d)
							html += '<span class="text-warning">&nbsp;/&nbsp;' + d + '</span>';
						ce.html(html);
					}, calculateCellValue: function (rowData) {
						var a = Hesabfa.farsiDigit(rowData.Name);
						var d = Hesabfa.farsiDigit(rowData.DtName);
						if (d)
							a += " / " + d;
						return a;
					}
				},
				{ caption: 'شــرح', dataField: 'Description', printWidth: 200 },
				{ caption: 'بدهکار', dataField: 'Debit', dataType: "number", format: "#", printFormat: "#", width: 120, printWidth: 100 },
				{ caption: 'بستانکار', dataField: 'Credit', dataType: "number", format: "#", printFormat: "#", width: 120, printWidth: 100 },
				{ caption: 'مانده', dataField: 'Remain', dataType: "number", format: "<b>#</b>", printFormat: "#", width: 120, printWidth: 100 },
				{ caption: 'تشخیص', dataField: 'RemainType', width: 60, printWidth: 65 }
			],
			print: {
				fileName: "Ledger.pdf",
				rowColumnWidth: 40,
				page: {
					landscape: false
				},
				header: {
					title1: $rootScope.currentBusiness.Name,
					title2: function () {
						if ($scope.detailAccount) return "دفتر تفصیلی";
						else if ($scope.account.Level === 2) return "دفتر کل";
						else if ($scope.account.Level === 3) return "دفتر معین";
					},
					left: $scope.currentFinanYear.Name,
					center: function () {
						return $scope.selectedDateRange;
					},
					right: function () {
						return $scope.account.Name + ($scope.detailAccount ? ' / ' + $scope.detailAccount.Name : '');
					}
				}
			}
		});

		$scope.account = null;
		$scope.detailAccount = null;
		$scope.getChartOfAccounts();
		$scope.getDateNow();
	};
	$scope.rangeChanged = function () {
		$scope.getLedger();
	}
	$scope.getChartOfAccounts = function () {
		$scope.loading = true;
         $.ajax({
            type:"POST",
            //data: JSON.stringify({ start: "", end: "" }),
            url:"/app/api/Accounting/GetFinanAccounts",
            contentType: "application/json"
        }).done(function(res){
            var result = res.data;
            	$scope.loading = false;
            	var accounts = result.finanAccounts;
            	$scope.accounts = accounts;
            	$scope.createTree(accounts);

            	if ($stateParams.accountId) {
            	    $scope.account = findById(accounts, $stateParams.accountId);
            		if ($scope.account) {
            		    if ($stateParams.detailAccountId)
            				$scope.getDetailAccounts($scope.account);
            			else
            				$scope.getLedger();
            		}
            	}

            	$scope.$apply();
            }).fail(function (error) {
            	$scope.loading = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
	};
	$scope.getDetailAccounts = function (account) {
	    $scope.modalCalling = true;

	    $.ajax({
	        type:"POST",
	        data: JSON.stringify(account),
	        url:"/app/api/Accounting/GetAccountDetailAccounts",
	        contentType: "application/json"
	    }).done(function(res){
	        var detailAccounts = res.data;
            	$scope.modalCalling = false;
            	$scope.detailAccounts = detailAccounts;

            	if ($stateParams && $stateParams.accountId && $stateParams.detailAccountId) {
            	    $scope.detailAccount = findById(detailAccounts, $stateParams.detailAccountId);
            		if ($scope.detailAccount) {
            			$scope.getLedger();
            		}
            	}


            	$scope.$apply();
            }).fail(function (error) {
            	$scope.modalCalling = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
	};
	$scope.getLedger = function () {
		if (!$scope.account) {
			alertbox({ content: "یک حساب را انتخاب کنید", type: "warning" });
			return;
		}
		$scope.loading = true;
		grid.beginCustomLoading();
		var model = {
		    account: $scope.account,
		    detailAccount: $scope.detailAccount,
		    start: $scope.fromDate,
		    end: $scope.toDate
		};

		$.ajax({
		    type:"POST",
		    data: JSON.stringify(model),
		    url: "/app/api/Transaction/GetLedger",
		    contentType: "application/json"
		}).done(function(res){
		    var data = res.data;
            	$scope.loading = false;
            	grid.endCustomLoading();
            	grid.fill(data.Rows);
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.loading = false;
            	applyScope($scope);
            	grid.endCustomLoading();
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
	$scope.createTree = function (accounts) {
		tree = createTree({
			containerId: 'tree1',
			onClick: function (node) {
				$scope.selectedNode = node;
				$scope.selectedAccount = node.tag;
				$scope.selectedDetailAccount = null;
				$scope.detailAccounts = [];
				if ($scope.selectedAccount.Level === 3)
					$scope.getDetailAccounts($scope.selectedAccount);
				else if ($scope.selectedAccount.Level === 3)
					$scope.selectedAccount = null;
				$scope.$apply();
			},
			onDblClick: function (node) {

			}
		});
		tree.init();

		var treeNodes = [];
		$scope.treeNodes = treeNodes;
		for (j = 0; j < accounts.length; j++) {
			for (var i = 0; i < accounts.length; i++) {
				if ($scope.isInTree(accounts[i].Coding) == null) {
					var node;
					if (accounts[i].Level === 1) { // ریشه : گروه ها که والد ندارند
						var image = "folder";
						node = tree.addNode(null, accounts[i], "(گروه) " + accounts[i].Name, "", image);
						treeNodes.push(node);
					} else {
						var parentNode = $scope.isInTree(accounts[i].ParentCoding);
						if (parentNode != null) {
							var title = "";
							var cssClass = "";
							if (accounts[i].Level === 2)
							{ title = "(کل) " + accounts[i].Name; cssClass = "success" }
							else if (accounts[i].Level === 3)
							{ title = "(معین) " + accounts[i].Name; cssClass = "info" }
							node = tree.addNode(parentNode, accounts[i], title, cssClass, "folder");
							treeNodes.push(node);
						}
					}
				}
			}
		}
	};
	$scope.isInTree = function (parentCoding) {
		var tree = $scope.treeNodes;
		for (var j = 0; j < tree.length; j++) {
			if (tree[j].tag.Coding === parentCoding)
				return tree[j];
		}
		return null;
	};
	$scope.openSelectAccount = function () {
		$scope.modalAlert = false;
		$("#selectAccountModal").modal({ keyboard: false }, "show");
	};
	$scope.selectAccount = function () {
		$scope.modalAlert = false;
		if (!$scope.selectedAccount) {
			$scope.modalAlert = true;
			$scope.modalAlertMessage = "ابتدا یک حساب را انتخاب کنید";
			$scope.modalAlertType = "warning";
			return;
		}
		if ($scope.selectedAccount.Level === 1) {
			$scope.modalAlert = true;
			$scope.modalAlertMessage = "یک حساب کل، معین یا تفصیلی را انتخاب کنید";
			$scope.modalAlertType = "warning";
			return;
		}
		$("#selectAccountModal").modal("hide");
		$scope.account = $scope.selectedAccount;
		$scope.detailAccount = $scope.selectedDetailAccount;
		$scope.getLedger();
	};
	$scope.detailAccountClick = function (detailAccount) {
		$scope.selectedDetailAccount = detailAccount;
		$scope.$apply();
	};
	$scope.getDocument = function (id) {
		if ($scope.calling) return;
		$scope.calling = true;
		callws(DefaultUrl.MainWebService + 'GetDocument', { id: id })
            .success(function (document) {
            	$scope.calling = false;
            	$scope.document = document;
            	// open view document modal
            	$("#viewDocumentModal").modal({ keyboard: false }, "show");
            	$("#viewDocumentModal .modal-dialog").draggable({ handle: ".modal-header" });
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.calling = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ title: "خطا", content: error });
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
	$scope.showInvoice = function (documentId) {
		if ($scope.calling) return;
		$scope.calling = true;
		callws(DefaultUrl.MainWebService + 'GetInvoiceIdByDocumentId', { id: documentId })
            .success(function (invoiceId) {
            	$scope.calling = false;
            	if (invoiceId === 0) return;
            	// go to view invoice
            	$window.open("#viewInvoice/id=" + invoiceId, '_blank');
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.calling = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error });
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};

	$scope.exportLedgerToExcel = function (level) {
		if ($scope.callingImport) return;
		$('#loadingModal').modal('show');
		$scope.callingImport = true;
		grid.beginCustomLoading();
		callws(DefaultUrl.MainWebService + "ExportLedgerToExcel", { level: level })
            .success(function (data) {
            	$scope.callingImport = false;
            	grid.endCustomLoading();
            	$('#loadingModal').modal('hide');
            	$scope.$apply();
            	var contentType = 'application/vnd.ms-excel';
            	var blob = b64toBlob(data, contentType);
            	if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            		window.navigator.msSaveOrOpenBlob(blob, "Hesabfa_Ledger.xlsx");
            	}
            	else {
            		var objectUrl = URL.createObjectURL(blob);
            		window.open(objectUrl);
            	}
            }).fail(function (error) {
            	$scope.callingImport = false;
            	grid.endCustomLoading();
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error });
            	return;
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
	//
	$rootScope.loadCurrentBusinessAndBusinesses($scope.init);
}])
});