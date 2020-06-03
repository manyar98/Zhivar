define(['application', 'dataService', 'messageService', 'leafletMap', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup'], function (app) {
    app.register.directive('editSaze', ['dataService', 'messageService', '$compile', '$rootScope', function (Service, messageService, $compile, $rootScope) {
    return {
        restrict: 'E',
        transclude: true,
        templateUrl: "/App/template/edit-saze.html",
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
                 
                    if (scope.item != null && angular.isDefined(scope.item) && scope.item.Images !== null && angular.isDefined(scope.item.Images)) {
                        scope.item.Images = [];
                    }

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

                scope.item.Images = [];
                for (var i = 0; i < totalFiles.length; ++i) {


                    scope.item.Images.push({
                        TasvirBlobBase64 : totalFiles[i].src.replace("data:image/jpeg;base64,", ""),
                        Blob: null,
                        FileName : totalFiles[i].file.name,
                        FileSize : totalFiles[i].file.size,
                        MimeType : totalFiles[i].file.type,

                    });


                }


                scope.saveItem(next);
            };

            scope.deleteImage = function (id,array,index) {

                questionbox({
                    content: "آیا شما از حذف این تصویر مطمئن هستید؟ ",
                    onBtn1Click: function () {
                        //if (scope.calling) return;
                        //scope.calling = true;

                        $.ajax({
                            type: "POST",
                            data: JSON.stringify(id),
                            url: "/app/api/SazeImage/DeleteImage",
                            contentType: "application/json"
                        }).done(function (res) {

   
                           // scope.calling = false;
                            if (res.resultCode === 1) {
                                alertbox({ content: res.data, type: "warning" });
                                return;
                            }
                
                            array.splice(index, 1);

                            DevExpress.ui.notify("تصویر مورد نظر با موفقیت حذف شد.", "success", 3000);
 
                           // scope.$apply();
                        }).fail(function (error) {
                            scope.calling = false;
                            if (rootScope.accessError(error)) { applyScope(scope); return; }
                            scope.$apply();
                            alertbox({ content: error });
                        });
                    }
                });
            };
       
            scope.getNewInventoryItem = function (defaultNode) {
                scope.calling = true;
        

                $.ajax({
                    type: "POST",
                    // data: JSON.stringify(id),
                    url: "/app/api/Saze/GetNewObject",
                    contentType: "application/json"
                }).done(function (res) {
                   
                   var result = res.data;

                 
                    scope.calling = false;
                    if (scope.defaultnode)
                        result.item.Node = scope.defaultnode;
                    scope.item = result.item;
                    scope.item.Arz = null;
                    scope.item.Tol = null;
                    //scope.itemUnits = result.itemUnits;
                    //scope.showItemUnit = result.showItemUnit;
                    if (defaultNode)
                        scope.item.Node = defaultNode;


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
		                   url: "/app/api/Saze/GetSazeItem",
		                   contentType: "application/json"
		               }).done(function (res) {
		                   var result = res.data;
                               scope.calling = false;
                               scope.item = result.item;
                               //scope.itemUnits = result.itemUnits;
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

                       var isValid = true;

                       if (scope.item.Images === null || scope.item.Images.length === 0) {
                           messageService.error('وارد نمودن تصویر  الزامیست', '');
                           isValid = false;
                       }

                       if (scope.item.Status === '' || angular.isUndefined(scope.item.Status)) {
                           messageService.error('وضعیت رسانه را مشخص کنید', '');
                           isValid = false;
                       }

                       if (scope.item.Title === '' || angular.isUndefined(scope.item.Title))
                       {
                           messageService.error('وارد نمودن عنوان  الزامیست', '');
                           isValid = false;
                       }

                       if (scope.item.Address === '' || angular.isUndefined(scope.item.Address)) {
                           messageService.error('وارد نمودن آدرس  الزامیست', '');
                           isValid = false;
                       }

                       if (scope.item.Tol === '' || angular.isUndefined(scope.item.Tol) || scope.item.Tol === 0) {
                           messageService.error('وارد نمودن طول  الزامیست', '');
                           isValid = false;
                       }

                       if (scope.item.Arz === '' || angular.isUndefined(scope.item.Arz) || scope.item.Arz === 0 ) {
                           messageService.error('وارد نمودن عرض  الزامیست', '');
                           isValid = false;
                       }

                       if (scope.item.Longitude === null || angular.isUndefined(scope.item.Longitude) || scope.item.Longitude === 0) {
                           messageService.error('مشخص کردن موقعیت جغرافیایی رسانه الزامیست', '');
                           isValid = false;
                       }

                       if (scope.item.Latitude === null || angular.isUndefined(scope.item.Latitude) || scope.item.Latitude === 0) {
                           messageService.error('مشخص کردن موقعیت جغرافیایی رسانه الزامیست', '');
                           isValid = false;
                       }

                       if (scope.item.NoorDard === '' || angular.isUndefined(scope.item.NoorDard)) {
                           messageService.error('مشخص کنید رسانه نور دارد یا خیر', '');
                           isValid = false;
                       }

                       if (scope.item.NodeGoroheSaze === null || angular.isUndefined(scope.item.NodeGoroheSaze) || scope.item.NodeGoroheSaze.Id === 2) {
                           messageService.error('انتخاب نمودن گروه رسانه الزامیست', '');
                           isValid = false;
                       }

                       if (scope.item.NodeNoeSaze === null || angular.isUndefined(scope.item.NodeNoeSaze || scope.item.NodeNoeSaze.Id === 4)) {
                           messageService.error('انتخاب نمودن نوع رسانه  الزامیست', '');
                           isValid = false;
                       }

                       if (scope.item.NodeNoeEjare === null || angular.isUndefined(scope.item.NodeNoeEjare || scope.item.NodeNoeEjare.Id === 5)) {
                           messageService.error('انتخاب نمودن نوع اجاره  الزامیست', '');
                           isValid = false;
                       }


                       if (!isValid)
                       {
                           scope.calling = false;
                           return;
                       }

                      


      
       
                    
                   
                       $.ajax({
                            type: "POST",
                            data: JSON.stringify(scope.item),
                            url: "/app/api/Saze/Add",
                            contentType: "application/json"
                        }).done(function (res) {

                            if (res.resultCode === 1) {
                                alertbox({ content: res.data, type: "warning" });
                                scope.calling = false;
                                return;
                            }

                            var savedItem = res.data;

                            scope.calling = false;
                            scope.item = savedItem;
                            scope.onsuccess(savedItem);
                            if (!next)
                                $('#editItemModal').modal('hide');
                            else {
                                //var node = scope.item.DetailAccount.Node;
                                scope.item = null;
                                scope.initEditItem();
                                //scope.initEditItem(node);
                            }
                            //scope.$apply();
                        }).fail(function (error) {

                                scope.calling = false;
                               scope.alertBoxVisible = true;
                               scope.alertMessage = error;

                });

		
		           };
		           //scope.trackItemChange = function () {
		           //    if (scope.item.TrackItem) {
		           //        scope.item.PurchaseItem = scope.item.SellItem = true;
		           //        //scope.$apply();
		           //    }
		           //};
		           //scope.itemNameLeave = function () {
		           //    scope.alertBoxVisible = false;
		           //    scope.checkNameDuplication();
		           //    var prefix = "";
		           //    if (scope.item.DetailAccount.Node.Parent != null)
		           //        prefix = scope.item.DetailAccount.Node.Name + " - ";
		           //    scope.item.PurchasesTitle = !scope.item.PurchasesTitle ? prefix + scope.item.Name : scope.item.PurchasesTitle;
		           //    scope.item.SalesTitle = !scope.item.SalesTitle ? prefix + scope.item.Name : scope.item.SalesTitle;
		           //};
		           //scope.checkNameDuplication = function() {
		           //    callws('CheckItemDuplication', { item: scope.item })
             //              .success(function (oldItem) {
             //                  if (oldItem) {
             //                      scope.alertBoxVisible = true;
             //                      scope.alertMessage = "هشدار: قبلاً یک آیتم با نام مشابه در سیستم ثبت شده است.";
             //                      scope.alertMessage += "(در گروه: " + oldItem.DetailAccount.Node.FamilyTree + ")";
             //                     // scope.$apply();
             //                  }
             //              }).fail(function(error) {
             //                  scope.calling = false;
             //                  scope.alertBoxVisible = true;
             //                  scope.alertMessage = error;
             //                  //scope.$apply();
             //              }).loginFail(function() {
             //                 // window.location = DefaultUrl.login;
             //              });
		           //};
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

                   scope.calculateMasaht = function () {
                       scope.Masaht = scope.item.Tol * scope.item.Arz;
                   }
                   scope.mapOptions = {
                       tabularPage: true,
                       center: {
                           lat: 32.757552763570196,
                           lng: 53.41000747680664,
                           zoom: 5
                       },
                       zoomControl: true,
                       zoomControlPosition: "topright",
                       positions: ["ShahrRoustaOnvan", "BakhshOnvan", "ShahrestanOnvan", "OstanOnvan"],
                       marker: {
                           icon: {
                               iconUrl: "Content/images/map/green.png",
                               iconSize: [22, 33],
                               iconAnchor: [12, 32],
                               popupAnchor: [-2, -30]
                           },
                           draggable: true,
                           count: 1
                       },
                       load: function (e) {
                       },
                       ready: function (e, options) {
                           mapReady = true;
                           loadMarkerAddress(options);
                       },
                       click: function (e, marker, markers) {
                           //if (scope.model)
                           //marker.bindPopup('<strong>' + scope.model.Address + '</strong>').openPopup();
                           if (scope.item) {
                               scope.item.Latitude = e.latlng.lat;
                               scope.item.Longitude = e.latlng.lng;
                           }
                       },
                       popupOpen: function (e) {
                       },
                       popupClose: function (e) {
                       },
                       locationError: function (e, msg, code) {
                           e
                       }
                   }

                   function loadMarkerAddress(options) {
                       if (!scope.item)
                           return;
                       if (scope.item.Longitude && scope.item.Latitude)
                           loadMarker(options);
                       //else if (scope.model.ShahrId)
                       //    loadAddress(options);
                   }

                   function loadMarker(options) {
                       var marker = options.setDefaultMarker({ latlng: { lat: scope.item.Latitude, lng: scope.item.Longitude }, zoom: 18 });
                       marker.bindPopup('<strong>' + scope.item.Address + '</strong>').openPopup();
                   }

                   function loadAddress(options) {
                       dataService.getPagedData('/app/api/TaghsimatKeshvari/GetTaghsimatKeshvariIds', { shahrRoustaId: scope.model.ShahrId }).then(function (data) {
                           options.geocodeAddress(data, null, 8).then(function (data) {
                               data
                           }, function (error) {
                               error
                           });
                       });
                   };
		    }
            }   
    }]);
});