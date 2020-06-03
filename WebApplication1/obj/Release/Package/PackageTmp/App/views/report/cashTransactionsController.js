define(['application','combo', 'scrollbar', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper'], function (app) {
    app.register.controller('cashTransactionsController', ['$scope','$rootScope','$stateParams', '$location',
        function ($scope,$rootScope,$stateParams, $location) {

    var grid;
    var selectedCashName = "";
    $scope.init = function () {
        $rootScope.pageTitle("تراکنش های صندوق");
        $('#businessNav').show();
        $scope.fromDate = $scope.currentFinanYear.DisplayStartDate;
        $scope.toDate = $scope.currentFinanYear.DisplayEndDate;
        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
            elementId: 'gridContainer',
            layoutKeyName: 'grid-report-cashTransactions',
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
				{ caption: 'تشخیص', dataField: 'RemainType', width: 60, printWidth: 2 }
            ],
            print: {
                fileName: "تراکنش های صندوق.pdf",
                rowColumnWidth: 1,
                page: {
                    landscape: false
                },
                header: {
                    title1: $rootScope.currentBusiness.Name,
                    title2: "گزارش تراکنش های صندوق",
                    left: $scope.currentFinanYear.Name,
                    center: function () {
                        return selectedCashName;
                    },
                    right: function () {
                        return $scope.selectedDateRange;
                    }
                }
            }
        });

        $scope.comboCash = new HesabfaCombobox({
            items: [],
            containerEle: document.getElementById("comboCash"),
            toggleBtn: true,
            itemClass: "hesabfa-combobox-item",
            activeItemClass: "hesabfa-combobox-activeitem",
            itemTemplate: Hesabfa.comboCashTemplate,
            matchBy: "item.DetailAccount.Id",
            displayProperty: "{Name}",
            searchBy: ["Name", "DetailAccount.Code"],
            onSelect: $scope.cashSelect,
            divider: true
        });
        $scope.cashId = $stateParams.id;
        if ($scope.cashId)
            $scope.getLedger();

        $scope.loadCashes();

        applyScope($scope);
    };
    $scope.cashSelect = function (item) {
        if (item) {
            $scope.cashId = item.ID;
            selectedCashName = item.Name;
            $scope.getLedger();
        }
    };
    $scope.loadCashes = function () {
        $.ajax({
            type:"POST",
            //data: JSON.stringify({ start: "", end: "" }),
            url:"/app/api/Cash/GetAllByOrganId",
            contentType: "application/json"
        }).done(function (res) {
            var items = res.data;
                $scope.comboCash.items = items;
                $scope.$apply();
            }).fail(function (error) {
                $scope.loading = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };

    $scope.getLedger = function () {
        if (!$scope.cashId) {
            alertbox({ content: "صندوق را انتخاب کنید", type: "error" });
            return;
        }
        $scope.loading = true;
        grid.beginCustomLoading();

        var model = {
            Id: $scope.cashId, 
            start: $scope.fromDate, 
            end: $scope.toDate
        };

        $.ajax({
            type:"POST",
            data: JSON.stringify(model),
            url: "/app/api/Transaction/GetCashTransactions",
            contentType: "application/json"
        }).done(function (res) {
            var result = res.data;
                $scope.loading = false;
                grid.endCustomLoading();
                $scope.comboCash.setSelected(result.cash);
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
    //
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);

}])
});