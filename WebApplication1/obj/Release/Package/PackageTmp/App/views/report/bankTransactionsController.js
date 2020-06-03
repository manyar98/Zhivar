define(['application','combo', 'scrollbar', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper'], function (app) {
    app.register.controller('bankTransactionsController', ['$scope','$rootScope','$stateParams', '$location',
        function ($scope,$rootScope,$stateParams, $location) {

	var grid;
	var selectedBankName = "";
	$scope.init = function () {
		$rootScope.pageTitle("تراکنش های بانک");
		$('#businessNav').show();
		$scope.fromDate = "1397/01/01",//$scope.currentFinanYear.DisplayStartDate;
		$scope.toDate = "1397/12/29",//$scope.currentFinanYear.DisplayEndDate;
            grid = dxGrid({
            columnFixing: {
                enabled: true
            },
			elementId: 'gridContainer',
			layoutKeyName: 'grid-report-bankTransactions',
			sorting: { mode: 'none' },
			filterRow: { visible: false },
			headerFilter: { visible: false },
			height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 70),
			columns: [
				{ caption: 'تاریخ', dataField: 'Date', width: 90, printWidth: 2 },
				{
					caption: 'سند', dataField: 'DocNum', width: 60, printWidth: 1.5, cellTemplate: function (ce, ci) {
						var txt = Hesabfa.farsiDigit(ci.data.DocNum);
						if (ci.data.DocId > 0)
							ce.html("<a href='javascript:void(0);' onclick='angular.element(this).scope().viewDocumentModal(" + ci.data.DocId + ")' class='text-primary txt-bold'>" + txt + "</a>");
						else
							ce.html(txt);
					}
				},
				{ caption: 'شــرح', dataField: 'Description', printWidth: 8 },
				{ caption: 'بدهکار', dataField: 'Debit', dataType: "number", format: "#", width: 120, printWidth: 2, printFormat: "#" },
				{ caption: 'بستانکار', dataField: 'Credit', dataType: "number", format: "#", width: 120, printWidth: 2, printFormat: "#" },
				{ caption: 'مانده', dataField: 'Remain', dataType: "number", format: "<b>#</b>", width: 120, printWidth: 2, printFormat: "#" },
				{ caption: 'تشخیص', dataField: 'RemainType', width: 60, printWidth: 1.5 }
			],
			print: {
				fileName: "تراکنش های بانک.pdf",
				rowColumnWidth: 1,
				page: {
					landscape: false
				},
				header: {
					title1: $rootScope.currentBusiness.Name,
					title2: "گزارش تراکنش های بانک",
					left: $scope.currentFinanYear.Name,
					center: function () {
						return $scope.selectedDateRange;
					},
					right: function () {
						return selectedBankName;
					}
				}
			}
		});

		$scope.comboBank = new HesabfaCombobox({
			items: [],
			containerEle: document.getElementById("comboBank"),
			toggleBtn: true,
			itemClass: "hesabfa-combobox-item",
			activeItemClass: "hesabfa-combobox-activeitem",
			itemTemplate: Hesabfa.comboBankTemplate,
			matchBy: "item.DetailAccount.Id",
			displayProperty: "{Name}",
			searchBy: ["Name", "DetailAccount.Code"],
			onSelect: $scope.bankSelect,
			divider: true
		});
		$scope.bankId = $stateParams.id;
		if ($scope.bankId)
			$scope.getLedger();

		$scope.loadBanks();

		applyScope($scope);
	};
	$scope.bankSelect = function (item) {
		if (item) {
			$scope.bankId = item.ID;
			selectedBankName = item.Name;
			$scope.getLedger();
		}
	};
	$scope.loadBanks = function () {
        $.ajax({
            type:"POST",
            //data: JSON.stringify({ start: "", end: "" }),
            url:"/app/api/Bank/GetAllByOrganId",
            contentType: "application/json"
           }).done(function (res) {
           var items = res.data;
		//callws(DefaultUrl.MainWebService + 'GetBanks', {})
        //    .success(function (items) {
            	$scope.comboBank.items = items;
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.loading = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
	};

	$scope.getLedger = function () {
		if (!$scope.bankId) {
			alertbox({ content: "بانک را انتخاب کنید", type: "error" });
			return;
		}
		$scope.loading = true;
		grid.beginCustomLoading();

		var model = {
		    Id: $scope.bankId, 
		    start: $scope.fromDate, 
		    end: $scope.toDate
		};

		$.ajax({
		    type:"POST",
		    data: JSON.stringify(model),
		    url: "/app/api/Transaction/GetBankTransactions",
		    contentType: "application/json"
		}).done(function (res) {
		    var result = res.data;
            	$scope.loading = false;
            	grid.endCustomLoading();
            	$scope.comboBank.setSelected(result.bank);
            	grid.fill(result.ledger.Rows);
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
    
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);

}])
});