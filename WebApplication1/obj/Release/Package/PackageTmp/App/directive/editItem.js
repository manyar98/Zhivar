define(['application', 'dataService'], function (app) {
    app.register.directive('editItem', ['dataService','$compile', function (Service,$compile) {
    return {
        restrict: 'E',
        transclude: true,
        templateUrl: "/App/template/edit-item.html",
        scope: {
            item: '=',
            onsuccess: '=',
            open: '=',
            unitItems: '=?',
            showItemUnit: '=?',
            next: '=?',
            defaultnode: '=?'
        },
        link: function (scope, element, attrs) {
            scope.initEditItem = function (defaultNode) {
                setTimeout(function () {
                    //$('.selectpicker').selectpicker();
                }, 500);
                scope.alertBoxVisible = false;
                scope.alertMessage = "";
                if (!scope.unitItems) scope.unitItems = {};
                if (!scope.showItemUnit) scope.showItemUnit = false;
                //scope.getFinanAccounts();
                setTimeout(function () {
                    //$('.selectpicker').selectpicker('val', '-1');
                }, 500);
                if (!scope.item || scope.item.ID === 0) {
                    scope.getNewInventoryItem(defaultNode);
                } else {
                    scope.getInventoryItem(scope.item.ID);
                }
                scope.startNodeSelect = true;
                $('#editItemModal').modal({ keyboard: false }, 'show');
                $("#editItemModal .modal-dialog").draggable({ handle: ".modal-header" });
                $("#barcode").keypress(function (event) {
                    if (event.which === 13) {
                        event.preventDefault();
                        return false;
                    }
                });
            };

            scope.$watch('open', function (value) {
                if (value)
                    scope.initEditItem();
                scope.open = false;
            }, true);
            scope.submitItem = function (next) {
                scope.item.FinanAccount = scope.trackAccountId === '-1' ? null : findById(scope.accounts, scope.trackAccountId);
                scope.item.PurchasesAccount = scope.purchaseAccountId === '-1' ? null : findById(scope.accounts, scope.purchaseAccountId);
                scope.item.SalesAccount = scope.saleAccountId === '-1' ? null : findById(scope.accounts, scope.saleAccountId);
                scope.saveItem(next);
            };
            scope.getNewInventoryItem = function (defaultNode) {
                scope.calling = true;
        

                $.ajax({
                    type: "POST",
                    // data: JSON.stringify(id),
                    url: "/app/api/Item/GetNewObject",
                    contentType: "application/json"
                }).done(function (res) {
                   
                   var result = res.data;

                 
                    scope.calling = false;
                    if (scope.defaultnode)
                        result.item.DetailAccount.Node = scope.defaultnode;
                    scope.item = result.item;
                    scope.itemUnits = result.itemUnits;
                    scope.showItemUnit = result.showItemUnit;
                    if (defaultNode)
                        scope.item.DetailAccount.Node = defaultNode;


                    //scope.$apply();
                }).fail(function (error) {
                        $('#editItemModal').modal('hide');
                    //    if (typeof error == "string" && error.indexOf("[accessDenied]") > -1) {
                    //        var errorString = error.substr("[accessDenied]".length);
                    //        alertbox({
                    //            content: Hesabfa.accessDeniedString + "</br>" + errorString, onBtn1Click: function () {
                    //                return;
                    //            }
                    //        });
                    //        return;
                    //    }
                        alertbox({ content: error });
                        return;

                });
            };
		           scope.getInventoryItem = function (id) {
		               scope.calling = true;
		               $.ajax({
		                   type: "POST",
		                   data: JSON.stringify(id),
		                   url: "/app/api/Item/GetById",
		                   contentType: "application/json"
		               }).done(function (res) {
		                   var result = res.data;
                               scope.calling = false;
                               scope.item = result;
                               scope.itemUnits = result.itemUnits;

                               if (scope.item.Unit != null && scope.item.Unit.ID > 0)
                                   selectItemUnit(scope.item.Unit);

                               //scope.showItemUnit = result.showItemUnit;
                               //if (scope.item.FinanAccount) {
                               //    scope.trackAccountId = scope.item.FinanAccount.Id;
                               //    $('#itemTrackAccount').selectpicker('val', scope.item.FinanAccount.Id);
                               //}
                               //if (scope.item.PurchasesAccount) {
                               //    scope.purchaseAccountId = scope.item.PurchasesAccount.Id;
                               //    $('#itemPurchaseAccount').selectpicker('val', scope.item.PurchasesAccount.Id);
                               //}
                               //if (scope.item.SalesAccount) {
                               //    scope.saleAccountId = scope.item.SalesAccount.Id;
                               //    $('#itemSaleAccount').selectpicker('val', scope.item.SalesAccount.Id);
                               //}
                               //scope.$apply();
                           }).fail(function (error) {
                               window.location = '/error.html';
                           });
		           };
		           scope.getFinanAccounts = function () {
		               scope.calling = true;
		               callws( 'GetFinanAccounts', {})
                           .success(function (result) {
                               scope.calling = false;
                               var data = result.finanAccounts;
                               scope.accounts = data;
                               //scope.$apply();
                           }).fail(function (error) {
                               window.location = '/error.html';
                           }).loginFail(function () {
                               //window.location = DefaultUrl.login;
                           });
		           };
		           scope.saveItem = function (next) {
		               if (scope.calling) return;
		               scope.alertBoxVisible = false;
		               scope.alertMessage = "";
		               scope.calling = true;
		               //if (!isValueValid("integer", scope.item.Code, "", false)) {
		               //    scope.alertBoxVisible = true;
		               //    scope.alertMessage = "کد وارد شده صحیح نمی باشد";
		               //    scope.calling = false;
		               //    return;
		               //}
		               if (!scope.item.BuyPrice) scope.item.BuyPrice = 0;
                       if (!scope.item.SellPrice) scope.item.SellPrice = 0;

                       scope.item.Next = next;

                       $.ajax({
                            type: "POST",
                            data: JSON.stringify(scope.item),
                            url: "/app/api/Item/AddItem",
                            contentType: "application/json"
                        }).done(function (res) {
                            
                            var savedItem = res.data;

                            scope.calling = false;
                            scope.item = savedItem;
                            scope.onsuccess(savedItem);
                            if (!next)
                                $('#editItemModal').modal('hide');
                            else {
                                var node = scope.item.DetailAccount.Node;
                                scope.item = null;
                                scope.initEditItem(node);
                            }
                            //scope.$apply();
                        }).fail(function (error) {

                                scope.calling = false;
                               scope.alertBoxVisible = true;
                               scope.alertMessage = error;

                });

		
		           };
		           scope.trackItemChange = function () {
		               if (scope.item.TrackItem) {
		                   scope.item.PurchaseItem = scope.item.SellItem = true;
		                   //scope.$apply();
		               }
		           };
		           scope.itemNameLeave = function () {
		               scope.alertBoxVisible = false;
		               //scope.checkNameDuplication();
		               var prefix = "";
		               if (scope.item.DetailAccount.Node.Parent != null)
		                   prefix = scope.item.DetailAccount.Node.Name + " - ";
		               scope.item.PurchasesTitle = !scope.item.PurchasesTitle ? prefix + scope.item.Name : scope.item.PurchasesTitle;
		               scope.item.SalesTitle = !scope.item.SalesTitle ? prefix + scope.item.Name : scope.item.SalesTitle;
		           };
		           scope.checkNameDuplication = function() {
		               callws('CheckItemDuplication', { item: scope.item })
                           .success(function (oldItem) {
                               if (oldItem) {
                                   scope.alertBoxVisible = true;
                                   scope.alertMessage = "هشدار: قبلاً یک آیتم با نام مشابه در سیستم ثبت شده است.";
                                   scope.alertMessage += "(در گروه: " + oldItem.DetailAccount.Node.FamilyTree + ")";
                                  // scope.$apply();
                               }
                           }).fail(function(error) {
                               scope.calling = false;
                               scope.alertBoxVisible = true;
                               scope.alertMessage = error;
                               //scope.$apply();
                           }).loginFail(function() {
                              // window.location = DefaultUrl.login;
                           });
                   };
                   scope.selectItemUnit = function (itemUnit) {
                       for (var i = 0; i < scope.itemUnits.length; i++) {
                           if (scope.itemUnits[i] === itemUnit) {
                               scope.item.Unit = itemUnit;
                               break;
                           }
                       }
                   };
		           scope.deleteItemUnit = function (itemUnit) {
		               //delete from list
		               for (var i = scope.itemUnits.length - 1; i >= 0; i--) {
		                   if (scope.itemUnits[i] === itemUnit) {
		                       scope.itemUnits.splice(i, 1);
		                       break;
		                   }
		               }
		               //scope.$apply();
		               //delete from database
		               callws('DeleteItemUnit', { itemUnit: itemUnit })
                           .success(function () {
                               scope.calling = false;
                           })
                           .fail(function (error) {
                               scope.calling = false;
                               scope.alertBoxVisible = true;
                               scope.alertMessage = error;
                               //scope.$apply();
                           })
                           .loginFail(function () {
                               //window.location = DefaultUrl.login;
                           });
		           };
		    }
            }   
    }]);
});