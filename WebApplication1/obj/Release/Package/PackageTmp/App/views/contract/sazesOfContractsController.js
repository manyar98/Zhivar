define(['application', 'dataService', 'digitGroup', 'number', 'dx', 'roweditor', 'helper', 'gridHelper', 'messageService'], function (app) {
    app.register.controller('sazesOfContractsController', ['$scope', 'dataService', '$location', '$stateParams', '$compile', '$state', '$rootScope', 'messageService',
        function ($scope,dataService, $location, $stateParams, $compile, $state, $rootScope, messageService) {

            //    app.register.controller('invoicesController', ['$window',
            //        function (  $window) {

            $rootScope.applicationModule = "رسانه های قرارداد";
            $scope.applicationDescription = "در این صفحه امکان جستجو، اضافه و ویرایش رسانه های قرارداد وجود دارد.";


            var grid;
            $("#progressBarDiv").hide();

            $scope.init = function () {
                $('#businessNav').show();
                $("#progressBarDiv").hide();

                $scope.contractId = $stateParams.id;
                $scope.searchBy = "number";
                $scope.searchValue = "";
                $scope.otherFilters = "";

                $scope.tab = null;

                var status = $stateParams.status;// $scope.getRouteQuery($routeParams.params, 'status');
                var show = $stateParams.show;//$scope.getRouteQuery($routeParams.params, 'show');
                var cid = $stateParams.contactid;//$scope.getRouteQuery($routeParams.params, 'contactid');
                if (cid) $scope.contactId = cid;

                var invoiceId = $stateParams.invoiceId; //$scope.getRouteQuery($routeParams.params, 'invoiceId');
                var invoiceNumber = $stateParams.invoiceNumber; //$scope.getRouteQuery($routeParams.params, 'invoiceNumber');
                var invoiceReference = $stateParams.invoiceReference; //$scope.getRouteQuery($routeParams.params, 'invoiceReference');
                var invoiceType = $stateParams.invoiceType; //$scope.getRouteQuery($routeParams.params, 'invoiceType');
                var contactName = $stateParams.contactName; //$scope.getRouteQuery($routeParams.params, 'contactName');
                var price = $stateParams.price; //$scope.getRouteQuery($routeParams.params, 'price');
                var smsSent = $stateParams.smsSent; // $scope.getRouteQuery($routeParams.params, 'smsSent');

                if (status && status === 'draft' && invoiceId) {
                    $scope.alert = true;
                    $scope.alertMessage = "فاکتور {2} ذخیره شد - {0} - مبلغ کل: {1}".
                        formatString(contactName, $scope.money(price), invoiceNumber !== "" ? invoiceNumber : invoiceReference, invoiceType === 0 ? "فروش" : "خرید");
                    $scope.alertType = 'success';
                    applyScope($scope);
                }
                else if (status && status === 'awaitingApproval' && invoiceId) {
                    $scope.alert = true;
                    $scope.alertMessage = "فاکتور جهت تایید ثبت و ذخیره شد - {0} - مبلغ کل: {1}".
                        formatString(contactName, $scope.money(price));
                    if (smsSent) $scope.alertMessage += " [پیامک ارسال شد]";
                    $scope.alertType = 'success';
                    $scope.$apply();
                }
                else if (status && status === 'payment' && transId) {
                    $scope.alert = true;
                    $scope.alertMessage = "پرداخت به مبلغ {0} ثبت شد - {1} - سر رسید: {2} - پرداخت در {3} - مبلغ فاکتور: {4} - باقیمانده: {5}".
                        formatString($scope.money(transAmount), contactName, dueDateShamsi, dateTimeShamsi,
                        $scope.money(price), $scope.money(rest));
                    $scope.alertType = 'success';
                    $scope.$apply();
                }
                else if (status && status === 'deleted' && invoiceId) {
                    $scope.alert = true;
                    $scope.alertMessage = "فاکتور شماره {0} از سیستم حذف شد.".
                        formatString(invoiceNumber === "" ? (invoiceReference === "" ? "---" : invoiceReference) : invoiceNumber);
                    $scope.alertType = 'success';
                    $scope.$apply();
                }

                var invoicesNewWindow = $rootScope.getUISetting("invoicesNewWindow");
                $scope.openNewWindow = !!(invoicesNewWindow && invoicesNewWindow === "true");

                var filter = $stateParams.filter;// $scope.getRouteQuery($routeParams.params, 'filter');
                if (filter && filter === 'draft') {
                    $scope.tab = 0;
                    $('#tabs li:eq(1) a').tab('show');
                } else if (filter && filter === 'awaitingPayment') {
                    $scope.tab = 1;
                    $('#tabs li:eq(3) a').tab('show');
                } else if (filter && filter === 'overDue') {
                    $scope.tab = 1;
                    $('#tabs li:eq(3) a').tab('show');
                }

                if (show && show === "bills") {
                    $scope.type = 1;
                    $scope.workflowCode = "InvoiceBuy";
                }
                else if (show && show === "saleReturns") {
                    $scope.type = 2;
                    $scope.workflowCode = "InvoiceReturnSell";
                }
                else if (show && show === "purchaseReturns") {
                    $scope.type = 3;
                    $scope.workflowCode = "InvoiceReturnBuy";
                }
                else if (show && show === "wastes") {
                    $scope.type = 4;
                    $scope.workflowCode = "";
                }
                else {
                    $scope.type = 0;
                    $scope.workflowCode = "InvoiceSell";
                }

                if ($scope.type === 0) $rootScope.pageTitle("فاکتورهای فروش");
                else if ($scope.type === 1) $rootScope.pageTitle("فاکتورهای خرید");
                else if ($scope.type === 2) $rootScope.pageTitle("برگشت از فروش ها");
                else if ($scope.type === 3) $rootScope.pageTitle("برگشت از خرید ها");
                else if ($scope.type === 4) $rootScope.pageTitle("ضایعات");

                var lookup = { dataSource: [{ key: 2, val: "ثبت" }, { key: 3, val: "ارسال" }, { key: 4, val: "تمام شده" }], valueExpr: 'key', displayExpr: 'val' };
               // var lookup = { dataSource: [{ key: 0, val: "ذخیره موقت" }, { key: 1, val: "منتظر تایید" }, { key: 2, val: $scope.type === 0 ? 'منتظر دریافت' : 'منتظر پرداخت' }, { key: 3, val: $scope.type === 0 ? 'دریافت شده' : 'پرداخت شده' }], valueExpr: 'key', displayExpr: 'val' };
                function getStatusIcon(invoice) {
                    if (invoice.Returned) return "<span class='fa fa-reply red pull-center'></span>";
                    else if (invoice.Status === 0) return "<span class='fa fa-save text-info pull-center'></span>";
                    else if (invoice.Status === 1) return "<span class='fa fa-hourglass txt-silver pull-center'></span>";
                    else if (invoice.Status === 2) return "<span class='fa fa-check txt-silver pull-center'></span>";
                    else if (invoice.Status === 3) return "<span class='fa fa-check text-success pull-center'></span>";
                    else return "";
                }
                grid = dxGrid({
                    columnFixing: {
                        enabled: true
                    },
                    elementId: 'gridContainer',
                    layoutKeyName: "grid-factor-" + $scope.type,
                   // selection: { mode: "multiple" },
                    columns: [
                        {
                            caption: '',
                            width: 30,
                            dataField: '',
                            allowHeaderFiltering: false,
                            allowSorting: false,
                            cellTemplate: function (cellElement, cellInfo) {
                                cellElement.html(getStatusIcon(cellInfo.data));
                            },
                            printWidth: 3
                        },
                        //{
                        //    caption: 'شماره',
                        //    width: 100,
                        //    dataField: 'Number',
                        //    dataType: 'number',
                        //    cellTemplate: function (cellElement, cellInfo) {
                        //        if (cellInfo.data.Status === 0)
                        //            cellElement.html("<a class='text-info txt-bold' href='#editInvoice/" + cellInfo.data.Id + "'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</a>");
                        //        else
                        //            cellElement.html("<a class='text-info txt-bold' href='#viewInvoice/id=" + cellInfo.data.Id + "'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</a>");
                        //    },
                        //    printWidth: 3
                        //},
                        //{ caption: 'ارجاع', dataField: 'Reference', width: 100, printWidth: 3, hidingPriority: 5 },
                        {
                            caption: 'نام رسانه',
                            dataField: 'Saze.Title',
                            width: 200,
                            printWidth: 7,
                            cellTemplate: function (cellElement, cellInfo) {
                                cellElement.html("<a class='text-info txt-bold' href='#contactCard/" + (cellInfo.data.Contact ? cellInfo.data.Contact.Id : "") + "'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</a>");
                            }
                        },
                        { caption: ' تاریخ شروع اکران/عملیات', dataField: 'DisplayTarikhShorou', width: 80 },
                        { caption: 'فی اجاره', dataField: 'UnitPrice', width: 80, printFormat: "#" },
                        { caption: 'مدت اجاره', dataField: 'Quantity', dataType: "number", format: "#", width: 60, printFormat: "#" },
                        { caption: 'نوع اجاره', dataField: 'NoeEjare.Title', width: 80 },
                        { caption: ' بازاریاب', dataField: 'PriceBazareab', dataType: "number", format: "#", width: 80, printFormat: "#" },
                        { caption: ' طراحی', dataField: 'PriceTarah', dataType: "number", format: "#", width: 80},
                        { caption: ' چاپ', dataField: 'PriceChap', dataType: "number", format: "#", width: 80 },
                        { caption: ' نصب', dataField: 'PriceNasab', dataType: "number", format: "#", width: 80 },
                        { caption: 'مبلغ کل', dataField: 'TotalAmount', dataType: "number", format: "#", width: 80 },
                        {
                            caption: 'وضعیت',
                            dataField: 'Status',
                            width: 100,
                            printWidth: 3,
                            lookup: lookup,
                           // hidingPriority: 6,
                            printCalcValue: function (data) {
                                for (var l in lookup.dataSource) {
                                    if (lookup.dataSource[l].key === data.Status)
                                        return lookup.dataSource[l].val;
                                }
                            }
                        },
                        //{
                        //    caption: 'ارسال',
                        //    dataField: 'Sent',
                        //    dataType: 'boolean',
                        //    width: 20,
                        //    printWidth: 2,
                        //    allowHeaderFiltering: false,
                        //    allowSorting: false,
                        //    headerCellTemplate: function (cellElement, cellInfo) {
                        //        cellElement.html("<span class='fa fa-envelope'></span> ");
                        //    }
                        //},
                        //{ caption: 'استان', dataField: 'State', width: 60, hidingPriority: 1 },
                        //{ caption: 'شهر', dataField: 'City', width: 60, hidingPriority: 0 },
                        {
                            caption: 'عملیات', dataField: '', width: 60, printVisible: false, cellTemplate: function (cellElement, cellInfo) {
                                //if (cellInfo.data.Status === 0) {
                                  //  var html = "<a class='text-info p-l-15' href='javascript:void(0);' onclick='angular.element(this).scope().editInvoice(" + cellInfo.data.ID + ")'><span class='fa fa-pencil'></span></a>";
                                    //html += "<a class='text-danger' href='javascript:void(0);' onclick='angular.element(this).scope().deleteInvoice()'><span class='fa fa-times'></span></a>";
                               //     cellElement.html(html);
                              //  }

                              //  else {
                                    var html = "<a class='text-info p-l-15' href='javascript:void(0);' onclick='angular.element(this).scope().viewInvoice(" + cellInfo.data.ID + ")'><span class='fa fa-pencil'></span></a>";
                                    //html += "<a class='text-danger' href='javascript:void(0);' onclick='angular.element(this).scope().deleteInvoice()'><span class='fa fa-times'></span></a>";
                                    cellElement.html(html);
                             //   }

                            }
                        },
                        //{
                        //    caption: 'گردش کار', dataField: '', width: 60, printVisible: false, cellTemplate: function (cellElement, cellInfo) {
                        //        if (cellInfo.data.Status === 0) {

                        //            var html = "<button type='button' class='k-button k-button - icontext' onclick='angular.element(this).scope().startInvoice(" + cellInfo.data.ID + "," + cellInfo.data.Status + "," + '"' + cellInfo.data.Saze.Title + '"' + ")'><span class='fa fa-reply'></span>ارسال</button>";
                                    
                        //            cellElement.html(html);
                        //        }

                        //        else {
                        //            var html = "<button type='button' class='k-button k-button - icontext' onclick='angular.element(this).scope().startInvoice(" + cellInfo.data.ID + "," + cellInfo.data.Status + "," + '"' + cellInfo.data.Saze.Title + '"' + ")'><span class='fa fa-reply'></span>ارسال</button>";
                                    
                        //            cellElement.html(html);
                        //        }

                        //    }
                        //},
                    ],
                    //onCellClick: function (item) {
                    //    if (item.column.index !== -2)
                    //    {
                    //        if (item.data.Status === 0)
                    //        {
                    //            $state.go('invoice', { id: item.data.ID });
                    //        }
                    //        else {
                    //            $state.go('viewInvoice', { id: item.data.ID });
                    //        }
                    //    }


                    //    //				if (!item.column || !item.data) return;
                    //    //				if (item.column.index !== -2 && item.column.index !== 2 && item.column.index !== 0) {
                    //    //					window.open("#viewInvoice/id=" + item.data.Id);
                    //    //				}
                    //},
                    print: {
                        fileName: "InvoiceList.pdf",
                        rowColumnWidth: 1.5,
                        page: {
                            landscape: true
                        },
                        header: {
                            title1: $rootScope.currentBusiness.Name,
                            title2: $rootScope.pageTitleText
                        }
                    }
                });

                $scope.getInvoices();

                $('#tabs a').click(function (e) {
                    e.preventDefault();
                    $(this).tab('show');
                });

                //$(function () {
                //    $('[data-toggle="popover"]').popover();
                //    $.getScript("/js/app/printReports.js?ver=1.2.9.1", function () { });
                //});
            };
            $scope.getInvoices = function () {
                if ($scope.pageLoading) return;
                $scope.pageLoading = true;
                //		$rootScope.setUISetting("invoicesRpp", rpp + "");
                var status = $scope.tab == null ? -1 : $scope.tab;
                grid.beginCustomLoading();
                $.ajax({
                    type: "POST",
                    data: JSON.stringify($scope.contractId),
                    url: "/app/api/Contract/GetSazesOfContract",
                    contentType: "application/json"
                }).done(function (res) {

                    var result = res.data.SazesOfContract;
                    $scope.pageLoading = false;
                    grid.endCustomLoading();
                    $scope.invoices = result;
                    grid.fill(result);
                    $scope.calculateInvoicesStatuses();
                    $scope.today = $scope.todayDate;

                    //				if ($scope.contactId) grid.filter("Contact.Id", "=", $scope.contactId);
                    $state.current.ncyBreadcrumb.label = res.data.Title;
                    $scope.titleButton = res.data.TitleButton;
                    $scope.type = res.data.Type;

                    applyScope($scope);
                }).fail(function (error) {
                    $scope.pageLoading = false;
                    grid.endCustomLoading();
                    applyScope($scope);
                    if ($scope.accessError(error)) return;
                    alertbox({ content: error, type: "error" });
                });
            };
            $scope.selectInvoice = function (invoice) {
                //        document.getElementById('openInvoiceNewWindow').click();
                //        return;

                if (invoice.Status === 0 || invoice.Status === 1) // draft & awaitingApproval
                {
                    if ($scope.openNewWindow) {
                        $window.open("#editInvoice/" + invoice.Id, '_blank');
                    }
                    else
                        $location.path("/editInvoice/" + invoice.Id);
                    return;
                } else {
                    if ($scope.openNewWindow) {
                        $window.open("#viewInvoice/id=" + invoice.Id, '_blank');
                    }
                    else
                        $location.path("/viewInvoice/id=" + invoice.Id);
                    return;
                }
            };
            $scope.search = function () {
                $scope.grid.search($scope.searchValue, $scope);
            };
            $scope.editInvoice = function (id) {
                $state.go('newContract', { id: id });

            }

            

  

            $scope.startInvoice = function (id, status, title) {

                if (status === 1) {
                    DevExpress.ui.notify(" این رسانه قبلا ارسال شده است. ", "warning", 3000);

                    return;
                }
                else if (status === 2 || status === 3) {
                    DevExpress.ui.notify(" این رسانه قبلا ارسال شده است. ", "warning", 3000);

                    return;
                }
                $scope.model = {
                };


                //$scope.model.Code = 'InvoiceSell';
                $scope.model.ID = id;
                // var ID = this.dataItem($(e.currentTarget).closest("tr")).ID;
                $scope.model.OrganizationID = 22;//this.dataItem($(e.currentTarget).closest("tr")).OrganizationID;
                var InstanceTitle = " سازه " + title;//this.dataItem($(e.currentTarget).closest("tr")).DaroukhanehNam;
                $scope.model.InstanceTitle = InstanceTitle;


                // var daroukhanehShakhsID = this.dataItem($(e.currentTarget).closest("tr")).DaroukhanehShakhsID;
                var organizationID = 22;//this.dataItem($(e.currentTarget).closest("tr")).OrganizationID;
                //  var darkhastID = this.dataItem($(e.currentTarget).closest("tr")).ID;
                //  var MasoulFaniShakhsID = this.dataItem($(e.currentTarget).closest("tr")).ShakhsID;

                dataService.addEntity('/app/api/Contract/StartWorkFlow', $scope.model).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد.');
                    grid.refresh();
                });

            }


            $scope.viewInvoice = function (id) {
                $state.go('viewSazeOfContract', { id: id });
            }
            $scope.return = function () {

                if ($scope.type === 1)
                    $state.go('contractsPrimative');
                else if ($scope.type === 2)
                    $state.go('contracts');
                else if ($scope.type === 3)
                    $state.go('rentContracts');
            };

            $scope.deleteInvoice = function () {
                var items = grid.getSelectedRowsData();
                if (items.length === 0) {
                    alertbox({ content: "هیچ فاکتوری جهت حذف انتخاب نشده است" });
                    return;
                }
                var s = items.length === 1 ? "فاکتور" : "فاکتورهای";
                var haveApproved = false;
                for (var i = 0; i < items.length; i++) {
                    if (items[i].Status > 1)
                        haveApproved = true;
                }
                var tip1 = "";
                var tip2 = "";
                if (haveApproved) {
                    //tip1 = "<div class='bg-warning pd-5 small'>";
                    //tip1 += "نکته: با حذف فاکتورهای تایید شده، دریافت ها و پرداخت های ثبت شده برای آنها نیز حذف می گردد.";
                    //tip1 += "</div>";
                    if ($scope.type === 1 || $scope.type === 2) {
                        tip2 = "<div class='bg-warning pd-5 small'>";
                        tip2 += "حذف فاکتور خرید (یا برگشت از فروش) ممکن است باعث ایجاد موجودی منفی در کالاها شود";
                        tip2 += " ، که این موضوع باید توسط شما کنترل شود.";
                        tip2 += "</div>";
                    }
                }
                questionbox({
                    content: "آیا از حذف " + s + " انتخاب شده مطمئن هستید؟" + tip1 + tip2,
                    onBtn1Click: function () {
                        if ($scope.calling) return;
                        $scope.calling = true;
                        var strIds = "";
                        for (var j = 0; j < items.length; j++) {
                            strIds += items[j].ID + ",";
                        }

                        $.ajax({
                            type: "POST",
                            data: JSON.stringify(strIds),
                            url: "/app/api/Invoice/DeleteInvoices",
                            contentType: "application/json"
                        }).done(function (res) {

                            // var res = result.data;
                            $scope.calling = false;
                            if (res.resultCode === 1) {
                                alertbox({ content: res.data, type: "warning" });
                                return;
                            }
                            for (var i = 0; i < items.length; i++)
                                findAndRemove(grid._options.dataSource, items[i]);
                            grid.refresh();
                            grid.clearSelection();
                            var strMessage = items.length > 1 ? "فاکتورهای " : "فاکتور ";
                            strMessage += "انتخاب شده حذف " + (items.length > 1 ? "شدند " : "شد ");
                            DevExpress.ui.notify(strMessage, "success", 3000);
                            $scope.calculateInvoicesStatuses();
                            $scope.$apply();
                        }).fail(function (error) {
                            $scope.calling = false;
                            if ($scope.accessError(error)) { applyScope($scope); return; }
                            $scope.$apply();
                            alertbox({ content: error });
                        });
                    }
                });
            };
            $scope.returnInvoice = function () {
                var selectedItems = grid.getSelectedRowsData();
                if (selectedItems.length === 0) {
                    alertbox({ content: "هیچ فاکتوری جهت مرجوع کردن انتخاب نشده است" });
                    return;
                }
                if (selectedItems.length > 1) {
                    alertbox({ content: "برای مرجوع کردن فقط یک فاکتور را انتخاب کنید" });
                    return;
                }
                if (selectedItems[0].Returned) {
                    alertbox({ content: "این فاکتور قبلاً مرجوع شده است." });
                    return;
                }
                questionbox({
                    content: "آیا از مرجوع کردن فاکتور انتخاب شده مطمئن هستید؟",
                    onBtn1Click: function () {
                        if ($scope.type === 0)
                            $location.path("/editInvoice/saleReturnId=" + selectedItems[0].Id);
                        else if ($scope.type === 1)
                            $location.path("/editInvoice/purchaseReturnId=" + selectedItems[0].Id);
                        $scope.$apply();
                        return;
                    }
                });
            };
            $scope.markAs = function (status) {
                $scope.alert = false;
                var selectedItems = grid.getSelectedRowsData();
                if (selectedItems.length === 0) {
                    alertbox({ content: "هیچ فاکتوری انتخاب نشده است" });
                    return;
                }
                var s = selectedItems.length === 1 ? "فاکتور" : "فاکتورهای";
                for (var i = 0; i < selectedItems.length; i++) {
                    if (selectedItems[i].Status < 2) {
                        alertbox({ content: "یکی از فاکتورهای انتخاب شده تایید نشده است و امکان تغییر وضعیت آن وجود ندارد." });
                        return;
                    }
                }
                if ($scope.calling) return;
                $scope.calling = true;
                var strIds = "";
                for (var j = 0; j < selectedItems.length; j++)
                    strIds += selectedItems[j].Id + ",";
                var strIds = strIds.replace(/(^,)|(,$)/g, "");

                callws(DefaultUrl.MainWebService + 'MarkInvoicesAs', { invoiceIds: strIds, status: status })
                    .success(function () {
                        $scope.calling = false;
                        var ids = strIds.split(',');
                        for (var k = 0; k < ids.length; k++) {
                            var invFinded = findById(grid._options.dataSource, parseInt(ids[k]));
                            if (status === "paid") invFinded.Status = 3;
                            else if (status === "approved") invFinded.Status = 2;
                            else if (status === "sent") invFinded.Sent = true;
                        }

                        if (status === "paid") {
                            DevExpress.ui.notify(s + " انتخاب شده بعنوان " + ($scope.type === 0 ? "دریافت" : "پرداخت") +
                                " شده علامت گذاری شد" + (selectedItems.length > 1 ? "ند" : ""), "success", 3000);
                            //                    $scope.approvedStat -= invoices.length;
                            //                    $scope.tab = 4;
                            $scope.approvedStat -= selectedItems.length;
                            $('#tabs li:eq(4) a').tab('show');
                            $scope.selectTab(3);
                        }
                        else if (status === "approved") {
                            DevExpress.ui.notify(s + " انتخاب شده بعنوان منتظر " + ($scope.type === 0 ? "دریافت" : "پرداخت") +
                                " علامت گذاری شد" + (selectedItems.length > 1 ? "ند" : ""), "success", 3000);
                            //                    $scope.approvedStat += invoices.length;
                            //                    $scope.tab = 3;
                            $scope.approvedStat += selectedItems.length;
                            $('#tabs li:eq(3) a').tab('show');
                            $scope.selectTab(2);
                        }
                        else if (status === "sent")
                            DevExpress.ui.notify(s + " انتخاب شده بعنوان ارسال شده علامت گذاری شد" + (selectedItems.length > 1 ? "ند" : ""), "success", 3000);
                        grid.refresh();
                        grid.clearSelection();
                        $scope.calculateInvoicesStatuses();

                        //            	$scope.grid.selectNone();
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) { applyScope($scope); return; }
                        $scope.alert = true;
                        $scope.alertType = "danger";
                        $scope.alertMessage = error;
                        applyScope($scope);
                    }).loginFail(function () {
                        $scope.calling = false;
                        if (Hesabfa.user) $scope.inactivityLogout();
                        else window.location = DefaultUrl.login;
                    });
            };
            $scope.selectTab = function (tab) {
                switch (tab) {
                    case null:
                        grid.clearFilter();
                        break;
                    case 0:
                        grid.filter(["Status", "=", 0]);
                        break;
                    case 1:
                        grid.filter(["Status", "=", 1]);
                        break;
                    case 2:
                        grid.filter(["Status", "=", 2]);
                        break;
                    case 3:
                        grid.filter(["Status", "=", 3]);
                        break;
                    default:
                        grid.clearFilter();
                        break;
                }
                $scope.tab = tab;
                applyScope($scope);
            };
            $scope.calculateInvoicesStatuses = function () {
                var invoices = grid._options.dataSource;
                var len = invoices.length;
                var draft = 0, awaitingApproval = 0, approved = 0;
                for (var i = 0; i < len; i++) {
                    if (invoices[i].Status === 0) draft++;
                    else if (invoices[i].Status === 1) awaitingApproval++;
                    else if (invoices[i].Status === 2) approved++;
                }
                $scope.draftStat = draft;
                $scope.awaitingApprovalStat = awaitingApproval;
                $scope.approvedStat = approved;
            };
            $scope.printInvoiceList = function () {
                grid.print();
            };
            $scope.pdfInvoiceList = function () {
                grid.pdf();
            };
            $scope.printAddressLabelDialog = function () {
                var gridSelectedItems = grid.getSelectedRowsData();

                if (gridSelectedItems.length === 0) {
                    alertbox({ content: "هیچ فاکتوری انتخاب نشده است" });
                    return;
                }
                messagebox({
                    title: "چاپ برچسب آدرس",
                    buttonCount: 2,
                    btn1Title: "چاپ", btn1Class: "btn btn-sm btn-primary",
                    btn2Title: "انصراف", btn2Class: "btn btn-sm btn-default btn-smoke",
                    content: "<div>چاپ برچسب آدرس جهت بسته پستی</div>" +
                    "<div class='row'>" +
                    "<div class='col-sm-12'>" +
                    "<label><input type='checkbox' id='contactAddress' checked disabled> آدرس گیرنده</label><br/>" +
                    "<label><input type='checkbox' id='businessAddress'> آدرس فرستنده</label><br/>" +
                    "<label><input type='checkbox' id='invoiceNumber' checked> شماره فاکتور</label><br/>" +
                    "<label><input type='checkbox' id='invoiceNote'> افزودن توضیحات فاکتور به برچسب</label>" +
                    "</div>" +
                    "</div><hr>" +
                    "<div class='radio'><label><input type='radio' name='addressLabelRadios' id='addressLabelPlan1'>" +
                    "طرح یک (برگ A4، یک ردیفه)" + "</label></div>" +
                    "<div class='radio'><label><input type='radio' name='addressLabelRadios' id='addressLabelPlan2' checked>" +
                    "طرح دو (برگ A4، دو ردیفه)" + "</label></div>",
                    onBtn1Click: function () {
                        var contactAddress = $('#contactAddress').is(':checked');
                        var businessAddress = $('#businessAddress').is(':checked');
                        //                var businessLogo = $('#businessLogo').is(':checked');
                        var invoiceNumber = $('#invoiceNumber').is(':checked');
                        var invoiceNote = $('#invoiceNote').is(':checked');
                        //                var sendDate = $('#sendDate').is(':checked');
                        var addressLabelPlan1 = $('#addressLabelPlan1').is(':checked');
                        var addressLabelPlan2 = $('#addressLabelPlan2').is(':checked');

                        var ids = [];
                        for (var i = 0; i < gridSelectedItems.length; i++) {
                            ids.push(gridSelectedItems[i].Id);
                        }

                        callws(DefaultUrl.MainWebService + 'GetInvoicesData', { ids: ids })
                            .success(function (data) {

                                $scope.callingImport = false;
                                var options = {};
                                options.date = $scope.todayDate;
                                options.contactAddress = contactAddress;
                                options.businessAddress = businessAddress;
                                options.invoiceNumber = invoiceNumber;
                                options.invoiceNote = invoiceNote;
                                if (addressLabelPlan1) options.plan = 1;
                                if (addressLabelPlan2) options.plan = 2;
                                $scope.printAddressLabel(data, options);

                            }).fail(function (error) {
                                $scope.callingImport = false;
                                applyScope($scope);
                                if ($scope.accessError(error)) return;
                                alertbox({ content: error });
                            }).loginFail(function () {
                                window.location = DefaultUrl.login;
                            });
                    }
                });
                $('#barcodeTextArea').val("");
            };
            $scope.printAddressLabel = function (list, options) {
                $("#printBtns").hide();
                $("#progressBarDiv").show();
                $scope.$apply();
                setTimeout(function () {
                    pdfAddressLabel(list, options, $rootScope.currentBusiness);
                }, 100);
            };
            $scope.printInvoices = function () {
                var gridSelectedItems = grid.getSelectedRowsData();
                if (gridSelectedItems.length === 0) {
                    alertbox({ content: "ابتدا یک یا چند فاکتور را انتخاب کنید." });
                    return;
                }
                $("#printBtns").hide();
                $("#progressBarDiv").show();
                applyScope($scope);

                var listCount = gridSelectedItems.length;
                var pdf;
                var i = 0;
                (function loop() {
                    callws(DefaultUrl.MainWebService + 'Load_Invoice_TransObj', { id: gridSelectedItems[i].Id })
                        .success(function (result) {
                            $scope.invoice = result.invoice;
                            $scope.invoice.InvoiceItems = result.invoiceItems;
                            $scope.transObj = result.transactionObj;
                            $scope.receiveDate = $scope.todayDate;
                            $scope.payDate = $scope.todayDate;
                            $scope.payments = result.payments;
                            $scope.invoiceSettings = result.invoiceSettings;
                            var l = $scope.invoice.InvoiceItems.length; // محاسبه مالیات و تخفیف کل فاکتور جهت نمایش
                            $scope.totalDiscount = 0;
                            $scope.totalTax = 0;
                            for (var j = 0; j < l; j++) {
                                $scope.totalDiscount += $scope.invoice.InvoiceItems[j].Discount;
                                $scope.totalTax += $scope.invoice.InvoiceItems[j].Tax;
                            }

                            $("#imgLogo").attr("src", $scope.invoiceSettings.businessLogo);
                            $scope.$apply();

                            var progressValue = Math.floor(((i + 1) * 100) / listCount);
                            pdf = generateInvoicePDF($scope.invoice, $scope.invoiceSettings, $scope.totalDiscount, $scope.totalTax, null, null, $scope.payments, true, pdf, $rootScope.currentBusiness, $scope.getCurrency());

                            $('#progressPdf').attr('aria-valuenow', progressValue).css('width', progressValue + "%");
                            $("#progressPdfTitle").text("در حال تهیه PDF... فاکتور " + Hesabfa.farsiDigit(i + 1) + " از " + Hesabfa.farsiDigit(listCount) + " (" + Hesabfa.farsiDigit(progressValue) + '%)');
                            i++;
                            if (i < listCount) setTimeout(loop, 0);
                            else {
                                pdf.save("Invoices.pdf");
                                setTimeout(function () {
                                    $("#progressBarDiv").hide();
                                    $("#printBtns").show();
                                    $('#progressPdf').attr('aria-valuenow', 1).css('width', 1 + "%");
                                    $("#progressPdfTitle").text("در حال تهیه PDF...");
                                }, 1000);
                            }
                        })
                        .fail(function (error) {
                            $scope.loading = false;
                            applyScope($scope);
                            if ($scope.accessError(error)) return;
                            alertbox({ content: error, title: "خطا" });
                            return;
                        })
                        .loginFail(function () {
                            window.location = DefaultUrl.login;
                        });
                })();
            };
            $scope.exportInvoicesToExcel = function () {
                if ($scope.callingImport) return;
                $scope.callingImport = true;
                $('#loadingModal').modal('show');
                callws(DefaultUrl.MainWebService + 'ExportInvoicesToExcel', { invoiceType: $scope.type })
                    .success(function (data) {
                        $scope.callingImport = false;
                        $('#loadingModal').modal('hide');
                        applyScope($scope);
                        var fileName = "";
                        if ($scope.type === 0) fileName = "SaleInvoices";
                        else if ($scope.type === 1) fileName = "PurchaseInvoices";
                        else if ($scope.type === 2) fileName = "SaleReturns";
                        else if ($scope.type === 3) fileName = "PurchaseReturns";
                        else if ($scope.type === 4) fileName = "WasteInvoices";

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

            $scope.redirectToEditInvoice = function (id) {
                $state.go("newContract", { id: id });

            }

            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);



        }])
});