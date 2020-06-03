define(['application', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper','printReports'], function (app) {
    app.register.controller('journalTotalAccountsReportController', ['$scope','$rootScope','$stateParams', '$location',
        function ($scope,$rootScope,$stateParams, $location) {
    $scope.init = function () {
        $rootScope.pageTitle("بارگیری دفتر روزنامه...");
        $('#businessNav').show();
        applyScope($scope);
        $scope.pageSummation = [];
        $scope.grid = new gridHelper({ scope: $scope, rpp: 22, onRppChange: $scope.onRppChange });
        $scope.getJournal();
        
        //$(function () {
        //    $.getScript("/App/printReports.js", function () { });

        //});
    };

    $scope.getJournal = function () {
        $scope.loading = true;
        //$scope.$apply();
           $.ajax({
            type:"POST",
            //data: JSON.stringify({ start: "", end: "" }),
            url: "/app/api/Transaction/GetJournalInTotalAccounts",
            contentType: "application/json"
           }).done(function (res) {
               var data = res.data;
                $scope.loading = false;
                $scope.data = data;
                var copy = [];
                angular.copy($scope.data, copy);
                $scope.buildJournalInTotalAccounts(copy);
                $rootScope.pageTitle("دفتر روزنامه در سطح معین");
                $scope.$apply();
            }).fail(function (error) {
                $scope.loading = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.buildJournalInTotalAccounts = function (data) {
        $scope.buildingReport = true;
        $scope.percent = 0;
        var length = data.length;
        var sumDebitPage = 0;
        var sumCreditPage = 0;
        var pageCount = 1;
        var rpp = parseInt($scope.grid.rpp + "");
        var rppCounter = 0;
        var pageSummation = [];
        $scope.pageSummation = [];
        $scope.journal = [];
        var journal = []; // The journal is going to build
        var currentDocTranses = []; // current document transactions
        $scope.$apply();

        var i = 0;

        function loop() {
            $scope.percent = Math.round((i * 100) / length);
            $scope.$apply();

            var trans = data[i];
            if (currentDocTranses.length !== 0 && currentDocTranses[currentDocTranses.length - 1].Number === trans.Number) {
                var oldTrans = findTransInCurrentDoc(trans);
                if (oldTrans != null) {
                    if (oldTrans.IsDebit)
                        oldTrans.Debit += trans.Debit;
                    else
                        oldTrans.Credit += trans.Credit;
                } else
                    currentDocTranses.push(trans);
            } else {
                if (currentDocTranses.length !== 0) {
                    addToJournal(currentDocTranses);
                    currentDocTranses = [];
                }
                currentDocTranses.push(trans);
            }
            i++;
            if (i < length)
                setTimeout(loop, 1);
            else {
                // add final document to journal
                if (currentDocTranses.length !== 0)
                    addToJournal(currentDocTranses, true);

                $scope.journal = journal;
                $scope.pageSummation = pageSummation;
                initGrid();
                $scope.buildingReport = false;
                $scope.$apply();
            }
        }

        loop();

        //		for (var i = 0; i < length; i++) {
        //			var trans = data[i];
        //			if (currentDocTranses.length !== 0 && currentDocTranses[currentDocTranses.length - 1].Number === trans.Number) {
        //				var oldTrans = findTransInCurrentDoc(trans);
        //				if (oldTrans != null) {
        //					oldTrans.Credit += trans.Credit;
        //					oldTrans.Debit += trans.Debit;
        //				}
        //				else
        //					currentDocTranses.push(trans);
        //			} else {
        //				if (currentDocTranses.length !== 0) {
        //					addToJournal(currentDocTranses);
        //					currentDocTranses = [];
        //				}
        //				currentDocTranses.push(trans);
        //			}
        //		}

        function findTransInCurrentDoc(trans) {
            var lCdt = currentDocTranses.length;
            for (var j = 0; j < lCdt; j++) {
                var cdtt = currentDocTranses[j];
                if (cdtt.AccountId === trans.AccountId && cdtt.IsDebit === trans.IsDebit)
                    return cdtt;
            }
            return null;
        }

        function addToJournal(cdt, finalPage) {
            var l = cdt.length;
            for (var j = 0; j < l; j++) {
                journal.push(cdt[j]);
                sumDebitPage += cdt[j].Debit;
                sumCreditPage += cdt[j].Credit;
                rppCounter++;
                if (rppCounter === rpp) {
                    rppCounter = 0;
                    pageSummation[pageCount] = [];
                    pageSummation[pageCount][0] = sumDebitPage;
                    pageSummation[pageCount][1] = sumCreditPage;
                    pageCount++;
                }
            }
            if (finalPage) {
                pageSummation[pageCount] = [];
                pageSummation[pageCount][0] = sumDebitPage;
                pageSummation[pageCount][1] = sumCreditPage;
            }
        }

        function initGrid() {
            $scope.grid.data = journal;
            $scope.grid.init();
        }

        function updateProgressBar(i, l) {
            $scope.percent = (i * 100) / l;
            $scope.$apply();
        }
    };
    $scope.onRppChange = function () {
        var copy = [];
        angular.copy($scope.data, copy);
        $scope.buildJournalInTotalAccounts(copy);
    };
    $scope.showDocument = function (id) {
        $scope.viewDocumentModal(id);
    };
    $scope.print = function () {
        if ($scope.grid.rpp > 22) {
            alertbox({ content: "امکان چاپ بیشتر از ۲۲ سطر در صفحه وجود ندارد.", title: "خطا", type: "error" });
            return;
        }
        else
            printJournalTotalAccounts($scope.grid.data, $scope.pageSummation, $scope.grid.rpp, $scope.grid.totalPage, $rootScope.currentBusiness, $scope.getCurrency());
    };
    $scope.pdf = function () {
        if ($scope.grid.rpp > 22) {
            alertbox({ content: "امکان چاپ بیشتر از ۲۲ سطر در صفحه وجود ندارد.", title: "خطا", type: "error" });
            return;
        }
        else
            pdfJournalTotalAccounts($scope.grid.data, $scope.pageSummation, $scope.grid.rpp, $scope.grid.totalPage, $rootScope.currentBusiness, $scope.getCurrency());
    };

    
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);

}])
});