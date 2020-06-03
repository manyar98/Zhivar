define(['application', 'dataService','messageService'], function (app) {
    app.register.directive('editPersonel', ['dataService', 'messageService', '$compile', function (dataService, messageService, $compile) {

        return {
            restrict: 'E',
            transclude: true,
            templateUrl: '/App/template/edit-personel.html',
            scope: {
                item: '=',
                onsuccess: '=',
                open: '=',
                type: '=',
                positions: '=',
                iscreated: '='
            },
            link: function (scope, element, attrs) {
                scope.initEditItem = function () {

                    var azTarikhDateObj = new AMIB.persianCalendar('azTarikhDate');
                    scope.alertBoxVisible = false;
                    scope.alertMessage = "";
                    if (!scope.item) {
                        scope.getNewItemObject();
                    } else {
                        scope.getItem(scope.item.ID);
                    }


                    $('#editItemModal').modal({ keyboard: false }, 'show');
                    $("#editItemModal .modal-dialog").draggable({ handle: ".modal-header" });

                    if (!scope.iscreated)
                    {
                        scope.comboNoeSemat = new HesabfaCombobox({
                            items: scope.positions,
                            containerEle: document.getElementById("comboNoeSemat"),
                            toggleBtn: true,
                            newBtn: false,
                            deleteBtn: false,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: "<div class='hesabfa-combobox-title-blue' style='overflow:hidden;white-space: nowrap;'> {Key} &nbsp;-&nbsp; {Value}&nbsp;&rlm;</div>",
                            inputClass: "form-control input-sm",
                            matchBy: "Value",
                            displayProperty: "{Value}",
                            searchBy: ["Key", "Value"],
                            onSelect: function (item) {
                                scope.model.RoleID = item.Key;
                            },
                            onNew: {},
                            onDelete: {},
                            divider: true
                        });
                        scope.iscreated = true;
                    }

                    if (scope.type != null)
                    {
                        scope.model.NoeSemat = scope.type;
                    }
                  
                };
                scope.$watch('open', function (value) {
                    if (value)
                        scope.initEditItem();
                    scope.open = false;
                }, true);
                scope.submitItem = function () {
                    scope.saveItem();
                };
                scope.closeModal = function () {
                    $('#editItemModal').modal('hide');
                };
                scope.getNewItemObject = function () {
                    scope.calling = true;
                    $.ajax({
                        type: "POST",
                        // data: JSON.stringify(id),
                        url: "/app/api/Personel/GetNewPersonelObject",
                        contentType: "application/json"
                    }).done(function (res) {
                        scope.calling = false;
                        scope.model = res.data;

                        scope.$apply();
                    });

                };

                scope.getItem = function (id) {
                    scope.calling = true;
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(id),
                        url: "/app/api/Personel/GetPersonelById",
                        contentType: "application/json"
                    }).done(function (res) {
                        var item = res.data;
                        scope.calling = false;
                        scope.model = item;
                        scope.$apply();
                    }).fail(function (error) {
                        window.location = '/error.html';
                    });
                };
                scope.saveItem = function () {
                    scope.alertBoxVisible = false;
                    scope.alertMessage = "";
                    scope.calling = true;
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(scope.model),
                        url: "/app/api/Personel/Add",
                        contentType: "application/json"
                    }).done(function (res) {
                        var savedItem = res.data;
                        scope.calling = false;
                        scope.model = savedItem;
                        scope.onsuccess(savedItem);
                        $('#editItemModal').modal('hide');
                        scope.$apply();
                    }).fail(function (error) {
                        scope.calling = false;
                        scope.alertBoxVisible = true;
                        scope.alertMessage = error;
                        scope.$apply();
                    });
                };

                scope.action = function (model) {
                    if (model.Password !== model.ConfirmPassword) {
                        messageService.error('یکسان بودن رمز عبور و تایید آن الزامیست!', '');
                        return;
                    }

                    if (model.AuthenticationType == 2) {
                        model.OrganizationId = $scope.organization.MemberID == 0 ? null : $scope.organization.MemberID;
                        model.OUChartId = $scope.organizationUnitChart.MemberID == 0 ? null : $scope.organizationUnitChart.MemberID;

                        if (model.OrganizationId == null || model.OrganizationId == 0 || model.OrganizationId == "") {
                            messageService.error('انتخاب سازمان برای کاربر سازمانی الزامیست!', '');
                            return;
                        }
                        if (model.OUChartId == null || model.OUChartId == 0 || model.OUChartId == "") {
                            messageService.error('انتخاب واحد متعاملین سازمان برای کاربر سازمانی الزامیست!', '');
                            return;
                        }
                    }
                    else {
                        model.OrganizationId = null;
                        model.OUChartId = null;
                    }

                    model.IsActive = model.IsActive == 1 ? true : false;

                    scope.add(model);

                    //if (model.ID != "") {
                    //    scope.edit(model);
                    //}
                    //else {
                    //    if ($rootScope.userModel.isOrganizationUser)
                    //        model.AuthenticationType = 2;

                    //    scope.add(model);
                    //}
                }

                scope.add = function (model) {

                    //model.RoleId = scope.type;

                    //dataService.addEntity("/app/api/UserInfo/AddUserInfo2", model).then(function (data) {
                    //    messageService.success('عملیات با موفقیت انجام شد', '');
                        
                    //    scope.onsuccess(scope.type, data);
                    //    $('#editItemModal').modal('hide');

                    //});

                    dataService.addEntity('/app/api/Personel/SavePersonel', model).then(function (data) {
                        messageService.success('عملیات با موفقیت انجام شد.');
                        scope.onsuccess(scope.type, data);
                        $('#editItemModal').modal('hide');
                    });
                }

                scope.setPosition = function (positionId) {
                    dataService.getPagedData('/app/api/Personel/GetPositionByOrganizationId').then(function (data) {
                        scope.positions = data.Positions;
                        scope.model.RoleID = positionId;
                    
                    });
                }
            }
        };
    }]);
});