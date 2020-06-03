define(['application'], function (app) {
    app.register.controller('defaultController', ['$scope', '$rootScope', '$state',
        function ($scope, $rootScope, dataService, messageService, $state) {

            $rootScope.applicationModule = "صفحه اصلی";

            $rootScope.userModel = {};
            $rootScope.forgotPass = {};
            $rootScope.visibleForLoginView = true;
            $rootScope.control = {
                loginControlVisible: false,
                registrationControlVisible: false,
                defaultControlVisible: false,
            }

            function showPanelAfterLoadpage() {
                $(".panel-loadPage").hide();
                $(".panel-after-loadpage").show();
            }

            $scope.onSelectFile = function (e) {
                $.each(e.files, function (index, value) {
                    //var ok = value.extension == ".xlsx"
                    //    || value.extension == ".xls";

                    //if (!ok) {
                    //    e.preventDefault();
                    //    messageService.error("فقط فايل اكسل قابل قبول است.");
                    //}
                    //else {
                    $scope.readMultipleFiles(value);
                    //}
                });
            }
            $scope.myFile = {
                FileName: "",
                FileData: "",
                FileSize: "",
                MimeType: "",
            };
            $scope.readMultipleFiles = function (file) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    // bind the file content
                    $scope.myFile.FileName = file.name,
                        $scope.myFile.FileData = getBase64(e.target.result),
                        $scope.myFile.FileSize = file.size,
                        $scope.myFile.MimeType = file.rawFile.type,
                        $scope.myFile.ContractId = $rootScope.contractId;

                    //dataService.postData("/app/app/api/Azmayesh/UploadWord").then(function (data) {

                    //    if (data == 0) {
                    //        messageService.info('عضو جديدي افزوده نشد.', '');
                    //        return;
                    //    }
                    //    if (data > 0) {

                    //        var message = data + ' عضو جديد افزوده شد.'
                    //        messageService.succsse(message, '');
                    //        $scope.mainGridOptions.dataSource.read();
                    //        return;
                    //    }
                    //    if (Array.isArray(data))
                    //        openErrorListModal(data);

                    //});
                }
                reader.readAsDataURL(file.rawFile);
            }
            function getBase64(file) {
                if (file)
                    //return file.replace(/^data:application\/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,/, "");
                    return file.replace(/^data:application\/msword;base64,/, "");


            }

            $scope.logoff = function () {
                showPanelAfterLoadpage();
              //  dataService.logOff(showPanelAfterLoadpage);
            };

        }
    ]);
});


