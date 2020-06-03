define(['application', 'digitGroup', 'number', 'combo', 'scrollbar', 'helper', 'editItem', 'editCash', 'editContact', 'editBank', 'nodeSelect', 'dx', 'roweditor', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup','datePicker'], function (app) {
    app.register.controller('accountsBalanceController', ['$scope','$rootScope',
        function ($scope, $rootScope) {

            var docDateObj = new AMIB.persianCalendar('docDate');
            var chequesRecDateObj = new AMIB.persianCalendar('chequesRecDate'); 
            var chequesPayDateObj = new AMIB.persianCalendar('chequesPayDate');
            var chequesInProgressDateObj = new AMIB.persianCalendar('chequesInProgressDate'); 
            var chequesPayPayeeObj = new AMIB.persianCalendar('chequesPayPayee'); 
            var chequesInProgressDepositDateObj = new AMIB.persianCalendar('chequesInProgressDepositDate');


            $scope.init = function () {
                $rootScope.pageTitle("سند افتتاحیه: مانده اول دوره حساب ها");
                $scope.alert = false;
                $('#businessNav').show();
                $scope.actRow = {};

                $scope.totalDebit = 0;
                $scope.totalCredit = 0;

                $scope.activeSection = "openingBalancePageHeader";
                $scope.olds = [];

                $scope.assets = {};
                $scope.liabilities = {};
                $scope.assets.cash = 0;             // صندوق ها
                $scope.assets.bank = 0;             // بانک ها
                $scope.assets.debtors = 0;          // بدهکاران
                $scope.assets.inventory = 0;        // موجودی کالا
                $scope.assets.receivables = 0;      // اسناد دریافتنی
                $scope.assets.inProgress = 0;       // اسناد در جریان وصول 
                $scope.assets.sum = 0;
                $scope.liabilities.creditors = 0;   // بستانکاران
                $scope.liabilities.payables = 0;    // اسناد پرداختنی
                $scope.liabilities.sum = 0;
                $scope.capital = {};
                $scope.capital.capital = 0;         // سرمایه
                $scope.capital.withdrawals = 0;     // برداشت ها
                $scope.capital.share = 0;           // سهم سود و زیان
                $scope.capital.sum = 0;

                $scope.actRow = { stock: 0, amount: 0, unitPrice: 0 };
                $scope.loadOpeningBalanceStat(); // Load Opening Balance stats
            };
            $scope.loadOpeningBalanceStat = function () {
                $scope.loading = true;
                $.ajax({
                    type:"POST",
                    url: "/app/api/Document/LoadOpeningBalanceStat",
                    contentType: "application/json"
                }).done(function (res) {
                    var stats = res.data;
                    $scope.loading = false;
                    $scope.assets.cash = stats.cash;                 // صندوق ها
                    $scope.assets.bank = stats.bank;                 // بانک ها
                    $scope.assets.debtors = stats.debtors;           // بدهکاران
                    $scope.liabilities.creditors = stats.creditors;  // بستانکاران
                    $scope.assets.receivables = stats.receivables;   // اسناد دریافتنی
                    $scope.assets.inProgress = stats.inProgress;     // اسناد در جریان وصول
                    $scope.liabilities.payables = stats.payables;    // اسناد پرداختنی
                    $scope.assets.inventory = stats.inventory;       // موجودی کالا
                    $scope.assets.otherAssets = stats.otherAssets;   // سایر دارایی ها
                    $scope.liabilities.otherLiabilities = stats.otherLiabilities;      // سایر بدهی ها
                    $scope.capital.withdrawals = stats.withdrawals;  // برداشت ها
                    $scope.capital.share = stats.share;              // سهم سود و زیان
                    $scope.docDate = stats.docDate;
                    $scope.calOpeningBalance();
                    $scope.$apply();
                }).fail(function (error) {
                            $scope.loading = false;
                        $scope.$apply();
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error });
                });
              
            };

            // بازیابی حساب های اول دوره
            $scope.loadOpeningBalanceCash = function () {
                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $("#" + $scope.activeSection).slideUp();
                $scope.loadingSection = true;
                var getOpeningBalanceCash = function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('cash'),
                        url: "/app/api/Document/LoadOpeningBalance",
                        contentType: "application/json"
                    }).done(function (res) {
                        var data = res.data;
                        $scope.loadingSection = false;
                        $scope.transObject = data.transObj;
                        $scope.olds = [];
                        $scope.transactions = [];
                        $scope.transactions = data.transactions;
                        angular.copy(data.transactions, $scope.olds);
                        $scope.setIndexRow();
                        $scope.calTotal();
                        $("#openingBalanceCash").slideDown();
                        $scope.activeSection = "openingBalanceCash";

                        if (!$scope.comboCash) {
                            $scope.comboCash = new HesabfaCombobox({
                                items: $scope.cashes,
                                containerEle: document.getElementById("cashSelect"),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: "<div> {Code} &nbsp;-&nbsp; {Name} </div>",
                                matchBy: "cash.Id",
                                displayProperty: "{Name}",
                                searchBy: ["Name", "Code"],
                                onSelect: $scope.cashSelect,
                                onNew: $scope.newCash
                            });
                        }

                        var onMoveRoweditor = function () {
                            var length = $scope.cashes.length;
                            var found = false;
                            for (var i = 0; i < length; i++) {
                                if ($scope.roweditor1.activeRow.DetailAccount && $scope.cashes[i].DetailAccount.Id === $scope.roweditor1.activeRow.DetailAccount.Id) {
                                    $scope.comboCash.setSelected($scope.cashes[i]);
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                $scope.comboCash.setSelected(null);
                            $scope.calTotal();
                        };

                        var controls = [];
                        controls.push({ id: "cashSelect", element: "tdCash", onfocus: $scope.comboCash.focus });
                        controls.push({ id: "cashAmount", element: "tdCashAmount", focus: "cashAmount" });
                        $scope.roweditor1 = new rowEditor({ controls: controls, itemArray: $scope.transactions, onMove: onMoveRoweditor, elementsContainerId: "cashElementContainer" });
                        if ($scope.transactions && $scope.transactions.length > 0)
                            $scope.roweditor1.move(0);

                        $scope.$apply();
                    }).fail(function (error) {
                        $scope.loadingSection = false;
                        $scope.$apply();
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
             
                };
                  $.ajax({
                    type:"POST",
                    url: "/app/api/Cash/GetAllByOrganId",
                    contentType: "application/json"
                }).done(function (res) {
                    var cashes = res.data;
                    $scope.cashes = cashes;
                    getOpeningBalanceCash();
                    $scope.$apply();
                }).fail(function (error) {
                    if ($scope.accessError(error)) return;
                    alertbox({ content: error, type: "error" });
                });

           

            };
            $scope.loadOpeningBalanceBank = function () {
                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $("#" + $scope.activeSection).slideUp();
                $scope.loadingSection = true;
                var getOpeningBalanceBank = function () {

                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('bank'),
                        url: "/app/api/Document/LoadOpeningBalance",
                        contentType: "application/json"
                    }).done(function (res) {
                        var data = res.data;
                        $scope.loadingSection = false;
                        $scope.transObject = data.transObj;
                        $scope.olds = [];
                        $scope.transactions = [];
                        $scope.transactions = data.transactions;
                        angular.copy(data.transactions, $scope.olds);
                        $scope.setIndexRow();
                        $scope.calTotal();
                        $("#openingBalanceBank").slideDown();
                        $scope.activeSection = "openingBalanceBank";
                        $scope.$apply();

                        if (!$scope.comboBank) {
                            $scope.comboBank = new HesabfaCombobox({
                                items: $scope.banks,
                                containerEle: document.getElementById("bankSelect"),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: Hesabfa.comboBankTemplate,
                                divider: true,
                                matchBy: "bank.DetailAccount.Id",
                                displayProperty: "{Name}",
                                searchBy: ["Name", "Code"],
                                onSelect: $scope.bankSelect,
                                onNew: $scope.newBank
                            });
                        }

                        var onMoveRoweditor = function () {
                            var length = $scope.banks.length;
                            var found = false;
                            for (var i = 0; i < length; i++) {
                                if ($scope.roweditor1.activeRow.DetailAccount && $scope.banks[i].DetailAccount.Id === $scope.roweditor1.activeRow.DetailAccount.Id) {
                                    $scope.comboBank.setSelected($scope.banks[i]);
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                $scope.comboBank.setSelected(null);
                            $scope.calTotal();
                        };

                        var controls = [];
                        controls.push({ id: "bankSelect", element: "tdBank", onfocus: $scope.comboBank.focus });
                        controls.push({ id: "bankAmount", element: "tdBankAmount", focus: "bankAmount" });
                        $scope.roweditor1 = new rowEditor({ controls: controls, itemArray: $scope.transactions, onMove: onMoveRoweditor, elementsContainerId: "bankElementContainer" });
                        if ($scope.transactions && $scope.transactions.length > 0)
                            $scope.roweditor1.move(0);

                        $scope.$apply();
                    }).fail(function (error) {
                        $scope.loadingSection = false;
                        $scope.$apply();
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
                    
                   
                };
                $.ajax({
                    type:"POST",
                    url: "/app/api/Bank/GetAllByOrganId",
                    contentType: "application/json"
                }).done(function (res) {
                    var banks = res.data;
                        $scope.banks = banks;
                        getOpeningBalanceBank();
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
            };
            $scope.loadOpeningBalanceProducts = function () {
                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $("#" + $scope.activeSection).slideUp();
                $scope.loadingSection = true;
                var getOpeningBalanceProduct = function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('product'),
                        url: "/app/api/Document/LoadOpeningBalance",
                        contentType: "application/json"
                    }).done(function (res) {
                        var data = res.data;
                        $scope.loadingSection = false;
                        $scope.transObject = data.transObj;
                        $scope.olds = [];
                        $scope.transactions = [];
                        $scope.transactions = data.transactions;
                        $scope.setIndexRow();
                        $scope.activeSection = "openingBalanceProduct";
                        $scope.calTotal();
                        $("#openingBalanceProduct").slideDown();

                        if (!$scope.comboProduct) {
                            $scope.comboProduct = new HesabfaCombobox({
                                items: $scope.items,
                                containerEle: document.getElementById("productSelect"),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: Hesabfa.comboItemTemplate,
                                matchBy: "item.DetailAccount.Id",
                                displayProperty: "{Name}",
                                searchBy: ["Name", "DetailAccount.Code"],
                                onSelect: $scope.itemSelect,
                                onNew: $scope.newItem,
                                divider: true
                            });
                        }

                        var onMoveRoweditor = function () {
                            var length = $scope.items.length;
                            var found = false;
                            for (var i = 0; i < length; i++) {
                                if ($scope.roweditor1.activeRow.DetailAccount && $scope.items[i].DetailAccount.Id === $scope.roweditor1.activeRow.DetailAccount.Id) {
                                    $scope.comboProduct.setSelected($scope.items[i]);
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                $scope.comboProduct.setSelected(null);

                            $scope.calTotal();
                        };

                        var controls = [];
                        controls.push({ id: "productSelect", element: "tdProduct", onfocus: $scope.comboProduct.focus });
                        controls.push({ id: "productStock", element: "tdProductStock", focus: "productStock" });
                        controls.push({ id: "productPrice", element: "tdProductUnitPrice", focus: "productPrice" });
                        $scope.roweditor1 = new rowEditor({ controls: controls, itemArray: $scope.transactions, onMove: onMoveRoweditor, elementsContainerId: "productElementContainer" });
                        if ($scope.transactions && $scope.transactions.length > 0)
                            $scope.roweditor1.move(0);

                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        $scope.$apply();
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
                };
                $.ajax({
                    type:"POST",
                    data: JSON.stringify('product'),
                    url: "/app/api/Item/GetAllByOrganId",
                    contentType: "application/json"
                }).done(function (res) {
                    var items = res.data;
                        $scope.items = items;
                        getOpeningBalanceProduct();
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
            };
            $scope.loadOpeningBalanceDebtors = function () {
           
                $scope.typeaccount = "debtors";
                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $("#" + $scope.activeSection).slideUp();
                $scope.loadingSection = true;
                var getOpeningBalanceDebtors = function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('debtors'),
                        url: "/app/api/Document/LoadOpeningBalance",
                        contentType: "application/json"
                    }).done(function (res) {
                        var data = res.data;
                        $scope.loadingSection = false;
                        $scope.transObject = data.transObj;
                        $scope.olds = [];
                        $scope.transactions = [];
                        $scope.transactions = data.transactions;
                        angular.copy(data.transactions, $scope.olds);
                        $scope.setIndexRow();
                        $scope.calTotal();
                        $("#openingBalanceDebtors").slideDown();
                        $scope.activeSection = "openingBalanceDebtors";

                        if (!$scope.comboDebtor) {
                            $scope.comboDebtor = new HesabfaCombobox({
                                items: $scope.contacts,
                                containerEle: document.getElementById("debtorSelect"),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: Hesabfa.comboContactTemplate,
                                divider: true,
                                matchBy: "contact.DetailAccount.Id",
                                displayProperty: "{Name}",
                                searchBy: ["Name", "Code"],
                                onSelect: $scope.debtorCreditorSelect,
                                onNew: $scope.newContact
                            });
                        }

                        var onMoveRoweditor = function () {
                            var length = $scope.contacts.length;
                            var found = false;
                            for (var i = 0; i < length; i++) {
                                if ($scope.roweditor1.activeRow.DetailAccount && $scope.contacts[i].DetailAccount.Id === $scope.roweditor1.activeRow.DetailAccount.Id) {
                                    $scope.comboDebtor.setSelected($scope.contacts[i]);
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                $scope.comboDebtor.setSelected(null);

                            $scope.calTotal();
                        };

                        var controls = [];
                        controls.push({ id: "debtorSelect", element: "tdDebtor", onfocus: $scope.comboDebtor.focus });
                        controls.push({ id: "debtorAmount", element: "tdDebtorAmount", focus: "debtorAmount" });
                        $scope.roweditor1 = new rowEditor({ controls: controls, itemArray: $scope.transactions, onMove: onMoveRoweditor, elementsContainerId: "debtorElementContainer" });
                        if ($scope.transactions && $scope.transactions.length > 0)
                            $scope.roweditor1.move(0);

                        $scope.$apply();

                    }).fail(function (error) {
                            $scope.loadingSection = false;
                            $scope.$apply();
                            if ($scope.accessError(error)) return;
                            alertbox({ content: error, type: "error" });
                        });
                };
                $.ajax({
                    type: "POST",
                    data: JSON.stringify('debtors'),
                    url: "/app/api/Contact/GetAllByOrganId",
                    contentType: "application/json"
                }).done(function (res) {
                    var contacts = res.data;
                        $scope.contacts = contacts;
                        getOpeningBalanceDebtors();
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
            };
            $scope.loadOpeningBalanceCreditors = function () {
                $scope.typeaccount = "creditors";

                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $("#" + $scope.activeSection).slideUp();
                $scope.loadingSection = true;
                var getOpeningBalanceCreditors = function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('creditors'),
                        url: "/app/api/Document/LoadOpeningBalance",
                        contentType: "application/json"
                    }).done(function (res) {
                        var data = res.data;
                            $scope.loadingSection = false;
                            $scope.transObject = data.transObj;
                            $scope.olds = [];
                            $scope.transactions = [];
                            $scope.transactions = data.transactions;
                            angular.copy(data.transactions, $scope.olds);
                            $scope.setIndexRow();
                            $scope.calTotal();
                            $("#openingBalanceCreditors").slideDown();
                            $scope.activeSection = "openingBalanceCreditors";

                            if (!$scope.comboCreditor) {
                                $scope.comboCreditor = new HesabfaCombobox({
                                    items: $scope.contacts,
                                    containerEle: document.getElementById("creditorSelect"),
                                    toggleBtn: true,
                                    newBtn: true,
                                    deleteBtn: true,
                                    itemClass: "hesabfa-combobox-item",
                                    activeItemClass: "hesabfa-combobox-activeitem",
                                    itemTemplate: Hesabfa.comboContactTemplate,
                                    divider: true,
                                    matchBy: "contact.DetailAccount.Id",
                                    displayProperty: "{Name}",
                                    searchBy: ["Name", "Code"],
                                    onSelect: $scope.debtorCreditorSelect,
                                    onNew: $scope.newContact
                                });
                            }

                            var onMoveRoweditor = function () {
                                var length = $scope.contacts.length;
                                var found = false;
                                for (var i = 0; i < length; i++) {
                                    if ($scope.roweditor1.activeRow.DetailAccount && $scope.contacts[i].DetailAccount.Id === $scope.roweditor1.activeRow.DetailAccount.Id) {
                                        $scope.comboCreditor.setSelected($scope.contacts[i]);
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                    $scope.comboCreditor.setSelected(null);
                                $scope.calTotal();
                            };

                            var controls = [];
                            controls.push({ id: "creditorSelect", element: "tdCreditor", onfocus: $scope.comboCreditor.focus });
                            controls.push({ id: "creditorAmount", element: "tdCreditorAmount", focus: "creditorAmount" });
                            $scope.roweditor1 = new rowEditor({ controls: controls, itemArray: $scope.transactions, onMove: onMoveRoweditor, elementsContainerId: "creditorElementContainer" });
                            if ($scope.transactions && $scope.transactions.length > 0)
                                $scope.roweditor1.move(0);

                            $scope.$apply();
                        }).fail(function (error) {
                            $scope.loadingSection = false;
                            $scope.$apply();
                            if ($scope.accessError(error)) return;
                            alertbox({ content: error, type: "error" });
                        });
                };
                $.ajax({
                    type: "POST",
                    data: JSON.stringify('creditors'),
                    url: "/app/api/Contact/GetAllByOrganId",
                    contentType: "application/json"
                }).done(function (res) {
                    var contacts = res.data;
                        $scope.contacts = contacts;
                        getOpeningBalanceCreditors();
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
            };
            $scope.loadOpeningBalanceChequesPay = function () {
                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $("#" + $scope.activeSection).slideUp();
                $scope.loadingSection = true;
                var getOpeningBalanceChequesPay = function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('payables'),
                        url: "/app/api/Document/LoadOpeningBalance",
                        contentType: "application/json"
                    }).done(function (res) {
                        var data = res.data;
                            $scope.loadingSection = false;
                            $scope.transObject = data.transObj;
                            $scope.olds = [];
                            $scope.transactions = [];
                            $scope.transactions = data.transactions;
                            var length = data.transactions.length;
                            for (var k = 0; k < length; k++) {
                                var trans = $scope.transactions[k];
                                if (!trans.Cheque)
                                    trans.Cheque = { DisplayDate: "" };
                                if (trans.Cheque && trans.Cheque.Id > 0 && trans.Cheque.Status !== 0)
                                    trans.disabled = true;
                            }
                            $scope.setIndexRow();
                            $scope.calTotal();
                            $("#OpeningBalanceChequesPay").slideDown();
                            $scope.activeSection = "OpeningBalanceChequesPay";

                            if (!$scope.comboChequePayContact) {
                                $scope.comboChequePayContact = new HesabfaCombobox({
                                    items: $scope.contacts,
                                    containerEle: document.getElementById("chequesPayContactSelect"),
                                    toggleBtn: true,
                                    newBtn: true,
                                    deleteBtn: true,
                                    itemClass: "hesabfa-combobox-item",
                                    activeItemClass: "hesabfa-combobox-activeitem",
                                    itemTemplate: Hesabfa.comboContactTemplate,
                                    divider: true,
                                    matchBy: "contact.DetailAccount.Id",
                                    displayProperty: "{Name}",
                                    searchBy: ["Name", "Code"],
                                    onSelect: $scope.chequePayContactSelect,
                                    onNew: $scope.newContact
                                });
                            }
                            if (!$scope.comboChequePayBank) {
                                $scope.comboChequePayBank = new HesabfaCombobox({
                                    items: $scope.banks,
                                    containerEle: document.getElementById("chequesPayBankSelect"),
                                    toggleBtn: true,
                                    newBtn: true,
                                    deleteBtn: true,
                                    itemClass: "hesabfa-combobox-item",
                                    activeItemClass: "hesabfa-combobox-activeitem",
                                    itemTemplate: Hesabfa.comboBankTemplate,
                                    divider: true,
                                    matchBy: "bank.DetailAccount.Id",
                                    displayProperty: "{Name}",
                                    searchBy: ["Name", "Code"],
                                    onSelect: $scope.chequePayBankSelect,
                                    onNew: $scope.newBank
                                });
                            }

                            var onMoveRoweditor = function () {
                                var banksCount = $scope.banks.length;
                                var contactsCount = $scope.contacts.length;
                                var foundBank = false;
                                var foundContact = false;
                                for (var i = 0; i < banksCount; i++) {
                                    if ($scope.roweditor1.activeRow.DetailAccount && $scope.banks[i].DetailAccount.Id === $scope.roweditor1.activeRow.DetailAccount.Id) {
                                        $scope.comboChequePayBank.setSelected($scope.banks[i]);
                                        foundBank = true;
                                        break;
                                    }
                                }
                                for (var j = 0; j < contactsCount; j++) {
                                    if ($scope.roweditor1.activeRow.Cheque.Contact && $scope.contacts[j].ID === $scope.roweditor1.activeRow.Cheque.Contact.ID) {
                                        $scope.comboChequePayContact.setSelected($scope.contacts[j]);
                                        foundContact = true;
                                        break;
                                    }
                                }
                                if (!foundBank)
                                    $scope.comboChequePayBank.setSelected(null);
                                if (!foundContact)
                                    $scope.comboChequePayContact.setSelected(null);
                                $scope.calTotal();
                            };

                            var controls = [];
                            controls.push({ id: "chequesPayContactSelect", element: "tdChequePayContact", onfocus: $scope.comboChequePayContact.focus });
                            controls.push({ id: "chequesPayBankSelect", element: "tdChequePayBank", onfocus: $scope.comboChequePayBank.focus });
                            controls.push({ id: "chequesPayChequeNumber", element: "tdChequePayChequeNumber", focus: "chequesPayChequeNumber" });
                            controls.push({ id: "chequesPayAmount", element: "tdChequePayAmount", focus: "chequesPayAmount" });
                            controls.push({ id: "chequesPayDate", element: "tdChequePayChequeDate", focus: "chequesPayDate" });
                            controls.push({ id: "chequesPayPayee", element: "tdChequePayChequePayee", focus: "chequesPayPayee" });
                            $scope.roweditor1 = new rowEditor({ controls: controls, itemArray: $scope.transactions, onMove: onMoveRoweditor, elementsContainerId: "chequesPayElementContainer" });
                            if ($scope.transactions && $scope.transactions.length > 0)
                                $scope.roweditor1.move(0);
                            applyScope($scope);
                        }).fail(function (error) {
                            $scope.loadingSection = false;
                            $scope.$apply();
                            if ($scope.accessError(error)) return;
                            alertbox({ content: error, type: "error" });
                        });
                };
                var getContacts = function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('creditors'),
                        url: "/app/api/Contact/GetAllByOrganId",
                        contentType: "application/json"
                    }).done(function (res) {
                        var contacts = res.data;
                            $scope.contacts = contacts;
                            getOpeningBalanceChequesPay();
                            $scope.$apply();
                        }).fail(function (error) {
                            if ($scope.accessError(error)) return;
                            alertbox({ content: error, type: "error" });
                        });
                };
                $.ajax({
                    type: "POST",
                    data: JSON.stringify('payables'),
                    url: "/app/api/Bank/GetAllByOrganIdAndType",
                    contentType: "application/json"
                }).done(function (res) {
                var banks = res.data;
                        $scope.banks = banks;
                        getContacts();
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
            };
            $scope.loadOpeningBalanceChequesRec = function () {
                $scope.typeaccount = "receivables";

                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $("#" + $scope.activeSection).slideUp();
                $scope.loadingSection = true;
                var getOpeningBalanceChequesRec = function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('receivables'),
                        url: "/app/api/Document/LoadOpeningBalance",
                        contentType: "application/json"
                    }).done(function (res) {
                        var data = res.data;
                            $scope.loadingSection = false;
                            $scope.transObject = data.transObj;
                            $scope.olds = [];
                            $scope.transactions = [];
                            $scope.transactions = data.transactions;
                            var length = data.transactions.length;
                            for (var k = 0; k < length; k++) {
                                var trans = $scope.transactions[k];
                                if (!trans.Cheque)
                                    trans.Cheque = { DisplayDate: "" };
                                if (trans.Cheque && trans.Cheque.Id > 0 && trans.Cheque.Status !== 0)
                                    trans.disabled = true;
                            }
                            $scope.setIndexRow();
                            $scope.calTotal();
                            $("#OpeningBalanceChequesRec").slideDown();
                            $scope.activeSection = "OpeningBalanceChequesRec";

                            if (!$scope.comboChequeRecContact) {
                                $scope.comboChequeRecContact = new HesabfaCombobox({
                                    items: $scope.contacts,
                                    containerEle: document.getElementById("chequesRecContactSelect"),
                                    toggleBtn: true,
                                    newBtn: true,
                                    deleteBtn: true,
                                    itemClass: "hesabfa-combobox-item",
                                    activeItemClass: "hesabfa-combobox-activeitem",
                                    itemTemplate: Hesabfa.comboContactTemplate,
                                    divider: true,
                                    matchBy: "contact.DetailAccount.Id",
                                    displayProperty: "{Name}",
                                    searchBy: ["Name", "Code"],
                                    onSelect: $scope.chequeRecContactSelect,
                                    onNew: $scope.newContact

                                });
                            }

                            var onMoveRoweditor = function () {
                                var contactsCount = $scope.contacts.length;
                                var foundContact = false;
                                for (var j = 0; j < contactsCount; j++) {
                                    if ($scope.roweditor1.activeRow.Cheque.Contact && $scope.contacts[j].Id === $scope.roweditor1.activeRow.Cheque.Contact.Id) {
                                        $scope.comboChequeRecContact.setSelected($scope.contacts[j]);
                                        foundContact = true;
                                        break;
                                    }
                                }
                                if (!foundContact)
                                    $scope.comboChequeRecContact.setSelected(null);
                                $scope.calTotal();
                            };

                            var controls = [];
                            controls.push({ id: "chequesRecContactSelect", element: "tdChequeRecContact", onfocus: $scope.comboChequeRecContact.focus });
                            controls.push({ id: "chequesRecChequeNumber", element: "tdChequeRecChequeNumber", focus: "chequesRecChequeNumber" });
                            controls.push({ id: "chequesRecAmount", element: "tdChequeRecAmount", focus: "chequesRecAmount" });
                            controls.push({ id: "chequesRecBankName", element: "tdChequeRecBankName", focus: "chequesRecBankName" });
                            controls.push({ id: "chequesRecBankBranch", element: "tdChequeRecBankBranch", focus: "chequesRecBankBranch" });
                            controls.push({ id: "chequesRecDate", element: "tdChequeRecChequeDate", focus: "chequesRecDate" });
                            $scope.roweditor1 = new rowEditor({ controls: controls, itemArray: $scope.transactions, onMove: onMoveRoweditor, elementsContainerId: "chequesRecElementContainer" });
                            if ($scope.transactions && $scope.transactions.length > 0)
                                $scope.roweditor1.move(0);

                            applyScope($scope);
                        }).fail(function (error) {
                            $scope.loadingSection = false;
                            $scope.$apply();
                            if ($scope.accessError(error)) return;
                            alertbox({ content: error, type: "error" });
                        });
                };
                $.ajax({
                    type: "POST",
                    data: JSON.stringify('receivables'),
                    url: "/app/api/Contact/GetAllByOrganId",
                    contentType: "application/json"
                }).done(function (res) {
                    var contacts = res.data;
                        $scope.contacts = contacts;
                        getOpeningBalanceChequesRec();
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
            };
            $scope.loadOpeningBalanceChequesInProgress = function () {

                $scope.typeaccount = "inProgress";

                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $("#" + $scope.activeSection).slideUp();
                $scope.loadingSection = true;
                var getOpeningBalanceChequesInProgress = function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('inProgress'),
                        url: "/app/api/Document/LoadOpeningBalance",
                        contentType: "application/json"
                    }).done(function (res) {
                        var data = res.data;
                            $scope.loadingSection = false;
                            $scope.transObject = data.transObj;
                            $scope.olds = [];
                            $scope.transactions = [];
                            $scope.transactions = data.transactions;
                            var length = data.transactions.length;
                            for (var k = 0; k < length; k++) {
                                var trans = $scope.transactions[k];
                                if (!trans.Cheque)
                                    trans.Cheque = { DisplayDate: "" };
                                if (trans.Cheque && trans.Cheque.Id > 0 && trans.Cheque.Status !== 1)
                                    trans.disabled = true;
                            }
                            $scope.setIndexRow();
                            $scope.calTotal();
                            $("#OpeningBalanceChequesInProgress").slideDown();
                            $scope.activeSection = "OpeningBalanceChequesInProgress";

                            if (!$scope.comboChequeInProgressContact) {
                                $scope.comboChequeInProgressContact = new HesabfaCombobox({
                                    items: $scope.contacts,
                                    containerEle: document.getElementById("chequesInProgressContactSelect"),
                                    toggleBtn: true,
                                    newBtn: true,
                                    deleteBtn: true,
                                    itemClass: "hesabfa-combobox-item",
                                    activeItemClass: "hesabfa-combobox-activeitem",
                                    itemTemplate: Hesabfa.comboContactTemplate,
                                    matchBy: "contact.DetailAccount.Id",
                                    displayProperty: "{Name}",
                                    searchBy: ["Name", "Code"],
                                    onSelect: $scope.chequeInProgressContactSelect,
                                    onNew: $scope.newContact,
                                    onDelete: function () {
                                        $scope.roweditor1.activeRow.Cheque.Contact = null;
                                        $scope.roweditor1.activeRow.Cheque.ContactDetailAccount = null;
                                        applyScope($scope);
                                    }
                                });
                            }
                            if (!$scope.comboChequeInProgressBank) {
                                $scope.comboChequeInProgressBank = new HesabfaCombobox({
                                    items: $scope.banks,
                                    containerEle: document.getElementById("chequesInProgressBankSelect"),
                                    toggleBtn: true,
                                    newBtn: true,
                                    deleteBtn: true,
                                    itemClass: "hesabfa-combobox-item",
                                    activeItemClass: "hesabfa-combobox-activeitem",
                                    itemTemplate: Hesabfa.comboBankTemplate,
                                    divider: true,
                                    matchBy: "bank.DetailAccount.Id",
                                    displayProperty: "{Name}",
                                    searchBy: ["Name", "Code"],
                                    onSelect: $scope.chequeInProgressBankSelect,
                                    onNew: $scope.newBank,
                                    onDelete: function () {
                                        $scope.roweditor1.activeRow.DetailAccount = null;
                                        $scope.roweditor1.activeRow.Cheque.DepositBank = null;
                                        applyScope($scope);
                                    }
                                });
                            }

                            var onMoveRoweditor = function () {
                                var banksCount = $scope.banks.length;
                                var contactsCount = $scope.contacts.length;
                                var foundBank = false;
                                var foundContact = false;
                                for (var i = 0; i < banksCount; i++) {
                                    if ($scope.roweditor1.activeRow.DetailAccount && $scope.banks[i].DetailAccount.Id === $scope.roweditor1.activeRow.DetailAccount.Id) {
                                        $scope.comboChequeInProgressBank.setSelected($scope.banks[i]);
                                        foundBank = true;
                                        break;
                                    }
                                }
                                for (var j = 0; j < contactsCount; j++) {
                                    if ($scope.roweditor1.activeRow.Cheque.Contact && $scope.contacts[j].Id === $scope.roweditor1.activeRow.Cheque.Contact.Id) {
                                        $scope.comboChequeInProgressContact.setSelected($scope.contacts[j]);
                                        foundContact = true;
                                        break;
                                    }
                                }
                                if (!foundBank)
                                    $scope.comboChequeInProgressBank.setSelected(null);
                                if (!foundContact)
                                    $scope.comboChequeInProgressContact.setSelected(null);
                                $scope.calTotal();
                            };

                            var controls = [];
                            controls.push({ id: "chequesInProgressContactSelect", element: "tdChequeInProgressContact", onfocus: $scope.comboChequeInProgressContact.focus });
                            controls.push({ id: "chequesInProgressChequeNumber", element: "tdChequeInProgressChequeNumber", focus: "chequesInProgressChequeNumber" });
                            controls.push({ id: "chequesInProgressAmount", element: "tdChequeInProgressAmount", focus: "chequesInProgressAmount" });
                            controls.push({ id: "chequesInProgressBankName", element: "tdChequeInProgressBankName", focus: "chequesInProgressBankName" });
                            controls.push({ id: "chequesInProgressBankBranch", element: "tdChequeInProgressBankBranch", focus: "chequesInProgressBankBranch" });
                            controls.push({ id: "chequesInProgressDate", element: "tdChequeInProgressChequeDate", focus: "chequesInProgressDate" });
                            controls.push({ id: "chequesInProgressBankSelect", element: "tdChequeInProgressDepositBank", onfocus: $scope.comboChequeInProgressBank.focus });
                            controls.push({ id: "chequesInProgressDepositDate", element: "tdChequeInProgressDepositDate", focus: "chequesInProgressDepositDate" });
                            $scope.roweditor1 = new rowEditor({ controls: controls, itemArray: $scope.transactions, onMove: onMoveRoweditor, elementsContainerId: "chequesInProgressElementContainer" });
                            if ($scope.transactions && $scope.transactions.length > 0)
                                $scope.roweditor1.move(0);

                            applyScope($scope);
                        }).fail(function (error) {
                            $scope.loadingSection = false;
                            $scope.$apply();
                            if ($scope.accessError(error)) return;
                            alertbox({ content: error, type: "error" });
                        });
                };
                $.ajax({
                    type: "POST",
                    data: JSON.stringify('inProgress'),
                    url: "/app/api/Contact/GetAllByOrganId",
                    contentType: "application/json"
                }).done(function (res) {
                    var contacts = res.data;
                        $scope.contacts = contacts;
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('inProgress'),
                        url: "/app/api/Bank/GetAllByOrganIdAndType",
                        contentType: "application/json"
                    }).done(function (res) {
                        var banks = res.data;
                                $scope.banks = banks;
                                getOpeningBalanceChequesInProgress();
                                $scope.$apply();
                            }).fail(function (error) {
                                if ($scope.accessError(error)) return;
                                alertbox({ content: error, type: "error" });
                            });
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
            };
            $scope.loadOpeningBalanceOtherAssets = function () {
                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $("#" + $scope.activeSection).hide();
                $scope.loadingSection = true;
                var getOpeningBalanceOtherAssets = function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('assets'),
                        url: "/app/api/Document/LoadOpeningBalance",
                        contentType: "application/json"
                    }).done(function (res) {
                        var data = res.data;
                            $scope.loadingSection = false;
                            $scope.transObject = data.transObj;
                            $scope.olds = [];
                            $scope.transactions = [];
                            $scope.transactions = data.transactions;
                            angular.copy(data.transactions, $scope.olds);
                            $scope.calTotal();

                            if (!$scope.otherAssetsComboAccount) {
                                $scope.otherAssetsComboAccount = new HesabfaCombobox({
                                    items: $scope.accounts,
                                    containerEle: document.getElementById("otherAssetAccountSelect"),
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
                            }
                            if (!$scope.otherAssetsComboDetailAccount) {
                                $scope.otherAssetsComboDetailAccount = new HesabfaCombobox({
                                    items: [],
                                    containerEle: document.getElementById("otherAssetDetailAccountSelect"),
                                    toggleBtn: true,
                                    deleteBtn: true,
                                    itemClass: "hesabfa-combobox-item",
                                    activeItemClass: "hesabfa-combobox-activeitem",
                                    itemTemplate: "<div> {Coding} &nbsp;-&nbsp; {Name} </div>",
                                    matchBy: "detailAccount.Id",
                                    displayProperty: "{Name}",
                                    searchBy: ["Name", "Coding"],
                                    onSelect: $scope.detailAccountSelect
                                });
                            }

                            var onMoveRoweditor = function () {
                                $scope.otherAssetsComboAccount.setSelected($scope.roweditor1.activeRow.Account);
                                $scope.otherAssetsComboDetailAccount.setSelected($scope.roweditor1.activeRow.DetailAccount);
                                $scope.loadAccountDetailAccounts($scope.roweditor1.activeRow.Account);
                            };

                            $scope.setIndexRow();
                            var controls = [];
                            controls.push({ id: "otherAssetAccountSelect", element: "tdAccount", onfocus: $scope.otherAssetsComboAccount.focus });
                            controls.push({ id: "otherAssetDetailAccountSelect", element: "tdDetailAccount", onfocus: $scope.otherAssetsComboDetailAccount.focus });
                            controls.push({ id: "otherAssetAmount", element: "tdAmount", focus: "otherAssetAmount" });
                            $scope.roweditor1 = new rowEditor({ controls: controls, itemArray: $scope.transactions, onMove: onMoveRoweditor });
                            if ($scope.transactions && $scope.transactions.length > 0)
                                $scope.roweditor1.move(0);

                            $("#openingBalanceOtherAssets").show();
                            $scope.activeSection = "openingBalanceOtherAssets";
                            $scope.$apply();
                        }).fail(function (error) {
                            $scope.loadingSection = false;
                            $scope.$apply();
                            if ($scope.accessError(error)) return;
                            alertbox({ content: error, type: "error" });
                        });
                };
                $.ajax({
                    type: "POST",
                    data: JSON.stringify('1'),
                    url: "/app/api/Accounting/GetOtherAccountsByGroup4OpeningBalance",
                    contentType: "application/json"
                }).done(function (res) {
                    var accounts = res.data;
                        $scope.accounts = accounts;
                        getOpeningBalanceOtherAssets();
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
            };
            $scope.loadOpeningBalanceOtherLiabilities = function () {
                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $("#" + $scope.activeSection).slideUp();
                $scope.loadingSection = true;
                var getOpeningBalanceOtherLibilities = function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('liabilities'),
                        url: "/app/api/Document/LoadOpeningBalance",
                        contentType: "application/json"
                    }).done(function (res) {
                        var data = res.data;
                            $scope.loadingSection = false;
                            $scope.transObject = data.transObj;
                            $scope.olds = [];
                            $scope.transactions = [];
                            $scope.transactions = data.transactions;
                            angular.copy(data.transactions, $scope.olds);
                            $scope.calTotal();

                            if (!$scope.otherLiabilityComboAccount) {
                                $scope.otherLiabilityComboAccount = new HesabfaCombobox({
                                    items: $scope.accounts,
                                    containerEle: document.getElementById("otherLiabilityAccountSelect"),
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
                            }
                            if (!$scope.otherLiabilityComboDetailAccount) {
                                $scope.otherLiabilityComboDetailAccount = new HesabfaCombobox({
                                    items: [],
                                    containerEle: document.getElementById("otherLiabilityDetailAccountSelect"),
                                    toggleBtn: true,
                                    deleteBtn: true,
                                    itemClass: "hesabfa-combobox-item",
                                    activeItemClass: "hesabfa-combobox-activeitem",
                                    itemTemplate: "<div> {Coding} &nbsp;-&nbsp; {Name} </div>",
                                    matchBy: "detailAccount.Id",
                                    displayProperty: "{Name}",
                                    searchBy: ["Name", "Coding"],
                                    onSelect: $scope.detailAccountSelect
                                });
                            }

                            var onMoveRoweditor = function () {
                                $scope.otherLiabilityComboAccount.setSelected($scope.roweditor1.activeRow.Account);
                                $scope.otherLiabilityComboDetailAccount.setSelected($scope.roweditor1.activeRow.DetailAccount);
                                $scope.loadAccountDetailAccounts($scope.roweditor1.activeRow.Account);
                            };

                            $scope.setIndexRow();
                            var controls = [];
                            controls.push({ id: "otherLiabilityAccountSelect", element: "tdOtherLiabilityAccount", onfocus: $scope.otherLiabilityComboAccount.focus });
                            controls.push({ id: "otherLiabilityDetailAccountSelect", element: "tdOtherLiabilityDetailAccount", onfocus: $scope.otherLiabilityComboDetailAccount.focus });
                            controls.push({ id: "otherLiabilityAmount", element: "tdOtherLiabilityAmount", focus: "otherLiabilityAmount" });
                            $scope.roweditor1 = new rowEditor({ controls: controls, itemArray: $scope.transactions, onMove: onMoveRoweditor, elementsContainerId: "otherLiabilitesElementContainer" });
                            if ($scope.transactions && $scope.transactions.length > 0)
                                $scope.roweditor1.move(0);

                            $("#openingBalanceOtherLiabilities").slideDown();
                            $scope.activeSection = "openingBalanceOtherLiabilities";
                            $scope.$apply();
                        }).fail(function (error) {
                            $scope.loadingSection = false;
                            $scope.$apply();
                            if ($scope.accessError(error)) return;
                            alertbox({ content: error, type: "error" });
                        });
                };
                $.ajax({
                    type: "POST",
                    data: JSON.stringify('2'),
                    url: "/app/api/Accounting/GetOtherAccountsByGroup4OpeningBalance",
                    contentType: "application/json"
                }).done(function (res) {
                    var accounts = res.data;
                        $scope.accounts = accounts;
                        getOpeningBalanceOtherLibilities();
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
            };
            $scope.loadOpeningBalanceWithdrawals = function () {
                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $("#" + $scope.activeSection).slideUp();
                $scope.loadingSection = true;
                var getOpeningBalanceWithdrawals = function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('withdrawal'),
                        url: "/app/api/Document/LoadOpeningBalance",
                        contentType: "application/json"
                    }).done(function (res) {
                        var data = res.data;
                            $scope.loadingSection = false;
                            $scope.transObject = data.transObj;
                            $scope.olds = [];
                            $scope.transactions = [];
                            $scope.transactions = data.transactions;
                            angular.copy(data.transactions, $scope.olds);
                            $scope.calTotal();
                            $("#openingBalanceWithdrawals").slideDown();
                            $scope.activeSection = "openingBalanceWithdrawals";
                            $scope.$apply();
                        }).fail(function (error) {
                            $scope.loadingSection = false;
                            $scope.$apply();
                            if ($scope.accessError(error)) return;
                            window.location = '/error.html';
                        });
                };
                callws(DefaultUrl.MainWebService + 'GetShareholders', {})
                    .success(function (data) {
                        $scope.shareholders = data.shareholders;
                        getOpeningBalanceWithdrawals();
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        window.location = '/error.html';
                    }).loginFail(function () {
                        alertbox({ content: error, type: "error" });
                    });
            };
            $scope.loadOpeningBalanceShares = function () {
                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $("#" + $scope.activeSection).slideUp();
                $scope.loadingSection = true;
                var getOpeningBalanceShares = function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify('share'),
                        url: "/app/api/Document/LoadOpeningBalance",
                        contentType: "application/json"
                    }).done(function (res) {
                        var data = res.data;
                            $scope.loadingSection = false;
                            $scope.transObject = data.transObj;
                            $scope.olds = [];
                            $scope.transactions = [];
                            $scope.transactions = data.transactions;
                            angular.copy(data.transactions, $scope.olds);
                            $scope.calTotal();
                            $("#openingBalanceShare").slideDown();
                            $scope.activeSection = "openingBalanceShare";
                            $scope.$apply();
                        }).fail(function (error) {
                            $scope.loadingSection = false;
                            $scope.$apply();
                            if ($scope.accessError(error)) return;
                            alertbox({ content: error, type: "error" });
                        });
                };
                callws(DefaultUrl.MainWebService + 'GetShareholders', {})
                    .success(function (data) {
                        $scope.shareholders = data.shareholders;
                        getOpeningBalanceShares();
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };

            $scope.chequePayDateChange = function () {
                $scope.roweditor1.activeRow.Cheque.DisplayDate = $("#chequesPayDate").val();
            };
            $scope.chequeRecDateChange = function () {
                $scope.roweditor1.activeRow.Cheque.DisplayDate = $("#chequesRecDate").val();
            };
            $scope.chequeInProgressDateDateChange = function () {
                $scope.roweditor1.activeRow.Cheque.DisplayDate = $("#chequesInProgressDate").val();
            };
            $scope.chequeInProgressDepositDateChange = function () {
                $scope.roweditor1.activeRow.Cheque.DepositDate = $("#chequesInProgressDepositDate").val();
            };
            $scope.asignChequeDates = function () {
                var transactions = $scope.transactions;
                var prefix = "";
                var prefix2 = "";
                if ($scope.activeSection === "OpeningBalanceChequesPay")
                    prefix = "dChqPay";
                else if ($scope.activeSection === "OpeningBalanceChequesRec")
                    prefix = "dChqRec";
                else if ($scope.activeSection === "OpeningBalanceChequesInProgress")
                { prefix = "dChqInProg1"; prefix2 = "dChqInProg2"; }
                for (var i = 0; i < transactions.length; i++) {
                    if (!transactions[i].Cheque) continue;
                    transactions[i].Cheque.DisplayDate = $('#' + prefix + i).val();
                    if ($scope.activeSection === "OpeningBalanceChequesInProgress")
                        transactions[i].Cheque.DepositDate = $('#' + prefix2 + i).val();
                }
            };
            $scope.moveRowEditor = function (index) {
                $scope.roweditor1.move(index);
            };
            $scope.getActiveRowEditorIndex = function () {
                return !$scope.roweditor1 ? -1 : $scope.roweditor1.activeRow.rowIndex;
            };
            $scope.loadAccountDetailAccounts = function (account) {
                if (!account) {
                    if ($scope.otherAssetsComboDetailAccount)
                        $scope.otherAssetsComboDetailAccount.items = [];
                    if ($scope.otherLiabilityComboDetailAccount)
                        $scope.otherLiabilityComboDetailAccount.items = [];
                    return;
                }

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(account),
                    url: "/app/api/Accounting/GetAccountDetailAccounts",
                    contentType: "application/json"
                }).done(function (res) {
                    var detailAccounts = res.data;
                        if ($scope.otherAssetsComboDetailAccount)
                            $scope.otherAssetsComboDetailAccount.items = detailAccounts;
                        if ($scope.otherLiabilityComboDetailAccount)
                            $scope.otherLiabilityComboDetailAccount.items = detailAccounts;
                        $scope.$apply();
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                    });
            };

            $scope.setIndexRow = function () {
                if ($scope.transactions) {
                    for (var j = 0; j < $scope.transactions.length; j++) {
                        $scope.transactions[j].rowIndex = j;
                        if (!$scope.transactions[j].unitPrice)
                            $scope.transactions[j].unitPrice = 0;
                    }
                }
            };

            // ثبت و ذخیره حساب های اول دوره
            $scope.submitOpeningBalanceCash = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                if (!$scope.validateDocDate()) return;
                $scope.calling = true;
                var model ={
                    transactions: $scope.transactions, 
                    systemAccount: 'cash', 
                    docDate: $scope.docDate 

                };

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Document/SubmitOpeningBalance",
                    contentType: "application/json"
                }).done(function (res) {
                    var sumCash = res.data;
                        $scope.calling = false;
                        $scope.assets.cash = sumCash;
                        $scope.calTotal();
                        $scope.calOpeningBalance();
                        if ($scope.roweditor1)
                            $scope.roweditor1.hide();
                        $("#" + $scope.activeSection).slideUp("slow", function () {
                            $("#openingBalancePageHeader").slideDown();
                            $scope.activeSection = "openingBalancePageHeader";
                        });
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                    });
            };
            $scope.submitOpeningBalanceBank = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                if (!$scope.validateDocDate()) return;
                $scope.calling = true;
                var model ={
                    transactions: $scope.transactions, 
                    systemAccount: 'bank', 
                    docDate: $scope.docDate 

                };

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Document/SubmitOpeningBalance",
                    contentType: "application/json"
                }).done(function (res) {
                    var sumBank = res.data;
                        $scope.calling = false;
                        $scope.assets.bank = sumBank;
                        $scope.calTotal();
                        $scope.calOpeningBalance();
                        if ($scope.roweditor1)
                            $scope.roweditor1.hide();
                        $("#" + $scope.activeSection).slideUp("slow", function () {
                            $("#openingBalancePageHeader").slideDown();
                            $scope.activeSection = "openingBalancePageHeader";
                        });
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                    });
            };
            $scope.submitOpeningBalanceItems = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                $scope.calTotal();
                var items = [];

               
                for (var i = 0; i < $scope.transactions.length; i++) {
                    
                        var itemInfo = {
                            stock: $scope.transactions[i].Stock,
                            totalAmount: $scope.transactions[i].Stock * $scope.transactions[i].UnitPrice
                        }
                        for (var j = 0; j < $scope.items.length; j++) {
                            if ($scope.transactions[i].DetailAccount && $scope.items[j].DetailAccount.Id === $scope.transactions[i].DetailAccount.Id) {
                                itemInfo.Id = $scope.items[j].ID;
                                break;
                            }
                        }
                        if (!itemInfo.Id)
                            continue;
                        items.push(itemInfo);
                    
                }

                if (!$scope.validateDocDate()) return;
                $scope.calling = true;

                $scope.calling = true;

                var model ={
                    items: items, 
                    docDate: $scope.docDate 

                };

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Document/SubmitOpeningBalanceItems",
                    contentType: "application/json"
                }).done(function (res) {
                    var sumInventory = res.data;

                        $scope.calling = false;
                        $scope.assets.inventory = sumInventory;
                        $scope.calTotalProducts();
                        $scope.calOpeningBalance();


                        if ($scope.roweditor1)
                            $scope.roweditor1.hide();
                        $("#" + $scope.activeSection).slideUp("slow", function () {
                            $("#openingBalancePageHeader").slideDown();
                            $scope.activeSection = "openingBalancePageHeader";
                        });
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                        return;
                    });
            };
            $scope.submitOpeningBalanceProducts = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                if (!$scope.validateDocDate()) return;
                $scope.calling = true;
                callws(DefaultUrl.MainWebService + "SubmitOpeningBalance", { transactions: $scope.transactions, systemAccount: "product", docDate: $scope.docDate })
                    .success(function (sumInventory) {
                        $scope.calling = false;
                        $scope.assets.inventory = sumInventory;
                        $scope.calTotalProducts();
                        $scope.calOpeningBalance();
                        $("#" + $scope.activeSection).slideUp("slow", function () {
                            $("#openingBalancePageHeader").slideDown();
                            $scope.activeSection = "openingBalancePageHeader";
                        });
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
            $scope.submitOpeningBalanceDebtors = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                if (!$scope.validateDocDate()) return;
                $scope.calling = true;
                       var model ={
                    transactions: $scope.transactions, 
                    systemAccount: 'debtors',
                    docDate: $scope.docDate 

                };

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Document/SubmitOpeningBalance",
                    contentType: "application/json"
                }).done(function (res) {
                    var sumDebtors = res.data;
               
                        $scope.calling = false;
                        $scope.assets.debtors = sumDebtors;
                        $scope.calTotal();
                        $scope.calOpeningBalance();
                        if ($scope.roweditor1)
                            $scope.roweditor1.hide();
                        $("#" + $scope.activeSection).slideUp("slow", function () {
                            $("#openingBalancePageHeader").slideDown();
                            $scope.activeSection = "openingBalancePageHeader";
                        });
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                    });
            };
            $scope.submitOpeningBalanceCreditors = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                if (!$scope.validateDocDate()) return;
                $scope.calling = true;

                var model ={
                    transactions: $scope.transactions, 
                    systemAccount: 'creditors',
                    docDate: $scope.docDate 

                };

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Document/SubmitOpeningBalance",
                    contentType: "application/json"
                }).done(function (res) {
                    var sumCreditors = res.data;
                        $scope.calling = false;
                        $scope.liabilities.creditors = sumCreditors;
                        $scope.calTotal();
                        $scope.calOpeningBalance();
                        if ($scope.roweditor1)
                            $scope.roweditor1.hide();
                        $("#" + $scope.activeSection).slideUp("slow", function () {
                            $("#openingBalancePageHeader").slideDown();
                            $scope.activeSection = "openingBalancePageHeader";
                        });
                        $scope.$apply();
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        $scope.$apply();
                    });
            };
            $scope.submitOpeningBalanceChequesPay = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                if (!$scope.validateDocDate()) return;
                $scope.calling = true;

                $scope.calling = true;
                var model ={
                    transactions: $scope.transactions, 
                    systemAccount: 'payables',
                    docDate: $scope.docDate 

                };

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Document/SubmitOpeningBalance",
                    contentType: "application/json"
                }).done(function (res) {
                    var sumPayables = res.data;

                        $scope.calling = false;
                        $scope.liabilities.payables = sumPayables;
                        $scope.calTotal();
                        $scope.calOpeningBalance();
                        $("#" + $scope.activeSection).slideUp("slow", function () {
                            $("#openingBalancePageHeader").slideDown();
                            $scope.activeSection = "openingBalancePageHeader";
                        });
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                    });
            };
            $scope.submitOpeningBalanceChequesRec = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                if (!$scope.validateDocDate()) return;
                $scope.calling = true;
                       var model ={
                    transactions: $scope.transactions, 
                    systemAccount: 'receivables',
                    docDate: $scope.docDate 

                };

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Document/SubmitOpeningBalance",
                    contentType: "application/json"
                }).done(function (res) {
                var sumReceivables = res.data;
                
                        $scope.calling = false;
                        $scope.assets.receivables = sumReceivables;
                        $scope.calTotal();
                        $scope.calOpeningBalance();
                        $("#" + $scope.activeSection).slideUp("slow", function () {
                            $("#openingBalancePageHeader").slideDown();
                            $scope.activeSection = "openingBalancePageHeader";
                        });
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                    });
            };
            $scope.submitOpeningBalanceChequesInProgress = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                if (!$scope.validateDocDate()) return;
                $scope.calling = true;

                $scope.calling = true;

                var model = {
                    transactions: $scope.transactions, 
                    systemAccount: 'inProgress',
                    docDate: $scope.docDate
                };

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Document/SubmitOpeningBalance",
                    contentType: "application/json"
                }).done(function (res) {

                    var sumInProgress = res.data;

                    $scope.calling = false;
                    $scope.assets.inProgress = sumInProgress;
                    $scope.calTotal();
                    $scope.calOpeningBalance();

                    $("#" + $scope.activeSection).slideUp("slow", function () {
                        $("#openingBalancePageHeader").slideDown();
                        $scope.activeSection = "openingBalancePageHeader";
                    });

                    applyScope($scope);

                }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                });
            };
            $scope.submitOpeningBalanceOtherAssets = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                if (!$scope.validateDocDate()) return;
                $scope.calling = true;

                var model = {
                    transactions: $scope.transactions, 
                    systemAccount: 'assets',
                    docDate: $scope.docDate
                };

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Document/SubmitOpeningBalance",
                    contentType: "application/json"
                }).done(function (res) {

                    var sumOtherAssets = res.data;
                        $scope.calling = false;
                        $scope.assets.otherAssets = sumOtherAssets;
                        $scope.calTotal();
                        $scope.calOpeningBalance();
                        if ($scope.roweditor1)
                            $scope.roweditor1.hide();
                        $("#" + $scope.activeSection).slideUp("slow", function () {
                            $("#openingBalancePageHeader").slideDown();
                            $scope.activeSection = "openingBalancePageHeader";
                        });
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                    });
            };
            $scope.submitOpeningBalanceOtherLiabilities = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                if (!$scope.validateDocDate()) return;
                $scope.calling = true;

                var model ={
                    transactions: $scope.transactions, 
                    systemAccount: 'liabilities',
                    docDate: $scope.docDate 

                };

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Document/SubmitOpeningBalance",
                    contentType: "application/json"
                }).done(function (res) {
                    var sumOtherLiabilities = res.data;
                        $scope.calling = false;
                        $scope.liabilities.otherLiabilities = sumOtherLiabilities;
                        $scope.calTotal();
                        $scope.calOpeningBalance();
                        $("#" + $scope.activeSection).slideUp("slow", function () {
                            $("#openingBalancePageHeader").slideDown();
                            $scope.activeSection = "openingBalancePageHeader";
                        });
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                    });
            };
            $scope.submitOpeningBalanceWithdrawals = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                if (!$scope.validateDocDate()) return;
                $scope.calling = true;
                callws(DefaultUrl.MainWebService + 'SubmitOpeningBalance', { transactions: $scope.transactions, systemAccount: "withdrawal", docDate: $scope.docDate })
                    .success(function (sumWithdrawals) {
                        $scope.calling = false;
                        $scope.capital.withdrawals = sumWithdrawals;
                        $scope.calTotal();
                        $scope.calOpeningBalance();
                        $("#" + $scope.activeSection).slideUp("slow", function () {
                            $("#openingBalancePageHeader").slideDown();
                            $scope.activeSection = "openingBalancePageHeader";
                        });
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
            $scope.submitOpeningBalanceShare = function () {
                $scope.alert = false;
                if ($scope.calling) return;
                if (!$scope.validateDocDate()) return;
                $scope.calling = true;
                callws(DefaultUrl.MainWebService + 'SubmitOpeningBalance', { transactions: $scope.transactions, systemAccount: "share", docDate: $scope.docDate })
                    .success(function (sumShare) {
                        $scope.calling = false;
                        $scope.capital.share = sumShare;
                        $scope.calTotal();
                        $scope.calOpeningBalance();
                        $("#" + $scope.activeSection).slideUp("slow", function () {
                            $("#openingBalancePageHeader").slideDown();
                            $scope.activeSection = "openingBalancePageHeader";
                        });
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };

            $scope.newCash = function () {
                //		$scope.activeRow = activeRow;
                $scope.alert = false;
                $scope.cash = null;
                $scope.editCashModal = true;
            };
            $scope.newBank = function () {
                //		$scope.activeRow = activeRow;
                $scope.alert = false;
                $scope.bank = null;
                $scope.editBankModal = true;
            };
            $scope.newContact = function () {
                //		$scope.activeRow = activeRow;
                $scope.alert = false;
                $scope.contact = null;
                $scope.editContactModal = true;
            };
            $scope.newItem = function (activeRow) {
                $scope.activeRow = activeRow;
                $scope.alert = false;
                $scope.item = null;
                $scope.editItemModal = true;
            };
            $scope.getEditedCash = function (cash) {
                if (!cash) return;
                $scope.cashes.push(cash);
                $scope.editCashModal = false;
                $scope.roweditor1.activeRow.DetailAccount = cash.DetailAccount;
                $scope.comboCash.setSelected(cash);
                $scope.calTotal();
                $scope.$apply();
            };
            $scope.getEditedBank = function (bank) {
                if (!bank) return;
                $scope.banks.push(bank);
                $scope.editBankModal = false;
                if ($scope.activeSection === "OpeningBalanceChequesInProgress") {
                    $scope.activeRow.DepositBank = bank;
                    $scope.activeRow.AccountInput2 = bank.Name;
                } else {
                    $scope.roweditor1.activeRow.DetailAccount = bank.DetailAccount;
                    $scope.comboBank.setSelected(bank);
                }
                $scope.calTotal();
                $scope.$apply();
            };
            $scope.getEditedContact = function (contact) {
                if (!contact) return;
                $scope.contacts.push(contact);
                $scope.editContactModal = false;
                if ($scope.activeSection === "OpeningBalanceChequesPay") {
                    $scope.activeRow.Cheque.Contact = contact;
                    $scope.activeRow.Cheque.ContactDetailAccount = contact.DetailAccount;
                }
                else if ($scope.activeSection === "OpeningBalanceChequesInProgress") {
                    $scope.activeRow.AccountInput = contact.Name;
                    $scope.activeRow.Cheque.Contact = contact;
                    $scope.activeRow.Cheque.ContactDetailAccount = contact.DetailAccount;
                }
                else if ($scope.activeSection === "OpeningBalanceChequesRec") {
                    $scope.activeRow.Cheque.Contact = contact;
                    $scope.activeRow.Cheque.ContactDetailAccount = contact.DetailAccount;
                    $scope.activeRow.DetailAccount = contact.DetailAccount;
                }
                else {
                    $scope.roweditor1.activeRow.DetailAccount = contact.DetailAccount;
                    if ($scope.activeSection === "openingBalanceDebtors")
                        $scope.comboDebtor.setSelected(contact);
                    else if ($scope.activeSection === "openingBalanceCreditors")
                        $scope.comboCreditor.setSelected(contact);
                }
                $scope.$apply();
            };
            $scope.getEditedItem = function (item) {
                if (!item) return;
                $scope.items.push(item);
                $scope.editItemModal = false;
                $scope.selectedItemId = item.Id;
                $scope.roweditor1.activeRow.item = item;
                $scope.roweditor1.activeRow.unitPrice = item.BuyPrice;
                $scope.roweditor1.activeRow.DetailAccount = item.DetailAccount;
                $scope.comboProduct.setSelected(item);
                $scope.calTotal();
                $scope.$apply();
            };
            $scope.itemSelect = function (item) {
                $scope.alert = false;
                // first check duplication
                var rowIndex = 0;
                var count = 0;
                for (var i = 0; i < $scope.transactions.length; i++) {
                    if ($scope.roweditor1.activeRow.rowIndex === i)
                        continue;
                    if ($scope.transactions[i].DetailAccount && $scope.transactions[i].DetailAccount.Id === item.DetailAccount.Id) {
                        if (rowIndex === 0) rowIndex = i + 1;
                        count++;
                    }
                }
                if (count > 0) {
                    $scope.alert = true;
                    $scope.alertType = "warning";
                    $scope.alertMessage = "این کالا قبلاً در ردیف " + rowIndex + " ثبت شده است";
                    $scope.roweditor1.activeRow.DetailAccount = null;
                    $scope.comboProduct.setSelected(null);
                    $scope.$apply();
                    return false;
                }
                $scope.roweditor1.activeRow.item = item;
                $scope.roweditor1.activeRow.unitPrice = item.BuyPrice;
                $scope.roweditor1.activeRow.DetailAccount = item.DetailAccount;
                $scope.roweditor1.activeRow.DetailAccount.Name = item.Name;
                $scope.calTotal();
                $scope.$apply();
                return true;
            };
            $scope.cashSelect = function (cash) {
                $scope.alert = false;
                // first check duplication
                var rowIndex = 0;
                var count = 0;
                for (var i = 0; i < $scope.transactions.length; i++) {
                    if ($scope.roweditor1.activeRow.rowIndex === i)
                        continue;
                    if ($scope.transactions[i].DetailAccount && $scope.transactions[i].DetailAccount.Id === cash.DetailAccount.Id) {
                        if (rowIndex === 0) rowIndex = i + 1;
                        count++;
                    }
                }
                if (count > 0) {
                    $scope.alert = true;
                    $scope.alertType = "warning";
                    $scope.alertMessage = "این صندوق قبلاً در ردیف " + rowIndex + " ثبت شده است";
                    $scope.roweditor1.activeRow.DetailAccount = null;
                    $scope.comboCash.setSelected(null);
                    $scope.$apply();
                    return false;
                }
                $scope.roweditor1.activeRow.DetailAccount = cash.DetailAccount;
                $scope.calTotal();
                $scope.$apply();
                return true;
            };
            $scope.bankSelect = function (bank) {
                $scope.alert = false;
                // first check duplication
                var rowIndex = 0;
                var count = 0;
                for (var i = 0; i < $scope.transactions.length; i++) {
                    if ($scope.roweditor1.activeRow.rowIndex === i)
                        continue;
                    if ($scope.transactions[i].DetailAccount && $scope.transactions[i].DetailAccount.Id === bank.DetailAccount.Id) {
                        if (rowIndex === 0) rowIndex = i + 1;
                        count++;
                    }
                }
                if (count > 0) {
                    $scope.alert = true;
                    $scope.alertType = "warning";
                    $scope.alertMessage = "این بانک قبلاً در ردیف " + rowIndex + " ثبت شده است";
                    $scope.roweditor1.activeRow.DetailAccount = null;
                    $scope.comboBank.setSelected(null);
                    $scope.$apply();
                    return false;
                }
                $scope.roweditor1.activeRow.DetailAccount = bank.DetailAccount;
                $scope.calTotal();
                $scope.$apply();
                return true;
            };
            $scope.chequePayBankSelect = function (bank) {
                $scope.alert = false;
                $scope.roweditor1.activeRow.DetailAccount = bank.DetailAccount;
                $scope.roweditor1.activeRow.Cheque.BankName = bank.Name;
                $scope.roweditor1.activeRow.Cheque.BankBranch = bank.Branch;
                applyScope($scope);
            };
            $scope.chequeInProgressBankSelect = function (bank) {
                $scope.alert = false;
                $scope.roweditor1.activeRow.DetailAccount = bank.DetailAccount;
                $scope.roweditor1.activeRow.Cheque.DepositBank = bank;
                applyScope($scope);
            };
            $scope.chequePayContactSelect = function (contact) {
                $scope.alert = false;
                $scope.roweditor1.activeRow.Cheque.Contact = contact;
                $scope.roweditor1.activeRow.Cheque.ContactDetailAccount = contact.DetailAccount;
                applyScope($scope);
            };
            $scope.chequeRecContactSelect = function (contact) {
                $scope.alert = false;
                $scope.roweditor1.activeRow.Cheque.Contact = contact;
                $scope.roweditor1.activeRow.Cheque.ContactDetailAccount = contact.DetailAccount;
                $scope.roweditor1.activeRow.DetailAccount = contact.DetailAccount;
                applyScope($scope);
            };
            $scope.chequeInProgressContactSelect = function (contact) {
                $scope.alert = false;
                $scope.roweditor1.activeRow.Cheque.Contact = contact;
                $scope.roweditor1.activeRow.Cheque.ContactDetailAccount = contact.DetailAccount;
                applyScope($scope);
            };
            $scope.debtorCreditorSelect = function (contact) {
                $scope.alert = false;
                // first check duplication
                var rowIndex = 0;
                var count = 0;
                for (var i = 0; i < $scope.transactions.length; i++) {
                    if ($scope.transactions[i].DetailAccount && $scope.transactions[i].DetailAccount.Id === contact.DetailAccount.Id) {
                        if (rowIndex === 0) rowIndex = i + 1;
                        count++;
                    }
                }
                if (count > 0) {
                    $scope.alert = true;
                    $scope.alertType = "warning";
                    $scope.alertMessage = "این شخص قبلاً در ردیف " + rowIndex + " ثبت شده است";
                    $scope.roweditor1.activeRow.DetailAccount = null;
                    if ($scope.activeSection === "openingBalanceDebtors")
                        $scope.comboDebtor.setSelected(null);
                    else if ($scope.activeSection === "openingBalanceCreditors")
                        $scope.comboCreditor.setSelected(null);
                    $scope.$apply();
                    return false;
                }
                $scope.roweditor1.activeRow.DetailAccount = contact.DetailAccount;
                $scope.roweditor1.activeRow.DetailAccount.Name = contact.Name;
                $scope.calTotal();
                $scope.$apply();
                return true;
            };
            $scope.itemSelect2 = function (item, selectBy, row) {
                $scope.alert = false;
                $scope.selectedItemId = item.Id;
                row.unit = item.Unit;
                row.unitPrice = item.BuyPrice;
                row.DetailAccount = {};
                row.DetailAccount.Id = item.DetailAccount.Id;
                row.DetailAccount.Code = item.DetailAccount.Code;
                $scope.getOpeningBalanceItem(item.Id, row);
                return true;
            };
            $scope.accountSelect = function (account) {
                $scope.roweditor1.activeRow.Account = account;
                // clear detail account to force user select again
                if ($scope.otherLiabilityComboDetailAccount)
                    $scope.otherLiabilityComboDetailAccount.setSelected(null);
                if ($scope.otherAssetsComboDetailAccount)
                    $scope.otherAssetsComboDetailAccount.setSelected(null);
                $scope.roweditor1.activeRow.DetailAccount = null;
                $scope.loadAccountDetailAccounts(account);
            };
            $scope.detailAccountSelect = function (dtAccount) {
                $scope.roweditor1.activeRow.DetailAccount = dtAccount;
            };
            $scope.cheqPayBankSelect = function (bank, selectBy, row) {
                row.Cheque.BankName = bank.Name;
                row.Cheque.BankBranch = bank.Branch;
                $scope.$apply();
            };
            $scope.addRow = function () {
                var newTrans = {};
                //		var newTrans = Hesabfa.newTransactionObj();
                angular.copy($scope.transObject, newTrans);
                newTrans.unitPrice = 0;
                newTrans.Stock = 0;
                if ($scope.activeSection === "OpeningBalanceChequesPay" || $scope.activeSection === "OpeningBalanceChequesRec"
                    || $scope.activeSection === "OpeningBalanceChequesInProgress")
                    newTrans.Cheque = { DisplayDate: "", Amount: 0, BankName: "", BankBranch: "", DepositDate: "" };
                $scope.transactions.push(newTrans);
                $scope.setIndexRow();
                //		// اگر چک بود فیلد تاریخ را راه اندازی کن
                //		if ($scope.activeSection === "OpeningBalanceChequesPay" || $scope.activeSection === "OpeningBalanceChequesRec"
                //            || $scope.activeSection === "OpeningBalanceChequesInProgress")

                if ($scope.transactions.length === 1) {
                    setTimeout(function () {
                        $scope.roweditor1.move(0);
                    }, 200);
                    //			$scope.rowEditor.move(0);
                }
            };
            $scope.deleteRow = function (index) {
                if ($scope.activeSection === "OpeningBalanceChequesPay" ||
                    $scope.activeSection === "OpeningBalanceChequesRec" ||
                    $scope.activeSection === "OpeningBalanceChequesInProgress") {
                    var valid = $scope.ValidateChequesDelete($scope.transactions[index].Cheque);
                    if (!valid) return;
                }
                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                var transactions = $scope.transactions;
                transactions.splice(index, 1);
                $scope.calTotal();
                $scope.setIndexRow();
                if ($scope.roweditor1)
                    $scope.roweditor1.itemDeleted(index);
            };
            $scope.openProductOpeningBalanceEditPanel = function (selectedRow) {
                if (!selectedRow) {
                    var newTrans = {};
                    angular.copy($scope.transObject, newTrans);
                    $scope.actRow = newTrans;
                } else {
                    $scope.actRow = selectedRow;
                }
                $scope.addProductOpeiningBalancePanel = true;
                $scope.$apply();
            };
            $scope.addProductOpeningBalanceToTable = function (actRow) {
                $scope.alert = false;
                if (!actRow.DetailAccount || actRow.DetailAccount.Id === 0) {
                    $scope.alert = true;
                    $scope.alertType = "danger";
                    $scope.alertMessage = "ابتدا یک کالا را انتخاب کنید";
                    return;
                }
                if (actRow.Stock === 0 || actRow.unitPrice === 0) {
                    $scope.alert = true;
                    $scope.alertType = "danger";
                    $scope.alertMessage = "موجودی اول دوره کالا را مشخص کنید";
                    return;
                }
                var arr = $scope.gridProducts.data;
                for (var i = 0; i < arr.length; i++) {
                    if (arr[i].DetailAccount.Id === actRow.DetailAccount.Id) {
                        arr[i] = actRow;
                        $scope.addProductOpeiningBalancePanel = false;
                        return;
                    }
                }
                arr.push(actRow);
                $scope.addProductOpeiningBalancePanel = false;
            };
            $scope.getGridProductIndex = function (index) {
                return ($scope.gridProducts.currentPage === 1 ? 0 : ($scope.gridProducts.currentPage - 1) * $scope.gridProducts.rpp) + index + 1;
            };
            $scope.validateDocDate = function () {
                var docDate = $scope.docDate;

                if (angular.isUndefined(docDate) || docDate === null || docDate === "") {
                    alertbox({ content: "تاریخ سند افتتاحیه را وارد کنید." });
                    return false;
                }

                if (docDate.length !== 10) {
                    alertbox({ content: "تاریخ سند افتتاحیه را صحیح وارد کنید(10 کاراکتر، مثال: 1395/01/01)." });
                    return false;
                }
                return true;
            };
            $scope.ValidateChequesDelete = function (cheque) {
                if (cheque.Id > 0 && cheque.Status !== 0) {
                    alertbox({ content: "وضعیت این چک تغییر کرده و امکان حذف آن وجود ندارد." });
                    return false;
                }
                return true;
            };

            $scope.cancel = function () {
                if ($scope.roweditor1)
                    $scope.roweditor1.hide();
                $scope.alert = false;
                $("#" + $scope.activeSection).slideUp("slow", function () {
                    $("#openingBalancePageHeader").slideDown();
                    $scope.activeSection = "openingBalancePageHeader";
                });
            };
            $scope.calTotal = function () {
                var transactions = $scope.transactions;
                var length = transactions.length;
                var total = 0;
                for (var i = 0; i < length; i++) {
                    if (isNaN(transactions[i].Amount)) transactions[i].Amount = 0;
                    if (transactions[i].Stock > 0)
                        transactions[i].Amount = transactions[i].Stock * transactions[i].UnitPrice;
                    total += parseFloat(transactions[i].Amount);
                }
                $scope.total = total;
            };
            $scope.calTotalProducts = function () {
                var transactions = $scope.gridProducts.data;
                var length = transactions.length;
                var total = 0;
                for (var i = 0; i < length; i++) {
                    if (transactions[i].Stock > 0)
                        transactions[i].Amount = transactions[i].Stock * transactions[i].unitPrice;
                    total += transactions[i].Amount;
                }
                $scope.total = total;
            };
            $scope.calOpeningBalance = function () {
                $scope.assets.sum = $scope.assets.cash + $scope.assets.bank + $scope.assets.debtors + $scope.assets.receivables
                                    + $scope.assets.inProgress + $scope.assets.inventory + $scope.assets.otherAssets;
                $scope.liabilities.sum = $scope.liabilities.creditors + $scope.liabilities.payables + $scope.liabilities.otherLiabilities;
                $scope.capital.capital = $scope.assets.sum - $scope.liabilities.sum;
                $scope.capital.sum = $scope.capital.capital;//- $scope.capital.withdrawals + $scope.capital.share;
                $scope.sumRight = $scope.assets.sum;
                $scope.sumLeft = $scope.liabilities.sum + $scope.capital.sum;
                $scope.$apply();
            };

            $scope.getFinanAccounts = function () {
                $scope.calling = true;
                callws(DefaultUrl.MainWebService + 'GetFinanAccounts', {})
                    .success(function (result) {
                        $scope.calling = false;
                        var accounts = result.finanAccounts;
                        accounts.pop();
                        for (var n = 0; n < accounts.length; n++) {
                            if (accounts[n].AccountType && accounts[n].AccountType.Category === 0)
                                accounts[n].credit = '';
                            else if (accounts[n].AccountType && accounts[n].AccountType.Category === 1)
                                accounts[n].debit = '';
                            else if (accounts[n].AccountType && accounts[n].AccountType.Category === 2)
                                accounts[n].debit = '';
                            else if (accounts[n].AccountType && accounts[n].AccountType.Category === 3)
                                accounts[n].debit = '';
                            else if (accounts[n].AccountType && accounts[n].AccountType.Category === 4)
                                accounts[n].credit = '';
                            else
                                accounts[n].credit = accounts[n].debit = '';
                        }
                        $scope.accounts = accounts;
                        $scope.changeTotal();
                        $scope.$apply();
                    }).fail(function (error) {
                        $scope.calling = false;
                        $scope.$apply();
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error });
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
            $scope.adjustGridHeader = function () {
                var width = $('#tdCode')[0].offsetWidth;
                $scope.tdCode = width;
                width = $('#tdName')[0].offsetWidth;
                $scope.tdName = width;
                $scope.$apply();
            };
            $scope.changeTotal = function () {
                var totalDebit = 0;
                var totalCredit = 0;
                for (var i = 0; i < $scope.accounts.length; i++) {

                    //			$scope.accounts[i].debit = $scope.accounts[i].debit ? Parser.evaluate($scope.accounts[i].debit.toString()) : 0;
                    //			$scope.accounts[i].credit = $scope.accounts[i].credit ? Parser.evaluate($scope.accounts[i].credit.toString()) : 0;

                    totalDebit += $scope.accounts[i].debit;
                    totalCredit += $scope.accounts[i].credit;

                }
                $scope.totalDebit = totalDebit;
                $scope.totalCredit = totalCredit;
            };
            $scope.totalRowClass = function () {
                if (!$scope.totalDebit && !$scope.totalCredit) return 'bg-info';
                else if ($scope.totalDebit === $scope.totalCredit) return 'bg-success';
                else return 'bg-warning';
            };
            $scope.getDeference = function () {
                return Math.abs($scope.totalDebit - $scope.totalCredit);
            };

            $scope.showOpeningDocument = function () {
                if ($scope.calling) return;
                $scope.calling = true;

                $.ajax({
                    type: "POST",
                    //data: JSON.stringify(model),
                    url: "/app/api/Document/GetOpeningDocument",
                    contentType: "application/json"
                }).done(function (res) {
                    var document = res.data;
                        $scope.calling = false;
                        if (!document) {
                            $scope.$apply();
                            alertbox({ content: "هنوز سند افتتاحیه ایجاد نشده است" });
                            return;
                        }
                        $scope.document = document;
                        // open view document modal
                        $("#viewDocumentModal").modal({ keyboard: false }, "show");
                        $("#viewDocumentModal .modal-dialog").draggable({ handle: ".modal-header" });
                        $scope.$apply();
                    }).fail(function (error) {
                        $scope.calling = false;
                        $scope.$apply();
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error });
                    });
            };

            $scope.getTotalDebit = function () {
                var total = 0;
                if(!angular.isUndefined($scope.document) && $scope.document !=null && $scope.document.Transactions != null)
                {
                    for (var i = 0; i < $scope.document.Transactions.length; i++) {
                        var product = $scope.document.Transactions[i];
                    total += product.Debit;
                }

                }
                return total;
            }

            $scope.getTotalCredit = function () {
                var total = 0;
                if (!angular.isUndefined($scope.document) && $scope.document != null && $scope.document.Transactions != null)
                {
                    for (var i = 0; i < $scope.document.Transactions.length; i++) {
                        var product = $scope.document.Transactions[i];
                        total += product.Credit;
                    }
                }
                return total;
            }

            
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});