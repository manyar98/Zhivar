define(['application', 'digitGroup', 'number', 'dx', 'roweditor', 'helper', 'dateRange', 'gridHelper'], function (app) {
    app.register.controller('inventoryRptController', ['$scope','$rootScope',
        function ($scope, $rootScope) {

	var grid;
	$scope.init = function () {
		$rootScope.pageTitle("بارگیری گزارش...");
		$('#businessNav').show();
        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
			elementId: 'gridContainer',
			layoutKeyName: 'grid-report-inventory',
			height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 50),
			columns: [
                { caption: 'کـد کالا', dataField: 'Code', width: 80, printWidth: 2 },
                { caption: 'گروه', dataField: 'NodeName', printWidth: 8, hidingPriority: 0 },
                {
                	caption: 'نام کالا', dataField: 'Name', printWidth: 8, cellTemplate: function (cellElement, cellInfo) {
                		cellElement.html("<a class='txt-bold' href='#itemCard/" + cellInfo.data.Id + "', target='_blank'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</a>");
                	}
                },
                { caption: 'قیمت خرید', dataField: 'BuyPrice', dataType: "number", format: "#", width: 120, printWidth: 2, printFormat: "#", hidingPriority: 2 },
                { caption: 'قیمت فروش', dataField: 'SellPrice', dataType: "number", format: "#", width: 120, printWidth: 2, printFormat: "#", hidingPriority: 3 },
                { caption: 'موجودی (تعداد)', dataField: 'Stock', dataType: "number", width: 100, printWidth: 2, format: "<b>#</b>" },
                { caption: 'میانگین قیمت', dataField: 'Wap', dataType: "number", format: "#", width: 120, printWidth: 2, printFormat: "#", hidingPriority: 1 },
                {
                	caption: 'موجودی', dataField: 'MoneyStock', dataType: "number", format: "<b>#</b>", width: 120, printWidth: 4, printFormat: "#", hidingPriority: 4,
                	printCalcValue: function (data) {
                		return Math.round(data.MoneyStock);
                	}
                }
			],
			print: {
				fileName: "inventory.pdf",
				rowColumnWidth: 1.5,
				page: {
					landscape: false
				},
				header: {
					title1: $rootScope.currentBusiness.Name,
					title2: function () { return "گزارش موجودی کالا تا تاریخ " + $scope.untilDate },
					center: "",
					//left: $scope.currentFinanYear.Name, right: function () { return cmb.option("value"); }
                    left: "ژیور", right: function () { return cmb.option("value"); }
				}
			},
			summary: {
				totalItems: [
                    {
                    	column: "Stock",
                    	summaryType: "sum",
                    	customizeText: function (data) {
                    		return Hesabfa.farsiDigit(data.value);
                    	}
                    },
                    {
                    	column: "MoneyStock", summaryType: "sum",
                    	customizeText: function (data) {
                    		return Hesabfa.money(data.value);
                    	}
                    }
				]
			}
		});
		$scope.untilDate = "1397/01/01",//$scope.currentFinanYear.DisplayEndDate;
		$scope.grid = new gridHelper({ scope: $scope });
		$scope.onlyHaveStock = false;
		$scope.getInventoryItems();
		$scope.getDateNow();
		$scope.startNodeSelect = true;
		applyScope($scope);
	};

	var cmb = $("#selectStatus").dxSelectBox({
		dataSource: ['نمایش همه', 'نمایش کالاهای با موجودی صفر', 'نمایش کالاهای با موجودی منفی', 'نمایش کالاهای با موجودی مثبت'],
		value: 'نمایش همه',
		rtlEnabled: true,
		onValueChanged: function (data) {
			if (data.value === "نمایش همه")
				grid.clearFilter();
			if (data.value === "نمایش کالاهای با موجودی صفر")
				grid.filter(["Stock", "=", 0]);
			if (data.value === "نمایش کالاهای با موجودی منفی")
				grid.filter(["Stock", "<", 0]);
			if (data.value === "نمایش کالاهای با موجودی مثبت")
				grid.filter(["Stock", ">", 0]);
		}
	}).dxSelectBox("instance");
	$scope.getInventoryItems = function () {
		$scope.loading = true;
		grid.beginCustomLoading();
		$.ajax({
		    type:"POST",
		    data: JSON.stringify("" ),
		    url:"/app/api/Item/GetInventoryItems",
		    contentType: "application/json"
		}).done(function(res){
		    var data = res.data;
            	$scope.loading = false;
            	grid.fill(data);
            	grid.endCustomLoading();
            	$rootScope.pageTitle("گزارش موجودی کالا");
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.loading = false;
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
    
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});