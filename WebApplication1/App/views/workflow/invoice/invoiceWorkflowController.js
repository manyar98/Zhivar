define(['application', 'dataService', 'combo', 'scrollbar', 'helper', 'editItem', 'editContact', 'nodeSelect', 'dx', 'digitGroup', 'number', 'combo', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker', 'maskedinput',  'preApp'], function (app) {
    app.register.controller('invoiceWorkflowController', ['$scope', '$stateParams', '$location', '$state', 'dataService',
        function ($scope, $stateParams, $location, $state, dataService) {

            $scope.operationAccess = dataService.getOperationAccess("invoice");

            var invoiceDueDateObj = new AMIB.persianCalendar('invoiceDueDate');
            var invoiceDateObj = new AMIB.persianCalendar('invoiceDate');
            $scope.automaticNumber = true;

            $("#automaticNumberSwitch").dxSwitch({
                onText: 'اتوماتیک',
                offText: 'دستی',
                width: 80,
                onValueChanged: function (data) {

                    $scope.automaticNumber = data.value;
                    //$scope.calculateInvoice(true);
                }

            });

            $("#taxSwitch").dxSwitch({
                onText: 'محاسبه مالیات',
                offText: 'بدون مالیات',
                width: 110,
                value: true,
                onValueChanged: function (data) {
                    $scope.autoCalTax = data.value;
                    $scope.calculateInvoice(true);
                }

            });


            $scope.taxSwitchChecked = function (data) {

                $scope.autoCalTax = data;
                $scope.calculateInvoice(true);
            }

            $scope.init = function () {
                $scope.alert = false;
                $scope.alertMessage = "";

                $scope.loadInvoiceData($stateParams.id);

            };


            $scope.loadInvoiceData = function (id) {
                $scope.loading = true;

                // var result = testApi();

                //  callws("/app/api/FactorApi/loadInvoiceData", { id: id })
                //   .success(function (result) {

                $(function () {
                    //var id = 0;

                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(id),
                        url: "/app/api/Invoice/loadInvoiceData",
                        contentType: "application/json"
                    }).done(function (res) {
                        //return res;
                        var result = res.data;
                        var moveItemEdit = false;
                        $scope.invoiceSettings = result.invoiceSettings;
                        $scope.footerNote = result.invoiceSettings.footerNote;
                        $scope.autoCalTax = $scope.invoiceSettings.autoAddTax;
                        applyScope($scope);

                        if ($scope.saleReturnId)
                            $scope.getInvoice4SaleReturn($scope.saleReturnId);
                        else if ($scope.purchaseReturnId)
                            $scope.getInvoice4PurchaseReturn($scope.purchaseReturnId);
                        else if ($scope.saleCopyId)
                            $scope.getInvoice4Copy($scope.saleCopyId);
                        else if ($scope.purchaseCopyId)
                            $scope.getInvoice4Copy($scope.purchaseCopyId);
                        else {
                            $scope.loading = false;
                            moveItemEdit = true;
                        }
                        $scope.invoice = result.invoice;
                        $scope.contacts = result.contacts;
                        //$scope.comboContact.items = result.contacts;
                        //$scope.items = result.items;
                        //$scope.comboItem.items = result.items;
                        //$scope.itemNodes = result.itemNodes;
                        //$scope.invoiceItem = $scope.Hesabfa.newInvoiceItemObj();
                        //$scope.item = $scope.Hesabfa.newItemObj();
                        //$scope.contact = $scope.Hesabfa.newContactObj();
                        //$scope.currentInvoiceStatus = $scope.invoice.Status;

                        //$scope.sms = result.defaultSms;
                        //$scope.showNote = $scope.invoice.Note !== "" ? true : false;

                        $("#imgLogo").attr("src", $scope.invoiceSettings.businessLogo);
                       // var nextInvoiceNumber;
                        //if ($stateParams.id === "new" || $scope.saleCopyId) {
                        //    nextInvoiceNumber = result.invoiceNextNumber;
                        //    if (nextInvoiceNumber)
                        //        $scope.invoice.Number = nextInvoiceNumber;
                        //}
                        //else if ($stateParams.id === "newBill" || $scope.purchaseCopyId) $scope.invoice.InvoiceType = 1;
                        //else if ($stateParams.id === "saleReturn" || $scope.saleReturnId) $scope.invoice.InvoiceType = 2;
                        //else if ($stateParams.id === "purchaseReturn" || $scope.purchaseReturnId) $scope.invoice.InvoiceType = 3;
                        //else if ($stateParams.id === "newWaste" || $scope.purchaseReturnId) {
                        //    $scope.invoice.InvoiceType = 4;
                        //    var products = [];
                        //    var length = $scope.items.length;
                        //    for (var i = 0; i < length; i++) {
                        //        if ($scope.items[i].ItemType === 0)
                        //            products.push($scope.items[i]);
                        //    }
                        //    $scope.items = products;
                        //    $scope.comboItem.items = $scope.items;
                        //}
                        //else if ($stateParams.id.indexOf("new_contactId") > -1) {
                        //	nextInvoiceNumber = result.invoiceNextNumber;
                        //	if (nextInvoiceNumber) $scope.invoice.Number = nextInvoiceNumber;
                        //	$scope.invoice.Contact = findById($scope.contacts, $scope.contactId);
                        //	$scope.contactInput = $scope.invoice.Contact.Name;
                        //}
                        //else if ($stateParams.id.indexOf("newBill_contactId") > -1) {
                        //	$scope.invoice.InvoiceType = 1;
                        //	$scope.invoice.Contact = findById($scope.contacts, $scope.contactId);
                        //	$scope.contactInput = $scope.invoice.Contact.Name;
                        //}

                        //if ($scope.invoice.ID > 0 && $scope.invoice.Contact)
                        //    $scope.comboContact.setSelected($scope.invoice.Contact);

                        //if ($scope.invoice.Status === 0) {
                        //    var settingAutoSave = $rootScope.getUISetting("editInvoiceAutoSave");
                        //    if (settingAutoSave && settingAutoSave === "true") {
                        //        $scope.setAutoSave(true);
                        //        $scope.autoSaveInvoice = true;
                        //    }
                        //}

                        //if ($scope.invoice.InvoiceType === 4)
                        //    $scope.autoCalTax = false;

                        //$scope.setEditInvoicePageTitle();
                        //applyScope($scope);
                        //$scope.calculateInvoice();
                        //angular.element(document).ready(function () {
                        //    if (moveItemEdit) $scope.moveRowEditor(0);
                        //});

                        //}).fail(function (error) {
                        //	$scope.loading = false;
                        //	applyScope($scope);
                        //	if ($scope.accessError(error)) return;
                        //	alertbox({ content: error, title: "خطا" });
                        //}).loginFail(function () {
                        //	window.location = DefaultUrl.login;
                        //});
                        // Do something with the result :)
                    });
                })

            };
      

            $scope.calculateInvoice = function (calTax) {
                if (!$scope.invoice) return;
                $scope.invoice.Sum = 0;
                $scope.invoice.Payable = 0;
                $scope.totalTax = 0;
                $scope.totalDiscount = 0;
                $scope.totalQuantity = 0;
                var length = $scope.invoice.InvoiceItems.length;
                var taxRate = $rootScope.currentBusiness.TaxRate;
                for (var i = 0; i < length; i++) {
                    var invoiceItem = $scope.invoice.InvoiceItems[i];
                    invoiceItem.ItemInput = invoiceItem.Item ? invoiceItem.Item.Name : ""; // new
                    invoiceItem.Quantity = parseFloat((invoiceItem.Quantity + "").replace("/", "."));
                    invoiceItem.Sum = invoiceItem.UnitPrice * invoiceItem.Quantity;
                    invoiceItem.Sum = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Sum * 100) / 100 : Math.round(invoiceItem.Sum);
                    $scope.totalQuantity += invoiceItem.Quantity;

                    var strDiscount = invoiceItem.Discount.toString();
                    if (strDiscount.charAt(strDiscount.length - 1) === '%') {
                        strDiscount = strDiscount.substr(0, strDiscount.length - 1);
                        invoiceItem.Discount = invoiceItem.Sum * parseInt(strDiscount) / 100;
                    }
                    invoiceItem.Discount = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Discount * 100) / 100 : Math.round(invoiceItem.Discount);

                    if (calTax) {
                        if ($scope.invoice.InvoiceType === 4)
                            invoiceItem.Tax = 0;
                        else if ($scope.autoCalTax) {
                            invoiceItem.Tax = (invoiceItem.Sum - invoiceItem.Discount) * (taxRate / 100);
                            invoiceItem.Tax = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Tax * 100) / 100 : Math.round(invoiceItem.Tax);
                        } else {
                            invoiceItem.Tax = 0;
                        }
                        invoiceItem.calcTax = false;
                    } else if ($scope.autoCalTax && invoiceItem.calcTax) {
                        invoiceItem.Tax = (invoiceItem.Sum - invoiceItem.Discount) * (taxRate / 100);
                        invoiceItem.Tax = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Tax * 100) / 100 : Math.round(invoiceItem.Tax);
                        invoiceItem.calcTax = false;
                    }

                    //			}
                    //				if ($scope.autoCalTax) {
                    //				if ($scope.autoCalTax) {
                    //					if (invoiceItem.calcTax) {
                    //						invoiceItem.Tax = (invoiceItem.Sum - invoiceItem.Discount) * (taxRate / 100);
                    //						invoiceItem.Tax = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Tax * 100) / 100 : Math.round(invoiceItem.Tax);
                    //						invoiceItem.calcTax = false;
                    //					}
                    //				} else {
                    //				}
                    //			}

                    invoiceItem.TotalAmount = invoiceItem.Sum - parseFloat(invoiceItem.Discount + "") + parseFloat(invoiceItem.Tax + "");
                    $scope.invoice.Sum += invoiceItem.Sum;
                    $scope.invoice.Payable += invoiceItem.TotalAmount;

                    $scope.totalTax += parseFloat(invoiceItem.Tax + "");
                    $scope.totalDiscount += parseFloat(invoiceItem.Discount + "");
                }
                $scope.invoice.Payable = $rootScope.isDecimalCurrency() ? Math.round($scope.invoice.Payable * 100) / 100 : Math.round($scope.invoice.Payable);
                $scope.invoice.Rest = $scope.invoice.Payable - $scope.invoice.Paid;
                applyScope($scope);
            };
  


            $scope.init();
   

        }])
}); 