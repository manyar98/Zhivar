define(['application'], function (app) {
    app.register.service('redirectService', ['$rootScope',
        function ($rootScope) {
            function postFn(location, args) {
                var form = '';
                for (var arg in args) {
                    form += '<input type="hidden" name="' + arg + '" value="' + args[arg] + '">';
                }
                $('<form action="' + location + '" method="POST">' + form + '</form>').appendTo($(document.body)).submit();
            }

            return {
                postData: function (data) {
                    postFn('http://localhost:9194/Home/Index', data);
                }
            }
        }
    ])
});

