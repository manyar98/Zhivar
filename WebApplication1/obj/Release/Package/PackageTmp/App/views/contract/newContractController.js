define(['application', 'dataService', 'combo', 'scrollbar', 'helper', 'editChap', 'editSaze', 'editContact', 'nodeSelect', 'goroheSazeSelect', 'vahedSelect', 'noeSazeSelect', 'noeEjareSelect', 'dx', 'digitGroup', 'number', 'combo', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker', 'maskedinput'], function (app) {
    app.register.controller('newContractController', ['$scope','$rootScope', '$stateParams', '$location', '$state', '$rootScope', 'dataService',
        function ($scope, $rootScope, $stateParams, $location, $state, $rootScope, dataService) {

            var isCartable = false;

            $scope.operationAccess = dataService.getOperationAccess('Contract');
            if (!$scope.operationAccess.canView)
                return;


            //var invoiceDueDateObj = new AMIB.persianCalendar('invoiceDueDate');
           // var invoiceDateObj = new AMIB.persianCalendar('invoiceDate');
            //var invoiceDateObj = new AMIB.persianCalendar('invoiceDate', {
            //    onchange: function (pdate) {
            //        if (pdate) {
            //            alert(pdate.join('/'));
            //        } else {
            //            alert('تاریخ واردشده نادرست است');
            //        }
            //    }
            //}
            //);

            


            $scope.change = function () {
                $scope.hasRent();    
            };

            $scope.changeQuantity = function () {
                $scope.actRow.calcTax = true;

                $scope.hasRent();
            };
            

            
            $scope.automaticNumber = true;

            $scope.listnasabs = [];
            $scope.listtarahs = [];
            $scope.listbazareabs = [];
            $scope.listchapkhanes = [];
            $scope.listnoeChaps = [];


            $scope.curentRowIndex = -1;


         

            $("#taxSwitch").dxSwitch({
                onText: 'محاسبه مالیات',
                offText: 'بدون مالیات',
                width: 110,
                value: true,
                onValueChanged: function (data) {
                    $scope.autoCalTax = data.value;
                    $scope.calculateInvoice(true);
                }

            });


            $scope.taxSwitchChecked = function (data) {

                $scope.autoCalTax = data;
                $scope.calculateInvoice(true);
            };

            $scope.chapSwitchChecked = function (data, index) {
                // $scope.activeIvoiceItem = invoiceItem;
                // $scope.alert = false;
                var rowIndex = $scope.actRow.RowNumber;
                var test = $scope.contract.Contract_Sazes[rowIndex];

                //$scope.item = null;

                $scope.chapkhanes = [];
                $scope.bazareabs = [];
                $scope.tarahs = [];
                $scope.nasabs = [];


                if ($scope.contract.Contract_Sazes[rowIndex].Contract_Saze_Chapkhanes)
                    $scope.chapkhanes = $scope.contract.Contract_Sazes[rowIndex].Contract_Saze_Chapkhanes;
                if ($scope.contract.Contract_Sazes[rowIndex].Contarct_Saze_Bazareabs)
                    $scope.bazareabs = $scope.contract.Contract_Sazes[rowIndex].Contarct_Saze_Bazareabs;
                if ($scope.contract.Contract_Sazes[rowIndex].Contract_Saze_Tarahs)
                    $scope.tarahs = $scope.contract.Contract_Sazes[rowIndex].Contract_Saze_Tarahs;
                if ($scope.contract.Contract_Sazes[rowIndex].Contract_Saze_Nasabs)
                    $scope.nasabs = $scope.contract.Contract_Sazes[rowIndex].Contract_Saze_Nasabs;

                $scope.curentRowIndex = rowIndex;

                $scope.editChapModal = data;
                $scope.semat = index;
                //$scope.$apply();

            };

            $scope.init = function () {
                $('#editDetailAccountModal').modal({ keyboard: false }, 'show');

                $rootScope.pageTitle("بارگیری قرارداد...");

                $("#businessNav").show();
                $("#contactSelect").hide();
                $scope.actRow = {
                    Description: '',
                    Quantity: 0,
                    UnitPrice: 0,
                    Discount: 0,
                    Tax: 0


                };
                //if (!$scope.$rootScope.Hesabfa.businesses)
                //	$scope.getCurrentUserAndBusinessInfo($scope.updatePage);
                //else $scope.updatePage();

                $scope.alert = false;
                $scope.alertMessage = "";
                $("#barcodeOK").hide();
                $("#barcodeNoOK").hide();

                $scope.comboContact = new HesabfaCombobox({
                    items: [],
                    containerEle: document.getElementById("comboContact"),
                    toggleBtn: true,
                    newBtn: true,
                    deleteBtn: true,
                    itemClass: "hesabfa-combobox-item",
                    activeItemClass: "hesabfa-combobox-activeitem",
                    itemTemplate: $rootScope.Hesabfa.comboContactTemplate,
                    divider: true,
                    matchBy: "contact.DetailAccount.ID",
                    displayProperty: "{Name}",
                    searchBy: ["Name", "Code"],
                    onSelect: $scope.contactSelect,
                    onNew: $scope.newContact
                });
                $scope.comboItem = new HesabfaCombobox({
                    items: [],
                    containerEle: document.getElementById("comboItem"),
                    toggleBtn: true,
                    newBtn: true,
                    deleteBtn: true,
                    itemClass: "hesabfa-combobox-item",
                    activeItemClass: "hesabfa-combobox-activeitem",
                    itemTemplate: $rootScope.Hesabfa.comboSazeTemplate,
                    inputClass: "form-control input-sm input-grid-factor",
                    matchBy: "item.DetailAccount.Id",
                    displayProperty: "{Title}",
                    searchBy: ["Title", "DetailAccount.Code"],
                    onSelect: $scope.itemSelect,
                    onNew: $scope.newItem,
                    onDelete: $scope.deleteItem,
                    divider: true
                });

                $scope.comboNoeEjare = new HesabfaCombobox({
                    items: [],
                    containerEle: document.getElementById("comboNoeEjare"),
                    toggleBtn: true,
                    newBtn: true,
                    deleteBtn: true,
                    itemClass: "hesabfa-combobox-item",
                    activeItemClass: "hesabfa-combobox-activeitem",
                    itemTemplate: $rootScope.Hesabfa.comboNoeEjareTemplate,
                    inputClass: "form-control input-sm input-grid-factor",
                    matchBy: "ID",
                    displayProperty: "{Title}",
                    searchBy: ["Title"],
                    onSelect: $scope.noeEjareSelect,
                    onNew: $scope.newNoeEjare,
                    // onDelete: $scope.deleteItem,
                    divider: true
                });

                if ($stateParams.reservationID !== null && $stateParams.reservationID > 0)
                {
                    $scope.loadReservationData($stateParams.reservationID);
                }
                // اگر جدید بود
                else if ($stateParams.id === "new") {
                    $scope.automaticNumber = false;
                    $scope.loadInvoiceData(0, 2);
            
                }
                // اگر ویرایش یک قرارداد بود
                else
                    $scope.loadInvoiceData($stateParams.id, null);

                $(".input-grid-factor").keydown(function (event) {
                    if (event.which === 38) {    // up key
                        $scope.goToTopRow($scope.actRow, event);
                    }
                    else if (event.which === 40) {  // down key
                        $scope.goToBottomRow($scope.actRow, event);
                    }
                });
                $("#inputTax").keydown(function (event) {
                    if (event.which === 9) {    // tab key
                        $scope.tabToNextRow($scope.actRow, event);
                    }
                });
          
            };


            $scope.loadInvoiceData = function (id, contractType) {
                $scope.loading = true;

                var lstSaze = null;

                if (angular.isDefined($stateParams.lstSaze) && $stateParams.lstSaze !== null) {
                    lstSaze = $stateParams.lstSaze;
                }

                 

                var data = { id: id, contractType: contractType, lstSaze: lstSaze };
                $(function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(data),
                        url: "/app/api/Contract/loadContractData",
                        contentType: "application/json"
                    }).done(function (res) {
                        var result = res.data;
                        var moveItemEdit = false;
                        $scope.contractSettings = result.contractSettings;
                        $scope.footerNote = result.contractSettings.footerNote;
                        $scope.autoCalTax = result.contract.AutoTax;// $scope.contractSettings.autoAddTax;
                        applyScope($scope);

                        $scope.loading = false;
                        moveItemEdit = true;

                        $scope.contract = result.contract;
                        // $scope.contract.Items = [];
                        $scope.contacts = result.contacts;

                        $scope.listnasabs = result.nasabs;
                        $scope.listtarahs = result.tarahs;
                        $scope.listbazareabs = result.bazaryabs;
                        $scope.listchapkhanes = result.chapkhanes;
                        $scope.listnoeChaps = result.noeChaps;
                        $scope.cashes = result.cashes;
                        $scope.banks = result.banks;

                        if ($scope.cashId) $scope.addCashPay();
                        if ($scope.bankId) $scope.addBankPay();

                        $scope.comboContact.items = result.contacts;
                        $scope.items = result.items;
                        $scope.comboItem.items = result.items;
                        $scope.comboNoeEjare.items = result.noeEjares;
                        //$scope.comboBazaryab.items = result.items;
                        // $scope.comboTarah.items = result.items;
                        //  $scope.comboNasb.items = result.items;
                        // $scope.comboChap.items = result.items;
                        $scope.itemNodes = result.itemNodes;
                        $scope.invoiceItem = $rootScope.Hesabfa.newContractSazeItemObj();
                        $scope.item = $rootScope.Hesabfa.newItemObj();
                        $scope.contact = $rootScope.Hesabfa.newContactObj();
                        $scope.currentInvoiceStatus = $scope.contract.Status;

                        $scope.sms = result.defaultSms;
                        $scope.showNote = $scope.contract.Note !== "" ? true : false;

                        $("#imgLogo").attr("src", $scope.contractSettings.businessLogo);
                        var nextInvoiceNumber;

                        nextInvoiceNumber = result.invoiceNextNumber;
                        if (nextInvoiceNumber)
                            $scope.contract.Number = nextInvoiceNumber;


                        if ($scope.contract.ID > 0 && $scope.contract.Contact)
                            $scope.comboContact.setSelected($scope.contract.Contact);

                        if ($scope.contract.Status === 0) {
                            var settingAutoSave = $rootScope.getUISetting("editInvoiceAutoSave");
                            if (settingAutoSave && settingAutoSave === "true") {
                                $scope.setAutoSave(true);
                                $scope.autoSaveInvoice = true;
                            }
                        }

                        if ($scope.contract.InvoiceType === 4)
                            $scope.autoCalTax = false;

                        $scope.setEditInvoicePageTitle();
                        applyScope($scope);
                        $scope.calculateInvoice();
                        //angular.element(document).ready(function () {
                        //    if (moveItemEdit) $scope.moveRowEditor(0);
                        //});

                        if (angular.isDefined($stateParams.displayDate) && $stateParams.displayDate !== null)
                            $scope.contract.DisplayDate = $stateParams.displayDate;

                      
                            
                        //}).fail(function (error) {
                        //	$scope.loading = false;
                        //	applyScope($scope);
                        //	if ($scope.accessError(error)) return;
                        //	alertbox({ content: error, title: "خطا" });
                        //}).loginFail(function () {
                        //	window.location = DefaultUrl.login;
                        //});
                        // Do something with the result :)
                    });
                })

            };


            $scope.loadReservationData = function (id) {
                $scope.loading = true;

               // var data = { id: id };
                $(function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(id),
                        url: "/app/api/Contract/loadReservationData",
                        contentType: "application/json"
                    }).done(function (res) {
                        var result = res.data;
                        var moveItemEdit = false;
                        $scope.contractSettings = result.contractSettings;
                        $scope.footerNote = result.contractSettings.footerNote;
                        $scope.autoCalTax = $scope.contractSettings.autoAddTax;
                        applyScope($scope);

                        $scope.loading = false;
                        moveItemEdit = true;

                        $scope.contract = result.contract;

                        $scope.contacts = result.contacts;

                        $scope.listnasabs = result.nasabs;
                        $scope.listtarahs = result.tarahs;
                        $scope.listbazareabs = result.bazaryabs;
                        $scope.listchapkhanes = result.chapkhanes;
                        $scope.listnoeChaps = result.noeChaps;
                        $scope.cashes = result.cashes;
                        $scope.banks = result.banks;

                        if ($scope.cashId) $scope.addCashPay();
                        if ($scope.bankId) $scope.addBankPay();

                        $scope.comboContact.items = result.contacts;
                        $scope.items = result.items;
                        $scope.comboItem.items = result.items;
                        $scope.comboNoeEjare.items = result.noeEjares;
                        //$scope.comboBazaryab.items = result.items;
                        // $scope.comboTarah.items = result.items;
                        //  $scope.comboNasb.items = result.items;
                        // $scope.comboChap.items = result.items;
                        $scope.itemNodes = result.itemNodes;
                        $scope.invoiceItem = $rootScope.Hesabfa.newContractSazeItemObj();
                        $scope.item = $rootScope.Hesabfa.newItemObj();
                        $scope.contact = $rootScope.Hesabfa.newContactObj();
                        $scope.currentInvoiceStatus = $scope.contract.Status;

                        $scope.sms = result.defaultSms;
                        $scope.showNote = $scope.contract.Note !== "" ? true : false;

                        $("#imgLogo").attr("src", $scope.contractSettings.businessLogo);
                        var nextInvoiceNumber;

                        nextInvoiceNumber = result.invoiceNextNumber;
                        if (nextInvoiceNumber)
                            $scope.contract.Number = nextInvoiceNumber;


                        if ( $scope.contract.Contact !== null)
                            $scope.comboContact.setSelected($scope.contract.Contact);

                        if ($scope.contract.Status === 0) {
                            var settingAutoSave = $rootScope.getUISetting("editInvoiceAutoSave");
                            if (settingAutoSave && settingAutoSave === "true") {
                                $scope.setAutoSave(true);
                                $scope.autoSaveInvoice = true;
                            }
                        }

                        if ($scope.contract.InvoiceType === 4)
                            $scope.autoCalTax = false;

                        $scope.setEditInvoicePageTitle();
                        applyScope($scope);
                        $scope.calculateInvoice();
                        angular.element(document).ready(function () {
                            if (moveItemEdit) $scope.moveRowEditor(0);
                        });

                        if (angular.isDefined($stateParams.displayDate) && $stateParams.displayDate !== null)
                            $scope.contract.DisplayDate = $stateParams.displayDate;



                        //}).fail(function (error) {
                        //	$scope.loading = false;
                        //	applyScope($scope);
                        //	if ($scope.accessError(error)) return;
                        //	alertbox({ content: error, title: "خطا" });
                        //}).loginFail(function () {
                        //	window.location = DefaultUrl.login;
                        //});
                        // Do something with the result :)
                    });
                })

            };

            $scope.moveRowEditor = function (index, $event) {
                if ($scope.actRow && $scope.actRow.RowNumber === index) return;
                $scope.actRow = $scope.contract.Contract_Sazes[index];
                $("#comboItem").prependTo("#tdItem" + index);
                $("#inputContractDate").prependTo("#tdDate" + index);
                //$("#inputDescription").prependTo("#tdDescription" + index);
                $("#inputQuantity").prependTo("#tdStock" + index);
                $("#inputUnitPrice").prependTo("#tdUnitPrice" + index);
                $("#comboNoeEjare").prependTo("#tdNoeEjare" + index);
                $("#inputBazaryab").prependTo("#tdBazaryab" + index);
                $("#inputTarah").prependTo("#tdTarah" + index);
                $("#inputNasb").prependTo("#tdNasb" + index);
                $("#inputChap").prependTo("#tdChap" + index);
                $("#inputDiscount").prependTo("#tdDiscount" + index);
                $("#inputTax").prependTo("#tdTax" + index);
                if ($scope.actRow) {
                    $scope.comboItem.setSelected($scope.actRow.Saze);
                    $scope.comboNoeEjare.setSelected($scope.actRow.NoeEjare)
                }

               // var objCal4 = new AMIB.persianCalendar('inputContractDate', {
                  //  onchange: function (pdate) {
                        //if (pdate) {
                        //    alert(pdate.join('/'));
                        //} else {
                        //    alert('تاریخ واردشده نادرست است');
                        //}
                    //}
                //}
                //);

                applyScope($scope);
                if ($event) $('#' + $event.currentTarget.id + " input").select();
            };
            $scope.outRowEditor = function () {
                $("#comboItem").prependTo("#hideme");
                $("#inputContractDate").prependTo("#hideme");
                $("#inputQuantity").prependTo("#hideme");
                $("#inputUnitPrice").prependTo("#hideme");
                $("#comboNoeEjare").prependTo("#hideme");
                $("#inputBazaryab").prependTo("#hideme");
                $("#inputTarah").prependTo("#hideme");
                $("#inputNasb").prependTo("#hideme");
                $("#inputChap").prependTo("#hideme");
                $("#inputDiscount").prependTo("#hideme");
                $("#inputTax").prependTo("#hideme");
            };
            $scope.tabToNextRow = function (actRow, event) {
                $scope.calculateInvoice();
                if ($scope.contract.Contract_Sazes.length > actRow.RowNumber) {
                    event.preventDefault();
                    $scope.moveRowEditor(actRow.RowNumber);
                    $("#itemInputitemSelectInput").select();
                }
            };

            $scope.goToTopRow = function (actRow, event) {
                $scope.calculateInvoice();
                if (actRow.RowNumber > 1) {
                    event.preventDefault();
                    $scope.moveRowEditor(actRow.RowNumber - 2);
                    event.target.select();
                }
            };
            $scope.goToBottomRow = function (actRow, event) {
                $scope.calculateInvoice();
                if (actRow.RowNumber < $scope.contract.Contract_Sazes.length) {
                    event.preventDefault();
                    $scope.moveRowEditor(actRow.RowNumber);
                    event.target.select();
                }
            };
            $scope.setEditInvoicePageTitle = function () {

                $rootScope.pageTitle("قرارداد" + ($scope.contract.Contact ? " " + $scope.contract.Contact.Name : ""));

            };

            $scope.setTitle = function () {
                var jenset = "";
                var company = "";

                if (angular.isDefined($scope.contract.Contact.Jensiat) && $scope.contract.Contact.Jensiat !== null) {
                    if ($scope.contract.Contact.Jensiat === 1)
                        jenset = " جناب آقای ";
                    else if ($scope.contract.Contact.Jensiat === 1)
                        jenset = " سرکار خانم ";
                }

                if (angular.isDefined($scope.contract.Contact.Company) && $scope.contract.Contact.Company !== null) {
                    company = " به نمایندگی " + $scope.contract.Contact.Company;

                };

                $scope.contract.ContractTitle = jenset + $scope.contract.Contact.Name + company;
            };

            $scope.selectFromTree = function () {
                $scope.itemSelectModal = true;
            };

            $scope.newContact = function () {
                $scope.alert = false;
                $scope.contact = null;
                $scope.editContactModal = true;
                //$scope.$apply();
            };
            $scope.getEditedContact = function (contact) {
                if (!contact) return;
                $scope.contacts.push(contact);
                $scope.editContactModal = false;
                $scope.contract.Contact = contact;
                $scope.contract.ContactTitle = contact.Name;
                $scope.setEditInvoicePageTitle();
                $scope.comboContact.setSelected(contact);
                //$scope.$apply();
            };

            $scope.noeEjareSelect = function (item, selectedBy) {

                $scope.activeIvoiceItem = $scope.actRow;


                //if (!item) {
                //    $scope.activeIvoiceItem.NoeEjare = null;
                //    $scope.$apply();
                //    return false;
                //}


                $scope.activeIvoiceItem.NoeEjare = item;


                //$scope.$apply();
                $scope.hasRent();

            };
            $scope.newNoeEjare = function (invoiceItem) {
                $scope.activeIvoiceItem = invoiceItem;
                $scope.alert = false;
                $scope.item = null;
                $scope.editItemModal = true;
                //$scope.$apply();
            };

            $scope.itemSelect = function (item, selectedBy) {
                $scope.activeIvoiceItem = $scope.actRow;
                $scope.activeIvoiceItem.calcTax = true;

                if (!item) {
                    $scope.activeIvoiceItem.Saze = null;
                    // $scope.activeIvoiceItem.ItemInput = "";
                    //$scope.$apply();
                    return false;
                }
                // first check repetation
                if (!$scope.allowItemRepeat) {
                    var items = $scope.contract.Contract_Sazes;
                    for (var i = 0; i < items.length; i++) {
                        if (items[i].Saze && items[i].Saze.ID === item.ID && (items[i].RowNumber !== $scope.actRow.RowNumber)) {
                            items[i].Quantity++;
                            $scope.calculateInvoice();
                            applyScope($scope);
                            // repeated item then stay, add count and select current input text then notify user
                            DevExpress.ui.notify($rootScope.Hesabfa.farsiDigit("آیتم تکراری به تعداد ردیف " + (i + 1) + " اضافه شد"), "success", 3000);
                            //					notification(Hesabfa.farsiDigit("آیتم تکراری به تعداد ردیف " + (i + 1) + " اضافه شد"));
                            $scope.comboItem.setSelected(null);

                            return false;
                        }
                    }
                }

                $scope.activeIvoiceItem.Saze = item;
            
                //برای انتخاب نوع اجاره
                $scope.comboNoeEjare.setSelected(item.NoeEjare);
                $scope.activeIvoiceItem.NoeEjare = item.NoeEjare;
                $scope.metrazh = item.Tol * item.Arz;

                if ($scope.activeIvoiceItem.Quantity === 0) $scope.activeIvoiceItem.Quantity = 1;


                //$scope.activeIvoiceItem.UnitPrice = $scope.contract.InvoiceType === 0 || $scope.contract.InvoiceType === 2 ? item.SellPrice : item.BuyPrice;
                $scope.activeIvoiceItem.UnitPrice = 0;
                $scope.calculateInvoice();
                //$scope.$apply();
                if (selectedBy === 'enter') {
                    if ($scope.activeIvoiceItem.RowNumber === $scope.contract.Contract_Sazes.length)
                        $scope.addRow();
                    $scope.moveRowEditor($scope.actRow.RowNumber);
                    //            $(':input:eq(' + ($(':input').index(this)) + ')').focus();
                    //            $('#tdItem' + activeRow.RowNumber + ' input').focus();
                    $('#itemInputitemSelectInput').select();
                    $('#itemInputitemSelectInput').val('');
                    $('#itemInputitemSelectInput').focus();
                }

                $scope.hasRent();

                return false;
            };
            $scope.newItem = function (invoiceItem) {
                $scope.activeIvoiceItem = invoiceItem;
                $scope.alert = false;
                $scope.item = null;
                $scope.editItemModal = true;
                // $scope.$apply();
            };

            $scope.newChap = function (invoiceItem) {
                $scope.activeIvoiceItem = invoiceItem;
                $scope.alert = false;
                $scope.item = null;
                $scope.editChapModal = true;
                //$scope.$apply();
            };

            $scope.deleteItem = function () {
                $scope.actRow.Saze = null;
                $scope.actRow.Description = "";
                $scope.actRow.Discount = 0;
                $scope.actRow.Tax = 0;
                $scope.actRow.Sum = 0;
                $scope.actRow.TotalAmount = 0;
                $scope.actRow.UnitPrice = 0;
                $scope.actRow.Quantity = 0;
            };
            $scope.getEditedItem = function () {
                //if (!item) return;
                //$scope.items.push(item);
                //$scope.editItemModal = false;
                //$scope.comboItem.setSelected(item);
                //$scope.itemSelect(item, "new", $scope.activeIvoiceItem);
                //applyScope($scope);


                $scope.contract.Contract_Sazes[$scope.curentRowIndex].Contract_Saze_Chapkhanes = $scope.chapkhanes;
                if ($scope.chapkhanes) {
                    var hazineh = 0;
                    angular.forEach($scope.chapkhanes, function (value, key) {
                        hazineh += parseFloat(value.TotalMoshtari);
                        value.UserID = value.User.ID;
                    });

                    $scope.contract.Contract_Sazes[$scope.curentRowIndex].PriceChap = parseFloat(hazineh);
                }
                $scope.contract.Contract_Sazes[$scope.curentRowIndex].Contarct_Saze_Bazareabs = $scope.bazareabs;
                if ($scope.bazareabs) {
                    var hazineh = 0;
                    var darsad = 0;
                    angular.forEach($scope.bazareabs, function (value, key) {
                        darsad += parseFloat(value.Darsad);
                        value.UserID = value.User.ID;
                        value.Hazine = (parseFloat(darsad) * ($scope.contract.Contract_Sazes[$scope.curentRowIndex].Quantity * $scope.contract.Contract_Sazes[$scope.curentRowIndex].UnitPrice));
                    });

                    $scope.contract.Contract_Sazes[$scope.curentRowIndex].PriceBazareab = (parseFloat(darsad) * ($scope.contract.Contract_Sazes[$scope.curentRowIndex].Quantity * $scope.contract.Contract_Sazes[$scope.curentRowIndex].UnitPrice));

                }

                $scope.contract.Contract_Sazes[$scope.curentRowIndex].Contract_Saze_Tarahs = $scope.tarahs;
                if ($scope.tarahs) {
                    var hazineh = 0;
                    angular.forEach($scope.tarahs, function (value, key) {
                        hazineh += parseFloat(value.HazineMoshtari);
                        value.UserID = value.User.ID;
                    });

                    $scope.contract.Contract_Sazes[$scope.curentRowIndex].PriceTarah = parseFloat(hazineh);
                }

                $scope.contract.Contract_Sazes[$scope.curentRowIndex].Contract_Saze_Nasabs = $scope.nasabs;
                if ($scope.nasabs) {
                    var hazineh = 0;
                    angular.forEach($scope.nasabs, function (value, key) {
                        hazineh += parseFloat(value.HazineMoshtari);
                        value.UserID = value.User.ID;
                    });

                    $scope.contract.Contract_Sazes[$scope.curentRowIndex].PriceNasab = parseFloat(hazineh);
                }
                $scope.calculateInvoice();
            };
            $scope.contactSelect = function (contact) {
                if (contact) {
                    $scope.contract.Contact = contact;
                    //			$scope.contract.Contact.DetailAccount = {};
                    //			$scope.contract.Contact.DetailAccount.Id = item.DetailAccountId;
                    //$scope.contract.ContactTitle = contact.Name;
                    $scope.setEditInvoicePageTitle();

                    $scope.setTitle();
                  

                    applyScope($scope);
                }
            };
            $scope.getContactBalance = function (balance) {
                if (balance > 0) return balance;
                else if (balance < 0) return balance * -1;
                else return 0;
            };
            $scope.getContactTashkhis = function (balance) {
                if (balance > 0) return "(بدهکار)";
                else if (balance < 0) return "(بستانکار)";
                else return "";
            };
            $scope.getContactBalanceClass = function (balance) {
                if (balance > 0) return "red";
                else if (balance < 0) return "green";
                else return "blue";
            };

            $scope.selectFromTree = function (invoiceItem) {
                $scope.activeIvoiceItem = invoiceItem;
                $scope.alert = false;
                $scope.item = null;
                $scope.itemSelectModal = true;
                applyScope($scope);
            };
            $scope.getSelectedItem = function (item) {
                if (!item) return;
                $scope.itemSelectModal = false;
                $scope.itemSelect(item, "new", $scope.activeIvoiceItem);
                $scope.comboItem.setSelected(item);
                if ($scope.actRow.RowNumber === $scope.contract.Contract_Sazes.length)
                    $scope.addRow(1);
                $scope.moveRowEditor($scope.actRow.RowNumber);
                //$scope.$apply();
            };

            $scope.calculateInvoice = function (calTax) {
                if (!$scope.contract) return;
                $scope.contract.Sum = 0;
                $scope.contract.Payable = 0;
                $scope.totalTax = 0;
                $scope.totalDiscount = 0;
                $scope.totalQuantity = 0;
                var length = $scope.contract.Contract_Sazes.length;
                var taxRate = $rootScope.currentBusiness.TaxRate;
                for (var i = 0; i < length; i++) {
                    var invoiceItem = $scope.contract.Contract_Sazes[i];
                    invoiceItem.ItemInput = invoiceItem.Item ? invoiceItem.Item.Name : ""; // new
                    invoiceItem.Quantity = parseFloat((invoiceItem.Quantity + "").replace("/", "."));
                    invoiceItem.Sum = (invoiceItem.UnitPrice * invoiceItem.Quantity) + invoiceItem.PriceNasab + invoiceItem.PriceChap + invoiceItem.PriceTarah;

                    invoiceItem.Sum = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Sum * 100) / 100 : Math.round(invoiceItem.Sum);
                    $scope.totalQuantity += invoiceItem.Quantity;

                    var strDiscount = invoiceItem.Discount.toString();
                    if (strDiscount.charAt(strDiscount.length - 1) === '%') {
                        strDiscount = strDiscount.substr(0, strDiscount.length - 1);
                        invoiceItem.Discount = invoiceItem.Sum * parseInt(strDiscount) / 100;
                    }
                    invoiceItem.Discount = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Discount * 100) / 100 : Math.round(invoiceItem.Discount);

                    if (calTax) {
                        if ($scope.contract.InvoiceType === 4)
                            invoiceItem.Tax = 0;
                        else if ($scope.autoCalTax) {
                            invoiceItem.Tax = (invoiceItem.Sum - invoiceItem.Discount) * (taxRate / 100);
                            invoiceItem.Tax = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Tax * 100) / 100 : Math.round(invoiceItem.Tax);
                        } else {
                            invoiceItem.Tax = 0;
                        }
                        invoiceItem.calcTax = false;
                    } else if ($scope.autoCalTax && invoiceItem.calcTax) {
                        invoiceItem.Tax = (invoiceItem.Sum - invoiceItem.Discount) * (taxRate / 100);
                        invoiceItem.Tax = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Tax * 100) / 100 : Math.round(invoiceItem.Tax);
                        invoiceItem.calcTax = false;
                    }

                    //			}
                    //				if ($scope.autoCalTax) {
                    //				if ($scope.autoCalTax) {
                    //					if (invoiceItem.calcTax) {
                    //						invoiceItem.Tax = (invoiceItem.Sum - invoiceItem.Discount) * (taxRate / 100);
                    //						invoiceItem.Tax = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Tax * 100) / 100 : Math.round(invoiceItem.Tax);
                    //						invoiceItem.calcTax = false;
                    //					}
                    //				} else {
                    //				}
                    //			}

                    invoiceItem.TotalAmount = invoiceItem.Sum - parseFloat(invoiceItem.Discount + "") + parseFloat(invoiceItem.Tax + "");
                    $scope.contract.Sum += invoiceItem.Sum;
                    $scope.contract.Payable += invoiceItem.TotalAmount;

                    $scope.totalTax += parseFloat(invoiceItem.Tax + "");
                    $scope.totalDiscount += parseFloat(invoiceItem.Discount + "");
                }
                $scope.contract.Payable = $rootScope.isDecimalCurrency() ? Math.round($scope.contract.Payable * 100) / 100 : Math.round($scope.contract.Payable);
                $scope.contract.Rest = $scope.contract.Payable - $scope.contract.Paid;
                applyScope($scope);
            };
            $scope.addDiscount = function () {
                var length = $scope.contract.Contract_Sazes.length;
                for (var i = 0; i < length; i++) {
                    var invoiceItem = $scope.contract.Contract_Sazes[i];
                    if (invoiceItem.Sum > 0) {
                        invoiceItem.calcTax = true;
                        invoiceItem.Discount = invoiceItem.Sum * ($scope.discount / 100);
                        invoiceItem.Discount = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Discount * 100) / 100 : Math.round(invoiceItem.Discount);
                    }
                }
                $scope.calculateInvoice();
            };
            $scope.addRow = function (n) {
                if (!n) n = 1;
                for (var i = 0; i < n; i++) {
                    var newInvoiceItem = {};
                    angular.copy($scope.invoiceItem, newInvoiceItem);
                    newInvoiceItem.RowNumber = $scope.contract.Contract_Sazes.length;// + 1;
                    $scope.contract.Contract_Sazes.push(newInvoiceItem);
                }
                //$scope.$apply();
            };
            $scope.deleteRow = function (invoiceItem) {
                if ($scope.contract.Contract_Sazes.length === 1) return;   // if only one remaind, do not delete
                // prevent removing autocompletes from page
                if ($scope.actRow.RowNumber === invoiceItem.RowNumber) {
                    if (invoiceItem.RowNumber > 1)
                        $scope.moveRowEditor(invoiceItem.RowNumber - 2);
                    else
                        $scope.moveRowEditor(invoiceItem.RowNumber);
                }

                // remove contract item from Contract_Sazes list
                var items = $scope.contract.Contract_Sazes;
                findAndRemoveByPropertyValue(items, 'RowNumber', invoiceItem.RowNumber);
                for (var i = 0; i < items.length; i++)  // add row numbers again
                    items[i].RowNumber = i + 1;
                $scope.calculateInvoice();  // recalculate contract
                //$scope.$apply();
            };

            $scope.deleteRow2 = function (index) {
                $scope.contract.Contract_PayRecevies[0].Contract_DetailPayRecevies.splice(index, 1);
                $scope.calTotal();
                applyScope($scope);
            };

            $scope.moveUpRow = function (invoiceItem) {
                var items = $scope.contract.Contract_Sazes;
                items.moveUp(invoiceItem);
                for (var i = 0; i < items.length; i++)
                    items[i].RowNumber = i + 1;
                //$scope.$apply();
            };
            $scope.moveDownRow = function (invoiceItem) {
                var items = $scope.contract.Contract_Sazes;
                items.moveDown(invoiceItem);
                for (var i = 0; i < items.length; i++)
                    items[i].RowNumber = i + 1;
                //$scope.$apply();
            };
            $scope.getReferenceTooltip = function () {
                return "شماره ارجاع";

            };
            $scope.enterTax = function (invoiceItem) {
                if (invoiceItem.Tax === 0)
                    invoiceItem.Tax = $rootScope.Hesabfa.taxRate && $rootScope.Hesabfa.taxRate > 0 ? $rootScope.Hesabfa.taxRate + "%" : "0";
                applyScope($scope);
                $("#inputTax").select();
            };

            $scope.saveTemp = function (command)
            {

                $.ajax({
                    type: "POST",
                    data: JSON.stringify($scope.contract),
                    url: "/app/api/Contract/ValidateDate",
                    contentType: "application/json"

                }).done(function (res) {

                    if (res.resultCode === 1) {
                        questionbox({
                            content: "در لیست رسانه ها تاریخ شروع اکران رسانه ی بزرگتر یا مساوی تاریخ روز است. آیا از این کار مطمین هستید؟",
                            onBtn1Click: function () {
                                if (command === "save" || command === "saveAndContinueEdit") {
                                    questionbox({
                                        content: "در صورتی که پیش قرارداد را به صورت موقت ذخیره کنید، پیش قرارداد برای مدیر ارشد ارسال نمی شود ولی قابل ویرایش خواهد بود. آیا از این کار مطمین هستید؟",
                                        onBtn1Click: function () {
                                            $scope.contract.Status = 0;
                                            $scope.saveInvoice(command);
                                        },
                                        onBtn2Click: function () {
                                            return;
                                        }
                                    });

                                } 
                            },
                            onBtn2Click: function () {
                                return;
                            }
                        });
                    }

                    

                }).fail(function (error) {

                    $scope.calling = false;
                    if ($scope.accessError(error)) { applyScope($scope); return; }
                    alertbox({ content: error, type: "warning" });
                    $scope.contract.Status = $scope.currentInvoiceStatus;
                    applyScope($scope);
                    window.scrollTo(0, 0);
                    });


           
            }

            $scope.saveInvoice = function (command) {


                if ($scope.calling) return;
                $scope.alert = false;
                $scope.alertMessage = "";
                $scope.alertMessages = [];

                if (!$scope.contract) return;
                var n = $scope.contract.Number;
                $scope.calling = true;

              
                
    
                if (command === "saveAndNext" || command === "saveAndSubmitForApproval") $scope.contract.Status = 1;


                var rowIndex = $scope.actRow.RowNumber;

                $scope.contract.AutoTax = $scope.autoCalTax;


                $.ajax({
                    type: "POST",
                    data: JSON.stringify($scope.contract),
                    url: "/app/api/Contract/SaveContract",
                    contentType: "application/json"

                }).done(function (res) {

                    if (res.resultCode === 1) {
                        $scope.calling = false;
                        if ($rootScope.accessError(res.data)) { applyScope($scope); return; }
                        alertbox({ content: $rootScope.farsiDigit(res.data), type: "warning" });
                        $scope.contract.Status = $scope.currentInvoiceStatus;
                        applyScope($scope);
                        window.scrollTo(0, 0);
                        return;
                    }

                    var result = res.data;
                    var smsSent = result.smsSent ? 1 : 0;

                    //var contract = result.contract;

                    var save = function () {
                        //var path = "";
                        if (!isCartable)
                            $state.go('contractsPrimative');
                        else
                            // message
                            DevExpress.ui.notify("قرارداد با موفقیت ثبت شد." , "success", 3000);


                        //$scope.$apply();
                        return;
                    };
                    var saveAndSubmitForApproval = function () {

                        if (!isCartable)
                            $state.go('contractsPrimative');
                           // $state.go('viewInvoice', { id: result.ID, status: "awaitingApproval" });
                        else
                            // message
                            DevExpress.ui.notify("قرارداد با موفقیت ثبت شد.", "success", 3000);

               
                        //$location.path("/ViewInvoice/status=awaitingApproval&sms=" + smsSent + "&id=" + result.id);
                        //$scope.$apply();
                        return;
                    };
                    //var approve = function () {
                    //    //$location.path("/ViewInvoice");

                    //    if (!isCartable)
                    //        $state.go('viewInvoice', { id: result.ID, status: "approved" });
                    //    else
                    //        // message
                    //        DevExpress.ui.notify("قرارداد با موفقیت ثبت شد.", "success", 3000);

                    //    //$location.path("/ViewInvoice/status=approved&sms=" + smsSent + "&id=" + result.id);
                    //    //$scope.$apply();
                    //    return;
                    //};
                    //var approveAndPrint = function () {
                    //    var status = "approved";

                    //    if (!isCartable)
                    //        $state.go('viewInvoice', { status: status, print: 1, sms: smsSent, id: result.ID });
                    //    else
                    //        // message
                    //        DevExpress.ui.notify("قرارداد با موفقیت ثبت شد.", "success", 3000);
              
                    //    //$location.path("/ViewInvoice/status=approved&print=true&sms=" + smsSent + "&id=" + result.id);
                    //    //$scope.$apply();
                    //    return;
                    //};

                    $scope.calling = false;
                    if (command === "saveAndContinueEdit") {
                        $scope.contract.ID = result.ID;
                        $scope.alert = true;
                        $scope.alertType = "success";
                        $scope.alertMessage = "قرارداد {2} ذخیره شد - {0} - مبلغ کل: {1}".
                            formatString($scope.contract.Contact.Name, $scope.money($scope.contract.Payable), $scope.contract.Number, $scope.contract.InvoiceType === 0 ? "فروش" : "خرید");
                        $scope.actRow = null;
                        $scope.outRowEditor();
                        //            		$scope.contract = $scope.contract;
                        //$scope.$apply();
                        if (rowIndex > $scope.contract.Contract_Sazes.length)
                            $scope.moveRowEditor(0);
                        else
                            $scope.moveRowEditor(rowIndex - 1);
                        return;
                    }
                    else if (command === "saveAndSubmitForApproval") {
                        saveAndSubmitForApproval();
                    }
                    else if (command === "save") {
                        if ($scope.contract.Status === 1) saveAndSubmitForApproval();
                        else save();
                    }
                    //else if (command === "approve") {
                    //    approve();
                    //}
                    //else if (command === "approveAndPrint") {
                    //    approveAndPrint();
                    //}
                    else if (command === "saveAndNext") {
                        $scope.alert = true;
                        $scope.alertType = "success";
                        $scope.alertMessage = "قرارداد {2} ذخیره شد - {0} - مبلغ کل: {1}".
                            formatString($scope.contract.Contact.Name, $scope.money($scope.contract.Payable), $scope.contract.Number, $scope.contract.InvoiceType === 0 ? "فروش" : "خرید");

                        $scope.actRow = null;
                        $scope.outRowEditor();
                        $scope.loadInvoiceData(0,2);
                        $scope.contactInput = "";
                        //$scope.$apply();

                        return;
                    }
                    //else if (command === "approveAndNext") {
                    //    $scope.alert = true;
                    //    $scope.alertType = "success";
                    //    $scope.alertMessage = "قرارداد {2} تایید شد - {0} - مبلغ کل: {1}".
                    //        formatString($scope.contract.Contact.Name, $scope.money($scope.contract.Payable), $scope.contract.Number, $scope.contract.InvoiceType === 0 ? "فروش" : "خرید");
                    //    if (result.smsSent) $scope.alertMessage += " [پیامک ارسال شد]";

                    //    $scope.actRow = null;
                    //    $scope.outRowEditor();
                    //    $scope.loadInvoiceData(0,2);
                    //    $scope.contactInput = "";
                    //    //$scope.$apply();

                    //    return;
                    //}

                }).fail(function (error) {

                    $scope.calling = false;
                    if ($scope.accessError(error)) { applyScope($scope); return; }
                    alertbox({ content: error, type: "warning" });
                    $scope.contract.Status = $scope.currentInvoiceStatus;
                    applyScope($scope);
                    window.scrollTo(0, 0);
                });


            };
            $scope.setAutoSave = function (switchOnOff) {
                if (switchOnOff) {
                    $rootScope.Hesabfa.autoSaveTimerInvoice = window.setInterval(function () {
                        $scope.saveInvoice("saveAndContinueEdit");
                    }, 60000);
                    $rootScope.setUISetting("editInvoiceAutoSave", "true");
                }
                else {
                    window.clearInterval($rootScope.Hesabfa.autoSaveTimerInvoice);
                    $rootScope.setUISetting("editInvoiceAutoSave", "false");
                }
            };
            $scope.setSendSms = function () {
                $scope.sendSmsByApprove = !$scope.sendSmsByApprove ? true : false;
                if ($scope.sendSmsByApprove)
                    $rootScope.setUISetting("sendInvoiceSms", "true");
                else
                    $rootScope.setUISetting("sendInvoiceSms", "false");
            };
            $scope.setAllowItemRepeat = function () {
                $scope.allowItemRepeat = !$scope.allowItemRepeat ? true : false;
                if ($scope.allowItemRepeat)
                    $rootScope.setUISetting("allowItemRepeatInvoice", "true");
                else
                    $rootScope.setUISetting("allowItemRepeatInvoice", "false");
            };
            $scope.setAutoCalTax = function () {
                $scope.autoCalTax = !$scope.autoCalTax ? true : false;
            };
            $scope.cancel = function () {
                window.history.back();
            };
            $scope.openSmsEditor = function () {
                $scope.sendSmsByApprove = !$scope.sendSmsByApprove ? true : false;
                $scope.editSmsModal = true;
                applyScope($scope);
            };
            $scope.getEditedSms = function (sms) {
                callws(DefaultUrl.MainWebService + "SaveSetting", { name: "SMS_DefaultInvoiceSMS", value: sms })
                    .success(function () {
                        return;
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error });
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };

            $scope.getEditedSaze = function (saze) {
                if (!saze) return;
                saze.DetailAccount =
                    {
                        Code: saze.Code,
                        Id: saze.ID,
                        Node:
                        {
                            FamilyTree: saze.Title,
                            Name: saze.Title,
                            Id: saze.ID
                        }
                    };
                $scope.items.push(saze);
                $scope.editItemModal = false;
                $scope.comboItem.setSelected(saze);
                $scope.itemSelect(saze, "new", $scope.activeIvoiceItem);
                applyScope($scope);
            };
            $scope.$on("$destroy", function () {
                window.clearInterval($rootScope.Hesabfa.autoSaveTimerInvoice);
            });

            $scope.printProformaInvoice = function () {
                var settings = jQuery.extend(true, {}, $scope.contractSettings);
                settings.footerNote = $scope.contract.Note + "\n" + settings.footerNote + "\n";
                settings.footerNoteDraft = $scope.contract.Note + "\n" + settings.footerNoteDraft + "\n";

                var business = {
                    Name: "ژیور",
                    LegalName: "ژیور",
                    Address: "",
                    PostalCode: "",
                    Fax: ""
                };

                printInvoice($scope.contract, settings, $scope.totalDiscount, $scope.totalTax, null, business, $scope.getCurrency());
            };



            $scope.addCashPay = function () {
                var newItem = Hesabfa.newReceiveAndPayItem();
                newItem.Type = 1;
                newItem.Amount = $scope.getRemindedMoney();
                //$scope.rp.Items.push(newItem);
                $scope.contract.Contract_PayRecevies[0].Contract_DetailPayRecevies.push(newItem);
                applyScope($scope);
                // $scope.calTotal();


                // receive
                setTimeout(function () {
                    var index = $scope.contract.Contract_PayRecevies[0].Contract_DetailPayRecevies.length - 1;

                    if (!$scope.comboReceiveCash) $scope.comboReceiveCash = [];
                    $scope.comboReceiveCash[index] = new HesabfaCombobox({
                        items: $scope.cashes,
                        containerEle: document.getElementById("receiveCashSelect" + index),
                        toggleBtn: true,
                        newBtn: true,
                        deleteBtn: true,
                        itemClass: "hesabfa-combobox-item",
                        activeItemClass: "hesabfa-combobox-activeitem",
                        itemTemplate: "<div> {Code} &nbsp;-&nbsp; {Name} </div>",
                        matchBy: "cash.Id",
                        displayProperty: "{Name}",
                        searchBy: ["Name", "Code"],
                        onSelect: function (cash) {
                            newItem.Cash = cash;
                        },
                        onNew: function () {
                            $scope.newCash(index, newItem);
                        }
                    });
                }, 0);

            };
            $scope.addBankPay = function () {
                var newItem = Hesabfa.newReceiveAndPayItem();
                newItem.Type = 2;
                newItem.Amount = $scope.getRemindedMoney();
                $scope.contract.Contract_PayRecevies[0].Contract_DetailPayRecevies.push(newItem);
                applyScope($scope);
                //$scope.calTotal();


                // receive
                setTimeout(function () {
                    var index = $scope.contract.Contract_PayRecevies[0].Contract_DetailPayRecevies.length - 1;
                    if (!$scope.comboReceiveBank) $scope.comboReceiveBank = [];
                    $scope.comboReceiveBank[index] = new HesabfaCombobox({
                        items: $scope.banks,
                        containerEle: document.getElementById("receiveBankSelect" + index),
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
                        onSelect: function (bank) {
                            newItem.Bank = bank;
                        },
                        onNew: function () {
                            $scope.newBank(index, newItem);
                        }
                    });
                }, 0);

            };
            $scope.addChequePay = function () {
                var newItem = Hesabfa.newReceiveAndPayItem();
                newItem.Type = 3;
                newItem.Cheque = { id: 0, Amount: 0, Status: 0 };
                newItem.Cheque.Contact = $scope.contract.Contact;
                newItem.Contact = $scope.contract.Contact;
                newItem.Amount = $scope.getRemindedMoney();
                newItem.Cheque.Amount = newItem.Amount;
                $scope.contract.Contract_PayRecevies[0].Contract_DetailPayRecevies.push(newItem);
                applyScope($scope);
                //$scope.calTotal();


                setTimeout(function () {
                    var index = $scope.contract.Contract_PayRecevies[0].Contract_DetailPayRecevies.length - 1;
                    if (!$scope.comboReceiveBank) $scope.comboReceiveBank = [];
                    $scope.comboReceiveBank[index] = new HesabfaCombobox({
                        items: $scope.banks,
                        containerEle: document.getElementById("receiveBankSelect" + index),
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
                        onSelect: function (bank) {
                            newItem.Bank = bank;
                        },
                        onNew: function () {
                            $scope.newBank(index, newItem);
                        }
                    });
                }, 0);

            };

            $scope.getRemindedMoney = function () {
                var currentMoneyInTransactions = 0;
                if ($scope.contract.Contract_PayRecevies[0].Contract_DetailPayRecevies && $scope.contract.Contract_PayRecevies[0].Contract_DetailPayRecevies.length > 0) {
                    var l = $scope.contract.Contract_PayRecevies[0].Contract_DetailPayRecevies.length;
                    for (var i = 0; i < l; i++)
                        currentMoneyInTransactions += parseFloat($scope.contract.Contract_PayRecevies[0].Contract_DetailPayRecevies[i].Amount);
                }
                var rest = $scope.contract.Payable - currentMoneyInTransactions;
                return rest >= 0 ? rest : 0;
            };

            //$scope.savePayment = function () {
            //    if ($scope.calling) return;
            //    $scope.alert = false;
            //    $scope.calling = true;
            //    $scope.asignChequeDatesAndContact();
            //    $scope.rp.Contact = $scope.invoice.Contact;
            //    $scope.rp.Description = $scope.invoice.Number + " دریافت/پرداخت وجه بابت صورتحساب شماره ";

            //    $.ajax({
            //        type: "POST",
            //        data: JSON.stringify($scope.rp),
            //        url: "/app/api/Contract_PayRecevie/SaveReceiveAndPay",
            //        contentType: "application/json"
            //    }).done(function (res) {
            //        $scope.calling = false;
            //        $scope.invoice.Status = 3;
            //        // message
            //        $scope.alert = true;
            //        $scope.alertType = "success";
            //        $scope.alertMessage = "پرداخت با موفقیت ثبت شد. مبلغ: " + $scope.money($scope.total);
            //        // clean form for new entry
            //        $scope.rp.Items = [];
            //        $scope.calTotal();
            //        // reload invoice
            //        $scope.loadInvoice($scope.invoice.ID);
            //        applyScope($scope);
            //    }).fail(function (error) {
            //        $scope.calling = false;
            //        applyScope($scope);
            //        if ($scope.accessError(error)) return;
            //        DevExpress.ui.notify(Hesabfa.farsiDigit(error), "error", 3000);
            //    });
            //};

            $scope.pdfProformaInvoice = function (asString, doAfterRead) {
                if ($scope.contract.Status === 0 && $scope.contract.InvoiceType === 0)
                    $scope.contractSettings.footerNote = $scope.footerNote + "\n" + $scope.contract.Note;

                var business = {

                    Address: "",
                    BusinessLine: "",
                    CalendarType: 0,
                    City: "",
                    Country: "",
                    Currency: 0,
                    DocCredit: 0,
                    EconomicCode: "",
                    Email: "",
                    ExpireDisplayDate: "1398/01/07",
                    Fax: "",
                    Id: 13798,
                    IsExpire: false,
                    LegalName: "",
                    Name: "ژیور",
                    NationalCode: "",
                    OrganizationType: 0,
                    Phone: "",
                    PostalCode: "",
                    RegisterationDisplayDate: "1397/12/06",
                    RegistrationNumber: "",
                    SetupStep: 7,
                    State: "",
                    SubscriptionLevel: 0,
                    TaxRate: 9,
                    Website: "",
                    setupInProgress: false,
                    token: "cW5GSAXUjtOZ-BfMy-mQVw..",

                };

                generateInvoicePDF($scope.contract, $scope.contractSettings, $scope.totalDiscount, $scope.totalTax, asString, doAfterRead, null, null, null, business, $scope.getCurrency());
            };
     
            if (angular.isDefined($stateParams.nestedstate18)) {

                nestedstate = $stateParams.nestedstate18;
                isCartable = true;
  
                $rootScope.loadCurrentBusinessAndBusinesses($scope.init);

            }
            else {
                $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
            }


            $scope.hasRent = function () {

                if ($scope.actRow.Saze && $scope.actRow.DisplayTarikhShorou &&
                    $scope.actRow.Quantity && $scope.actRow.NoeEjare)
                {
                    $scope.calling = true;

                    var model = {
                        SazeID : $scope.actRow.Saze.ID,
                        DisplayTarikhShorou: $scope.actRow.DisplayTarikhShorou,
                        Quantity: $scope.actRow.Quantity,
                        NoeEjare: $scope.actRow.NoeEjare.ID,
                        ContractID: $scope.contract.ID
                    };

                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(model),
                        url: "/app/api/Contract/HasRent",
                        contentType: "application/json"

                    }).done(function (res) {

                        $scope.calling = false;

                        if (res.resultCode === 1) {
                            
                            
                            alertbox({ content: $rootScope.farsiDigit(res.data), type: "warning" });
                   
                            applyScope($scope);
                            window.scrollTo(0, 0);
                            return;
                        }
                        else if (res.resultCode === 4)
                        {
                            alertbox({ content: $rootScope.farsiDigit(res.data), type: "warning" });

                            applyScope($scope);
                            window.scrollTo(0, 0);
                            return;
                        }
              


               
        

                    }).fail(function (error) {

                        $scope.calling = false;
                        if ($scope.accessError(error)) { applyScope($scope); return; }
                        alertbox({ content: error, type: "warning" });
       
                        applyScope($scope);
                        window.scrollTo(0, 0);
                    });
                }
                      

            };

            function applyScope($scope) {
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }

        }])
}); 