//بارگذاری صفحات زیر مجموعه کارتابل
define(['application', 'dataService', 'kendoUi', 'kendoGridFa', 'messageService', 'ui-bootstrapTemep', 'ui-bootstrap'], function (app) {
    app.register.directive('cartableVerificationForm', ['dataService', 'messageService', '$rootScope', '$state', '$uibModal', function (dataService, messageService, $rootScope, $state, $uibModal) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                option: "="
            },
            transclude: true,
            templateUrl: "/App/template/cartableVerificationForm.html",
            controller: function ($scope, $element, $attrs) {

            },
            link: function ($scope, $elem, $attrs) {
                if ($scope.option == null || angular.equals({}, $scope.option)) {
                    $state.go('cartable');
                    return;
                }

                $scope.model = {};
                $scope.typeActions = [];

                //IsView for indicate between view or action
                if ($scope.option.IsView == true) {
                    $scope.model.IsView = true;
                }
                else {
                    $scope.model.IsView = false;

                    dataService.postData("app/api/Cartable/GetWorkflowStepAction", { StepId: $scope.option.WorkflowStepId, StateId: $scope.option.ID }).then(function (data) {
                        $scope.typeActions = data;
                    });
                }

                //call this method in root of link function.
                $scope.loadPage = function () {
                    var url = "verificationForm." + $scope.option.MasterUriRoute;
                    $state.go(url, { id: $scope.option.MasterId, exchangeData: $scope.option.ExchangeData });
                }

                //call from buttons in bottom of page (action button)
                $scope.openModalAction = function (typeAction) {
                    var req = {
                        ActionId: typeAction.ActionId,
                        StepId: typeAction.StepId,
                        WfInstanceStateID: $scope.option.ID,
                        WfInstanceID: $scope.option.WorkflowInstanceId
                    };

                    dataService.postData('/app/api/Cartable/GetNextSteps', req).then(function (nextSteps) {
                        typeAction.NextSteps = nextSteps;

                        var modalInstance = $uibModal.open({
                            animation: true,
                            templateUrl: 'verification-confirm-template.html',
                            controller: $scope.confirmController,
                            windowClass: 'app-modal-window',
                            size: 'lg',
                            resolve: {
                                message: function () {
                                    return $scope.option;
                                },
                                modelTypeAction: function () {
                                    return typeAction;
                                }
                            }
                        });

                        modalInstance.result.then(function (data) {
                            $state.go('cartable');
                        });
                    });
                };

                //controller for modal of action button
                $scope.confirmController = function ($scope, $uibModalInstance, message, modelTypeAction) {
                    $scope.formInit = function () {
                        $scope.modalTitle = "توضیحات:";

                        //isRefuse the term for 'انصراف درخواست دهنده'
                        $scope.isRefuse = modelTypeAction.ActionId < 0 ? true : false;
                        if (modelTypeAction.NeedConfirm)
                            $scope.confirmAction = { active: false, model: {}, message: modelTypeAction.ConfirmMessage }

                        $scope.model = {
                            ActionTitle: modelTypeAction.ActionTitle,
                            ActionId: modelTypeAction.ActionId,
                            Message: message,
                            UserComment: "",
                            NextSteps: modelTypeAction.NextSteps,
                            WorkflowStatus: message.WorkflowStatus
                        };

                        if ($scope.isRefuse == true) {
                            $scope.modalTitle = "این فرایند توسط در خواست دهنده انصراف داده شده است!";
                            $scope.model.UserCommentRefuse = message.UserComment
                        }
                    }

                    $scope.labelClick = function (e) {
                        var radioInput = $(e.target).parent('.radio').find('input:radio');
                        if (radioInput.length == 1) {
                            var checkStatus = $(radioInput[0]).prop("checked");
                            if (angular.isDefined(checkStatus) && (checkStatus == true || checkStatus == 'checked'))
                                return;
                            else
                                $(radioInput[0]).prop("checked", true);
                        }
                    }

                    $scope.action = function (model) {
                        if (angular.isUndefined(model.UserComment) || model.UserComment == "" || model.UserComment == null) {
                            messageService.error('توضیحات اجباریست!', '');
                            return;
                        }

                        if (modelTypeAction.NeedConfirm) {
                            $scope.confirmAction.active = true;
                            $scope.confirmAction.model = model;
                        }
                        else {
                            $scope.continueWorkflow(model);
                        }
                    };

                    $scope.continueWorkflow = function (model) {
                        var selectedUser = $('#cartable-nextSteps input[type=radio]:checked');

                        if ($scope.isRefuse == false && selectedUser.length == 0 && model.NextSteps[0].UserInfoes.length > 0) {
                            messageService.error('لطفا کاربر را انتخاب نمایید!', 'خطا:');
                            return;
                        }

                        var userIds = -1;
                        $.each(selectedUser, function (index, user) {
                            //alert("UserId: " + user.value + "- RoleId: " + user.id);
                            userIds = user.value;
                        });

                        var workflowContinueInfo = [];
                        var workflow = {
                            ActionId: model.ActionId,
                            ExchangeData: model.Message.ExchangeData,
                            CurrentStateID: model.Message.ID,
                            UserComment: model.UserComment,
                            TargetUserId: $scope.isRefuse == false && model.NextSteps[0].UserInfoes.length > 1 ? userIds : null
                        }
                        workflowContinueInfo.push(workflow);

                        dataService.postData("/app/api/Cartable/ContinueWorkflow", workflowContinueInfo).then(function (data) {
                            messageService.success('عملیات با موفقیت انجام شد.', '');
                            $uibModalInstance.close(data);
                        }, function (e) {
                            if (e.resultCode === 4)
                                $scope.closePopup();
                        });
                    };

                    $scope.acceptConfirm = function () {
                        $scope.continueWorkflow($scope.confirmAction.model);
                        $scope.confirmAction.active = false;
                    }

                    $scope.closePopup = function () {
                        $uibModalInstance.dismiss('cancel');
                    };
                };

                //'مشاهده گردش' modal
                $scope.openFlowModal = function () {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'panel-flow.html',
                        controller: $scope.FlowController,
                        windowClass: 'app-modal-window-history',
                        size: 'lg',
                        resolve: {
                            message: function () {
                                return $scope.option;
                            }
                        }
                    });
                };

                $scope.FlowController = function ($scope, $uibModalInstance, message) {
                    $scope.model = { Title: message.Title }

                    $scope.mainGridOptions = {
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: "/app/api/Cartable/GetMessageInfoHistories",
                                    data: {
                                        CurrentStateId: message.ID,
                                        WorkflowInstanceId: message.WorkflowInstanceId
                                    }
                                },
                            },
                            schema: {
                                data: function (data) {
                                    return dataService.processResponse(data);
                                },
                                total: function (data) {
                                    return dataService.getCount(data);
                                },
                            },
                            batch: false,
                            serverFiltering: false,
                            serverPaging: false,
                            serverSorting: false,
                        }),
                        height: 450,
                        filterable: false,
                        sortable: true,
                        columns: [
                             {
                                 title: "اقدام کننده",
                                 template: "<span>#=UserFullName#<span> <span style='color: rgb(51, 122, 183); font-size: 12px ! important;'>[#=RoleName#]<span> ",
                                 width: 330
                             }, {
                                 field: "Action",
                                 title: "نوع اقدام",
                                 width: 120
                             }, {
                                 field: "UserComment",
                                 title: "توضیحات"
                             }, {
                                 field: "InstantiationTime",
                                 title: " تاریخ ارسال",
                                 template: "#= moment(InstantiationTime).format('jYYYY/jMM/jDD - HH:mm')#",
                                 filterable: false,
                                 width: 120
                             }, {
                                 field: "AccomplishTime",
                                 title: " تاریخ اقدام",
                                 template: function (e) {
                                     if (e.AccomplishTime)
                                         return moment(e.AccomplishTime).format('jYYYY/jMM/jDD - HH:mm');
                                     else
                                         return " ";
                                 },
                                 filterable: false,
                                 width: 120
                             }
                        ]
                    };

                    $scope.closePopup = function () {
                        $uibModalInstance.close();
                    };
                };

                $scope.redirect = function () {
                    //var url = 'cartable';
                    var url = 'mainCartable';
                    if ($state.current.previousState && $state.current.previousState.previousState && $state.current.previousState.previousState.previousState) {
                       // var previousState = $state.current.previousState.previousState.previousState;
                        var previousState = $state.current.previousState.previousState;
                        if (previousState && previousState.name == url && previousState.data) {
                            $state.current.previousState = previousState;
                            $state.current.previousState.data.fromChildState = true;
                        }
                        else
                            $state.current.previousState = null;
                    }
                    else
                        $state.get(url).data = {};
                    $state.go(url);
                }

                $scope.loadPage();
            } //end of link function
        };
    }]);
});