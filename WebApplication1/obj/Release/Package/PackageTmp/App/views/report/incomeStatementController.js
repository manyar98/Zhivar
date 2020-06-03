
    define(['application', 'digitGroup','number', 'dx', 'roweditor', 'helper'], function (app) {
        app.register.controller('incomeStatementController', ['$scope','$rootScope',
            function ($scope, $rootScope) {

    $scope.init = function () {
        $rootScope.pageTitle("بارگیری صورت سود و زیان...");
        $('#businessNav').show();

        $scope.fromDate = "";
        $scope.toDate = "";
        $scope.table = [];
        $scope.netIncome = 0;
        $scope.getIncomeStatement();
        applyScope($scope);
    };
    $scope.getIncomeStatement = function () {
        $scope.loading = true;
      
        $.ajax({
            type:"POST",
            //data: JSON.stringify({ start: "", end: "" }),
            url:"/app/api/FinaceReporting/GetIncomeStatement",
            contentType: "application/json"
        }).done(function(res){
            var incomeStatementData=res.data;
                $scope.loading = false;
                $scope.table = getKeyValue(incomeStatementData, 'table');
                $scope.netIncome = getKeyValue(incomeStatementData, 'netIncome');
                $rootScope.pageTitle("صورت سود و زیان");
                $scope.$apply();
            }).fail(function (error) {
                $scope.loading = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };


    function getPrintImage(rect, pageWidthInMM, pageHeightInMM, business, currency, finanYear) {

        var mm = rect.height / pageHeightInMM;

        var scale = 3;
        var tempCanvas = document.createElement("canvas");
        document.body.appendChild(tempCanvas);
        tempCanvas.style.display = "none";
        tempCanvas.width = rect.width * scale;
        tempCanvas.height = rect.height * scale;

        var gr = new graphics(tempCanvas);
        gr.scale(scale, scale);

        gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);

        var r = { left: 10 * mm, top: 10 * mm, width: rect.width - 20 * mm, height: rect.height - 20 * mm };

        gr.drawText(r.left, r.top, r.width, r.height, "صورت سود و زيان", "iransans", "12pt", "#000", "top", "center", true, false, false, true);
        gr.drawText(r.left, r.top + 30, r.width, r.height, business.Name, "iransans", "11pt", "#000", "top", "center", true, false, false, true);
        gr.drawText(r.left, r.top + 55, r.width, r.height, $rootScope.farsiDigit(finanYear.Name), "iransans", "10pt", "#000", "top", "center", false, false, false, true);

        gr.drawText(r.left, r.top + 100, r.width - 10, r.height, "شــرح", "iransans", "11pt", "black", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + 100, r.width / 2, r.height, "مبلغ " + "(" + currency + ")", "iransans", "11pt", "black", "top", "right", true, false, false, true);

        gr.drawLine(r.left, r.top + 130, r.width + 35, r.top + 130, "black", "1px", "solid");

        var rows = $scope.table;
        var n = 135;
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            var title = row.Type === "title" ? true : false;
            if (!title) {
                var moneyStr = row.Type === "minus" && row.Amount !== 0 ? "(" + $scope.money(row.Amount) + ")" : $scope.money(row.Amount);
                var color = row.Type === "minus" ? "red" : "black";
                gr.drawText(r.left, r.top + n, r.width / 2, r.height, moneyStr, "iransans", "10pt", color, "top", "right", false, false, false, true);
            }
            else
                gr.fillRect(r.left, r.top + n - 3, r.width - 5, 26, "silver", "1px", 0);
            gr.drawText(r.left, r.top + n, r.width - 10, r.height, row.Description, "iransans", "10pt", "black", "top", "right", title, false, false, true);
            gr.drawLine(r.left, r.top + n + 25, r.width + 35, r.top + n + 25, "black", "1px", "solid");
            n += 30;
        }

        gr.fillRect(r.left, r.top + n - 3, r.width - 5, 26, "silver", "1px", 0);
        gr.drawText(r.left, r.top + n, r.width - 10, r.height, "سود (زیان) خالص", "iransans", "10pt", "black", "top", "right", true, false, false, true);
        var moneyStr = $scope.netIncome < 0 ? "(" + $scope.money($scope.netIncome) + ")" : $scope.money($scope.netIncome);
        var color = $scope.netIncome < 0 ? "red" : "black";
        gr.drawText(r.left, r.top + n, r.width / 2, r.height, moneyStr, "iransans", "10pt", color, "top", "right", true, false, false, true);
        gr.drawLine(r.left, r.top + n + 25, r.width + 35, r.top + n + 25, "black", "1px", "solid");

        // ژیور
        if (n <= r.height - 10)
            gr.drawText(r.left, r.height - 10, r.width, r.height, "ژیور - www.smbt.ir", "iransans", "10pt", "silver", "top", "center", true, false, false, true);

        tempCanvas.ctx = gr.ctx;

        return tempCanvas;
    }
    $scope.print = function () {
        var pageWidthInMM = 210;
        var pageHeightInMM = 297;
        var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

        var business = {
            Name: "ژیور",
            LegalName: "ژیور",
            Address: "",
            PostalCode: "",
            Fax: ""
        };
        var finanYear =
            {
                Name:"سال مال منتهی به 97",
            };
        var canvas = getPrintImage(rect, pageWidthInMM, pageHeightInMM, business, $scope.getCurrency(), finanYear);
        //var canvas = getPrintImage(rect, pageWidthInMM, pageHeightInMM, business, "ریال", finanYear);
        var wnd = window.open("");

        var img = wnd.document.createElement("img");
        img.width = rect.width;
        img.height = rect.height;

        img.src = canvas.toDataURL("image/png");
        wnd.document.body.appendChild(img);

        var div = wnd.document.createElement("div");
        $(div).css("page-break-after", "always");
        wnd.document.body.appendChild(div);

        document.body.removeChild(canvas);
        delete canvas;



        wnd.document.title = "صفحه " + 1 + " از " + 1;
        setTimeout(function () {
            wnd.print();
        }, 1000);
    };
    $scope.generatePDF = function () {
        var pageWidthInMM = 210;
        var pageHeightInMM = 297;
        var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };
        var business = {
            Name: "ژیور",
            LegalName: "ژیور",
            Address: "",
            PostalCode: "",
            Fax: ""
        };
        var finanYear =
        {
            Name: "سال مال منتهی به 97",
        };
        var canvas = getPrintImage(rect, pageWidthInMM, pageHeightInMM, business, $scope.getCurrency(), finanYear);
        var pdf = new jsPDF("p", "mm", [pageWidthInMM, pageHeightInMM]);
        pdf.addRawImage(canvas.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, pageWidthInMM, pageHeightInMM);
        pdf.save("report.pdf");
        document.body.removeChild(canvas);
        delete canvas;
    };
    
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});