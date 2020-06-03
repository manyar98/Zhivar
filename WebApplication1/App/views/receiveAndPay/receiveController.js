define(['application', 'combo', 'scrollbar', 'helper', 'editItem', 'editContact', 'editBank', 'editCash', 'nodeSelect', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker'], function (app) {
    app.register.controller('receiveController', ['$scope','$rootScope', '$location', '$state', '$stateParams','dataService' ,
        function ($scope, $rootScope, $location, $state, $stateParams, dataService) {

        var invoiceDueDateObj = new AMIB.persianCalendar('transactionDate');

    $scope.init = function () {
        $rootScope.pageTitle("دریافت وجه / چک");
        $('#businessNav').show();
        $scope.alert = false;
        $scope.total = 0;
        $scope.receiptText = "";
        $scope.receiptNote = "";
  
        $scope.getContacts();
        $scope.getCashes();
        $scope.getBanks();
        $scope.getIncomeAccounts();
        $scope.getAccounts();
        //$scope.getDetailAccounts();

        $scope.cashId = $stateParams.cashId;// $scope.getRouteQuery($routeParams.params, 'cash');
        $scope.bankId = $stateParams.bankId;//$scope.getRouteQuery($routeParams.params, 'bank');
        $scope.rpId = $stateParams.rpId;//$scope.getRouteQuery($routeParams.params, 'id');

        applyScope($scope);
        $(function () {
            $.getScript("/App/printReports.js", function () { });
        });

        $scope.comboAccount = new HesabfaCombobox({
            items: [],
            containerEle: document.getElementById("comboAccountSelect"),
            toggleBtn: true,
            deleteBtn: true,
            itemClass: "hesabfa-combobox-item",
            activeItemClass: "hesabfa-combobox-activeitem",
            itemTemplate: "<div> {Coding} &nbsp;-&nbsp; {Name} </div>",
            matchBy: "account.Id",
            displayProperty: "{Name}",
            searchBy: ["Name", "Coding"],
            onSelect: $scope.accountSelect
        });
        $scope.comboDetailAccount = new HesabfaCombobox({
            items: [],
            containerEle: document.getElementById("comboDetailAccountSelect"),
            toggleBtn: true,
            deleteBtn: true,
            itemClass: "hesabfa-combobox-item",
            activeItemClass: "hesabfa-combobox-activeitem",
            itemTemplate: "<div> {Code} &nbsp;-&nbsp; {Name} </div>",
            matchBy: "detailAccount.Id",
            displayProperty: "{Name}",
            searchBy: ["Name", "Code"],
            onSelect: $scope.detailAccountSelect
        });

        //$(function () {
        //    $('[data-toggle="tooltip"]').tooltip();
        //    $.getScript("/js/app/printReports.js?ver=1.2.9.1", function () { });
        //});
    };
    $scope.getContacts = function () {
        $scope.loading = true;
        $.ajax({
            type: "POST",
            data: JSON.stringify("receivables"),
            url: "app/api/Contact/GetAllByOrganId",
            contentType: "application/json"
        }).done(function (res) {
            var contacts = res.data
            $scope.loading = false;
            $scope.contacts = contacts;

            $scope.comboContactSelect = new HesabfaCombobox({
                items: contacts,
                containerEle: document.getElementById("comboContactSelect"),
                toggleBtn: true,
                newBtn: true,
                deleteBtn: true,
                itemClass: "hesabfa-combobox-item",
                activeItemClass: "hesabfa-combobox-activeitem",
                itemTemplate: Hesabfa.comboContactTemplate,
                divider: true,
                matchBy: "contact.DetailAccount.ID",
                displayProperty: "{Name}",
                searchBy: ["Name", "Code"],
                onSelect: $scope.contactSelect,
                onNew: $scope.newContact
            });

            if ($scope.rpId)
                $scope.getReceiveAndPay(Number($scope.rpId));
            else {
                var rp = Hesabfa.newReceiveAndPay();
                rp.FinanYear = $scope.currentFinanYear;
                rp.DisplayDate = $scope.todayDate;
                rp.IsReceive = true;
                $scope.rp = rp;
            }

            applyScope($scope);
            }).fail(function (error) {
                $scope.loading = false;
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.getCashes = function () {
            $.ajax({
                type: "POST",
                url: "/app/api/Cash/GetAllByOrganId",
                contentType: "application/json"
            }).done(function (res) {
                var cashes = res.data;
                $scope.cashes = cashes;
                applyScope($scope);
                if ($scope.cashId) $scope.addCashPay(null, null, true);
            }).fail(function (error) {
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.getBanks = function () {
                $.ajax({
                    type: "POST",
                    url: "/app/api/Bank/GetAllByOrganId",
                    contentType: "application/json"
                }).done(function (res) {
                    var banks = res.data;
                $scope.banks = banks;
                applyScope($scope);
                if ($scope.bankId) $scope.addBankPay(null, null, true);
            }).fail(function (error) {
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.getIncomeAccounts = function () {
                    $.ajax({
                        type: "POST",
                        url: "/app/api/Accounting/GetIncomeAccounts",
                        contentType: "application/json"
                    }).done(function (res) {
                        var incomeAccounts = res.data;
       
                $scope.incomeAccounts = incomeAccounts;
                $scope.comboAccount.items = incomeAccounts;
                $scope.comboDetailAccount.items = [];
                applyScope($scope);
            }).fail(function (error) {
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.getAccounts = function () {
                        $.ajax({
                            type: "POST",
                            data: JSON.stringify(3),
                            url: "/app/api/Accounting/GetAccountsByLevel",
                            contentType: "application/json"
                        }).done(function (res) {
                            var accounts = res.data;

                $scope.accounts = accounts;
                $scope.comboAccount.items = accounts;
                $scope.comboDetailAccount.items = [];
                applyScope($scope);
            }).fail(function (error) {
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.getDetailAccounts = function () {
                            $.ajax({
                                type: "POST",
                                data: JSON.stringify("all"),
                                url: "/app/api/Accounting/GetDetailAccounts",
                                contentType: "application/json"
                            }).done(function (res) {
                                var detailAccounts = res.data;

                $scope.detailAccounts = detailAccounts;
                applyScope($scope);
            }).fail(function (error) {
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.getReceiveAndPay = function (id) {

        $.ajax({
            type: "POST",
            data: JSON.stringify(id),
            url: "/app/api/PayRecevie/LoadReceiveAndPay",
            contentType: "application/json"
        }).done(function (rp) {
            $scope.rp = rp.data;
            $scope.setView(rp.data);
            applyScope($scope);
        }).fail(function (error) {
            if ($scope.accessError(error)) return;
            alertbox({ content: error, type: "error" });
            });

   
    };
    $scope.setView = function (rp) {
        $scope.disableContact = false;
        if (rp.Type === 1)
            $scope.changeReceive('contact');
        else if (rp.Type === 2)
            $scope.changeReceive('income');
        else if (rp.Type === 4)
            $scope.changeReceive('other');

        if (rp.Contact) {
            $scope.comboContactSelect.setSelected(rp.Contact);
            $scope.contactSelect(rp.Contact);
        }
        if (rp.Account) {
            $scope.comboAccount.setSelected(rp.Account);
            $scope.accountSelect(rp.Account);
        }
        if (rp.DetailAccount) {
            setTimeout(function () {
                $scope.comboDetailAccount.setSelected(rp.DetailAccount);
                $scope.detailAccountSelect(rp.DetailAccount);
            }, 50);
        }
        var length = rp.Items.length;
        for (var i = 0; i < length; i++) {
            var item = rp.Items[i];
            if (item.Type === 1)
                $scope.addCashPay(item, i);
            else if (item.Type === 2)
                $scope.addBankPay(item, i);
            else if (item.Type === 3) {
                $scope.addChequePay(item);
                if (item.Cheque.Status !== 0)
                    $scope.disableContact = true;
            }
        }

        $scope.calTotal();
    };
    $scope.addCashPay = function (item, index, newCash) {
        var newItem = Hesabfa.newReceiveAndPayItem(item);
        newItem.Type = 1;
        if (newCash)
            $scope.rp.Items.push(newItem);
        applyScope($scope);

        setTimeout(function () {
            if (typeof index === "undefined" || index == null)
                index = $scope.rp.Items.length - 1;
            if (!$scope.comboCash) $scope.comboCash = [];
            $scope.comboCash[index] = new HesabfaCombobox({
                items: $scope.cashes,
                containerEle: document.getElementById("cashSelect" + index),
                toggleBtn: true,
                newBtn: true,
                deleteBtn: true,
                itemClass: "hesabfa-combobox-item",
                activeItemClass: "hesabfa-combobox-activeitem",
                itemTemplate: "<div> {DetailAccount.Code} &nbsp;-&nbsp; {Name} </div>",
                matchBy: "cash.Id",
                displayProperty: "{Name}",
                searchBy: ["Name", "DetailAccount.Code"],
                onSelect: function (cash) {
                    newItem.Cash = cash;
                },
                onNew: function () {
                    $scope.newCash(index, newItem);
                }
            });

            if (newItem.Cash)
                $scope.comboCash[index].setSelected(newItem.Cash);

            if ($scope.cashId) {
                var cash = findByID($scope.cashes, $scope.cashId);
                newItem.Cash = cash;
                $scope.comboCash[index].setSelected(cash);
            }
        }, 0);
    };
    $scope.addBankPay = function (item, index, newBank) {
        var newItem = Hesabfa.newReceiveAndPayItem(item);
        newItem.Type = 2;
        if (newBank)
            $scope.rp.Items.push(newItem);
        applyScope($scope);

        setTimeout(function () {
            if (typeof index === "undefined" || index == null)
                index = $scope.rp.Items.length - 1;
            if (!$scope.comboBank) $scope.comboBank = [];
            $scope.comboBank[index] = new HesabfaCombobox({
                items: $scope.banks,
                containerEle: document.getElementById("bankSelect" + index),
                toggleBtn: true,
                newBtn: true,
                deleteBtn: true,
                itemClass: "hesabfa-combobox-item",
                activeItemClass: "hesabfa-combobox-activeitem",
                itemTemplate: Hesabfa.comboBankTemplate,
                divider: true,
                matchBy: "bank.Id",
                displayProperty: "{Name}",
                searchBy: ["Name", "DetailAccount.Code"],
                onSelect: function (bank) {
                    newItem.Bank = bank;
                },
                onNew: function () {
                    $scope.newBank(index, newItem);
                }
            });

            if (newItem.Bank)
                $scope.comboBank[index].setSelected(newItem.Bank);

            if ($scope.bankId) {
                var bank = findByID($scope.banks, $scope.bankId);
                newItem.Bank = bank;
                $scope.comboBank[index].setSelected(bank);
            }
        }, 0);
    };
    $scope.addChequePay = function (item, newCheque) {

        var index = $scope.rp.Items.length - 1;
        var chqDate = document.getElementById("chqDate" + index);
        var chqDateObj = new AMIB.persianCalendar(chqDate);

        $scope.hasCheque = true;
        var newItem = Hesabfa.newReceiveAndPayItem(item);
        newItem.Type = 3;
        if (newCheque) {
            newItem.Cheque = { Status: 0, Amount: 0 };
            $scope.rp.Items.push(newItem);
        }
        applyScope($scope);
    };

    $scope.newCash = function (index, activeRow) {
        $scope.activeRow = activeRow;
        $scope.activeRowIndex = index;
        $scope.alert = false;
        $scope.cash = null;
        $scope.editCashModal = true;
    };
    $scope.newBank = function (index, activeRow) {
        $scope.activeRow = activeRow;
        $scope.activeRowIndex = index;
        $scope.alert = false;
        $scope.bank = null;
        $scope.editBankModal = true;
    };
    $scope.newContact = function () {
        $scope.alert = false;
        $scope.contact = null;
        $scope.editContactModal = true;
    };
    $scope.getEditedCash = function (cash) {
        if (!cash) return;
        $scope.cashes.push(cash);
        $scope.editCashModal = false;
        $scope.activeRow.Cash = cash;
        $scope.activeRow.DetailAccount = cash.DetailAccount;
        $scope.comboCash[$scope.activeRowIndex].setSelected(cash);
    };
    $scope.getEditedBank = function (bank) {
        if (!bank) return;
        $scope.banks.push(bank);
        $scope.editBankModal = false;
        $scope.activeRow.Bank = bank;
        $scope.activeRow.DetailAccount = bank.DetailAccount;
        $scope.comboBank[$scope.activeRowIndex].setSelected(bank);
    };
    $scope.getEditedContact = function (contact) {
        if (!contact) return;
        $scope.fromContactDetailAccount = contact.DetailAccount;
        $scope.fromContact = contact;
        $scope.editContactModal = false;
        $scope.comboContactSelect.setSelected(contact);
    };
    $scope.deleteRow = function (index) {
        var item = $scope.rp.Items[index];
        if (item.Cheque && item.Cheque.Status !== 0) {
            alertbox({ content: "امکان حذف چک بعلت تغییر وضعیت وجود ندارد", type: "warning" });
            return;
        }

        $scope.rp.Items.splice(index, 1);
        $scope.calTotal();

        $scope.hasCheque = false;
        for (var i = 0; i < $scope.rp.Items.length; i++) {
            if ($scope.rp.Items[i].Type === 3)
                $scope.hasCheque = true;
        }
    };
    $scope.contactSelect = function (contact) {
        var findById = window.findById($scope.contacts, contact.ID);
        $scope.fromContactDetailAccount = findById.DetailAccount;
        $scope.fromContact = findById;
        applyScope($scope);
    };
    $scope.accountSelect = function (account) {
        $scope.SelectedAccount = account;
        var relatedAccounts = [];
        if ($scope.detailAccounts) {
            for (var i = 0; i < $scope.detailAccounts.length; i++) {
                if ($scope.detailAccounts[i].RelatedAccounts.indexOf("," + account.Id + ",") > -1)
                    relatedAccounts.push($scope.detailAccounts[i]);
            }
        }
        $scope.comboDetailAccount.items = relatedAccounts;
        $scope.comboDetailAccount.setSelected(null);
        return true;
    };
    $scope.detailAccountSelect = function (detailAccount) {
        $scope.SelectedDetailAccount = detailAccount;
    };
    $scope.changeReceive = function (type) {
        $scope.Receive = type;
        $scope.fromContactDetailAccount = null;
        $scope.SelectedAccount = null;
        $scope.SelectedDetailAccount = null;
        $scope.comboAccount.setSelected(null);
        $scope.comboDetailAccount.setSelected(null);
        if (type === "income") {
            $scope.comboAccount.items = $scope.incomeAccounts;
            $scope.comboDetailAccount.items = [];
            $scope.rp.Type = 2;
        }
        else if (type === "other") {
            $scope.comboAccount.items = $scope.accounts;
            $scope.comboDetailAccount.items = [];
            $scope.rp.Type = 3;
        }
        else if (type === "contact")
            $scope.rp.Type = 1;
    };
    $scope.calTotal = function () {
        var transactions = $scope.rp.Items;
        var total = 0;
        for (var i = 0; i < transactions.length; i++)
            total += parseFloat(transactions[i].Amount);
        $scope.total = total;
    };
    $scope.submit = function (printReceipt, next) {
        if ($scope.calling) return;
        $scope.alert = false;
        $scope.calling = true;
        $scope.asignChequeDatesAndContact();

        if ($scope.total == undefined || $scope.total == 0) {
            $scope.calling = false;
            $scope.alert = true;
            $scope.alertType = "danger";
            $scope.alertMessage = "بابت دریافت باید مبلغ بیشتر از صفر باشد.";
            $scope.$apply();
            DevExpress.ui.notify("بابت دریافت باید مبلغ بیشتر از صفر باشد ", "danger", 3000);
            return;
        }

        if ($scope.Receive == undefined) {
            $scope.calling = false;
            $scope.alert = true;
            $scope.alertType = "danger";
            $scope.alertMessage = "نوع دریافت را مشخص کنید: از شخص، بابت درآمد یا سایر";
            applyScope($scope);
            return;
        }
        $scope.rp.Contact = $scope.fromContact;
        $scope.rp.Account = $scope.SelectedAccount;
        $scope.rp.DetailAccount = $scope.SelectedDetailAccount;
        $scope.rp.IsReceive = true;

        $.ajax({
            type: "POST",
            data: JSON.stringify($scope.rp),
            url: "app/api/PayRecevie/SaveReceiveAndPay",
            contentType: "application/json"
        }).done(function (res) {
        
            var rp = res.data

     
                $scope.calling = false;

                $scope.rp = rp;
                applyScope($scope);
                $scope.setView(rp);

                // message
                DevExpress.ui.notify("دریافت با موفقیت ثبت شد. مبلغ: " + $scope.money($scope.total) + " " + $scope.getCurrency(), "success", 3000);

                if (printReceipt) $scope.printReceiptDialog();
                if (next) $scope.cleanFormForNextEntry();
                applyScope($scope);
            })
            .fail(function (error) {
                $scope.calling = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                DevExpress.ui.notify(Hesabfa.farsiDigit(error), "error", 3000);
                applyScope($scope);
                window.scrollTo(0, 0);
            });
    };
    $scope.cleanFormForNextEntry = function () {
        $scope.rpId = 0;
        $scope.comboContactSelect.setSelected(null);
        $scope.comboAccount.setSelected(null);
        $scope.comboDetailAccount.setSelected(null);
        $scope.Receive = undefined;
        var rp = Hesabfa.newReceiveAndPay();
        rp.FinanYear = $scope.currentFinanYear;
        rp.DisplayDate = $scope.rp.DisplayDate;  //$scope.todayDate;
        rp.IsReceive = true;
        $scope.rp = rp;
        $scope.calTotal();
    };
    $scope.cancel = function () {
        window.history.back();
    };
    $scope.asignChequeDatesAndContact = function () {
        var transactions = $scope.rp.Items;
        for (var i = 0; i < transactions.length; i++) {
            if (transactions[i].Type === 3) {
                if ($scope.fromContact)
                    transactions[i].Cheque.Contact = $scope.fromContact;
                transactions[i].Cheque.Amount = transactions[i].Amount;
            }
        }
    };
    $scope.getBalance = function (contact) {
        if (!contact) return 0;
        contact.Balance = contact.Liability - contact.Credits;
        if (contact.Balance >= 0) return contact.Balance;
        else return contact.Balance * -1;
    };
    $scope.getBalanceAfterTrans = function (contact) {
        if (!contact) return 0;
        var balance = contact.Liability - contact.Credits;
        if ($scope.total) balance = (contact.Liability - (contact.Credits + $scope.total));
        if (balance >= 0) return balance;
        else return balance * -1;
    };
    $scope.getTashkhis = function (contact) {
        if (!contact) return "-";
        contact.Balance = contact.Liability - contact.Credits;
        if (contact.Balance > 0) return "بدهکار";
        else if (contact.Balance < 0) return "بستانکار";
        else return "-";
    };
    $scope.getTashkhisAfterTrans = function (contact) {
        if (!contact) return "-";
        var balance = contact.Liability - contact.Credits;
        if ($scope.total) balance = (contact.Liability - (contact.Credits + $scope.total));
        if (balance > 0) return "بدهکار";
        else if (balance < 0) return "بستانکار";
        else return "-";
    };
    $scope.getBgByBalanceState = function (tashkhis) {
        if (tashkhis === "بدهکار") return "bg-danger";
        else if (tashkhis === "بستانکار") return "bg-success";
        else return "bg-info";
    };
    $scope.gatherReceiptInfo = function () {
        var i = {};
        $scope.asignChequeDatesAndContact();
        i.date = $scope.rp.DisplayDate;
        var newTranses = [];
        angular.copy($scope.rp.Items, newTranses);
        i.transactions = newTranses;
        i.totalStr = "مبلغ کل: " + $scope.money($scope.total) + " " + $scope.getCurrency() + " (" + wordifyfa($scope.total, 0) + " " + $scope.getCurrency() + ") ";
        $scope.receiptInfo = i;
    };
    $scope.printReceiptDialog = function () {
        $scope.gatherReceiptInfo();
        var strName = $scope.fromContact ? $scope.fromContact.Name : "";
        if ($scope.Receive === "income") strName = $scope.SelectedAccount ? $scope.SelectedAccount.Name : "";
        if ($scope.Receive === "other") strName = $scope.SelectedAccount ? $scope.SelectedAccount.Name : "";
        var strForWhat = $scope.rp.Invoice ? "بابت فاکتور شماره " + $scope.rp.Invoice.Number : "بابت بدهی ";
        $scope.receiptText = "مبلغ " + $scope.money($scope.total) + " " + $scope.getCurrency();
        $scope.receiptText += " (" + wordifyfa($scope.total, 0) + " " + $scope.getCurrency() + ") ";
        $scope.receiptText += "از آقای/خانم " + strName;
        $scope.receiptText += " " + strForWhat + " به صورت نقد/چک دریافت گردید.";
        $scope.receiptText = $rootScope.farsiDigit($scope.receiptText);
        $scope.addTransactionListToReceipt = true;
        $('#modalPrintRecipt').modal('show');
    };
    $scope.printReceipt = function () {
        applyScope($scope);
        $('#modalPrintRecipt').modal('hide');
        var info = {};
        info.text = $('#receiptText').val();
        info.note = $('#receiptNote').val();
        info.title = $rootScope.currentBusiness.Name;
        info.subTitle = "رسید دریافت وجه/چک";
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
        $('#modalPrintRecipt').modal('hide');
        var info = {};
        info.text = $('#receiptText').val();
        info.note = $('#receiptNote').val();
        info.title = $rootScope.currentBusiness.Name;
        info.subTitle = "رسید دریافت وجه/چک";
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
    //
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});
