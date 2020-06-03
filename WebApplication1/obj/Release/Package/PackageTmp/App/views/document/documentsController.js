define(['application','digitGroup', 'number', 'dx', 'roweditor', 'helper', 'gridHelper'], function (app) {
    app.register.controller('documentsController', ['$scope','$rootScope', '$state','$stateParams', '$location','$window',
        function ($scope,$rootScope, $state, $stateParams, $location, $window) {

    var grid;
    $scope.init = function () {
        $rootScope.pageTitle("بارگیری اسناد...");
        $('#businessNav').show();
        $("#progressBarDiv").hide();
        $scope.searchBy = "numberName";
        $scope.activeTab = 0;
        var status = $stateParams.status; // $scope.getRouteQuery($routeParams.params, 'status');
        var document = Hesabfa.pageParams && Hesabfa.pageParams.document ? Hesabfa.pageParams.document : null;

        if (status && status === 'draft' && Hesabfa.pageParams && Hesabfa.pageParams.document) {
            $scope.alert = true;
            $scope.alertMessage = "پیش نویس سند حسابداری ذخیره شد - {0}".formatString(document.Description);
            $scope.alertType = 'success';
            $scope.$apply();
        }
        else if (status && status === 'approved' && Hesabfa.pageParams && Hesabfa.pageParams.document) {
            $scope.alert = true;
            $scope.alertMessage = "سند حسابداری تایید و درج در حساب ها شد - {0}".formatString(document.Description);
            $scope.alertType = 'success';
            $scope.$apply();
        }

        var statusLookup = { dataSource: [{ key: 0, val: "پیش نویس" }, { key: 1, val: "تایید شده" }], valueExpr: 'key', displayExpr: 'val' };
        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
            elementId: 'gridContainer',
            layoutKeyName: "grid-docuemnts",
            selection: { mode: "multiple" },
            height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 100),
            columns: [
				{
				    caption: 'شماره سند', dataField: 'Number', dataType: "number", width: 90, printWidth: 2, cellTemplate: function (ce, ci) {
				        var txt = Hesabfa.farsiDigit(ci.data.Number);
				        ce.html("<a href='javascript:void(0);' onclick='angular.element(this).scope().viewDocumentModal(" + ci.data.ID + ")' class='text-primary txt-bold'>" + txt + "</a>");
				    }
				},
				{ caption: 'عطف', dataField: 'Number2', dataType: "number", width: 90, printWidth: 2, hidingPriority: 2 },
				{ caption: 'تاریخ', dataField: 'DisplayDate', width: 100, printWidth: 3 },
				{ caption: 'شــرح', dataField: 'Description', printWidth: 8 },
                {
                    caption: 'وضعیت', dataField: 'Status', width: 100, printWidth: 3, hidingPriority: 1, lookup: statusLookup,
                    printCalcValue: function (data) {
                        for (var l in statusLookup.dataSource) {
                            if (statusLookup.dataSource[l].key === data.Status)
                                return statusLookup.dataSource[l].val;
                        }
                        return "";
                    }
                },
				{
				    caption: 'نوع', dataField: 'IsManual', width: 100, printWidth: 3, hidingPriority: 0, lookup: { dataSource: [{ key: true, val: "دستی" }, { key: false, val: "اتوماتیک" }], valueExpr: 'key', displayExpr: 'val' },
				    printCalcValue: function (data) {
				        return data.IsManual ? "دستی" : "اتوماتیک";
				    }
				},
                { caption: 'بدهکار', dataField: 'Debit', width: 120, printWidth: 3, dataType: "number", format: "#", printFormat: "#" },
				{ caption: 'بستانکار', dataField: 'Credit', width: 120, printWidth: 3, dataType: "number", format: "#", printFormat: "#" }
            ],
            print: {
                fileName: "DocumentsList.pdf",
                rowColumnWidth: 1.5,
                page: {
                    landscape: false
                },
                header: {
                    title1: $rootScope.currentBusiness.Name,
                    title2: "لیست اسناد حسابداری"
                }
            },
            			//onCellClick: function (item) {
            			//	if (!item.column || !item.data) return;
            			//	if (item.column.index !== -2) {
            			//		$scope.viewDocumentModal(item.data.ID);
            			//	}
            			//}
        });
        $scope.getDouments();
        $scope.showEditDocBtn = true;
        $scope.showDeleteDocBtn = true;
        $scope.showPrintDocBtn = true;

        $('#tabs a').click(function (e) {
            e.preventDefault();
            $(this).tab('show');
        });

        $(function () {
            $.getScript("/js/app/printReports.js?ver=1.2.9.1", function () { });
        });
    };
    $scope.getDouments = function () {
        $scope.loading = true;
        grid.beginCustomLoading();
        
        $.ajax({
            type:"POST",
            //data: JSON.stringify($scope.invoice),
            url: "/app/api/Document/GetDocumentsAndStats",
            contentType:"application/json"

        }).done(function (result) {
            //var result = res.data;
            //callws(DefaultUrl.MainWebService + "GetDocumentsAndStats", {})
            //    .success(function (result) {
            $scope.loading = false;
            grid.endCustomLoading();
            grid.fill(result.data);
            $scope.draftStat = result.draftStat;
            $scope.docSettings = result.docSettings;
            $rootScope.pageTitle("اسناد حسابداری");
            applyScope($scope);
        }).fail(function (error) {
            $scope.loading = false;
            grid.endCustomLoading();
            applyScope($scope);
            if ($scope.accessError(error)) return;
            alertbox({ content: error, type: "error" });
        });//.loginFail(function () {
            //    window.location = DefaultUrl.login;
            //});
    };
    $scope.search = function () {
        var searchBy = $scope.searchBy;
        var key;
        if (!searchBy || searchBy === "numberName") key = $scope.searchKey;
        else if (searchBy === "number2") key = $scope.searchNumber2;
        else if (searchBy === "date") key = $scope.searchDate;
        if (!key || key === ' ') {
            var activeTab = $scope.activeTab;
            if (activeTab === 0) {
                $scope.grid.removeFilter();
                applyScope($scope);
            }
            else if (activeTab === 1) $scope.grid.applyFilter('Status', 0);
            else if (activeTab === 2) $scope.grid.applyFilter('Status', 1);
            else if (activeTab === 3) $scope.grid.applyFilter('IsManual', true);
            else if (activeTab === 4) $scope.grid.applyFilter('IsManual', false);
            return;
        }
        if (!searchBy || searchBy === "numberName") {
            if (!isNaN(parseInt(key, 10)))
                $scope.grid.applyFilter('Number', key, true, $scope, true);
            else
                $scope.grid.applyFilter('Description', key, true, $scope, true);
        }
        else if (searchBy === "number2") {
            $scope.grid.applyFilter('Number2', key, true, $scope, true);
        }
        else if (searchBy === "date") {
            if (key.toString().length === 10)
                $scope.grid.applyFilter('DisplayDate', key, true, $scope, true);
        }
    };
    $scope.deleteDocument = function (document) {
        if (!document.IsManual) {
            alertbox({ content: "امکان حذف سند اتوماتیک وجود ندارد" });
            return;
        }
        questionbox({
            content: "آیا از حذف این سند مطمئن هستید؟",
            onBtn1Click: function () {
                if ($scope.calling) return;
                $scope.calling = true;

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(document.ID),
                    url: "/app/api/Document/DeleteManualDocument",
                    contentType: "application/json"

                }).done(function (result) {
                        $scope.calling = false;
                        $("#viewDocumentModal").modal("hide");
                    
                        DevExpress.ui.notify("سند حسابداری با موفقیت حذف گردید", "success", 3000);
                        applyScope($scope);
                        $scope.getDouments();   // reload documents
                    }).fail(function (error) {
                        $scope.calling = false;
                        applyScope($scope);
                        if ($scope.accessError(error)) return;
                        $("#viewDocumentModal").modal("hide");
                        $scope.alert = true;
                        $scope.alertType = "danger";
                        $scope.alertMessage = error;
                        applyScope($scope);
                    });
            }
        });
    };
    $scope.activeClass = function (n) {
        $("#dropdownSearch li").removeClass("active");
        $("#dropdownSearch li").eq(n).addClass("active");
    };
    $scope.printDocument = function () {
        printDocument($scope.document, $scope.docSettings, $rootScope.currentBusiness, $scope.getCurrency());
    };
    $scope.pdfDocument = function () {
        pdfDocument($scope.document, $scope.docSettings, null, null, $rootScope.currentBusiness, $scope.getCurrency());
    };
    $scope.arrangeDocuments = function () {
        questionbox({
            content: "آیا از مرتب کردن شماره اسناد بر اساس تاریخ مطمئن هستید؟",
            onBtn1Click: function () {
                if ($scope.calling) return;
                $scope.calling = true;
                callws(DefaultUrl.MainWebService + 'ArrangeDocuments', {})
                    .success(function () {
                        $scope.calling = false;
                        DevExpress.ui.notify("اسناد با موفقیت مرتب شدند", "success", 3000);
                        applyScope($scope);
                        $scope.getDouments();   // reload documents
                    }).fail(function (error) {
                        $scope.calling = false;
                        applyScope($scope);
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, type: "error" });
                        applyScope($scope);
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            }
        });
    };
    $scope.selectTab = function (tab) {
        switch (tab) {
            case null, 'all':
                grid.clearFilter();
                break;
            case "draft":
                grid.filter(["Status", "=", 0]);
                break;
            case "approved":
                grid.filter(["Status", "=", 1]);
                break;
            case "manual":
                grid.filter(["IsManual", "=", true]);
                break;
            case "automatic":
                grid.filter(["IsManual", "=", false]);
                break;
            default:
                grid.clearFilter();
                break;
        }
        applyScope($scope);
    };
    $scope.printDocuments = function () {
        var list = grid.getSelectedRowsData();
        if (list.length === 0) {
            alertbox({ content: "ابتدا يك يا چند سند را انتخاب كنيد" });
            return;
        }

        $("#printBtns").hide();
        $("#progressBarDiv").show();
        applyScope($scope);
        var listCount = list.length;
        var pdf;
        var i = 0;
        (function loop() {
            callws(DefaultUrl.MainWebService + 'GetDocument', { id: list[i].Id })
                .success(function (doc) {
                    var progressValue = Math.floor(((i + 1) * 100) / listCount);
                    pdf = pdfDocument(doc, $scope.docSettings, true, pdf, $rootScope.currentBusiness, $scope.getCurrency());
                    $('#progressPdf').attr('aria-valuenow', progressValue).css('width', progressValue + "%");
                    $("#progressPdfTitle")
                        .text("در حال تهیه PDF... سند " + Hesabfa.farsiDigit(i + 1) + " از " + Hesabfa.farsiDigit(listCount) + " (" + Hesabfa.farsiDigit(progressValue) + '%)');
                    i++;
                    if (i < listCount) setTimeout(loop, 0);
                    else {
                        pdf.save("Documents.pdf");
                        setTimeout(function () {
                            $("#progressBarDiv").hide();
                            $("#printBtns").show();
                            $('#progressPdf').attr('aria-valuenow', 1).css('width', 1 + "%");
                            $("#progressPdfTitle").text("در حال تهیه PDF...");
                        },
                            1000);
                    }
                })
                .fail(function (error) {
                    $scope.loading = false;
                    applyScope($scope);
                    if ($scope.accessError(error)) return;
                    alertbox({ content: error, title: "خطا" });
                    return;
                })
                .loginFail(function () {
                    window.location = DefaultUrl.login;
                });
        })();
    };
    $scope.exportDocsToExcel = function () {
        if ($scope.callingImport) return;
        $scope.callingImport = true;
        $('#loadingModal').modal('show');
        callws(DefaultUrl.MainWebService + 'ExportAccDocsToExcel', {})
            .success(function (data) {
                $scope.callingImport = false;
                $('#loadingModal').modal('hide');
                $scope.$apply();

                var contentType = 'application/vnd.ms-excel';
                var blob = b64toBlob(data, contentType);
                if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                    window.navigator.msSaveOrOpenBlob(blob, "documents.xlsx");
                }
                else {
                    var objectUrl = URL.createObjectURL(blob);
                    //                    window.open(objectUrl);
                    //                    var encodedUri = "data:application/vnd.ms-excel," + encodeURI(data);
                    var link = document.createElement("a");
                    link.href = objectUrl;
                    link.download = "documents.xlsx";
                    link.target = '_blank';
                    document.body.appendChild(link);
                    link.click();
                }

            }).fail(function (error) {
                $scope.callingImport = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error });
                return;
            }).loginFail(function () {
                window.location = DefaultUrl.login;
            });
    };
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);

    $scope.redirect = function(url,id){
 
        $state.go(url, {id:id});
    };

    //$scope.getTotalDebit = function () {
    //    var total = 0;
    //    for (var i = 0; i < $scope.document.Transactions.length; i++) {
    //        var product = $scope.document.Transactions[i];
    //        total += product.Debit;
    //    }
    //    return total;
    //}

    //$scope.getTotalCredit = function () {
    //    var total = 0;
    //    for (var i = 0; i < $scope.document.Transactions.length; i++) {
    //           var product = $scope.document.Transactions[i];
    //        total += product.Credit ;
    //    }
    //    return total;
    //}

    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);

        }])
});