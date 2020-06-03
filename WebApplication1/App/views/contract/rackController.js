define(['application', 'dataService', 'messageService', 'combo', 'scrollbar', 'helper', 'dx', 'digitGroup', 'number', 'combo', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker', 'maskedinput', 'ngRightClick','editReservation','editContractStop'], function (app) {
    app.register.controller('rackController', ['$scope', '$stateParams', '$location', '$state', '$rootScope', 'dataService', 'messageService', '$uibModal','$filter',
        function ($scope, $stateParams, $location, $state, $rootScope, dataService, messageService, $uibModal, $filter) {

            $scope.operationAccess = dataService.getOperationAccess('Contract');
            if (!$scope.operationAccess.canView)
                return;

            var objstartDate = new AMIB.persianCalendar('startDate');

            $scope.isCreatedRes = false;
            $scope.isCreatedStop = false;
            $scope.lstSaze = [{}];
            $scope.typeName = "هفته";

            $scope.model = {
                TempleteType: 1,
                IsDefault: true
            };

            $scope.calling = true;
            $scope.init = function () {
                $rootScope.pageTitle("رک قراردادها...");
                $scope.isCreatedRes = false;
                $scope.isCreatedStop = false;

                $scope.currency = $scope.getCurrency();

                $scope.comboItem = new HesabfaCombobox({
                    items: [],
                    containerEle: document.getElementById("comboItem"),
                    toggleBtn: true,
                    newBtn: false,
                    deleteBtn: true,
                    itemClass: "hesabfa-combobox-item",
                    activeItemClass: "hesabfa-combobox-activeitem",
                    itemTemplate: $rootScope.Hesabfa.comboSazeTemplate,
                    //inputClass: "form-control input-sm input-grid-factor",
                    matchBy: "item.DetailAccount.Id",
                    displayProperty: "{Title}",
                    searchBy: ["Title", "DetailAccount.Code"],
                    onSelect: function (item) {
                        $scope.model.SazeID = item.ID;
                    },
                    onDelete: function ()
                    {
                        $scope.model.SazeID = null;
                    },
                    divider: true
                });

                $scope.comboContact = new HesabfaCombobox({
                    items: [],
                    containerEle: document.getElementById("comboContact"),
                    toggleBtn: true,
                    newBtn: false,
                    deleteBtn: true,
                    itemClass: "hesabfa-combobox-item",
                    activeItemClass: "hesabfa-combobox-activeitem",
                    itemTemplate: $rootScope.Hesabfa.comboContactTemplate,
                    divider: true,
                    matchBy: "contact.DetailAccount.ID",
                    displayProperty: "{Name}",
                    searchBy: ["Name", "Code"],
                    onSelect: function (item) {
                        $scope.model.ContactID = item.ID;
                    },
                    onDelete: function () {
                        $scope.model.ContactID = null;
                    },
                });

                $scope.loadRockData();
            };

            var lst = [{}]; 
            var colTotal = 0;

            $scope.changeTemplateDate = function (status) {
                if (status === 1) {
                    $('#btnWeek').removeClass("btn-default");
                    $('#btnWeek').addClass("btn-primary");

                    $('#btnMonth').removeClass("btn-primary");
                    $('#btnMonth').addClass("btn-default");

                    $('#btnYear').removeClass("btn-primary");
                    $('#btnYear').addClass("btn-default");

                    $scope.typeName = "هفته";
                }
                else if (status === 2) {
                    $('#btnWeek').removeClass("btn-primary");
                    $('#btnWeek').addClass("btn-default");

                    $('#btnMonth').removeClass("btn-default");
                    $('#btnMonth').addClass("btn-primary");

                    $('#btnYear').removeClass("btn-primary");
                    $('#btnYear').addClass("btn-default");

                    $scope.typeName = "ماه";
                }
                else if (status === 3) {
                    $('#btnWeek').removeClass("btn-primary");
                    $('#btnWeek').addClass("btn-default");

                    $('#btnMonth').removeClass("btn-primary");
                    $('#btnMonth').addClass("btn-default");

                    $('#btnYear').removeClass("btn-default");
                    $('#btnYear').addClass("btn-primary");

                    $scope.typeName = "سال";
                }


                $scope.model.TempleteType = status;
                $scope.model.IsDefault = true;
                $scope.loadRockData();
            };

            $scope.loadRockData = function () {
                $scope.calling = true;

                $(function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify($scope.model),
                        url: "/app/api/Contract/loadRockData",
                        contentType: "application/json"
                    }).done(function (res) {
                        $scope.calling = false;
                        var result = res.data;


                        // $scope.comboItem.items = result.Sazes;

                        $scope.listGoroheSaze = result.listGoroheSaze;

                        $scope.totalCountOfRent = 0;
                        $scope.totalOccupy = 0;
                        $scope.totalCostRentTo = 0;
                        $scope.totalCostRentFrom = 0;
                        $scope.showCostRentFrom = false;

                        var totalCounter = 0;
                        angular.forEach($scope.listGoroheSaze, function (value, key) {

                            $scope.totalCountOfRent += value.CountOfRent;
                            $scope.totalOccupy += value.Occupy;
                            totalCounter += value.CountOfRent * value.Occupy;
                            $scope.totalCostRentTo += value.CostRentTo;
                            $scope.totalCostRentFrom += value.CostRentFrom;
                            $scope.showCostRentFrom = value.ShowCostFrom;
                      
                        });

                        $scope.totalOccupy = totalCounter / $scope.totalCountOfRent;
      
 




                        $scope.listTemplateDate = result.listTemplateDate;
                        colTotal = result.listTemplateDate.length;
                        $scope.Sazes = result.Sazes;
                        $scope.comboItem.items = result.Sazes;
                     
                        $scope.contacts = result.contacts;
                        
                        $scope.comboContact.items = $scope.contacts;


                         //angular.forEach($scope.listGoroheSaze, function (value1, key1) {
                         //       //console.log("1:", value);

                         //       angular.forEach(value1.Items, function (value2, key2) {
                         //           //console.log("2:", value);

                         //           angular.forEach(value2.SazeOfContractInTimes, function (value3, key3) {
                         //               //console.log("3:", value);
     
                  
                                        

                         //           }, log);
                         //       }, log);

                         //   }, log);

                        //$scope.comboGroupItem = new HesabfaCombobox({
                        //    items: $scope.listGoroheSaze,
                        //    containerEle: document.getElementById("comboGroupItem"),
                        //    toggleBtn: true,
                        //    newBtn: false,
                        //    deleteBtn: false,
                        //    itemClass: "hesabfa-combobox-item",
                        //    activeItemClass: "hesabfa-combobox-activeitem",
                        //    itemTemplate: $rootScope.Hesabfa.comboGroupSazeTemplate,
                        //    inputClass: "form-control input-sm input-grid-factor",
                        //    matchBy: "item.DetailAccount.Id",
                        //    displayProperty: "{Title}",
                        //    searchBy: ["Title", "DetailAccount.Code"],
                        //    onSelect: $scope.itemSelect,

                        //    divider: true
                        //});


                        //angular.element(document).ready(function () {
                        //    if (moveItemEdit) $scope.moveRowEditor(0);
                        //});

                    }).fail(function (error) {
                        $scope.loading = false;
                        applyScope($scope);
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error, title: "خطا" });
                    });

                });

            };
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);

            function applyScope($scope) {
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }

            $scope.filter = function (model) {
                //if (!angular.isDefined(model.StartDisplayDate) && model.StartDisplayDate == null) {
                //    messageService.error("تاریخ شروع را وارد نمایید.");
                //    return;
                //}

                //if (!angular.isDefined(model.EndDisplayDate) && model.EndDisplayDate == null) {
                //    messageService.error("تاریخ شروع را وارد نمایید.");
                //    return;
                //}

                //model.IsDefault = false;

                $scope.loadRockData();
                $('#editCashModal').modal('hide');

            };

            $scope.closePopup = function (model) {

                model.EndDisplayDate = null;
                model.StartDisplayDate = null;
                $scope.comboItem.setDelete();
         
                $scope.comboContact.setDelete();
       

                $('#editCashModal').modal('hide');


            };
            
            $scope.showTooltip = function (id, type) {



                $(document).mousemove(function (event) {
                    var height = $('#tooltip' + id).css("height");


                    var y = event.clientY - parseInt(height);

                    if (type === 1) {
                        $('#tooltip' + id).css('opacity', '1');
                        $('#tooltip' + id).css('left', event.clientX + 'px');
                        $('#tooltip' + id).css('top', y + 'px');
                    }

                    else
                        $('#tooltip' + id).css('opacity', '0');

                });
            };

            $scope.showTooltip2 = function (id, type) {



                $(document).mousemove(function (event) {

                    var height = $('#tooltip2' + id).css("height");


                    var y = event.clientY - parseInt(height);

                    if (type === 1) {
                        $('#tooltip2' + id).css('opacity', '1');
                        $('#tooltip2' + id).css('left', event.clientX + 'px');
                        $('#tooltip2' + id).css('top', y + 'px');
                    }

                    else
                        $('#tooltip2' + id).css('opacity', '0');

                });
            };

            $scope.showTooltip3 = function (id, type) {



                $(document).mousemove(function (event) {

                    var height = $('#tooltip3' + id).css("height");


                    var y = event.clientY - parseInt(height);

                    if (type === 1) {
                        $('#tooltip3' + id).css('opacity', '1');
                        $('#tooltip3' + id).css('left', event.clientX + 'px');
                        $('#tooltip3' + id).css('top', y + 'px');
                    }

                    else
                        $('#tooltip3' + id).css('opacity', '0');

                });
            };

            $scope.showTooltipSaze = function (id, type) {

                $(document).mousemove(function (event) {


                    if (type === 1) {
                        $('#tooltipSaze' + id).css('opacity', '1');
                        $('#tooltipSaze' + id).css('left', event.clientX + 'px');
                        $('#tooltipSaze' + id).css('top', event.clientY + 'px');
                    }

                    else
                        $('#tooltipSaze' + id).css('opacity', '0');

                });
            };


            $scope.redirect = function (id, date, itemID, templateDate) {



                if ($('#' + itemID + templateDate).css("background-color") === "rgb(0, 0, 255)") {
                    questionbox({
                        title: "هشدار",
                        content: "آیا می خواهید پیش قراداد ثبت کنید؟",
                        btn1Title: "بله",
                        btn2Title: "خیر",
                        onBtn1Click: function () {

                            $scope.lstSaze = [{}];

                            angular.forEach(lst, function (value, key) {

                                var item = {
                                    sazeID: value.row,
                                    minDate: value.minDate,
                                    maxDate: value.maxDate
                                };

                                if (!finded($scope.lstSaze, item))
                                    $scope.lstSaze.push(item);
                            });

                            $state.go("newContract", { id: id, displayDate: date, lstSaze: $scope.lstSaze });
                        }
                    });
                }
                else {
                    $('#' + itemID + templateDate).css('background-color', 'blue');
                }






            };
            $scope.redirectNewPreContract = function (id) {
                $scope.lstSaze = [{}];

                angular.forEach(lst, function (value, key) {

                    var item = {
                        sazeID: value.row,
                        minDate: value.minDate,
                        maxDate: value.maxDate
                    };

                    if (!finded($scope.lstSaze, item))
                        $scope.lstSaze.push(item);
                });

                $state.go("newContract", { id: id, lstSaze: $scope.lstSaze });
                     
            };

            $scope.cellSelect = function (id, date, itemID, templateDate) {

                if ($('#' + itemID + templateDate).css("background-color") === "rgb(0, 0, 255)") {
                    $('#' + itemID + templateDate).css('background-color', '#ffffff');

                    var item = {
                        row: itemID,
                        col: templateDate,
                        date: date
                    };
                    //var index = lst.indexOf(item, 0);
                    findAndRemoveLst(lst, item);
                    //lst.splice(index, 1);  
                }
                else {
                    $('#' + itemID + templateDate).css('background-color', 'blue');

                    var item2 = {
                        row: itemID,
                        col: templateDate,
                        date: date
                    };
                    lst.push(item2);

                    var lstFilter = $filter("filter")(lst, { row: itemID });

                    if (lstFilter.length === 2)
                    {
                        var minCol = templateDate;
                        var maxCol = templateDate;
                        var minDate = "";
                        var maxDate = "";

                        var log;
                        angular.forEach(lstFilter, function (value, key) {
                            if (minCol >= value.col)
                            {
                                minCol = value.col;
                                minDate = value.date;
                            }

                            if (maxCol <= value.col) {
                                maxCol = value.col;
                                maxDate = value.date;
                            }

                        }, log);

                        for (var j = 0; j < colTotal; j++) {
                            item2 = {
                                row: itemID,
                                col: j
                            };
                            findAndRemoveLst(lst, item2);
                            $('#' + itemID + j).css('background-color', '#ffffff');
                        }

                        for (var i = minCol; i <= maxCol; i++) {
                             item2 = {
                                row: itemID,
                                col: i,
                                minDate: minDate,
                                maxDate: maxDate
                      
                            };
                        

                            var finded = $filter("filter")(lst, { row: itemID, col: i });

                            if (finded.length === 0)
                            {
                                lst.push(item2);
                                $('#' + itemID + i).css('background-color', 'blue');
                            }
                        };
                    }

                    else if (lstFilter.length > 2)
                    {
                        for (var k = 0; k < colTotal; k++) {
                            item2 = {
                                row: itemID,
                                col: k
                            };
                            findAndRemoveLst(lst, item2);
                            $('#' + itemID + k).css('background-color', '#ffffff');
                        }

                        $('#' + itemID + templateDate).css('background-color', 'blue');

                        item2 = {
                            row: itemID,
                            col: templateDate
                        };
                        lst.push(item2);
                    }

                }

          
            };
            $scope.checkGroup = function (id) {

                if ($('#group' + id).css("display") === "none")
                    $('#group' + id).css('display', 'block');
                else
                    $('#group' + id).css('display', 'none');

                if ($('#group2' + id).css("display") === "none")
                    $('#group2' + id).css('display', 'block');
                else
                    $('#group2' + id).css('display', 'none');

                if ($('#totalInfo' + id).css("display") === "none")
                    $('#totalInfo' + id).css('display', 'block');
                else
                    $('#totalInfo' + id).css('display', 'none');

                if ($('#totalInfo2' + id).css("display") === "none")
                    $('#totalInfo2' + id).css('display', 'block');
                else
                    $('#totalInfo2' + id).css('display', 'none');

            };

            $scope.popup = function () {
                $('#editCashModal').modal({ keyboard: false }, 'show');
            };

            $scope.addNewReservation = function () {
 
                $scope.lstSaze = [{}];

                angular.forEach(lst, function (value, key) {

                    var item = {
                        sazeID: value.row,
                        minDate: value.minDate,
                        maxDate: value.maxDate
                    };

                    if (!finded($scope.lstSaze, item))
                        $scope.lstSaze.push(item);
                });

                $scope.reservation = null;
                $scope.editReservationModal = true;
            };

            $scope.editReservation = function (reservation) {
                $scope.reservation = reservation;
                $scope.editReservationModal = true;
            };

            $scope.getEditedReservation = function (reservation) {
                //var finded = findAndReplace($scope.cashes, cash.ID, cash);
                //if (!finded) $scope.cashes.push(cash);
                //$scope.$apply();
                $scope.loadRockData();
            };

            $scope.edit = function () {
                $scope.reservation = {
                    ID: $scope.reservationID
                };
                
                $scope.editReservationModal = true;
            };

            $scope.getID = function (reservationID, reservationDetailID) {
                $scope.reservationID = reservationID;
                $scope.reservationDetailID = reservationDetailID;
            };
            $scope.getIDCheckedIn = function (contractID, sazeID) {
                $scope.stop_ContractID = contractID;
                $scope.stop_SazeID = sazeID;
            };
            
            $scope.delete = function () {
              
                
                questionbox({
                    content: "آیا از حذف " + "آیتم رزرو" + " انتخاب شده مطمئن هستید؟",
                    onBtn1Click: function () {
                        if ($scope.calling) return;
                        $scope.calling = true;
                        var strIds = $scope.reservationDetailID;


                        $.ajax({
                            type: "POST",
                            data: JSON.stringify(strIds),
                            url: "/app/api/Reservation/DeteleDetailReservations",
                            contentType: "application/json"
                        }).done(function (res) {

                            // var res = result.data;
                            $scope.calling = false;
                            if (res.resultCode === 1) {
                                alertbox({ content: res.data, type: "warning" });
                                return;
                            }
                            //for (var i = 0; i < items.length; i++)
                            //    findAndRemove(grid._options.dataSource, items[i]);

                            var strMessage = "ایتم ";
                            strMessage += "انتخاب شده حذف " + "شد ";
                            DevExpress.ui.notify(strMessage, "success", 3000);
                            $scope.loadRockData();
                            $scope.$apply();
                        }).fail(function (error) {
                            $scope.calling = false;
                            if ($scope.accessError(error)) { applyScope($scope); return; }
                            $scope.$apply();
                            alertbox({ content: error });
                        });
                    }
                });
            };

            $scope.deleteAll = function () {
                questionbox({
                    content: "آیا از حذف " + "کل رزرو" + " انتخاب شده مطمئن هستید؟",
                    onBtn1Click: function () {
                        if ($scope.calling) return;
                        $scope.calling = true;
                        var strIds = $scope.reservationID;


                        $.ajax({
                            type: "POST",
                            data: JSON.stringify(strIds),
                            url: "/app/api/Reservation/DeleteReservations",
                            contentType: "application/json"
                        }).done(function (res) {

                            // var res = result.data;
                            $scope.calling = false;
                            if (res.resultCode === 1) {
                                alertbox({ content: res.data, type: "warning" });
                                return;
                            }

                            $scope.loadRockData();
                            //var log = [];
                            //angular.forEach($scope.listGoroheSaze, function (value1, key1) {
                            //    //console.log("1:", value);

                            //    angular.forEach(value1.Items, function (value2, key2) {
                            //        //console.log("2:", value);

                            //        angular.forEach(value2.SazeOfContractInTimes, function (value3, key3) {
                            //            //console.log("3:", value);
                            //            if (value3.ID === $scope.reservationDetailID)
                            //            {
                            //                findAndRemove(value2.SazeOfContractInTimes, value3);
                            //            }
                                        
                            //        }, log);
                            //    }, log);

                            //}, log);
                            //for (var i = 0; i < items.length; i++)
                            //    findAndRemove(grid._options.dataSource, items[i]);

                            var strMessage = "ایتم ";
                            strMessage += "انتخاب شده حذف " + "شد ";
                            DevExpress.ui.notify(strMessage, "success", 3000);

                            $scope.$apply();
                        }).fail(function (error) {
                            $scope.calling = false;
                            if ($scope.accessError(error)) { applyScope($scope); return; }
                            $scope.$apply();
                            alertbox({ content: error });
                        });
                    }
                });


            };

            $scope.update = function () {
                questionbox({
                    content: "در صورت تمدید رزرو تاریخ شروع رزرو از امروز شروع می شود. آیا می خواهید این کار را انجام دهید؟",
                    onBtn1Click: function () {
                        if ($scope.calling) return;
                        $scope.calling = true;
                        var strIds = $scope.reservationID;


                        $.ajax({
                            type: "POST",
                            data: JSON.stringify(strIds),
                            url: "/app/api/Reservation/TamdidReservations",
                            contentType: "application/json"
                        }).done(function (res) {

                            // var res = result.data;
                            $scope.calling = false;
                            if (res.resultCode === 1) {
                                alertbox({ content: res.data, type: "warning" });
                                return;
                            }
                            //for (var i = 0; i < items.length; i++)
                            //    findAndRemove(grid._options.dataSource, items[i]);

                            var strMessage = "ایتم ";
                            strMessage += "انتخاب شده حذف " + "شد ";
                            DevExpress.ui.notify(strMessage, "success", 3000);

                            $scope.$apply();
                        }).fail(function (error) {
                            $scope.calling = false;
                            if ($scope.accessError(error)) { applyScope($scope); return; }
                            $scope.$apply();
                            alertbox({ content: error });
                        });
                    }
                });
            };

            $scope.convertToPreCpntract = function () {
                $state.go("newContract", { reservationID: $scope.reservationID });
            };

            $scope.redirectRes = function (id, date, itemID, templateDate) {

                $scope.lstSaze = [{}];

                angular.forEach(lst, function (value, key) {

                    var item = {
                        sazeID: value.row,
                        minDate: value.minDate,
                        maxDate: value.maxDate
                    };

                    if (!finded(model, item))
                        $scope.lstSaze.push(item);
                });

            };

            $scope.extendedContract = function () { };
            $scope.showContract = function () { };
            $scope.stopSazeInContract = function () {
                $scope.isStopGroup = false;
                $scope.editContractStopModal = true;
       
            };
            $scope.stopAllSazeInContract = function () {
                $scope.isStopGroup = true;
                $scope.editContractStopModal = true;
       
            };

            $scope.operationSazeInContract = function () {
                $scope.isOperationGroup = false;
                $scope.editContractOperationModal = true;

            };
            $scope.operationAllSazeInContract = function () {
                $scope.isOperationGroup = true;
                $scope.editContractOperationModal = true;

            };

            function findAndRemoveLst(arr, obj) {
                if (!obj)
                    return null;
                for (var i = 0; i < arr.length; i++) {
                    if (arr[i].row === obj.row && arr[i].col === obj.col) {
                        var index = arr.indexOf(arr[i]);
                        if (index > -1) {
                            arr.splice(index, 1);
                            break;
                        }
                    }
                }
            }

            function finded(arr, obj) {
                if (!obj)
                    return null;
                for (var i = 0; i < arr.length; i++) {
                    if (arr[i].sazeID === obj.sazeID) {
                        return true;
                    }
               
                }
                return false;
            }
        }]);
}); 