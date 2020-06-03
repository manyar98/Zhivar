define(['application', 'combo', 'scrollbar', 'helper', 'nodeSelect', 'gridHelper','dx'], function (app) {
    app.register.controller('receiveAndPayController', ['$scope','$rootScope', '$stateParams','$state',
        function ($scope, $rootScope, $stateParams, $state) {

    var grid;
    $scope.init = function () {
        $scope.loading = true;

        $rootScope.pageTitle("بارگیری تراکنش ها...");
        $("#businessNav").show();
        $scope.grid = new gridHelper({ scope: $scope, rpp: 25, searchBy: ["Number", "Invoice.Number", "DisplayDate", "Description", "Amount", "AccountName", "DetailAccountName"] });
        var param = $stateParams.isReceive;
        $scope.isReceive = !!($stateParams && $stateParams.isReceive === "receive");

        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
            elementId: 'gridContainer',
            layoutKeyName: "grid-" + param,
            selection: { mode: "multiple" },
            height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 50),
            wordWrapEnabled: true,
            columns: [
				{
				    caption: 'شماره رسید', dataField: 'Number', width: 90, printWidth: 2, cellTemplate: function (cellElement, cellInfo) {
				        cellElement.html("<span class='text-info txt-bold hand'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</span>");
				    }
				},
				//{
    //                caption: 'شماره فاکتور', dataField: 'InvoiceId', width: 110, printWidth: 3, cellTemplate: function (cellElement, cellInfo) {
				//        cellElement.html("<a class='text-info txt-bold' href='#viewInvoice/id=" + (cellInfo.data.Invoice ? cellInfo.data.Invoice.Id : "") + "'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</a>");
				//    }
				//},
				{ caption: 'تاریخ', dataField: 'DisplayDate', width: 100, printWidth: 3 },
				{ caption: 'شرح', dataField: 'Description', printWidth: 8 },
				{ caption: 'مبلغ ' + $scope.getCurrency(), width: 120, printWidth: 3, dataField: 'Amount', dataType: 'number', format: "#", printFormat: "#" },
				{ caption: 'بابت', dataField: 'Type', width: 90, printWidth: 2, hidingPriority: 0 },
				{ caption: 'حساب معین', dataField: 'AccountName', printWidth: 4, hidingPriority: 1 },
				{ caption: 'حساب تفصیلی', dataField: 'DetailAccountName', printWidth: 5, hidingPriority: 2 },
				{
                    caption: 'عملیات', dataField: '', width: 60, printVisible: false, cellTemplate: function (cellElement, cellInfo) {
				        //var page = $scope.isReceive ? "receive" : "pay";
				        //if (cellInfo.data.Type === "پرداخت گروهی")
				        //    page = "pay-group";
                        var html = "<a class='text-info p-l-15' href='javascript:void(0);' onclick='angular.element(this).scope().editReceiveAndPay(" + cellInfo.data.ID + "," + cellInfo.data.Number + "," + cellInfo.data.Amount + ")'><span class='fa fa-pencil'></span></a>"; 
				        html += "<a class='text-danger' href='javascript:void(0);' onclick='angular.element(this).scope().deleteReceiveAndPay(" + cellInfo.data.ID + "," + cellInfo.data.Number + "," + cellInfo.data.Amount + ")'><span class='fa fa-times'></span></a>";
				        cellElement.html(html);
				    }
                },
                {
                    caption: 'گردش کار', dataField: '', width: 80, printVisible: false, cellTemplate: function (cellElement, cellInfo) {
                        //if (cellInfo.data.Status === 0) {

                        //    var html = "<button type='button' class='k-button k-button - icontext' onclick='angular.element(this).scope().startWorkflow(" + cellInfo.data.ID + "," + cellInfo.data.Status + "," + cellInfo.data.Contact.Title + ")'><span class='fa fa-reply'></span>ارسال</button>";

                        //    cellElement.html(html);
                        //}

                        //else {
                        var html = "<button type='button' class='k-button k-button - icontext' onclick='angular.element(this).scope().startWorkflow(" + cellInfo.data.ID + "," + cellInfo.data.Status + ")'><span class='fa fa-reply'></span>ارسال</button>";
                            cellElement.html(html);
                       // }

                    }
                },

            ],
            print: {
                fileName: "RecievesList.pdf",
                rowColumnWidth: 1.5,
                page: {
                    landscape: false
                },
                header: {
                    title1: $rootScope.currentBusiness.Name,
                    title2: $rootScope.pageTitleText
                }
            }
        });

        $scope.getTransactions();
        applyScope($scope);
    };

    $scope.startWorkflow = function (id, status, title) {

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

        if ($scope.isReceive)
        {
            $scope.model.Code = 'Recive';
            var InstanceTitle = " رسید دریافت "; //+ title;//this.dataItem($(e.currentTarget).closest("tr")).DaroukhanehNam;
            $scope.model.InstanceTitle = InstanceTitle;

        }
        else
        {
            $scope.model.Code = 'Pay';
            var InstanceTitle = " رسید پرداخت ";// + title;//this.dataItem($(e.currentTarget).closest("tr")).DaroukhanehNam;
            $scope.model.InstanceTitle = InstanceTitle;

        }
            

        $scope.model.ID = id;
        // var ID = this.dataItem($(e.currentTarget).closest("tr")).ID;
        $scope.model.OrganizationID = 22;//this.dataItem($(e.currentTarget).closest("tr")).OrganizationID;

        var organizationID = 22;//this.dataItem($(e.currentTarget).closest("tr")).OrganizationID;


        dataService.addEntity('/app/api/Invoice/StartWorkFlow', $scope.model).then(function (data) {
            messageService.success('عملیات با موفقیت انجام شد.');
            grid.refresh();
        });

    }
    $scope.getTransactions = function () {
        $scope.loading = true;
        $.ajax({
            type: "POST",
            data: JSON.stringify($scope.isReceive),
            url: "/app/api/PayRecevie/GetReceiveAndPayList",
            contentType: "application/json"
        }).done(function (res) {
            var list = res.data;

     
                $scope.loading = false;
                grid.fill(list);
                applyScope($scope);
            }).fail(function (error) {
                $scope.loading = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.search = function () {
        $scope.grid.search($scope.searchValue, $scope);
    };
    $scope.editReceiveAndPay = function (id, number, amount) {

        questionbox({
            title: "هشدار",
            content: "آیا از ویرایش این رسید مطمئن هستید؟" + "<br/>"
            + "شماره رسید: " + number + " ، مبلغ: " + amount,
            onBtn1Click: function () {
                if ($scope.calling) return false;
                $scope.calling = true;
                if ($scope.isReceive)
                    $state.go('receive', { rpId: id });
                else
                    $state.go('pay', { rpId: id });
              
            }
        });    
    };

    $scope.deleteReceiveAndPay = function (id, number, amount) {
        questionbox({
            title: "هشدار",
            content: "آیا از حذف این رسید مطمئن هستید؟" + "<br/>"
            + "شماره رسید: " + number + " ، مبلغ: " + amount,
            onBtn1Click: function () {
                if ($scope.calling) return false;
                $scope.calling = true;

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(id),
                    url: "/app/api/PayRecevie/DeleteReceiveAndPay",
                    contentType: "application/json"
                }).done(function (res) {
                    var rp = res.data;

                    if (res.resultCode == 1) {
                        $scope.calling = false;
                        if ($scope.accessError(res.data)) { applyScope($scope); return; }
                        alertbox({ content: res.data, type: "warning" });
                      
                        applyScope($scope);
                        window.scrollTo(0, 0);
                        return;
                    }

                        $scope.calling = false;
                        findAndRemove(grid._options.dataSource, rp);
                        grid.refresh();
                        DevExpress.ui.notify(Hesabfa.farsiDigit("رسید حذف شد. شماره: " + rp.Number + " ، مبلغ: " + rp.Amount), "success", 3000);
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
    $scope.exportTransactionsToExcel = function () {
        if ($scope.callingImport) return;
        $scope.callingImport = true;
        $('#loadingModal').modal('show');
        callws(DefaultUrl.MainWebService + 'ExportMoneyTransactionsToExcel', {})
            .success(function (data) {
                $scope.callingImport = false;
                $('#loadingModal').modal('hide');
                $scope.$apply();
                var fileName = "MoneyTransactions";

                var contentType = 'application/vnd.ms-excel';
                var blob = b64toBlob(data, contentType);
                if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                    window.navigator.msSaveOrOpenBlob(blob, fileName + ".xlsx");
                }
                else {
                    var objectUrl = URL.createObjectURL(blob);
                    //                    window.open(objectUrl);
                    //                    var encodedUri = "data:application/vnd.ms-excel," + encodeURI(data);
                    var link = document.createElement("a");
                    link.href = objectUrl;
                    link.download = fileName + ".xlsx";
                    link.target = '_blank';
                    document.body.appendChild(link);
                    link.click();
                }

            }).fail(function (error) {
                $scope.callingImport = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error });
            }).loginFail(function () {
                window.location = DefaultUrl.login;
            });

    };

    $scope.redirect = function (url) {
        $state.go(url);

    }
    //
        $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
    }])
});