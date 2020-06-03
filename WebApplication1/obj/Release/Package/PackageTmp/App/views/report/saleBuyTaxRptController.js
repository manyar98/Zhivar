define(['application', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper'], function (app) {
    app.register.controller('saleBuyTaxRptController', ['$scope','$rootScope',
        function ($scope, $rootScope) {
    var cmbBuyOrSaleItems = ['فروش و برگشت از فروش', 'خرید و برگشت از خرید'];
    var cmbContactTypeItems = ['تمامی اشخاص', 'اشخاص دارای کدملی / شناسه اقتصادی', 'اشخاص بدون کدملی / شناسه اقتصادی'];
    var cmbConditionItems = ['تمامی فاکتورها', 'فقط فاکتورهای دارای مالیات'];
    var grid;
    var allData = [];
    $scope.init = function () {
        $scope.fromDate = "";
        $scope.toDate = "";
        $scope.invoiceType = 0;
        $scope.contactType = 0;
        $scope.allInvoices = true;
        $scope.refreshGrid();

        $rootScope.pageTitle("گزارش خرید، فروش و مالیات فصلی");
        $('#businessNav').show();

        $("#cmbBuyOrSale").dxSelectBox({
            items: cmbBuyOrSaleItems,
            value: cmbBuyOrSaleItems[0],
            rtlEnabled: true,
            onValueChanged: function (e) {
                $scope.invoiceType = cmbBuyOrSaleItems.indexOf(e.value);
                $scope.refreshGrid();
            }
        });
        $("#cmbContactType").dxSelectBox({
                items: cmbContactTypeItems,
                value: cmbContactTypeItems[0],
                rtlEnabled: true,
                onValueChanged: function (e) {
                    $scope.contactType = cmbContactTypeItems.indexOf(e.value);
                    $scope.refreshGrid();
                }
            });
        $("#cmbCondition").dxSelectBox({
                items: cmbConditionItems,
                value: cmbConditionItems[0],
                rtlEnabled: true,
                onValueChanged: function (e) {
                    $scope.allInvoices = cmbConditionItems.indexOf(e.value) == 0;
                    $scope.refreshGrid();
                }
            });

        var contactLookup = { dataSource: [{ key: 0, val: "نامشخص" }, { key: 1, val: "حقیقی" }, { key: 2, val: 'حقوقی' }], valueExpr: 'key', displayExpr: 'val' }
        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
            elementId: 'gridContainer',
            layoutKeyName: 'grid-report-saleBuyTax',
            height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 50),
            columns: [
				{ caption: 'تاریخ', dataField: 'Date', width: 90, printWidth: 2 },
				{ caption: 'نام کالا یا خدمات', dataField: 'ItemName', width: 200, printWidth: 6 },
				{ caption: 'کد کالا', dataField: 'Code', width: 80, printWidth: 2 },
				{ caption: 'برگشتی', dataField: 'Ret', dataType: "boolean", width: 100, printWidth: 1.5 },
				{ caption: 'مبلغ خالص', dataField: 'Sum', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'مالیات', dataField: 'Tax', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'عوارض', dataField: 'Toll', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" },
                {
                    caption: 'نوع شخص', dataField: 'ContactType', lookup: contactLookup, width: 100, printWidth: 1.5,
                    printCalcValue: function (data) {
                        for (var l in contactLookup.dataSource) {
                            if (contactLookup.dataSource[l].key === data.ContactType)
                                return contactLookup.dataSource[l].val;
                        }
                        return "";
                    }
                },
				{ caption: 'کد پستی', dataField: 'PostalCode', width: 100, printVisible: false },
				{ caption: 'تلفن', dataField: 'Phone', width: 100, printVisible: false },
				{ caption: 'آدرس', dataField: 'Address', width: 250, printVisible: false },
				{ caption: 'نام شخص', dataField: 'Name', width: 120, printWidth: 5 },
				{ caption: 'کد اقتصادی', dataField: 'EconomicCode', width: 100, printWidth: 2 },
				{ caption: 'کد/شناسه ملی', dataField: 'NationalCode', width: 100, printWidth: 2 },
				{ caption: 'استان', dataField: 'State', width: 100, printVisible: false },
				{ caption: 'شهر', dataField: 'City', width: 100, printVisible: false },
				{ caption: 'جمع', dataField: 'TotalAmount', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" }
            ],
            print: {
                fileName: "گزارش خرید فروش و مالیات فصلی.pdf",
                rowColumnWidth: 1,
                page: {
                    landscape: true
                },
                header: {
                    title1:  $rootScope.currentBusiness.Name,
                    title2: "گزارش خرید، فروش و مالیات فصلی",
                    left:  $rootScope.currentBusiness.Name,
                    center: function () {
                        return $scope.selectedDateRange;
                    },
                    right: function () {
                        return cmbBuyOrSaleItems[$scope.invoiceType];
                    }
                }
            },
            summary: {
                totalItems: [
					{
					    column: "Sum", summaryType: "sum",
					    customizeText: function (data) {
					        return Hesabfa.money(data.value);
					    }
					},
					{
					    column: "Tax", summaryType: "sum",
					    customizeText: function (data) {
					        return Hesabfa.money(data.value);
					    }
					},
					{
					    column: "Toll", summaryType: "sum",
					    customizeText: function (data) {
					        return Hesabfa.money(data.value);
					    }
					},
					{
					    column: "TotalAmount", summaryType: "sum",
					    customizeText: function (data) {
					        return Hesabfa.money(data.value);
					    }
					}
                ]
            }
        });
    };

    $scope.getReportData = function () {
        if ($scope.loading) return;
        $scope.loading = true;
        grid.beginCustomLoading();
        callws(DefaultUrl.MainWebService + 'GetSaleBuyTaxRpt', { start: $scope.fromDate, end: $scope.toDate })
            .success(function (data) {
                $scope.loading = false;
                grid.endCustomLoading();
                allData = data;
                for (var i = 0; i < data.length; i++) {
                    if (data[i].Type === 2 || data[i].Type === 2) { // SaleReturn || PurchaseReturn
                        data[i].Sum = -data[i].Sum;
                        data[i].Tax = -data[i].Tax;
                        data[i].Toll = -data[i].Toll;
                        data[i].TotalAmount = -data[i].TotalAmount;
                    }
                }
                $scope.refreshGrid();
                $scope.$apply();
            }).fail(function (error) {
                $scope.loading = false;
                grid.endCustomLoading();
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            }).loginFail(function () {
                window.location = DefaultUrl.login;
            });
    };
    $scope.refreshGrid = function () {
        var p = $scope.sums = {
            sumSale: 0,
            sumSaleTax: 0,
            sumSaleToll: 0,
            sumSaleTaxToll: 0,
            sumPurchase: 0,
            sumPurchaseTax: 0,
            sumPurchaseToll: 0,
            sumPurchaseTaxToll: 0,
            tax: 0
        };

        var list = [];
        for (var i = 0; i < allData.length; i++) {
            var d = allData[i];
            var isSale = d.Type === 0 || d.Type === 2;
            var isPurchase = d.Type === 1 || d.Type === 3;
            if ($scope.contactType === 1) {
                if (!d.NationalCode && !d.EconomicCode)
                    continue;
            }
            if ($scope.contactType === 2) {
                if (d.NationalCode || d.EconomicCode)
                    continue;
            }
            if (!$scope.allInvoices && d.Tax === 0)
                continue;

            if (isSale) {
                p.sumSale += d.Sum;
                p.sumSaleTax += d.Tax;
                p.sumSaleToll += d.Toll;
                p.sumSaleTaxToll += d.Tax + d.Toll;
            }
            if (isPurchase) {
                p.sumPurchase += d.Sum;
                p.sumPurchaseTax += d.Tax;
                p.sumPurchaseToll += d.Toll;
                p.sumPurchaseTaxToll += d.Tax + d.Toll;
            }
            if (isSale && $scope.invoiceType !== 0)
                continue;
            if (isPurchase && $scope.invoiceType !== 1)
                continue;

            list.push(d);
        }
        p.tax = p.sumSaleTaxToll - p.sumPurchaseTaxToll;
        if (grid) {
            grid.fill(list);
            grid.doScroll = true;
        }
        applyScope($scope);
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