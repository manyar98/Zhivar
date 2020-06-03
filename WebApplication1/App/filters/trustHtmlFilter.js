define(['angularAMD', 'momentJalaali'], function (app) {
    app.filter('trustHtml', ['$sce', function ($sce) {
        return function (htmlEncode) {
            htmlEncode = htmlEncode || '<br />';
            return $sce.trustAsHtml(htmlEncode);
        };
    }])
});