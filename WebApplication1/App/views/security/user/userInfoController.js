define(['application', 'dataService', 'momentJalaali', 'messageService', 'kendoUi', 'kendoGridFa', 'treeDropDown', 'autoComplete'], function (app) {
    app.register.controller('userInfoController', ['$scope', '$rootScope', '$stateParams', '$state', 'dataService', 'messageService',
        function ($scope, $rootScope, $stateParams, $state, dataService, messageService) {

            $rootScope.applicationModule = "اطلاعات کاربر";
            $scope.applicationDescription = "برای انجام عملیات روی کاربر سیستم، ابتدا اطلاعات را وارد / ویرایش نموده سپس برروی دکمه ثبت کلیک نمایید.";

            $scope.showOrgan = false;
            $scope.canViewPassFields = true;
            $scope.updateState = false;
            $scope.AuthenticationTypeTitle = '';

            $scope.operationAccess = dataService.getOperationAccess("UserInfo");
            if (!$scope.operationAccess.canInsert)
                return;

            $scope.organization = {
                OperationAccess: $scope.operationAccess,
                ControlName: "userInfoOrganization",
                ID: "ID",
                Title: "Title",
                ParentId: "ParentId",
                Url: "/app/api/Organization",
                MemberID: 0,
                optionLabel: "لطفا سازمان را انتخاب نمایید",
                MessageError: "انتخاب سازمان الزامیست",
                Required: false,
                Disabled: false
            }

            $scope.optionsMarkazDarmani = {
                url: '/app/api/GeneralMarkazDarmani/GetMarkazAndKargozariListByFilter',
                //dataSource: [],
                cssClass: '',
                controlName: 'GeneralMarkazDarmani',
                visible: true,
                enable: true,               
                dataTextField: "Value",
                dataValueField: "Key",
                placeholder: 'لطفا نام مرکز درمانی را جستجو نمایید',
                tabindex: 1,
                listRightPosition: 2,
                minLength: 4,
                autoLoad: true,
                serverFiltering: true,
                select: function (selectedId, selectedItem) {

                }
            }

            $scope.organizationUnitChart = {
                OperationAccess: $scope.operationAccess,
                ControlName: "userInfoOrganizationUnitChart",
                ID: "ID",
                Title: "Title", 
                ParentId: "ParentId",
                Url: "/app/api/OrganizationUnitChart/GetOrganizationUnitChart",
                GetByIdUrl: "app/api/OrganizationUnitChart/GetOrganizationUnitChartById",
                MemberID: 0,
                CascadeId: -1,
                SelectedId: -1,
                optionLabel: "لطفا واحد متعامل را انتخاب نمایید",
                MessageError: "انتخاب واحد متعامل الزامیست",
                Required: false,
                Disabled: false
            }

            $scope.model = {
                ID: '',
                FirstName: "",
                LastName: "",
                MobileNo: "",
                Tel: "",
                Email: "",
                UserName: "",
                Password: "",
                ConfirmPassword: "",
                Gender: 1,
                LoginTryTime: new Date(),
                LastLoginDate: new Date(),
                AuthenticationType: $rootScope.userModel.AuthenticationType,
                IsDeleted: false,
                IsActive: true,
                NationalCode: "",
                OrganizationId: "",
                OUChartId: ""
            };

            $scope.init = function () {
                if ($stateParams.id != '-1') {
                    $scope.updateState = true;
                    $scope.canViewPassFields = false;
                    $scope.model.ID = $stateParams.id;
                    dataService.getById('/app/api/UserInfo/FindUserInfo?userID=', $scope.model.ID).then(function (data) {
                        $scope.model = data;
                        if (data.AuthenticationType == 2) {
                            $scope.organization.MemberID = data.OrganizationId;
                            $scope.organizationUnitChart.CascadeId = data.OrganizationId;
                            $scope.organizationUnitChart.SelectedId = data.OUChartId;
                            $scope.organizationUnitChart.MemberID = data.OUChartId;
                        }
                        $scope.AuthenticationTypeTitle = data.AuthenticationType == 1 ? 'سیستمی' : 'سایر';
                    });
                }

                $scope.organization.Required = $rootScope.userModel.isOrganizationUser;
                $scope.organizationUnitChart.Required = $rootScope.userModel.isOrganizationUser;
            }

            $scope.onChangeOrganization = function () {
                $scope.organizationUnitChart.CascadeId = $scope.organization.MemberID;
            }

            $scope.onChangeOrganizationUnitChart = function () {
            }

            $scope.redirect = function () {
                var url = 'userSearch';
                if ($state.current.previousState && $state.current.previousState.name == url && $state.current.previousState.data)
                    $state.current.previousState.data.fromChildState = true;
                else
                    $state.get(url).data = {};

                $state.go(url);
            }

            $scope.action = function (model) {
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
                if (model.ID != "") {
                    $scope.edit(model);
                }
                else {
                    if ($rootScope.userModel.isOrganizationUser)
                        model.AuthenticationType = 2;

                    $scope.add(model);
                }
            }

            $scope.add = function (model) {
                dataService.addEntity("/app/api/UserInfo/AddUserInfo", model).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.redirect();
                });
            }

            $scope.edit = function (model) {
                dataService.addEntity("/app/api/UserInfo/UpdateUserInfo", model).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.redirect();
                });
            }

        }
    ]);
});
