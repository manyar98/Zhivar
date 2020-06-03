define(['application', 'combo', 'scrollbar', 'dx', 'roweditor', 'helper', 'gridHelper', 'editBank', 'editCash', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker', 'number'], function (app) {
    app.register.controller('viewSazeOfContractController', ['$scope','$rootScope', '$stateParams', '$location', '$compile', '$state',
        function ($scope, $rootScope, $stateParams, $location, $compile, $state) {

            var payDateObj = new AMIB.persianCalendar('payDate');
            var receiveDateObj = new AMIB.persianCalendar('receiveDate');

            $scope.init = function () {
                $rootScope.pageTitle("بارگیری فاکتور...");
                $('#businessNav').show();

                var status = $stateParams.status; // $scope.getRouteQuery($routeParams.params, 'status');
                var smsSent = $stateParams.sms; //$scope.getRouteQuery($routeParams.params, 'sms');
                var id = $stateParams.id;// $scope.getRouteQuery($routeParams.params, 'id');
                $scope.submisionStatus = status;
                $scope.smsSent = smsSent === "0" ? false : true;

                $scope.alert = false;
                var rp = Hesabfa.newReceiveAndPay();
                rp.FinanYear = $scope.currentFinanYear;
                rp.DisplayDate = $scope.todayDate;
                rp.IsReceive = true; // depend on invoice type
                rp.Type = 1;
                $scope.rp = rp;
                $scope.total = 0;

                $scope.loadInvoice(id);


                var sendInvoiceSms = $rootScope.getUISetting("sendInvoiceSms2");
                $scope.sendSmsByReceive = !!sendInvoiceSms;

                applyScope($scope);
                $(function () {
                    $.getScript("/App/printReports.js", function () { });
                });
            };
            $scope.showSuccessSubmision = function (invoice, docNumber) {
                if ($scope.submisionStatus && $scope.submisionStatus === 'approved') {
                    $scope.alert = true;
                    var contactStr = invoice.Contact ? invoice.Contact.Name : "";
                    if (invoice.InvoiceType === 4) contactStr = "ضایعات کالا";
                    $scope.alertMessage = "فاکتور {2} تایید شد - {0} - مبلغ کل: {1}".
                        formatString(contactStr, $scope.money(invoice.Payable), invoice.Number, invoice.InvoiceType === 0 ? "فروش" : "خرید");
                    if (docNumber) $scope.alertMessage += " - شماره سند: " + docNumber + ".";
                    if ($scope.smsSent) $scope.alertMessage += " [پیامک ارسال شد]";
                    $scope.alertType = 'success';
                    $scope.$apply();
                }
            };
            $scope.loadInvoice = function (id, sendSms) {
                $scope.loading = true;
                $.ajax({
                    type: "POST",
                    data: JSON.stringify(id),
                    url: "/app/api/Contract/LoadContractSazeTransObj",
                    contentType: "application/json"
                }).done(function (res) {
                    var result = res.data;
                    $scope.loading = false;
                    $scope.Contract_Saze = result.Contract_Saze;
                    $scope.Contract_Saze.Contarct_Saze_Bazareabs = result.Contarct_Saze_Bazareabs;
                    $scope.Contract_Saze.Contract_Saze_Tarahs = result.Contract_Saze_Tarahs;
                    $scope.Contract_Saze.Contract_Saze_Chapkhanes = result.Contract_Saze_Chapkhanes;
                    $scope.Contract_Saze.Contract_Saze_Nasabs = result.Contract_Saze_Nasabs;
                   // $scope.receiveDate = $scope.todayDate;
                   // $scope.payDate = $scope.todayDate;
                   // $scope.payments = result.payments;
                   // $scope.invoiceSettings = result.invoiceSettings;
                   // $scope.invoiceSettingsCopy = {};
                   // angular.copy(result.invoiceSettings, $scope.invoiceSettingsCopy);
                   // $scope.rp.IsReceive = result.invoice.InvoiceType === 0 || result.invoice.InvoiceType === 3;
                   // $scope.rp.Invoice = result.invoice;

                   // $scope.showSuccessSubmision($scope.invoice, result.docNumber);

                   // $scope.cashes = result.cashes;
                   // $scope.banks = result.banks;
                   // if ($scope.cashId) $scope.addCashPay();
                   // if ($scope.bankId) $scope.addBankPay();

                   // var l = $scope.invoice.InvoiceItems.length; // محاسبه مالیات و تخفیف کل فاکتور جهت نمایش
                   // $scope.totalDiscount = 0;
                   // $scope.totalTax = 0;
                   // for (var i = 0; i < l; i++) {
                   //     $scope.totalDiscount += $scope.invoice.InvoiceItems[i].Discount;
                   //     $scope.totalTax += $scope.invoice.InvoiceItems[i].Tax;
                   // }

                  

                    //if (sendSms) $scope.sendSms();

                    //var print = $stateParams.print;
                    //if (print && print === 'true')
                    //    $scope.generatePDF();

                    //$scope.setViewInvoicePageTitle();
                    //applyScope($scope);
                }).fail(function (error) {
                    $scope.loading = false;
                    $scope.invoice = null;
                    applyScope($scope);
                    if ($scope.accessError(error)) return;
                    alertbox({ content: error, title: "خطا", type: "error" });
                });
            };
            $scope.accountChange = function () {
                var value = $scope.paidToAccount;
                if (value == "") {
                    $scope.filteredAccounts = $scope.accounts;
                    return;
                }
                $("#accountSelect").show();
                var searchKey = !isNaN(parseInt(value, 10)) ? 'Code' : 'Name';
                $scope.filteredAccounts = filterData($scope.accounts, searchKey, value);
                $scope.activeAccountIndex = 0;
                $scope.$apply();
            };
            $scope.showAccountList = function () {
                var value = $scope.paidToAccount;
                if (value == "" || !value)
                    $scope.filteredAccounts = $scope.accounts;
                setTimeout(function () {
                    $("#accountSelect").show();
                }, 200);
            };
           
 
            $scope.setViewInvoicePageTitle = function () {
                if ($scope.invoice.InvoiceType === 0) $rootScope.pageTitle("فاکتور" + ($scope.invoice.Contact ? " " + $scope.invoice.Contact.Name : "") + " - شماره فاکتور: " + $scope.invoice.Number);
                if ($scope.invoice.InvoiceType === 1) $rootScope.pageTitle("صورتحساب" + ($scope.invoice.Contact ? " " + $scope.invoice.Contact.Name : ""));
                if ($scope.invoice.InvoiceType === 2) $rootScope.pageTitle("برگشت از فروش" + ($scope.invoice.Contact ? " " + $scope.invoice.Contact.Name : ""));
                if ($scope.invoice.InvoiceType === 3) $rootScope.pageTitle("برگشت از خرید" + ($scope.invoice.Contact ? " " + $scope.invoice.Contact.Name : ""));
            };

         
            $scope.getRemindedMoney = function () {
                var currentMoneyInTransactions = 0;
                if ($scope.rp.Items && $scope.rp.Items.length > 0) {
                    var l = $scope.rp.Items.length;
                    for (var i = 0; i < l; i++)
                        currentMoneyInTransactions += parseFloat($scope.rp.Items[i].Amount);
                }
                var rest = $scope.invoice.Rest - currentMoneyInTransactions;
                return rest >= 0 ? rest : 0;
            };
        
    
   
            $scope.deleteRow = function (index) {
                $scope.rp.Items.splice(index, 1);
                $scope.calTotal();
                applyScope($scope);
            };
            $scope.calTotal = function () {
                var transactions = $scope.rp.Items;
                var total = 0;
                for (var i = 0; i < transactions.length; i++) {
                    total += parseFloat(transactions[i].Amount);
                }
                $scope.total = total;
                applyScope($scope);
            };
            $scope.asignChequeDatesAndContact = function () {
                var transactions = $scope.rp.Items;
                for (var i = 0; i < transactions.length; i++) {
                    if (transactions[i].Type === 3) {
                        transactions[i].Cheque.Contact = $scope.invoice.Contact;
                        transactions[i].Cheque.Amount = transactions[i].Amount;
                    }
                }
            };
            $scope.printPDF = function () {
                window.location = '/View/InvoicePrintPDF.ashx?id=' + $scope.invoice.Id;
            };

            $scope.print = function (reset) {
                if (reset)

                    $scope.invoiceSettings = {
                        allowApproveWithoutStock: false,
                        autoAddTax: true,
                        bottomMargin: "20",
                        businessLogo: "",
                        font: "Iransans",
                        fontSize: "Medium",
                        footerNote: "",
                        footerNoteDraft: "",
                        hideZeroItems: false,
                        onlineInvoiceEnabled: false,
                        pageSize: "A4portrait",
                        payReceiptTitle: "رسید پرداخت وجه / چک",
                        purchaseInvoiceTitle: "فاکتور خرید",
                        receiveReceiptTitle: "رسید دریافت وجه / چک",
                        rowPerPage: "18",
                        saleDraftInvoiceTitle: "پیش فاکتور",
                        saleInvoiceTitle: "صورتحساب فروش کالا و خدمات",
                        showAmountInWords: false,
                        showCustomerBalance: false,
                        showItemUnit: false,
                        showSignaturePlace: true,
                        showTransactions: true,
                        showVendorInfo: true,
                        topMargin: "10",
                        updateBuyPrice: false,
                        updateSellPrice: false,

                    };
                // angular.copy($scope.invoiceSettingsCopy, $scope.invoiceSettings);
                //callws(DefaultUrl.MainWebService + 'GetBusinessFullInfo', {})
                //success(function (business) {
                var settings = jQuery.extend(true, {}, $scope.invoiceSettings);
               // settings.footerNote = $scope.invoice.Note + "\n" + settings.footerNote;
             //   settings.footerNoteDraft = $scope.invoice.Note + "\n" + settings.footerNoteDraft;

                var business = {
                    Name: "ژیور",
                    LegalName: "ژیور",
                    Address: "",
                    PostalCode: "",
                    Fax: ""
                };
                printInvoice($scope.invoice, settings, $scope.totalDiscount, $scope.totalTax, $scope.payments, business, $scope.getCurrency());
                //})
                //.fail(function (error) {
                //    if ($scope.accessError(error)) return;
                //    alertbox({ content: error });
                //})
                //.loginFail(function () {
                //    window.location = DefaultUrl.login;
                //});
            };
            $scope.generatePDF = function (asString, doAfterRead) {
                // callws(DefaultUrl.MainWebService + 'GetBusinessFullInfo', {})
                //.success(function (business) {
                $scope.invoiceSettings = {
                    allowApproveWithoutStock: false,
                    autoAddTax: true,
                    bottomMargin: "20",
                    businessLogo: "",
                    font: "Iransans",
                    fontSize: "Medium",
                    footerNote: "",
                    footerNoteDraft: "",
                    hideZeroItems: false,
                    onlineInvoiceEnabled: false,
                    pageSize: "A4portrait",
                    payReceiptTitle: "رسید پرداخت وجه / چک",
                    purchaseInvoiceTitle: "فاکتور خرید",
                    receiveReceiptTitle: "رسید دریافت وجه / چک",
                    rowPerPage: "18",
                    saleDraftInvoiceTitle: "پیش فاکتور",
                    saleInvoiceTitle: "صورتحساب فروش کالا و خدمات",
                    showAmountInWords: false,
                    showCustomerBalance: false,
                    showItemUnit: false,
                    showSignaturePlace: true,
                    showTransactions: true,
                    showVendorInfo: true,
                    topMargin: "10",
                    updateBuyPrice: false,
                    updateSellPrice: false,

                };

                var business = {
                    Name: "ژیور",
                    LegalName: "ژیور",
                    Address: "",
                    PostalCode: "",
                    Fax: ""
                };
                var settings = jQuery.extend(true, {}, $scope.invoiceSettings);
               // settings.footerNote = $scope.invoice.Note + "\n" + settings.footerNote;
              //  settings.footerNoteDraft = $scope.invoice.Note + "\n" + settings.footerNoteDraft;

                generateInvoicePDF($scope.invoice, settings, $scope.totalDiscount, $scope.totalTax, asString, doAfterRead, $scope.payments, null, null, business, $scope.getCurrency());
                //})
                //.fail(function (error) {
                //    if ($scope.accessError(error)) return;
                //    alertbox({ content: error });
                //})
                //.loginFail(function () {
                //    window.location = DefaultUrl.login;
                //});
            };
            $scope.printInvoiceDialog = function () {
                $('#modalPrintInvoiceBySettings').modal('show');
            };
            $scope.printStorehouseOrder = function () {
                $scope.invoiceSettings.saleInvoiceTitle = "حواله انبار";
                $scope.invoiceSettings.hidePrices = true;
                $scope.print();
            };
            $scope.printStorehouseOrderPDF = function () {
                $scope.invoiceSettings.saleInvoiceTitle = "حواله انبار";
                $scope.invoiceSettings.hidePrices = true;
                $scope.generatePDF();
            };
            $scope.gatherReceiptInfo = function () {
                var i = {};
                $scope.asignChequeDatesAndContact();
                i.date = $('#payDate').val();
                var newTranses = [];
                angular.copy($scope.transactions, newTranses);
                i.transactions = newTranses;
                i.totalStr = "مبلغ کل: " + $scope.money($scope.total) + " " + $scope.getCurrency() + " (" + wordifyfa($scope.total, 0) + " " + $scope.getCurrency() + ") ";
                $scope.receiptInfo = i;
            };
            $scope.printReceiptDialog = function () {
                $scope.gatherReceiptInfo();
                var IsReceive = $scope.invoice.InvoiceType === 0 || $scope.invoice.InvoiceType === 2 ? true : false;
                var strName = $scope.invoice.ContactTitle ? $scope.invoice.ContactTitle : ($scope.invoice.Contact ? $scope.invoice.Contact.Name : "");
                var strForWhat = "بابت فاکتور شماره " + $scope.invoice.Number;
                $scope.receiptText = "مبلغ " + $scope.money($scope.total) + " " + $scope.getCurrency();
                $scope.receiptText += " (" + wordifyfa($scope.total, 0) + " " + $scope.getCurrency() + ") ";
                $scope.receiptText += (IsReceive ? "از " : "به ") + "آقای/خانم " + strName;
                $scope.receiptText += " " + strForWhat + " به صورت نقد/چک " + (IsReceive ? "دریافت گردید." : "پرداخت گردید.");
                $scope.receiptTitle = IsReceive ? "رسید دریافت وجه/چک" : "رسید پرداخت وجه/چک";
                $scope.receiptText = $rootScope.farsiDigit($scope.receiptText);
                $scope.addTransactionListToReceipt = true;
                //$scope.$apply();
                $('#modalPrintRecipt').modal('show');
            };
            $scope.printReceipt = function () {
                applyScope($scope);
                $('#modalPrintRecipt').modal('hide');
                var info = {};
                info.text = $scope.receiptText;
                info.note = $scope.receiptNote;
                info.title = $rootScope.currentBusiness.Name;
                info.subTitle = $scope.receiptTitle;
                info.docNumber = 0;
                info.date = $scope.receiptInfo.date;
                info.showTransactions = $('#addTransactionListToReceipt').is(':checked');
                var pageSize = "A5Landscape";
                if ($('#radioA5Landscape').is(':checked')) pageSize = "A5Landscape";
                else if ($('#radioA4Portrait').is(':checked')) pageSize = "A4Portrait";
                info.settings = { pageSize: pageSize };
                info.transactions = $scope.receiptInfo.transactions;
                info.totalStr = $scope.receiptInfo.totalStr;
                info.moneyUnit = $scope.getCurrency();
                printReceipt(info);
            };
            $scope.generateReceiptPDF = function () {
                $scope.$apply();
                $('#modalPrintRecipt').modal('hide');
                var info = {};
                info.text = $scope.receiptText;
                info.note = $scope.receiptNote;
                info.title = $rootScope.currentBusiness.Name;
                info.subTitle = $scope.receiptTitle;
                info.docNumber = 0;
                info.date = $scope.receiptInfo.date;
                info.showTransactions = $('#addTransactionListToReceipt').is(':checked');
                var pageSize = "A5Landscape";
                if ($('#radioA5Landscape').is(':checked')) pageSize = "A5Landscape";
                else if ($('#radioA4Portrait').is(':checked')) pageSize = "A4Portrait";
                info.settings = { pageSize: pageSize };
                info.transactions = $scope.receiptInfo.transactions;
                info.totalStr = $scope.receiptInfo.totalStr;
                info.moneyUnit = $scope.getCurrency();
                pdfReceipt(info);
            };
            $scope.setSendSms = function () {
                $scope.sendSmsByReceive = !$scope.sendSmsByReceive ? true : false;
                if ($scope.sendSmsByReceive)
                    $rootScope.setUISetting("sendInvoiceSms2", "true");
                else
                    $rootScope.setUISetting("sendInvoiceSms2", "false");
            };
            $scope.openSmsEditor = function () {
                callws(DefaultUrl.MainWebService + 'GetSetting', { settingName: "SMS_DefaultInvoiceSMS2" })
                    .success(function (settingValue) {
                        $scope.sms = settingValue;
                        $scope.sendSmsByReceive = !$scope.sendSmsByReceive ? true : false;
                        $scope.editSmsModal = true;
                        applyScope($scope);
                    })
                    .fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) { applyScope($scope); return; }
                        $scope.alert = true;
                        $scope.alertType = "danger";
                        $scope.alertMessage = error;
                        applyScope($scope);
                    })
                    .loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
            $scope.getEditedSms = function (sms) {
                callws(DefaultUrl.MainWebService + "SaveSetting", { name: "SMS_DefaultInvoiceSMS2", value: sms })
                    .success(function () {
                        return;
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error });
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
            $scope.sendSms = function () {
                callws(DefaultUrl.MainWebService + 'SendSmsBySetting', {
                    settingName: "SMS_DefaultInvoiceSMS2", invoice: $scope.invoice
                })
                    .success(function (result) {
                        if (result.smsSent) $scope.alertMessage += " (پیامک ارسال شد)";
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: "خطا در هنگام ارسال پیامک" });
                        $scope.$apply();
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };

            $scope.openSendInvoiceByEmailDialog = function () {
                $scope.toEmail = $scope.invoice.Contact.ContactEmail ? $scope.invoice.Contact.ContactEmail + "" : $scope.invoice.Contact.Email + "";
                $scope.emailSubject = "فاکتور شماره " + $scope.invoice.Number + " از طرف "
                    + $rootScope.currentBusiness.Name + " برای " + $scope.invoice.Contact.Name;
                $scope.emailText = "سلام " + $scope.invoice.Contact.Name + "\n";
                $scope.emailText += "فاکتور صادر شده برای شما به شماره " + $scope.invoice.Number + " و مبلغ "
                    + $scope.money($scope.invoice.Payable) + " " + $scope.getCurrency() + " را از ضمیمه ایمیل دانلود کنید.";
                $scope.emailText += "\n\n" + "تاریخ سررسید این فاکتور " + $scope.invoice.DisplayDueDate + " می باشد.";
                $scope.emailText += "\n\n" + "با تشکر" + "\n" + $rootScope.currentBusiness.Name;
                $scope.includePdfAttachment = true;
                $scope.markAsSent = true;
                $scope.sendACopyToMyself = false;
                applyScope($scope);
                $("#sendInvoiceByEmailModal").modal({ keyboard: false }, "show");
            };
            $scope.sendInvoiceByEmail = function () {
                if ($scope.callingSendEmail) return;
                $scope.callingSendEmail = true;
                var emailInfo = {};
                emailInfo.toEmail = $scope.toEmail;
                emailInfo.emailSubject = $scope.emailSubject;
                emailInfo.emailText = $scope.emailText;
                emailInfo.fromEmail = $rootScope.currentBusiness.Email;
                emailInfo.includePdfAttachment = $scope.includePdfAttachment;
                emailInfo.markAsSent = $scope.markAsSent;
                emailInfo.sendACopyToMyself = $scope.sendACopyToMyself;
                emailInfo.invoiceId = $scope.invoice.Id;
                $scope.generatePDF(true, function (pdfFile) {
                    emailInfo.pdfFile = pdfFile;
                    callws(DefaultUrl.MainWebService + 'SendInvoiceByEmail', { emailInfo: emailInfo })
                        .success(function () {
                            $scope.callingSendEmail = false;
                            $("#sendInvoiceByEmailModal").modal("hide");
                            alertbox({ content: "ایمیل با موفقیت ارسال شد" });
                            $scope.$apply();
                            if ($scope.cashId) $scope.addCashPay();
                        }).fail(function (error) {
                            $scope.callingSendEmail = false;
                            applyScope($scope);
                            if ($scope.accessError(error)) return;
                            alertbox({ content: error, title: "خطا" });
                        }).loginFail(function () {
                            window.location = DefaultUrl.login;
                        });
                });
            };

            $scope.showPublicLink = function () {
                if (!$scope.invoiceSettings.onlineInvoiceEnabled) {
                    alertbox({ content: "ارسال فاکتور آنلاین فعال نیست. برای فعالسازی به منوی [تنظیمات / تنظیمات فاکتور آنلاین] مراجعه کنید." });
                    return;
                }
                if ($scope.calling)
                    return;
                $scope.contactEmail = $scope.invoice.Contact.Email;
                callws(DefaultUrl.MainWebService + "GetOnlineInvoiceLink", { invoiceId: $scope.invoice.Id })
                    .success(function (data) {
                        $scope.calling = false;
                        $scope.publicLink = data;
                        linkPopup.show();
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        $scope.$apply();
                        if ($scope.accessError(error)) return;
                        DevExpress.ui.notify(error, "error", 4000);
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
            $scope.copyPublicLink = function () {
                $("#publicLink").focus().select();
                try {
                    var successful = document.execCommand('copy');
                    if (successful)
                        DevExpress.ui.notify("لینک فاکتور آنلاین کپی شد.", "success", 2000);
                    else
                        DevExpress.ui.notify("لینک را به صورت دستی کپی کنید.", "error", 2000);
                } catch (err) {

                }
            };
            $scope.emailPublicLink = function () {
                if ($scope.calling)
                    return;
                if (!$scope.contactEmail) {
                    DevExpress.ui.notify("آدرس ایمیل را وارد کنید.", "error", 3000);
                    return;
                }
                callws(DefaultUrl.MainWebService + "EmailOnlineInvoiceLink", { invoiceId: $scope.invoice.Id, email: $scope.contactEmail })
                    .success(function (data) {
                        $scope.calling = false;
                        DevExpress.ui.notify("لینک فاکتور آنلاین ایمیل شد.", "success", 3000);
                        linkPopup.hide();
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        $scope.$apply();
                        if ($scope.accessError(error)) return;
                        DevExpress.ui.notify(error, "error", 3000);
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };

            $scope.redirect = function (id) {
                $state.go('')
            }

            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
            //


        }])
});