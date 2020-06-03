
define(['application'], function (app) {
    app.register.directive('editDetailaccount', [function ($compile) {
        return {
            restrict: 'E',
            transclude: true,
            templateUrl: 'edit-detailaccount.html?ver=1.2.9.1',
            scope: {
                bind: '=',
                onsuccess: '=',
                open: '=',
                defaultnode: '=?'
            },
            link: function (scope, element, attrs) {
                scope.init = function () {
                    scope.alertBoxVisible = false;
                    scope.alertMessage = "";
                    scope.selectedAccount = null;
                    scope.selectedAccountInput = null;
                    scope.getAccounts();

                    if (!scope.comboRelatedAccount) {
                        scope.comboRelatedAccount = new HesabfaCombobox({
                            items: [],
                            containerEle: document.getElementById("comboRelatedAccount"),
                            toggleBtn: true,
                            deleteBtn: true,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: "<div> {Coding} &nbsp;-&nbsp; {Name} </div>",
                            matchBy: "account.Id",
                            displayProperty: "{Name}",
                            searchBy: ["Name", "Coding"],
                            onSelect: scope.accountSelect
                        });
                    }

                    if (!scope.bind) {
                        scope.getDetailAccount(0);
                    } else {
                        scope.detailAccount = scope.bind;
                        scope.getDetailAccount(scope.detailAccount.Id);
                    }

                    $('#editDetailAccountModal').modal({ keyboard: false }, 'show');
                    $("#editDetailAccountModal .modal-dialog").draggable({ handle: ".modal-header" });
                };
                scope.$watch('open', function (value) {
                    if (value)
                        scope.init();
                    scope.open = false;
                }, true);
                scope.submit = function () {
                    scope.calling = true;
                    var acc = scope.detailAccount;
                    if (acc.Accounts.length > 0) scope.RelatedAccounts = ',';
                    else scope.RelatedAccounts = "";
                    for (var i = 0; i < acc.Accounts.length; i++)
                        scope.RelatedAccounts += acc.Accounts[i].Id + ',';
                    callws('SaveDetailAccount', { account: acc })
                        .success(function (account) {
                            scope.calling = false;
                            scope.alertBoxVisible = false;
                            scope.onsuccess(account);
                            $('#editDetailAccountModal').modal('hide');
                            scope.$apply();
                        }).fail(function (error) {
                            scope.calling = false;
                            scope.alertBoxVisible = true;
                            scope.alertMessage = error;
                            scope.$apply();
                        }).loginFail(function () {
                            //window.location = DefaultUrl.login;
                            scope.$apply();
                        });
                };
                scope.getDetailAccount = function (id) {
                    scope.calling = true;
                    callws('GetDetailAccount', { id: id })
                        .success(function (account) {
                            scope.calling = false;
                            scope.detailAccount = account;
                            switch (scope.detailAccount.Node.SystemAccount) {
                                case 0: scope.nodeFilter = "other"; break;
                                case 1: scope.nodeFilter = "person"; break;
                                case 2: scope.nodeFilter = "product"; break;
                                case 3: scope.nodeFilter = "cash"; break;
                                case 4: scope.nodeFilter = "bank"; break;
                                default: scope.nodeFilter = "other"; break;
                            }
                            scope.startNodeSelect = true;
                            if (scope.defaultnode && scope.detailAccount.Id === 0)
                                scope.detailAccount.Node = scope.defaultnode;
                            scope.$apply();
                        }).fail(function (error) {
                            window.location = '/error.html';
                        }).loginFail(function () {
                            //window.location = DefaultUrl.login;
                            scope.$apply();
                        });
                };
                scope.checkAccCode = function () {
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
                scope.accountSelect = function (account, selectedBy) {
                    if (!account) return;
                    scope.selectedAccount = account;
                    var finded = findById(scope.detailAccount.Accounts, account.Id);
                    if (!finded) scope.detailAccount.Accounts.push(account);
                    scope.$apply();
                };
                scope.removeRelatedAccount = function (account) {
                    if (!account) return;
                    findAndRemove(scope.detailAccount.Accounts, account);
                    scope.$apply();
                };
                scope.getAccounts = function () {
                    callws('GetAccountsByLevel', { level: 3 })
                        .success(function (accounts) {
                            scope.accounts = accounts;
                            scope.comboRelatedAccount.items = accounts;
                            scope.comboRelatedAccount.setSelected(null);
                            scope.$apply();
                        }).fail(function (error) {
                            window.location = '/error.html';
                        }).loginFail(function () {
                            //window.location = DefaultUrl.login;
                        });
                };
            }
        };
    }]);
});