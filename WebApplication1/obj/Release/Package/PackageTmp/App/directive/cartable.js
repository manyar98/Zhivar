//کارتابل اصلی
define(['application', 'dataService', 'kendoUi', 'kendoGridFa', 'messageService','number'], function (app) {
    app.register.directive('cartable', ['dataService', 'messageService', '$rootScope', '$state','$uibModal',
        function (dataService, messageService, $rootScope, $state, $uibModal) {
            return {
                restrict: 'E',
                replace: true,
                scope: {
                    selected: "&"
                },
                transclude: true,
                templateUrl: "/App/template/cartable.html",
                controller: function ($scope, $element, $attrs) {
                    //$scope.clickRow = function (row) {
                    //    $scope.selected({ e: row });
                    //}
                },
                link: function ($scope, $elem, $attrs) {
                    $scope.typeActions = [];
                    $scope.workflowItems = [];
                    $scope.model = {
                        position: {
                            RoleID: -1,
                            RelatedUserId: -1
                        }
                    };
                    var workflowStepId = -1000, workflowId = null, multiCheckAction = true;

                    //get main menu items by current user id, call this in root of link method.
                    $scope.getMenuItems = function () {
                        var menuReq = {};
                        if ($scope.model.position.RoleID == -1 || $scope.model.position.RoleID == "")
                            menuReq = { UserId: $rootScope.userModel.UserId, RoleId: null };
                        else
                            menuReq = { UserId: $scope.model.position.RelatedUserId, RoleId: $scope.model.position.RoleID };

                        dataService.postData("app/api/Cartable/GetMenuItems", menuReq).then(function (data) {
                            for (var i = 0; i < data.length; i++)
                                data[i].SubMenuItems = [];

                            $scope.workflowItems = data;
                            setTimeout(function () { bindEventForMenu(); }, 1000);
                            if ($state.current.data.fromChildState == true) {
                                $scope.getSubMenuItems(workflowId);
                                $scope.mainGridOptions.dataSource.read();
                            }
                        });
                    }

                    //get sub menu in right of panel by click parent.
                    $scope.getSubMenuItems = function (wfId) {
                        for (var i = 0; i < $scope.workflowItems.length; i++) {
                            if ($scope.workflowItems[i].WorkflowId == wfId) {
                                var subMenuReq = {}; $scope.workflowItems[i].SubMenuItems = [];
                                if ($scope.model.position.RoleID == -1 || $scope.model.position.RoleID == "")
                                    subMenuReq = { UserId: $rootScope.userModel.UserId, RoleId: null, WorkflowId: wfId };
                                else
                                    subMenuReq = { UserId: $scope.model.position.RelatedUserId, RoleId: $scope.model.position.RoleID, WorkflowId: wfId };

                                dataService.postData("app/api/Cartable/GetSubMenuItems", subMenuReq).then(function (data) {
                                    $scope.workflowItems[i].SubMenuItems = data;
                                });

                                $("li.workflowItem li").removeClass("active");
                                var children = $(this).parent('li.workflowItem').find(' > ul > li');
                                var items = $('li.workflowItem').find(' > ul > li');
                                items.hide(100);
                                children.show(100);
                                break;
                            }
                        }
                    }

                    //call this by click sub menu item,
                    //load buttons for panel under grid
                    $scope.loadGrid = function (subMenuItem) {
                        multiCheckAction = subMenuItem.MultiCheckAction;
                        $scope.model.PanelAction = false;
                        dataGrid = [];
                        workflowStepId = subMenuItem.StepId;
                        workflowId = subMenuItem.WorkflowId;

                        //Get Actions For this StateType
                        dataService.postData("app/api/Cartable/GetWorkflowStepAction", { StepId: subMenuItem.StepId, StateId: null }).then(function (data) {
                            $scope.typeActions = data;
                         
                            $scope.mainGridOptions.dataSource.read();
                        });
                    }

                    //call with grid command
                    function showDetail(e) {
                        e.preventDefault();
                        $("#panel-grid-type").removeClass("fa-check-square-o");
                        $scope.model.PanelAction = false;
                        var row = this.dataItem($(e.currentTarget).closest("tr"));
                        row.IsView = row.MessageType == 1 ? false : true;

                        $scope.$apply(function () {
                            $scope.selected({ e: row });
                        });
                    }

                    function openFlowModal(e) {
                        var row = this.dataItem($(e.currentTarget).closest("tr"));
                        row.IsView = row.MessageType == 1 ? false : true;

                        var modalInstance = $uibModal.open({
                            animation: true,
                            templateUrl: 'cartable-panel-flow.html',
                            controller: $scope.FlowController,
                            windowClass: 'app-modal-window-history',
                            size: 'lg',
                            resolve: {
                                message: function () {
                                    return row;
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
                                     width: 330,
                                     title: "اقدام کننده",
                                     template: "<span>#=UserFullName#<span> <span style='color: rgb(51, 122, 183); font-size: 12px ! important;'>[#=RoleName#]<span> "
                                 }, {
                                     width: 120,
                                     field: "Action",
                                     title: "نوع اقدام"
                                 }, {
                                     field: "UserComment",
                                     title: "توضیحات"
                                 }, {
                                     field: "InstantiationTime",
                                     title: " تاریخ  ارسال",
                                     template: "#= moment(InstantiationTime).format('jYYYY/jMM/jDD - HH:mm')#",
                                     width: 120,
                                     filterable: false
                                 }, {
                                     field: "AccomplishTime",
                                     title: " تاریخ  اقدام",
                                     template: function (e) {
                                         if (e.AccomplishTime)
                                             return moment(e.AccomplishTime).format('jYYYY/jMM/jDD - HH:mm');
                                         else
                                             return " ";
                                     },
                                     width: 120,
                                     filterable: false
                                 }
                            ]
                        };

                        $scope.closePopup = function () {
                            $uibModalInstance.close();
                        };
                    };

                    $scope.mainGridOptions = {
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    type: "GET",
                                    url: "/app/api/Cartable/GetMessages",
                                    dataType: "json",
                                },
                                //send parameter with all request
                                parameterMap: function (options, operation) {
                                    if (operation == "read") {
                                        if (!$state.current.data.fromChildState) {
                                            options.WorkflowId = workflowId;
                                            options.WorkflowStepId = workflowStepId;
                                            options.UserId = $scope.model.position.RoleID != -1 && $scope.model.position.RoleID != "" ? $scope.model.position.RelatedUserId : $rootScope.userModel.UserId,
                                            options.RoleId = $scope.model.position.RoleID != -1 && $scope.model.position.RoleID != "" ? $scope.model.position.RoleID : null;
                                            $state.current.data.gridOptions = options;
                                        }
                                        else if (angular.isDefined($state.current.data.gridOptions) && !angular.equals({}, $state.current.data.gridOptions)) {
                                            //$state.current.data.fromChildState = false;
                                            options = $state.current.data.gridOptions;
                                        }
                                    }
                                    return options;
                                }
                            },
                            change: function (e) {
                                if (workflowId == null) {
                                    e.sender._filter = undefined;
                                    e.sender._sort = undefined;
                                }
                                else if (!$state.current.data.fromChildState) {
                                    $state.current.data.gridOptions.filter = e.sender._filter;
                                    $state.current.data.gridOptions.sort = e.sender._sort;
                                }
                                else if (angular.isDefined($state.current.data.gridOptions) && angular.isDefined($state.current.data.gridOptions) && !angular.equals({}, $state.current.data.gridOptions)) {
                                    $state.current.data.fromChildState = false;
                                    e.sender._filter = $state.current.data.gridOptions.filter;
                                    e.sender._sort = $state.current.data.gridOptions.sort;
                                }
                            },
                            requestStart: function (e) {
                                if (e.type == "read") {
                                    if ($state.current.data.fromChildState == true && angular.isDefined($state.current.data.gridOptions) && !angular.equals({}, $state.current.data.gridOptions)) {
                                        e.sender._filter = $state.current.data.gridOptions.filter;
                                        e.sender._sort = $state.current.data.gridOptions.sort;
                                    }
                                }
                            },
                            schema: {
                                data: function (result) {
                                    return dataService.processResponse(result);
                                },
                                total: function (data) {
                                    return dataService.getCount(data);
                                },
                                model: {
                                    id: "ID",
                                    fields: {
                                        ID: { type: "number", editable: false, nullable: false },
                                        WorkflowStepId: { type: "number" },
                                        Title: { type: "string" },
                                        InstantiationTime: { type: "date" },
                                        AccomplishTime: { type: "date" },
                                        FullName: { type: "string" },
                                        WorkflowInstanceId: { type: "string" },
                                        MessageType: { type: "number" },
                                        WorkflowStatus: { type: "number" },
                                        UserComment: { type: "string" },
                                    }
                                }
                            },
                            serverPaging: false,
                            serverSorting: false,
                            serverFiltering: false,
                        }),
                        filterable: {
                            extra: false
                        },
                        height: 380,
                        groupable: false,
                        resizable: true,
                        scrollable: true,
                        selectable: 'single',
                        sortable: {
                            mode: "single",
                            allowUnsort: true
                        },
                        pageable: false,
                        autoBind: false,
                        dataBound: function (e) {
                            $("#grid tbody tr .k-grid-typeDetail").each(function () {
                                var currentDataItem = $("#grid").data("kendoGrid").dataItem($(this).closest("tr"));
                                if (currentDataItem.MessageType == 2) {
                                    $(this).html('<span class="fa fa-file-text"></span>مشاهده');
                                }
                                if (currentDataItem.WorkflowStatus == 3) {
                                    $(this).closest("tr").find("td").css("color", "red");
                                }
                            });

                            if (multiCheckAction == false) {
                                $("#grid tbody tr .icon-checked").each(function () {
                                    $(this).remove();
                                });
                            }

                            dbClickRow();
                        },
                        columns: [
                             {
                                 //multi check span
                                 template: "<span class='icon-checked is-view-{{#=MessageType#}}'><i id='#=ID #' ng-click='onChange($event)'  class='fa fa-square-o'></i> </span>",
                                 width: 40
                             }, {
                                 field: "SenderFullName",
                                 title: "نام فرستنده"
                             }, {
                                 field: "SenderRoleName",
                                 title: "نقش فرستنده"
                             }, {
                                 field: "Title",
                                 title: "عنوان"
                             }, {
                                 field: "SendDateTime",
                                 title: " تاریخ",
                                 template: "#= moment(SendDateTime).format('jYYYY/jMM/jDD - HH:mm')#",
                                 filterable: false,
                                 width: 130
                             }, {
                                 command: [
                                     { text: "اقدام", name: "typeDetail", imageClass: "k-icon k-i-connector", click: showDetail },
                                     { text: "گردش کار", name: "openFlowModal", imageClass: "k-icon k-i-refresh", click: openFlowModal }
                                 ],
                                 title: " ",
                                 width: 160,
                             }
                        ]
                    };

                    //call by buttons under grid
                    $scope.openConfirmModal = function (typeAction) {
                        //checked elements that have id of row
                        var elm = $("#panel-grid-type .fa-check-square-o");

                        if (elm.length > 0) {
                            var req = {
                                ActionId: typeAction.ActionId,
                                StepId: typeAction.StepId
                            };

                            dataService.postData('/app/api/Cartable/GetNextSteps', req).then(function (nextSteps) {
                                typeAction.NextSteps = nextSteps;

                                var dataSource = $scope.mainGridOptions.dataSource.data();
                                //list of selected row
                                var recordSelected = [];

                                for (var i = 0; i < elm.length; i++) {
                                    var item = $.grep(dataSource, function (e) { return e.ID == elm[i].id; })[0];
                                    recordSelected.push(item);
                                }

                                var modalInstance = $uibModal.open({
                                    animation: true,
                                    templateUrl: 'cartable-confirm-template.html',
                                    controller: $scope.confirmController,
                                    windowClass: 'app-modal-window',
                                    size: 'lg',
                                    resolve: {
                                        messages: function () {
                                            return recordSelected;
                                        },
                                        modelTypeAction: function () {
                                            return typeAction;
                                        }
                                    }
                                });

                                //after close or dismiss modal
                                modalInstance.result.then(function (data) {
                                    $scope.getMenuItems();
                                    workflowStepId = -1000; //for load empty grid
                                    workflowId = null;
                                    $scope.mainGridOptions.dataSource.read();
                                    $scope.model.PanelAction = false; //to hide buttons panel under grid
                                });
                            });
                        }
                    };

                    $scope.confirmController = function ($scope, $uibModalInstance, messages, modelTypeAction) {
                        $scope.formInit = function () {
                            $scope.modalTitle = "اقدام:";

                            $scope.model = {
                                ActionTitle: modelTypeAction.ActionTitle,
                                ActionId: modelTypeAction.ActionId,
                                Messages: messages,
                                UserComment: "",
                                NextSteps: modelTypeAction.NextSteps
                            };

                            if (modelTypeAction.NeedConfirm)
                                $scope.confirmAction = { active: false, model: {}, message: modelTypeAction.ConfirmMessage }
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

                        //call by click modal button validation control
                        $scope.action = function (model) {
                            if (angular.isUndefined(model.UserComment) || model.UserComment === null || model.UserComment == "") {
                                messageService.error('توضیحات اجباریست!', 'خطا:');
                                return;
                            }

                            if (modelTypeAction.NeedConfirm) {
                                $scope.confirmAction.active = true;
                                $scope.confirmAction.model = model;
                            }
                            else {
                                $scope.continueWorkflow(model);
                            }
                        }

                        $scope.acceptConfirm = function () {
                            $scope.continueWorkflow($scope.confirmAction.model);
                            $scope.confirmAction.active = false;
                        }

                        $scope.continueWorkflow = function (model) {
                            var selectedUser = $('#cartable-nextSteps input[type=radio]:checked');

                            if (selectedUser.length == 0 && model.NextSteps[0].UserInfoes.length > 0) {
                                messageService.error('لطفا کاربر را انتخاب نمایید!', 'خطا:');
                                return;
                            }

                            var userIds = -1;
                            $.each(selectedUser, function (index, user) {
                                //alert("UserId: " + user.value + "- RoleId: " + user.id);
                                userIds = user.value;
                            });

                            var workflowContinueInfo = [];
                            for (var i = 0; i < model.Messages.length; i++) {
                                var workflow = {
                                    ActionId: model.ActionId,
                                    ExchangeData: model.Messages[i].ExchangeData,
                                    CurrentStateID: model.Messages[i].ID,
                                    UserComment: model.UserComment,
                                    TargetUserId: model.NextSteps[0].UserInfoes.length > 1 ? userIds : null
                                }

                                workflowContinueInfo.push(workflow);
                            }

                            dataService.postData("/app/api/Cartable/ContinueWorkflow", workflowContinueInfo).then(function (data) {
                                messageService.success('عملیات با موفقیت انجام شد.', '');
                                $uibModalInstance.close(data);
                            }, function (e) {
                                if (e.resultCode === 4)
                                    $scope.closePopup();
                            });
                        };

                        $scope.closePopup = function () {
                            $uibModalInstance.dismiss('cancel');
                        };
                    };

                    //call with check box change in multi check column.
                    //show/hide buttons under grid.
                    $scope.onChange = function (e) {
                        var elm = $(e.currentTarget);
                        if (!elm.parent().hasClass("is-view-2")) {
                            if (elm.hasClass("fa-square-o")) {
                                elm.removeClass('fa-square-o');
                                elm.addClass('fa-check-square-o');
                            }
                            else {
                                elm.removeClass('fa-check-square-o');
                                elm.addClass('fa-square-o');
                            }
                        }

                        if ($("#panel-grid-type .fa-check-square-o").length == 0) {
                            $scope.model.PanelAction = false;
                        }
                        else {
                            $scope.model.PanelAction = true;
                        }
                    }

                    //active menu item, show/hide sub menu items.
                    function bindEventForMenu() {
                        $(".tree").on("click", 'li.parent_li li', function () {
                            $("li.parent_li li").removeClass("active");
                            $(this).addClass("active");
                        })
                    }

                    //double click event
                    function dbClickRow() {
                        $("#grid tbody tr").off("dblclick");
                        $("#grid tbody tr").dblclick(function (e) {
                            e.preventDefault();
                            var selectedRow = $("#grid").data("kendoGrid").dataItem($(this).closest("tr"));
                            selectedRow.IsView = selectedRow.MessageType == 1 ? false : true;
                            $scope.$apply(function () { $scope.selected({ e: selectedRow }); });
                        });
                    }

                    $scope.positionChange = function (positionId) {
                        $scope.positionDataSource.forEach(function (item) {
                            if (item.RoleID == positionId) {
                                $scope.model.position.RelatedUserId = item.RelatedUserId;
                            }
                        });

                        $scope.getMenuItems();
                    }

                    $scope.cartableInitalize = function () {
                        dataService.getAll('/app/api/Cartable/GetCurrentUserRoles').then(function (data) {
                            $scope.positionDataSource = data;

                            data.forEach(function (item) {
                                if (item.RoleCode == $rootScope.userModel.Tag8) {
                                    $scope.model.position.RoleID = item.RoleID;
                                    $scope.model.position.RelatedUserId = item.RelatedUserId;
                                }
                            });
                            $scope.getMenuItems();
                        });

                        $scope.model.PanelAction = false; //to hide buttons panel under grid
                        if ($state.current.data.fromChildState == true) {
                            workflowId = $state.current.data.gridOptions.WorkflowId;
                            workflowStepId = $state.current.data.gridOptions.WorkflowStepId;
                        }
                        else {
                            workflowId = null;
                            workflowStepId = -1000;
                            $scope.mainGridOptions.dataSource.read();
                        }
                    }

                    $scope.cartableInitalize();
                },//End of Link Function

            };
        }
    ]);
});