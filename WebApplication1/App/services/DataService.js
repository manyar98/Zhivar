define(['angularAMD', 'messageService'], function (app) {
    app.service('dataService', ['$state', '$http', '$q', 'messageService', 'blockUI', '$rootScope', '$interval', 'defaultUrlOtherwise',
        function ($state, $http, $q, messageService, blockUI, $rootScope, $interval, defaultUrlOtherwise) {
            var intervalPA = [];
            return {
                callBackData: function (setMethod, listApi, operationAccess) {
                    var fn = function (res) {
                        return res;
                    };
                    if (angular.isUndefined(operationAccess)) {
                        operationAccess = { canView: true };
                    }
                    var promises = [];
                    for (var i = 0; i < listApi.length; i++) {
                        promises.push(this.getAllByParams(listApi[i], operationAccess).then(fn))
                    }
                    return $q.all(promises).then(function (data) {
                        return setMethod(data);
                    });
                },
                callBackKeyValues: function (setMethod, listApi, parentName) {
                    var fn = function (res) {
                        return res;
                    };

                    if (angular.isUndefined(parentName))
                        throw Error("ParentName is Undefined in callBackKeyValues!");

                    var promises = [];
                    for (var i = 0; i < listApi.length; i++)
                        promises.push(this.getPagedData(listApi[i], { parentEntityName: parentName }).then(fn))

                    return $q.all(promises).then(function (data) {
                        return setMethod(data);
                    });
                },
                getPagedData: function (url, filterInfo, needToBlockUI) {
                    needToBlockUI != false && blockUI.start();
                    var then = this;
                    var deferred = $q.defer();
                    $http({ Method: 'GET', url: url, params: filterInfo }).success(function (response) {
                        var result = then.processResponse(response, deferred);
                        needToBlockUI != false && blockUI.stop();
                        deferred.resolve(result);
                    });
                    return deferred.promise;
                },
                getAllByParams: function (url, filterInfo, needToBlockUI) {
                    var deferred = $q.defer();
                    needToBlockUI != false && blockUI.start();
                    var then = this;
                    $.get(url, filterInfo).then(function (response) {
                        var result = then.processResponse(response, deferred);
                        needToBlockUI != false && blockUI.stop();
                        deferred.resolve(result);
                    });
                    return deferred.promise;
                },
                getValue: function (url, filterInfo) {
                    blockUI.start();
                    var deferred = $q.defer();
                    var then = this;
                    var response = angular.fromJson($.ajax({ type: "GET", url: url, data: filterInfo, async: false }).responseText);
                    var result = then.processResponse(response, deferred);
                    blockUI.stop();
                    return result;
                },
                checkAccess: function (url) {
                    var deferred = $q.defer();
                    var then = this;
                    var response = angular.fromJson($.ajax({ type: "GET", url: url, async: false }).responseText);
                    return response;
                },
                getById: function (url, id, needToBlockUI) {
                    needToBlockUI != false && blockUI.start();
                    var deferred = $q.defer();
                    var then = this;
                    $http.get(url + id).success(function (response) {
                        var result = then.processResponse(response, deferred);
                        needToBlockUI != false && blockUI.stop();
                        deferred.resolve(result);
                    });
                    return deferred.promise;
                },
                updateEntity: function (url, entity, isPopup) {
                    blockUI.start();
                    var deferred = $q.defer();
                    var then = this;
                    $http.put(url, entity).success(function (response) {
                        var result = then.processResponse(response, deferred);
                        blockUI.stop();
                        deferred.resolve(result);
                        if (angular.isDefined(isPopup) && isPopup == false) {
                            $rootScope.panelShowDialog = false;
                        }
                        else {
                            $rootScope.panelShowDialog = true;
                        }
                    });
                    return deferred.promise;
                },
                addEntity: function (url, entity, isPopup) {
                    blockUI.start();
                    var deferred = $q.defer();
                    var then = this;
                    $http.post(url, entity).success(function (response) {
                        var result = then.processResponse(response, deferred);
                        blockUI.stop();
                        deferred.resolve(result);
                        if (angular.isDefined(isPopup) && isPopup == false) {
                            $rootScope.panelShowDialog = false;
                        }
                        else {
                            $rootScope.panelShowDialog = true;
                        }
                    });
                    return deferred.promise;
                },
                postData: function (url, data, needToBlockUI) {
                    needToBlockUI != false && blockUI.start();
                    var deferred = $q.defer();
                    var then = this;
                    $http.post(url, data).success(function (response) {
                        var result = then.processResponse(response, deferred);
                        needToBlockUI != false && blockUI.stop();
                        deferred.resolve(result);
                    });
                    return deferred.promise;
                },
                deleteEntity: function (url, id) {
                    blockUI.start();
                    var deferred = $q.defer();
                    var then = this;
                    $http.delete(url + id).success(function (response) {
                        var result = then.processResponse(response, deferred);
                        blockUI.stop();
                        deferred.resolve(result);
                    });
                    return deferred.promise;
                },
                getAll: function (url, needToBlockUI) {
                    needToBlockUI != false && blockUI.start();
                    var deferred = $q.defer();
                    var then = this;
                    $http({ Method: 'GET', url: url }).success(function (response) {
                        var result = then.processResponse(response, deferred);
                        needToBlockUI != false && blockUI.stop();
                        deferred.resolve(result);
                    });
                    return deferred.promise;
                },
                getOperationAccess: function (contoller) {
                    var operationAccess = {
                        canView: false,
                        canInsert: false,
                        canUpdate: false,
                        canDelete: false,
                        canImport: false,
                        canExport: false,
                        canPrint: false,
                    };
                    var then = this;
                    var response = angular.fromJson($.ajax({ type: "GET", url: "/app/api/" + contoller + "/GetOperationAccess", async: false }).responseText);
                    var result = then.processResponse(response);

                    if (result)
                        operationAccess = {
                            canView: result.CanView,
                            canInsert: result.CanInsert,
                            canUpdate: result.CanUpdate,
                            canDelete: result.CanDelete,
                            canImport: result.CanImport,
                            canExport: result.CanExport,
                            canPrint: result.CanPrint,
                        }

                    return operationAccess;
                },
                getOperationAccessRoute: function (route) {
                    var operationAccess = {
                        canView: false,
                        canInsert: false,
                        canUpdate: false,
                        canDelete: false,
                        canImport: false,
                        canExport: false,
                        canPrint: false,
                    };
                    var then = this;
                    var response = angular.fromJson($.ajax({ type: "GET", url: route, async: false }).responseText);
                    var result = then.processResponse(response);

                    if (result) {
                        operationAccess = {
                            canView: result.CanView,
                            canInsert: result.CanInsert,
                            canUpdate: result.CanUpdate,
                            canDelete: result.CanDelete,
                            canImport: result.CanImport,
                            canExport: result.CanExport,
                            canPrint: result.CanPrint,
                        }
                    }
                    return operationAccess;
                },
                checkSession: function (isClear) {
                    if (isClear) {
                        intervalPA.forEach(function (item) {
                            $interval.cancel(item);
                        });
                    }
                    else {
                        var then = this;
                        var intervalItem = $interval(function () {
                            $.get('/app/api/Login/GetAuthenticatedUser', {}, function (data) {
                                var result = then.processResponse(data);
                                if (result && result.UserId != $rootScope.userModel.UserId) {
                                    $rootScope.control.loginControlVisible = false;
                                    $rootScope.visibleForLoginView = false;
                                    $rootScope.userModel = result;
                                    $rootScope.userModelAuthenticationType();
                                    then.checkSession(false);
                                    then.getMenus();
                                    $state.go("home");
                                }
                            })
                        }, 10000);
                        intervalPA.push(intervalItem);
                    }
                },
                logOff: function (func) {
                    then = this;
                    then.postData("/app/api/Login/LogOff/", {}).then(function (data) {
                        then.checkSession(true);
                        if (func)
                            return func();
                    });
                },
                processResponse: function (result, deferred) {
                    if (result.resultCode === 0) {
                        if (result["data"] == null) {
                            return null;
                        }
                        else if (result["data"].records) {
                            return result["data"].records;
                        }
                        else {
                            return result["data"];
                        }
                    }
                    else if (result.resultCode === 1) {
                        var failures = $("<ul></ul>")
                        for (var i = 0; i < result.failures.length; i++) {
                            failures.append('<li>' + result.failures[i] + '</li>');
                        }
                        messageService.warningMessage(failures);
                        if (deferred)
                            deferred.reject(result);
                    }
                    else if (result.resultCode === 2) {
                        messageService.warning(result.message);
                        if (deferred)
                            deferred.reject(result);
                    }
                    else if (result.resultCode === 3) {
                        $state.go(defaultUrlOtherwise);
                        if (deferred)
                            deferred.reject(result);
                    }
                    else if (result.resultCode === 4) {
                        if ($rootScope.userModel && $rootScope.userModel.UserName)
                            $state.go('error', { message: result.message, stackTrace: result.stackTrace });
                        //else
                        //    $state.go('anonymousError', { message: result.message, stackTrace: result.stackTrace });

                        if (deferred)
                            deferred.reject(result);
                    }
                    else {
                        if (deferred)
                            deferred.reject(result);
                    }
                },
                getCount: function (result) {
                    if (result.resultCode === 0)
                        return result["data"].count;
                    else
                        return 0;
                },
                getMenus: function () {
                    this.getAll("app/api/Login/GetMenus").then(function (result) {
                        $rootScope.MenuItems = result;
                    });
                }
            }
        }
    ])
});

