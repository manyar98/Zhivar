define(['application','digitGroup', 'number', 'dx', 'roweditor', 'helper', 'gridHelper'], function (app) {
    app.register.controller('transfersController', ['$scope', '$rootScope', '$state','$stateParams',
        function ($scope,$rootScope, $state, $stateParams) {

    var grid;
    $scope.init = function () {
        $rootScope.pageTitle("لیست انتقال وجه ها");
        $("#businessNav").show();

        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
            elementId: 'gridContainer',
            layoutKeyName: "grid-transfers",
            selection: { mode: "multiple" },
            height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 50),
            wordWrapEnabled: true,
            columns: [
				{ caption: 'از حساب', dataField: 'FromDetailAccountName', width: 130, printWidth: 4 },
				{ caption: 'به حساب', dataField: 'ToDetailAccountName', width: 130, printWidth: 4 },
				{ caption: 'تاریخ', dataField: 'DisplayDate', width: 100, printWidth: 2 },
				{ caption: 'مبلغ ' + $scope.getCurrency(), width: 120, printWidth: 2.5, dataField: 'Amount', dataType: 'number', format: "#", printFormat: "#" },
				{ caption: '', dataField: 'FromDetailAccountId', printVisible: false, visible: false },
				{ caption: '', dataField: 'ToDetailAccountId', printVisible: false, visible: false },
				{ caption: 'شرح', dataField: 'Description', printWidth: 8 },
				{ caption: 'ارجاع از', dataField: 'FromReference', width: 90, printWidth: 2, hidingPriority: 2 },
				{ caption: 'ارجاع به', dataField: 'ToReference', width: 90, printWidth: 2, hidingPriority: 1 },
				{ caption: 'شماره سند', dataField: 'DocumentNumber', width: 100, printWidth: 2, hidingPriority: 0 },
				{
				    caption: '', dataField: '', width: 60, printVisible: false, cellTemplate: function (cellElement, cellInfo) {
				        var html = "<a class='text-info p-l-15' href='javascript:void(0);' onclick='angular.element(this).scope().redirect(" + cellInfo.data.DocumentId + ")'><span class='fa fa-pencil'></span></a>";
				        html += "<a class='text-danger' href='javascript:void(0);' onclick='angular.element(this).scope().deleteTransfer(" + cellInfo.data.ID + "," + cellInfo.data.DocumentNumber + "," + cellInfo.data.Amount + ")'><span class='fa fa-times'></span></a>";
				        cellElement.html(html);
				    }
                },
                {
                    caption: 'گردش کار', dataField: '', width: 80, printVisible: false, cellTemplate: function (cellElement, cellInfo) {
                        if (cellInfo.data.Status === 0) {

                            var html = "<button type='button' class='k-button k-button - icontext' onclick='angular.element(this).scope().startInvoice(" + cellInfo.data.ID + "," + cellInfo.data.Status + "," + '"' + cellInfo.data.FromDetailAccountName + '"' + "," + '"' + cellInfo.data.ToDetailAccountName + '"' +")'><span class='fa fa-reply'></span>ارسال</button>";
                            cellElement.html(html);
                        }

                        else {
                            var html = "<button type='button' class='k-button k-button - icontext' onclick='angular.element(this).scope().startInvoice(" + cellInfo.data.ID + "," + cellInfo.data.Status + "," + '"' + cellInfo.data.FromDetailAccountName + '"' + "," + '"' + cellInfo.data.ToDetailAccountName + '"' +")'><span class='fa fa-reply'></span>ارسال</button>";
                            cellElement.html(html);
                        }

                    }
                },
            ],
            print: {
                fileName: "TransferList.pdf",
                rowColumnWidth: 1.5,
                page: {
                    landscape: false
                },
                header: {
                    title1: "",//$rootScope.currentBusiness.Name,
                    title2: $rootScope.pageTitleText
                }
            }
        });

        $scope.getTransactions();
        applyScope($scope);
    };
    $scope.getTransactions = function () {
        $scope.loading = true;
        
        $.ajax({
            type: "POST",
            //data: JSON.stringify($scope.type),
            url: "/app/api/TransferMoney/GetTransferList",
            contentType: "application/json"
        }).done(function (res) {
            var list = res.data;
       
                $scope.loading = false;
                grid.fill(list);
                applyScope($scope);
            }).fail(function (error) {
                $scope.loading = false;
                applyScope($scope);
                if ($scope.accessError(error))
                    return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.redirect = function (docId) {
        $state.go('moneyTransfer', { docId: docId });
    };

    $scope.startWorkflow = function (id, status, from,to) {

        if (status === 1) {
            DevExpress.ui.notify(" این فاکتور قبلا ارسال شده است. ", "warning", 3000);

            return;
        }
        else if (status === 2 || status === 3) {
            DevExpress.ui.notify(" این فاکتور قبلا ارسال شده است. ", "warning", 3000);

            return;
        }
        $scope.model = {
        };

       
            var InstanceTitle =" از " +from + " به "+to;//this.dataItem($(e.currentTarget).closest("tr")).DaroukhanehNam;
        


        $scope.model.ID = id;
        // var ID = this.dataItem($(e.currentTarget).closest("tr")).ID;
        $scope.model.OrganizationID = 22;//this.dataItem($(e.currentTarget).closest("tr")).OrganizationID;

        var organizationID = 22;//this.dataItem($(e.currentTarget).closest("tr")).OrganizationID;


        dataService.addEntity('/app/api/Invoice/StartWorkFlow', $scope.model).then(function (data) {
            messageService.success('عملیات با موفقیت انجام شد.');
            grid.refresh();
        });

    }
    $scope.deleteTransfer = function (id, number, amount) {
        questionbox({
            title: "هشدار",
            content: "آیا از حذف این انتقال مطمئن هستید؟" + "<br/>"
			+ "شماره سند: " + number + " ، مبلغ: " + amount,
            onBtn1Click: function () {
                if ($scope.calling) return false;
                $scope.calling = true;

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(id),
                    url: "/app/api/TransferMoney/DeleteTransfer",
                    contentType: "application/json"
                }).done(function (res) {
                    var rp = res.data;
                        $scope.calling = false;
                        findAndRemove(grid._options.dataSource, rp);
                        grid.refresh();
                        DevExpress.ui.notify(Hesabfa.farsiDigit("انتقال وجه حذف شد. شماره: " + rp.DocumentNumber + " ، مبلغ: " + rp.Amount), "success", 3000);
                        $scope.$apply();
                    }).fail(function (error) {
                        $scope.calling = false;
                        applyScope($scope);
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error });
                    });
            }
        });
    };
    $scope.showDocument = function (id) {
        $scope.viewDocumentModal(id);
    };
    $scope.exportToCsv = function () {
        var csvContent = "";
        var transactions = $scope.grid.data;
        // headers
        csvContent += "ردیف" + "," + "نوع" + "," + "مبلغ" + "," + "مبدا/مقصد" + "," + "تاریخ" + "," + "شرح" + "," + "ارجاع" + "\n";
        var length = transactions.length;
        for (var i = 0; i < length; i++) {
            var trans = transactions[i];
            var dataString = i + 1 + "," + (trans.Type === 0 ? 'دریافت' : 'پرداخت') + "," + trans.Amount + "," + trans.DetailAccount.Name + "," + trans.DisplayDate + "," + trans.Description + "," + trans.Reference;
            csvContent += i < length ? dataString + "\n" : dataString;
        }
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            var contentType = 'text/csv';
            var blob = b64toBlob(window.btoa(unescape(encodeURI(csvContent))), contentType);
            window.navigator.msSaveOrOpenBlob(blob, "money_transactions.csv");
        }
        else {
            var encodedUri = "data:text/csv;charset=utf-8," + encodeURI(csvContent);
            var link = document.createElement("a");
            link.href = encodedUri;
            link.download = "money_transactions.csv";
            link.target = '_blank';
            document.body.appendChild(link);
            link.click();
        }
    };

    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
    


        }])
});