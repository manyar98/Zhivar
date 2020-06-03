define(['application', 'combo', 'scrollbar', 'dx', 'roweditor', 'helper', 'gridHelper', 'editBank', 'editCash', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker', 'number'], function (app) {
    app.register.controller('payController', ['$scope','$rootScope',  '$stateParams', '$location', '$compile', '$state',
        function ($scope, $rootScope, $stateParams, $location, $compile, $state) {

    var gridChequeList;
    $scope.init = function () {
        $rootScope.pageTitle("پرداخت وجه / چک");
        $('#businessNav').show();
        $scope.alert = false;
        $scope.total = 0;
        $scope.getContacts();
        $scope.getCashes();
        $scope.getBanks();
        $scope.getExpenseAccounts();
        $scope.getAccounts();
        //$scope.getDetailAccounts();

        $scope.cashId = $stateParams.cashId;// $scope.getRouteQuery($routeParams.params, 'cash');
        $scope.bankId = $stateParams.bankId;// $scope.getRouteQuery($routeParams.params, 'bank');
        $scope.rpId = $stateParams.rpId;//$scope.getRouteQuery($routeParams.params, 'id');

        $scope.receiptText = "امید محمدی";

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

        applyScope($scope);
        $(function () {
            $.getScript("/App/printReports.js", function () { });
        });
        //$('[data-toggle="tooltip"]').tooltip();
        //$.getScript("/js/app/printReports.js?ver=1.2.9.1", function () { });
    };
    $scope.getContacts = function () {
        $scope.loading = true;
        $.ajax({
            type: "POST",
            data: JSON.stringify("creditors"),
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
                matchBy: "contact.Id",
                displayProperty: "{Name}",
                searchBy: ["Name", "DetailAccount.Code"],
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
    $scope.getExpenseAccounts = function () {
        $.ajax({
            type: "POST",
            url: "/app/api/Accounting/getExpenseAccounts",
            contentType: "application/json"
        }).done(function (res) {
            var expenseAccounts = res.data;

            $scope.expenseAccounts = expenseAccounts;
            $scope.comboAccount.items = expenseAccounts;
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
            $scope.changePay('contact');
        else if (rp.Type === 3)
            $scope.changePay('expense');
        else if (rp.Type === 4)
            $scope.changePay('other');

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
                $scope.addChequePay(item, i);
                if (item.Cheque.Status !== 0)
                    $scope.disableContact = true;
            }
            else if (item.Type === 4)
                $scope.addOtherChequePay(item.Cheque, item);
        }

        $scope.calTotal();
        applyScope($scope);
    };
    $scope.addCashPay = function (item, index, newCash) {
        var newItem = !item ? Hesabfa.newReceiveAndPayItem(item) : item;
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
        var newItem = !item ? Hesabfa.newReceiveAndPayItem(item) : item;
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
    $scope.addChequePay = function (item, index, newCheque) {
        $scope.hasCheque = true;
        var newItem = !item ? Hesabfa.newReceiveAndPayItem(item) : item;
        newItem.Type = 3;
        if (newCheque) {
            newItem.Cheque = { Status: 0, Amount: 0 };
            $scope.rp.Items.push(newItem);
        }
        applyScope($scope);

        setTimeout(function () {
            if (typeof index === "undefined" || index == null)
                index = $scope.rp.Items.length - 1;
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
                searchBy: ["Name", "DetailAccount.Code"],
                onSelect: function (bank) {
                    newItem.DetailAccount = bank.DetailAccount;
                    newItem.Cheque.BankName = bank.Name;
                    newItem.Cheque.BankBranch = bank.Branch;
                    newItem.Cheque.BankDetailAccount = bank.DetailAccount;
                    //					newItem.ChequeBank = { Id: bank.Id, Name: bank.Name, DetailAccount: bank.DetailAccount };
                    if (!newItem.ChequeBank)
                        newItem.ChequeBank = bank;
                    else {
                        newItem.ChequeBank.BankId = bank.ID;
                        newItem.ChequeBank.Name = bank.Name;
                        newItem.ChequeBank.DetailAccount = bank.DetailAccount;
                    }
                    //					newItem.Bank = bank;
                    applyScope($scope);
                    var rp = $scope.rp;
                },
                onNew: function () {
                    $scope.newBank(index, newItem);
                }
            });

            if (newItem.ChequeBank)
                $scope.comboChequeBank[index].setSelected(newItem.ChequeBank);

            if ($scope.bankId) {
                var bank = findByID($scope.banks, $scope.bankId);
                newItem.DetailAccount = bank.DetailAccount;
                $scope.comboChequeBank[index].setSelected(bank);
            }
        }, 0);
    };
    $scope.openChequeList = function () {
        $('#modalChequeList').modal('show');
        $scope.loadingChequeList = true;
        gridChequeList.beginCustomLoading();

        $.ajax({
            type: "POST",
            url: "/app/api/Cheque/GetChequesToPay",
            contentType: "application/json"
        }).done(function (res) {
            var cheques = res.data;
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
        if (!newItem.Cheque) {
            newItem.Cheque = cheque;
            newItem.Amount = cheque.Amount;
        }
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
        $scope.activeRow.DetailAccount = cash.DetailAccount;
        $scope.comboCash[$scope.activeRowIndex].setSelected(cash);
    };
    $scope.getEditedBank = function (bank) {
        if (!bank) return;
        $scope.banks.push(bank);
        $scope.editBankModal = false;
        $scope.activeRow.DetailAccount = bank.DetailAccount;

        if ($scope.comboBank[$scope.activeRowIndex])
            $scope.comboBank[$scope.activeRowIndex].setSelected(bank);

        if ($scope.comboChequeBank[$scope.activeRowIndex])
            $scope.comboChequeBank[$scope.activeRowIndex].setSelected(bank);

        if ($scope.activeRow.Cheque) {
            $scope.activeRow.Cheque.BankName = bank.Name;
            $scope.activeRow.Cheque.BankBranch = bank.Branch;
            $scope.activeRow.Cheque.BankDetailAccount = bank.DetailAccount;
        }
    };
    $scope.getEditedContact = function (contact) {
        if (!contact) return;
        $scope.toContactDetailAccount = contact.DetailAccount;
        $scope.toContact = contact;
        $scope.editContactModal = false;
        $scope.comboContactSelect.setSelected(contact);
    };
    $scope.deleteRow = function (index) {
        var item = $scope.rp.Items[index];
        if (item.Cheque && item.Cheque.Status !== 0) {
            if (item.Cheque.Status !== 5) {
                alertbox({ content: "امکان حذف چک بعلت تغییر وضعیت وجود ندارد", type: "warning" });
                return;
            }
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
        $scope.toContactDetailAccount = findById.DetailAccount;
        $scope.toContact = findById;
        applyScope($scope);
    };
    $scope.accountSelect = function (account) {
        $scope.SelectedAccount = account;
        //var relatedAccounts = [];
        //for (var i = 0; i < $scope.detailAccounts.length; i++) {
        //    if ($scope.detailAccounts[i].RelatedAccounts.indexOf("," + account.Id + ",") > -1)
        //        relatedAccounts.push($scope.detailAccounts[i]);
        //}
        //		$scope.dtAccounts = relatedAccounts;
       // $scope.comboDetailAccount.items = relatedAccounts;
        $scope.comboDetailAccount.setSelected(null);
        return true;
    };
    $scope.detailAccountSelect = function (detailAccount) {
        $scope.SelectedDetailAccount = detailAccount;
    };
    $scope.bankSelect = function (bank, selectedBy, trans) {
        if (!bank) return;
        trans.Cheque.BankName = bank.Name;
        trans.Cheque.BankBranch = bank.Branch;
        trans.Cheque.BankDetailAccount = bank.DetailAccount;
    };
    $scope.changePay = function (type) {
        $scope.Pay = type;
        $scope.toContactDetailAccount = null;
        $scope.SelectedAccount = null;
        $scope.SelectedDetailAccount = null;
        $scope.comboAccount.setSelected(null);
        $scope.comboDetailAccount.setSelected(null);
        if (type === "expense") {
            $scope.comboAccount.items = $scope.expenseAccounts;
            $scope.comboDetailAccount.items = [];
            $scope.rp.Type = 3;
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
        for (var i = 0; i < transactions.length; i++) {
            total += parseFloat(transactions[i].Amount);
        }
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
            $scope.alertMessage = "بابت پرداخت باید مبلغ بیشتر از صفر باشد.";
            $scope.$apply();
            DevExpress.ui.notify("بابت پرداخت باید مبلغ بیشتر از صفر باشد ", "danger", 3000);
            return;
        }

        if ($scope.Pay == undefined) {
            $scope.calling = false;
            $scope.alert = true;
            $scope.alertType = "danger";
            $scope.alertMessage = "نوع پرداخت را مشخص کنید: بابت بدهکاری، بابت هزینه یا سایر";
            $scope.$apply();
           
            DevExpress.ui.notify("نوع پرداخت را مشخص کنید: بابت بدهکاری، بابت هزینه یا سایر ", "danger", 3000);
            return;
        }
        $scope.rp.Contact = $scope.toContact;
        $scope.rp.Account = $scope.SelectedAccount;
        $scope.rp.DetailAccount = $scope.SelectedDetailAccount;
        $scope.rp.IsReceive = false;

        console.log($scope.transactions);

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
                DevExpress.ui.notify("پرداخت با موفقیت ثبت شد. مبلغ: " + $scope.money($scope.total) + " " + $scope.getCurrency(), "success", 3000);

                if (printReceipt) $scope.printReceiptDialog();
                if (next) $scope.cleanFormForNextEntry();
                applyScope($scope);
            }).fail(function (error) {
                $scope.calling = false;
                if ($scope.accessError(error)) { applyScope($scope); return; }
                DevExpress.ui.notify(Hesabfa.farsiDigit(error), "error", 2000);
                applyScope($scope);
                window.scrollTo(0, 0);
            });
    };
    $scope.cleanFormForNextEntry = function () {
        $scope.rpId = 0;
        $scope.comboContactSelect.setSelected(null);
        $scope.comboAccount.setSelected(null);
        $scope.comboDetailAccount.setSelected(null);
        $scope.Pay = undefined;
        var rp = Hesabfa.newReceiveAndPay();
        rp.FinanYear = $scope.currentFinanYear;
        rp.DisplayDate = $scope.rp.DisplayDate;  //$scope.todayDate;
        rp.IsReceive = false;
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
                if ($scope.toContact)
                    transactions[i].Cheque.Contact = $scope.toContact;
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
        if ($scope.total) balance = ((contact.Liability + $scope.total) - contact.Credits);
        if (balance >= 0) return balance;
        else return balance * -1;
    };
    $scope.getTashkhis = function (contact) {
        if (!contact) return "";
        contact.Balance = contact.Liability - contact.Credits;
        if (contact.Balance > 0) return "بدهکار";
        else if (contact.Balance < 0) return "بستانکار";
        else return "-";
    };
    $scope.getTashkhisAfterTrans = function (contact) {
        if (!contact) return "";
        var balance = contact.Liability - contact.Credits;
        if ($scope.total) balance = ((contact.Liability + $scope.total) - contact.Credits);
        if (balance > 0) return "بدهکار";
        else if (balance < 0) return "بستانکار";
        else return "-";
    };
    $scope.getBgByBalanceState = function (tashkhis) {
        if (!tashkhis) return "";
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
        var strName = $scope.toContact ? $scope.toContact.Name : "";
        if ($scope.Receive === "income") strName = $scope.SelectedAccount ? $scope.SelectedAccount.Name : "";
        if ($scope.Receive === "other") strName = $scope.SelectedAccount ? $scope.SelectedAccount.Name : "";
        var strForWhat = $scope.rp.Invoice ? "بابت فاکتور شماره " + $scope.rp.Invoice.Number : "بابت بستانکاری ";
        $scope.receiptText = "مبلغ " + $scope.money($scope.total) + " " + $scope.getCurrency();
        $scope.receiptText += " (" + wordifyfa($scope.total, 0) + " " + $scope.getCurrency() + ") ";
        $scope.receiptText += "به آقای/خانم " + strName;
        $scope.receiptText += " " + strForWhat + " به صورت نقد/چک پرداخت گردید.";
        $scope.addTransactionListToReceipt = true;
        $('#modalPrintRecipt').modal('show');
    };


    $scope.printReceipt = function () {
       // $scope.$apply();
        $('#modalPrintRecipt').modal('hide');
        $scope.asignChequeDatesAndContact();
        var info = {};
        info.text = $('#receiptText').val();
        info.note = $('#receiptNote').val();
        info.title = $rootScope.currentBusiness.Name;
        info.subTitle = "رسید پرداخت وجه/چک";
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
        $scope.asignChequeDatesAndContact();
        var info = {};
        info.text = $('#receiptText').val();
        info.note = $('#receiptNote').val();
        info.title = $rootScope.currentBusiness.Name;
        info.subTitle = "رسید پرداخت وجه/چک";
        info.docNumber = 0;
        info.date = $scope.receiptInfo.date;
        info.showTransactions = $('#addTransactionListToReceipt').is(':checked');
        var pageSize = "A5Landscape";
        if ($('#radioA5Landscape').is(':checked')) pageSize = "A5Landscape";
        else if ($('#radioA4Portrait').is(':checked')) pageSize = "A4Portrait";
        info.settings = { pageSize: pageSize };
        info.transactions = $scope.receiptInfo.transactions;
        info.totalStr = $scope.receiptInfo.totalStr;
        pdfReceipt(info);
    };
    //
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
    }])
});