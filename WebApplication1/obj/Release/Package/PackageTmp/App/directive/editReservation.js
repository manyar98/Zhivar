define(['application','editContact'], function (app) {
    app.register.directive('editReservation', ['$compile', function ($compile) {

        return {
            restrict: 'E',
            transclude: true,
            templateUrl: '/App/template/edit-reservation.html',
            scope: {
                reservation: '=',
                onsuccess: '=',
                open: '=',
                iscreated: '=',
                lstsaze: '='
            },
            link: function (scope, element, attrs) {
                scope.initEditReservation = function () {
                    scope.alertBoxVisible = false;
                    scope.alertMessage = "";

                    scope.actRow = {
                        Quantity: 0
                    };
                    scope.invoiceItem = Hesabfa.newContractSazeItemObj();

                    if (!scope.iscreated)
                    {
                        scope.iscreated = true;
                    
                    scope.comboContactReservation = new HesabfaCombobox({
                        items: [],
                        containerEle: document.getElementById("comboContactReservation"),
                        toggleBtn: true,
                        newBtn: true,
                        deleteBtn: true,
                        itemClass: "hesabfa-combobox-item",
                        activeItemClass: "hesabfa-combobox-activeitem",
                        itemTemplate: Hesabfa.comboContactTemplate,
                        divider: true,
                        matchBy: "contact.DetailAccount.ID",
                        displayProperty: "{Name}",
                        searchBy: ["Name", "Code"],
                        onSelect: scope.contactSelect,
                        onNew: scope.newContact
                    });

                    scope.comboItemReservation = new HesabfaCombobox({
                        items: [],
                        containerEle: document.getElementById("comboItemReservation"),
                        toggleBtn: true,
                        newBtn: true,
                        deleteBtn: true,
                        itemClass: "hesabfa-combobox-item",
                        activeItemClass: "hesabfa-combobox-activeitem",
                        itemTemplate: Hesabfa.comboSazeTemplate,
                        inputClass: "form-control input-sm input-grid-factor",
                        matchBy: "item.DetailAccount.Id",
                        displayProperty: "{Title}",
                        searchBy: ["Title", "DetailAccount.Code"],
                        onSelect: scope.itemSelect,
                        onNew: scope.newItem,
                        onDelete: scope.deleteItem,
                        divider: true
                    });

                    scope.comboNoeEjareReservation = new HesabfaCombobox({
                        items: [],
                        containerEle: document.getElementById("comboNoeEjareReservation"),
                        toggleBtn: true,
                        newBtn: true,
                        deleteBtn: true,
                        itemClass: "hesabfa-combobox-item",
                        activeItemClass: "hesabfa-combobox-activeitem",
                        itemTemplate: Hesabfa.comboNoeEjareTemplate,
                        inputClass: "form-control input-sm input-grid-factor",
                        matchBy: "ID",
                        displayProperty: "{Title}",
                        searchBy: ["Title"],
                        onSelect: scope.noeEjareSelect,
                        onNew: scope.newNoeEjare,
                        // onDelete: scope.deleteItem,
                        divider: true
                    });
                    }
                    if (!scope.reservation) {
                        scope.getNewReservationObject(0);
                    } else {
                        scope.getNewReservationObject(scope.reservation.ID);
                    }
                    $('#editReservationModal').modal({ keyboard: false }, 'show');
                   // $("#editReservationModal .modal-dialog").draggable({ handle: ".modal-header" });
                };
                scope.$watch('open', function (value) {
                    if (value)
                        scope.initEditReservation();
                    scope.open = false;
                }, true);
                scope.submitReservation = function () {
                    scope.saveReservation();
                };
                scope.getNewReservationObject = function (id) {
                    scope.calling = true;

                    var model = {
                        id: id,
                        lstSaze: scope.lstsaze
                    };

                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(model),
                        url: "/app/api/Reservation/loadReservationData",
                        contentType: "application/json"
                    }).done(function (res) {

                        scope.calling = false;
                        scope.reservation = res.data.Reservation;
                        
                      

                        if (res.data.Reservation.ID > 0 && res.data.Reservation.Contact)
                            scope.comboContactReservation.setSelected(res.data.Reservation.Contact);


                        var hideDiv = document.getElementById('hideme');

                        if (!$('#comboItemReservation').length) {
                            var comboItemReservation = document.createElement('div');

                            comboItemReservation.setAttribute("id", "comboItemReservation");

                            $compile(comboItemReservation)(scope);
                            hideDiv.appendChild(comboItemReservation);

        
                            scope.comboItemReservation = new HesabfaCombobox({
                                items: [],
                                containerEle: document.getElementById("comboItemReservation"),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: Hesabfa.comboSazeTemplate,
                                inputClass: "form-control input-sm input-grid-factor",
                                matchBy: "item.DetailAccount.Id",
                                displayProperty: "{Title}",
                                searchBy: ["Title", "DetailAccount.Code"],
                                onSelect: scope.itemSelect,
                                onNew: scope.newItem,
                                onDelete: scope.deleteItem,
                                divider: true
                            });

                     
                        }

                        if (!$('#inputStartDate').length) {
                            var inputStartDate = document.createElement('input');

                            inputStartDate.setAttribute("date-picker", "");
                            inputStartDate.setAttribute("display-numbers", "");
                            inputStartDate.setAttribute("ng-model", "actRow.StartDisplayDate");
                            inputStartDate.setAttribute("data-toggle", "popover");
                            inputStartDate.setAttribute("data-placement", "top");
                            inputStartDate.setAttribute("data-trigger", "hover");
                            inputStartDate.setAttribute("data-content", "تاریخ شروع اکران...");
                            inputStartDate.setAttribute("type", "text");
                            inputStartDate.setAttribute("data-original-title", "");
                            inputStartDate.setAttribute("title", "");
                            inputStartDate.setAttribute("aria-describedby", "popover216080");
                            inputStartDate.setAttribute("class", "pdate form-control input-sm ng-valid ng-pristine ng-not-empty ng-touched");
                            inputStartDate.setAttribute("id", "inputStartDate");

                            $compile(inputStartDate)(scope);
                            hideDiv.appendChild(inputStartDate);
                        }

                        if (!$('#inputQuantity').length) {
                            var inputQuantity = document.createElement('input');

                            inputQuantity.setAttribute("display-numbers", "");
                            inputQuantity.setAttribute("keyboard-filter", "float");
                            inputQuantity.setAttribute("ng-model", "actRow.Quantity");
                            inputQuantity.setAttribute("data-toggle", "popover");
                            inputQuantity.setAttribute("data-placement", "top");
                            inputQuantity.setAttribute("data-trigger", "hover");
                            inputQuantity.setAttribute("data-content", "مدت اجاره ...");
                            inputQuantity.setAttribute("type", "text");
                            inputQuantity.setAttribute("class", "input-grid-factor input-sm");
                            inputQuantity.setAttribute("id", "inputQuantity");

                            $compile(inputQuantity)(scope);
                            hideDiv.appendChild(inputQuantity);
                        }
                        if (!$('#comboNoeEjareReservation').length) {
                            var comboNoeEjareReservation = document.createElement('div');

                            comboNoeEjareReservation.setAttribute("id", "comboNoeEjareReservation");

                            $compile(comboNoeEjareReservation)(scope);
                            hideDiv.appendChild(comboNoeEjareReservation);

                            scope.comboNoeEjareReservation = new HesabfaCombobox({
                                items: [],
                                containerEle: document.getElementById("comboNoeEjareReservation"),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: Hesabfa.comboNoeEjareTemplate,
                                inputClass: "form-control input-sm input-grid-factor",
                                matchBy: "ID",
                                displayProperty: "{Title}",
                                searchBy: ["Title"],
                                onSelect: scope.noeEjareSelect,
                                onNew: scope.newNoeEjare,
                                // onDelete: scope.deleteItem,
                                divider: true
                            });
                        }

                        if (!$('#inputUnitPrice').length) {
                            var inputUnitPrice = document.createElement('input');

                            inputUnitPrice.setAttribute("display-numbers", "");
                            inputUnitPrice.setAttribute("decimal-point-zero", "");
                            inputUnitPrice.setAttribute("keyboard-filter", "integer");
                            inputUnitPrice.setAttribute("digit-group", "");
                            inputUnitPrice.setAttribute("ng-model", "actRow.UnitPrice");
                            inputUnitPrice.setAttribute("ng-change", "actRow.calcTax=true");
                            inputUnitPrice.setAttribute("ng-blur", "calculateInvoice()");
                            inputUnitPrice.setAttribute("data-toggle", "popover");
                            inputUnitPrice.setAttribute("data-placement", "top");
                            inputUnitPrice.setAttribute("data-trigger", "hover");
                            inputUnitPrice.setAttribute("data-content", "مبلغ اجاره: ");
                            inputUnitPrice.setAttribute("type", "text");
                            inputUnitPrice.setAttribute("data-original-title", "");
                            inputUnitPrice.setAttribute("title", "");
                            inputUnitPrice.setAttribute("aria-describedby", "popover216080");
                            inputUnitPrice.setAttribute("class", "input-grid-factor input-sm");
                            inputUnitPrice.setAttribute("id", "inputUnitPrice");

                            $compile(inputUnitPrice)(scope);
                            hideDiv.appendChild(inputUnitPrice);
                        }

                        if (!$('#inputPriceBazareab').length) {
                            var inputPriceBazareab = document.createElement('input');

                            inputPriceBazareab.setAttribute("display-numbers", "");
                            inputPriceBazareab.setAttribute("decimal-point-zero", "");
                            inputPriceBazareab.setAttribute("keyboard-filter", "integer");
                            inputPriceBazareab.setAttribute("digit-group", "");
                            inputPriceBazareab.setAttribute("ng-model", "actRow.PriceBazareab");
                            inputPriceBazareab.setAttribute("ng-change", "actRow.calcTax=true");
                            inputPriceBazareab.setAttribute("ng-blur", "calculateInvoice()");
                            inputPriceBazareab.setAttribute("data-toggle", "popover");
                            inputPriceBazareab.setAttribute("data-placement", "top");
                            inputPriceBazareab.setAttribute("data-trigger", "hover");
                            inputPriceBazareab.setAttribute("data-content", "هزینه بازاریاب: ");
                            inputPriceBazareab.setAttribute("type", "text");
                            inputPriceBazareab.setAttribute("data-original-title", "");
                            inputPriceBazareab.setAttribute("title", "");
                            inputPriceBazareab.setAttribute("aria-describedby", "popover216080");
                            inputPriceBazareab.setAttribute("class", "input-grid-factor input-sm");
                            inputPriceBazareab.setAttribute("id", "inputPriceBazareab");

                            $compile(inputPriceBazareab)(scope);
                            hideDiv.appendChild(inputPriceBazareab);
                        }

                        if (!$('#inputPriceTarah').length) {
                            var inputPriceTarah = document.createElement('input');

                            inputPriceTarah.setAttribute("display-numbers", "");
                            inputPriceTarah.setAttribute("decimal-point-zero", "");
                            inputPriceTarah.setAttribute("keyboard-filter", "integer");
                            inputPriceTarah.setAttribute("digit-group", "");
                            inputPriceTarah.setAttribute("ng-model", "actRow.PriceTarah");
                            inputPriceTarah.setAttribute("ng-change", "actRow.calcTax=true");
                            inputPriceTarah.setAttribute("ng-blur", "calculateInvoice()");
                            inputPriceTarah.setAttribute("data-toggle", "popover");
                            inputPriceTarah.setAttribute("data-placement", "top");
                            inputPriceTarah.setAttribute("data-trigger", "hover");
                            inputPriceTarah.setAttribute("data-content", "هزینه طراحی: ");
                            inputPriceTarah.setAttribute("type", "text");
                            inputPriceTarah.setAttribute("data-original-title", "");
                            inputPriceTarah.setAttribute("title", "");
                            inputPriceTarah.setAttribute("aria-describedby", "popover216080");
                            inputPriceTarah.setAttribute("class", "input-grid-factor input-sm");
                            inputPriceTarah.setAttribute("id", "inputPriceTarah");

                            $compile(inputPriceTarah)(scope);
                            hideDiv.appendChild(inputPriceTarah);
                        }

                        if (!$('#inputPriceChap').length) {
                            var inputPriceChap = document.createElement('input');

                            inputPriceChap.setAttribute("display-numbers", "");
                            inputPriceChap.setAttribute("decimal-point-zero", "");
                            inputPriceChap.setAttribute("keyboard-filter", "integer");
                            inputPriceChap.setAttribute("digit-group", "");
                            inputPriceChap.setAttribute("ng-model", "actRow.PriceChap");
                            inputPriceChap.setAttribute("ng-change", "actRow.calcTax=true");
                            inputPriceChap.setAttribute("ng-blur", "calculateInvoice()");
                            inputPriceChap.setAttribute("data-toggle", "popover");
                            inputPriceChap.setAttribute("data-placement", "top");
                            inputPriceChap.setAttribute("data-trigger", "hover");
                            inputPriceChap.setAttribute("data-content", "هزینه چاپ: ");
                            inputPriceChap.setAttribute("type", "text");
                            inputPriceChap.setAttribute("data-original-title", "");
                            inputPriceChap.setAttribute("title", "");
                            inputPriceChap.setAttribute("aria-describedby", "popover216080");
                            inputPriceChap.setAttribute("class", "input-grid-factor input-sm");
                            inputPriceChap.setAttribute("id", "inputPriceChap");

                            $compile(inputPriceChap)(scope);
                            hideDiv.appendChild(inputPriceChap);
                        }

                        if (!$('#inputPriceNasab').length) {
                            var inputPriceNasab = document.createElement('input');

                            inputPriceNasab.setAttribute("display-numbers", "");
                            inputPriceNasab.setAttribute("decimal-point-zero", "");
                            inputPriceNasab.setAttribute("keyboard-filter", "integer");
                            inputPriceNasab.setAttribute("digit-group", "");
                            inputPriceNasab.setAttribute("ng-model", "actRow.PriceNasab");
                            inputPriceNasab.setAttribute("ng-change", "actRow.calcTax=true");
                            inputPriceNasab.setAttribute("ng-blur", "calculateInvoice()");
                            inputPriceNasab.setAttribute("data-toggle", "popover");
                            inputPriceNasab.setAttribute("data-placement", "top");
                            inputPriceNasab.setAttribute("data-trigger", "hover");
                            inputPriceNasab.setAttribute("data-content", "هزینه نصب: ");
                            inputPriceNasab.setAttribute("type", "text");
                            inputPriceNasab.setAttribute("data-original-title", "");
                            inputPriceNasab.setAttribute("title", "");
                            inputPriceNasab.setAttribute("aria-describedby", "popover216080");
                            inputPriceNasab.setAttribute("class", "input-grid-factor input-sm");
                            inputPriceNasab.setAttribute("id", "inputPriceNasab");

                            $compile(inputPriceNasab)(scope);
                            hideDiv.appendChild(inputPriceNasab);
                        }

                        scope.comboItemReservation.items = res.data.items;
                        scope.comboNoeEjareReservation.items = res.data.noeEjares;
                        scope.contacts = res.data.contacts;
                        scope.comboContactReservation.items = scope.contacts;
                        
                        scope.$apply();
                    });



                 
                };
                scope.getReservation = function (id) {
                    scope.calling = true;

                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(id),
                        url: "/app/api/Reservation/GetReservationById",
                        contentType: "application/json"
                    }).done(function (res) {
                        var reservation = res.data;
                        scope.calling = false;
                        scope.reservation = reservation;
                        scope.$apply();
                    }).fail(function (error) {
                        window.location = '/error.html';
                    });
                };
                scope.saveReservation = function () {
                    scope.alertBoxVisible = false;
                    scope.alertMessage = "";
                    scope.calling = true;
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(scope.reservation),
                        url: "/app/api/Reservation/SaveReservation",
                        contentType: "application/json"
                    }).done(function (res) {
                        if (res.resultCode === 0) {
                            var savedItem = res.data;
                            scope.calling = false;
                            scope.reservation = savedItem;
                            scope.onsuccess(savedItem);
                            $('#editReservationModal').modal('hide');
                        }
                        else
                        {
                            scope.calling = false;
                          //  if ($scope.accessError(res.data)) { applyScope($scope); return; }
                            alertbox({ content: res.data, type: "warning" });
                            applyScope(scope);
                            window.scrollTo(0, 0);
                        }
                        scope.$apply();
                    }).fail(function (error) {
                        scope.calling = false;
                        scope.alertBoxVisible = true;
                        scope.alertMessage = error;
                        scope.$apply();
                    });
                };

                scope.moveRowEditor = function (index, $event) {
                    if (scope.actRow && scope.actRow.RowNumber === index) return;
                    scope.actRow = scope.reservation.ReservationDetails[index];
                    $("#comboItemReservation").prependTo("#tdItem" + index);
                    $("#inputStartDate").prependTo("#tdDate" + index);
                    $("#inputQuantity").prependTo("#tdStock" + index);
                    $("#comboNoeEjareReservation").prependTo("#tdNoeEjare" + index);
                    $("#inputUnitPrice").prependTo("#tdUnitPrice" + index);
                    $("#inputPriceBazareab").prependTo("#tdBazaryab" + index);
                    $("#inputPriceTarah").prependTo("#tdTarah" + index);
                    $("#inputPriceChap").prependTo("#tdChap" + index);
                    $("#inputPriceNasab").prependTo("#tdNasb" + index);
                    if (scope.actRow) {
                        scope.comboItemReservation.setSelected(scope.actRow.Saze);
                        scope.comboNoeEjareReservation.setSelected(scope.actRow.NoeEjare);
                    }

                    //applyScope(scope);
                    if ($event) $('#' + $event.currentTarget.id + " input").select();
                };
                scope.outRowEditor = function () {
                    $("#comboItemReservation").prependTo("#hideme");
                    $("#inputStartDate").prependTo("#hideme");
                    $("#inputQuantity").prependTo("#hideme");
                    $("#comboNoeEjareReservation").prependTo("#hideme");
                    $("#inputUnitPrice").prependTo("#hideme");
                    $("#inputPriceBazareab").prependTo("#hideme");
                    $("#inputPriceTarah").prependTo("#hideme");
                    $("#inputPriceChap").prependTo("#hideme");
                    $("#inputPriceNasab").prependTo("#hideme");
                };

                scope.itemSelect = function (item, selectedBy) {
                    scope.activeIvoiceItem = scope.actRow;
          

                    if (!item) {
                        scope.activeIvoiceItem.Saze = null;
                        return false;
                    }
             
                    var items = scope.reservation.ReservationDetails;
                        for (var i = 0; i < items.length; i++) {
                            if (items[i].Saze && items[i].Saze.ID === item.ID && (items[i].RowNumber !== scope.actRow.RowNumber)) {
                                items[i].Quantity++;
             
                                //applyScope(scope);
        
                                DevExpress.ui.notify(Hesabfa.farsiDigit("آیتم تکراری به تعداد ردیف " + (i + 1) + " اضافه شد"), "success", 3000);
                                scope.comboItemReservation.setSelected(null);

                                return false;
                            }
                        }
                    

                    scope.activeIvoiceItem.Saze = item;

                    //برای انتخاب نوع اجاره
                    scope.comboNoeEjareReservation.setSelected(item.NoeEjare);
                    scope.activeIvoiceItem.NoeEjare = item.NoeEjare;
        

                    if (scope.activeIvoiceItem.Quantity === 0) scope.activeIvoiceItem.Quantity = 1;

           
                    //scope.$apply();
                    if (selectedBy === 'enter') {
                        if (scope.activeIvoiceItem.RowNumber === scope.reservation.ReservationDetails.length)
                            scope.addRow();
                        scope.moveRowEditor(scope.actRow.RowNumber);
                        //            $(':input:eq(' + ($(':input').index(this)) + ')').focus();
                        //            $('#tdItem' + activeRow.RowNumber + ' input').focus();
                        $('#itemInputitemSelectInput').select();
                        $('#itemInputitemSelectInput').val('');
                        $('#itemInputitemSelectInput').focus();
                    }
                    return false;
                };

                scope.deleteItem = function () {
                    scope.actRow.Saze = null;
                    scope.actRow.Quantity = 0;
                };

                scope.noeEjareSelect = function (item, selectedBy) {

                    scope.activeIvoiceItem = scope.actRow;

                    scope.activeIvoiceItem.NoeEjare = item;


                    //scope.$apply();


                };

                scope.addRow = function (n) {
                    if (!n) n = 1;
                    for (var i = 0; i < n; i++) {
                        var newInvoiceItem = {};
                        angular.copy(scope.invoiceItem, newInvoiceItem);
                        newInvoiceItem.RowNumber = scope.reservation.ReservationDetails.length;// + 1;
                        scope.reservation.ReservationDetails.push(newInvoiceItem);
                    }
                    //scope.$apply();
                };

                scope.deleteRow = function (invoiceItem) {
                    if (scope.reservation.ReservationDetails.length === 1) return;   // if only one remaind, do not delete
                    // prevent removing autocompletes from page
                    if (scope.actRow.RowNumber === invoiceItem.RowNumber) {
                        if (invoiceItem.RowNumber > 1)
                            scope.moveRowEditor(invoiceItem.RowNumber - 2);
                        else
                            scope.moveRowEditor(invoiceItem.RowNumber);
                    }

                    // remove contract item from Contract_Sazes list
                    var items = scope.reservation.ReservationDetails;
                    findAndRemoveByPropertyValue(items, 'RowNumber', invoiceItem.RowNumber);
                    for (var i = 0; i < items.length; i++)  // add row numbers again
                        items[i].RowNumber = i + 1;

                    //scope.$apply();
                };

                scope.contactSelect = function (contact) {
                    if (contact) {
                        scope.reservation.Contact = contact;
                    }
                };

                scope.newContact = function () {
                    
                    scope.contact = null;
                    scope.editContactModal = true;
                 
                };

                scope.getEditedContact = function (contact) {
                    if (!contact) return;
                    scope.contacts.push(contact);
                    scope.editContactModal = false;
                    scope.reservation.Contact = contact;
                    scope.reservation.ContactTitle = contact.Name;
                    //scope.setEditInvoicePageTitle();
                    scope.comboContactReservation.setSelected(contact);
                    //$scope.$apply();
                };

            }
        };
    }]);
});