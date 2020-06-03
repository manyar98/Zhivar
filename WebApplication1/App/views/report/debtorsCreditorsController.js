define(['application', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper'], function (app) {
    app.register.controller('debtorsCreditorsController', ['$scope','$rootScope','$stateParams', '$location',
        function ($scope,$rootScope,$stateParams, $location) {
    var grid, cmb;
    var statuses = ['نمایش همه افراد', 'نمایش بدهکاران', 'نمایش بستانکاران', 'نمایش افراد با مانده صفر'];
    function applyFilter() {
        var val = cmb.option("value");
        if (val === statuses[0])
            grid.clearFilter();
        if (val === statuses[1])
            grid.filter(["Balance", ">", 0]);
        if (val === statuses[2])
            grid.filter(["Balance", "<", 0]);
        if (val === statuses[3])
            grid.filter(["Balance", "=", 0]);
    }
    $scope.init = function () {
        $rootScope.pageTitle("بارگیری بدهکاران و بستانکاران...");
        $('#businessNav').show();
        $scope.fromDate = "1397/01/01";// $scope.currentFinanYear.DisplayStartDate;
        $scope.toDate = "1397/12/29";// $scope.currentFinanYear.DisplayEndDate;
        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
            elementId: 'gridContainer',
            layoutKeyName: 'grid-report-debtorsCreditors',
            height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 50),
            columns: [
				{
				    caption: 'کد', dataField: 'DetailAccount.Code', width: 60, printWidth: 2, hidingPriority: 6, cellTemplate: function (cellElement, cellInfo) {
				        cellElement.html("<a class='text-info txt-bold' href='#contactCard/" + cellInfo.data.Id + "' target='_blank'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</a>");
				    }
				},
				//{ caption: 'گروه', dataField: 'DetailAccount.FamilyTree', printWidth: 5, hidingPriority: 3 },
				{ caption: 'نام', dataField: 'Name', printWidth: 5 },
				{ caption: 'تلفن', dataField: 'Phone', width: 100, printWidth: 2, hidingPriority: 4 },
				{ caption: 'موبایل', dataField: 'Mobile', width: 100, printWidth: 2, hidingPriority: 5 },
				{ caption: 'فروشنده', dataField: 'IsVendor', dataType: "boolean", width: 60, allowHeaderFiltering: false, printVisible: false, printWidth: 1, hidingPriority: 1 },
				{ caption: 'مشتری', dataField: 'IsCustomer', dataType: "boolean", width: 60, allowHeaderFiltering: false, printVisible: false, printWidth: 1, hidingPriority: 2 },
				{ caption: 'سهامدار', dataField: 'IsShareHolder', dataType: "boolean", width: 60, allowHeaderFiltering: false, printVisible: false, printWidth: 1, hidingPriority: 0 },
				{ caption: 'گردش بدهکار', dataField: 'Liability', dataType: "number", format: "#", width: 120, printFormat: "#", printWidth: 2, hidingPriority: 8 },
				{ caption: 'گردش بستانکار', dataField: 'Credits', dataType: "number", format: "#", width: 120, printFormat: "#", printWidth: 2, hidingPriority: 7 },
				{
				    caption: 'مانده', dataField: 'Balance', dataType: "number", width: 120, printWidth: 2, printFormat: "#", printBold: true, cellTemplate: function (ce, ci) {
				        var a = Hesabfa.money(Math.abs(ci.data.Balance));
				        if (ci.data.Balance <= 0)
				            ce.html('<span class="text-primary txt-bold">' + a + '</span>');
				        else
				            ce.html('<span class="text-danger txt-bold">' + a + '</span>');
				    },
				    printCalcValue: function (data) {
				        return Math.abs(data.Balance);
				    }
				},
				{
				    caption: 'تشخیص', dataField: '', width: 60, printWidth: 1, calculateCellValue: function (rowData) {
				        if (rowData.Balance === 0)
				            return "-";
				        else if (rowData.Balance > 0)
				            return "بد";
				        else
				            return "بس";
				    }
				}
            ],
            print: {
                fileName: "بدهکاران و بستانکاران.pdf",
                rowColumnWidth: 1,
                page: {
                    landscape: true
                },
                header: {
                    title1:  $rootScope.currentBusiness.Name,
                    title2: "گزارش بدهکاران و بستانکاران",
                    left: $scope.currentFinanYear.Name,
                    center: function () {
                        return $scope.selectedDateRange;
                    },
                    right: function () {
                        return cmb.option("value");
                    }
                }
            }
        });
        applyScope($scope);
        $scope.GetContactsBalance();
    };
    cmb = $("#selectStatus").dxSelectBox({
        dataSource: statuses,
        value: statuses[0],
        rtlEnabled: true,
        onValueChanged: function (data) {
            applyFilter();
        }
    }).dxSelectBox("instance");
    $scope.GetContactsBalance = function () {
        $scope.loading = true;
        grid.beginCustomLoading();
          $.ajax({
            type:"POST",
            //data: JSON.stringify({ start: "", end: "" }),
            url: "/app/api/Transaction/GetContactsBalance",
            contentType: "application/json"
           }).done(function (res) {
               var data = res.data;
                $scope.loading = false;
                grid.fill(data);
                $rootScope.pageTitle("بدهکاران و بستانکاران");
                grid.endCustomLoading();
                applyFilter();
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