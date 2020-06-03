define(['application', 'combo', 'scrollbar', 'helper', 'editItem','editBank','editCash', 'nodeSelect','dx'], function (app) {
    app.register.controller('moneyTransferController', ['$scope','$rootScope', '$location', '$state', '$stateParams','dataService' ,
        function ($scope, $rootScope, $location, $state, $stateParams, dataService) {

        
        var transferDateObj = new AMIB.persianCalendar('transferDate');

        $scope.init = function () {

            $scope.comboCashFrom = new HesabfaCombobox({
                    items: [],
                    containerEle: document.getElementById("comboCashFrom"),
                    toggleBtn: true,
                    newBtn: true,
                    deleteBtn: true,
                    itemClass: "hesabfa-combobox-item",
                    activeItemClass: "hesabfa-combobox-activeitem",
                    itemTemplate: "<div> {Code} &nbsp;-&nbsp; {Name} </div>",
                    matchBy: "cash.Id",
                    displayProperty: "{Name}",
                    searchBy: ["Name", "DetailAccount.Code"],
                    onSelect: $scope.fromSelect,
                    onNew: function () { $scope.newCash('from') }
                  });
            $scope.comboCashTo = new HesabfaCombobox({
                      items: [],
                      containerEle: document.getElementById("comboCashTo"),
                      toggleBtn: true,
                      newBtn: true,
                      deleteBtn: true,
                      itemClass: "hesabfa-combobox-item",
                      activeItemClass: "hesabfa-combobox-activeitem",
                      itemTemplate: "<div> {Code} &nbsp;-&nbsp; {Name} </div>",
                      matchBy: "cash.Id",
                      displayProperty: "{Name}",
                      searchBy: ["Name", "DetailAccount.Code"],
                      onSelect: $scope.toSelect,
                      onNew: function () { $scope.newCash('to') }
                  });
            $scope.comboBankFrom = new HesabfaCombobox({
                      items: [],
                      containerEle: document.getElementById("comboBankFrom"),
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
                      onSelect: $scope.fromSelect,
                      onNew: function () { $scope.newBank('from') }
                  });
            $scope.comboBankTo = new HesabfaCombobox({
                      items: [],
                      containerEle: document.getElementById("comboBankTo"),
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
                      onSelect: $scope.toSelect,
                      onNew: function () { $scope.newBank('to') }
                  });

            $rootScope.pageTitle("انتقال وجه");
            $('#businessNav').show();
            $scope.alert = false;
            //        $scope.amount = 0;
            $scope.docId = $stateParams.docId;// $scope.getRouteQuery($routeParams.params, 'docId');
            $scope.docId = !$scope.docId || $scope.docId === "" ? 0 : $scope.docId;
            $scope.loadTransfer();
            $("#moneyAmount").focus();
            $scope.transferDate = $scope.todayDate;


            applyScope($scope);
            $(function () {
                $('[data-toggle="tooltip"]').tooltip();
            });
        };
        $scope.loadTransfer = function () {


            $scope.loading = true;

     

            $.ajax({
                type: "POST",
                data: JSON.stringify($scope.docId),
                url: "/app/api/TransferMoney/GetLoadTransfer",
                contentType: "application/json"
            }).done(function (res) {

                var transferInfo = res.data;
                $scope.fromDetailAccount = {};
                $scope.toDetailAccount = {};
                $scope.from = transferInfo.From;
                $scope.to = transferInfo.To;
                $scope.transferDescription = transferInfo.Description;
                $scope.fromReference = transferInfo.FromReference;
                $scope.toReference = transferInfo.ToReference;
                $scope.amount = transferInfo.Amount;
                $scope.transferDate = transferInfo.DisplayDate;
                $scope.DocumentNumber = transferInfo.DocumentNumber;

                if (transferInfo.From === "cash")
                    $scope.fromCash = transferInfo.FromDetailAccountId;
                if (transferInfo.From === "bank")
                    $scope.fromBank = transferInfo.FromDetailAccountId;

                if (transferInfo.To === "cash")
                    $scope.toCash = transferInfo.ToDetailAccountId;
                if (transferInfo.To === "bank")
                    $scope.toBank = transferInfo.ToDetailAccountId;

                if ($stateParams.fromCash)// $scope.getRouteQuery($routeParams.params, 'fromCash'))
                    $scope.fromCash = $stateParams.fromCash;// $scope.getRouteQuery($routeParams.params, 'fromCash');
                if ($stateParams.fromBank)//$scope.getRouteQuery($routeParams.params, 'fromBank'))
                    $scope.fromBank = $stateParams.fromBank;// $scope.getRouteQuery($routeParams.params, 'fromBank');
                if ($stateParams.toCash)//$scope.getRouteQuery($routeParams.params, 'toCash'))
                    $scope.toCash = $stateParams.toCash;// $scope.getRouteQuery($routeParams.params, 'toCash');
                if ($stateParams.toBank)//$scope.getRouteQuery($routeParams.params, 'toBank'))
                    $scope.toBank = $stateParams.toBank;// $scope.getRouteQuery($routeParams.params, 'toBank');


                $scope.getCashes();
                $scope.getBanks();





                $scope.$apply();
                $("#moneyAmount").select();
            });

            
        };
        $scope.getCashes = function (cashes) {

            $.ajax({
                type: "POST",
                data: JSON.stringify($scope.docId),
                url: "/app/api/Cash/GetAllByOrganId",
                contentType: "application/json"
            }).done(function (res) {

                $scope.cashes = res.data;
                $scope.comboCashFrom.items = $scope.cashes;
                $scope.comboCashTo.items = $scope.cashes;
                if ($scope.fromCash) {
                    $scope.from = "cash";
                    var fromCash = {};
                    if ($scope.docId) {
                        for (var i = 0; i < $scope.cashes.length; i++) {
                            if ($scope.cashes[i].DetailAccount.Id === $scope.fromCash) {
                                fromCash = $scope.cashes[i];
                                break;
                            }
                        }
                    } else
                        fromCash = findByID($scope.cashes, $scope.fromCash);
                    $scope.fromDetailAccount = fromCash.DetailAccount;
                    $scope.comboCashFrom.setSelected(fromCash);
                }
                if ($scope.toCash) {
                    $scope.to = "cash";
                    var toCash = {};
                    if ($scope.docId) {
                        for (var i = 0; i < $scope.cashes.length; i++) {
                            if ($scope.cashes[i].DetailAccount.Id === $scope.toCash) {
                                toCash = $scope.cashes[i];
                                break;
                            }
                        }
                    } else
                        toCash = findByID($scope.cashes, $scope.toCash);
                    $scope.toDetailAccount = toCash.DetailAccount;
                    $scope.comboCashTo.setSelected(toCash);
                }

            
         

                $scope.$apply();
                $("#moneyAmount").select();
            }).fail(function (error) {
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
        };
        $scope.getBanks = function (banks) {
            $.ajax({
                type: "POST",
                data: JSON.stringify($scope.docId),
                url: "/app/api/Bank/GetAllByOrganId",
                contentType: "application/json"
            }).done(function (res) {

                $scope.banks = res.data;

                $scope.comboBankFrom.items = $scope.banks;
                $scope.comboBankTo.items = $scope.banks;
                if ($scope.fromBank) {
                    $scope.from = "bank";
                    var fromBank = {};
                    if ($scope.docId) {
                        for (var i = 0; i < $scope.banks.length; i++) {
                            if ($scope.banks[i].DetailAccount.Id === $scope.fromBank) {
                                fromBank = $scope.banks[i];
                                break;
                            }
                        }
                    } else
                        fromBank = findByID($scope.banks, $scope.fromBank);
                    $scope.fromDetailAccount = fromBank.DetailAccount;
                    $scope.comboBankFrom.setSelected(fromBank);
                }
                if ($scope.toBank) {
                    $scope.to = "bank";
                    var toBank = {};
                    if ($scope.docId) {
                        for (var i = 0; i < $scope.banks.length; i++) {
                            if ($scope.banks[i].DetailAccount.Id === $scope.toBank) {
                                toBank = $scope.banks[i];
                                break;
                            }
                        }
                    } else
                        toBank = findByID($scope.banks, $scope.toBank);
                    $scope.toDetailAccount = toBank.DetailAccount;
                    $scope.comboBankTo.setSelected(toBank);
                }


 

                $scope.$apply();
                $("#moneyAmount").select();
                if ($scope.bankId) $scope.addBankPay();
            }).fail(function (error) {
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
        };

        $scope.showTransferInfo = function (transferInfo) {

            };
        $scope.newCash = function (fromTo) {
                $scope.fromTo = fromTo;
                $scope.alert = false;
                $scope.cash = null;
                $scope.editCashModal = true;
                applyScope($scope);
            };
        $scope.newBank = function (fromTo) {
                $scope.fromTo = fromTo;
                $scope.alert = false;
                $scope.bank = null;
                $scope.editBankModal = true;
                applyScope($scope);
            };
        $scope.getEditedCash = function (cash) {
                if (!cash) return;
                $scope.cashes.push(cash);
                $scope.editCashModal = false;
                if ($scope.fromTo === "from") {
                    $scope.fromDetailAccount = cash.DetailAccount;
                    $scope.comboCashFrom.setSelected(cash);
                } else {
                    $scope.toDetailAccount = cash.DetailAccount;
                    $scope.comboCashTo.setSelected(cash);
                }
                $scope.$apply();
            };
        $scope.getEditedBank = function (bank) {
                if (!bank) return;
                $scope.banks.push(bank);
                $scope.editBankModal = false;
                if ($scope.fromTo === "from") {
                    $scope.fromDetailAccount = bank.DetailAccount;
                    $scope.comboBankFrom.setSelected(bank);
                } else {
                    $scope.toDetailAccount = bank.DetailAccount;
                    $scope.comboBankTo.setSelected(bank);
                }
                $scope.$apply();
            };
        $scope.fromSelect = function (cashOrBank) {
                $scope.fromDetailAccount = cashOrBank.DetailAccount;
            };
        $scope.toSelect = function (cashOrBank) {
                $scope.toDetailAccount = cashOrBank.DetailAccount;
            };
        $scope.submit = function () {
                if ($scope.calling) return;
                // validate
                if (!$scope.fromDetailAccount || !$scope.fromDetailAccount.Id) {
                    $scope.alert = true;
                    $scope.alertType = "danger";
                    $scope.alertMessage = "مبداً را مشخص کنید.";
                    return;
                }
                if (!$scope.toDetailAccount || !$scope.toDetailAccount.Id) {
                    $scope.alert = true;
                    $scope.alertType = "danger";
                    $scope.alertMessage = "مقصد را مشخص کنید.";
                    return;
                }
                if (!$scope.amount || $scope.amount === 0) {
                    $scope.alert = true;
                    $scope.alertType = "danger";
                    $scope.alertMessage = "مبلغ را مشخص کنید.";
                    return;
                }

                $scope.alert = false;
                $scope.calling = true;
               // $scope.$apply();

                var model =
                    {
                        from: $scope.from, to: $scope.to, displayDate: $scope.transferDate, amount: $scope.amount,
                        fromDetailAccountId: $scope.fromDetailAccount.Id, toDetailAccountId: $scope.toDetailAccount.Id,
                        description: $scope.transferDescription, fromReference: $scope.fromReference, toReference: $scope.toReference,
                        documentNumber: $scope.DocumentNumber, documentId: $scope.docId
                    };


                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/TransferMoney/SaveTransfer",
                    contentType: "application/json"
                }).done(function (res) {

                    var report = res.data;

                    $scope.calling = false;
                    var split = report.split(",");
                    var amount = split[0];
                    var from = split[1];
                    var to = split[2];

                    // message

                    DevExpress.ui.notify("انتقال مبلغ " + $scope.money(amount) + " " + $scope.getCurrency() +
                            " از " + from + " به " + to + " با موفقیت ثبت شد.", "success", 3000);

                        // clean form for new entry
                        $scope.fromDetailAccount = {};
                        $scope.toDetailAccount = {};
                        $scope.from = "";
                        $scope.to = "";
                        $scope.amount = 0;
                        $scope.transferDescription = "";
                        $scope.$apply();
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) { applyScope($scope); return; }
                        $scope.alert = true;
                        $scope.alertType = "danger";
                        $scope.alertMessage = error;
                        $scope.$apply();
                    });
            };
        $scope.cancel = function () {
                window.history.back();
            };
        
	$rootScope.loadCurrentBusinessAndBusinesses($scope.init);
    }])
}); 