define(['application', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper'], function (app) {
    app.register.controller('capitalStatementController', ['$scope','$rootScope',
        function ($scope, $rootScope) {
    var grid;
    $scope.init = function () {
        $rootScope.pageTitle("بارگیری صورتحساب سرمایه...");
        $('#businessNav').show();

        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
            elementId: 'gridContainer',
            layoutKeyName: 'grid-report-capitalStatement',
            sorting: { mode: 'none' },
            filterRow: { visible: false },
            headerFilter: { visible: false },
            height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 100),
            columns: [
            { caption: 'شــرح', printWidth: 10, dataField: 'Description' },
            {
                caption: 'مبلغ', printWidth: 2, dataField: 'Amount', width: 140, dataType: "number", format: "#", printFormat: "#", cellTemplate: function (cellElement, cellInfo) {
                    cellElement.html("<span class='" + (cellInfo.data.Type === 'Debit' ? 'red' : '') + "'>" + $scope.money(cellInfo.data.Amount) + "</span>");
                }
            },
            { caption: 'تاریخ', printWidth: 2, dataField: 'Date', width: 95 }
            ],
            print: {
                fileName: "CapitalStatement.pdf",
                rowColumnWidth: 1.5,
                page: {
                    landscape: false
                },
                header: {
                    title1: $rootScope.currentBusiness.Name,
                    title2: "صورتحساب سرمایه",
                    center: $scope.currentFinanYear.Name,
                    left: "", right: ""
                }
            }
        });

        $scope.reportDate = "";
        $scope.getCapitalStatement();
        applyScope($scope);
    };
    $scope.getCapitalStatement = function () {
        $scope.loading = true;
        $.ajax({
            type:"POST",
            //data: JSON.stringify({ start: "", end: "" }),
            url:"/app/api/Transaction/GetCapitalStatement",
            contentType: "application/json"
        }).done(function (res) {
            var capitalStatementData = res.data;
                $scope.loading = false;
                grid.fill(capitalStatementData.tableCapitals);
                $scope.reportDate = capitalStatementData.reportDate;
                $rootScope.pageTitle("صورتحساب سرمایه");
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
    $scope.generatePDF = function () {
        grid.pdf();
    };
    $scope.excel = function () {
        grid.exportToExcel(false);
    };
    
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);

}])
});