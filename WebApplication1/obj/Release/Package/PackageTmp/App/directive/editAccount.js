define(['application', 'dataService'], function (app) {
    app.register.directive('editAccount', [function ($compile) {
        return {
            restrict: 'E',
            transclude: true,
            templateUrl: 'edit-account.html?ver=1.2.9.1',
            scope: {
                account: '=',
                accounts: '=',
                onsuccess: '=',
                open: '=',
                systemAccountTypes: '='
            },
            link: function (scope, element, attrs) {
                scope.initEditAccount = function () {
                    //				scope.defineComboboxes();
                    scope.alertBoxVisible = false;
                    scope.alertMessage = "";
                    scope.selectedSysAcc = "";
                    scope.showSystemAccountSelection = false;
                    scope.accountTypeId = 1;
                    if (!scope.account) {   // new
                        scope.getNewAccount();
                    } else {    // edit
                        scope.alertBoxVisible = false;
                        scope.accCodeExist = false;
                        scope.accCodeAvailable = false;
                        scope.accCodeHelp = true;
                        //                    $('#inputAccAccountType').focus();
                        scope.acc = scope.account;
                        scope.selectedSysAcc = scope.acc.SystemAccountName;

                        if (scope.account.Level > 1) {
                            scope.level1Code = scope.account.GroupCode;
                            var l = scope.accounts.length;
                            for (var i = 0; i < l; i++) {
                                var acc = scope.accounts[i];
                                if (acc.Level === 1 && acc.Code === scope.account.GroupCode) {
                                    scope.level1Name = acc.Name;
                                    break;
                                }
                            }
                        }
                        if (scope.account.Level === 3) {
                            var l = scope.accounts.length;
                            for (var i = 0; i < l; i++) {
                                var acc = scope.accounts[i];
                                if (acc.Level === 2 && acc.Coding === scope.account.ParentCoding) {
                                    scope.level2Code = acc.Code;
                                    scope.level2Name = acc.Name;
                                    break;
                                }
                            }
                        }
                        //applyScope(scope);
                    }

                    $('#editAccountModal').modal({ keyboard: false }, 'show');
                    $("#editAccountModal .modal-dialog").draggable({ handle: ".modal-header" });

                };
                scope.defineComboboxes = function () {
                    var groupAccounts = [];
                    var ledgerAccounts = [];
                    for (var i = 0; i < scope.account.length; i++) {
                        if (scope.account[i].Level === 1)
                            groupAccounts.push(scope.accout[i]);
                    }
                    if (!scope.comboGroupAccountSelect) {
                        scope.comboGroupAccountSelect = new HesabfaCombobox({
                            items: groupAccounts,
                            containerEle: document.getElementById("comboGroupAccountSelect"),
                            toggleBtn: true,
                            deleteBtn: true,
                            searchable: false,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            inputClass: "form-control input-sm",
                            itemTemplate: "<div> {account.Code} &nbsp;-&nbsp; {account.Name} </div>",
                            matchBy: "account.Id",
                            displayProperty: "account.Name",
                            searchBy: "Name|Code",
                            onSelect: scope.groupAccountSelect
                        });
                    }
                    if (!scope.comboLedgerAccountSelect) {
                        scope.comboLedgerAccountSelect = new HesabfaCombobox({
                            items: ledgerAccounts,
                            containerEle: document.getElementById("comboLedgerAccountSelect"),
                            toggleBtn: true,
                            deleteBtn: true,
                            searchable: false,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            inputClass: "form-control input-sm",
                            itemTemplate: "<div> {account.Code} &nbsp;-&nbsp; {account.Name} </div>",
                            matchBy: "account.Id",
                            displayProperty: "account.Name",
                            searchBy: "Name|Code",
                            onSelect: scope.ledgerAccountSelect
                        });
                    }
                };
                scope.groupAccountSelect = function () {
                    var ledgerAccounts = [];
                    for (var i = 0; i < scope.account.length; i++) {
                        if (scope.account[i].Level === 2 && scope.account[i].Level.GroupCode === scope.comboGroupAccountSelect.selected.Code)
                            ledgerAccounts.push(scope.accout[i]);
                    }
                    scope.comboLedgerAccountSelect.items = ledgerAccounts;
                };
                scope.$watch('open', function (value) {
                    if (value)
                        scope.initEditAccount();
                    scope.open = false;
                }, true);
                scope.submitAccount = function () {
                    var acc = scope.acc;
                    if (scope.calling) return;
                    var dublicateAcc = scope.checkSysAccDublication();
                    if (dublicateAcc) {
                        alertbox({
                            content: "حساب دیگری بعنوان این حساب سیستمی انتخاب شده است." +
                            "<br/>" + dublicateAcc.Name + " [کد: " + dublicateAcc.Code + "]"
                        });
                        return;
                    }
                    scope.calling = true;
                    callws('SaveAccount', { account: acc, selectedSysAcc: scope.selectedSysAcc })
                        .success(function (account) {
                            scope.calling = false;
                            scope.alertBoxVisible = false;
                            scope.onsuccess(account);
                            $('#inputAccCode').popover('hide');
                            $('#editAccountModal').modal('hide');
                            //scope.$apply();
                        }).fail(function (error) {
                            scope.calling = false;
                            scope.alertBoxVisible = true;
                            scope.alertMessage = error;
                            //scope.$apply();
                        }).loginFail(function () {
                            //window.location = DefaultUrl.login;
                            //scope.$apply();
                        });
                };
                scope.getNewAccount = function () {
                    scope.calling = true;
                    callws('GetFinanAccount', { id: 0 })
                        .success(function (account) {
                            scope.calling = false;
                            scope.alertBoxVisible = false;
                            scope.accCodeExist = false;
                            scope.accCodeAvailable = false;
                            scope.accCodeHelp = true;
                            scope.acc = account;
                            scope.codeTip = "";
                            scope.level2Code = "";
                            scope.level2Name = "";
                            //scope.$apply();
                        }).fail(function (error) {
                            window.location = '/error.html';
                        }).loginFail(function () {
                            //window.location = DefaultUrl.login;
                        });
                };
                scope.checkAccCode = function () {
                    scope.acc.Coding = scope.level1Code + scope.level2Code + scope.acc.Code;
                    scope.accCodeExist = false;
                    scope.accCodeAvailable = false;
                    scope.accCodeHelp = true;
                    if (scope.acc.Code == "") return;
                    callws('GetFinanAccountByCoding', { coding: scope.acc.Coding })
                        .success(function (account) {
                            if (account) {
                                if (scope.acc.Id > 0 && account.Id == scope.acc.Id) return;
                                scope.codeTip = '(' + account.Code + ' - ' + account.Name + ')';
                                scope.accCodeExist = true;
                                scope.accCodeAvailable = false;
                                scope.accCodeHelp = false;
                            } else {
                                scope.codeTip = scope.acc.Code;
                                scope.accCodeExist = false;
                                scope.accCodeAvailable = true;
                                scope.accCodeHelp = false;
                            }
                            scope.$apply();
                        }).fail(function (error) {
                            window.location = '/error.html';
                        }).loginFail(function () {
                            //window.location = DefaultUrl.login;
                        });
                };
                scope.levelChange = function () {
                    if (scope.acc.Level == 2) {
                        scope.level2Code = "";
                        scope.level2Name = "";
                    }
                    if (scope.level1Code) {
                        var accCode = scope.acc && scope.acc.Code ? scope.acc.Code : "";
                        scope.acc.Coding = scope.level1Code + (scope.level2Code || "") + accCode;
                    }
                    applyScope(scope);
                };
                scope.accountSelectLevel1 = function (accLvl1) {
                    scope.level1Code = accLvl1.Code;
                    scope.level1Name = accLvl1.Name;
                    scope.level2Code = "";
                    scope.level2Name = "";
                    var accCode = scope.acc && scope.acc.Code ? scope.acc.Code : "";
                    scope.acc.Coding = scope.level1Code + (scope.level2Code || "") + accCode;

                    // اگر در سطح کل بود
                    if (scope.acc.Level == "2") {
                        // کد بعدی حساب کل را در گروه انتخاب شده پیدا کن
                        var ledgerAccountsInGroup = [];
                        for (var i = 0; i < scope.accounts.length; i++) {
                            var account = scope.accounts[i];
                            if (account.GroupCode === scope.level1Code && account.Level === 2)
                                ledgerAccountsInGroup.push(account);
                        }
                        if (ledgerAccountsInGroup.length > 0) {
                            var lastCode = ledgerAccountsInGroup[ledgerAccountsInGroup.length - 1].Code;
                            var nextCode = parseInt(lastCode) + 1;
                            scope.acc.Code = nextCode + "";
                        } else
                            scope.acc.Code = "1";
                        // کدینگ حساب را بساز
                        scope.acc.Coding = scope.level1Code + (scope.level2Code || "") + scope.acc.Code;
                    }
                };
                scope.accountSelectLevel2 = function (accLvl2) {
                    scope.level2Code = accLvl2.Code;
                    scope.level2Name = accLvl2.Name;
                    var accCode = scope.acc && scope.acc.Code ? scope.acc.Code : "";
                    scope.acc.Coding = scope.level1Code + (scope.level2Code || "") + accCode;

                    // اگر در سطح معین بود
                    if (scope.acc.Level == "3") {
                        // کد بعدی حساب معین را در گروه انتخاب شده پیدا کن
                        var ledgerAccountsInGroup = [];
                        for (var i = 0; i < scope.accounts.length; i++) {
                            var account = scope.accounts[i];
                            if (account.ParentCoding === accLvl2.Coding && account.Level === 3)
                                ledgerAccountsInGroup.push(account);
                        }
                        if (ledgerAccountsInGroup.length > 0) {
                            var lastCode = ledgerAccountsInGroup[ledgerAccountsInGroup.length - 1].Code;
                            var nextCode = parseInt(lastCode) + 1;
                            scope.acc.Code = padleft(nextCode + "", 2);
                        } else
                            scope.acc.Code = "01";
                        // کدینگ حساب را بساز
                        scope.acc.Coding = scope.level1Code + (scope.level2Code || "") + scope.acc.Code;
                    }
                };

                scope.systemAccountSelectionQuestion = function () {
                    questionbox({
                        title: "هشدار",
                        content: "حساب سیستمی حسابی است که نرم افزار برای صدور سند اتوماتیک از آن استفاده می کند و " +
                        "تغییر اشتباه آن میتواند منجر به صدور سند اشتباه شود.",
                        btn1Title: "تایید",
                        btn2Title: "انصراف",
                        onBtn1Click: function () {
                            scope.showSystemAccountSelection = true;
                        }
                    });
                };
                scope.selectSysAcc = function (sysAcc) {
                    scope.selectedSysAcc = sysAcc;
                    scope.$apply();
                };
                scope.checkSysAccDublication = function () {
                    if (scope.selectedSysAcc === "") return false;
                    var l = scope.accounts.length;
                    for (var i = 0; i < l; i++) {
                        var acc = scope.accounts[i];
                        if (acc.SystemAccountName === scope.selectedSysAcc && acc.Id !== scope.acc.Id) return acc;
                    }
                    return false;
                }
                // useless
                scope.getFinanAccTypes = function () {
                    var ret = callwssync('GetFinanAccountTypes', {});
                    if (ret.result) {
                        scope.calling = false;
                        scope.accTypes = ret.result;
                        scope.$apply();
                    }
                    else if (ret.loginFailed)
                    { }
                    else if (ret.error)
                        window.location = '/error.html';
                };
            }
        };
    }]);
});