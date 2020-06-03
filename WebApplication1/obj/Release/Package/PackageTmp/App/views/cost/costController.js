define(['application', 'combo', 'scrollbar', 'helper', 'editContact', 'editCost', 'nodeSelect', 'dx', 'digitGroup', 'number', 'combo', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker'], function (app) {
    app.register.controller('costController', ['$scope','$rootScope', '$stateParams', '$location', '$state',
        function ($scope, $rootScope, $stateParams, $location, $state) {

            //$scope.message = "omid mohammadi";

            var costDueDateObj = new AMIB.persianCalendar('costDueDate');
            var costDateObj = new AMIB.persianCalendar('costDate');
    


            $scope.init = function () {
                $('#editDetailAccountModal').modal({ keyboard: false }, 'show');
                $rootScope.pageTitle("بارگیری صورت هزینه...");
                $("#businessNav").show();
                $("#contactSelect").hide();
                $scope.actRow = {
                    Description: '',
                    Quantity: 0,
                    UnitPrice: 0,
                    Discount: 0,
                    Tax: 0
                };
                //if (!$scope.$scope.Hesabfa.businesses)
                //	$scope.getCurrentUserAndBusinessInfo($scope.updatePage);
                //else $scope.updatePage();

                $scope.alert = false;
                $scope.alertMessage = "";


                $scope.comboContact = new HesabfaCombobox({
                    items: [],
                    containerEle: document.getElementById("comboContact"),
                    toggleBtn: true,
                    newBtn: true,
                    deleteBtn: true,
                    itemClass: "hesabfa-combobox-item",
                    activeItemClass: "hesabfa-combobox-activeitem",
                    itemTemplate: "<div class='hesabfa-combobox-title-blue'> {DetailAccount.Code} &nbsp;-&nbsp; {Name} </div>" +
                    "<div class='small'> {DetailAccount.Node.FamilyTree} </div>",
                    divider: true,
                    matchBy: "contact.DetailAccount.ID",
                    displayProperty: "{Name}",
                    searchBy: ["Name", "Code"],
                    onSelect: $scope.contactSelect,
                    onNew: $scope.newContact
                });

                
                $scope.newCostItemObj = function () {
                    var costItem = {
                        Description: "",
                        Title: "",
                        Sum: 0,
                        Rest: 0,
                        RowNumber: 0
                    };
                    return costItem;
                };
                $scope.newItemObj = function () {
                    var item = {
                        Name: "",
                        ItemType: 0,
                        Unit: "",
                        Barcode: "",
                        PurchasesTitle: "",
                        SalesTitle: "",
                        Stock: 0,
                        MinStock: 0,
                        BuyPrice: 0,
                        SellPrice: 0,
                        WeightedAveragePrice: 0,
                        DetailAccount: null,
                        FinanYear: null
                    };
                    return item;
                };

                $scope.newContactObj = function () {
                    var c = {
                        Name: "",
                        ContactType: 0,
                        NationalCode: "",
                        EconomicCode: "",
                        RegistrationNumber: "",
                        FirstName: "",
                        LastName: "",
                        Email: "",
                        People: "",
                        Address: "",
                        City: "",
                        State: "",
                        PostalCode: "",
                        Phone: "",
                        Fax: "",
                        Mobile: "",
                        ContactEmail: "",
                        Website: "",
                        Rating: 0,
                        IsCustomer: false,
                        IsVendor: false,
                        IsEmployee: false,
                        IsShareHolder: false,
                        Liability: 0,
                        Credits: 0,
                        Note: "",
                        HesabfaKey: 0,
                        SharePercent: 0,
                        DetailAccount: null,
                        FinanYear: null
                    }
                    return c;
                };
                $scope.comboItem = new HesabfaCombobox({
                    items: [],
                    containerEle: document.getElementById("comboItem"),
                    toggleBtn: true,
                    newBtn: true,
                    deleteBtn: true,
                    itemClass: "hesabfa-combobox-item",
                    activeItemClass: "hesabfa-combobox-activeitem",
                    itemTemplate: "<div> {Coding} &nbsp;-&nbsp; {Name} </div>",
                    inputClass: "form-control input-sm input-grid-factor",
                    matchBy: "item.Id",
                    displayProperty: "{Name}",
                    searchBy: ["Name", "Coding"],
                    onSelect: $scope.itemSelect,
                    onNew: $scope.newItem,
                    onDelete: $scope.deleteItem,

     
                    divider: true
                });

                // اگر جدید بود
                if ($stateParams.id === "new" ) {
                    $scope.loadCostData(0);
                    $scope.automaticNumber = false;
                }
                else
                    $scope.loadCostData($stateParams.id);

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
    

               // var t = document.getElementById("comboItem");


                var sendCostSms = $rootScope.getUISetting("sendCostSms");
                $scope.sendSmsByApprove = !sendCostSms || sendCostSms === "false" ? false : true;
                var showCostTips = $rootScope.getUISetting("showCostTips");
                $scope.showCostTips = showCostTips && showCostTips === "true" ? true : false;
                if (showCostTips == null) $scope.showCostTips = true;
                var allowItemRepeat = $rootScope.getUISetting("allowItemRepeatCost");
                $scope.allowItemRepeat = allowItemRepeat && allowItemRepeat === "true" ? true : false;



                //$('[data-toggle="popover"]').popover();
                //$.getScript("/App/printReports.js", function () { });

                applyScope($scope);
                $(function () {
                    $.getScript("/App/printReports.js", function () { });
                });
            };

            $scope.loadCostData = function (id) {
                $scope.loading = true;

                $(function () {
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(id),
                        url: "/app/api/Cost/loadCostData",
                        contentType: "application/json"
                    }).done(function (res) {
   
                        var result = res.data;
                        var moveItemEdit = false;
                        $scope.costSettings = result.costSettings;
 
                        applyScope($scope);

                        $scope.loading = false;
                        moveItemEdit = true;

                        $scope.cost = result.cost;
                        $scope.contacts = result.contacts;
                        $scope.comboContact.items = result.contacts;
                        $scope.items = result.items;
                        $scope.comboItem.items = result.items;
                        $scope.itemNodes = result.itemNodes;
                        $scope.costItem = $scope.newCostItemObj();
                        $scope.item = $scope.newItemObj();
                        $scope.contact = $scope.newContactObj();
                        $scope.currentCostStatus = $scope.cost.Status;

                        $scope.sms = result.defaultSms;


                        $("#imgLogo").attr("src", $scope.costSettings.businessLogo);
                       
                        if ($scope.cost.ID > 0 && $scope.cost.Contact)
                            $scope.comboContact.setSelected($scope.cost.Contact);

                        //if ($scope.cost.Status === 0) {
                        //    var settingAutoSave = $rootScope.getUISetting("editCostAutoSave");
                        //    if (settingAutoSave && settingAutoSave === "true") {
                        //        $scope.setAutoSave(true);
                        //        $scope.autoSaveCost = true;
                        //    }
                        //}

                        //if ($scope.cost.InvoiceType === 4)
                        //    $scope.autoCalTax = false;

                        $scope.setEditCostPageTitle();
                        applyScope($scope);
                        $scope.calculateCost();
                        angular.element(document).ready(function () {
                            if (moveItemEdit) $scope.moveRowEditor(0);
                        });


                        }).fail(function (error) {

                            $scope.calling = false;
                            if ($scope.accessError(error)) { applyScope($scope); return; }
                            alertbox({ content: error, type: "warning" });
                            //$scope.cost.Status = $scope.currentCostStatus;
                            applyScope($scope);
                            window.scrollTo(0, 0);
                        });
                })

            };
           
            
          
            $scope.moveRowEditor = function (index, $event) {
                if ($scope.actRow && $scope.actRow.RowNumber === index) return;
                $scope.actRow = $scope.cost.CostItems[index];
                $("#comboItem").prependTo("#tdItem" + index);
               // $("#inputTitle").prependTo("#tdTitle" + index);
                $("#inputDescription").prependTo("#tdDescription" + index);
                $("#inputSum").prependTo("#tdSum" + index);
                $("#inputRest").prependTo("#tdRest" + index);
                if ($scope.actRow)
                   $scope.comboItem.setSelected($scope.actRow.Item);
                //applyScope($scope);
                if ($event) $('#' + $event.currentTarget.id + " input").select();
            };
            $scope.outRowEditor = function () {
                $("#comboItem").prependTo("#hideme");
                $("#inputDescription").prependTo("#hideme");
                //$("#inputTitle").prependTo("#hideme");
                $("#inputSum").prependTo("#hideme");
                $("#inputRest").prependTo("#hideme");
            };
            $scope.tabToNextRow = function (actRow, event) {
                $scope.calculateCost();
                if ($scope.cost.CostItems.length > actRow.RowNumber) {
                    event.preventDefault();
                    $scope.moveRowEditor(actRow.RowNumber);
                    $("#itemInputitemSelectInput").select();
                }
            };
  
            $scope.goToTopRow = function (actRow, event) {
                $scope.calculateCost();
                if (actRow.RowNumber > 1) {
                    event.preventDefault();
                    $scope.moveRowEditor(actRow.RowNumber - 2);
                    event.target.select();
                }
            };
            $scope.goToBottomRow = function (actRow, event) {
                $scope.calculateCost();
                if (actRow.RowNumber < $scope.cost.CostItems.length) {
                    event.preventDefault();
                    $scope.moveRowEditor(actRow.RowNumber);
                    event.target.select();
                }
            };
            $scope.setEditCostPageTitle = function () {
                $rootScope.pageTitle("صورتحساب هزینه");
            };
            $scope.selectFromTree = function () {
                $scope.itemSelectModal = true;
            };


            $scope.getEditedCost = function (item) {
                if (!item) return;
                $scope.items.push(item);
                $scope.editCostModal = false;
                $scope.comboItem.setSelected(item);
                //$scope.costSelect(item, "new", $scope.activeCostItem);
                applyScope($scope);
            };
            $scope.newContact = function () {
                $scope.alert = false;
                $scope.contact = null;
                $scope.editContactModal = true;
                $scope.$apply();
            };
            $scope.getEditedContact = function (contact) {
                if (!contact) return;
                $scope.contacts.push(contact);
                $scope.editContactModal = false;
                $scope.cost.Contact = contact;
                $scope.cost.ContactTitle = contact.Name;
                $scope.setEditCostPageTitle();
                $scope.comboContact.setSelected(contact);
                $scope.$apply();
            };
            $scope.itemSelect = function (item, selectedBy) {
                $scope.activeIvoiceItem = $scope.actRow;
                $scope.activeIvoiceItem.calcTax = true;

                if (!item) {
                    $scope.activeIvoiceItem.Item = null;
                    $scope.activeIvoiceItem.ItemInput = "";
                    $scope.$apply();
                    return false;
                }
                // first check repetation
                //if (!$scope.allowItemRepeat) {
                //    var items = $scope.cost.InvoiceItems;
                //    for (var i = 0; i < items.length; i++) {
                //        if (items[i].Item && items[i].Item.ID === item.ID && (items[i].RowNumber !== $scope.actRow.RowNumber)) {
                //            items[i].Quantity++;
                //            $scope.calculateInvoice();
                //            applyScope($scope);
                //            // repeated item then stay, add count and select current input text then notify user
                //            DevExpress.ui.notify($scope.Hesabfa.farsiDigit("آیتم تکراری به تعداد ردیف " + (i + 1) + " اضافه شد"), "success", 3000);
                //            //					notification(Hesabfa.farsiDigit("آیتم تکراری به تعداد ردیف " + (i + 1) + " اضافه شد"));
                //            $scope.comboItem.setSelected(null);
                //            return false;
                //        }
                //    }
                //}

                $scope.activeIvoiceItem.Item = item;
                if (!item.DetailAccount) {
                    $scope.activeIvoiceItem.Item.DetailAccount = {};
                    $scope.activeIvoiceItem.Item.DetailAccount.Id = item.DetailAccountId;
                }
                $scope.activeIvoiceItem.ItemInput = item.Name;
                //if ($scope.activeIvoiceItem.Quantity === 0) $scope.activeIvoiceItem.Quantity = 1;
                //if ($scope.invoice.InvoiceType === 0 || $scope.invoice.InvoiceType === 2) { // فروش
                //    if (item.SalesTitle !== "") $scope.activeIvoiceItem.Description = item.SalesTitle;
                //} else if ($scope.invoice.InvoiceType === 1 || $scope.invoice.InvoiceType === 3 || $scope.invoice.InvoiceType === 4) { // خرید
                //    if (item.PurchasesTitle !== "") $scope.activeIvoiceItem.Description = item.PurchasesTitle;
                //}

                //$scope.activeIvoiceItem.UnitPrice = $scope.invoice.InvoiceType === 0 || $scope.invoice.InvoiceType === 2 ? item.SellPrice : item.BuyPrice;
                //$scope.calculateInvoice();
                $scope.$apply();
                //if (selectedBy === 'enter') {
                //    if ($scope.activeIvoiceItem.RowNumber === $scope.invoice.InvoiceItems.length)
                //        $scope.addRow();
                //    $scope.moveRowEditor($scope.actRow.RowNumber);
                //    //            $(':input:eq(' + ($(':input').index(this)) + ')').focus();
                //    //            $('#tdItem' + activeRow.RowNumber + ' input').focus();
                //    $('#itemInputitemSelectInput').select();
                //    $('#itemInputitemSelectInput').val('');
                //    $('#itemInputitemSelectInput').focus();
                //}
                return false;
            };
            $scope.newItem = function (invoiceItem) {
                $scope.activeIvoiceItem = invoiceItem;
                $scope.alert = false;
                $scope.item = null;
                $scope.editItemModal = true;
                $scope.$apply();
            };
            $scope.deleteItem = function () {
                $scope.actRow.Item = null;
                $scope.actRow.Description = "";
                $scope.actRow.Discount = 0;
                $scope.actRow.Tax = 0;
                $scope.actRow.Sum = 0;
                $scope.actRow.TotalAmount = 0;
                $scope.actRow.UnitPrice = 0;
                $scope.actRow.Quantity = 0;
            };
            $scope.getEditedItem = function (item) {
                if (!item) return;
                $scope.cost.Item = item.DetailAccount.Node;

                $scope.items.push(item.DetailAccount.Node);
                $scope.editItemModal = false;
                $scope.comboItem.setSelected(item.DetailAccount.Node);
                $scope.itemSelect(item.DetailAccount.Node, "new", $scope.activeIvoiceItem);
                applyScope($scope);
            };
            $scope.contactSelect = function (contact) {
                if (contact) {
                    $scope.cost.Contact = contact;
                    //			$scope.cost.Contact.DetailAccount = {};
                    //			$scope.cost.Contact.DetailAccount.Id = item.DetailAccountId;
                    $scope.cost.ContactTitle = contact.Name;
                    $scope.setEditCostPageTitle();
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

            $scope.selectFromTree = function (costItem) {
                $scope.activeIvoiceItem = costItem;
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
                if ($scope.actRow.RowNumber === $scope.cost.CostItems.length)
                    $scope.addRow(1);
                $scope.moveRowEditor($scope.actRow.RowNumber);
                $scope.$apply();
            };

            $scope.calculateCost = function (cal) {

                if (cal === "sum")
                    $scope.actRow.Rest = $scope.actRow.Sum;

                if (!$scope.cost) return;
                $scope.cost.Sum = 0;
                $scope.cost.Rest = 0;
                $scope.totalPay = 0;
        
                var length = $scope.cost.CostItems.length;

                for (var i = 0; i < length; i++) {
                    var costItem = $scope.cost.CostItems[i];
                    costItem.ItemInput = costItem.Item ? costItem.Item.Name : ""; // new
                   
  
                    //costItem.TotalAmount = costItem.Sum - parseFloat(costItem.Discount + "") + parseFloat(costItem.Tax + "");
                    $scope.cost.Sum += parseFloat(costItem.Sum + "")
                    //$scope.cost.Payable += costItem.TotalAmount;

                    $scope.cost.Rest += parseFloat(costItem.Rest + "")

                    $scope.totalPay += parseFloat(costItem.Sum + "") - parseFloat(costItem.Rest + "");
                }
               // $scope.cost.Payable = $rootScope.isDecimalCurrency() ? Math.round($scope.cost.Payable * 100) / 100 : Math.round($scope.cost.Payable);

                applyScope($scope);
            };
       
            $scope.addRow = function (n) {
                if (!n) n = 1;
                for (var i = 0; i < n; i++) {
                    var newCostItem = {};
                    angular.copy($scope.costItem, newCostItem);
                    newCostItem.RowNumber = $scope.cost.CostItems.length + 1;
                    $scope.cost.CostItems.push(newCostItem);
                }
                //$scope.$apply();
            };
            $scope.deleteRow = function (costItem) {
                if ($scope.cost.CostItems.length === 1) return;   // if only one remaind, do not delete
                // prevent removing autocompletes from pageapplyScope($scope);
                if ($scope.actRow.RowNumber === costItem.RowNumber) {
                    if (costItem.RowNumber > 1)
                        $scope.moveRowEditor(costItem.RowNumber - 2);
                    else
                        $scope.moveRowEditor(costItem.RowNumber); applyScope($scope);
                }

                // remove cost item from CostItems list
                var items = $scope.cost.CostItems;
                findAndRemoveByPropertyValue(items, 'RowNumber', costItem.RowNumber);
                for (var i = 0; i < items.length; i++)  // add row numbers again
                    items[i].RowNumber = i + 1;
                $scope.calculateCost();  // recalculate cost
                //$scope.$apply();
            };
            $scope.moveUpRow = function (costItem) {
                var items = $scope.cost.CostItems;
                items.moveUp(costItem);
                for (var i = 0; i < items.length; i++)
                    items[i].RowNumber = i + 1;
                $scope.$apply();
            };
            $scope.moveDownRow = function (costItem) {
                var items = $scope.cost.CostItems;
                items.moveDown(costItem);
                for (var i = 0; i < items.length; i++)
                    items[i].RowNumber = i + 1;
                $scope.$apply();
            };
          
   

            $scope.saveCost = function (command) {


                if ($scope.calling) return;
                $scope.alert = false;
                $scope.alertMessage = "";
                $scope.alertMessages = [];

                if (!$scope.cost) return;
                var n = $scope.cost.Number;
                $scope.calling = true;
                if (command === "saveAndSubmitForApproval" || command ==="saveAndNext") $scope.cost.Status = 2;
                if (command === "save" || command === "saveAndContinueEdit") $scope.cost.Status = 0;

                //	$scope.cost.DisplayDate = $("#costDate").val();
                //	$scope.cost.DisplayDueDate = $("#costDueDate").val();
                var rowIndex = $scope.actRow.RowNumber;


                //callws(DefaultUrl.MainWebService + "SaveCost", { cost: $scope.cost, sendSms: $scope.sendSmsByApprove })
                //    .success(function (result) {

                $.ajax({
                    type: "POST",
                    data: JSON.stringify($scope.cost),
                    url: "/app/api/Cost/SaveCost",
                    contentType: "application/json"

                }).done(function (res) {
                    

                    if (res.resultCode == 1) {
                        $scope.calling = false;
                        if ($scope.accessError(res.data)) { applyScope($scope); return; }
                        alertbox({ content: res.data, type: "warning" });
                        $scope.cost.Status = $scope.currentCostStatus;
                        applyScope($scope);
                        window.scrollTo(0, 0);
                        return;
                    }

                    var result = res.data;
                    var smsSent = result.smsSent ? 1 : 0;

                    //var cost = result.cost;

                    var save = function () {
                        var path = "";

                        $state.go('costs');

                        //if ($scope.cost.CostType === 0)
                        //	path = "/costs/status=draft&sms={0}&costId={1}&costNumber={2}&costReference={3}&costType={4}&contactName={5}&price={6}"
                        //		.formatString(smsSent, result.id, $scope.cost.Number, $scope.cost.Reference, $scope.cost.CostType, $scope.cost.Contact.Name, $scope.cost.Payable);
                        //else if ($scope.cost.CostType === 1)
                        //	path = "/costs/show=bills";
                        //$location.path(path);
                        $scope.$apply();
                        return;
                    };
                    var saveAndSubmitForApproval = function () {

                        $state.go('viewCost', { id: result.ID, status: "awaitingApproval" });
                        //$location.path("/ViewCost/status=awaitingApproval&sms=" + smsSent + "&id=" + result.id);
                        $scope.$apply();
                        return;
                    };
                    var approve = function () {
                        //$location.path("/ViewCost");

                        $state.go('viewCost', { id: result.ID, status: "approved" });
                        //$location.path("/ViewCost/status=approved&sms=" + smsSent + "&id=" + result.id);
                        $scope.$apply();
                        return;
                    };
                    var approveAndPrint = function () {
                        var status = "approved";
                        $state.go('viewCost', { status: status, print: 1, sms: smsSent, id: result.ID });
                        //$location.path("/ViewCost/status=approved&print=true&sms=" + smsSent + "&id=" + result.id);
                        $scope.$apply();
                        return;
                    };

                    $scope.calling = false;
                    if (command === "saveAndContinueEdit") {
                        $scope.cost.ID = result.ID;
                        $scope.alert = true;
                        $scope.alertType = "success";
                        $scope.alertMessage = "صورت هزینه {2} ذخیره شد - {0} - مبلغ کل: {1}".
                            formatString($scope.cost.Contact.Name, $scope.money($scope.cost.Payable), $scope.cost.Number, $scope.cost.CostType === 0 ? "فروش" : "خرید");
                        $scope.actRow = null;
                        $scope.outRowEditor();
                        //            		$scope.cost = $scope.cost;
                        $scope.$apply();
                        if (rowIndex > $scope.cost.CostItems.length)
                            $scope.moveRowEditor(0);
                        else
                            $scope.moveRowEditor(rowIndex - 1);
                        return;
                    }
                    else if (command === "saveAndSubmitForApproval") {
                        saveAndSubmitForApproval();
                    }
                    else if (command === "save") {
                        if ($scope.cost.Status === 1) saveAndSubmitForApproval();
                        else save();
                    }
                    else if (command === "approve") {
                        approve();
                    }
                    else if (command === "approveAndPrint") {
                        approveAndPrint();
                    }
                    else if (command === "saveAndNext") {
                        $scope.alert = true;
                        $scope.alertType = "success";
                        $scope.alertMessage = "صورت هزینه {2} ذخیره شد - {0} - مبلغ کل: {1}".
                            formatString($scope.cost.Contact.Name, $scope.money($scope.cost.Payable), $scope.cost.Number, $scope.cost.CostType === 0 ? "فروش" : "خرید");

                        $scope.actRow = null;
                        $scope.outRowEditor();
                        $scope.loadCostData(0);
                        $scope.contactInput = "";
                        $scope.$apply();

                        return;
                    }
                    else if (command === "approveAndNext") {
                        $scope.alert = true;
                        $scope.alertType = "success";
                        $scope.alertMessage = "صورت هزینه {2} تایید شد - {0} - مبلغ کل: {1}".
                            formatString($scope.cost.Contact.Name, $scope.money($scope.cost.Payable), $scope.cost.Number, $scope.cost.CostType === 0 ? "فروش" : "خرید");
                        if (result.smsSent) $scope.alertMessage += " [پیامک ارسال شد]";

                        $scope.actRow = null;
                        $scope.outRowEditor();
                        $scope.loadCostData(0);
                        $scope.contactInput = "";
                        $scope.$apply();

                        return;
                    }

                }).fail(function (error) {

                    $scope.calling = false;
                    if ($scope.accessError(error)) { applyScope($scope); return; }
                    alertbox({ content: error, type: "warning" });
                    $scope.cost.Status = $scope.currentCostStatus;
                    applyScope($scope);
                    window.scrollTo(0, 0);
                });


            };
            $scope.setAutoSave = function (switchOnOff) {
                if (switchOnOff) {
                    $scope.Hesabfa.autoSaveTimerCost = window.setInterval(function () {
                        $scope.saveCost("saveAndContinueEdit");
                    }, 60000);
                    $rootScope.setUISetting("editCostAutoSave", "true");
                }
                else {
                    window.clearInterval($scope.Hesabfa.autoSaveTimerCost);
                    $rootScope.setUISetting("editCostAutoSave", "false");
                }
            };
            $scope.setSendSms = function () {
                $scope.sendSmsByApprove = !$scope.sendSmsByApprove ? true : false;
                if ($scope.sendSmsByApprove)
                    $rootScope.setUISetting("sendCostSms", "true");
                else
                    $rootScope.setUISetting("sendCostSms", "false");
            };
            $scope.setAllowItemRepeat = function () {
                $scope.allowItemRepeat = !$scope.allowItemRepeat ? true : false;
                if ($scope.allowItemRepeat)
                    $rootScope.setUISetting("allowItemRepeatCost", "true");
                else
                    $rootScope.setUISetting("allowItemRepeatCost", "false");
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
                callws(DefaultUrl.MainWebService + "SaveSetting", { name: "SMS_DefaultCostSMS", value: sms })
                    .success(function () {
                        return;
                    }).fail(function (error) {
                        if ($scope.accessError(error)) return;
                        alertbox({ content: error });
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
  
            $scope.$on("$destroy", function () {
                window.clearInterval($scope.Hesabfa.autoSaveTimerCost);
            });

            $scope.printProformaCost = function () {
                var settings = jQuery.extend(true, {}, $scope.costSettings);
                settings.footerNote = $scope.cost.Note + "\n" + settings.footerNote + "\n";
                settings.footerNoteDraft = $scope.cost.Note + "\n" + settings.footerNoteDraft + "\n";

                var business = {
                    Name: "ژیور",
                    LegalName: "ژیور",
                    Address: "",
                    PostalCode: "",
                    Fax: ""
                };

                printCost($scope.cost, settings, $scope.totalDiscount, $scope.totalTax, null, business, $scope.getCurrency());
            };
            $scope.pdfProformaCost = function (asString, doAfterRead) {
                if ($scope.cost.Status === 0 && $scope.cost.CostType === 0)
                    $scope.costSettings.footerNote = $scope.footerNote + "\n" + $scope.cost.Note;

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

                generateCostPDF($scope.cost, $scope.costSettings, $scope.totalDiscount, $scope.totalTax, asString, doAfterRead, null, null, null, business, $scope.getCurrency());
            };
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
            //

         
            function applyScope($scope) {
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            }

        }])
}); 