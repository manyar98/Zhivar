define(['application', 'digitGroup', 'number', 'dx', 'roweditor', 'helper', 'gridHelper'], function (app) {
    app.register.controller('journalReportController', ['$scope','$rootScope','$stateParams', '$location','$state',
        function ($scope,$rootScope,$stateParams, $location,$state) {
    var grid, cmb;
    var statuses = ['نمایش همه تراکنش ها', 'نمایش تراکنش های اتوماتیک', 'نمایش تراکنش های دستی'];
    function applyFilter() {
        var val = cmb.option("value");
        if (val === statuses[0])
            grid.clearFilter();
        if (val === statuses[1])
            grid.filter(["IsManual", "=", false]);
        if (val === statuses[2])
            grid.filter(["IsManual", "=", true]);
    }

    $scope.init = function () {
        $rootScope.pageTitle("بارگیری دفتر روزنامه...");
        $('#businessNav').show();
        $scope.fromDate = window.todayDate;
        $scope.toDate = window.todayDate;
        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
            elementId: 'gridContainer',
            layoutKeyName: 'grid-report-journal',
            height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 50),
            columns: [
				{ caption: 'تاریخ', dataField: 'DisplayDate', width: 90, printWidth: 2 },
				{
				    caption: 'سند', dataField: 'Number', width: 60, printWidth: 1.5, cellTemplate: function (ce, ci) {
				        var txt = Hesabfa.farsiDigit(ci.data.Number);
				        if (ci.data.Id > 0)
				            ce.html("<a href='javascript:void(0);' onclick='angular.element(this).scope().viewDocumentModal(" + ci.data.Id + ")' class='text-primary txt-bold'>" + txt + "</a>");
				        else
				            ce.html(txt);
				    }
				},
				{ caption: 'کد حساب', dataField: 'Code', width: 100, printWidth: 3 },
				{
				    caption: 'نام حساب', dataField: 'Name', printWidth: 5, cellTemplate: function (ce, ci) {
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
				{ caption: 'شــرح', dataField: 'Description', printWidth: 10 },
				{ caption: 'بدهکار', dataField: 'Debit', dataType: "number", format: "<b>#</b>", width: 120, printWidth: 3, printFormat: "#" },
				{ caption: 'بستانکار', dataField: 'Credit', dataType: "number", format: "<b>#</b>", width: 120, printWidth: 3, printFormat: "#" }
            ]
		    , print: {
		        fileName: "journalReport.pdf",
		        rowColumnWidth: 1.5,
		        page: {
		            landscape: false
		        },
		        header: {
                     title1: $rootScope.currentBusiness.Name,
                    title2: "گزارش دفتر روزنامه",
                    center: $scope.currentFinanYear.Name,
                    left: "", right: ""
		         

		        }
		    }
        });
        applyScope($scope);
        $scope.getJournal();
    };
    cmb = $("#selectStatus").dxSelectBox({
        dataSource: statuses,
        value: statuses[0],
        rtlEnabled: true,
        onValueChanged: function (data) {
            applyFilter();
        }
    }).dxSelectBox("instance");

    $scope.getJournal = function () {
        $scope.loading = true;
        grid.beginCustomLoading();

        var model = {
            start: $scope.fromDate, 
            end: $scope.toDate
        };

        $.ajax({
            type:"POST",
            data: JSON.stringify(model),
            url: "/app/api/Transaction/GetJournal",
            contentType: "application/json"
        }).done(function (res) {
            var data =res.data;;
                $scope.loading = false;
                grid.fill(data);
                $rootScope.pageTitle("دفتر روزنامه");
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
    $scope.redirect = function (url) {
        $state.go(url);
    };
    
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);

}])
});