define(['application'], function (app) {
    app.register.controller('dashboardController', ['$rootScope', '$scope', '$state', function ($rootScope,$scope, $state) {
            $rootScope.applicationDescription = "در این صفحه امکان جستجو، اضافه و ویرایش بیماری ها وجود دارد.";

    }])
}); 
