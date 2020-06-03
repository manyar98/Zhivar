define(['application', 'combo', 'scrollbar', 'dx', 'roweditor', 'helper', 'gridHelper', 'editBank', 'editCash', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker', 'number'], function (app) {
    app.register.controller('viewCostController', ['$scope','$rootScope', '$stateParams', '$location', '$compile', '$state',
        function ($scope, $rootScope, $stateParams, $location, $compile, $state) {

            var gridChequeList, linkPopup;

            var payDateObj = new AMIB.persianCalendar('payDate');
            var receiveDateObj = new AMIB.persianCalendar('receiveDate');

            $scope.init = function () {
                $rootScope.pageTitle("بارگیری صورت هزینه...");
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
                rp.IsReceive = true; // depend on cost type
                rp.Type = 1;
                $scope.rp = rp;
                $scope.total = 0;

                $scope.loadCost(id);

                linkPopup = $("<div/>").appendTo("body").dxPopup({
                    rtlEnabled: true,
                    showTitle: true,
                    height: 'auto',
                    width: 500,
                    title: "لینک صورت هزینه آنلاین",
                    dragEnabled: true,
                    closeOnOutsideClick: true,
                    contentTemplate: function (contentElement) {
                        var html = $('#publicLinkPopup').html();
                        $('#publicLinkPopup').html("");
                        $(html).appendTo(contentElement);
                        $compile(contentElement.contents())($scope);
                    }
                }).dxPopup("instance");

                gridChequeList = dxGrid({
                    elementId: 'gridChequeList',
                    layoutKeyName: "grid-pay-cheque",
                    selection: { mode: "multiple" },
                    height: 400,
                    columns: [
                        { caption: 'شماره چک', dataField: 'ChequeNumber', width: 105, printWidth: 2 },
                        { caption: 'مبلغ چک', dataField: 'Amount', width: 120, printWidth: 2, printFormat: "#" },
                        { caption: 'تاریخ چک', dataField: 'DisplayDate', width: 100, printWidth: 2 },
                        { caption: 'دریافت از', dataField: 'Contact.Name', printWidth: 4 },
                        { caption: 'بانک', dataField: 'BankName', width: 120, printWidth: 3 },
                        { caption: 'شعبه', dataField: 'BankBranch', width: 100, printWidth: 3 }
                    ],
                    print: {
                        fileName: "chequeList.pdf",
                        rowColumnWidth: 1.5,
                        page: {
                            landscape: false
                        },
                        header: {
                            title1: $rootScope.currentBusiness.Name,
                            title2: "لیست چک ها"
                        }
                    },
                    onCellClick: function (item) {
                        if (!item.data) return;
                        $scope.addOtherChequePay(item.data, null, true);
                    }
                });

                var sendCostSms = $rootScope.getUISetting("sendCostSms2");
                $scope.sendSmsByReceive = !!sendCostSms;

                applyScope($scope);
                $(function () {
                    $.getScript("/App/printReports.js", function () { });
                });
            };
            $scope.showSuccessSubmision = function (cost, docNumber) {
                if ($scope.submisionStatus && $scope.submisionStatus === 'approved') {
                    $scope.alert = true;
                    var contactStr = cost.Contact ? cost.Contact.Name : "";
                    if (cost.CostType === 4) contactStr = "ضایعات کالا";
                    $scope.alertMessage = "صورت هزینه {2} تایید شد - {0} - مبلغ کل: {1}".
                        formatString(contactStr, $scope.money(cost.Payable), cost.Number, cost.CostType === 0 ? "فروش" : "خرید");
                    if (docNumber) $scope.alertMessage += " - شماره سند: " + docNumber + ".";
                    if ($scope.smsSent) $scope.alertMessage += " [پیامک ارسال شد]";
                    $scope.alertType = 'success';
                    $scope.$apply();
                }
            };
            $scope.loadCost = function (id, sendSms) {
                $scope.loading = true;
                $.ajax({
                    type: "POST",
                    data: JSON.stringify(id),
                    url: "/app/api/Cost/LoadCostTransObj",
                    contentType: "application/json"
                }).done(function (res) {
                    var result = res.data;
                    $scope.loading = false;
                    $scope.cost = result.cost;
                    $scope.cost.CostItems = result.costItems;
                    $scope.receiveDate = $scope.todayDate;
                    $scope.payDate = $scope.todayDate;
                    $scope.payments = result.payments;
                    $scope.costSettings = result.costSettings;
                    $scope.costSettingsCopy = {};
                    angular.copy(result.costSettings, $scope.costSettingsCopy);
                    $scope.rp.IsReceive = result.cost.CostType === 0 || result.cost.CostType === 3;
                    $scope.rp.Cost = result.cost;

                    $scope.showSuccessSubmision($scope.cost, result.docNumber);

                    $scope.cashes = result.cashes;
                    $scope.banks = result.banks;
                    if ($scope.cashId) $scope.addCashPay();
                    if ($scope.bankId) $scope.addBankPay();

                    var l = $scope.cost.CostItems.length; // محاسبه مالیات و تخفیف کل صورت هزینه جهت نمایش
                    $scope.totalDiscount = 0;
                    $scope.totalTax = 0;
                    for (var i = 0; i < l; i++) {
                        $scope.totalDiscount += $scope.cost.CostItems[i].Discount;
                        $scope.totalTax += $scope.cost.CostItems[i].Tax;
                    }

                    //  $("#imgLogo").attr("src", $scope.costSettings.businessLogo);

                    if (sendSms) $scope.sendSms();

                    var print = $stateParams.print;// $scope.getRouteQuery($routeParams.params, 'print');
                    if (print && print === 'true')
                        $scope.generatePDF();

                    $scope.setViewCostPageTitle();
                    applyScope($scope);
                }).fail(function (error) {
                    $scope.loading = false;
                    $scope.cost = null;
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
            $scope.accountLeave = function () {
                setTimeout(function () { $("#accountSelect").hide(); }, 100);
            };
            $scope.accountSelect = function (account) {
                $scope.transaction.Account = account;
                $scope.paidToAccount = account.Name;
                $scope.accountChange();
                $("#accountSelect").hide();
                $scope.$apply();
            };
            $scope.bankSelect = function (bank, selectedBy, trans) {
                if (!bank) return;
                trans.Cheque.BankName = bank.Name;
                trans.Cheque.BankBranch = bank.Branch;
                trans.Cheque.BankDetailAccount = bank.DetailAccount;
                applyScope($scope);
            };
            $scope.saveReceipt = function () {
                if ($scope.calling) return;
                $scope.alert = false;
                $scope.calling = true;
                $scope.asignChequeDatesAndContact();
                $scope.rp.Contact = $scope.cost.Contact;

                $.ajax({
                    type: "POST",
                    data: JSON.stringify($scope.rp),
                    url: "/app/api/PayRecevie/SaveReceiveAndPay",
                    contentType: "application/json"
                }).done(function (res) {
                    // var rp = res.data;
                    $scope.calling = false;
                    $scope.cost.Status = 3;
                    // message
                    $scope.alert = true;
                    $scope.alertType = "success";
                    $scope.alertMessage = "دریافت با موفقیت ثبت شد. مبلغ: " + $scope.money($scope.total);
                    // clean form for new entry
                    $scope.rp.Items = [];
                    $scope.calTotal();
                    // reload cost
                    $scope.loadCost($scope.cost.ID, $scope.sendSmsByReceive);
                    applyScope($scope);
                }).fail(function (error) {
                    $scope.calling = false;
                    applyScope($scope);
                    if ($scope.accessError(error)) return;
                    DevExpress.ui.notify(Hesabfa.farsiDigit(error), "error", 3000);
                });
            };
            $scope.savePayment = function () {
                if ($scope.calling) return;
                $scope.alert = false;
                $scope.calling = true;
                $scope.asignChequeDatesAndContact();
                $scope.rp.Contact = $scope.cost.Contact;
                $scope.rp.Description = $scope.cost.Explain;

                $.ajax({
                    type: "POST",
                    data: JSON.stringify($scope.rp),
                    url: "/app/api/PayRecevie/SaveReceiveAndPay",
                    contentType: "application/json"
                }).done(function (res) {
                    $scope.calling = false;
                    $scope.cost.Status = 3;
                    // message
                    $scope.alert = true;
                    $scope.alertType = "success";
                    $scope.alertMessage = "پرداخت با موفقیت ثبت شد. مبلغ: " + $scope.money($scope.total);
                    // clean form for new entry
                    $scope.rp.Items = [];
                    $scope.calTotal();
                    // reload cost
                    $scope.loadCost($scope.cost.ID);
                    applyScope($scope);
                }).fail(function (error) {
                    $scope.calling = false;
                    applyScope($scope);
                    if ($scope.accessError(error)) return;
                    DevExpress.ui.notify(Hesabfa.farsiDigit(error), "error", 3000);
                });
            };
            $scope.deleteCost = function () {
                var tip1 = "";
                var tip2 = "";
                if ($scope.payments.length > 0) {
                    tip1 = "<div class='bg-warning pd-5 small'>";
                    tip1 += "نکته: با حذف این صورت هزینه، دریافت ها و پرداخت های ثبت شده برای این صورت هزینه نیز حذف می گردد.";
                    tip1 += "</div>";
                }
                if ($scope.cost.CostType === 1 || $scope.cost.CostType === 2) {
                    tip2 = "<div class='bg-warning pd-5 small'>";
                    tip2 += "حذف صورت هزینه خرید (یا برگشت از فروش) ممکن است باعث ایجاد موجودی منفی در کالاها شود";
                    tip2 += " ، که این موضوع باید توسط شما کنترل شود.";
                    tip2 += "</div>";
                }
                questionbox({
                    content: "آیا از حذف این صورت هزینه مطمئن هستید؟" + tip1 + tip2,
                    onBtn1Click: function () {
                        if ($scope.calling) return;
                        $scope.calling = true;
                        var strIds = $scope.cost.Id + ",";
                        callws(DefaultUrl.MainWebService + 'DeleteCosts', { costIds: strIds })
                            .success(function (result) {
                                $scope.calling = false;
                                Hesabfa.pageParams = {};
                                Hesabfa.pageParams.cost = $scope.cost;
                                var pathPart = "";
                                if ($scope.cost.CostType === 1) pathPart = "show=bills&";
                                if ($scope.cost.CostType === 2) pathPart = "show=saleReturns&";
                                if ($scope.cost.CostType === 3) pathPart = "show=purchaseReturns&";
                                if ($scope.cost.CostType === 4) pathPart = "show=wastes&";
                                $location.path('/costs/' + pathPart + 'status=deleted');
                                applyScope($scope);
                            }).fail(function (error) {
                                $scope.calling = false;
                                if ($scope.accessError(error)) { applyScope($scope); return; }
                                $scope.alert = true;
                                $scope.alertType = "danger";
                                $scope.alertMessage = error;
                                applyScope($scope);
                            }).loginFail(function () {
                                window.location = DefaultUrl.login;
                            });
                    }
                });
            };
            $scope.deleteTransaction = function (trans) {
                // delete transaction
                if (trans.Cheque == null) { // if was cash or bank trans
                    questionbox({
                        title: "هشدار",
                        content: "آیا از حذف تراکنش انتخاب شده مطمئن هستید؟" + "</br>"
                        + $scope.money(trans.Amount) + " " + $scope.getCurrency() + " در تاریخ " + $rootScope.farsiDigit(trans.AccDocument.DisplayDate)
                        + (trans.Cheque == null ? (" " + ($scope.cost.CostType === 0 || $scope.cost.CostType === 3 ? 'به' : 'از') + " " + trans.DetailAccount.Name) : '')
                        + (trans.Cheque != null ? (" طی چک شماره " + trans.Cheque.ChequeNumber) : '')
                        + (trans.Reference ? (" ارجاع: " + trans.Reference) : ''),
                        onBtn1Click: function () {
                            if ($scope.calling) return false;
                            $scope.calling = true;
                            var selectedItems = [];
                            selectedItems.push(trans);
                            callws(DefaultUrl.MainWebService + "DeleteTransactions", { transactions: selectedItems })
                                .success(function () {
                                    $scope.calling = false;
                                    // reload cost
                                    $scope.loadCost($scope.cost.Id);
                                    $scope.alert = true;
                                    $scope.alertMessage = " تراکنش با موفقیت حذف شد";
                                    $scope.alertType = "success";
                                    applyScope($scope);
                                }).fail(function (error) {
                                    $scope.calling = false;
                                    applyScope($scope);
                                    if ($scope.accessError(error)) return;
                                    alertbox({ content: error });
                                }).loginFail(function () {
                                    window.location = DefaultUrl.login;
                                });
                        }
                    });
                } else { // if was cheque
                    var cheque = trans.Cheque;
                    if (cheque.Status === 2) {
                        alertbox({ content: "امکان حذف چکی که پاس شده است وجود ندارد." });
                        return false;
                    }
                    if (cheque.Status !== 0) {
                        alertbox({ content: "فقط چکی که در وضعیت عادی قرار داد قابل حذف می باشد." });
                        return false;
                    }

                    $scope.calling = true;
                    callws(DefaultUrl.MainWebService + "GetChequeRelatedDocumentsNumber", { chequeId: cheque.Id })
                        .success(function (docsNumbers) {
                            $scope.calling = false;
                            $scope.docsNumbers = docsNumbers;
                            questionbox({
                                title: "هشدار",
                                content: "آیا از حذف این چک مطمئن هستید؟" + "<br>"
                                + "<input id='deleteChequeDocs' type='checkbox' checked='checked'> حذف سند (یا اسناد) مرتبط با چک" + "<br><br>"
                                + "شماره سند (یا اسناد) مرتبط: " + $scope.docsNumbers + "<br>"
                                + "<p class='bg-warning small' style='padding:5px;'><span class='fa fa-warning' aria-hidden='true'></span>&nbsp" +
                                "توجه: اگر چک دریافتی یا پرداختی در قالب یک سند ترکیبی همراه با چندین دریافت و پرداخت دیگر ثبت شده است توصیه میشود سند یا اسناد مربوطه حذف نشده و بصورت دستی ویرایش شوند در غیر این صورت تمام تراکنش های مرتبط با سند حذف خواهند شد." +
                                "</p>",
                                onBtn1Click: function () {
                                    if ($scope.calling) return false;
                                    $scope.calling = true;
                                    $scope.deleteChequeDocs = $('#deleteChequeDocs').is(":checked");
                                    callws(DefaultUrl.MainWebService + "DeleteCheque", {
                                        chequeId: cheque.Id,
                                        deleteChequeDocs: $scope.deleteChequeDocs
                                    })
                                        .success(function (result) {
                                            $scope.calling = false;
                                            // reload cost
                                            $scope.loadCost($scope.cost.Id);
                                            $scope.alert = true;
                                            $scope.alertMessage = "چک انتخاب شده با موفقیت حذف شد";
                                            $scope.alertType = "success";
                                            applyScope($scope);
                                        }).fail(function (error) {
                                            $scope.calling = false;
                                            applyScope($scope);
                                            if ($scope.accessError(error)) return;
                                            alertbox({ content: error });
                                        }).loginFail(function () {
                                            window.location = DefaultUrl.login;
                                        });
                                }
                            });
                        }).fail(function (error) {
                            $scope.calling = false;
                            applyScope($scope);
                            if ($scope.accessError(error)) return;
                            alertbox({ content: error });
                        }).loginFail(function () {
                            window.location = DefaultUrl.login;
                        });
                }
            };
            $scope.setViewCostPageTitle = function () {
                if ($scope.cost.CostType === 0) $rootScope.pageTitle("صورت هزینه" + ($scope.cost.Contact ? " " + $scope.cost.Contact.Name : "") + " - شماره صورت هزینه: " + $scope.cost.Number);
                if ($scope.cost.CostType === 1) $rootScope.pageTitle("صورتحساب" + ($scope.cost.Contact ? " " + $scope.cost.Contact.Name : ""));
                if ($scope.cost.CostType === 2) $rootScope.pageTitle("برگشت از فروش" + ($scope.cost.Contact ? " " + $scope.cost.Contact.Name : ""));
                if ($scope.cost.CostType === 3) $rootScope.pageTitle("برگشت از خرید" + ($scope.cost.Contact ? " " + $scope.cost.Contact.Name : ""));
            };

            $scope.getCashes = function () {
                callws(DefaultUrl.MainWebService + 'GetCashes', {})
                    .success(function (cashes) {
                        $scope.cashes = cashes;
                        $scope.$apply();
                        if ($scope.cashId) $scope.addCashPay();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
            $scope.getBanks = function () {
                callws(DefaultUrl.MainWebService + 'GetBanksOptimized', {})
                    .success(function (banks) {
                        $scope.banks = banks;
                        $scope.$apply();
                        if ($scope.bankId) $scope.addBankPay();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
            $scope.addCashPay = function () {
                var newItem = Hesabfa.newReceiveAndPayItem();
                newItem.Type = 1;
                newItem.Amount = $scope.getRemindedMoney();
                $scope.rp.Items.push(newItem);
                applyScope($scope);
                $scope.calTotal();

                if ($scope.cost.CostType === 0 || $scope.cost.CostType === 3) {
                    // receive
                    setTimeout(function () {
                        var index = $scope.rp.Items.length - 1;

                        if (!$scope.comboReceiveCash) $scope.comboReceiveCash = [];
                        $scope.comboReceiveCash[index] = new HesabfaCombobox({
                            items: $scope.cashes,
                            containerEle: document.getElementById("receiveCashSelect" + index),
                            toggleBtn: true,
                            newBtn: true,
                            deleteBtn: true,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: "<div> {Code} &nbsp;-&nbsp; {Name} </div>",
                            matchBy: "cash.Id",
                            displayProperty: "{Name}",
                            searchBy: ["Name", "Code"],
                            onSelect: function (cash) {
                                newItem.Cash = cash;
                            },
                            onNew: function () {
                                $scope.newCash(index, newItem);
                            }
                        });
                    }, 0);
                } else {
                    // payment
                    setTimeout(function () {
                        var index = $scope.rp.Items.length - 1;


                        if (!$scope.comboPayCash) $scope.comboPayCash = [];
                        $scope.comboPayCash[index] = new HesabfaCombobox({
                            items: $scope.cashes,
                            containerEle: document.getElementById("payCashSelect" + index),
                            toggleBtn: true,
                            newBtn: true,
                            deleteBtn: true,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: "<div> {Code} &nbsp;-&nbsp; {Name} </div>",
                            matchBy: "cash.Id",
                            displayProperty: "{Name}",
                            searchBy: ["Name", "Code"],
                            onSelect: function (cash) {
                                newItem.Cash = cash;
                            },
                            onNew: function () {
                                $scope.newCash(index, newItem);
                            }
                        });
                    }, 0);
                }
            };
            $scope.addBankPay = function () {
                var newItem = Hesabfa.newReceiveAndPayItem();
                newItem.Type = 2;
                newItem.Amount = $scope.getRemindedMoney();
                $scope.rp.Items.push(newItem);
                applyScope($scope);
                $scope.calTotal();

                if ($scope.cost.CostType === 0 || $scope.cost.CostType === 3) {
                    // receive
                    setTimeout(function () {
                        var index = $scope.rp.Items.length - 1;
                        if (!$scope.comboReceiveBank) $scope.comboReceiveBank = [];
                        $scope.comboReceiveBank[index] = new HesabfaCombobox({
                            items: $scope.banks,
                            containerEle: document.getElementById("receiveBankSelect" + index),
                            toggleBtn: true,
                            newBtn: true,
                            deleteBtn: true,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: Hesabfa.comboBankTemplate,
                            divider: true,
                            matchBy: "bank.Id",
                            displayProperty: "{Name}",
                            searchBy: ["Name", "Code"],
                            onSelect: function (bank) {
                                newItem.Bank = bank;
                            },
                            onNew: function () {
                                $scope.newBank(index, newItem);
                            }
                        });
                    }, 0);
                } else {
                    // payment
                    setTimeout(function () {
                        var index = $scope.rp.Items.length - 1;
                        if (!$scope.comboPayBank) $scope.comboPayBank = [];
                        $scope.comboPayBank[index] = new HesabfaCombobox({
                            items: $scope.banks,
                            containerEle: document.getElementById("payBankSelect" + index),
                            toggleBtn: true,
                            newBtn: true,
                            deleteBtn: true,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: Hesabfa.comboBankTemplate,
                            divider: true,
                            matchBy: "bank.Id",
                            displayProperty: "{Name}",
                            searchBy: ["Name", "Code"],
                            onSelect: function (bank) {
                                newItem.Bank = bank;
                            },
                            onNew: function () {
                                $scope.newBank(index, newItem);
                            }
                        });
                    }, 0);
                }
            };
            $scope.addChequePay = function () {
                var newItem = Hesabfa.newReceiveAndPayItem();
                newItem.Type = 3;
                newItem.Cheque = { id: 0, Amount: 0, Status: 0 };
                newItem.Cheque.Contact = $scope.cost.Contact;
                newItem.Contact = $scope.cost.Contact;
                newItem.Amount = $scope.getRemindedMoney();
                newItem.Cheque.Amount = newItem.Amount;
                $scope.rp.Items.push(newItem);
                applyScope($scope);
                $scope.calTotal();

                if ($scope.cost.CostType === 1 || $scope.cost.CostType === 2) {
                    // پرداخت چک	
                    setTimeout(function () {
                        var index = $scope.rp.Items.length - 1;
                        var chqDatePay = document.getElementById("chqDatePay" + index);
                        var chqDatePayObj = new AMIB.persianCalendar(chqDatePay);

                        if (!$scope.comboChequeBank) $scope.comboChequeBank = [];
                        $scope.comboChequeBank[index] = new HesabfaCombobox({
                            items: $scope.banks,
                            containerEle: document.getElementById("chequeBankSelect" + index),
                            toggleBtn: true,
                            newBtn: true,
                            deleteBtn: true,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: Hesabfa.comboBankTemplate,
                            divider: true,
                            matchBy: "bank.Id",
                            displayProperty: "{Name}",
                            searchBy: ["Name", "Code"],
                            onSelect: function (bank) {
                                newItem.DetailAccount = bank.DetailAccount;
                                newItem.ChequeBank = bank;
                                $scope.bankSelect(bank, null, newItem);
                            },
                            onNew: function () {
                                $scope.newBank(index, newItem);
                            }
                        });
                    }, 0);
                }
            };
            $scope.getRemindedMoney = function () {
                var currentMoneyInTransactions = 0;
                if ($scope.rp.Items && $scope.rp.Items.length > 0) {
                    var l = $scope.rp.Items.length;
                    for (var i = 0; i < l; i++)
                        currentMoneyInTransactions += parseFloat($scope.rp.Items[i].Amount);
                }
                var rest = $scope.cost.Rest - currentMoneyInTransactions;
                return rest >= 0 ? rest : 0;
            };
            $scope.openChequeList = function () {
                $('#modalChequeList').modal('show');
                $scope.loadingChequeList = true;
                gridChequeList.beginCustomLoading();
                callws(DefaultUrl.MainWebService + "GetChequesToPay", {})
                    .success(function (cheques) {
                        $scope.loadingChequeList = false;
                        gridChequeList.endCustomLoading();
                        gridChequeList.fill(cheques);
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.loadingChequeList = false;
                        gridChequeList.endCustomLoading();
                        applyScope($scope);
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error });
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
            $scope.addOtherChequePay = function (cheque, item, newCheque) {
                if (typeof item === "undefined" || item == null) {
                    for (var i = 0; i < $scope.rp.Items.length; i++) {
                        if ($scope.rp.Items[i].Cheque && $scope.rp.Items[i].Cheque.Id === cheque.Id) {
                            alertbox({ content: "این چک قبلاً به لیست پرداخت اضافه شده" });
                            return;
                        }
                    }
                }
                $('#modalChequeList').modal('hide');
                var newItem = Hesabfa.newReceiveAndPayItem(item);
                newItem.Type = 4;
                newItem.Cheque = cheque;
                newItem.Amount = cheque.Amount;
                if (newCheque)
                    $scope.rp.Items.push(newItem);
                applyScope($scope);
                $scope.calTotal();
            };
            $scope.newCash = function (index, activeRow) {
                $scope.activeRow = activeRow;
                $scope.activeRowIndex = index;
                $scope.alert = false;
                $scope.cash = null;
                $scope.editCashModal = true;
                applyScope($scope);
            };
            $scope.newBank = function (index, activeRow) {
                $scope.activeRow = activeRow;
                $scope.activeRowIndex = index;
                $scope.alert = false;
                $scope.bank = null;
                $scope.editBankModal = true;
                applyScope($scope);
            };
            $scope.getEditedCash = function (cash) {
                if (!cash) return;
                $scope.cashes.push(cash);
                $scope.editCashModal = false;
                $scope.activeRow.Cash = cash;
                $scope.activeRow.DetailAccount = cash.DetailAccount;
                $scope.activeRow.AccountInput = cash.Name;
                if ($scope.cost.CostType === 0 || $scope.cost.CostType === 3) {
                    $scope.comboReceiveCash[$scope.activeRowIndex].setSelected(cash);
                } else {
                    $scope.comboPayCash[$scope.activeRowIndex].setSelected(cash);
                }
                $scope.$apply();
            };
            $scope.getEditedBank = function (bank) {
                if (!bank) return;
                $scope.banks.push(bank);
                $scope.editBankModal = false;
                $scope.activeRow.Bank = bank;
                $scope.activeRow.DetailAccount = bank.DetailAccount;
                $scope.activeRow.AccountInput = bank.Name;
                if ($scope.cost.CostType === 0 || $scope.cost.CostType === 3) {
                    $scope.comboReceiveBank[$scope.activeRowIndex].setSelected(bank);
                } else {
                    var comboPayBank = $scope.comboPayBank ? $scope.comboPayBank[$scope.activeRowIndex] : null;
                    if (comboPayBank)
                        comboPayBank.setSelected(bank);

                    var comboChequeBank = $scope.comboChequeBank ? $scope.comboChequeBank[$scope.activeRowIndex] : null;
                    if (comboChequeBank)
                        comboChequeBank.setSelected(bank);
                }
                $scope.$apply();
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
                        transactions[i].Cheque.Contact = $scope.cost.Contact;
                        transactions[i].Cheque.Amount = transactions[i].Amount;
                    }
                }
            };
            $scope.printPDF = function () {
                window.location = '/View/CostPrintPDF.ashx?id=' + $scope.cost.Id;
            };

            $scope.print = function (reset) {
                if (reset)

                    $scope.costSettings = {
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
                        purchaseInvoiceTitle: "صورت هزینه خرید",
                        receiveReceiptTitle: "رسید دریافت وجه / چک",
                        rowPerPage: "18",
                        saleDraftInvoiceTitle: "پیش صورت هزینه",
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
                // angular.copy($scope.costSettingsCopy, $scope.costSettings);
                //callws(DefaultUrl.MainWebService + 'GetBusinessFullInfo', {})
                //success(function (business) {
                var settings = jQuery.extend(true, {}, $scope.costSettings);
                settings.footerNote = $scope.cost.Note + "\n" + settings.footerNote;
                settings.footerNoteDraft = $scope.cost.Note + "\n" + settings.footerNoteDraft;

                var business = {
                    Name: "ژیور",
                    LegalName: "ژیور",
                    Address: "",
                    PostalCode: "",
                    Fax: ""
                };
                printCost($scope.cost, settings, $scope.payments, business, $scope.getCurrency());
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
                $scope.costSettings = {
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
                    purchaseInvoiceTitle: "صورت هزینه خرید",
                    receiveReceiptTitle: "رسید دریافت وجه / چک",
                    rowPerPage: "18",
                    saleDraftInvoiceTitle: "پیش صورت هزینه",
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
                var settings = jQuery.extend(true, {}, $scope.costSettings);
                settings.footerNote = $scope.cost.Note + "\n" + settings.footerNote;
                settings.footerNoteDraft = $scope.cost.Note + "\n" + settings.footerNoteDraft;

                generateCostPDF($scope.cost, settings, $scope.totalDiscount, $scope.totalTax, asString, doAfterRead, $scope.payments, null, null, business, $scope.getCurrency());
                //})
                //.fail(function (error) {
                //    if ($scope.accessError(error)) return;
                //    alertbox({ content: error });
                //})
                //.loginFail(function () {
                //    window.location = DefaultUrl.login;
                //});
            };
            $scope.printCostDialog = function () {
                $('#modalPrintCostBySettings').modal('show');
            };
            $scope.printStorehouseOrder = function () {
                $scope.costSettings.saleCostTitle = "حواله انبار";
                $scope.costSettings.hidePrices = true;
                $scope.print();
            };
            $scope.printStorehouseOrderPDF = function () {
                $scope.costSettings.saleCostTitle = "حواله انبار";
                $scope.costSettings.hidePrices = true;
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
                var IsReceive = $scope.cost.CostType === 0 || $scope.cost.CostType === 2 ? true : false;
                var strName = $scope.cost.ContactTitle ? $scope.cost.ContactTitle : ($scope.cost.Contact ? $scope.cost.Contact.Name : "");
                var strForWhat = "بابت صورت هزینه شماره " + $scope.cost.Number;
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
                    $rootScope.setUISetting("sendCostSms2", "true");
                else
                    $rootScope.setUISetting("sendCostSms2", "false");
            };
            $scope.openSmsEditor = function () {
                callws(DefaultUrl.MainWebService + 'GetSetting', { settingName: "SMS_DefaultCostSMS2" })
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
                callws(DefaultUrl.MainWebService + "SaveSetting", { name: "SMS_DefaultCostSMS2", value: sms })
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
                    settingName: "SMS_DefaultCostSMS2", cost: $scope.cost
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

            $scope.openSendCostByEmailDialog = function () {
                $scope.toEmail = $scope.cost.Contact.ContactEmail ? $scope.cost.Contact.ContactEmail + "" : $scope.cost.Contact.Email + "";
                $scope.emailSubject = "صورت هزینه شماره " + $scope.cost.Number + " از طرف "
                    + $rootScope.currentBusiness.Name + " برای " + $scope.cost.Contact.Name;
                $scope.emailText = "سلام " + $scope.cost.Contact.Name + "\n";
                $scope.emailText += "صورت هزینه صادر شده برای شما به شماره " + $scope.cost.Number + " و مبلغ "
                    + $scope.money($scope.cost.Payable) + " " + $scope.getCurrency() + " را از ضمیمه ایمیل دانلود کنید.";
                $scope.emailText += "\n\n" + "تاریخ سررسید این صورت هزینه " + $scope.cost.DisplayDueDate + " می باشد.";
                $scope.emailText += "\n\n" + "با تشکر" + "\n" + $rootScope.currentBusiness.Name;
                $scope.includePdfAttachment = true;
                $scope.markAsSent = true;
                $scope.sendACopyToMyself = false;
                applyScope($scope);
                $("#sendCostByEmailModal").modal({ keyboard: false }, "show");
            };
            $scope.sendCostByEmail = function () {
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
                emailInfo.costId = $scope.cost.Id;
                $scope.generatePDF(true, function (pdfFile) {
                    emailInfo.pdfFile = pdfFile;
                    callws(DefaultUrl.MainWebService + 'SendCostByEmail', { emailInfo: emailInfo })
                        .success(function () {
                            $scope.callingSendEmail = false;
                            $("#sendCostByEmailModal").modal("hide");
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
                if (!$scope.costSettings.onlineCostEnabled) {
                    alertbox({ content: "ارسال صورت هزینه آنلاین فعال نیست. برای فعالسازی به منوی [تنظیمات / تنظیمات صورت هزینه آنلاین] مراجعه کنید." });
                    return;
                }
                if ($scope.calling)
                    return;
                $scope.contactEmail = $scope.cost.Contact.Email;
                callws(DefaultUrl.MainWebService + "GetOnlineCostLink", { costId: $scope.cost.Id })
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
                        DevExpress.ui.notify("لینک صورت هزینه آنلاین کپی شد.", "success", 2000);
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
                callws(DefaultUrl.MainWebService + "EmailOnlineCostLink", { costId: $scope.cost.Id, email: $scope.contactEmail })
                    .success(function (data) {
                        $scope.calling = false;
                        DevExpress.ui.notify("لینک صورت هزینه آنلاین ایمیل شد.", "success", 3000);
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