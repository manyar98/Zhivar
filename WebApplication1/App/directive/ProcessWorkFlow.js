define(['application', 'dataService', 'kendoUi', 'kendoGridFa', 'messageService', 'ui-bootstrapTemep', 'ui-bootstrap', ], function (app) {
    app.register.directive('processWorkFlow', ['dataService', 'messageService', '$rootScope', '$state', '$modal', function (dataService, messageService, $rootScope, $state, $modal) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
              //  option: "=",
            },
            transclude: true,
            controller: function ($scope, $element, $attrs) {

            },
            link: function ($scope, $elem, $attrs) {

                //مشاهده گردش کار
                $scope.callMethod = function (data) {
                    $scope.openModal(data)
                };
                $scope.$on('callMethod', function (event, data) {

                    $scope.callMethod(data)
                });
                $scope.openModal = function (data) {
                    var modalInstance = $modal.open({
                        templateUrl: '/App/template/processWorkFlow.html',
                        controller: $scope.FlowController,
                        windowClass: 'app-modal-window-history',
                        resolve: {
                            message: function () {

                                return data;
                            },
                        }
                    });
                };
                $scope.FlowController = function ($scope, $modalInstance, message) {
                    //$scope.model = { Title: message.Title }
                    $scope.mainGridOptions = {
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: "/app/api/Cartable/GetMessageInfoHistories",
                                    data: {
                                        RelatedRecordID: message.RelatedRecordID,
                                        WorkflowInfoCode: message.WorkflowInfoCode
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
                        //{
                        //    extra: false
                        //},
                        sortable: true,
                        columns: [
                             {

                                 //  field: "UserFullName",
                                 width: 250,
                                 title: "اقدام کننده",
                                 template: "<span>#=UserFullName#<span> <span style='color: rgb(51, 122, 183); font-size: 12px ! important;'>[#=RoleName#]<span> "
                             },
                               {
                                   width: 100,
                                   field: "Action",
                                   title: "نوع اقدام",

                               },

                          {
                              field: "UserComment",
                              title: "توضیحات",
                              width: 100,

                          },
                              { width: 100, field: "InstantiationTime", title: " تاریخ  ارسال", template: "#= moment(InstantiationTime).format('jYYYY/jMM/jDD - HH:mm')#", filterable: false },
                               { width: 100, field: "AccomplishTime", title: " تاریخ اقدام", template: "#= moment(AccomplishTime).format('jYYYY/jMM/jDD - HH:mm')#", filterable: false },


                        ]
                    };
                    $scope.closePopup = function () {
                        $modalInstance.close();
                    };
                };


                //انصراف از گردش کار
                $scope.callMethodEnseraf = function (data) {
                    $scope.openModalEnserf(data)
                };
                $scope.$on('callMethodEnseraf', function (event, data) {

                    $scope.callMethodEnseraf(data)
                });

                $scope.openModalEnserf = function (data) {
                    var modalInstance = $modal.open({
                        templateUrl: '/App/template/enserafWorkflow.html',
                        controller: $scope.FlowControllerEnserf,
                        windowClass: 'app-modal-window-history',
                        resolve: {
                            message: function () {

                                return data;
                            },
                        }
                    });
                };
                $scope.FlowControllerEnserf = function ($scope, $modalInstance, message) {

                    $scope.continueWorkflow = function (model) {

                        if (!angular.isDefined(model))
                        {
                            messageService.error('درج توضیحات الزامیست', '');
                            return;
                        }

                        if (model.UserComment == "" || model.UserComment==null) {
                            messageService.error('درج توضیحات الزامیست', '');
                            return;
                        }


                        model.ID = message.RelatedRecordID;
                        model.Code = message.WorkflowInfoCode;

                        dataService.addEntity('/app/api/Darkhast/EnserafVaKhatemehWorkFlow', model).then(function (data) {
                            messageService.success('عملیات با موفقیت انجام شد.');
                            $modalInstance.close();
                        });
                    }

                    $scope.closePopup = function () {
                        $modalInstance.close();
                    };
                };
            },
        };
    }]);
});