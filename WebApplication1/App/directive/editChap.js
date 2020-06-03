define(['application', 'dataService','messageService', 'editPersonel','editNoeChap'], function (app) {
    app.register.directive('editChap', ['dataService', 'messageService', '$compile', function (Service, messageService, $compile) {
        return {
            restrict: 'E',
            transclude: true,
            templateUrl: "/App/template/edit-chap.html",
            scope: {
                item: '=',
                onsuccess: '=',
                open: '=',
                chapkhanes: '=?',
                bazareabs: '=?',
                tarahs: '=?',
                nasabs: '=?',
                showItemUnit: '=?',
                next: '=?',
                defaultnode: '=?',
                listnasabs :'=?',
                listtarahs: '=?',
                listbazareabs: '=?',
                listchapkhanes: '=?',
                listnoechaps: '=?',
                semat: '=?',
                metrazh:'=?'
 
            },
            link: function (scope, element, attrs) {
                scope.initEditItem = function (defaultNode) {

                    scope.alertBoxVisible = false;
                    scope.alertMessage = "";

                    scope.startNodeSelect = true;
                    $('#editChapModal').modal({ keyboard: false }, 'show');
                    $("#editChapModal .modal-dialog").draggable({ handle: ".modal-header" });

                    //scope.comboBazareabs = [];
                    //if (scope.comboBazareabs)
                    //{
                    //         angular.forEach(scope.bazareabs, function (value, key) {

                    //        var newItem = Hesabfa.newReceiveAndPayItem();
                    //        setTimeout(function () {

                    //            var index = key;

                    //            if (!scope.comboBazareabs) scope.comboBazareabs = [];
                    //            scope.comboBazareabs[index] = new HesabfaCombobox({
                    //                items: scope.listbazareabs,
                    //                containerEle: document.getElementById("bazaryabSelect" + index),
                    //                toggleBtn: true,
                    //                newBtn: true,
                    //                deleteBtn: true,
                    //                itemClass: "hesabfa-combobox-item",
                    //                activeItemClass: "hesabfa-combobox-activeitem",
                    //                itemTemplate: Hesabfa.comboPersonalTemplate,
                    //                divider: true,
                    //                matchBy: "user.Id",
                    //                displayProperty: "{Name}",
                    //                searchBy: ["Name"],
                    //                onSelect: function (user) {
                    //                    newItem.User = user;
                    //                },
                    //                onNew: function () {
                    //                    scope.newUser(7);
                    //                }
                    //            });
                    //        }, 0);
                    //    });
                    //}
                   

                

                    if (scope.semat)
                    {
                        if (scope.semat === 7 && scope.bazareabs.length === 0)
                        {
                            scope.addBazareab();
                        }
                        else if (scope.semat === 7 && scope.bazareabs.length > 0) {
                            scope.editBazareab();
                        }
                        else if (scope.semat === 8 && scope.tarahs.length === 0) {
                            scope.addTarah();
                        }
                        else if (scope.semat === 8 && scope.tarahs.length> 0) {
                            scope.editTarah();
                        }
                        else if (scope.semat === 9 && scope.chapkhanes.length === 0) {
                            scope.addChapkhane();
                        }
                        else if (scope.semat === 9 && scope.chapkhanes.length > 0) {
                            scope.editChapkhane();
                        }
                        else if (scope.semat === 10 && scope.nasabs.length === 0) {
                            scope.addNasab();
                        }
                        else if (scope.semat === 10 && scope.nasabs.length > 0) {
                            scope.editNasab();
                        }

                    }
             
                };

                scope.$watch('open', function (value) {
                    if (value)
                        scope.initEditItem();
                    scope.open = false;
                }, true);
                scope.submitItem = function (next) {

                    var isValid = true;

                    if (scope.bazareabs)
                    {
                        angular.forEach(scope.bazareabs, function (value, key) {

                            if (!value.NoeMozdBazryab) {
                                messageService.error('نوع دستمزد بازاریاب را مشخص کنید!', '');
                                isValid = false;
                            }

                            if (value.NoeMozdBazryab && value.NoeMozdBazryab == 1) {
                                if (!value.Darsad) {
                                    messageService.error('درصد دستمزد بازاریاب را مشخص کنید!', '');
                                    isValid = false;
                                }
                            }

                            if (!value.User) {
                                messageService.error('شخص بازاریاب را انتخاب کنید!', '');
                                isValid = false;
                            }
                            
                        });   

                      
                    }

                    if (scope.tarahs) {
                        angular.forEach(scope.tarahs, function (value, key) {

                            if (!value.NoeMozdTarah) {
                                messageService.error('نوع دستمزد طراح را مشخص کنید!', '');
                                isValid = false;
                            }

                            if (value.NoeMozdTarah && value.NoeMozdTarah == 1) {
                            if (!value.Hazine) {
                                messageService.error('دستمزد طراح را مشخص کنید!', '');
                                isValid = false;
                                }

                            if (value.Hazine && parseFloat(value.Hazine) > parseFloat(value.HazineMoshtari))
                            {
                                messageService.error('هزینه طراحی بیشتر از هزینه طراحی مشتری است!', '');
                                isValid = false;
                            }
                            }

                            if (!value.HazineMoshtari) {
                                messageService.error('هزینه طراحی مشتری را مشخص کنید!', '');
                                isValid = false;
                            }
                            
                            if (!value.User) {
                                messageService.error('شخص طراح را انتخاب کنید!', '');
                                isValid = false;
                            }

                        });


                    }

                    if (scope.chapkhanes) {
                        angular.forEach(scope.chapkhanes, function (value, key) {

                            if (!value.ChapkhaneType) {
                                messageService.error(' چاپخانه را مشخص کنید!', '');
                                isValid = false;
                            }

                            if (value.ChapkhaneType && value.ChapkhaneType == 2) {
                            if (!value.TotalChapkhane) {
                                messageService.error('هزینه چاپخانه را مشخص کنید!', '');
                                isValid = false;
                                }


                            if (value.TotalChapkhane && parseFloat(value.TotalChapkhane) > parseFloat(value.TotalMoshtari)) {
                                messageService.error('هزینه چاپ بیشتر از هزینه چاپ مشتری است!', '');
                                isValid = false;
                            }
                            }

                            if (!value.TotalMoshtari) {
                                messageService.error('هزینه چاپ مشتری را مشخص کنید!', '');
                                isValid = false;
                            }
                            
                            if (!value.User) {
                                messageService.error('شخص چاپخانه را انتخاب کنید!', '');
                                isValid = false;
                            }

                        });


                    }

                    if (scope.nasabs) {
                        angular.forEach(scope.nasabs, function (value, key) {

                            if (!value.NoeMozdNasab) {
                                messageService.error('نوع دستمزد طراح را مشخص کنید!', '');
                                isValid = false;
                            }

                            if (value.NoeMozdNasab && value.NoeMozdNasab == 1) {
                            if (!value.Hazine) {
                                messageService.error('دستمزد نصاب را مشخص کنید!', '');
                                isValid = false;
                                }

                            if (value.Hazine && parseFloat(value.Hazine) > parseFloat(value.HazineMoshtari)) {
                                messageService.error('هزینه نصب بیشتر از هزینه نصب مشتری است!', '');
                                isValid = false;
                            }
                            }

                            if (!value.HazineMoshtari) {
                                messageService.error('هزینه نصب مشتری را مشخص کنید!', '');
                                isValid = false;
                            }

                            if (!value.User) {
                                messageService.error('شخص نصاب را انتخاب کنید!', '');
                                isValid = false;
                            }

                        });


                    }

                    if (isValid)
                        scope.saveItem(next);
                };
                scope.getNewInventoryItem = function (defaultNode) {
                    scope.calling = true;


                    $.ajax({
                        type: "POST",
                        // data: JSON.stringify(id),
                        url: "/app/api/UserInfo/UsersByRole",
                        contentType: "application/json"
                    }).done(function (res) {

                        var result = res.data;


                        scope.calling = false;
                        scope.listnasabs = result.nasabs;
                        scope.listtarahs = result.tarahs;
                        scope.listbazareabs = result.bazaryabs;
                        scope.listchapkhanes = result.chapkhanes;

                        //if (scope.defaultnode)
                        //    result.item.DetailAccount.Node = scope.defaultnode;
                        //scope.item = result.item;
                        //scope.itemUnits = result.itemUnits;
                        //scope.showItemUnit = result.showItemUnit;
                        //if (defaultNode)
                        //    scope.item.DetailAccount.Node = defaultNode;


                        //scope.$apply();
                    }).fail(function (error) {
                        $('#editChapModal').modal('hide');
                
                        alertbox({ content: error });
                        return;

                    });
                };
            
    
                scope.saveItem = function (next) {
                    if (scope.calling) return;
                    ////scope.alertBoxVisible = false;
                    ////scope.alertMessage = "";
                    ////scope.calling = true;
                   
                    //if (!scope.item.BuyPrice) scope.item.BuyPrice = 0;
                    //if (!scope.item.SellPrice) scope.item.SellPrice = 0;

                    //scope.item.Next = next;

                    //$.ajax({
                    //    type: "POST",
                    //    data: JSON.stringify(scope.item),
                    //    url: "/app/api/Item/AddItem",
                    //    contentType: "application/json"
                    //}).done(function (res) {

                    //    var savedItem = res.data;

                    //    scope.calling = false;
                    //    scope.item = savedItem;
                    scope.onsuccess();
                    $('#editChapModal').modal('hide');
                    //    if (!next)
                    //        $('#editChapModal').modal('hide');
                    //    else {
                    //        var node = scope.item.DetailAccount.Node;
                    //        scope.item = null;
                    //        scope.initEditItem(node);
                    //    }
                    //    //scope.$apply();
                    //}).fail(function (error) {

                    //    scope.calling = false;
                    //    scope.alertBoxVisible = true;
                    //    scope.alertMessage = error;

                    //});


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
                scope.checkNameDuplication = function () {
                    callws('CheckItemDuplication', { item: scope.item })
                        .success(function (oldItem) {
                            if (oldItem) {
                                scope.alertBoxVisible = true;
                                scope.alertMessage = "هشدار: قبلاً یک آیتم با نام مشابه در سیستم ثبت شده است.";
                                scope.alertMessage += "(در گروه: " + oldItem.DetailAccount.Node.FamilyTree + ")";
                                // scope.$apply();
                            }
                        }).fail(function (error) {
                            scope.calling = false;
                            scope.alertBoxVisible = true;
                            scope.alertMessage = error;
                            //scope.$apply();
                        }).loginFail(function () {
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

                scope.addChapkhane = function () {
                    var newItem = Hesabfa.newReceiveAndPayItem();
                   // newItem.Type = 2;
                    //  newItem.Amount = scope.getRemindedMoney();
                    newItem.MetrazhChapkhane = scope.metrazh;
                    newItem.MetrazhMoshtari = scope.metrazh;
                    scope.chapkhanes.push(newItem);
                  //  applyScope(scope);
                    //    scope.calTotal();

                    setTimeout(function () {
                        var index = scope.chapkhanes.length - 1;

                        if (!scope.comboChapkhanes) scope.comboChapkhanes = [];
                        scope.comboChapkhanes[index] = new HesabfaCombobox({
                            items: scope.listchapkhanes,
                                containerEle: document.getElementById("chapkhanehSelect" + index),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: Hesabfa.comboPersonalTemplate,
                                divider: true,
                                matchBy: "user.Id",
                                displayProperty: "{Name}",
                                searchBy: ["Name", "Code"],
                                onSelect: function (user) {
                                    newItem.User = user;
                                    if (user !== null) {
                                        document.getElementById("divMainChap1" + index).style.display = "block";
                                        document.getElementById("divMainChap2" + index).style.display = "block";
                                        document.getElementById("divMainChap3" + index).style.display = "block";
                                    }

                                    else {
                                        document.getElementById("divMainChap1" + index).style.display = "none";
                                        document.getElementById("divMainChap2" + index).style.display = "none";
                                        document.getElementById("divMainChap3" + index).style.display = "none";
                                    }
                                },
                                onNew: function () {
                                    scope.newUser(9);
                                }
                        });



                        if (!scope.comboNoeChaps) scope.comboNoeChaps = [];
                        scope.comboNoeChaps[index] = new HesabfaCombobox({
                            items: scope.listnoechaps,
                            containerEle: document.getElementById("noeChapSelect" + index),
                            toggleBtn: true,
                            newBtn: true,
                            deleteBtn: true,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: "<div> &nbsp; {Title} </div>",
                            matchBy: "noeChap.Id",
                            displayProperty: "{Title}",
                            searchBy: ["Title"],
                            onSelect: function (noeChap) {
                                newItem.NoeChap = noeChap;
                                newItem.NoeChapID = noeChap.ID;
                            },
                            onNew: function () {
                                scope.newNoeChap(index, newItem);
                            }
                        });


                    }, 0);

                   
                    
                    
                };
                scope.editChapkhane = function () {

                    angular.forEach(scope.chapkhanes, function (value, key) {

                        var newItem = scope.chapkhanes[key];
                    

                        setTimeout(function () {

                            var index = key;

                            scope.metrazh = newItem.MetrazhChapkhane;
                            scope.metrazh = newItem.MetrazhMoshtari;

                            if (newItem.User !== null) {
                                document.getElementById("divMainChap1" + index).style.display = "block";
                                document.getElementById("divMainChap2" + index).style.display = "block";
                                document.getElementById("divMainChap3" + index).style.display = "block";
                            }

                            else {
                                document.getElementById("divMainChap1" + index).style.display = "none";
                                document.getElementById("divMainChap2" + index).style.display = "none";
                                document.getElementById("divMainChap3" + index).style.display = "none";
                            }

                            if (newItem.ChapkhaneType === 1) {

                                document.getElementById("divChap1" + index).style.display = "none";
                                document.getElementById("divChap2" + index).style.display = "none";
                                document.getElementById("divChap3" + index).style.display = "none";
                            }

                            else {
                                document.getElementById("divChap1" + index).style.display = "block";
                                document.getElementById("divChap2" + index).style.display = "block";
                                document.getElementById("divChap3" + index).style.display = "block";
                            }


                            if (!scope.comboChapkhanes) scope.comboChapkhanes = [];
                            scope.comboChapkhanes[index] = new HesabfaCombobox({
                                items: scope.listchapkhanes,
                                containerEle: document.getElementById("chapkhanehSelect" + index),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: Hesabfa.comboPersonalTemplate,
                                divider: true,
                                matchBy: "user.Id",
                                displayProperty: "{Name}",
                                searchBy: ["Name"],
                                onSelect: function (user) {
                                    newItem.User = user;
                                    if (user !== null) {
                                        document.getElementById("divMainChap1" + index).style.display = "block";
                                        document.getElementById("divMainChap2" + index).style.display = "block";
                                        document.getElementById("divMainChap3" + index).style.display = "block";
                                    }

                                    else {
                                        document.getElementById("divMainChap1" + index).style.display = "none";
                                        document.getElementById("divMainChap2" + index).style.display = "none";
                                        document.getElementById("divMainChap3" + index).style.display = "none";
                                    }
                                },
                                onNew: function () {
                                    scope.newUser(9);
                                }
                            });

                            if (!scope.comboNoeChaps) scope.comboNoeChaps = [];
                            scope.comboNoeChaps[index] = new HesabfaCombobox({
                                items: scope.listnoechaps,
                                containerEle: document.getElementById("noeChapSelect" + index),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: "<div> &nbsp; {Title} </div>",
                                matchBy: "noeChap.Id",
                                displayProperty: "{Title}",
                                searchBy: ["Title"],
                                onSelect: function (noeChap) {
                                    newItem.NoeChap = noeChap;
                                    newItem.NoeChapID = noeChap.ID;
                                },
                                onNew: function () {
                                    scope.newNoeChap(index, newItem);
                                }
                            });

                            if (newItem.User !== null)
                                scope.comboChapkhanes[index].setSelected(newItem.User);

                            if (newItem.NoeChap !== null)
                                scope.comboNoeChaps[index].setSelected(newItem.NoeChap);

                        }, 0);
                    });

                };
                scope.addTarah = function () {
                    var newItem = Hesabfa.newReceiveAndPayItem();
                    scope.tarahs.push(newItem);

                    setTimeout(function () {
                        var index = scope.tarahs.length - 1;

                        if (!scope.comboTarahs) scope.comboTarahs = [];
                        scope.comboTarahs[index] = new HesabfaCombobox({

                            items: scope.listtarahs,
                            containerEle: document.getElementById("tarahSelect" + index),
                            toggleBtn: true,
                            newBtn: true,
                            deleteBtn: true,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: Hesabfa.comboPersonalTemplate,
                            divider: true,
                            matchBy: "user.Id",
                            displayProperty: "{Name}",
                            searchBy: ["Name", "Code"],
                            onSelect: function (user) {
                                newItem.User = user;
                                if (user !== null) {
                                    document.getElementById("divMainTarah1" + index).style.display = "block";
                                    document.getElementById("divMainTarah2" + index).style.display = "block";
                                }

                                else {
                                    document.getElementById("divMainTarah1" + index).style.display = "none";
                                    document.getElementById("divMainTarah2" + index).style.display = "none";
                                }
                            },
                            onNew: function () {
                                scope.newUser(8);
                            }
                        });
                    }, 0);
                };
                scope.editTarah = function () {

                    angular.forEach(scope.tarahs, function (value, key) {

                        var newItem = scope.tarahs[key];

                        setTimeout(function () {

                            var index = key;

                            if (newItem.User !== null) {
                                document.getElementById("divMainTarah1" + index).style.display = "block";
                                document.getElementById("divMainTarah2" + index).style.display = "block";
                            }

                            else {
                                document.getElementById("divMainTarah1" + index).style.display = "none";
                                document.getElementById("divMainTarah2" + index).style.display = "none";
                            }


                            if (!scope.comboTarahs) scope.comboTarahs = [];
                            scope.comboTarahs[index] = new HesabfaCombobox({
                                items: scope.listtarahs,
                                containerEle: document.getElementById("tarahSelect" + index),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: Hesabfa.comboPersonalTemplate,
                                divider: true,
                                matchBy: "user.Id",
                                displayProperty: "{Name}",
                                searchBy: ["Name"],
                                onSelect: function (user) {
                                    newItem.User = user;
                                    if (user !== null) {
                                        document.getElementById("divMainTarah1" + index).style.display = "block";
                                        document.getElementById("divMainTarah2" + index).style.display = "block";
                                    }

                                    else {
                                        document.getElementById("divMainTarah1" + index).style.display = "none";
                                        document.getElementById("divMainTarah2" + index).style.display = "none";
                                    }
                                },
                                onNew: function () {
                                    scope.newUser(8);
                                }
                            });

                            if (newItem.User !== null)
                                scope.comboTarahs[index].setSelected(newItem.User);

                        }, 0);
                    });

                   
                };
                scope.addNasab = function () {
                    var newItem = Hesabfa.newReceiveAndPayItem();
                    scope.nasabs.push(newItem);

                    setTimeout(function () {
                        var index = scope.nasabs.length - 1;

                        if (!scope.comboNasabs) scope.comboNasabs = [];
                        scope.comboNasabs[index] = new HesabfaCombobox({
                            items: scope.listnasabs,
                            containerEle: document.getElementById("nasabSelect" + index),
                            toggleBtn: true,
                            newBtn: true,
                            deleteBtn: true,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: Hesabfa.comboPersonalTemplate,
                            divider: true,
                            matchBy: "user.Id",
                            displayProperty: "{Name}",
                            searchBy: ["Name"],
                            onSelect: function (user) {
                                newItem.User = user;
                                if (user !== null) {
                                    document.getElementById("divMainNasab1" + index).style.display = "block";
                                    document.getElementById("divMainNasab2" + index).style.display = "block";
                                }

                                else {
                                    document.getElementById("divMainNasab1" + index).style.display = "none";
                                    document.getElementById("divMainNasab2" + index).style.display = "none";
                                }
                            },
                            onNew: function () {
                                scope.newUser(10);
                            }
                        });
                    }, 0);

                };
                scope.editNasab = function () {

                    angular.forEach(scope.nasabs, function (value, key) {

                        var newItem = scope.nasabs[key];
              
                        setTimeout(function () {

                            var index = key;

                            if (newItem.User !== null) {
                                document.getElementById("divMainNasab1" + index).style.display = "block";
                                document.getElementById("divMainNasab2" + index).style.display = "block";
                            }

                            else {
                                document.getElementById("divMainNasab1" + index).style.display = "none";
                                document.getElementById("divMainNasab2" + index).style.display = "none";
                            }


                            if (!scope.comboNasabs) scope.comboNasabs = [];

                            if (angular.isUndefined(scope.nasabs[key].createdcomboNasab) || scope.nasabs[key].createdcomboNasab === null || scope.nasabs[key].createdcomboNasab === false )
                            {
                            
                                scope.comboNasabs[index] = new HesabfaCombobox({
                                    items: scope.listnasabs,
                                    containerEle: document.getElementById("nasabSelect" + index),
                                    toggleBtn: true,
                                    newBtn: true,
                                    deleteBtn: true,
                                    itemClass: "hesabfa-combobox-item",
                                    activeItemClass: "hesabfa-combobox-activeitem",
                                    itemTemplate: Hesabfa.comboPersonalTemplate,
                                    divider: true,
                                    matchBy: "user.Id",
                                    displayProperty: "{Name}",
                                    searchBy: ["Name"],
                                    onSelect: function (user) {
                                        newItem.User = user;
                                        if (user !== null) {
                                            document.getElementById("divMainNasab1" + index).style.display = "block";
                                            document.getElementById("divMainNasab2" + index).style.display = "block";
                                        }

                                        else {
                                            document.getElementById("divMainNasab1" + index).style.display = "none";
                                            document.getElementById("divMainNasab2" + index).style.display = "none";
                                        }
                                    },
                                    onNew: function () {
                                        scope.newUser(10);
                                    }
                                });

                                scope.nasabs[key].createdcomboNasab = true;
                            }

                                if (newItem.User !== null)
                                    scope.comboNasabs[index].setSelected(newItem.User);

                            }, 0);
                        });
                   
                };

                scope.addBazareab = function () {
                    var newItem = Hesabfa.newReceiveAndPayItem();
                    scope.bazareabs.push(newItem);

                    setTimeout(function () {
                        var index = scope.bazareabs.length - 1;

                        if (!scope.comboBazareabs) scope.comboBazareabs = [];
                        scope.comboBazareabs[index] = new HesabfaCombobox({
                            items: scope.listbazareabs,
                            containerEle: document.getElementById("bazaryabSelect" + index),
                            toggleBtn: true,
                            newBtn: true,
                            deleteBtn: true,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: Hesabfa.comboPersonalTemplate,
                            divider: true,
                            matchBy: "user.Id",
                            displayProperty: "{Name}",
                            searchBy: ["Name"],
                            onSelect: function (user) {
                                newItem.User = user;
                                if (user !== null) {
                                    document.getElementById("divMainBazryab" + index).style.display = "block";
                                }

                                else {
                                    document.getElementById("divMainBazryab" + index).style.display = "none";
                                }
                            },
                            onNew: function () {
                                scope.newUser(7);
                            }
                        });
                    }, 0);

                };
                scope.editBazareab = function () {

                    angular.forEach(scope.bazareabs, function (value, key) {

                        var newItem = scope.bazareabs[key];

                        setTimeout(function () {

                            var index = key;

                            if (newItem.User !== null) {
                                document.getElementById("divMainBazryab" + index).style.display = "block";
                            }

                            else {
                                document.getElementById("divMainBazryab" + index).style.display = "none";
                            }


                            if (!scope.comboBazareabs) scope.comboBazareabs = [];
                            scope.comboBazareabs[index] = new HesabfaCombobox({
                                items: scope.listbazareabs,
                                containerEle: document.getElementById("bazaryabSelect" + index),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: Hesabfa.comboPersonalTemplate,
                                divider: true,
                                matchBy: "user.Id",
                                displayProperty: "{Name}",
                                searchBy: ["Name"],
                                onSelect: function (user) {
                                    newItem.User = user;
                                    if (user !== null) {
                                        document.getElementById("divMainBazryab" + index).style.display = "block";
                                    }

                                    else {
                                        document.getElementById("divMainBazryab" + index).style.display = "none";
                                    }
                                },
                                onNew: function () {
                                    scope.newUser(7);
                                }
                            });

                            if (newItem.User !== null)
                                scope.comboBazareabs[index].setSelected(newItem.User);

                        }, 0);
                    });

                 

                };
                scope.newUser = function (type) {
                    //scope.activeRow = activeRow;
                  // scope.activeRowIndex = index;
                  //  scope.alert = false;
                  //  scope.user = null;
                    scope.editBankModal = true;
                    scope.type = type;
                    //applyScope(scope);
                };

                scope.deleteRowChapkhane = function (index) {
                    scope.chapkhanes.splice(index, 1);
            
                    //applyScope(scope);
                };
                scope.deleteRowNasab = function (index) {
                    scope.nasabs.splice(index, 1);

                    //applyScope(scope);
                };
                scope.deleteRowTarah = function (index) {
                    scope.tarahs.splice(index, 1);
                    //applyScope(scope);
                };
                scope.deleteRowBazareab = function (index) {
                    scope.bazareabs.splice(index, 1);

                    //applyScope(scope);
                };

                scope.getEditedPerson = function (type, person) {
                    switch (type) {
                        case 7:
                            {
                                var finded = findAndReplace(scope.listbazareabs, person.ID, person);
                                if (!finded) scope.listbazareabs.push(person);
                                break;
                            }
                        case 8:
                            {
                                finded = findAndReplace(scope.listtarahs, person.ID, person);
                                if (!finded) scope.listtarahs.push(person);
                                break;
                            }
                        case 9:
                            {
                                finded = findAndReplace(scope.listchapkhanes, person.ID, person);
                                if (!finded) scope.listchapkhanes.push(person);
                                break;
                            }
                        case 10:
                            {
                                finded = findAndReplace(scope.listnasabs, person.ID, person);
                                if (!finded) scope.listnasabs.push(person);
                                break;
                            }
                        default:
                    }
                    
               
                  //  scope.$apply();
                };

                scope.setChapkhaneType = function (type,index)
                {
                    scope.chapkhanes[index].ChapkhaneType = type;
                    if (type === 1)
                    {

                        document.getElementById("divChap1" + index).style.display = "none";
                        document.getElementById("divChap2" + index).style.display = "none";
                        document.getElementById("divChap3" + index).style.display = "none";
                    }
                        
                    else
                    {
                        document.getElementById("divChap1" + index).style.display = "block";
                        document.getElementById("divChap2" + index).style.display = "block";
                        document.getElementById("divChap3" + index).style.display = "block";
                    }
                        
                }
                scope.setNoeMozdBazryab = function (type, index) {
                    scope.bazareabs[index].NoeMozdBazryab = type;
                    if (type === 2) {
                        document.getElementById("divBazryab" + index).style.display = "none";
                    }

                    else {
                        document.getElementById("divBazryab" + index).style.display = "block";
                    }

                }
                scope.setNoeMozdTarah = function (type, index) {
                    scope.tarahs[index].NoeMozdTarah = type;
                    if (type === 2) {
                        document.getElementById("divTarah" + index).style.display = "none";
                    }

                    else {
                        document.getElementById("divTarah" + index).style.display = "block";
                    }

                }
                scope.setNoeMozdNasab = function (type, index) {
                    scope.nasabs[index].NoeMozdNasab = type;
                    if (type == 2) {
                        document.getElementById("divNasab" + index).style.display = "none";
                    }

                    else {
                        document.getElementById("divNasab" + index).style.display = "block";
                    }

                }


                scope.newNoeChap = function (index, activeRow) {
                    //scope.activeRow = activeRow;
                    //scope.activeRowIndex = index;
                    //scope.alert = false;
                    scope.noeChap = null;
                    scope.editNoeChapModal = true;
                    //applyScope(scope);
                };
          
                scope.getEditedNoeChap = function (noeChap) {
                    if (!noeChap) return;
                    scope.listnoechaps.push(noeChap);
                    scope.editNoeChapModal = false;
                    //scope.activeRow.Cash = cash;
                   // scope.activeRow.DetailAccount = cash.DetailAccount;
                    //scope.activeRow.AccountInput = cash.Name;
                    //if (scope.invoice.InvoiceType === 0 || scope.invoice.InvoiceType === 3) {
                    //    scope.comboReceiveCash[scope.activeRowIndex].setSelected(cash);
                    //} else {
                    //    scope.comboPayCash[scope.activeRowIndex].setSelected(cash);
                    //}
                    scope.$apply();
                };

            
                scope.calcAmountChap = function (index) {
                    var fiChapkhane = 0;
                    if (angular.isDefined(scope.chapkhanes[index].FiChapkhane))
                        fiChapkhane = scope.chapkhanes[index].FiChapkhane;

                    var metrazhChapkhane = 0;
                    if (angular.isDefined(scope.chapkhanes[index].MetrazhChapkhane))
                        metrazhChapkhane = scope.chapkhanes[index].MetrazhChapkhane;
                    var total = fiChapkhane * metrazhChapkhane;
                    scope.chapkhanes[index].TotalChapkhane = total;
                 

                }

                scope.calcAmountMoshtari = function (index) {
                    var fiMoshtari = 0;
                    if (angular.isDefined(scope.chapkhanes[index].FiMoshtari))
                        fiMoshtari = scope.chapkhanes[index].FiMoshtari;

                    var metrazhMoshtari = 0;
                    if (angular.isDefined(scope.chapkhanes[index].MetrazhMoshtari))
                        metrazhMoshtari = scope.chapkhanes[index].MetrazhMoshtari;
                    var total = fiMoshtari * metrazhMoshtari;
                    scope.chapkhanes[index].TotalMoshtari = total;


                }
                
            }
        }
    }]);
});