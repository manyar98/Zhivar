define(['application'], function (app) {
    app.register.controller('editDocumentController', ['$scope','$rootScope', '$state', '$stateParams', '$location',
        function ($scope, $rootScope, $state, $stateParams, $location) {

            var documentDateObj = new AMIB.persianCalendar('documentDate');

    $scope.init = function () {
        $rootScope.pageTitle("بارگیری سند...");
        //$('#businessNav').show();
        //$("body").removeClass("modal-open");

        $scope.alert = false;
        $scope.alertMessage = "";

        $scope.sumDebit = 0;
        $scope.sumCredit = 0;
        $scope.deference = 0;

        $scope.dAccounts = [];
        $scope.document = null;

        if ($stateParams.id === 'new')
            $scope.loadDocumentData(0);
        else
            $scope.loadDocumentData($stateParams.id);

        applyScope($scope);
    };
    $scope.loadDocumentData = function (id) {
        if ($scope.roweditor1)
            $scope.roweditor1.hide();
        $scope.loading = true;

        $.ajax({
            type: "POST",
            data: JSON.stringify(id),
            url: "/app/api/Document/LoadDocumentData",
            contentType: "application/json"
        }).done(function (res) {
            var result = res.data;

            $scope.loading = false;
                var previewsDate = "";
                if ($scope.document && $scope.document.DisplayDate)
                    previewsDate = $scope.document.DisplayDate;

                $scope.document = result.document;
                $scope.document.IsManual = true;
                $scope.accounts = result.accounts;
                //            	$scope.detailAccounts = result.detailAccounts;
                $scope.defaultDescriptions = result.defaultDescriptions;
                if (previewsDate) $scope.document.DisplayDate = previewsDate;

                var transactions = [];
                if ($scope.document.Id > 0)
                    transactions = $scope.document.Transactions;
                else {
                    $scope.document.Transactions = [];
                    $scope.addRow(4);
                }

                if ($scope.document.Id > 0) $rootScope.pageTitle("ویرایش سند شماره " + $scope.document.Number);
                else $rootScope.pageTitle("سند حسابداری");

                $scope.$apply();
                $scope.calculateDoc();
                $scope.setIndexRow();

                if (!$scope.comboAccount) {
                    $scope.comboAccount = new HesabfaCombobox({
                        items: $scope.accounts,
                        containerEle: document.getElementById("accountSelect"),
                        toggleBtn: true,
                        deleteBtn: true,
                        itemClass: "hesabfa-combobox-item",
                        activeItemClass: "hesabfa-combobox-activeitem",
                        itemTemplate: "<div style='white-space: nowrap;overflow: hidden;'> {Coding} &nbsp;-&nbsp; {Name} </div>",
                        matchBy: "account.Id",
                        displayProperty: "{Name}",
                        searchBy: ["Name", "Coding"],
                        onSelect: $scope.accountSelect
                    });
                }
                if (!$scope.comboDetailAccount) {
                    $scope.comboDetailAccount = new HesabfaCombobox({
                        items: [],
                        containerEle: document.getElementById("detailAccountSelect"),
                        toggleBtn: true,
                        deleteBtn: true,
                        itemClass: "hesabfa-combobox-item",
                        activeItemClass: "hesabfa-combobox-activeitem",
                        itemTemplate: "<div> {Code} &nbsp;-&nbsp; {Name} </div>",
                        matchBy: "detailAccount.Id",
                        displayProperty: "{Name}",
                        searchBy: ["Name", "Code"],
                        onSelect: $scope.detailAccountSelect,
                        onDelete: function () {
                            $scope.roweditor1.activeRow.DetailAccount = null;
                        }
                    });
                }

                var onMoveRoweditor = function () {
                    $scope.comboAccount.setSelected($scope.roweditor1.activeRow.Account);
                    $scope.comboDetailAccount.setSelected($scope.roweditor1.activeRow.DetailAccount);
                    $scope.loadAccountDetailAccounts($scope.roweditor1.activeRow.Account);
                };

                $scope.setIndexRow();
                var controls = [];
                controls.push({ id: "accountSelect", element: "tdAcc" });
                controls.push({ id: "detailAccountSelect", element: "tdDt" });
                controls.push({ id: "inputDescription", element: "tdDes" });
                controls.push({ id: "inputDebit", element: "tdDebit" });
                controls.push({ id: "inputCredit", element: "tdCredit" });
                $scope.roweditor1 = new rowEditor({
                    controls: controls,
                    itemArray: $scope.document.Transactions, onMove: onMoveRoweditor
                });
                if ($scope.document.Transactions && $scope.document.Transactions.length > 0)
                    $scope.roweditor1.move(0);
                $scope.$apply();
        }).fail(function(error){
            $scope.loading = false;
            applyScope($scope);
            if ($scope.accessError(error)) return;
            alertbox({ content: error, type: "error" }); return;
        });
      
    };

    $scope.setIndexRow = function () {
        var transactions = $scope.document.Transactions;
        if (transactions) {
            for (var j = 0; j < transactions.length; j++) {
                transactions[j].rowIndex = j;
            }
        }
    };
    $scope.loadAccountDetailAccounts = function (account) {
        if (!account) {
            $scope.comboDetailAccount.items = [];
            return;
        }
        
        $.ajax({
            type: "POST",
            data: JSON.stringify(account),
            url: "/app/api/Accounting/GetAccountDetailAccounts",
            contentType: "application/json"
        }).done(function (res) {
            var detailAccounts = res.data;
			    $scope.comboDetailAccount.items = detailAccounts;
			    $scope.$apply();
			}).fail(function (error) {
			    if ($scope.accessError(error)) return;
			    alertbox({ content: error, type: "error" });
			});
    };
    $scope.accountSelect = function (account) {
        $scope.roweditor1.activeRow.Account = account;
        // clear detail account to force user select again
        $scope.comboDetailAccount.setSelected(null);
        $scope.roweditor1.activeRow.DetailAccount = null;
        $scope.loadAccountDetailAccounts(account);
    };
    $scope.detailAccountSelect = function (dtAccount) {
        $scope.roweditor1.activeRow.DetailAccount = dtAccount;
    };

    $scope.newAccount = function (activeTrans) {
        $scope.activeTrans = activeTrans;
        $scope.alert = false;
        $scope.account = null;
        $scope.editAccountModal = true;
        $scope.$apply();
    };
    $scope.getEditedAccount = function (account) {
        if (!account) return;
        $scope.accounts.push(account);
        $scope.editAccountModal = false;
        $scope.activeTrans.Account = account;
        $scope.activeTrans.AccountInput = account.Name;
        $scope.$apply();
    };

    $scope.initInvoiceAccount = function (elm) {
        $scope.canNewAccount = false;   // temporary, until edit account modal create
        $scope.activeAccountIndex = 0;
        $scope.filteredAccounts = [];
        var bindEvents = function (e) {
            if (e.which == 40) { // down
                if ($scope.filteredAccounts && $scope.filteredAccounts.length > 1) {
                    if ($scope.filteredAccounts.length - 1 > $scope.activeAccountIndex)
                        $scope.activeAccountIndex++;
                    else $scope.activeAccountIndex = 0;
                    $scope.$apply();
                    var selected = $('.list-group-item.active.98723473')[0];
                    var p = $('#accountSelect');
                    p.scrollTop(selected.offsetTop - 250);
                }
                return false; // stops the page from scrolling
            }
            if (e.which == 38) { // up
                if ($scope.filteredAccounts && $scope.filteredAccounts.length > 1) {
                    if ($scope.activeAccountIndex > 0)
                        $scope.activeAccountIndex--;
                    else $scope.activeAccountIndex = $scope.filteredAccounts.length - 1;
                    $scope.$apply();
                    var selected2 = $('.list-group-item.active.98723473')[0];
                    var p2 = $('#accountSelect');
                    p2.scrollTop(selected2.offsetTop);
                }
                return false; // stops the page from scrolling
            }
            if (e.which == 13 || e.which == 9) { // enter
                if ($scope.filteredAccounts.length > 0)
                    $scope.accountSelect($scope.filteredAccounts[$scope.activeAccountIndex]);
            }
            $scope.$apply();
        };
        elm.bind('keydown', bindEvents);
    };
    $scope.accountFocused = function () {
        var elm = $(':focus');
        $scope.focusedElm = elm;
        $scope.initInvoiceAccount(elm);

        $("#accountSelect").insertAfter(elm);
    };
    $scope.accountChange = function (item) {
        $scope.activeTransaction = item;
        var value = item.AccountInput;
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
    $scope.accountLeave = function () {
        if ($scope.focusedElm) $scope.focusedElm.unbind('keydown');;
        setTimeout(function () { $("#accountSelect").hide(); }, 200);
    };

    $scope.calculateDoc = function () {
        var transactions = $scope.document.Transactions;
        var totalDebit = 0;
        var totalCredit = 0;
        for (var i = 0; i < transactions.length; i++) {
            transactions[i].Debit = Number(transactions[i].Debit) || 0;
            transactions[i].Credit = Number(transactions[i].Credit) || 0;
            transactions[i].Amount = Number(transactions[i].Credit) || Number(transactions[i].Debit);
            transactions[i].Type = transactions[i].Debit > transactions[i].Credit ? 0 : 1;
            totalDebit += transactions[i].Debit;
            totalCredit += transactions[i].Credit;
        }

        $scope.sumDebit = totalDebit;
        $scope.sumCredit = totalCredit;
        $scope.deference = totalDebit > totalCredit ? totalDebit - totalCredit : totalCredit - totalDebit;

        $scope.$apply();
    };
    $scope.totalRowClass = function () {
        if (!$scope.sumDebit && !$scope.sumCredit) return 'bg-info';
        else if ($scope.sumDebit == $scope.sumCredit) return 'bg-success';
        else return 'bg-warning';
    };
    $scope.addRow = function (n) {
        if (!n) n = 1;
        for (var i = 0; i < n; i++) {
            var newTransaction = Hesabfa.newTransactionObj();
            newTransaction.RowNumber = $scope.document.Transactions.length + 1;
            $scope.document.Transactions.push(newTransaction);
        }
        $scope.setIndexRow();
        if ($scope.document.Transactions.length === 1)
            $scope.roweditor1.move(0);
    };
    $scope.deleteRow = function (transaction) {
        $scope.roweditor1.hide();
        // remove transaction item from doc transactions list
        var transactions = $scope.document.Transactions;
        findAndRemoveByPropertyValue(transactions, 'RowNumber', transaction.RowNumber);
        for (var i = 0; i < transactions.length; i++)  // add row numbers again
            transactions[i].RowNumber = i + 1;
        $scope.calculateDoc();  // recalculate document
        $scope.setIndexRow();
        $scope.roweditor1.itemDeleted(transaction.rowIndex);
    };
    $scope.moveUpRow = function (transaction) {
        var transactions = $scope.document.Transactions;
        transactions.moveUp(transaction);
        for (var i = 0; i < transactions.length; i++)
            transactions[i].RowNumber = i + 1;
        $scope.$apply();
        $scope.setIndexRow();

    };
    $scope.moveDownRow = function (transaction) {
        var transactions = $scope.document.Transactions;
        transactions.moveDown(transaction);
        for (var i = 0; i < transactions.length; i++)
            transactions[i].RowNumber = i + 1;
        $scope.$apply();
        $scope.setIndexRow();

    };
    $scope.transDesFocus = function (index) {
        if (!$scope.roweditor1.activeRow.Description) {
            $scope.roweditor1.activeRow.Description = $scope.document.Description;
            $scope.$apply();
            $('#transDes' + index).select();
        }
    };

    $scope.saveDocument = function (command) {
        if ($scope.calling) return;
        $scope.calling = true;
        if (command === 'approve' || command === 'approveAndNext') $scope.document.Status = 1;
        //		$scope.document.DisplayDate = $('#documentDate').val();
        $.ajax({
            type: "POST",
            data: JSON.stringify($scope.document),
            url: "/app/api/Document/SaveDocument",
            contentType: "application/json"
        }).done(function (res) {
            var document = res.data;
    
                var save = function () {
                    Hesabfa.pageParams = {};
                    Hesabfa.pageParams.document = document;
                    $state.go("/documents", { status: "draft" });
                    //$location.path('/documents/status=draft');
                    $scope.$apply();
                    return;
                };
                var approve = function () {
                    Hesabfa.pageParams = {};
                    Hesabfa.pageParams.document = document;
                    $state.go("/documents", { status: "approved" });
                    //$location.path('/documents/status=approved');
                    $scope.$apply();
                    return;
                };

                $scope.calling = false;
                if (command == 'save') {
                    save();
                }
                else if (command == 'approve') {
                    approve();
                }
                else if (command == 'saveAndNext') {
                    $scope.alert = true;
                    $scope.alertType = 'success';
                    $scope.alertMessage = "پیش نویس سند حسابداری ذخیره شد - {0}".formatString(document.Description);
                    $scope.alertMessages = null;
                    $scope.actRow = null;
                    $scope.outRowEditor();
                    $scope.loadDocumentData(0);
                    $scope.$apply();
                    return;
                }
                else if (command == 'approveAndNext') {
                    $scope.alert = true;
                    $scope.alertType = 'success';
                    $scope.alertMessage = "سند حسابداری تایید و درج در حساب ها شد - {0}".formatString(document.Description);
                    $scope.alertMessages = null;
                    $scope.actRow = null;
                    $scope.outRowEditor();
                    $scope.loadDocumentData(0);
                    $scope.$apply();
                    return;
                }
            }).fail(function (error) {
                $scope.calling = false;
                alertbox({ content: error, type: "warning" });
                applyScope($scope);
            });
    };
    $scope.cancel = function () {
        window.history.back();
    };

    $scope.moveRowEditor = function (index, $event) {
        if ($scope.actRow && $scope.actRow.RowNumber - 1 === index) return;
        $scope.actRow = $scope.document.Transactions[index];
        $("#inputAccSelect").prependTo("#tdAcc" + index);
        $("#inputDtSelect").prependTo("#tdDt" + index);
        $("#inputDescription").prependTo("#tdDes" + index);
        $("#inputDebit").prependTo("#tdDebit" + index);
        $("#inputCredit").prependTo("#tdCredit" + index);
       // $scope.$apply();
        if ($event) $('#' + $event.currentTarget.id + " input").select();
    };
    $scope.outRowEditor = function () {
        $("#inputAccSelect").prependTo("#hideme");
        $("#inputDtSelect").prependTo("#hideme");
        $("#inputDescription").prependTo("#hideme");
        $("#inputDebit").prependTo("#hideme");
        $("#inputCredit").prependTo("#hideme");
    };

    $scope.showDefaultDes = function (activeRow) {
        // open view document modal
        $scope.desInputTarget = !activeRow ? "docDes" : "";
        $("#defaultDesModal").modal({ keyboard: false }, "show");
    };
    $scope.selectDefaultDes = function (des) {
        $("#defaultDesModal").modal('hide');
        if ($scope.desInputTarget === "docDes")
            $scope.document.Description = des;
        else
            $scope.roweditor1.activeRow.Description = des;
    };
    $scope.addDescription = function () {
        if ($scope.newDescription !== "") {
            $scope.defaultDescriptions.push($scope.newDescription);
            $scope.$apply();
            $scope.saveDefaultDes();
        }
    };
    $scope.deleteDescription = function (index) {
        $scope.defaultDescriptions.splice(index, 1);
        $scope.$apply();
        $scope.saveDefaultDes();
    };
    $scope.saveDefaultDes = function () {
        callws(DefaultUrl.MainWebService + "SaveDocDefaultDescriptions", { defaultDescriptions: $scope.defaultDescriptions })
            .success(function () {
                $scope.$apply();
            }).fail(function (error) {
                if ($scope.accessError(error)) return;
                alertbox({ content: error });
            }).loginFail(function () {
                window.location = DefaultUrl.login;
            });
    };
    //
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
    }])
}); 