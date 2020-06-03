define(['application', 'digitGroup','number', 'dx', 'roweditor', 'helper'], function (app) {
    app.register.controller('balanceSheetController', ['$scope','$rootScope',
        function ($scope, $rootScope) {
    $scope.init = function () {
        $scope.loading = true;
        $rootScope.pageTitle("بارگیری ترازنامه...");
        $('#businessNav').show();

        $scope.assets = [];
        $scope.liabilities = [];
        $scope.capitals = [];

        $scope.assetsSum = 0;
        $scope.liabilitiesSum = 0;
        $scope.capitalSum = 0;
        $scope.sumRight = 0;
        $scope.sumLeft = 0;

        $scope.getBalanceSheet();
        applyScope($scope);
    };
    $scope.getBalanceSheet = function () {
        $scope.loading = true;
        $.ajax({
            type:"POST",
            url:"/app/api/FinaceReporting/GetBalanceSheet",
            contentType: "application/json"
        }).done(function(res){
            var balanceSheetData = res.data;

                $scope.loading = false;

                $scope.assets = balanceSheetData.assets;
                $scope.liabilities = balanceSheetData.liabilities;
                $scope.capitals = balanceSheetData.capitals;

                $scope.assetsSum = balanceSheetData.assetsSum;
                $scope.liabilitiesSum = balanceSheetData.liabilitiesSum;
                $scope.capitalSum = balanceSheetData.capitalSum;

                $scope.sumRight = balanceSheetData.sumRight;
                $scope.sumLeft = balanceSheetData.sumLeft;

                $scope.balanceSheetDate = balanceSheetData.balanceSheetDate;
                $rootScope.pageTitle("ترازنامه");
                $scope.$apply();
            }).fail(function (error) {
                $scope.loading = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };

    function getPrintImage(rect, pageWidthInMM, pageHeightInMM, business, currency) {

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

        gr.drawText(r.left, r.top, r.width, r.height, "ترازنامه", "iransans", "12pt", "#000", "top", "center", true, false, false, true);
        gr.drawText(r.left, r.top + 30, r.width, r.height, business.Name, "iransans", "11pt", "#000", "top", "center", true, false, false, true);
        gr.drawText(r.left, r.top + 55, r.width, r.height, $rootScope.farsiDigit($scope.balanceSheetDate), "iransans", "10pt", "#000", "top", "center", false, false, false, true);
        gr.drawLine(r.left, r.top + 90, r.width + 35, r.top + 90, "black", "1px", "solid");

        gr.drawText(r.left, r.top + 100, r.width - 10, r.height, "دارایی ها", "iransans", "12pt", "green", "top", "right", true, false, false, true);
        var assets = $scope.assets;
        var n = 130;
        for (var i = 0; i < assets.length; i++) {
            var asset = assets[i];
            var title = asset.Type === "title" ? true : false;
            if (!title)
                gr.drawText(r.left, r.top + n, r.width - 230, r.height, $scope.money(asset.Amount), "iransans", "10pt", "black", "top", "right", false, false, false, true);
            else
                gr.fillRect(r.left + (r.width / 2) + 5, r.top + n, (r.width / 2) - 10, 25, "silver", "1px", 0);
            gr.drawText(r.left, r.top + n, r.width - 10, r.height, asset.Description, "iransans", "10pt", "black", "top", "right", title, false, false, true);

            n += 30;
        }
        gr.fillRect(r.left + (r.width / 2) + 5, r.top + n, (r.width / 2) - 10, 25, "#DFF0D8", "1px", 0);
        gr.drawText(r.left, r.top + n, r.width - 10, r.height, "جمع دارایی ها", "iransans", "10pt", "black", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n, r.width - 230, r.height, $scope.money($scope.assetsSum) + " " + currency, "iransans", "10pt", "black", "top", "right", false, false, false, true);
        var nRight = n;

        gr.drawText(r.left, r.top + 100, (r.width / 2) - 10, r.height, "بدهی ها", "iransans", "12pt", "red", "top", "right", true, false, false, true);
        var liabilities = $scope.liabilities;
        n = 130;
        for (var i = 0; i < liabilities.length; i++) {
            var liability = liabilities[i];
            var title = liability.Type === "title" ? true : false;
            if (!title)
                gr.drawText(r.left, r.top + n, (r.width / 2) - 230, r.height, $scope.money(liability.Amount), "iransans", "10pt", "black", "top", "right", false, false, false, true);
            else
                gr.fillRect(r.left + 5, r.top + n, (r.width / 2) - 10, 25, "silver", "1px", 0);
            gr.drawText(r.left, r.top + n, (r.width / 2) - 10, r.height, liability.Description, "iransans", "10pt", "black", "top", "right", title, false, false, true);

            n += 30;
        }
        gr.fillRect(r.left + 5, r.top + n, (r.width / 2) - 10, 25, "#F2DEDE", "1px", 0);
        gr.drawText(r.left, r.top + n, (r.width / 2) - 10, r.height, "جمع بدهی ها", "iransans", "10pt", "black", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n, (r.width / 2) - 230, r.height, $scope.money($scope.liabilitiesSum) + " " + currency, "iransans", "10pt", "black", "top", "right", false, false, false, true);


        n += 50;
        gr.drawText(r.left, r.top + n, (r.width / 2) - 10, r.height, "حقوق صاحبان سهام", "iransans", "12pt", "blue", "top", "right", true, false, false, true);
        n += 30;
        var capitals = $scope.capitals;
        for (var i = 0; i < capitals.length; i++) {
            var capital = capitals[i];
            var title = capital.Type === "title" ? true : false;
            if (!title)
                gr.drawText(r.left, r.top + n, (r.width / 2) - 230, r.height, $scope.money(capital.Amount), "iransans", "10pt", "black", "top", "right", false, false, false, true);
            else
                gr.fillRect(r.left + 5, r.top + n, (r.width / 2) - 10, 25, "silver", "1px", 0);
            gr.drawText(r.left, r.top + n, (r.width / 2) - 10, r.height, capital.Description, "iransans", "10pt", "black", "top", "right", title, false, false, true);

            n += 30;
        }
        gr.fillRect(r.left + 5, r.top + n, (r.width / 2) - 10, 25, "#D9EDF7", "1px", 0);
        gr.drawText(r.left, r.top + n, (r.width / 2) - 10, r.height, "جمع حقوق صاحبان سهام", "iransans", "10pt", "black", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n, (r.width / 2) - 230, r.height, $scope.money($scope.capitalSum) + " " + currency, "iransans", "10pt", "black", "top", "right", false, false, false, true);
        var nLeft = n;

        n = nRight > nLeft ? nRight + 60 : nLeft + 60;
        gr.drawLine(r.left + (r.width / 2), r.top + 90, r.left + (r.width / 2), r.top + n, "black", "1px", "solid");

        gr.fillRect(r.left + 5, r.top + n, r.width - 10, 25, "silver", "1px", 0);
        gr.drawLine(r.left + 5, r.top + n, r.width + 35, r.top + n, "black", "1px", "solid", 0);
        gr.drawLine(r.left + 5, r.top + n + 25, r.width + 35, r.top + n + 25, "black", "1px", "solid", 0);
        gr.drawText(r.left, r.top + n, r.width - 10, r.height, "جمع دارایی ها", "iransans", "10pt", "black", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n, r.width - 230, r.height, $scope.money($scope.sumRight) + " " + currency, "iransans", "10pt", "black", "top", "right", false, false, false, true);

        gr.drawText(r.left, r.top + n, (r.width / 2) - 10, r.height, "جمع بدهی ها و سرمایه", "iransans", "10pt", "black", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n, (r.width / 2) - 230, r.height, $scope.money($scope.sumLeft) + " " + currency, "iransans", "10pt", "black", "top", "right", false, false, false, true);

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

        var canvas = getPrintImage(rect, pageWidthInMM, pageHeightInMM, business, $scope.getCurrency());
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
        $(document).ready(function () {
            wnd.print();
        });
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

        var canvas = getPrintImage(rect, pageWidthInMM, pageHeightInMM, business, $scope.getCurrency());

        var pdf = new jsPDF("p", "mm", [pageWidthInMM, pageHeightInMM]);
        pdf.addRawImage(canvas.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, pageWidthInMM, pageHeightInMM);

        pdf.save("report.pdf");


        document.body.removeChild(canvas);
        delete canvas;
    };
    //
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});