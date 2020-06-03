// فاکتور
function getInvoicePageCount(pageSize, invoiceSettings, itemsLength) {
	var rpp0 = invoiceSettings.rowPerPage ? parseInt(invoiceSettings.rowPerPage) : null;
	var rpp = { A4portrait: rpp0 || 18, A4landscape: rpp0 || 9, A5: rpp0 || 9, FishPrint: rpp0 || 10 };
	var rowsInsteadVendorBox = invoiceSettings.showVendorInfo ? 2 : 0;
	return Math.ceil((itemsLength + 4) / (rpp[pageSize.name] + rowsInsteadVendorBox));
};

function getCostPageCount(pageSize, costSettings, itemsLength) {
    var rpp0 = costSettings.rowPerPage ? parseInt(costSettings.rowPerPage) : null;
    var rpp = { A4portrait: rpp0 || 18, A4landscape: rpp0 || 9, A5: rpp0 || 9, FishPrint: rpp0 || 10 };
    var rowsInsteadVendorBox = costSettings.showVendorInfo ? 2 : 0;
    return Math.ceil((itemsLength + 4) / (rpp[pageSize.name] + rowsInsteadVendorBox));
};

function invoiceFactory(invoice, info) {
	// فاکتور رسمی
	if (info.pageSize.name === "A4landscape2")
		return getInvoicePrintImage_A4Landscape2(invoice, info);
	else if (info.pageSize.name === "8cm")
		return getInvoicePrintImage_8cm(invoice, info);

	var chromeFactor = 0;
	if (isChrome())
		chromeFactor = 5;

	var rect = info.rect;
	var pageSize = info.pageSize;
	var pageNumber = info.pageNumber;
	var invoiceSettings = info.invoiceSettings;
	var totalDiscount = info.totalDiscount;
	var totalTax = info.totalTax;

	var reachPageEnd = false;

	var rpp0 = invoiceSettings.rowPerPage ? parseInt(invoiceSettings.rowPerPage) : null;
	var rpp = { A4portrait: rpp0 || 18, A4landscape: rpp0 || 9, A5: rpp0 || 9, FishPrint: rpp0 || 10 };
	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = invoiceSettings.font === "" ? "iransans" : invoiceSettings.font;
	var invoiceFontSize = invoiceSettings.fontSize;
	var fontX = 0;
	if (invoiceFontSize === "Small") fontX = -1;
	else if (invoiceFontSize === "Medium") fontX = 0;
	else if (invoiceFontSize === "Large") fontX = 1;
	var fontSize = 10;

	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);
	var invoicePageCount = getInvoicePageCount(pageSize, invoiceSettings, invoice.InvoiceItems.length);

	gr.fillRect(rect.left, rect.top - chromeFactor, rect.width, rect.height, "#fff", "1px", 0);

	var topMargin = invoiceSettings.topMargin;
	var bottomMargin = invoiceSettings.bottomMargin;
	var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
	var n = 0;
	var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;

	var titleFontSize = fontSize + fontX + 2 + "pt";
	var headerFontSize = fontSize + fontX + "pt";
	var addressFont = fontSize + fontX - 1 + "pt";
	if (pageSize.name === "A5") {
		titleFontSize = fontSize + fontX + "pt";
		headerFontSize = fontSize + fontX - 2 + "pt";
		addressFont = fontSize + fontX - 2 + "pt";
	}

	var invoiceTitle = "";
	if (invoice.InvoiceType === 0 && invoice.Status > 1)
		invoiceTitle = invoiceSettings.saleInvoiceTitle;
	else if (invoice.InvoiceType === 0 && invoice.Status <= 1)
		invoiceTitle = invoiceSettings.saleDraftInvoiceTitle;
	else if (invoice.InvoiceType === 1)
		invoiceTitle = invoiceSettings.purchaseInvoiceTitle;
	else if (invoice.InvoiceType === 4)
		invoiceTitle = "ضایعات";
	else if (invoice.InvoiceType === 2)
		invoiceTitle = "فاکتور برگشت از فروش";
	else if (invoice.InvoiceType === 3)
		invoiceTitle = "فاکتور برگشت از خرید";


	var img = document.getElementById("imgLogo");
	if (invoiceSettings.businessLogo)
		gr.drawImageElement(r.width - img.clientWidth + 40, r.top, img.clientWidth, img.clientHeight, img);

	gr.drawText(r.left, r.top, r.width, r.height, strName, font, titleFontSize, "#000", "top", "center", true, false, false, true);
	gr.drawText(r.left, r.top + 30, r.width, r.height, invoiceTitle, font, titleFontSize, "gray", "top", "center", true, false, false, true);
	if (invoice.InvoiceType === 1)
		gr.drawText(r.left, r.top, r.width, r.height, "ارجاع: " + invoice.Reference, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);
	else {
		gr.drawText(r.left, r.top, r.width, r.height, "شماره: " + invoice.Number, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);
		if (invoice.Reference !== "")
			gr.drawText(r.left, r.top + 50, r.width, r.height, "ارجاع: " + invoice.Reference, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);
	}

	gr.drawText(r.left, r.top + 25, r.width, r.height, "تاریخ: " + invoice.DispalyDate, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);
	n += 75;
	//        gr.drawLine(r.left, r.top + n, r.width + 35, r.top + n, "black", "1px", "solid");

	var strAddress = "";

	// vendor info box
	var strEconomicCode;
	if (invoice.InvoiceType !== 4 && (invoiceSettings.showVendorInfo || invoice.InvoiceType === 1 || invoice.InvoiceType === 3)) {
		if (invoice.InvoiceType === 1 || invoice.InvoiceType === 3) {
			strName = invoice.Contact.Name + "";
			strEconomicCode = invoice.Contact.NationalCode;
			strAddress = invoice.Contact.State ? invoice.Contact.State + "، " : "";
			strAddress += invoice.Contact.City ? invoice.Contact.City + "، " : "";
			strAddress += invoice.Contact.Address ? invoice.Contact.Address + "، " : "";
			strAddress += invoice.Contact.PostalCode ? "  کد پستی: " + invoice.Contact.PostalCode : "";
			strAddress += invoice.Contact.Phone ? "  تلفن: " + invoice.Contact.Phone : "";
			strAddress += invoice.Contact.Mobile ? "  تلفن همراه: " + invoice.Contact.Mobile : "";
			strAddress += invoice.Contact.Fax ? "  فکس: " + invoice.Contact.Fax : "";
		} else {
			strEconomicCode = info.business.EconomicCode ? info.business.EconomicCode : null;
			strAddress = info.business.Address;
			if (info.business.PostalCode && info.business.PostalCode !== "")
				strAddress += info.business.PostalCode !== "" ? "  کد پستی: " + info.business.PostalCode : "";
			if (info.business.Phone && info.business.Phone !== "")
				strAddress += info.business.Phone !== "" ? "  تلفن: " + info.business.Phone : "";
			if (info.business.Fax && info.business.Fax !== "")
				strAddress += info.business.Fax !== "" ? "  فکس: " + info.business.Fax : "";
		}
		strAddress = strAddress.trim();
		strAddress = strAddress.replace(/،$/, "");// trim end char '،'
		gr.drawRect(r.left, r.top + n - chromeFactor, r.width, 90, "black", "1px", "solid", 5);
		gr.fillRect(r.width - 7, r.top + n + 1 - chromeFactor, 45, 88, "silver", "1px", 5);
		gr.drawTextRotated(-90, r.left, r.top + n + 14, r.width - 35, r.height, "فروشنــده", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 70, r.height, "فروشنده: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 140, r.height, strName, font, headerFontSize, "#000", "top", "right", false, false, false, true);
		if (strEconomicCode) {   // اگر شماره اقتصادی داشت نشان بده
			if (pageSize.name === "A4portrait" || pageSize.name === "A4landscape") {
				gr.drawText(r.left, r.top + n + 5, r.width / 2 - 90, r.height, "شماره اقتصادی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
				gr.drawText(r.left, r.top + n + 5, r.width / 2 - 190, r.height, strEconomicCode, font, headerFontSize, "#000", "top", "right", false, false, false, true);
			}
			else if (pageSize.name === "A5") {
				gr.drawText(r.left, r.top + n + 5, r.width / 2 - 70, r.height, "شماره اقتصادی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
				gr.drawText(r.left, r.top + n + 5, r.width / 2 - 170, r.height, strEconomicCode, font, headerFontSize, "#000", "top", "right", false, false, false, true);
			}
		}

		if (invoice.InvoiceType === 1 || invoice.InvoiceType === 3)
			gr.drawText(r.left, r.top + n + 35, r.width - 70, r.height, "نشانی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		else if (info.business.Address && info.business.Address !== "")
			gr.drawText(r.left, r.top + n + 35, r.width - 70, r.height, "نشانی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 35, r.width - 140, r.height, strAddress, font, addressFont, "#000", "top", "right", false, false, false, true);

		n += 95;
	}
	// customer info box (if sale invoice)
	var strContactCode = "";
	if (invoice.Contact)
		strContactCode = invoice.Contact.EconomicCode ? invoice.Contact.EconomicCode : invoice.Contact.NationalCode;
	if (invoice.InvoiceType !== 4 && (invoice.InvoiceType === 0 || invoice.InvoiceType === 2)) {
		gr.drawRect(r.left, r.top + n - chromeFactor, r.width, 90, "black", "1px", "solid", 5);
		gr.fillRect(r.width - 7, r.top + n + 1 - chromeFactor, 45, 88, "silver", "1px", 5);
		gr.drawTextRotated(-90, r.left, r.top + n + 14, r.width - 35, r.height, "خریــــدار", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 70, r.height, "خریدار: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 140, r.height, invoice.ContactTitle, font, headerFontSize, "#000", "top", "right", false, false, false, true);
		if (pageSize.name === "A4portrait" || pageSize.name === "A4landscape") {
			gr.drawText(r.left, r.top + n + 5, r.width / 2 - 90, r.height, "شماره اقتصادی / ملی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 5, r.width / 2 - 225, r.height, strContactCode, font, headerFontSize, "#000", "top", "right", false, false, false, true);
		} else if (pageSize.name === "A5") {
			gr.drawText(r.left, r.top + n + 5, r.width / 2 - 70, r.height, "شماره اقتصادی / ملی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 5, r.width / 2 - 180, r.height, strContactCode, font, headerFontSize, "#000", "top", "right", false, false, false, true);
		}

		strAddress = invoice.Contact.State ? invoice.Contact.State + "، " : "";
		strAddress += invoice.Contact.City ? invoice.Contact.City + "، " : "";
		strAddress += invoice.Contact.Address ? invoice.Contact.Address + "، " : "";
		strAddress += invoice.Contact.PostalCode ? "  کد پستی: " + invoice.Contact.PostalCode : "";
		strAddress += invoice.Contact.Phone ? "  تلفن: " + invoice.Contact.Phone : "";
		strAddress += invoice.Contact.Mobile ? "  تلفن همراه: " + invoice.Contact.Mobile : "";
		strAddress += invoice.Contact.Fax ? "  فکس: " + invoice.Contact.Fax : "";
		strAddress = strAddress.trim();
		strAddress = strAddress.replace(/،$/, "");// trim end char '،'
		gr.drawText(r.left, r.top + n + 35, r.width - 70, r.height, "نشانی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 35, r.width - 140, r.height, strAddress, font, addressFont, "#000", "top", "right", false, false, false, true);
		n += 95;
	}

	// invoice items (rows)
	var rowStart = n;
	if (info.itemCursor + 1 <= invoice.InvoiceItems.length) {    // اگر آیتمی باقیمانده بود جهت رسم
		// draw header
		gr.fillRect(r.left, r.top + n - chromeFactor, r.width, 50, "#dcdcdc", "1px", 0);
		gr.drawLine(r.left, r.top + n - chromeFactor, r.width + 40, r.top + n - chromeFactor, "black", "1px", "solid");
		gr.drawLine(r.left, r.top + n + 50 - chromeFactor, r.width + 40, r.top + n + 50 - chromeFactor, "black", "1px", "solid");
		//        gr.drawRect(r.left, r.top + n, r.width, 50, "black", "1px", "solid", 0);
		n += 5;
		gr.drawText(r.left, r.top + n, r.width - 10, r.height, "ردیف", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 130, r.height, "شــــرح", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
		if (pageSize.name === "A4portrait" || pageSize.name === "A4landscape") {
			gr.drawText(r.left + 410, r.top + n, 50, r.height, "تعداد", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left + 330, r.top + n, 80, r.height, "مبلغ واحد", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
			gr.drawText(r.left + 330, r.top + n + 20, 80, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
		}
		else if (pageSize.name === "A5") {
			gr.drawText(r.left + 195, r.top + n, 50, r.height, "تعداد", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left + 110, r.top + n, 80, r.height, "مبلغ واحد", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
			gr.drawText(r.left + 110, r.top + n + 20, 80, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
		}
		if (pageSize.name === "A4portrait" || pageSize.name === "A4landscape") {
			gr.drawText(r.left + 240, r.top + n, 60, r.height, "تخفیف", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
			gr.drawText(r.left + 240, r.top + n + 20, 60, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
			gr.drawText(r.left + 140, r.top + n, 60, r.height, "مالیات", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
			gr.drawText(r.left + 140, r.top + n + 20, 60, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
		}
		if (pageSize.name === "A4portrait" || pageSize.name === "A4landscape") {
			gr.drawText(r.left + 30, r.top + n, 70, r.height, "مبلغ کل", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
			gr.drawText(r.left + 30, r.top + n + 20, 70, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
		}
		else if (pageSize.name === "A5") {
			gr.drawText(r.left + 20, r.top + n, 70, r.height, "مبلغ کل", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
			gr.drawText(r.left + 20, r.top + n + 20, 70, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
		}

		n += 55;
		var items = invoice.InvoiceItems;
		// draw invoice rows (invoice Items)
		while (true) {
			if (info.itemCursor + 1 > items.length) break;
			var isBold = false;
			var rowFontSize = fontSize + fontX - 2 + "pt";
			var item = items[info.itemCursor];
			var des2Rows = false;
			if (pageSize.name === "A4landscape") { isBold = true; rowFontSize = fontSize + fontX - 1 + "pt" }

			if (pageSize.name === "A5" && gr.getTextSize(item.Description, font, rowFontSize, isBold, false).width > 205) des2Rows = true;
			else if (pageSize.name === "A4portrait" && gr.getTextSize(item.Description, font, rowFontSize, isBold, false).width > 230) des2Rows = true;
			else if (pageSize.name === "A4landscape" && gr.getTextSize(item.Description, font, rowFontSize, isBold, false).width > 550) des2Rows = true;

			if (r.top + n + 30 + (des2Rows ? 30 : 0) >= r.height) {
				reachPageEnd = true; break;
			}
			info.itemCursor++;

			gr.drawText(r.width - 10, r.top + n, 50, r.height, item.RowNumber+1, font, rowFontSize, "black", "top", "center", isBold, false, false, true);

			var desWidth = 300;
			if (pageSize.name === "A5") desWidth = 205;
			else if (pageSize.name === "A4portrait") desWidth = 230;
			else if (pageSize.name === "A4landscape") desWidth = 550;
			// شرح آیتم
			gr.drawText(r.width - 15 - desWidth, r.top + n, desWidth, 52, item.Description, font, rowFontSize, "black", "top", "right", isBold, false, false, true);

			if (pageSize.name === "A4portrait" || pageSize.name === "A4landscape") {
				if (invoiceSettings.showItemUnit && item.Item) {
					gr.drawText(r.left + 415, r.top + n - 8, 60, r.height, item.Quantity, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
					gr.drawText(r.left + 415, r.top + n + 8, 60, r.height, item.Item.Unit, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
				}
				else
					gr.drawText(r.left + 415, r.top + n, 60, r.height, item.Quantity, font, rowFontSize, "black", "top", "center", isBold, false, false, true);

				if (!invoiceSettings.hidePrices) {
					gr.drawText(r.left + 330, r.top + n, 80, r.height, Hesabfa.money(item.UnitPrice), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
					gr.drawText(r.left + 230, r.top + n, 80, r.height, Hesabfa.money(item.Discount), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
					gr.drawText(r.left + 130, r.top + n, 80, r.height, Hesabfa.money(item.Tax), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
				}

				if (!invoiceSettings.hidePrices)
					gr.drawText(r.left + 20, r.top + n, 80, r.height, Hesabfa.money(item.TotalAmount), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
			}
			else if (pageSize.name === "A5") {
				if (invoiceSettings.showItemUnit && item.Item) {
					gr.drawText(r.left + 200, r.top + n - 8, 60, r.height, item.Quantity, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
					gr.drawText(r.left + 200, r.top + n + 8, 60, r.height, item.Item.Unit, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
				} else
					gr.drawText(r.left + 200, r.top + n, 60, r.height, item.Quantity, font, rowFontSize, "black", "top", "center", isBold, false, false, true);

				if (!invoiceSettings.hidePrices) {
					gr.drawText(r.left + 110, r.top + n, 80, r.height, Hesabfa.money(item.UnitPrice), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
					gr.drawText(r.left + 15, r.top + n, 80, r.height, Hesabfa.money(item.TotalAmount), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
				}
			}
			gr.drawLine(r.left, r.top + n + 30 + (des2Rows ? 25 : 0) - chromeFactor, r.width + 40, r.top + n + 30 + (des2Rows ? 25 : 0) - chromeFactor, "black", "1px", "solid");
			n += 40;
			if (des2Rows) n += 25;
		}

		// خطوط عمودی فاکتور
		gr.drawLine(r.width + 40, r.top + rowStart - chromeFactor, r.width + 40, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		gr.drawLine(r.width - 10, r.top + rowStart - chromeFactor, r.width - 10, r.top + n - 10 - chromeFactor, "black", "1px", "solid");

		if (pageSize.name === "A4portrait") {
			gr.drawLine(r.width - 240, r.top + rowStart - chromeFactor, r.width - 240, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
			gr.drawLine(r.width - 290, r.top + rowStart - chromeFactor, r.width - 290, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
			gr.drawLine(r.width - 390, r.top + rowStart - chromeFactor, r.width - 390, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
			gr.drawLine(r.width - 490, r.top + rowStart - chromeFactor, r.width - 490, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
			gr.drawLine(r.width - 590, r.top + rowStart - chromeFactor, r.width - 590, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		}
		else if (pageSize.name === "A4landscape") {
			gr.drawLine(r.left + 470, r.top + rowStart - chromeFactor, r.left + 470, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
			gr.drawLine(r.left + 420, r.top + rowStart - chromeFactor, r.left + 420, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
			gr.drawLine(r.left + 320, r.top + rowStart - chromeFactor, r.left + 320, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
			gr.drawLine(r.left + 220, r.top + rowStart - chromeFactor, r.left + 220, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
			gr.drawLine(r.left + 120, r.top + rowStart - chromeFactor, r.left + 120, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		}
		else if (pageSize.name === "A5") {
			gr.drawLine(r.left + 255, r.top + rowStart - chromeFactor, r.left + 255, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
			gr.drawLine(r.left + 200, r.top + rowStart - chromeFactor, r.left + 200, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
			gr.drawLine(r.left + 105, r.top + rowStart - chromeFactor, r.left + 105, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		}
		gr.drawLine(r.left, r.top + rowStart - chromeFactor, r.left, r.top + n - 10 - chromeFactor, "black", "1px", "solid");

	} // end of if items remaind

	// summary
	var nn = 0;
	if (totalDiscount > 0) nn += 30;
	if (totalTax > 0) nn += 30;
	if (invoiceSettings.hidePrices) info.isSummaryPrinted = true;
	if (!reachPageEnd && !info.isSummaryPrinted && info.itemCursor + 1 > invoice.InvoiceItems.length) {
		if (r.top + n + 60 + nn <= r.height) {

			if (invoiceSettings.showAmountInWords && invoiceSettings.showAmountInWords === true) {
				gr.drawText(r.left, r.top + n, r.width, r.height, "مبلغ کل به حروف: " + wordifyfa(invoice.Payable, 0) + " " + info.currency,
					font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
				gr.drawLine(r.width - 270, r.top + n + 25 - chromeFactor, r.width + 40, r.top + n + 25 - chromeFactor, "black", "1px", "solid");
				gr.drawLine(r.width - 270, r.top + n + 28 - chromeFactor, r.width + 40, r.top + n + 28 - chromeFactor, "black", "1px", "solid");
				n += 30;
			}

			gr.drawText(r.left + 130, r.top + n, 120, r.height, "جمع", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(invoice.Sum) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
			n += 30;
			if (totalDiscount > 0) {
				gr.drawText(r.left + 130, r.top + n, 120, r.height, "جمع تخفیف", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
				gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(totalDiscount) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
				n += 30;
			}
			if (totalTax > 0) {
				gr.drawText(r.left + 130, r.top + n, 120, r.height, "جمع مالیات", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
				gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(totalTax) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
				n += 30;
			}
			gr.drawLine(r.left, r.top + n - chromeFactor, r.left + 270, r.top + n - chromeFactor, "black", "1px", "solid");
			n += 10;
			gr.drawText(r.left + 130, r.top + n, 120, r.height, "مبلغ کل", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(invoice.Payable) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
			gr.drawLine(r.left, r.top + n + 25 - chromeFactor, r.left + 270, r.top + n + 25 - chromeFactor, "black", "1px", "solid");
			gr.drawLine(r.left, r.top + n + 28 - chromeFactor, r.left + 270, r.top + n + 28 - chromeFactor, "black", "1px", "solid");

			// نمایش مانده حساب مشتری
			if (invoiceSettings.showCustomerBalance && invoice.InvoiceType === 0 && invoice.Status >= 2 && invoice.InvoiceType !== 4) {
				var Balance = invoice.Contact.Liability - invoice.Contact.Credits;
				var BalanceType = Balance > 0 ? "(بدهکار)" : "(بستانکار)";
				if (Balance === 0) BalanceType = "";
				if (Balance < 0) Balance = Balance * -1;

				var PriorBalance = 0;
				var PriorBalanceType = "";
				if (invoice.Status === 3 || invoice.Paid >= invoice.Payable) {
					PriorBalance = Balance;
					PriorBalanceType = BalanceType;
				} else {
					PriorBalance = BalanceType === "(بدهکار)" || BalanceType === "" ? Balance - invoice.Payable : Balance + invoice.Payable;
					PriorBalanceType = PriorBalance > 0 ? BalanceType : (BalanceType === "(بدهکار)" ? "(بستانکار)" : "(بدهکار)");
					if (PriorBalance === 0) PriorBalanceType = "";
					if (PriorBalance < 0) PriorBalance = PriorBalance * -1;
				}

				var fontBalanceSize = fontSize + fontX - 1;
				var fontBalanceBold = true;
				if (pageSize.name === "A5") {
					fontBalanceSize = fontBalanceSize - 2;
					fontBalanceBold = false;
				}

				if (pageSize.name === "A5") {
					gr.drawText((r.width / 2) + 30, r.top + n - 35, (r.width / 2) + 10, r.height, "مانده حساب شما از قبل: " + Hesabfa.money(PriorBalance) + " " + info.currency
                        + " " + PriorBalanceType, font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
					gr.drawText((r.width / 2) + 20, r.top + n - 10, (r.width / 2) + 20, r.height, "مانده حساب شما با احتساب این فاکتور:", font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
					gr.drawText((r.width / 2) + 20, r.top + n + 10, (r.width / 2) + 20, r.height, Hesabfa.money(Balance) + " " + info.currency
                        + " " + BalanceType, font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
				}
				else {
					gr.drawText((r.width / 2) + 20, r.top + n - 30, (r.width / 2) + 20, r.height, "مانده حساب شما از قبل:", font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
					gr.drawText((r.width / 2) + 20, r.top + n - 30, (r.width / 2) + 20, r.height, Hesabfa.money(PriorBalance) + " " + info.currency
                        + " " + PriorBalanceType, font, fontBalanceSize + "pt", "black", "top", "left", fontBalanceBold, false, false, true);
					gr.drawText((r.width / 2) + 20, r.top + n, (r.width / 2) + 20, r.height, "مانده حساب شما با احتساب این فاکتور:", font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
					gr.drawText((r.width / 2) + 20, r.top + n, (r.width / 2) + 20, r.height, Hesabfa.money(Balance) + " " + info.currency
                        + " " + BalanceType, font, fontBalanceSize + "pt", "black", "top", "left", fontBalanceBold, false, false, true);
				}
			}

			info.isSummaryPrinted = true;
		} else reachPageEnd = true;
	}

	// transactions if exist
	if (!reachPageEnd && info.isSummaryPrinted && info.payments && info.payments.length > 0 && !info.isTransactionsPrinted && invoiceSettings.showTransactions) {
		//        if (r.top + n + (info.payments.length * 25) + 80 <= r.height) {
		if (r.top + n + 60 <= r.height) {
			if (!info.paymentCursor) info.paymentCursor = 0;
			var rowFontSize2 = fontSize + fontX - 2 + "pt";
			n += 30;
			var start2 = r.top + n;
			var titleTranses = invoice.InvoiceType === 0 || invoice.InvoiceType === 3 ? "دریافت ها" : "پرداخت ها";
			gr.fillRect(r.width - 40, r.top + n - chromeFactor, 80, 23, "#dcdcdc", "1px", 0);
			gr.drawText(r.left, r.top + n + 1, r.width - 10, 25, titleTranses, font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			n += 5;
			while (true) {
				if (info.paymentCursor + 1 > info.payments.length) break;
				if (r.top + n + 25 > r.height) {
					reachPageEnd = true; break;
				}
				var payment = info.payments[info.paymentCursor];
				n += 25;
				var t = Hesabfa.money(payment.Amount) + " " + info.currency;
				t += " در تاریخ " + Hesabfa.farsiDigit(payment.DisplayDate);
				if (payment.Cheque == null)
					t += (invoice.InvoiceType === 0 || invoice.InvoiceType === 3 ? " به" : " از") + " " + payment.DetailAccount.Name;
				if (payment.Cheque != null)
					t += " طی چک شماره " + payment.Cheque.ChequeNumber;
				if (payment.Reference)
					t += " ارجاع: " + payment.Reference;
				gr.drawText(r.left, r.top + n, r.width - 10, 25, (info.paymentCursor + 1), font, fontSize - 1 + "pt", "black", "top", "right", false, false, false, true);
				gr.drawText(r.left, r.top + n, r.width - 40, 25, t, font, fontSize - 1 + "pt", "black", "top", "right", false, false, false, true);
				info.paymentCursor++;
				gr.drawRect(r.left, r.top + n - 3 - chromeFactor, r.width + 1, 25, "black", "1px", "solid");
				gr.drawLine(r.width + 10, r.top + n - 3 - chromeFactor, r.width + 10, r.top + n - 3 + 25 - chromeFactor, "black", "1px", "solid");
			}
			if (!reachPageEnd && r.top + n + 25 <= r.height) {
				n += 25;
				var t5 = "کل مبلغ " + (invoice.InvoiceType === 0 || invoice.InvoiceType === 3 ? "دریافت" : "پرداخت") + " شده: ";
				t5 += Hesabfa.money(invoice.Paid) + " " + info.currency;
				t5 += "  |  مبلغ باقیمانده: " + Hesabfa.money(invoice.Rest) + " " + info.currency;
				gr.fillRect(r.left, r.top + n - chromeFactor, r.width + 1, 30, "#dcdcdc", "1px", 0);
				gr.drawText(r.left, r.top + n + 5, r.width - 10, 25, t5, font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
				gr.drawRect(r.left, r.top + n - chromeFactor, r.width + 1, 30, "black", "1px", "solid");
				info.isTransactionsPrinted = true;
			} else reachPageEnd = true;
		} else reachPageEnd = true;
	}
	else if (!info.payments || info.payments.length === 0 || !invoiceSettings.showTransactions) info.isTransactionsPrinted = true;

	// footer note if exist
	var note = invoice.Status === 0 ? invoiceSettings.footerNoteDraft : invoiceSettings.footerNote;
	if (!reachPageEnd && info.isTransactionsPrinted && note && !info.isFooterNotePrinted) {
		var blockSize = gr.simulate(r.left, r.top + n, r.width, r.height, note, font, fontSize + fontX - 1 + "pt", "black", "top", "right", false, false, false, true);
		if (r.top + n + (blockSize.height) <= r.height) {
			n += 40;
			gr.drawText(r.left, r.top + n, r.width, r.height, note, font, fontSize + fontX - 1 + "pt", "black", "top", "right", false, false, false, true);
			n += blockSize.height;
			info.isFooterNotePrinted = true;
		} else reachPageEnd = true;
	}
	else if (!note) info.isFooterNotePrinted = true;

	// signature place if true
	if (invoice.InvoiceType === 4) invoiceSettings.showSignaturePlace = false;
	if (!reachPageEnd && info.isFooterNotePrinted && !info.isSignaturePrinted && invoiceSettings.showSignaturePlace) {
		if (r.top + n + 60 <= r.height) {
			n += 70;
			gr.drawText(r.left + (r.width / 2), r.top + n, (r.width / 2), r.height, "مهر و امضای فروشنده:", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
			gr.drawText(r.left, r.top + n, (r.width / 2), r.height, "مهر و امضای خریدار:", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
			info.isSignaturePrinted = true;
		} else reachPageEnd = true;
	}
	else if (!invoiceSettings.showSignaturePlace) info.isSignaturePrinted = true;

	if (info.isSummaryPrinted && info.isTransactionsPrinted && info.isFooterNotePrinted && info.isSignaturePrinted)
		info.done = true;

	if (pageNumber > 1 || !info.done)
		gr.drawText(r.left, r.height, r.width, 35, "صفحه " + pageNumber, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
};
function costFactory(cost, info) {
    // فاکتور رسمی
    if (info.pageSize.name === "A4landscape2")
        return getCostPrintImage_A4Landscape2(cost, info);
    else if (info.pageSize.name === "8cm")
        return getInvoicePrintImage_8cm(invoice, info);

    var chromeFactor = 0;
    if (isChrome())
        chromeFactor = 5;

    var rect = info.rect;
    var pageSize = info.pageSize;
    var pageNumber = info.pageNumber;
    var invoiceSettings = info.invoiceSettings;
    var totalDiscount = info.totalDiscount;
    var totalTax = info.totalTax;

    var reachPageEnd = false;

    var rpp0 = invoiceSettings.rowPerPage ? parseInt(invoiceSettings.rowPerPage) : null;
    var rpp = { A4portrait: rpp0 || 18, A4landscape: rpp0 || 9, A5: rpp0 || 9, FishPrint: rpp0 || 10 };
    var mm = rect.height / pageSize.height; // page height in mm
    var scale = 3;
    var tempCanvas = document.createElement("canvas");
    document.body.appendChild(tempCanvas);
    tempCanvas.style.display = "none";
    tempCanvas.width = rect.width * scale;
    tempCanvas.height = rect.height * scale;

    var font = invoiceSettings.font === "" ? "iransans" : invoiceSettings.font;
    var invoiceFontSize = invoiceSettings.fontSize;
    var fontX = 0;
    if (invoiceFontSize === "Small") fontX = -1;
    else if (invoiceFontSize === "Medium") fontX = 0;
    else if (invoiceFontSize === "Large") fontX = 1;
    var fontSize = 10;

    var gr = new graphics(tempCanvas);
    gr.scale(scale, scale);
    var costPageCount = getCostPageCount(pageSize, invoiceSettings, cost.CostItems.length);

    gr.fillRect(rect.left, rect.top - chromeFactor, rect.width, rect.height, "#fff", "1px", 0);

    var topMargin = invoiceSettings.topMargin;
    var bottomMargin = invoiceSettings.bottomMargin;
    var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
    var n = 0;
    var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;

    var titleFontSize = fontSize + fontX + 2 + "pt";
    var headerFontSize = fontSize + fontX + "pt";
    var addressFont = fontSize + fontX - 1 + "pt";
    if (pageSize.name === "A5") {
        titleFontSize = fontSize + fontX + "pt";
        headerFontSize = fontSize + fontX - 2 + "pt";
        addressFont = fontSize + fontX - 2 + "pt";
    }

    var costTitle = "صورت هزینه";
    


    var img = document.getElementById("imgLogo");
    if (invoiceSettings.businessLogo)
        gr.drawImageElement(r.width - img.clientWidth + 40, r.top, img.clientWidth, img.clientHeight, img);

    gr.drawText(r.left, r.top, r.width, r.height, strName, font, titleFontSize, "#000", "top", "center", true, false, false, true);
    gr.drawText(r.left, r.top + 30, r.width, r.height, costTitle, font, titleFontSize, "gray", "top", "center", true, false, false, true);
    gr.drawText(r.left, r.top, r.width, r.height, "شماره: " + cost.Number, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);


    gr.drawText(r.left, r.top + 25, r.width, r.height, "تاریخ: " + cost.DispalyDate, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);
    n += 75;
    //        gr.drawLine(r.left, r.top + n, r.width + 35, r.top + n, "black", "1px", "solid");

    var strAddress = "";

    // vendor info box
    var strEconomicCode;
    if (invoiceSettings.showVendorInfo ) {
   
            strName = cost.Contact.Name + "";
            strEconomicCode = cost.Contact.NationalCode;
            strAddress = cost.Contact.State ? cost.Contact.State + "، " : "";
            strAddress += cost.Contact.City ? cost.Contact.City + "، " : "";
            strAddress += cost.Contact.Address ? cost.Contact.Address + "، " : "";
            strAddress += cost.Contact.PostalCode ? "  کد پستی: " + cost.Contact.PostalCode : "";
            strAddress += cost.Contact.Phone ? "  تلفن: " + cost.Contact.Phone : "";
            strAddress += cost.Contact.Mobile ? "  تلفن همراه: " + cost.Contact.Mobile : "";
            strAddress += cost.Contact.Fax ? "  فکس: " + cost.Contact.Fax : "";
      
        strAddress = strAddress.trim();
        strAddress = strAddress.replace(/،$/, "");// trim end char '،'
        gr.drawRect(r.left, r.top + n - chromeFactor, r.width, 90, "black", "1px", "solid", 5);
        gr.fillRect(r.width - 7, r.top + n + 1 - chromeFactor, 45, 88, "silver", "1px", 5);
        gr.drawTextRotated(-90, r.left, r.top + n + 14, r.width - 35, r.height, "طرف حساب", font, headerFontSize, "#000", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n + 5, r.width - 70, r.height, "طرف حساب: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n + 5, r.width - 140, r.height, strName, font, headerFontSize, "#000", "top", "right", false, false, false, true);
        if (strEconomicCode) {   // اگر شماره اقتصادی داشت نشان بده
            if (pageSize.name === "A4portrait" || pageSize.name === "A4landscape") {
                gr.drawText(r.left, r.top + n + 5, r.width / 2 - 90, r.height, "شماره اقتصادی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
                gr.drawText(r.left, r.top + n + 5, r.width / 2 - 190, r.height, strEconomicCode, font, headerFontSize, "#000", "top", "right", false, false, false, true);
            }
            else if (pageSize.name === "A5") {
                gr.drawText(r.left, r.top + n + 5, r.width / 2 - 70, r.height, "شماره اقتصادی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
                gr.drawText(r.left, r.top + n + 5, r.width / 2 - 170, r.height, strEconomicCode, font, headerFontSize, "#000", "top", "right", false, false, false, true);
            }
        }

   
            gr.drawText(r.left, r.top + n + 35, r.width - 70, r.height, "نشانی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);

        gr.drawText(r.left, r.top + n + 35, r.width - 140, r.height, strAddress, font, addressFont, "#000", "top", "right", false, false, false, true);

        n += 95;
    }
    // customer info box (if sale invoice)
    var strContactCode = "";
    if (cost.Contact)
        strContactCode = cost.Contact.EconomicCode ? cost.Contact.EconomicCode : cost.Contact.NationalCode;
 
        gr.drawRect(r.left, r.top + n - chromeFactor, r.width, 90, "black", "1px", "solid", 5);
        gr.fillRect(r.width - 7, r.top + n + 1 - chromeFactor, 45, 88, "silver", "1px", 5);
        gr.drawTextRotated(-90, r.left, r.top + n + 14, r.width - 35, r.height, "طرف حساب", font, headerFontSize, "#000", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n + 5, r.width - 70, r.height, "طرف حساب: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n + 5, r.width - 140, r.height, cost.Contact.Name, font, headerFontSize, "#000", "top", "right", false, false, false, true);
        if (pageSize.name === "A4portrait" || pageSize.name === "A4landscape") {
            gr.drawText(r.left, r.top + n + 5, r.width / 2 - 90, r.height, "شماره اقتصادی / ملی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
            gr.drawText(r.left, r.top + n + 5, r.width / 2 - 225, r.height, strContactCode, font, headerFontSize, "#000", "top", "right", false, false, false, true);
        } else if (pageSize.name === "A5") {
            gr.drawText(r.left, r.top + n + 5, r.width / 2 - 70, r.height, "شماره اقتصادی / ملی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
            gr.drawText(r.left, r.top + n + 5, r.width / 2 - 180, r.height, strContactCode, font, headerFontSize, "#000", "top", "right", false, false, false, true);
        }

        strAddress = cost.Contact.State ? cost.Contact.State + "، " : "";
        strAddress += cost.Contact.City ? cost.Contact.City + "، " : "";
        strAddress += cost.Contact.Address ? cost.Contact.Address + "، " : "";
        strAddress += cost.Contact.PostalCode ? "  کد پستی: " + cost.Contact.PostalCode : "";
        strAddress += cost.Contact.Phone ? "  تلفن: " + cost.Contact.Phone : "";
        strAddress += cost.Contact.Mobile ? "  تلفن همراه: " + cost.Contact.Mobile : "";
        strAddress += cost.Contact.Fax ? "  فکس: " + cost.Contact.Fax : "";
        strAddress = strAddress.trim();
        strAddress = strAddress.replace(/،$/, "");// trim end char '،'
        gr.drawText(r.left, r.top + n + 35, r.width - 70, r.height, "نشانی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n + 35, r.width - 140, r.height, strAddress, font, addressFont, "#000", "top", "right", false, false, false, true);
        n += 95;
    

    // cost items (rows)
    var rowStart = n;
    if (info.itemCursor + 1 <= cost.CostItems.length) {    // اگر آیتمی باقیمانده بود جهت رسم
        // draw header
        gr.fillRect(r.left, r.top + n - chromeFactor, r.width, 50, "#dcdcdc", "1px", 0);
        gr.drawLine(r.left, r.top + n - chromeFactor, r.width + 40, r.top + n - chromeFactor, "black", "1px", "solid");
        gr.drawLine(r.left, r.top + n + 50 - chromeFactor, r.width + 40, r.top + n + 50 - chromeFactor, "black", "1px", "solid");
        //        gr.drawRect(r.left, r.top + n, r.width, 50, "black", "1px", "solid", 0);
        n += 5;
        gr.drawText(r.left, r.top + n, r.width - 10, r.height, "ردیف", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n, r.width - 130, r.height, "شــــرح", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
        if (pageSize.name === "A4portrait" || pageSize.name === "A4landscape") {
            gr.drawText(r.left + 410, r.top + n, 50, r.height, "تعداد", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
            gr.drawText(r.left + 330, r.top + n, 80, r.height, "مبلغ واحد", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
            gr.drawText(r.left + 330, r.top + n + 20, 80, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
        }
        else if (pageSize.name === "A5") {
            gr.drawText(r.left + 195, r.top + n, 50, r.height, "تعداد", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
            gr.drawText(r.left + 110, r.top + n, 80, r.height, "مبلغ واحد", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
            gr.drawText(r.left + 110, r.top + n + 20, 80, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
        }
        if (pageSize.name === "A4portrait" || pageSize.name === "A4landscape") {
            gr.drawText(r.left + 240, r.top + n, 60, r.height, "تخفیف", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
            gr.drawText(r.left + 240, r.top + n + 20, 60, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
            gr.drawText(r.left + 140, r.top + n, 60, r.height, "مالیات", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
            gr.drawText(r.left + 140, r.top + n + 20, 60, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
        }
        if (pageSize.name === "A4portrait" || pageSize.name === "A4landscape") {
            gr.drawText(r.left + 30, r.top + n, 70, r.height, "مبلغ کل", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
            gr.drawText(r.left + 30, r.top + n + 20, 70, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
        }
        else if (pageSize.name === "A5") {
            gr.drawText(r.left + 20, r.top + n, 70, r.height, "مبلغ کل", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
            gr.drawText(r.left + 20, r.top + n + 20, 70, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
        }

        n += 55;
        var items = cost.CostItems;
        // draw invoice rows (invoice Items)
        //while (true) {
        //    if (info.itemCursor + 1 > items.length) break;
        //    var isBold = false;
        //    var rowFontSize = fontSize + fontX - 2 + "pt";
        //    var item = items[info.itemCursor];
        //    var des2Rows = false;
        //    if (pageSize.name === "A4landscape") { isBold = true; rowFontSize = fontSize + fontX - 1 + "pt" }

        //    if (pageSize.name === "A5" && gr.getTextSize(item.Description, font, rowFontSize, isBold, false).width > 205) des2Rows = true;
        //    else if (pageSize.name === "A4portrait" && gr.getTextSize(item.Description, font, rowFontSize, isBold, false).width > 230) des2Rows = true;
        //    else if (pageSize.name === "A4landscape" && gr.getTextSize(item.Description, font, rowFontSize, isBold, false).width > 550) des2Rows = true;

        //    if (r.top + n + 30 + (des2Rows ? 30 : 0) >= r.height) {
        //        reachPageEnd = true; break;
        //    }
        //    info.itemCursor++;

        //    gr.drawText(r.width - 10, r.top + n, 50, r.height, item.RowNumber + 1, font, rowFontSize, "black", "top", "center", isBold, false, false, true);

        //    var desWidth = 300;
        //    if (pageSize.name === "A5") desWidth = 205;
        //    else if (pageSize.name === "A4portrait") desWidth = 230;
        //    else if (pageSize.name === "A4landscape") desWidth = 550;
        //    // شرح آیتم
        //    gr.drawText(r.width - 15 - desWidth, r.top + n, desWidth, 52, item.Description, font, rowFontSize, "black", "top", "right", isBold, false, false, true);

        //    if (pageSize.name === "A4portrait" || pageSize.name === "A4landscape") {
        //        if (invoiceSettings.showItemUnit && item.Item) {
        //            gr.drawText(r.left + 415, r.top + n - 8, 60, r.height, item.Quantity, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
        //            gr.drawText(r.left + 415, r.top + n + 8, 60, r.height, item.Item.Unit, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
        //        }
        //        else
        //            gr.drawText(r.left + 415, r.top + n, 60, r.height, item.Quantity, font, rowFontSize, "black", "top", "center", isBold, false, false, true);

        //        if (!invoiceSettings.hidePrices) {
        //            gr.drawText(r.left + 330, r.top + n, 80, r.height, Hesabfa.money(item.UnitPrice), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
        //            gr.drawText(r.left + 230, r.top + n, 80, r.height, Hesabfa.money(item.Discount), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
        //            gr.drawText(r.left + 130, r.top + n, 80, r.height, Hesabfa.money(item.Tax), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
        //        }

        //        if (!invoiceSettings.hidePrices)
        //            gr.drawText(r.left + 20, r.top + n, 80, r.height, Hesabfa.money(item.TotalAmount), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
        //    }
        //    else if (pageSize.name === "A5") {
        //        if (invoiceSettings.showItemUnit && item.Item) {
        //            gr.drawText(r.left + 200, r.top + n - 8, 60, r.height, item.Quantity, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
        //            gr.drawText(r.left + 200, r.top + n + 8, 60, r.height, item.Item.Unit, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
        //        } else
        //            gr.drawText(r.left + 200, r.top + n, 60, r.height, item.Quantity, font, rowFontSize, "black", "top", "center", isBold, false, false, true);

        //        if (!invoiceSettings.hidePrices) {
        //            gr.drawText(r.left + 110, r.top + n, 80, r.height, Hesabfa.money(item.UnitPrice), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
        //            gr.drawText(r.left + 15, r.top + n, 80, r.height, Hesabfa.money(item.TotalAmount), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
        //        }
        //    }
        //    gr.drawLine(r.left, r.top + n + 30 + (des2Rows ? 25 : 0) - chromeFactor, r.width + 40, r.top + n + 30 + (des2Rows ? 25 : 0) - chromeFactor, "black", "1px", "solid");
        //    n += 40;
        //    if (des2Rows) n += 25;
        //}

        // خطوط عمودی فاکتور
        gr.drawLine(r.width + 40, r.top + rowStart - chromeFactor, r.width + 40, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        gr.drawLine(r.width - 10, r.top + rowStart - chromeFactor, r.width - 10, r.top + n - 10 - chromeFactor, "black", "1px", "solid");

        if (pageSize.name === "A4portrait") {
            gr.drawLine(r.width - 240, r.top + rowStart - chromeFactor, r.width - 240, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
            gr.drawLine(r.width - 290, r.top + rowStart - chromeFactor, r.width - 290, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
            gr.drawLine(r.width - 390, r.top + rowStart - chromeFactor, r.width - 390, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
            gr.drawLine(r.width - 490, r.top + rowStart - chromeFactor, r.width - 490, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
            gr.drawLine(r.width - 590, r.top + rowStart - chromeFactor, r.width - 590, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        }
        else if (pageSize.name === "A4landscape") {
            gr.drawLine(r.left + 470, r.top + rowStart - chromeFactor, r.left + 470, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
            gr.drawLine(r.left + 420, r.top + rowStart - chromeFactor, r.left + 420, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
            gr.drawLine(r.left + 320, r.top + rowStart - chromeFactor, r.left + 320, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
            gr.drawLine(r.left + 220, r.top + rowStart - chromeFactor, r.left + 220, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
            gr.drawLine(r.left + 120, r.top + rowStart - chromeFactor, r.left + 120, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        }
        else if (pageSize.name === "A5") {
            gr.drawLine(r.left + 255, r.top + rowStart - chromeFactor, r.left + 255, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
            gr.drawLine(r.left + 200, r.top + rowStart - chromeFactor, r.left + 200, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
            gr.drawLine(r.left + 105, r.top + rowStart - chromeFactor, r.left + 105, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        }
        gr.drawLine(r.left, r.top + rowStart - chromeFactor, r.left, r.top + n - 10 - chromeFactor, "black", "1px", "solid");

    } // end of if items remaind

    //// summary
    //var nn = 0;
    //if (totalDiscount > 0) nn += 30;
    //if (totalTax > 0) nn += 30;
    //if (invoiceSettings.hidePrices) info.isSummaryPrinted = true;
    //if (!reachPageEnd && !info.isSummaryPrinted && info.itemCursor + 1 > invoice.InvoiceItems.length) {
    //    if (r.top + n + 60 + nn <= r.height) {

    //        if (invoiceSettings.showAmountInWords && invoiceSettings.showAmountInWords === true) {
    //            gr.drawText(r.left, r.top + n, r.width, r.height, "مبلغ کل به حروف: " + wordifyfa(invoice.Payable, 0) + " " + info.currency,
    //                font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
    //            gr.drawLine(r.width - 270, r.top + n + 25 - chromeFactor, r.width + 40, r.top + n + 25 - chromeFactor, "black", "1px", "solid");
    //            gr.drawLine(r.width - 270, r.top + n + 28 - chromeFactor, r.width + 40, r.top + n + 28 - chromeFactor, "black", "1px", "solid");
    //            n += 30;
    //        }

    //        gr.drawText(r.left + 130, r.top + n, 120, r.height, "جمع", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
    //        gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(invoice.Sum) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
    //        n += 30;
    //        if (totalDiscount > 0) {
    //            gr.drawText(r.left + 130, r.top + n, 120, r.height, "جمع تخفیف", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
    //            gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(totalDiscount) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
    //            n += 30;
    //        }
    //        if (totalTax > 0) {
    //            gr.drawText(r.left + 130, r.top + n, 120, r.height, "جمع مالیات", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
    //            gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(totalTax) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
    //            n += 30;
    //        }
    //        gr.drawLine(r.left, r.top + n - chromeFactor, r.left + 270, r.top + n - chromeFactor, "black", "1px", "solid");
    //        n += 10;
    //        gr.drawText(r.left + 130, r.top + n, 120, r.height, "مبلغ کل", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
    //        gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(invoice.Payable) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
    //        gr.drawLine(r.left, r.top + n + 25 - chromeFactor, r.left + 270, r.top + n + 25 - chromeFactor, "black", "1px", "solid");
    //        gr.drawLine(r.left, r.top + n + 28 - chromeFactor, r.left + 270, r.top + n + 28 - chromeFactor, "black", "1px", "solid");

    //        // نمایش مانده حساب مشتری
    //        if (invoiceSettings.showCustomerBalance && invoice.InvoiceType === 0 && invoice.Status >= 2 && invoice.InvoiceType !== 4) {
    //            var Balance = invoice.Contact.Liability - invoice.Contact.Credits;
    //            var BalanceType = Balance > 0 ? "(بدهکار)" : "(بستانکار)";
    //            if (Balance === 0) BalanceType = "";
    //            if (Balance < 0) Balance = Balance * -1;

    //            var PriorBalance = 0;
    //            var PriorBalanceType = "";
    //            if (invoice.Status === 3 || invoice.Paid >= invoice.Payable) {
    //                PriorBalance = Balance;
    //                PriorBalanceType = BalanceType;
    //            } else {
    //                PriorBalance = BalanceType === "(بدهکار)" || BalanceType === "" ? Balance - invoice.Payable : Balance + invoice.Payable;
    //                PriorBalanceType = PriorBalance > 0 ? BalanceType : (BalanceType === "(بدهکار)" ? "(بستانکار)" : "(بدهکار)");
    //                if (PriorBalance === 0) PriorBalanceType = "";
    //                if (PriorBalance < 0) PriorBalance = PriorBalance * -1;
    //            }

    //            var fontBalanceSize = fontSize + fontX - 1;
    //            var fontBalanceBold = true;
    //            if (pageSize.name === "A5") {
    //                fontBalanceSize = fontBalanceSize - 2;
    //                fontBalanceBold = false;
    //            }

    //            if (pageSize.name === "A5") {
    //                gr.drawText((r.width / 2) + 30, r.top + n - 35, (r.width / 2) + 10, r.height, "مانده حساب شما از قبل: " + Hesabfa.money(PriorBalance) + " " + info.currency
    //                    + " " + PriorBalanceType, font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
    //                gr.drawText((r.width / 2) + 20, r.top + n - 10, (r.width / 2) + 20, r.height, "مانده حساب شما با احتساب این فاکتور:", font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
    //                gr.drawText((r.width / 2) + 20, r.top + n + 10, (r.width / 2) + 20, r.height, Hesabfa.money(Balance) + " " + info.currency
    //                    + " " + BalanceType, font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
    //            }
    //            else {
    //                gr.drawText((r.width / 2) + 20, r.top + n - 30, (r.width / 2) + 20, r.height, "مانده حساب شما از قبل:", font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
    //                gr.drawText((r.width / 2) + 20, r.top + n - 30, (r.width / 2) + 20, r.height, Hesabfa.money(PriorBalance) + " " + info.currency
    //                    + " " + PriorBalanceType, font, fontBalanceSize + "pt", "black", "top", "left", fontBalanceBold, false, false, true);
    //                gr.drawText((r.width / 2) + 20, r.top + n, (r.width / 2) + 20, r.height, "مانده حساب شما با احتساب این فاکتور:", font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
    //                gr.drawText((r.width / 2) + 20, r.top + n, (r.width / 2) + 20, r.height, Hesabfa.money(Balance) + " " + info.currency
    //                    + " " + BalanceType, font, fontBalanceSize + "pt", "black", "top", "left", fontBalanceBold, false, false, true);
    //            }
    //        }

    //        info.isSummaryPrinted = true;
    //    } else reachPageEnd = true;
    //}

    //// transactions if exist
    //if (!reachPageEnd && info.isSummaryPrinted && info.payments && info.payments.length > 0 && !info.isTransactionsPrinted && invoiceSettings.showTransactions) {
    //    //        if (r.top + n + (info.payments.length * 25) + 80 <= r.height) {
    //    if (r.top + n + 60 <= r.height) {
    //        if (!info.paymentCursor) info.paymentCursor = 0;
    //        var rowFontSize2 = fontSize + fontX - 2 + "pt";
    //        n += 30;
    //        var start2 = r.top + n;
    //        var titleTranses = invoice.InvoiceType === 0 || invoice.InvoiceType === 3 ? "دریافت ها" : "پرداخت ها";
    //        gr.fillRect(r.width - 40, r.top + n - chromeFactor, 80, 23, "#dcdcdc", "1px", 0);
    //        gr.drawText(r.left, r.top + n + 1, r.width - 10, 25, titleTranses, font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
    //        n += 5;
    //        while (true) {
    //            if (info.paymentCursor + 1 > info.payments.length) break;
    //            if (r.top + n + 25 > r.height) {
    //                reachPageEnd = true; break;
    //            }
    //            var payment = info.payments[info.paymentCursor];
    //            n += 25;
    //            var t = Hesabfa.money(payment.Amount) + " " + info.currency;
    //            t += " در تاریخ " + Hesabfa.farsiDigit(payment.DisplayDate);
    //            if (payment.Cheque == null)
    //                t += (invoice.InvoiceType === 0 || invoice.InvoiceType === 3 ? " به" : " از") + " " + payment.DetailAccount.Name;
    //            if (payment.Cheque != null)
    //                t += " طی چک شماره " + payment.Cheque.ChequeNumber;
    //            if (payment.Reference)
    //                t += " ارجاع: " + payment.Reference;
    //            gr.drawText(r.left, r.top + n, r.width - 10, 25, (info.paymentCursor + 1), font, fontSize - 1 + "pt", "black", "top", "right", false, false, false, true);
    //            gr.drawText(r.left, r.top + n, r.width - 40, 25, t, font, fontSize - 1 + "pt", "black", "top", "right", false, false, false, true);
    //            info.paymentCursor++;
    //            gr.drawRect(r.left, r.top + n - 3 - chromeFactor, r.width + 1, 25, "black", "1px", "solid");
    //            gr.drawLine(r.width + 10, r.top + n - 3 - chromeFactor, r.width + 10, r.top + n - 3 + 25 - chromeFactor, "black", "1px", "solid");
    //        }
    //        if (!reachPageEnd && r.top + n + 25 <= r.height) {
    //            n += 25;
    //            var t5 = "کل مبلغ " + (invoice.InvoiceType === 0 || invoice.InvoiceType === 3 ? "دریافت" : "پرداخت") + " شده: ";
    //            t5 += Hesabfa.money(invoice.Paid) + " " + info.currency;
    //            t5 += "  |  مبلغ باقیمانده: " + Hesabfa.money(invoice.Rest) + " " + info.currency;
    //            gr.fillRect(r.left, r.top + n - chromeFactor, r.width + 1, 30, "#dcdcdc", "1px", 0);
    //            gr.drawText(r.left, r.top + n + 5, r.width - 10, 25, t5, font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
    //            gr.drawRect(r.left, r.top + n - chromeFactor, r.width + 1, 30, "black", "1px", "solid");
    //            info.isTransactionsPrinted = true;
    //        } else reachPageEnd = true;
    //    } else reachPageEnd = true;
    //}
    //else if (!info.payments || info.payments.length === 0 || !invoiceSettings.showTransactions) info.isTransactionsPrinted = true;

    //// footer note if exist
    //var note = invoice.Status === 0 ? invoiceSettings.footerNoteDraft : invoiceSettings.footerNote;
    //if (!reachPageEnd && info.isTransactionsPrinted && note && !info.isFooterNotePrinted) {
    //    var blockSize = gr.simulate(r.left, r.top + n, r.width, r.height, note, font, fontSize + fontX - 1 + "pt", "black", "top", "right", false, false, false, true);
    //    if (r.top + n + (blockSize.height) <= r.height) {
    //        n += 40;
    //        gr.drawText(r.left, r.top + n, r.width, r.height, note, font, fontSize + fontX - 1 + "pt", "black", "top", "right", false, false, false, true);
    //        n += blockSize.height;
    //        info.isFooterNotePrinted = true;
    //    } else reachPageEnd = true;
    //}
    //else if (!note) info.isFooterNotePrinted = true;

    //// signature place if true
    //if (invoice.InvoiceType === 4) invoiceSettings.showSignaturePlace = false;
    //if (!reachPageEnd && info.isFooterNotePrinted && !info.isSignaturePrinted && invoiceSettings.showSignaturePlace) {
    //    if (r.top + n + 60 <= r.height) {
    //        n += 70;
    //        gr.drawText(r.left + (r.width / 2), r.top + n, (r.width / 2), r.height, "مهر و امضای فروشنده:", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
    //        gr.drawText(r.left, r.top + n, (r.width / 2), r.height, "مهر و امضای خریدار:", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
    //        info.isSignaturePrinted = true;
    //    } else reachPageEnd = true;
    //}
    //else if (!invoiceSettings.showSignaturePlace) info.isSignaturePrinted = true;

    //if (info.isSummaryPrinted && info.isTransactionsPrinted && info.isFooterNotePrinted && info.isSignaturePrinted)
    //    info.done = true;

    //if (pageNumber > 1 || !info.done)
    //    gr.drawText(r.left, r.height, r.width, 35, "صفحه " + pageNumber, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);

    tempCanvas.ctx = gr.ctx;
    return tempCanvas;
};
function printInvoice(invoice, settings, totalDiscount, totalTax, payments, business, currency) {
	var pageSize = {};
	if (settings.pageSize === "A4portrait") {
		pageSize.name = "A4portrait";
		pageSize.width = 210;
		pageSize.height = 297;
	}
	else if (settings.pageSize === "A4landscape") {
		pageSize.name = "A4landscape";
		pageSize.width = 297;
		pageSize.height = 210;
	}
	else if (settings.pageSize === "A4landscape2") {
		pageSize.name = "A4landscape2";
		pageSize.width = 297;
		pageSize.height = 210;
	}
	else if (settings.pageSize === "A5") {
		pageSize.name = "A5";
		pageSize.width = 148;
		pageSize.height = 210;
	}
	else if (settings.pageSize === "8cm") {
		pageSize.name = "8cm";
		pageSize.width = 80;
		pageSize.height = 0;
	}

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	var wnd = settings.pageSize === "8cm" ? window.open("/dashboard/reports/invoice8cm.html?ver=1.2.9.5") : window.open("");
	var invoicePageCount = getInvoicePageCount(pageSize, settings, invoice.InvoiceItems.length);
	var printImg = [];
	var printDiv = [];

	var info = {
		rect: rect,
		pageSize: pageSize,
		pageNumber: 1,
		invoiceSettings: settings,
		totalDiscount: totalDiscount,
		totalTax: totalTax,
		done: false,
		itemCursor: 0,
		isSummaryPrinted: false,
		isTransactionsPrinted: false,
		payments: payments ? payments : null,
		business: business,
		currency: currency
	};

	while (!info.done) {
		if (settings.pageSize === "8cm") {
			wnd.companyTitle = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;
			wnd.moneyUnit = info.currency;
			wnd.info = info;
			wnd.invoice = invoice;
			break;
		}

		var canvas = invoiceFactory(invoice, info);
		var n = info.pageNumber - 1;

		printImg[n] = wnd.document.createElement("img");
		printImg[n].width = rect.width;
		printImg[n].height = rect.height;

		printImg[n].src = canvas.toDataURL("image/png");
		wnd.document.body.appendChild(printImg[n]);

		printDiv[n] = wnd.document.createElement("div");
		$(printDiv[n]).css("page-break-after", "always");
		wnd.document.body.appendChild(printDiv[n]);

		document.body.removeChild(canvas);
		delete canvas;
		info.pageNumber++;
	}

	wnd.document.title = "چاپ فاکتور";
	setTimeout(function () {
		wnd.companyTitle = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;
		wnd.moneyUnit = info.currency;
		wnd.info = info;
		wnd.invoice = invoice;
		wnd.print();
	}, 1000);
};
function printCost(cost, settings, payments, business, currency) {
    var pageSize = {};
    if (settings.pageSize === "A4portrait") {
        pageSize.name = "A4portrait";
        pageSize.width = 210;
        pageSize.height = 297;
    }
    else if (settings.pageSize === "A4landscape") {
        pageSize.name = "A4landscape";
        pageSize.width = 297;
        pageSize.height = 210;
    }
    else if (settings.pageSize === "A4landscape2") {
        pageSize.name = "A4landscape2";
        pageSize.width = 297;
        pageSize.height = 210;
    }
    else if (settings.pageSize === "A5") {
        pageSize.name = "A5";
        pageSize.width = 148;
        pageSize.height = 210;
    }
    else if (settings.pageSize === "8cm") {
        pageSize.name = "8cm";
        pageSize.width = 80;
        pageSize.height = 0;
    }

    var pageWidthInMM = pageSize.width;
    var pageHeightInMM = pageSize.height;
    var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

    var wnd = settings.pageSize === "8cm" ? window.open("/dashboard/reports/cost8cm.html?ver=1.2.9.5") : window.open("");
    var costPageCount = getCostPageCount(pageSize, settings, cost.CostItems.length);
    var printImg = [];
    var printDiv = [];

    var info = {
        rect: rect,
        pageSize: pageSize,
        pageNumber: 1,
        invoiceSettings: settings,
        done: false,
        itemCursor: 0,
        isSummaryPrinted: false,
        isTransactionsPrinted: false,
        payments: payments ? payments : null,
        business: business,
        currency: currency
    };

    while (!info.done) {
        if (settings.pageSize === "8cm") {
            wnd.companyTitle = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;
            wnd.moneyUnit = info.currency;
            wnd.info = info;
            wnd.invoice = invoice;
            break;
        }

        var canvas = costFactory(cost, info);
        var n = info.pageNumber - 1;

        printImg[n] = wnd.document.createElement("img");
        printImg[n].width = rect.width;
        printImg[n].height = rect.height;

        printImg[n].src = canvas.toDataURL("image/png");
        wnd.document.body.appendChild(printImg[n]);

        printDiv[n] = wnd.document.createElement("div");
        $(printDiv[n]).css("page-break-after", "always");
        wnd.document.body.appendChild(printDiv[n]);

        document.body.removeChild(canvas);
        delete canvas;
        info.pageNumber++;
    }

    wnd.document.title = "چاپ صورت هزینه";
    setTimeout(function () {
        wnd.companyTitle = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;
        wnd.moneyUnit = info.currency;
        wnd.info = info;
        wnd.invoice = invoice;
        wnd.print();
    }, 1000);
};
function generateInvoicePDF(invoice, settings, totalDiscount, totalTax, asString, doAfterRead, payments, mass, pdf, business, currency) {
	var pageSize = {};
	if (settings.pageSize === "A4portrait") {
		pageSize.name = "A4portrait";
		pageSize.width = 210;
		pageSize.height = 297;
	}
	else if (settings.pageSize === "A4landscape") {
		pageSize.name = "A4landscape";
		pageSize.width = 297;
		pageSize.height = 210;
	}
	else if (settings.pageSize === "A4landscape2") {
		pageSize.name = "A4landscape2";
		pageSize.width = 297;
		pageSize.height = 210;
	}
	else if (settings.pageSize === "A5") {
		pageSize.name = "A5";
		pageSize.width = 148;
		pageSize.height = 210;
	}

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	if (!pdf) pdf = new jsPDF("p", "mm", [pageWidthInMM, pageHeightInMM]);

	var info = {
		rect: rect,
		pageSize: pageSize,
		pageNumber: 1,
		invoiceSettings: settings,
		totalDiscount: totalDiscount,
		totalTax: totalTax,
		done: false,
		itemCursor: 0,
		isSummaryPrinted: false,
		isTransactionsPrinted: false,
		payments: payments ? payments : null,
		business: business,
		currency: currency
	};
	while (!info.done) {
		var canvas = invoiceFactory(invoice, info);
		pdf.addRawImage(canvas.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, pageWidthInMM, pageHeightInMM);
		document.body.removeChild(canvas);
		delete canvas;
		info.pageNumber++;
	}

	if (!mass) {
		if (asString && asString === true)
			pdf.getBase64data(doAfterRead);

		pdf.save("invoice.pdf");
	} else return pdf;
};

function getInvoicePrintImage_A4Landscape2(invoice, info) {
	var rect = info.rect;
	var pageSize = info.pageSize;
	var pageNumber = info.pageNumber;
	var invoiceSettings = info.invoiceSettings;
	var totalDiscount = info.totalDiscount;
	var totalTax = info.totalTax;

	var chromeFactor = 0;
	if (isChrome())
		chromeFactor = 5;

	var reachPageEnd = false;

	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = invoiceSettings.font === "" ? "iransans" : invoiceSettings.font;
	var invoiceFontSize = invoiceSettings.fontSize;
	var fontX = 0;
	if (invoiceFontSize === "Small") fontX = -1;
	else if (invoiceFontSize === "Medium") fontX = 0;
	else if (invoiceFontSize === "Large") fontX = 1;
	var fontSize = 10;

	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);

	gr.fillRect(rect.left, rect.top - chromeFactor, rect.width, rect.height, "#fff", "1px", 0);

	var topMargin = invoiceSettings.topMargin;
	var bottomMargin = invoiceSettings.bottomMargin;
	var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
	var n = 0;
	var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;

	var titleFontSize = fontSize + fontX + 3 + "pt";
	var headerFontSize = fontSize + fontX + "pt";
	var addressFont = fontSize + fontX - 1 + "pt";

	var invoiceTitle = "";
	if (invoice.InvoiceType === 0 && invoice.Status > 1)
		invoiceTitle = invoiceSettings.saleInvoiceTitle;
	else if (invoice.InvoiceType === 0 && invoice.Status <= 1)
		invoiceTitle = invoiceSettings.saleDraftInvoiceTitle;
	else if (invoice.InvoiceType === 1)
		invoiceTitle = invoiceSettings.purchaseInvoiceTitle;
	else if (invoice.InvoiceType === 4)
		invoiceTitle = "ضایعات";
	else if (invoice.InvoiceType === 2)
		invoiceTitle = "فاکتور برگشت از فروش";
	else if (invoice.InvoiceType === 3)
		invoiceTitle = "فاکتور برگشت از خرید";


	var img = document.getElementById("imgLogo");
	if (invoiceSettings.businessLogo)
		gr.drawImageElement(r.width - img.clientWidth + 40, r.top, img.clientWidth, img.clientHeight, img);

	gr.drawText(r.left, r.top, r.width, r.height, strName, font, titleFontSize, "#000", "top", "center", true, false, false, true);
	gr.drawText(r.left, r.top + 30, r.width, r.height, invoiceTitle, font, titleFontSize, "black", "top", "center", true, false, false, true);
	gr.drawText(r.left, r.top, r.width, r.height, "شماره: " + invoice.Number, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);
	gr.drawText(r.left, r.top + 25, r.width, r.height, "ارجاع: " + invoice.Reference, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);
	gr.drawText(r.left, r.top + 50, r.width, r.height, "تاریخ: " + invoice.DisplayDate, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);


	n += 75;
	//        gr.drawLine(r.left, r.top + n, r.width + 35, r.top + n, "black", "1px", "solid");

	var strAddress = "";

	// vendor info box
	var strNationalCode;
	var strEconomicCode;
	var strRegistrationNumber;
	var strPostalCode;
	if (invoice.InvoiceType !== 4 && (invoiceSettings.showVendorInfo || invoice.InvoiceType === 1 || invoice.InvoiceType === 3)) {
		if (invoice.InvoiceType === 1 || invoice.InvoiceType === 3) {
			strName = invoice.Contact.Name + "";
			strNationalCode = invoice.Contact.NationalCode;
			strEconomicCode = invoice.Contact.EconomicCode;
			strRegistrationNumber = invoice.Contact.RegistrationNumber;
			strPostalCode = invoice.Contact.PostalCode;
			strAddress = invoice.Contact.State ? invoice.Contact.State + "، " : "";
			strAddress += invoice.Contact.City ? invoice.Contact.City + "، " : "";
			strAddress += invoice.Contact.Address ? invoice.Contact.Address + "، " : "";
			//			strAddress += invoice.Contact.PostalCode ? "  کد پستی: " + invoice.Contact.PostalCode : "";
			strAddress += invoice.Contact.Phone ? "  تلفن: " + invoice.Contact.Phone : "";
			strAddress += invoice.Contact.Mobile ? "  تلفن همراه: " + invoice.Contact.Mobile : "";
			strAddress += invoice.Contact.Fax ? "  فکس: " + invoice.Contact.Fax : "";
		} else {
			strNationalCode = info.business.NationalCode;
			strEconomicCode = info.business.EconomicCode;
			strRegistrationNumber = info.business.RegistrationNumber;
			strAddress = info.business.Address;
			strPostalCode = info.business.PostalCode;
			if (info.business.PostalCode && info.business.PostalCode !== "")
				//				strAddress += info.business.PostalCode !== "" ? "  کد پستی: " + info.business.PostalCode : "";
				//			if (info.business.Phone && info.business.Phone !== "")
				strAddress += info.business.Phone !== "" ? "  تلفن: " + info.business.Phone : "";
			if (info.business.Fax && info.business.Fax !== "")
				strAddress += info.business.Fax !== "" ? "  فکس: " + info.business.Fax : "";
		}
		strAddress = strAddress.trim();
		strAddress = strAddress.replace(/،$/, "");// trim end char '،'
		gr.drawRect(r.left, r.top + n - chromeFactor, r.width, 90, "black", "1px", "solid", 5);
		gr.fillRect(r.width - 7, r.top + n + 1 - chromeFactor, 45, 88, "silver", "1px", 5);
		gr.drawTextRotated(-90, r.left, r.top + n + 14, r.width - 35, r.height, "فروشنــده", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 60, r.height, "فروشنده: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 120, r.height, strName, font, headerFontSize, "#000", "top", "right", false, false, false, true);

		// شماره اقتصادی
		gr.drawText(r.left + 550, r.top + n + 5, r.width, r.height, "شماره اقتصادی: ", font, headerFontSize, "#000", "top", "left", true, false, false, true);
		var k1 = 310;
		for (var j = 0; j < 12; j++) {
			gr.drawRect(r.left + k1, r.top + n + 5 - chromeFactor, 20, 25, "black", "1px", "solid", 0);
			var c1 = strEconomicCode.length >= j + 1 ? strEconomicCode.substr(j, 1) : "";
			gr.drawText(r.left + k1, r.top + n + 5, 20, 25, c1, font, headerFontSize, "#000", "center", "center", false, false, false, true);
			k1 = k1 + 20;
		}
		// کد پستی
		gr.drawText(r.left + 550, r.top + n + 35, r.width, r.height, "کد پستی: ", font, headerFontSize, "#000", "top", "left", true, false, false, true);
		k1 = 350;
		for (var j = 0; j < 10; j++) {
			gr.drawRect(r.left + k1, r.top + n + 35 - chromeFactor, 20, 25, "black", "1px", "solid", 0);
			var c2 = strPostalCode.length >= j + 1 ? strPostalCode.substr(j, 1) : "";
			gr.drawText(r.left + k1, r.top + n + 35, 20, 25, c2, font, headerFontSize, "#000", "center", "center", false, false, false, true);
			k1 = k1 + 20;
		}
		// شماره ثبت
		gr.drawText(r.left + 230, r.top + n + 5, r.width, r.height, "شماره ثبت: ", font, headerFontSize, "#000", "top", "left", true, false, false, true);
		k1 = 30;
		for (var j = 0; j < 10; j++) {
			gr.drawRect(r.left + k1, r.top + n + 5 - chromeFactor, 20, 25, "black", "1px", "solid", 0);
			var c3 = strRegistrationNumber.length >= j + 1 ? strRegistrationNumber.substr(j, 1) : "";
			gr.drawText(r.left + k1, r.top + n + 5, 20, 25, c3, font, headerFontSize, "#000", "center", "center", false, false, false, true);
			k1 = k1 + 20;
		}
		// شناسه ملی
		gr.drawText(r.left + 230, r.top + n + 35, r.width, r.height, "شناسه ملی: ", font, headerFontSize, "#000", "top", "left", true, false, false, true);
		k1 = 10;
		for (var j = 0; j < 11; j++) {
			gr.drawRect(r.left + k1, r.top + n + 35 - chromeFactor, 20, 25, "black", "1px", "solid", 0);
			var c4 = strNationalCode.length >= j + 1 ? strNationalCode.substr(j, 1) : "";
			gr.drawText(r.left + k1, r.top + n + 35, 20, 25, c4, font, headerFontSize, "#000", "center", "center", false, false, false, true);
			k1 = k1 + 20;
		}

		gr.drawText(r.left, r.top + n + 35, r.width - 60, r.height, "استان: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		var strState = (invoice.InvoiceType === 1 || invoice.InvoiceType === 3) ? invoice.Contact.State : info.business.State;
		gr.drawText(r.left, r.top + n + 35, r.width - 120, r.height, strState, font, addressFont, "#000", "top", "right", false, false, false, true);
		gr.drawText(r.left, r.top + n + 35, r.width - 245, r.height, "شهرستان: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		var strCity = (invoice.InvoiceType === 1 || invoice.InvoiceType === 3) ? invoice.Contact.City : info.business.City;
		gr.drawText(r.left, r.top + n + 35, r.width - 320, r.height, strCity, font, addressFont, "#000", "top", "right", false, false, false, true);

		if (invoice.InvoiceType === 1 || invoice.InvoiceType === 3)
			gr.drawText(r.left, r.top + n + 65, r.width - 60, r.height, "نشانی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		else if (info.business.Address && info.business.Address !== "")
			gr.drawText(r.left, r.top + n + 65, r.width - 60, r.height, "نشانی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 65, r.width - 120, r.height, strAddress, font, addressFont, "#000", "top", "right", false, false, false, true);

		n += 95;
	}
	// customer info box (if sale invoice)
	if (invoice.InvoiceType !== 4 && (invoice.InvoiceType === 0 || invoice.InvoiceType === 2)) {
		gr.drawRect(r.left, r.top + n - chromeFactor, r.width, 90, "black", "1px", "solid", 5);
		gr.fillRect(r.width - 7, r.top + n + 1 - chromeFactor, 45, 88, "silver", "1px", 5);
		gr.drawTextRotated(-90, r.left, r.top + n + 14, r.width - 35, r.height, "خریــــدار", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 60, r.height, "خریدار: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 120, r.height, invoice.ContactTitle, font, headerFontSize, "#000", "top", "right", false, false, false, true);

		// شماره اقتصادی
		gr.drawText(r.left + 550, r.top + n + 5, r.width, r.height, "شماره اقتصادی: ", font, headerFontSize, "#000", "top", "left", true, false, false, true);
		var k1 = 310;
		for (var j = 0; j < 12; j++) {
			gr.drawRect(r.left + k1, r.top + n + 5 - chromeFactor, 20, 25, "black", "1px", "solid", 0);
			var c1 = "";
			if (invoice.Contact.EconomicCode)
				c1 = invoice.Contact.EconomicCode.length >= j + 1 ? invoice.Contact.EconomicCode.substr(j, 1) : "";
			gr.drawText(r.left + k1, r.top + n + 5, 20, 25, c1, font, headerFontSize, "#000", "center", "center", false, false, false, true);
			k1 = k1 + 20;
		}
		// کد پستی
		gr.drawText(r.left + 550, r.top + n + 35, r.width, r.height, "کد پستی: ", font, headerFontSize, "#000", "top", "left", true, false, false, true);
		k1 = 350;
		for (var j = 0; j < 10; j++) {
			gr.drawRect(r.left + k1, r.top + n + 35 - chromeFactor, 20, 25, "black", "1px", "solid", 0);
			var c2 = "";
			if (invoice.Contact.PostalCode)
				c2 = invoice.Contact.PostalCode.length >= j + 1 ? invoice.Contact.PostalCode.substr(j, 1) : "";
			gr.drawText(r.left + k1, r.top + n + 35, 20, 25, c2, font, headerFontSize, "#000", "center", "center", false, false, false, true);
			k1 = k1 + 20;
		}
		// شماره ثبت
		gr.drawText(r.left + 230, r.top + n + 5, r.width, r.height, "شماره ثبت: ", font, headerFontSize, "#000", "top", "left", true, false, false, true);
		k1 = 30;
		for (var j = 0; j < 10; j++) {
			gr.drawRect(r.left + k1, r.top + n + 5 - chromeFactor, 20, 25, "black", "1px", "solid", 0);
			var c3 = "";
			if (invoice.Contact.RegistrationNumber)
				c3 = invoice.Contact.RegistrationNumber.length >= j + 1 ? invoice.Contact.RegistrationNumber.substr(j, 1) : "";
			gr.drawText(r.left + k1, r.top + n + 5, 20, 25, c3, font, headerFontSize, "#000", "center", "center", false, false, false, true);
			k1 = k1 + 20;
		}
		// شناسه ملی
		gr.drawText(r.left + 230, r.top + n + 35, r.width, r.height, "شناسه ملی: ", font, headerFontSize, "#000", "top", "left", true, false, false, true);
		k1 = 10;
		for (var j = 0; j < 11; j++) {
			gr.drawRect(r.left + k1, r.top + n + 35 - chromeFactor, 20, 25, "black", "1px", "solid", 0);
			var c4 = "";
			if (invoice.Contact.NationalCode)
				c4 = invoice.Contact.NationalCode.length >= j + 1 ? invoice.Contact.NationalCode.substr(j, 1) : "";
			gr.drawText(r.left + k1, r.top + n + 35, 20, 25, c4, font, headerFontSize, "#000", "center", "center", false, false, false, true);
			k1 = k1 + 20;
		}

		gr.drawText(r.left, r.top + n + 35, r.width - 60, r.height, "استان: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 35, r.width - 120, r.height, invoice.Contact.State, font, addressFont, "#000", "top", "right", false, false, false, true);
		gr.drawText(r.left, r.top + n + 35, r.width - 245, r.height, "شهرستان: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 35, r.width - 320, r.height, invoice.Contact.City, font, addressFont, "#000", "top", "right", false, false, false, true);

		strAddress = "";
		strAddress += invoice.Contact.Address ? invoice.Contact.Address + "، " : "";
		strAddress += invoice.Contact.Phone ? "  تلفن: " + invoice.Contact.Phone : "";
		strAddress += invoice.Contact.Mobile ? "  تلفن همراه: " + invoice.Contact.Mobile : "";
		strAddress += invoice.Contact.Fax ? "  فکس: " + invoice.Contact.Fax : "";
		strAddress = strAddress.trim();
		strAddress = strAddress.replace(/،$/, "");// trim end char '،'

		gr.drawText(r.left, r.top + n + 65, r.width - 60, r.height, "نشانی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 65, r.width - 120, r.height, strAddress, font, addressFont, "#000", "top", "right", false, false, false, true);
		n += 95;
	}

	// invoice items (rows)
	var rowStart = n + 30;
	if (info.itemCursor + 1 <= invoice.InvoiceItems.length) {    // اگر آیتمی باقیمانده بود جهت رسم
		gr.drawRect(r.left, r.top + n - chromeFactor, r.width, 30, "black", "1px", "solid", 0);
		gr.drawText(r.left, r.top + n + 5, r.width, r.height, "مشخصات کالا یا خدمات مورد معامله", font, fontSize + fontX - 1 + "pt", "black", "center", "center", true, false, false, true);
		n += 30;

		// draw header
		gr.fillRect(r.left, r.top + n - chromeFactor, r.width, 50, "#dcdcdc", "1px", 0);
		gr.drawLine(r.left, r.top + n - chromeFactor, r.width + 40, r.top + n - chromeFactor, "black", "1px", "solid");
		gr.drawLine(r.left, r.top + n + 50 - chromeFactor, r.width + 40, r.top + n + 50 - chromeFactor, "black", "1px", "solid");
		//        gr.drawRect(r.left, r.top + n, r.width, 50, "black", "1px", "solid", 0);
		n += 5;
		gr.drawText(r.width + 10, r.top + n, 30, r.height, "ردیف", font, fontSize + fontX - 2 + "pt", "black", "top", "center", true, false, false, true);
		gr.drawText(r.width - 40, r.top + n, 50, r.height, "کد کالا", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
		gr.drawText(r.left + 560, r.top + n, 500, r.height, "شــــرح کالا یا خدمات", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
		gr.drawText(r.left + 510, r.top + n, 75, r.height, "تعداد", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
		gr.drawText(r.left + 410, r.top + n, 120, r.height, "مبلغ واحد " + "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
		gr.drawText(r.left + 330, r.top + n, 80, r.height, "مبلغ کل " + "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
		gr.drawText(r.left + 240, r.top + n, 60, r.height, "تخفیف", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
		gr.drawText(r.left + 240, r.top + n + 20, 60, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
		gr.drawText(r.left + 130, r.top + n, 80, r.height, "مالیات و عوارض " + "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
		//		gr.drawText(r.left + 140, r.top + n + 20, 60, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
		gr.drawText(r.left + 2, r.top + n, 117, r.height, "مبلغ کل پس از تخفیف،", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
		gr.drawText(r.left + 2, r.top + n + 20, 117, r.height, " مالیات و عوارض " + "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);

		n += 55;
		var items = invoice.InvoiceItems;
		// draw invoice rows (invoice Items)
		while (true) {
			if (info.itemCursor + 1 > items.length) break;
			var isBold = false;
			var rowFontSize = fontSize + fontX - 2 + "pt";
			var item = items[info.itemCursor];
			var des2Rows = false;
			if (pageSize.name === "A4landscape") { isBold = true; rowFontSize = fontSize + fontX - 1 + "pt" }

			if (gr.getTextSize(item.Description, font, rowFontSize, isBold, false).width > 430) des2Rows = true;

			if (r.top + n + 30 + (des2Rows ? 30 : 0) >= r.height) {
				reachPageEnd = true; break;
			}
			info.itemCursor++;

			var desWidth = 430;
			// شرح آیتم
			gr.drawText(r.width, r.top + n, 50, r.height, item.RowNumber, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
			gr.drawText(r.width - 45, r.top + n, 60, r.height, item.Item ? item.Item.Code : "", font, rowFontSize, "black", "top", "center", isBold, false, false, true);
			gr.drawText(r.width - 50 - desWidth, r.top + n, desWidth, 52, item.Description, font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			if (invoiceSettings.showItemUnit && item.Item) {
				gr.drawText(r.left + 500, r.top + n - 8, 90, r.height, item.Quantity, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
				gr.drawText(r.left + 500, r.top + n + 8, 90, r.height, item.Item.Unit, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
			}
			else
				gr.drawText(r.left + 500, r.top + n, 90, r.height, item.Quantity, font, rowFontSize, "black", "top", "center", isBold, false, false, true);

			if (!invoiceSettings.hidePrices) {
				gr.drawText(r.left + 415, r.top + n, 110, r.height, Hesabfa.money(item.UnitPrice), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
				gr.drawText(r.left + 330, r.top + n, 80, r.height, Hesabfa.money(item.Sum), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
				gr.drawText(r.left + 230, r.top + n, 80, r.height, Hesabfa.money(item.Discount), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
				gr.drawText(r.left + 130, r.top + n, 80, r.height, Hesabfa.money(item.Tax), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
				gr.drawText(r.left + 20, r.top + n, 80, r.height, Hesabfa.money(item.TotalAmount), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
			}

			gr.drawLine(r.left, r.top + n + 30 + (des2Rows ? 25 : 0) - chromeFactor, r.width + 40, r.top + n + 30 + (des2Rows ? 25 : 0) - chromeFactor, "black", "1px", "solid");
			n += 40;
			if (des2Rows) n += 25;
		}

		// خطوط عمودی فاکتور
		gr.drawLine(r.width + 40, r.top + rowStart - chromeFactor, r.width + 40, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		gr.drawLine(r.width + 10, r.top + rowStart - chromeFactor, r.width + 10, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		gr.drawLine(r.width - 40, r.top + rowStart - chromeFactor, r.width - 40, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		gr.drawLine(r.left + 570, r.top + rowStart - chromeFactor, r.left + 570, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		gr.drawLine(r.left + 520, r.top + rowStart - chromeFactor, r.left + 520, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		gr.drawLine(r.left + 420, r.top + rowStart - chromeFactor, r.left + 420, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		gr.drawLine(r.left + 320, r.top + rowStart - chromeFactor, r.left + 320, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		gr.drawLine(r.left + 220, r.top + rowStart - chromeFactor, r.left + 220, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		gr.drawLine(r.left + 120, r.top + rowStart - chromeFactor, r.left + 120, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
		gr.drawLine(r.left, r.top + rowStart - chromeFactor, r.left, r.top + n - 10 - chromeFactor, "black", "1px", "solid");

	} // end of if items remaind

	// summary
	var nn = 0;
	if (totalDiscount > 0) nn += 30;
	if (totalTax > 0) nn += 30;
	if (invoiceSettings.hidePrices) info.isSummaryPrinted = true;
	if (!reachPageEnd && !info.isSummaryPrinted && info.itemCursor + 1 > invoice.InvoiceItems.length) {
		if (r.top + n + 60 + nn <= r.height) {

			if (invoiceSettings.showAmountInWords && invoiceSettings.showAmountInWords === true) {
				gr.drawText(r.left, r.top + n, r.width, r.height, "مبلغ کل به حروف: " + wordifyfa(invoice.Payable, 0) + " " + info.currency,
					font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
				gr.drawLine(r.width - 270, r.top + n + 25 - chromeFactor, r.width + 40, r.top + n + 25 - chromeFactor, "black", "1px", "solid");
				gr.drawLine(r.width - 270, r.top + n + 28 - chromeFactor, r.width + 40, r.top + n + 28 - chromeFactor, "black", "1px", "solid");
				n += 30;
			}

			gr.drawText(r.left, r.top + n, r.width, r.height, "شرایط و نحوه فروش:       نقدی          غیر نقدی",
                font, fontSize + fontX - 1 + "pt", "black", "top", "right", false, false, false, true);

			gr.drawText(r.left + 130, r.top + n, 120, r.height, "جمع", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(invoice.Sum) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
			n += 30;
			if (totalDiscount > 0) {
				gr.drawText(r.left + 130, r.top + n, 120, r.height, "جمع تخفیف", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
				gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(totalDiscount) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
				n += 30;
			}
			if (totalTax > 0) {
				gr.drawText(r.left + 130, r.top + n, 120, r.height, "جمع مالیات", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
				gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(totalTax) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
				n += 30;
			}
			gr.drawLine(r.left, r.top + n - chromeFactor, r.left + 270, r.top + n - chromeFactor, "black", "1px", "solid");
			n += 10;
			gr.drawText(r.left + 130, r.top + n, 120, r.height, "مبلغ کل", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(invoice.Payable) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
			gr.drawLine(r.left, r.top + n + 25 - chromeFactor, r.left + 270, r.top + n + 25 - chromeFactor, "black", "1px", "solid");
			gr.drawLine(r.left, r.top + n + 28 - chromeFactor, r.left + 270, r.top + n + 28 - chromeFactor, "black", "1px", "solid");

			// نمایش مانده حساب مشتری
			if (invoiceSettings.showCustomerBalance && invoice.InvoiceType === 0 && invoice.Status >= 2 && invoice.InvoiceType !== 4) {
				var Balance = invoice.Contact.Liability - invoice.Contact.Credits;
				var BalanceType = Balance > 0 ? "(بدهکار)" : "(بستانکار)";
				if (Balance === 0) BalanceType = "";
				if (Balance < 0) Balance = Balance * -1;

				var PriorBalance = 0;
				var PriorBalanceType = "";
				if (invoice.Status === 3 || invoice.Paid >= invoice.Payable) {
					PriorBalance = Balance;
					PriorBalanceType = BalanceType;
				} else {
					PriorBalance = BalanceType === "(بدهکار)" || BalanceType === "" ? Balance - invoice.Payable : Balance + invoice.Payable;
					PriorBalanceType = PriorBalance > 0 ? BalanceType : (BalanceType === "(بدهکار)" ? "(بستانکار)" : "(بدهکار)");
					if (PriorBalance === 0) PriorBalanceType = "";
					if (PriorBalance < 0) PriorBalance = PriorBalance * -1;
				}

				var fontBalanceSize = fontSize + fontX - 1;
				var fontBalanceBold = true;
				gr.drawText((r.width / 2) + 160, r.top + n - 15, (r.width / 2) - 120, r.height, "مانده حساب شما از قبل:", font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
				gr.drawText((r.width / 2) + 160, r.top + n - 15, 220, r.height, Hesabfa.money(PriorBalance) + " " + info.currency
                    + " " + PriorBalanceType, font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
				gr.drawText((r.width / 2) + 160, r.top + n + 10, (r.width / 2) - 120, r.height, "مانده حساب شما با احتساب این فاکتور:", font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
				gr.drawText((r.width / 2) + 160, r.top + n + 10, 220, r.height, Hesabfa.money(Balance) + " " + info.currency
                    + " " + BalanceType, font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
			}

			info.isSummaryPrinted = true;
		} else reachPageEnd = true;
	}

	// transactions if exist
	if (!reachPageEnd && info.isSummaryPrinted && info.payments && info.payments.length > 0 && !info.isTransactionsPrinted && invoiceSettings.showTransactions) {
		if (r.top + n + 60 <= r.height) {
			if (!info.paymentCursor) info.paymentCursor = 0;
			var rowFontSize2 = fontSize + fontX - 2 + "pt";
			n += 30;
			var start2 = r.top + n;
			var titleTranses = invoice.InvoiceType === 0 || invoice.InvoiceType === 3 ? "دریافت ها" : "پرداخت ها";
			gr.fillRect(r.width - 40, r.top + n - chromeFactor, 80, 23, "#dcdcdc", "1px", 0);
			gr.drawText(r.left, r.top + n + 1, r.width - 10, 25, titleTranses, font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			n += 5;
			while (true) {
				if (info.paymentCursor + 1 > info.payments.length) break;
				if (r.top + n + 25 > r.height) {
					reachPageEnd = true; break;
				}
				var payment = info.payments[info.paymentCursor];
				n += 25;
				var t = Hesabfa.money(payment.Amount) + " " + info.currency;
				t += " در تاریخ " + Hesabfa.farsiDigit(payment.DisplayDate);
				if (payment.Cheque == null)
					t += (invoice.InvoiceType === 0 || invoice.InvoiceType === 3 ? " به" : " از") + " " + payment.DetailAccount.Name;
				if (payment.Cheque != null)
					t += " طی چک شماره " + payment.Cheque.ChequeNumber;
				if (payment.Reference)
					t += " ارجاع: " + payment.Reference;
				gr.drawText(r.left, r.top + n, r.width - 10, 25, (info.paymentCursor + 1), font, fontSize - 1 + "pt", "black", "top", "right", false, false, false, true);
				gr.drawText(r.left, r.top + n, r.width - 40, 25, t, font, fontSize - 1 + "pt", "black", "top", "right", false, false, false, true);
				info.paymentCursor++;
				gr.drawRect(r.left, r.top + n - 3 - chromeFactor, r.width + 1, 25, "black", "1px", "solid");
				gr.drawLine(r.width + 10, r.top + n - 3 - chromeFactor, r.width + 10, r.top + n - 3 + 25 - chromeFactor, "black", "1px", "solid");
			}
			if (!reachPageEnd && r.top + n + 25 <= r.height) {
				n += 25;
				var t5 = "کل مبلغ " + (invoice.InvoiceType === 0 || invoice.InvoiceType === 3 ? "دریافت" : "پرداخت") + " شده: ";
				t5 += Hesabfa.money(invoice.Paid) + " " + info.currency;
				t5 += "  |  مبلغ باقیمانده: " + Hesabfa.money(invoice.Rest) + " " + info.currency;
				gr.fillRect(r.left, r.top + n - chromeFactor, r.width + 1, 30, "#dcdcdc", "1px", 0);
				gr.drawText(r.left, r.top + n + 5, r.width - 10, 25, t5, font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
				gr.drawRect(r.left, r.top + n - chromeFactor, r.width + 1, 30, "black", "1px", "solid");
				info.isTransactionsPrinted = true;
			} else reachPageEnd = true;
		} else reachPageEnd = true;
	}
	else if (!info.payments || info.payments.length === 0 || !invoiceSettings.showTransactions) info.isTransactionsPrinted = true;

	// footer note if exist
	var note = invoice.Status === 0 ? invoiceSettings.footerNoteDraft : invoiceSettings.footerNote;
	if (!reachPageEnd && info.isTransactionsPrinted && note && !info.isFooterNotePrinted) {
		var blockSize = gr.simulate(r.left, r.top + n, r.width, r.height, note, font, fontSize + fontX - 1 + "pt", "black", "top", "right", false, false, false, true);
		if (r.top + n + (blockSize.height) <= r.height) {
			n += 40;
			gr.drawText(r.left, r.top + n, r.width, r.height, note, font, fontSize + fontX - 1 + "pt", "black", "top", "right", false, false, false, true);
			n += blockSize.height;
			info.isFooterNotePrinted = true;
		} else reachPageEnd = true;
	}
	else if (!note) info.isFooterNotePrinted = true;

	// signature place if true
	if (invoice.InvoiceType === 4) invoiceSettings.showSignaturePlace = false;
	if (!reachPageEnd && info.isFooterNotePrinted && invoiceSettings.showSignaturePlace && !info.isSignaturePrinted) {
		if (r.top + n + 60 <= r.height) {
			n += 70;
			gr.drawText(r.left + (r.width / 2), r.top + n, (r.width / 2), r.height, "مهر و امضای فروشنده:", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
			gr.drawText(r.left, r.top + n, (r.width / 2), r.height, "مهر و امضای خریدار:", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
			info.isSignaturePrinted = true;
		} else reachPageEnd = true;
	}
	else if (!invoiceSettings.showSignaturePlace) info.isSignaturePrinted = true;

	if (info.isSummaryPrinted && info.isTransactionsPrinted && info.isFooterNotePrinted && info.isSignaturePrinted)
		info.done = true;

	if (pageNumber > 1 || !info.done)
		gr.drawText(r.left, r.height, r.width, 35, "صفحه " + pageNumber, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
};
function getCostPrintImage_A4Landscape2(cost, info) {
    var rect = info.rect;
    var pageSize = info.pageSize;
    var pageNumber = info.pageNumber;
    var costSettings = info.invoiceSettings;

    var chromeFactor = 0;
    if (isChrome())
        chromeFactor = 5;

    var reachPageEnd = false;

    var mm = rect.height / pageSize.height; // page height in mm
    var scale = 3;
    var tempCanvas = document.createElement("canvas");
    document.body.appendChild(tempCanvas);
    tempCanvas.style.display = "none";
    tempCanvas.width = rect.width * scale;
    tempCanvas.height = rect.height * scale;

    var font = invoiceSettings.font === "" ? "iransans" : invoiceSettings.font;
    var costFontSize = costSettings.fontSize;
    var fontX = 0;
    if (costFontSize === "Small") fontX = -1;
    else if (costFontSize === "Medium") fontX = 0;
    else if (costFontSize === "Large") fontX = 1;
    var fontSize = 10;

    var gr = new graphics(tempCanvas);
    gr.scale(scale, scale);

    gr.fillRect(rect.left, rect.top - chromeFactor, rect.width, rect.height, "#fff", "1px", 0);

    var topMargin = costSettings.topMargin;
    var bottomMargin = costSettings.bottomMargin;
    var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
    var n = 0;
    var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;

    var titleFontSize = fontSize + fontX + 3 + "pt";
    var headerFontSize = fontSize + fontX + "pt"; 

    var costTitle = "صورت هزینه";
   
    var img = document.getElementById("imgLogo");
    if (costSettings.businessLogo)
        gr.drawImageElement(r.width - img.clientWidth + 40, r.top, img.clientWidth, img.clientHeight, img);

    gr.drawText(r.left, r.top, r.width, r.height, strName, font, titleFontSize, "#000", "top", "center", true, false, false, true);
    gr.drawText(r.left, r.top + 30, r.width, r.height, costTitle, font, titleFontSize, "black", "top", "center", true, false, false, true);
    gr.drawText(r.left, r.top, r.width, r.height, "شماره: " + cost.Number, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);
    gr.drawText(r.left, r.top + 50, r.width, r.height, "تاریخ: " + invoice.DisplayDate, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);


    n += 75;
    //        gr.drawLine(r.left, r.top + n, r.width + 35, r.top + n, "black", "1px", "solid");

    var strAddress = "";

    // vendor info box
    var strNationalCode;
    var strEconomicCode;
    var strRegistrationNumber;
    var strPostalCode;
    if  (costSettings.showVendorInfo ) {
      
            strName = invoice.Contact.Name + "";
            strNationalCode = invoice.Contact.NationalCode;
            strEconomicCode = invoice.Contact.EconomicCode;
            strRegistrationNumber = invoice.Contact.RegistrationNumber;
            strPostalCode = invoice.Contact.PostalCode;
            strAddress = invoice.Contact.State ? invoice.Contact.State + "، " : "";
            strAddress += invoice.Contact.City ? invoice.Contact.City + "، " : "";
            strAddress += invoice.Contact.Address ? invoice.Contact.Address + "، " : "";
            //			strAddress += invoice.Contact.PostalCode ? "  کد پستی: " + invoice.Contact.PostalCode : "";
            strAddress += invoice.Contact.Phone ? "  تلفن: " + invoice.Contact.Phone : "";
            strAddress += invoice.Contact.Mobile ? "  تلفن همراه: " + invoice.Contact.Mobile : "";
            strAddress += invoice.Contact.Fax ? "  فکس: " + invoice.Contact.Fax : "";

        strAddress = strAddress.trim();
        strAddress = strAddress.replace(/،$/, "");// trim end char '،'
        gr.drawRect(r.left, r.top + n - chromeFactor, r.width, 90, "black", "1px", "solid", 5);
        gr.fillRect(r.width - 7, r.top + n + 1 - chromeFactor, 45, 88, "silver", "1px", 5);
        gr.drawTextRotated(-90, r.left, r.top + n + 14, r.width - 35, r.height, "طرف حساب", font, headerFontSize, "#000", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n + 5, r.width - 60, r.height, "طرف حساب: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
        gr.drawText(r.left, r.top + n + 5, r.width - 120, r.height, strName, font, headerFontSize, "#000", "top", "right", false, false, false, true);

        // شماره اقتصادی
        gr.drawText(r.left + 550, r.top + n + 5, r.width, r.height, "شماره اقتصادی: ", font, headerFontSize, "#000", "top", "left", true, false, false, true);
        var k1 = 310;
        for (var j = 0; j < 12; j++) {
            gr.drawRect(r.left + k1, r.top + n + 5 - chromeFactor, 20, 25, "black", "1px", "solid", 0);
            var c1 = strEconomicCode.length >= j + 1 ? strEconomicCode.substr(j, 1) : "";
            gr.drawText(r.left + k1, r.top + n + 5, 20, 25, c1, font, headerFontSize, "#000", "center", "center", false, false, false, true);
            k1 = k1 + 20;
        }
        // کد پستی
        gr.drawText(r.left + 550, r.top + n + 35, r.width, r.height, "کد پستی: ", font, headerFontSize, "#000", "top", "left", true, false, false, true);
        k1 = 350;
        for (var j = 0; j < 10; j++) {
            gr.drawRect(r.left + k1, r.top + n + 35 - chromeFactor, 20, 25, "black", "1px", "solid", 0);
            var c2 = strPostalCode.length >= j + 1 ? strPostalCode.substr(j, 1) : "";
            gr.drawText(r.left + k1, r.top + n + 35, 20, 25, c2, font, headerFontSize, "#000", "center", "center", false, false, false, true);
            k1 = k1 + 20;
        }
        // شماره ثبت
        gr.drawText(r.left + 230, r.top + n + 5, r.width, r.height, "شماره ثبت: ", font, headerFontSize, "#000", "top", "left", true, false, false, true);
        k1 = 30;
        for (var j = 0; j < 10; j++) {
            gr.drawRect(r.left + k1, r.top + n + 5 - chromeFactor, 20, 25, "black", "1px", "solid", 0);
            var c3 = strRegistrationNumber.length >= j + 1 ? strRegistrationNumber.substr(j, 1) : "";
            gr.drawText(r.left + k1, r.top + n + 5, 20, 25, c3, font, headerFontSize, "#000", "center", "center", false, false, false, true);
            k1 = k1 + 20;
        }
        // شناسه ملی
        gr.drawText(r.left + 230, r.top + n + 35, r.width, r.height, "شناسه ملی: ", font, headerFontSize, "#000", "top", "left", true, false, false, true);
        k1 = 10;
        for (var j = 0; j < 11; j++) {
            gr.drawRect(r.left + k1, r.top + n + 35 - chromeFactor, 20, 25, "black", "1px", "solid", 0);
            var c4 = strNationalCode.length >= j + 1 ? strNationalCode.substr(j, 1) : "";
            gr.drawText(r.left + k1, r.top + n + 35, 20, 25, c4, font, headerFontSize, "#000", "center", "center", false, false, false, true);
            k1 = k1 + 20;
        }

        gr.drawText(r.left, r.top + n + 35, r.width - 60, r.height, "استان: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
        var strState = cost.Contact.State ;
        gr.drawText(r.left, r.top + n + 35, r.width - 120, r.height, strState, font, addressFont, "#000", "top", "right", false, false, false, true);
        gr.drawText(r.left, r.top + n + 35, r.width - 245, r.height, "شهرستان: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);
        var strCity = invoice.Contact.City ;
        gr.drawText(r.left, r.top + n + 35, r.width - 320, r.height, strCity, font, addressFont, "#000", "top", "right", false, false, false, true);

        gr.drawText(r.left, r.top + n + 65, r.width - 60, r.height, "نشانی: ", font, headerFontSize, "#000", "top", "right", true, false, false, true);

        gr.drawText(r.left, r.top + n + 65, r.width - 120, r.height, strAddress, font, addressFont, "#000", "top", "right", false, false, false, true);

        n += 95;
    }

    // cost items (rows)
    var rowStart = n + 30;
    if (info.itemCursor + 1 <= cost.CostItems.length) {    // اگر آیتمی باقیمانده بود جهت رسم
        gr.drawRect(r.left, r.top + n - chromeFactor, r.width, 30, "black", "1px", "solid", 0);
        gr.drawText(r.left, r.top + n + 5, r.width, r.height, "مشخصات هزینه ", font, fontSize + fontX - 1 + "pt", "black", "center", "center", true, false, false, true);
        n += 30;

        // draw header
        gr.fillRect(r.left, r.top + n - chromeFactor, r.width, 50, "#dcdcdc", "1px", 0);
        gr.drawLine(r.left, r.top + n - chromeFactor, r.width + 40, r.top + n - chromeFactor, "black", "1px", "solid");
        gr.drawLine(r.left, r.top + n + 50 - chromeFactor, r.width + 40, r.top + n + 50 - chromeFactor, "black", "1px", "solid");
        //        gr.drawRect(r.left, r.top + n, r.width, 50, "black", "1px", "solid", 0);
        n += 5;
        gr.drawText(r.width + 10, r.top + n, 30, r.height, "ردیف", font, fontSize + fontX - 2 + "pt", "black", "top", "center", true, false, false, true);
        //gr.drawText(r.width - 40, r.top + n, 50, r.height, "کد کالا", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
        gr.drawText(r.left + 560, r.top + n, 500, r.height, "عنوان", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
        //gr.drawText(r.left + 510, r.top + n, 75, r.height, "تعداد", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
        gr.drawText(r.left + 410, r.top + n, 120, r.height, "مبلغ  " + "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
        gr.drawText(r.left + 330, r.top + n, 80, r.height, "باقیمانده " + "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true);
        gr.drawText(r.left + 560, r.top + n, 500, r.height, "شرح", font, fontSize + fontX - 1 + "pt", "black", "top", "center", true, false, false, true); 

        //gr.drawText(r.left + 240, r.top + n + 20, 60, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);

        //		gr.drawText(r.left + 140, r.top + n + 20, 60, r.height, "(" + info.currency + ")", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);

        n += 55;
        var items = cost.CostItems;
        // draw invoice rows (invoice Items)
        while (true) {
            if (info.itemCursor + 1 > items.length) break;
            var isBold = false;
            var rowFontSize = fontSize + fontX - 2 + "pt";
            var item = items[info.itemCursor];
            var des2Rows = false;
            if (pageSize.name === "A4landscape") { isBold = true; rowFontSize = fontSize + fontX - 1 + "pt" }

            if (gr.getTextSize(item.Description, font, rowFontSize, isBold, false).width > 430) des2Rows = true;

            if (r.top + n + 30 + (des2Rows ? 30 : 0) >= r.height) {
                reachPageEnd = true; break;
            }
            info.itemCursor++;

            var desWidth = 430;
            // شرح آیتم
            gr.drawText(r.width, r.top + n, 50, r.height, item.RowNumber, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
            //gr.drawText(r.width - 45, r.top + n, 60, r.height, item.Item ? item.Item.Code : "", font, rowFontSize, "black", "top", "center", isBold, false, false, true);
            gr.drawText(r.width - 50 - desWidth, r.top + n, desWidth, 52, item.Title, font, rowFontSize, "black", "top", "right", isBold, false, false, true);
            gr.drawText(r.left + 330, r.top + n, 80, r.height, Hesabfa.money(item.Sum), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
            gr.drawText(r.left + 230, r.top + n, 80, r.height, Hesabfa.money(item.Rest), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
            gr.drawText(r.width - 50 - desWidth, r.top + n, desWidth, 52, item.Description, font, rowFontSize, "black", "top", "right", isBold, false, false, true);

            gr.drawLine(r.left, r.top + n + 30 + (des2Rows ? 25 : 0) - chromeFactor, r.width + 40, r.top + n + 30 + (des2Rows ? 25 : 0) - chromeFactor, "black", "1px", "solid");
            n += 40;
            if (des2Rows) n += 25;
        }

        // خطوط عمودی فاکتور
        gr.drawLine(r.width + 40, r.top + rowStart - chromeFactor, r.width + 40, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        gr.drawLine(r.width + 10, r.top + rowStart - chromeFactor, r.width + 10, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        gr.drawLine(r.width - 40, r.top + rowStart - chromeFactor, r.width - 40, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        gr.drawLine(r.left + 570, r.top + rowStart - chromeFactor, r.left + 570, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        gr.drawLine(r.left + 520, r.top + rowStart - chromeFactor, r.left + 520, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        gr.drawLine(r.left + 420, r.top + rowStart - chromeFactor, r.left + 420, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        gr.drawLine(r.left + 320, r.top + rowStart - chromeFactor, r.left + 320, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        gr.drawLine(r.left + 220, r.top + rowStart - chromeFactor, r.left + 220, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        gr.drawLine(r.left + 120, r.top + rowStart - chromeFactor, r.left + 120, r.top + n - 10 - chromeFactor, "black", "1px", "solid");
        gr.drawLine(r.left, r.top + rowStart - chromeFactor, r.left, r.top + n - 10 - chromeFactor, "black", "1px", "solid");

    } // end of if items remaind

    // summary
    var nn = 0;
    if (totalDiscount > 0) nn += 30;
    if (totalTax > 0) nn += 30;
    if (invoiceSettings.hidePrices) info.isSummaryPrinted = true;
    if (!reachPageEnd && !info.isSummaryPrinted && info.itemCursor + 1 > invoice.InvoiceItems.length) {
        if (r.top + n + 60 + nn <= r.height) {

            if (invoiceSettings.showAmountInWords && invoiceSettings.showAmountInWords === true) {
                gr.drawText(r.left, r.top + n, r.width, r.height, "مبلغ کل به حروف: " + wordifyfa(invoice.Payable, 0) + " " + info.currency,
                    font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
                gr.drawLine(r.width - 270, r.top + n + 25 - chromeFactor, r.width + 40, r.top + n + 25 - chromeFactor, "black", "1px", "solid");
                gr.drawLine(r.width - 270, r.top + n + 28 - chromeFactor, r.width + 40, r.top + n + 28 - chromeFactor, "black", "1px", "solid");
                n += 30;
            }

            gr.drawText(r.left, r.top + n, r.width, r.height, "شرایط و نحوه فروش:       نقدی          غیر نقدی",
                font, fontSize + fontX - 1 + "pt", "black", "top", "right", false, false, false, true);

            gr.drawText(r.left + 130, r.top + n, 120, r.height, "جمع", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
            gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(invoice.Sum) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
            n += 30;
            if (totalDiscount > 0) {
                gr.drawText(r.left + 130, r.top + n, 120, r.height, "جمع تخفیف", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
                gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(totalDiscount) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
                n += 30;
            }
            if (totalTax > 0) {
                gr.drawText(r.left + 130, r.top + n, 120, r.height, "جمع مالیات", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
                gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(totalTax) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
                n += 30;
            }
            gr.drawLine(r.left, r.top + n - chromeFactor, r.left + 270, r.top + n - chromeFactor, "black", "1px", "solid");
            n += 10;
            gr.drawText(r.left + 130, r.top + n, 120, r.height, "مبلغ کل", font, fontSize + fontX - 1 + "pt", "black", "top", "right", true, false, false, true);
            gr.drawText(r.left + 10, r.top + n, 150, r.height, Hesabfa.money(invoice.Payable) + " " + info.currency, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);
            gr.drawLine(r.left, r.top + n + 25 - chromeFactor, r.left + 270, r.top + n + 25 - chromeFactor, "black", "1px", "solid");
            gr.drawLine(r.left, r.top + n + 28 - chromeFactor, r.left + 270, r.top + n + 28 - chromeFactor, "black", "1px", "solid");

            // نمایش مانده حساب مشتری
            if (invoiceSettings.showCustomerBalance && invoice.InvoiceType === 0 && invoice.Status >= 2 && invoice.InvoiceType !== 4) {
                var Balance = invoice.Contact.Liability - invoice.Contact.Credits;
                var BalanceType = Balance > 0 ? "(بدهکار)" : "(بستانکار)";
                if (Balance === 0) BalanceType = "";
                if (Balance < 0) Balance = Balance * -1;

                var PriorBalance = 0;
                var PriorBalanceType = "";
                if (invoice.Status === 3 || invoice.Paid >= invoice.Payable) {
                    PriorBalance = Balance;
                    PriorBalanceType = BalanceType;
                } else {
                    PriorBalance = BalanceType === "(بدهکار)" || BalanceType === "" ? Balance - invoice.Payable : Balance + invoice.Payable;
                    PriorBalanceType = PriorBalance > 0 ? BalanceType : (BalanceType === "(بدهکار)" ? "(بستانکار)" : "(بدهکار)");
                    if (PriorBalance === 0) PriorBalanceType = "";
                    if (PriorBalance < 0) PriorBalance = PriorBalance * -1;
                }

                var fontBalanceSize = fontSize + fontX - 1;
                var fontBalanceBold = true;
                gr.drawText((r.width / 2) + 160, r.top + n - 15, (r.width / 2) - 120, r.height, "مانده حساب شما از قبل:", font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
                gr.drawText((r.width / 2) + 160, r.top + n - 15, 220, r.height, Hesabfa.money(PriorBalance) + " " + info.currency
                    + " " + PriorBalanceType, font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
                gr.drawText((r.width / 2) + 160, r.top + n + 10, (r.width / 2) - 120, r.height, "مانده حساب شما با احتساب این فاکتور:", font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
                gr.drawText((r.width / 2) + 160, r.top + n + 10, 220, r.height, Hesabfa.money(Balance) + " " + info.currency
                    + " " + BalanceType, font, fontBalanceSize + "pt", "black", "top", "right", fontBalanceBold, false, false, true);
            }

            info.isSummaryPrinted = true;
        } else reachPageEnd = true;
    }

    // transactions if exist
    if (!reachPageEnd && info.isSummaryPrinted && info.payments && info.payments.length > 0 && !info.isTransactionsPrinted && invoiceSettings.showTransactions) {
        if (r.top + n + 60 <= r.height) {
            if (!info.paymentCursor) info.paymentCursor = 0;
            var rowFontSize2 = fontSize + fontX - 2 + "pt";
            n += 30;
            var start2 = r.top + n;
            var titleTranses = invoice.InvoiceType === 0 || invoice.InvoiceType === 3 ? "دریافت ها" : "پرداخت ها";
            gr.fillRect(r.width - 40, r.top + n - chromeFactor, 80, 23, "#dcdcdc", "1px", 0);
            gr.drawText(r.left, r.top + n + 1, r.width - 10, 25, titleTranses, font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
            n += 5;
            while (true) {
                if (info.paymentCursor + 1 > info.payments.length) break;
                if (r.top + n + 25 > r.height) {
                    reachPageEnd = true; break;
                }
                var payment = info.payments[info.paymentCursor];
                n += 25;
                var t = Hesabfa.money(payment.Amount) + " " + info.currency;
                t += " در تاریخ " + Hesabfa.farsiDigit(payment.DisplayDate);
                if (payment.Cheque == null)
                    t += (invoice.InvoiceType === 0 || invoice.InvoiceType === 3 ? " به" : " از") + " " + payment.DetailAccount.Name;
                if (payment.Cheque != null)
                    t += " طی چک شماره " + payment.Cheque.ChequeNumber;
                if (payment.Reference)
                    t += " ارجاع: " + payment.Reference;
                gr.drawText(r.left, r.top + n, r.width - 10, 25, (info.paymentCursor + 1), font, fontSize - 1 + "pt", "black", "top", "right", false, false, false, true);
                gr.drawText(r.left, r.top + n, r.width - 40, 25, t, font, fontSize - 1 + "pt", "black", "top", "right", false, false, false, true);
                info.paymentCursor++;
                gr.drawRect(r.left, r.top + n - 3 - chromeFactor, r.width + 1, 25, "black", "1px", "solid");
                gr.drawLine(r.width + 10, r.top + n - 3 - chromeFactor, r.width + 10, r.top + n - 3 + 25 - chromeFactor, "black", "1px", "solid");
            }
            if (!reachPageEnd && r.top + n + 25 <= r.height) {
                n += 25;
                var t5 = "کل مبلغ " + (invoice.InvoiceType === 0 || invoice.InvoiceType === 3 ? "دریافت" : "پرداخت") + " شده: ";
                t5 += Hesabfa.money(invoice.Paid) + " " + info.currency;
                t5 += "  |  مبلغ باقیمانده: " + Hesabfa.money(invoice.Rest) + " " + info.currency;
                gr.fillRect(r.left, r.top + n - chromeFactor, r.width + 1, 30, "#dcdcdc", "1px", 0);
                gr.drawText(r.left, r.top + n + 5, r.width - 10, 25, t5, font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
                gr.drawRect(r.left, r.top + n - chromeFactor, r.width + 1, 30, "black", "1px", "solid");
                info.isTransactionsPrinted = true;
            } else reachPageEnd = true;
        } else reachPageEnd = true;
    }
    else if (!info.payments || info.payments.length === 0 || !invoiceSettings.showTransactions) info.isTransactionsPrinted = true;

    // footer note if exist
    var note = invoice.Status === 0 ? invoiceSettings.footerNoteDraft : invoiceSettings.footerNote;
    if (!reachPageEnd && info.isTransactionsPrinted && note && !info.isFooterNotePrinted) {
        var blockSize = gr.simulate(r.left, r.top + n, r.width, r.height, note, font, fontSize + fontX - 1 + "pt", "black", "top", "right", false, false, false, true);
        if (r.top + n + (blockSize.height) <= r.height) {
            n += 40;
            gr.drawText(r.left, r.top + n, r.width, r.height, note, font, fontSize + fontX - 1 + "pt", "black", "top", "right", false, false, false, true);
            n += blockSize.height;
            info.isFooterNotePrinted = true;
        } else reachPageEnd = true;
    }
    else if (!note) info.isFooterNotePrinted = true;

    // signature place if true
    if (invoice.InvoiceType === 4) invoiceSettings.showSignaturePlace = false;
    if (!reachPageEnd && info.isFooterNotePrinted && invoiceSettings.showSignaturePlace && !info.isSignaturePrinted) {
        if (r.top + n + 60 <= r.height) {
            n += 70;
            gr.drawText(r.left + (r.width / 2), r.top + n, (r.width / 2), r.height, "مهر و امضای فروشنده:", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
            gr.drawText(r.left, r.top + n, (r.width / 2), r.height, "مهر و امضای خریدار:", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
            info.isSignaturePrinted = true;
        } else reachPageEnd = true;
    }
    else if (!invoiceSettings.showSignaturePlace) info.isSignaturePrinted = true;

    if (info.isSummaryPrinted && info.isTransactionsPrinted && info.isFooterNotePrinted && info.isSignaturePrinted)
        info.done = true;

    if (pageNumber > 1 || !info.done)
        gr.drawText(r.left, r.height, r.width, 35, "صفحه " + pageNumber, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);

    tempCanvas.ctx = gr.ctx;
    return tempCanvas;
};
// صورتحساب معین شخص
function getContactStatementPrintImage(transactions, info) {
	//    debugger;
	var rect = info.rect;
	var pageSize = info.pageSize;
	var pageNumber = info.pageNumber;

	var sumDebit = info.sumDebit;
	var sumCredit = info.sumCredit;
	var balance = info.balance;
	var balanceType = info.balanceType;

	var reachPageEnd = false;

	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = "iransans";
	var fontSize = 10;

	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);
	gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);

	var topMargin = 10;
	var bottomMargin = 10;
	var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
	var n = 0;
	var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;
	var rptTitle = "صورتحساب " + info.contact.Name + " (" + info.contact.Code + ")";
	var rptTitlePlus = "";
	if (info.date1 && info.date2)
		rptTitlePlus += " از تاریخ " + info.date1 + " تا " + info.date2;
	else if (!info.date1 && info.date2)
		rptTitlePlus += " تا تاریخ " + info.date2;

	var titleFontSize = fontSize + 2 + "pt";

	gr.drawText(r.left, r.top, r.width, r.height, strName, font, titleFontSize, "#000", "top", "center", true, false, false, true);
	if (info.date2) {
		gr.drawText(r.left, r.top + 30, r.width, r.height, rptTitle, font, titleFontSize, "gray", "top", "center", true, false, false, true);
		gr.drawText(r.left, r.top + 60, r.width, r.height, rptTitlePlus, font, fontSize + "pt", "gray", "top", "center", true, false, false, true);
		n += 20;
	}
	else gr.drawText(r.left, r.top + 30, r.width, r.height, rptTitle + rptTitlePlus, font, titleFontSize, "gray", "top", "center", true, false, false, true);
	gr.drawText(r.left, r.top + 25, r.width, r.height, info.currentDate, font, fontSize - 1 + "pt", "#000", "top", "left", false, false, false, true);

	// table
	n += 75;

	gr.fillRect(r.left, r.top + n, r.width, 50, "#dcdcdc", "1px", 0);
	gr.drawLine(r.left, r.top + n, r.width + 40, r.top + n, "black", "1px", "solid");
	gr.drawLine(r.left, r.top + n + 50, r.width + 40, r.top + n + 50, "black", "1px", "solid");
	//        gr.drawRect(r.left, r.top + n, r.width, 50, "black", "1px", "solid", 0);
	var rowStart = n;
	n += 5;
	// عنوان ستون ها
	if (info.itemCursor + 1 <= transactions.length) {    // اگر آیتمی باقیمانده بود جهت رسم
		gr.drawText(r.left, r.top + n, r.width - 10, r.height, "ردیف", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 60, r.height, "تاریخ ", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 130, r.height, "سند", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 250, r.height, "شرح", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 410, r.height, "بدهکار " + "(" + info.currency + ")", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 510, r.height, "بستانکار " + "(" + info.currency + ")", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawTextRotated(-90, r.left + 75, r.top + n - 8, 50, 250, "تشخیص", font, fontSize - 1 + "pt", "#000", "top", "left", false, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 640, r.height, "باقیمانده " + "(" + info.currency + ")", font, fontSize + "pt", "black", "top", "right", true, false, false, true);

		n += 55;
		var items = transactions;

		while (true) {
			if (info.itemCursor + 1 > items.length) break;
			var trans = items[info.itemCursor];
			var isBold = false;
			var rowFontSize = fontSize - 1 + "pt";
			if (pageSize.name === "A4landscape") { isBold = true; rowFontSize = fontSize - 1 + "pt" }
			var desRows = Math.ceil(gr.getTextSize(trans.Transaction.Description, font, rowFontSize, isBold, false).width / 223);

			if (r.top + n + (desRows * 30) >= r.height) {
				reachPageEnd = true; break;
			}
			info.itemCursor++;

			// فیلد ها
			gr.drawText(r.width - 10, r.top + n, 50, r.height, info.itemCursor, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 60, r.height, Hesabfa.farsiDigit(trans.Transaction.AccDocument.DisplayDate), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			if (trans.Transaction.AccDocument.Number !== 0)
				gr.drawText(r.width - 147, r.top + n, 82, 20, Hesabfa.farsiDigit(trans.Transaction.AccDocument.Number), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
			gr.drawText(r.width - 358, r.top + n, 223, 500, trans.Transaction.Description, font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 410, r.height, Hesabfa.money(trans.Debit), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 510, r.height, Hesabfa.money(trans.Credit), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.left + 115, r.top + n, 40, r.height, trans.RemainTypeString, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
			if (trans.Transaction.AccDocument.Number !== 0 || info.itemCursor === 1)
				gr.drawText(r.left + 10, r.top + n, r.left + 60, r.height, Hesabfa.money(trans.Remain), font, rowFontSize, "black", "top", "right", isBold, false, false, true);

			gr.drawLine(r.left, r.top + n + (desRows * 30), r.width + 40, r.top + n + (desRows * 30), "black", "1px", "solid");
			n += 10 + (desRows * 30);
		}

		// خطوط عمودی گزارش
		gr.drawLine(r.width + 40, r.top + rowStart, r.width + 40, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 10, r.top + rowStart, r.width - 10, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 85, r.top + rowStart, r.width - 85, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 130, r.top + rowStart, r.width - 130, r.top + n - 10, "black", "1px", "solid"); //سند
		gr.drawLine(r.width - 360, r.top + rowStart, r.width - 360, r.top + n - 10, "black", "1px", "solid"); // شرح
		gr.drawLine(r.width - 460, r.top + rowStart, r.width - 460, r.top + n - 10, "black", "1px", "solid"); // بدهكار
		gr.drawLine(r.width - 560, r.top + rowStart, r.width - 560, r.top + n - 10, "black", "1px", "solid"); // تشخيص
		gr.drawLine(r.width - 590, r.top + rowStart, r.width - 590, r.top + n - 10, "black", "1px", "solid");

		gr.drawLine(r.left, r.top + rowStart, r.left, r.top + n - 10, "black", "1px", "solid");
	} // end of if items remaind

	// summary
	if (!reachPageEnd && !info.isSummaryPrinted && info.itemCursor + 1 > transactions.length) {
		if (r.top + n + 30 <= r.height) {
			gr.fillRect(r.left, r.top + n - 9, r.width, 39, "#dcdcdc", "1px", 0);

			// فیلد ها
			gr.drawText(r.width - 10, r.top + n, 50, r.height, "جمع", font, fontSize + "pt", "black", "top", "center", true, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 410, r.height, Hesabfa.money(sumDebit), font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 510, r.height, Hesabfa.money(sumCredit), font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			var tashkhisStr = "";
			if (balanceType === "debit") tashkhisStr = "بد";
			else if (balanceType === "credit") tashkhisStr = "بس";
			else if (balanceType === "none") tashkhisStr = "-";
			gr.drawText(r.left + 115, r.top + n, 40, r.height, tashkhisStr, font, fontSize + "pt", "black", "top", "center", true, false, false, true);
			gr.drawText(r.left + 10, r.top + n, r.left + 60, r.height, Hesabfa.money(balance), font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			gr.drawLine(r.left, r.top + n + 30, r.width + 40, r.top + n + 30, "black", "1px", "solid");

			// خطوط عمودی سطر جمع
			n += 40;
			gr.drawLine(r.width + 40, r.top + rowStart, r.width + 40, r.top + n - 10, "black", "1px", "solid");
			gr.drawLine(r.width - 360, r.top + rowStart, r.width - 360, r.top + n - 10, "black", "1px", "solid"); // شرح
			gr.drawLine(r.width - 460, r.top + rowStart, r.width - 460, r.top + n - 10, "black", "1px", "solid"); // بدهكار
			gr.drawLine(r.width - 560, r.top + rowStart, r.width - 560, r.top + n - 10, "black", "1px", "solid"); // تشخيص
			gr.drawLine(r.width - 590, r.top + rowStart, r.width - 590, r.top + n - 10, "black", "1px", "solid");
			gr.drawLine(r.left, r.top + rowStart, r.left, r.top + n - 10, "black", "1px", "solid");

			info.isSummaryPrinted = true;
		} else reachPageEnd = true;
	}

	if (info.isSummaryPrinted) info.done = true;

	// حساب فا
	if (n <= r.height - 10)
		gr.drawText(r.left, r.height - 10, r.width, r.height, "حساب فا - www.hesabfa.com", font, "10pt", "silver", "top", "center", true, false, false, true);

	if (pageNumber > 1 || !info.done)
		gr.drawText(r.left, r.height, r.width, 35, "صفحه " + pageNumber, font, fontSize + "pt", "black", "top", "left", true, false, false, true);

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
}
function printContactStatement(transactions, sumDebit, sumCredit, balance, balanceType, contact, date1, date2, business, currency, currentDate) {
	var pageSize = {};
	pageSize.name = "A4portrait";
	pageSize.width = 210;
	pageSize.height = 297;

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	var wnd = window.open("");
	var printImg = [];
	var printDiv = [];

	var info = {
		rect: rect,
		pageSize: pageSize,
		pageNumber: 1,
		done: false,
		itemCursor: 0,
		isSummaryPrinted: false,
		sumDebit: sumDebit,
		sumCredit: sumCredit,
		balance: balance,
		balanceType: balanceType,
		contact: contact,
		date1: date1,
		date2: date2,
		business: business,
		currency: currency,
		currentDate: currentDate
	};
	while (!info.done) {
		var canvas = getContactStatementPrintImage(transactions, info);
		var n = info.pageNumber - 1;

		printImg[n] = wnd.document.createElement("img");
		printImg[n].width = rect.width;
		printImg[n].height = rect.height;

		printImg[n].src = canvas.toDataURL("image/png");
		wnd.document.body.appendChild(printImg[n]);

		printDiv[n] = wnd.document.createElement("div");
		$(printDiv[n]).css("page-break-after", "always");
		wnd.document.body.appendChild(printDiv[n]);

		document.body.removeChild(canvas);
		delete canvas;
		info.pageNumber++;
	}

	wnd.document.title = "چاپ صورتحساب معین";
	setTimeout(function () {
		wnd.print();
	}, 1000);
};
function pdfContactStatement(transactions, sumDebit, sumCredit, balance, balanceType, contact, date1, date2, business, currency, currentDate) {
	var pageSize = {};
	pageSize.name = "A4portrait";
	pageSize.width = 210;
	pageSize.height = 297;

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	var pdf = new jsPDF("p", "mm", [pageWidthInMM, pageHeightInMM]);

	var info = {
		rect: rect,
		pageSize: pageSize,
		pageNumber: 1,
		done: false,
		itemCursor: 0,
		isSummaryPrinted: false,
		sumDebit: sumDebit,
		sumCredit: sumCredit,
		balance: balance,
		balanceType: balanceType,
		contact: contact,
		date1: date1,
		date2: date2,
		business: business,
		currency: currency,
		currentDate: currentDate
	};
	while (!info.done) {
		var canvas = getContactStatementPrintImage(transactions, info);
		pdf.addRawImage(canvas.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, pageWidthInMM, pageHeightInMM);
		document.body.removeChild(canvas);
		delete canvas;
		info.pageNumber++;
	}

	pdf.save("statement.pdf");
};

// اشخاص
function getContactsPrintImage(contacts, info) {
	var rect = info.rect;
	var pageSize = info.pageSize;
	var pageNumber = info.pageNumber;
	var rptTitle = info.rptTitle;

	//    var sumDebit = info.sumDebit;
	//    var sumCredit = info.sumCredit;
	//    var balance = info.balance;
	//    var balanceType = info.balanceType;

	var reachPageEnd = false;

	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = "iransans";
	var fontSize = 10;

	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);
	gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);

	var topMargin = 10;
	var bottomMargin = 10;
	var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
	var n = 0;
	var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;

	var titleFontSize = fontSize + 2 + "pt";

	gr.drawText(r.left, r.top, r.width, r.height, strName, font, titleFontSize, "#000", "top", "center", true, false, false, true);
	gr.drawText(r.left, r.top + 30, r.width, r.height, rptTitle, font, titleFontSize, "gray", "top", "center", true, false, false, true);
	gr.drawText(r.left, r.top + 25, r.width, r.height, info.todayDate, font, fontSize - 1 + "pt", "#000", "top", "left", false, false, false, true);

	// table
	n += 75;

	gr.fillRect(r.left, r.top + n, r.width, 50, "#dcdcdc", "1px", 0);
	gr.drawLine(r.left, r.top + n, r.width + 40, r.top + n, "black", "1px", "solid");
	gr.drawLine(r.left, r.top + n + 50, r.width + 40, r.top + n + 50, "black", "1px", "solid");
	//        gr.drawRect(r.left, r.top + n, r.width, 50, "black", "1px", "solid", 0);
	var rowStart = n;
	n += 5;
	// عنوان ستون ها
	if (info.itemCursor + 1 <= contacts.length) {    // اگر آیتمی باقیمانده بود جهت رسم
		gr.drawText(r.left, r.top + n, r.width - 10, r.height, "ردیف", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 60, r.height, "کد ", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 130, r.height, "نام", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 260, r.height, "شماره تماس", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 370, r.height, "بدهکاری ", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 480, r.height, "بستانکاری ", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 590, r.height, "مانده " + "(" + info.currency + ")", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 695, r.height, "تشخیص ", font, fontSize + "pt", "black", "top", "right", true, false, false, true);

		n += 55;
		var items = contacts;

		while (true) {
			if (info.itemCursor + 1 > items.length) break;
			var contact = items[info.itemCursor];
			var isBold = false;
			var rowFontSize = fontSize - 1 + "pt";
			if (pageSize.name === "A4landscape") { isBold = true; rowFontSize = fontSize - 1 + "pt" }
			var nameRow = Math.ceil(gr.getTextSize(contact.Name, font, rowFontSize, isBold, false).width / 140);

			if (r.top + n + (nameRow * 30) >= r.height - 10) {
				reachPageEnd = true; break;
			}
			info.itemCursor++;

			// فیلد ها
			var contactBalance = contact.Balance >= 0 ? contact.Balance : contact.Balance * -1;
			gr.drawText(r.width - 10, r.top + n, 50, r.height, info.itemCursor, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 60, r.height, Hesabfa.farsiDigit(contact.DetailAccount.Code), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.width - 215, r.top + n, 140, 500, contact.Name, font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.width - 330, r.top + n, 110, 500, Hesabfa.farsiDigit(contact.Phone || contact.Mobile), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 370, r.height, Hesabfa.money(contact.Liability), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 480, r.height, Hesabfa.money(contact.Credits), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 590, r.height, Hesabfa.money(contactBalance), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			var tashkhis = "-";
			if (contact.Balance > 0) tashkhis = "بدهکار";
			else if (contact.Balance < 0) tashkhis = "بستانکار";
			gr.drawText(r.left + 10, r.top + n, 40, r.height, tashkhis, font, rowFontSize, "black", "top", "center", isBold, false, false, true);

			gr.drawLine(r.left, r.top + n + (nameRow * 30), r.width + 40, r.top + n + (nameRow * 30), "black", "1px", "solid");
			n += 10 + (nameRow * 30);
		}

		// خطوط عمودی گزارش
		gr.drawLine(r.width + 40, r.top + rowStart, r.width + 40, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 10, r.top + rowStart, r.width - 10, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 65, r.top + rowStart, r.width - 65, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 210, r.top + rowStart, r.width - 210, r.top + n - 10, "black", "1px", "solid"); //نام 
		gr.drawLine(r.width - 320, r.top + rowStart, r.width - 320, r.top + n - 10, "black", "1px", "solid"); // شماره تماس
		gr.drawLine(r.width - 430, r.top + rowStart, r.width - 430, r.top + n - 10, "black", "1px", "solid"); // بدهكار
		gr.drawLine(r.width - 540, r.top + rowStart, r.width - 540, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 650, r.top + rowStart, r.width - 650, r.top + n - 10, "black", "1px", "solid");

		gr.drawLine(r.left, r.top + rowStart, r.left, r.top + n - 10, "black", "1px", "solid");
	} // end of if items remaind

	if (info.itemCursor + 1 > contacts.length) {
		info.done = true;
		var strBalance = "اشخاص";
		if (info.role === "customer") strBalance = "مشتریان";
		else if (info.role === "vendor") strBalance = "فروشندگان";
		else if (info.role === "debtor") strBalance = "بدهکاران";
		else if (info.role === "creditor") strBalance = "بستانکاران";
		gr.drawText(r.left, r.top + n, 300, r.height, "جمع مانده " + strBalance + ": " + Hesabfa.money(info.balanceSum) + " " + Hesabfa.moneyUnit + " (" + info.balanceType + ")", font, fontSize + "pt", "black", "top", "left", true, false, false, true);
	}

	// حساب فا
	if (n <= r.height - 10)
		gr.drawText(r.left, r.height - 10, r.width, r.height, "حساب فا - www.hesabfa.com", font, "10pt", "silver", "top", "center", true, false, false, true);

	if (pageNumber > 1 || !info.done)
		gr.drawText(r.left, r.height, r.width, 35, "صفحه " + pageNumber, font, fontSize + "pt", "black", "top", "left", true, false, false, true);

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
}
function printContacts(contacts, rptTitle, balanceSum, balanceType, role, business, todayDate, currency) {
	var pageSize = {};
	pageSize.name = "A4portrait";
	pageSize.width = 210;
	pageSize.height = 297;

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	var wnd = window.open("");
	var printImg = [];
	var printDiv = [];

	var info = {
		rect: rect,
		pageSize: pageSize,
		pageNumber: 1,
		done: false,
		itemCursor: 0,
		rptTitle: rptTitle,
		balanceSum: balanceSum,
		balanceType: balanceType,
		role: role,
		business: business,
		todayDate: todayDate,
		currency: currency
	};
	while (!info.done) {
		var canvas = getContactsPrintImage(contacts, info);
		var n = info.pageNumber - 1;

		printImg[n] = wnd.document.createElement("img");
		printImg[n].width = rect.width;
		printImg[n].height = rect.height;

		printImg[n].src = canvas.toDataURL("image/png");
		wnd.document.body.appendChild(printImg[n]);

		printDiv[n] = wnd.document.createElement("div");
		$(printDiv[n]).css("page-break-after", "always");
		wnd.document.body.appendChild(printDiv[n]);

		document.body.removeChild(canvas);
		delete canvas;
		info.pageNumber++;
	}

	wnd.document.title = "چاپ " + rptTitle;
	setTimeout(function () {
		wnd.print();
	}, 1000);
};
function pdfContacts(contacts, rptTitle, balanceSum, balanceType, role, business, todayDate, currency) {
	var pageSize = {};
	pageSize.name = "A4portrait";
	pageSize.width = 210;
	pageSize.height = 297;

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	var pdf = new jsPDF("p", "mm", [pageWidthInMM, pageHeightInMM]);

	var info = {
		rect: rect,
		pageSize: pageSize,
		pageNumber: 1,
		done: false,
		itemCursor: 0,
		rptTitle: rptTitle,
		balanceSum: balanceSum,
		balanceType: balanceType,
		role: role,
		business: business,
		todayDate: todayDate,
		currency: currency
	};
	while (!info.done) {
		var canvas = getContactsPrintImage(contacts, info);
		pdf.addRawImage(canvas.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, pageWidthInMM, pageHeightInMM);
		document.body.removeChild(canvas);
		delete canvas;
		info.pageNumber++;
	}

	pdf.save("contacts.pdf");
};

// سند حسابداری
function getDocumentPrintImage(doc, info) {
	//        debugger;
	var rect = info.rect;
	var pageSize = info.pageSize;
	var pageNumber = info.pageNumber;

	var sumDebit = doc.Debit;
	var sumCredit = doc.Credit;
	var transactions = doc.Transactions;

	var reachPageEnd = false;

	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = "iransans";
	var fontSize = 10;

	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);
	gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);

	var topMargin = 10;
	var bottomMargin = 10;
	var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
	var n = 0;
	var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;
	var rptTitle = "سند حسابداری";

	var titleFontSize = fontSize + 4 + "pt";

	gr.drawText(r.left, r.top, r.width, r.height, rptTitle, font, titleFontSize, "#000", "top", "center", true, false, false, true);
	gr.drawText(r.left, r.top + 35, r.width, r.height, strName, font, fontSize + "pt", "gray", "top", "center", true, false, false, true);
	gr.drawText(r.left, r.top + 10, r.width, r.height, "شماره سند: " + doc.Number, font, fontSize - 1 + "pt", "#000", "top", "right", false, false, false, true);
	if (info.docSettings.showNumber2 && info.docSettings.showNumber2 === "true")
		gr.drawText(r.left, r.top + 35, r.width, r.height, "شماره عطف: " + doc.Number2, font, fontSize - 1 + "pt", "#000", "top", "right", false, false, false, true);
	gr.drawText(r.left, r.top + 10, r.width, r.height, "تاریخ: " + doc.DisplayDate, font, fontSize - 1 + "pt", "#000", "top", "left", false, false, false, true);
	gr.drawText(r.left, r.top + 35, r.width, r.height, "پیوست: " + ".....", font, fontSize - 1 + "pt", "#000", "top", "left", false, false, false, true);

	// table
	n += 75;

	gr.fillRect(r.left, r.top + n, r.width, 50, "#dcdcdc", "1px", 0);
	gr.drawLine(r.left, r.top + n, r.width + 40, r.top + n, "black", "1px", "solid");
	gr.drawLine(r.left, r.top + n + 50, r.width + 40, r.top + n + 50, "black", "1px", "solid");
	//        gr.drawRect(r.left, r.top + n, r.width, 50, "black", "1px", "solid", 0);
	var rowStart = n;
	n += 5;
	// عنوان ستون ها
	if (info.itemCursor + 1 <= transactions.length) {    // اگر آیتمی باقیمانده بود جهت رسم
		gr.drawText(r.left, r.top + n + 5, r.width - 5, r.height, "ردیف", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 50, r.height, "کد حساب ", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 130, r.height, "نام حساب", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 330, r.height, "شـــــــــــــــرح", font, fontSize + 1 + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 520, r.height, "بدهکار " + "(" + info.currency + ")", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 5, r.width - 640, r.height, "بستانکار " + "(" + info.currency + ")", font, fontSize + "pt", "black", "top", "right", true, false, false, true);

		n += 55;
		var items = transactions;

		while (true) {
			if (info.itemCursor + 1 > items.length) break;
			var trans = items[info.itemCursor];
			var isBold = false;
			var rowFontSize = fontSize - 1 + "pt";
			if (pageSize.name === "A4landscape") { isBold = true; rowFontSize = fontSize - 1 + "pt" }
			var desRows = Math.ceil(gr.getTextSize(trans.Description, font, rowFontSize, isBold, false).width / 237);
			var accName = trans.Account.Name + (trans.DetailAccount ? ' / ' + trans.DetailAccount.Name : '');
			var accNameRows = Math.ceil(gr.getTextSize(accName, font, rowFontSize, isBold, false).width / 153);
			if (accNameRows > desRows) desRows = accNameRows;

			if (r.top + n + (desRows * 30) >= r.height) {
				reachPageEnd = true; break;
			}

			info.itemCursor++;

			// فیلد ها
			gr.drawText(r.width - 5, r.top + n, 50, r.height, info.itemCursor, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
			gr.drawText(r.width - 80, r.top + n, 80, r.height, Hesabfa.farsiDigit(trans.Account.Coding + (trans.DetailAccount ? trans.DetailAccount.Code : "")), font, rowFontSize, "black", "top", "center", isBold, false, false, true);
			gr.drawText(r.width - 235, r.top + n, 155, 500, accName, font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.width - 480, r.top + n, 237, 500, trans.Description, font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 520, r.height, trans.Type === 0 ? Hesabfa.money(trans.Amount) : "", font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 640, r.height, trans.Type === 1 ? Hesabfa.money(trans.Amount) : "", font, rowFontSize, "black", "top", "right", isBold, false, false, true);

			gr.drawLine(r.left, r.top + n + (desRows * 30), r.width + 40, r.top + n + (desRows * 30), "black", "1px", "solid");
			n += 10 + (desRows * 30);
		}

		// خطوط عمودی گزارش
		gr.drawLine(r.width + 40, r.top + rowStart, r.width + 40, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width, r.top + rowStart, r.width, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 75, r.top + rowStart, r.width - 75, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 230, r.top + rowStart, r.width - 230, r.top + n - 10, "black", "1px", "solid"); //سند
		//        gr.drawLine(r.width - 360, r.top + rowStart, r.width - 360, r.top + n - 10, "black", "1px", "solid"); // شرح
		gr.drawLine(r.width - 470, r.top + rowStart, r.width - 470, r.top + n - 10, "black", "1px", "solid"); // بدهكار
		gr.drawLine(r.width - 590, r.top + rowStart, r.width - 590, r.top + n - 10, "black", "1px", "solid");

		gr.drawLine(r.left, r.top + rowStart, r.left, r.top + n - 10, "black", "1px", "solid");
	} // end of if items remaind

	// summary
	if (!reachPageEnd && !info.isSummaryPrinted && info.itemCursor + 1 > transactions.length) {
		if (r.top + n + 60 <= r.height) {
			gr.fillRect(r.left, r.top + n - 9, r.width, 39, "#dcdcdc", "1px", 0);

			var wordifyAmount = !Hesabfa.isDecimalCurrency() ? wordifyfa(sumDebit, 0) : "";
			var wf = 0;
			var hf = 0;
			var wfw = gr.getTextSize(wordifyAmount, font, fontSize + "pt", false, false).width;
			if (wfw > 440) {
				wf = 2;
				wfw = gr.getTextSize(wordifyAmount, font, fontSize - wf + "pt", false, false).width;
			}
			if (wfw > 440) hf = 12;

			// فیلد ها
			gr.drawText(r.left, r.top + n - 2, r.width - 10, r.height, "جمع", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.width - 465, r.top + n - hf, 440, r.height, wordifyAmount + " " + info.currency, font, fontSize - wf + "pt", "black", "top", "right", false, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 520, r.height, Hesabfa.money(sumDebit), font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 640, r.height, Hesabfa.money(sumCredit), font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			gr.drawLine(r.left, r.top + n + 30, r.width + 40, r.top + n + 30, "black", "1px", "solid");
			// شرح سند
			gr.drawText(r.left, r.top + n + 35, r.width, r.height, "شرح سند: " + doc.Description, font, fontSize + "pt", "black", "top", "right", false, false, false, true);

			// خطوط عمودی سطر جمع
			n += 40;
			gr.drawLine(r.width + 40, r.top + rowStart, r.width + 40, r.top + n - 10, "black", "1px", "solid");
			//            gr.drawLine(r.width - 360, r.top + rowStart, r.width - 360, r.top + n - 10, "black", "1px", "solid"); // شرح
			gr.drawLine(r.width - 470, r.top + rowStart, r.width - 470, r.top + n - 10, "black", "1px", "solid"); // بدهكار
			gr.drawLine(r.width - 590, r.top + rowStart, r.width - 590, r.top + n - 10, "black", "1px", "solid");
			gr.drawLine(r.left, r.top + rowStart, r.left, r.top + n - 10, "black", "1px", "solid");

			// signature place if true
			if (info.docSettings.showSignaturePlace && info.docSettings.signatures.length > 0) {
				var signatures = info.docSettings.signatures.reverse();
				var sn = info.docSettings.signatures.length;
				var sw = r.width / sn;
				for (var i = 0; i < sn; i++)
					gr.drawText(r.left + (sw * i), r.top + n + 50, sw, r.height, signatures[i], font, fontSize - 1 + "pt", "black", "top", "center", true, false, false, true);
			}

			info.isSummaryPrinted = true;
		} else reachPageEnd = true;
	}

	if (info.isSummaryPrinted) info.done = true;

	// حساب فا
	if (n <= r.height - 10)
		gr.drawText(r.left, r.height - 10, r.width, r.height, "حساب فا - www.hesabfa.com", font, "10pt", "silver", "top", "center", true, false, false, true);

	if (pageNumber > 1 || !info.done)
		gr.drawText(r.left, r.height, r.width, 35, "صفحه " + pageNumber, font, fontSize + "pt", "black", "top", "left", true, false, false, true);

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
};
function printDocument(doc, docSettings, business, currency) {
	var pageSize = {};
	pageSize.name = "A4portrait";
	pageSize.width = 210;
	pageSize.height = 297;

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	var wnd = window.open("");
	var printImg = [];
	var printDiv = [];

	var info = {
		rect: rect,
		pageSize: pageSize,
		pageNumber: 1,
		done: false,
		itemCursor: 0,
		isSummaryPrinted: false,
		docSettings: docSettings,
		business: business,
		currency: currency
	};
	while (!info.done) {
		var canvas = getDocumentPrintImage(doc, info);
		var n = info.pageNumber - 1;

		printImg[n] = wnd.document.createElement("img");
		printImg[n].width = rect.width;
		printImg[n].height = rect.height;

		printImg[n].src = canvas.toDataURL("image/png");
		wnd.document.body.appendChild(printImg[n]);

		printDiv[n] = wnd.document.createElement("div");
		$(printDiv[n]).css("page-break-after", "always");
		wnd.document.body.appendChild(printDiv[n]);

		document.body.removeChild(canvas);
		delete canvas;
		info.pageNumber++;
	}

	wnd.document.title = "چاپ سند حسابداری";
	setTimeout(function () {
		wnd.print();
	}, 1000);
};
function pdfDocument(doc, docSettings, mass, pdf, business, currency) {
	var pageSize = {};
	pageSize.name = "A4portrait";
	pageSize.width = 210;
	pageSize.height = 297;

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	if (!pdf) pdf = new jsPDF("p", "mm", [pageWidthInMM, pageHeightInMM]);

	var info = {
		rect: rect,
		pageSize: pageSize,
		pageNumber: 1,
		done: false,
		itemCursor: 0,
		isSummaryPrinted: false,
		docSettings: docSettings,
		business: business,
		currency: currency
	};
	while (!info.done) {
		var canvas = getDocumentPrintImage(doc, info);
		pdf.addRawImage(canvas.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, pageWidthInMM, pageHeightInMM);
		document.body.removeChild(canvas);
		delete canvas;
		info.pageNumber++;
	}

	if (!mass)
		pdf.save("document" + doc.Number + ".pdf");
	else return pdf;
};

// برچسب آدرس جهت بسته پستی
function pdfAddressLabel(list, options, business) {
	var pageSize = {};
	pageSize.name = "A4portrait";
	pageSize.width = 210;
	pageSize.height = 297;

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	var pdf = new jsPDF("p", "mm", [pageWidthInMM, pageHeightInMM]);

	var info = {
		rect: rect,
		pageSize: pageSize,
		business: business
	};
	var rpp = 0;
	if (options.plan === 1) rpp = 4;
	else if (options.plan === 2) rpp = 8;
	if (options.businessAddress) rpp = rpp / 2;
	var pageCount = Math.ceil(list.length / rpp);

	//    for (var i = 0; i < pageCount; i++) {
	var i = 0;
	(function loop() {
		var progressValue = Math.floor(((i + 1) * 100) / pageCount);
		var pageData = list.slice(i * rpp, rpp * (i + 1));
		var canvas;
		if (options.plan === 1) canvas = getAddressLabelPlan1_PrintImage(pageData, options, info);
		if (options.plan === 2) canvas = getAddressLabelPlan2_PrintImage(pageData, options, info);
		pdf.addRawImage(canvas.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, pageWidthInMM, pageHeightInMM);
		document.body.removeChild(canvas);
		delete canvas;
		$('#progressPdf').attr('aria-valuenow', progressValue).css('width', progressValue + "%");
		$("#progressPdfTitle").text("در حال تهیه PDF... صفحه " + Hesabfa.farsiDigit(i + 1) + " از " + Hesabfa.farsiDigit(pageCount) + " (" + Hesabfa.farsiDigit(progressValue) + '%)');
		i++;
		if (i < pageCount) setTimeout(loop, 0);
		else {
			pdf.save("addressLabel.pdf");
			setTimeout(function () {
				$("#progressBarDiv").hide();
				$("#printBtns").show();
				$('#progressPdf').attr('aria-valuenow', 1).css('width', 1 + "%");
				$("#progressPdfTitle").text("در حال تهیه PDF...");
			}, 1000);
		}
	})();
	//    }

	//    pdf.save("addressLabel.pdf");
};
function getAddressLabelPlan2_PrintImage(list, options, info) {
	var rect = info.rect;
	var pageSize = info.pageSize;

	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = "iransans";
	var fontSize = 10;

	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);
	gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);

	var topMargin = 10;
	var bottomMargin = 10;
	var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
	var n = 0;
	var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;

	var itemN = 0;
	for (var i = 0; i < 4; i++) {
		//        if (i + 1 >= list.length) break;
		if (itemN >= list.length) break;
		var item = list[itemN];
		itemN++;
		gr.drawRect((r.width / 2) + 40, r.top + n, (r.width / 2), 250, "black", "1px", "solid", 5);

		// box right
		if (options.invoiceNumber)
			gr.drawText(r.left, r.top + n - 20, r.width - 10, r.height, "شماره فاکتور: " + item.Number, font, (fontSize - 1) + "pt", "black", "top", "right", false, false, false, true);
		gr.fillRect(r.width - 45, r.top + n + 5, 80, 30, "silver", "1px", 5);
		gr.drawText(r.left, r.top + n + 10, r.width - 10, r.height, "گیرنده", font, (fontSize + 1) + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 40, r.width - 10, r.height, "استان: " + item.Contact.State, font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 40, r.width - 150, r.height, "شهر: " + item.Contact.City, font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		var strPhone = "   تلفن: ";
		if (item.Contact.Mobile && item.Contact.Mobile !== "") strPhone += item.Contact.Mobile;
		if (item.Contact.Phone && item.Contact.Phone !== "") strPhone += " - " + item.Contact.Phone;
		if (!item.Contact.Mobile && !item.Contact.Phone) strPhone = "";
		var strNoteNewLine = item.Contact.Address !== "" ? "\n" : "";
		var strNote = options.invoiceNote && item.Note !== "" ? strNoteNewLine + item.Note : "";
		gr.drawText((r.width / 2) + 40, r.top + n + 75, (r.width / 2) - 10, r.height, item.Contact.Address + strNote + "\n" + item.Contact.Name + strPhone, font, fontSize + "pt", "black", "top", "right", true, false, false, true);

		var strPostalCode = item.Contact.PostalCode + ""; // کد پستی
		gr.drawText((r.width / 2) + 40, r.top + n + 200, (r.width / 2) - 35, r.height, "کد پستی", font, fontSize + "pt", "#000", "top", "right", true, false, false, true);
		var k1 = (r.width / 2) + 30;
		for (var j = 0; j < 10; j++) {
			gr.drawRect(r.left + k1, r.top + n + 200, 25, 25, "black", "1px", "solid", 0);
			var c2 = strPostalCode.length >= j + 1 ? strPostalCode.substr(j, 1) : "";
			gr.drawText(r.left + k1, r.top + n + 202, 25, 25, c2, font, (fontSize + 1) + "pt", "#000", "center", "center", true, false, false, true);
			k1 = k1 + 25;
		}

		// box left
		if (!options.businessAddress) {
			if (itemN >= list.length) break;
			gr.drawRect(r.left, r.top + n, (r.width / 2) - 10, 250, "black", "1px", "solid", 5);
			var item = list[itemN];
			itemN++;
			if (options.invoiceNumber)
				gr.drawText(r.left, r.top + n - 20, (r.width / 2) - 10, r.height, "شماره فاکتور: " + item.Number, font, (fontSize - 1) + "pt", "black", "top", "right", false, false, false, true);
			gr.fillRect((r.width / 2) - 55, r.top + n + 5, 80, 30, "silver", "1px", 5);
			gr.drawText(r.left, r.top + n + 10, (r.width / 2) - 20, r.height, "گیرنده", font, (fontSize + 1) + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 40, (r.width / 2) - 20, r.height, "استان: " + item.Contact.State, font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 40, (r.width / 2) - 160, r.height, "شهر: " + item.Contact.City, font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			var strPhone = "   تلفن: ";
			if (item.Contact.Mobile && item.Contact.Mobile !== "") strPhone += item.Contact.Mobile;
			if (item.Contact.Phone && item.Contact.Phone !== "") strPhone += " - " + item.Contact.Phone;
			if (!item.Contact.Mobile && !item.Contact.Phone) strPhone = "";
			var strNoteNewLine = item.Contact.Address !== "" ? "\n" : "";
			var strNote = options.invoiceNote && item.Note !== "" ? strNoteNewLine + item.Note : "";
			gr.drawText(r.left, r.top + n + 75, (r.width / 2) - 20, r.height, item.Contact.Address + strNote + "\n" + item.Contact.Name + strPhone, font, fontSize + "pt", "black", "top", "right", true, false, false, true);

			var strPostalCode = item.Contact.PostalCode + ""; // کد پستی
			gr.drawText(r.left, r.top + n + 200, (r.width / 2) - 35, r.height, "کد پستی", font, fontSize + "pt", "#000", "top", "right", true, false, false, true);
			var k1 = 30;
			for (var j = 0; j < 10; j++) {
				gr.drawRect(r.left + k1, r.top + n + 200, 25, 25, "black", "1px", "solid", 0);
				var c2 = strPostalCode.length >= j + 1 ? strPostalCode.substr(j, 1) : "";
				gr.drawText(r.left + k1, r.top + n + 202, 25, 25, c2, font, (fontSize + 1) + "pt", "#000", "center", "center", true, false, false, true);
				k1 = k1 + 25;
			}
		} else {
			gr.drawRect(r.left, r.top + n, (r.width / 2) - 10, 250, "black", "1px", "solid", 5);
			gr.fillRect((r.width / 2) - 55, r.top + n + 5, 80, 30, "silver", "1px", 5);
			gr.drawText(r.left, r.top + n + 10, (r.width / 2) - 20, r.height, "فرستنده", font, (fontSize + 1) + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 40, (r.width / 2) - 20, r.height, "استان: " + info.business.State, font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 40, (r.width / 2) - 160, r.height, "شهر: " + info.business.City, font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			var strPhone = "   تلفن: ";
			if (info.business.Phone && info.business.Phone !== "") strPhone += info.business.Phone;
			else strPhone = "";
			gr.drawText(r.left, r.top + n + 75, (r.width / 2) - 20, r.height, info.business.Address + "\n" + strName + strPhone, font, fontSize + "pt", "black", "top", "right", true, false, false, true);

			var strPostalCode = info.business.PostalCode; // کد پستی
			gr.drawText(r.left, r.top + n + 200, (r.width / 2) - 35, r.height, "کد پستی", font, fontSize + "pt", "#000", "top", "right", true, false, false, true);
			var k1 = 30;
			for (var j = 0; j < 10; j++) {
				gr.drawRect(r.left + k1, r.top + n + 200, 25, 25, "black", "1px", "solid", 0);
				var c2 = strPostalCode.length >= j + 1 ? strPostalCode.substr(j, 1) : "";
				gr.drawText(r.left + k1, r.top + n + 202, 25, 25, c2, font, (fontSize + 1) + "pt", "#000", "center", "center", true, false, false, true);
				k1 = k1 + 25;
			}
		}
		n += 280;
	}

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
}
function getAddressLabelPlan1_PrintImage(list, options, info) {
	var rect = info.rect;
	var pageSize = info.pageSize;

	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = "iransans";
	var fontSize = 10;

	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);
	gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);

	var topMargin = 10;
	var bottomMargin = 10;
	var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
	var n = 0;
	var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;

	var itemN = 0;
	for (var i = 0; i < 2; i++) {
		if (itemN >= list.length) break;
		var item = list[itemN];
		itemN++;
		gr.drawRect(r.left, r.top + n, r.width, 250, "black", "1px", "solid", 5);

		// box top
		if (options.invoiceNumber)
			gr.drawText(r.left, r.top + n - 20, r.width - 10, r.height, "شماره فاکتور: " + item.Number, font, (fontSize - 1) + "pt", "black", "top", "right", false, false, false, true);
		gr.fillRect(r.width - 45, r.top + n + 5, 80, 30, "silver", "1px", 5);
		gr.drawText(r.left, r.top + n + 10, r.width - 10, r.height, "گیرنده", font, (fontSize + 1) + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 40, r.width - 10, r.height, "استان: " + item.Contact.State, font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n + 40, r.width - 150, r.height, "شهر: " + item.Contact.City, font, fontSize + "pt", "black", "top", "right", true, false, false, true);
		var strPhone = "   تلفن: ";
		if (item.Contact.Mobile && item.Contact.Mobile !== "") strPhone += item.Contact.Mobile;
		if (item.Contact.Phone && item.Contact.Phone !== "") strPhone += " - " + item.Contact.Phone;
		if (!item.Contact.Mobile && !item.Contact.Phone) strPhone = "";
		var strNoteNewLine = item.Contact.Address !== "" ? "\n" : "";
		var strNote = options.invoiceNote && item.Note !== "" ? strNoteNewLine + item.Note : "";
		gr.drawText(r.left, r.top + n + 75, r.width - 10, r.height, item.Contact.Address + strNote + "\n" + item.Contact.Name + strPhone, font, fontSize + "pt", "black", "top", "right", true, false, false, true);

		var strPostalCode = item.Contact.PostalCode; // کد پستی
		gr.drawText((r.width / 2) + 40, r.top + n + 200, (r.width / 2) - 35, r.height, "کد پستی", font, fontSize + "pt", "#000", "top", "right", true, false, false, true);
		var k1 = (r.width / 2) + 30;
		for (var j = 0; j < 10; j++) {
			gr.drawRect(r.left + k1, r.top + n + 200, 25, 25, "black", "1px", "solid", 0);
			var c2 = strPostalCode.length >= j + 1 ? strPostalCode.substr(j, 1) : "";
			gr.drawText(r.left + k1, r.top + n + 202, 25, 25, c2, font, (fontSize + 1) + "pt", "#000", "center", "center", true, false, false, true);
			k1 = k1 + 25;
		}

		n += 280;
		// box bottom
		if (!options.businessAddress) {
			if (itemN >= list.length) break;
			gr.drawRect(r.left, r.top + n, r.width, 250, "black", "1px", "solid", 5);
			var item = list[itemN];
			itemN++;
			if (options.invoiceNumber)
				gr.drawText(r.left, r.top + n - 20, r.width - 10, r.height, "شماره فاکتور: " + item.Number, font, (fontSize - 1) + "pt", "black", "top", "right", false, false, false, true);
			gr.fillRect(r.width - 45, r.top + n + 5, 80, 30, "silver", "1px", 5);
			gr.drawText(r.left, r.top + n + 10, r.width - 10, r.height, "گیرنده", font, (fontSize + 1) + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 40, r.width - 10, r.height, "استان: " + item.Contact.State, font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 40, r.width - 150, r.height, "شهر: " + item.Contact.City, font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			var strPhone = "   تلفن: ";
			if (item.Contact.Mobile && item.Contact.Mobile !== "") strPhone += item.Contact.Mobile;
			if (item.Contact.Phone && item.Contact.Phone !== "") strPhone += " - " + item.Contact.Phone;
			if (!item.Contact.Mobile && !item.Contact.Phone) strPhone = "";
			var strNoteNewLine = item.Contact.Address !== "" ? "\n" : "";
			var strNote = options.invoiceNote && item.Note !== "" ? strNoteNewLine + item.Note : "";
			gr.drawText(r.left, r.top + n + 75, r.width - 10, r.height, item.Contact.Address + strNote + "\n" + item.Contact.Name + strPhone, font, fontSize + "pt", "black", "top", "right", true, false, false, true);

			var strPostalCode = item.Contact.PostalCode + ""; // کد پستی
			gr.drawText((r.width / 2) + 40, r.top + n + 200, (r.width / 2) - 35, r.height, "کد پستی", font, fontSize + "pt", "#000", "top", "right", true, false, false, true);
			var k1 = (r.width / 2) + 30;
			for (var j = 0; j < 10; j++) {
				gr.drawRect(r.left + k1, r.top + n + 200, 25, 25, "black", "1px", "solid", 0);
				var c2 = strPostalCode.length >= j + 1 ? strPostalCode.substr(j, 1) : "";
				gr.drawText(r.left + k1, r.top + n + 202, 25, 25, c2, font, (fontSize + 1) + "pt", "#000", "center", "center", true, false, false, true);
				k1 = k1 + 25;
			}
		} else {
			gr.drawRect(r.left, r.top + n, r.width, 250, "black", "1px", "solid", 5);
			gr.fillRect(r.width - 45, r.top + n + 5, 80, 30, "silver", "1px", 5);
			gr.drawText(r.left, r.top + n + 10, r.width - 10, r.height, "فرستنده", font, (fontSize + 1) + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 40, r.width - 10, r.height, "استان: " + info.business.State, font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 40, r.width - 150, r.height, "شهر: " + info.business.City, font, fontSize + "pt", "black", "top", "right", true, false, false, true);
			var strPhone = "   تلفن: ";
			if (info.business.Phone && info.business.Phone !== "") strPhone += info.business.Phone;
			else strPhone = "";
			gr.drawText(r.left, r.top + n + 75, r.width - 10, r.height, info.business.Address + "\n" + strName + strPhone, font, fontSize + "pt", "black", "top", "right", true, false, false, true);

			var strPostalCode = info.business.PostalCode + ""; // کد پستی
			gr.drawText((r.width / 2) + 40, r.top + n + 200, (r.width / 2) - 35, r.height, "کد پستی", font, fontSize + "pt", "#000", "top", "right", true, false, false, true);
			var k1 = (r.width / 2) + 30;
			for (var j = 0; j < 10; j++) {
				gr.drawRect(r.left + k1, r.top + n + 200, 25, 25, "black", "1px", "solid", 0);
				var c2 = strPostalCode.length >= j + 1 ? strPostalCode.substr(j, 1) : "";
				gr.drawText(r.left + k1, r.top + n + 202, 25, 25, c2, font, (fontSize + 1) + "pt", "#000", "center", "center", true, false, false, true);
				k1 = k1 + 25;
			}
		}
		n += 280;
	}

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
}

// برچسب بارکد کالاها
function pdfBarcodeLabel(list, options, business, currency) {
	var pageSize = {};
	pageSize.name = "A4portrait";
	pageSize.width = 210;
	pageSize.height = 297;
	if (options.plan === 4) {
		pageSize.name = "Barcode";
		pageSize.width = 70;
		pageSize.height = 43;
	}

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	var pdf = new jsPDF("p", "mm", [pageWidthInMM, pageHeightInMM]);

	var info = {
		rect: rect,
		pageSize: pageSize,
		business: business,
		currency: currency
	};
	var rpp = 0;
	if (options.plan === 1) rpp = 21;
	else if (options.plan === 2) rpp = 36;
	else if (options.plan === 3) rpp = 36;
	else if (options.plan === 4) rpp = 1;
	var pageCount = Math.ceil(list.length / rpp);

	var i = 0;
	(function loop() {
		var progressValue = Math.floor(((i + 1) * 100) / pageCount);
		var pageData = list.slice(i * rpp, rpp * (i + 1));
		var canvas;
		if (options.plan === 1) canvas = getBarcodeLabelPlan1_PrintImage(pageData, options, info);
		if (options.plan === 2) canvas = getBarcodeLabelPlan2_PrintImage(pageData, options, info);
		if (options.plan === 3) canvas = getBarcodeLabelPlan3_PrintImage(pageData, options, info);
		if (options.plan === 4) canvas = getBarcodeLabelPlan4_PrintImage(pageData, options, info);
		pdf.addRawImage(canvas.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, pageWidthInMM, pageHeightInMM);
		document.body.removeChild(canvas);
		delete canvas;
		$('#progressPdf').attr('aria-valuenow', progressValue).css('width', progressValue + "%");
		$("#progressPdfTitle").text("در حال تهیه PDF... صفحه " + Hesabfa.farsiDigit(i + 1) + " از " + Hesabfa.farsiDigit(pageCount) + " (" + Hesabfa.farsiDigit(progressValue) + '%)');
		i++;
		if (i < pageCount) setTimeout(loop, 0);
		else {
			pdf.save("barcodeLabel.pdf");
			setTimeout(function () {
				$("#progressBarDiv").hide();
				$("#printBtns").show();
				$('#progressPdf').attr('aria-valuenow', 1).css('width', 1 + "%");
				$("#progressPdfTitle").text("در حال تهیه PDF...");
			}, 1000);
		}
	})();

	//    pdf.save("addressLabel.pdf");
};
function getBarcodeLabelPlan1_PrintImage(list, options, info) {
	var rect = info.rect;
	var pageSize = info.pageSize;

	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = "iransans";
	var fontSize = 10;

	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);
	gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);

	var topMargin = 10;
	var bottomMargin = 10;
	var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
	var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;

	var barcodeWidth = (r.width / 3) - 10;
	var barcodeHeight = 130;
	var n = 0;  // top
	for (var n1 = 0; n1 < 7; n1++) {
		if (list.length === 0) break;
		var l = 0; // left
		for (var n2 = 0; n2 < 3; n2++) {
			if (list.length === 0) break;
			var item = list.shift();
			if (options.border)
				gr.drawRect(r.left + l, r.top + n, barcodeWidth, barcodeHeight, "black", "1px", "solid", 5);
			if (options.businessName)
				gr.drawText(r.left + l, r.top + n + 5, barcodeWidth, 40, strName, font, fontSize + "pt", "black", "top", "center", false, false, false, true);
			if (item.Barcode !== "") {
				barcode(gr, item.Barcode, r.left + l + 20, r.top + n + 25, barcodeWidth - 40, barcodeHeight - 90);
				gr.drawText(r.left + l, r.top + n + 65, barcodeWidth, 40, item.Barcode, font, fontSize + "pt", "black", "top", "center", false, false, false, false, true);
			}
			if (options.itemName)
				gr.drawText(r.left + l, r.top + n + 85, barcodeWidth - 5, 70, item.Name, font, (fontSize - 1) + "pt", "black", "top", "right", false, false, false, true);
			if (options.price) {
				var w1 = gr.getTextSize(Hesabfa.money(item.SellPrice) + " " + info.currency, font, (fontSize - 1) + "pt", false, false).width;
				gr.fillRect(r.left + l + 3, r.top + n + 103, w1 + 13, 22, options.priceBg ? "silver" : "white", "1px", 5);
				gr.drawText(r.left + l + 7, r.top + n + 105, barcodeWidth - 3, 40, Hesabfa.money(item.SellPrice) + " " + info.currency, font, (fontSize - 1) + "pt", "black", "top", "left", false, false, false, true);
			}

			l += barcodeWidth + 15;
		}
		n += 150;
	}

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
}
function getBarcodeLabelPlan2_PrintImage(list, options, info) {
	var rect = info.rect;
	var pageSize = info.pageSize;

	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = "iransans";
	var fontSize = 10;

	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);
	gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);

	var topMargin = 10;
	var bottomMargin = 10;
	var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
	var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;

	var barcodeWidth = (r.width / 4) - 10;
	var barcodeHeight = 95;
	var n = 0;  // top
	for (var n1 = 0; n1 < 9; n1++) {
		if (list.length === 0) break;
		var l = 0; // left
		for (var n2 = 0; n2 < 4; n2++) {
			if (list.length === 0) break;
			var item = list.shift();
			if (options.border)
				gr.drawRect(r.left + l, r.top + n, barcodeWidth, barcodeHeight, "black", "1px", "solid", 5);
			if (item.Barcode !== "") {
				barcode(gr, item.Barcode, r.left + l + 10, r.top + n + 10, barcodeWidth - 20, barcodeHeight - 50);
				gr.drawText(r.left + l, r.top + n + 55, barcodeWidth, 40, item.Barcode, font, fontSize + "pt", "black", "top", "center", false, false, false, false, true);
			}
			if (options.itemName)
				gr.drawText(r.left + l, r.top + n + 73, barcodeWidth - 5, 70, item.Name, font, (fontSize - 2) + "pt", "black", "top", "right", true, false, false, true);
			if (options.price) {
				var w1 = gr.getTextSize(Hesabfa.money(item.SellPrice) + " " + info.currency, font, (fontSize - 2) + "pt", true, false).width;
				gr.fillRect(r.left + l + 3, r.top + n + 71, w1 + 5, 20, options.priceBg ? "silver" : "white", "1px", 5);
				gr.drawText(r.left + l + 3, r.top + n + 73, barcodeWidth - 3, 30, Hesabfa.money(item.SellPrice) + " " + info.currency, font, (fontSize - 2) + "pt", "black", "top", "left", true, false, false, true);
			}

			l += barcodeWidth + 15;
		}
		n += 115;
	}

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
}
function getBarcodeLabelPlan3_PrintImage(list, options, info) {
	var rect = info.rect;
	var pageSize = info.pageSize;

	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = "iransans";
	var fontSize = 10;

	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);
	gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);

	var topMargin = 10;
	var bottomMargin = 10;
	var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
	var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;

	var barcodeWidth = (r.width / 4) - 10;
	var barcodeHeight = 95;
	var n = 0;  // top
	for (var n1 = 0; n1 < 9; n1++) {
		if (list.length === 0) break;
		var l = 0; // left
		for (var n2 = 0; n2 < 4; n2++) {
			if (list.length === 0) break;
			var item = list.shift();
			if (options.border)
				gr.drawRect(r.left + l, r.top + n, barcodeWidth, barcodeHeight, "black", "1px", "solid", 5);
			if (item.Barcode !== "") {
				barcode(gr, item.Barcode, r.left + l + 10, r.top + n + 10, barcodeWidth - 20, barcodeHeight - 35);
				gr.drawText(r.left + l, r.top + n + 70, barcodeWidth, 40, item.Barcode, font, fontSize + "pt", "black", "top", "center", false, false, false, true, true);
			}

			l += barcodeWidth + 15;
		}
		n += 115;
	}

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
}
function getBarcodeLabelPlan4_PrintImage(list, options, info) {
	var rect = info.rect;
	var pageSize = info.pageSize;

	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = "iransans";
	var fontSize = 10;

	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);
	gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);

	var topMargin = 5;
	var bottomMargin = 5;
	var r = { left: 5 * mm, top: topMargin * mm, width: rect.width - 10 * mm, height: rect.height - bottomMargin * mm };
	var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;

	var barcodeWidth = (r.width);
	var barcodeHeight = 130;
	var n = 0;  // top
	var l = 0;
	if (list.length > 0) {
		var item = list.shift();
		if (options.border)
			gr.drawRect(r.left + l, r.top + n, barcodeWidth, barcodeHeight, "black", "1px", "solid", 5);
		if (options.businessName)
			gr.drawText(r.left + l, r.top + n + 5, barcodeWidth, 40, strName, font, fontSize + "pt", "black", "top", "center", false, false, false, true);
		if (item.Barcode !== "") {
			barcode(gr, item.Barcode, r.left + l + 20, r.top + n + 25, barcodeWidth - 40, barcodeHeight - 90);
			gr.drawText(r.left + l, r.top + n + 65, barcodeWidth, 40, item.Barcode, font, fontSize + "pt", "black", "top", "center", false, false, false, false, true);
		}
		if (options.itemName)
			gr.drawText(r.left + l, r.top + n + 85, barcodeWidth - 5, 70, item.Name, font, (fontSize - 1) + "pt", "black", "top", "right", false, false, false, true);
		if (options.price) {
			var w1 = gr.getTextSize(Hesabfa.money(item.SellPrice) + " " + info.currency, font, (fontSize - 1) + "pt", false, false).width;
			gr.fillRect(r.left + l + 3, r.top + n + 103, w1 + 13, 22, options.priceBg ? "silver" : "white", "1px", 5);
			gr.drawText(r.left + l + 7, r.top + n + 105, barcodeWidth - 3, 40, Hesabfa.money(item.SellPrice) + " " + info.currency, font, (fontSize - 1) + "pt", "black", "top", "left", false, false, false, true);
		}
	}
	n += 150;

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
}

function pdfBarcodeLabelNew(labelInfo, items, currency) {
	function mm(d) {
		return d * 3.9381;
	}
	var pageSize = {};
	var margins;
	if (labelInfo.printType === 0) {
		pageSize.width = 210;
		pageSize.height = 297;
		margins = 5;
	} else {
		pageSize.width = labelInfo.width;
		pageSize.height = labelInfo.height;
		margins = 0;
	}

	var width = pageSize.width;
	var height = pageSize.height;

	var pdf = new jsPDF("p", "mm", [width, height]);
	var canvas;
	function prepareGraphics() {
		var scale = labelInfo.printType === 0 ? 2 : 3;
		canvas = document.createElement("canvas");
		document.body.appendChild(canvas);
		canvas.style.direction = "rtl";
		canvas.style.display = "none";
		canvas.width = mm(width) * scale;
		canvas.height = mm(height) * scale;
		var _gr = new graphics(canvas);
		_gr.scale(scale, scale);
		return _gr;
	}

	function deleteGraphics() {
		$(canvas).remove();
	}

	function simulate() {
		var pageCount = 0;
		var l = margins, t = margins;
		var printed = false;
		for (var i = 0; i < items.length; i++) {
			printed = true;
			l += labelInfo.width;
			if (l + labelInfo.width > pageSize.width - 2 * margins) {
				t += labelInfo.height;
				l = margins;
			}
			if (t + labelInfo.height > pageSize.height - 2 * margins) {
				pageCount++;
				printed = false;
				t = margins;
				l = margins;
			}
		}
		if (printed)
			pageCount++;
		return pageCount;
	}


	var pageCount = simulate();
	var pageIndex = 0;
	var progressbar;
	var popup = $('<div>').appendTo(document.body);
	popup.dxPopup({
		width: 400,
		height: 200,
		contentTemplate: function (ce) {
			var progress = $('<div>').appendTo(ce);
			var status = $('<div class="text-center text-info" style="font-size:12pt">').appendTo(ce);
			progressbar = $(progress).dxProgressBar({
				min: 1,
				max: pageCount,
				height: 50,
				showStatus: false,
				statusFormat: function (value) {
					status.html(Hesabfa.farsiDigit("در حال ساخت صفحه " + (pageIndex + 1) + " از " + pageCount));
				}
			}).dxProgressBar("instance");

		},
		showTitle: true,
		rtlEnabled: true,
		showCloseButton: false,
		title: "در حال ساخت فایل PDF",
		visible: true,
		dragEnabled: true,
		closeOnOutsideClick: false
	}).dxPopup("instance");

	var gr = prepareGraphics();
	function print(startIndex) {
		gr.fillRect(0, 0, mm(width), mm(height), "#fff", "1px", 0);
		var l = margins, t = margins;
		var printed = false;
		for (var i = startIndex; i < items.length; i++) {
			pdfBarcodeLabelNewPrint(gr, items[i], labelInfo, l, t, currency);
			printed = true;
			l += labelInfo.width;
			if (l + labelInfo.width > pageSize.width - 2 * margins) {
				t += labelInfo.height;
				l = margins;
			}
			if (t + labelInfo.height > pageSize.height - 2 * margins) {
				pdf.addRawImageGray(gr.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, width, height);
				setTimeout(function () {
					progressbar.option("value", ++pageIndex);
					print(i + 1);
				}, 500);
				return;
			}
		}
		if (printed)
			pdf.addRawImageGray(gr.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, width, height);
		deleteGraphics();
		pdf.save("label.pdf");
		popup.hide();
	}

	print(0);

};
function pdfBarcodeLabelNewPrint(gr, item, labelInfo, left, top, currency) {
	function mm(d) {
		return d * 3.9381;
	}
	function drawCell(gr, l, t, w, h, text, fontName, fontSize, bold, hAlign, vAlign, textColor, backColor, borderColor, borderWidth, borderStype, rtl) {
		gr.fillRect(mm(l), mm(t), mm(w), mm(h), backColor, "1px");
		gr.drawRect(mm(l), mm(t), mm(w), mm(h), borderColor, borderWidth, borderStype);
		gr.drawText(mm(l), mm(t), mm(w), mm(h), text, fontName, fontSize, textColor, vAlign, hAlign, bold, false, false, rtl);
	}

	var margins = 2;
	var l = left + margins;
	var t = top + margins;
	var r = left + labelInfo.width - margins;
	var b = top + labelInfo.height - margins;
	var w = r - l;
	var h = b - t;
	if (labelInfo.showBorder)
		gr.drawRect(mm(l), mm(t), mm(w), mm(h), "#000", "1px", "solid");
	var fs1 = "6pt";
	var fs2 = "8pt";
	var fs3 = "6pt";
	var fs4 = "12pt";
	var rh = 0;
	if (labelInfo.title)
		rh += 5;
	if (labelInfo.showItemName || labelInfo.showItemCode)
		rh += 7;
	else
		rh += 2;
	if (labelInfo.showBarcode)
		rh += 12;
	if (labelInfo.showPrice)
		rh += 8;

	if (!labelInfo.showBarcode) {
		fs1 = "8pt";
		fs2 = "10pt";
		fs4 = "14pt";
	}
	var ratio = h / rh;
	if (labelInfo.title) {
		drawCell(gr, l, t, w, 5 * ratio, labelInfo.title, "Iransans", fs1, true, "center", "middle", "#000", "transparent", "transparent", "0", "none", true);
		t += 5 * ratio;
	}
	if (labelInfo.showItemName || labelInfo.showItemCode) {
		var str = (labelInfo.showItemCode ? item.code : "");
		if (labelInfo.showItemName) {
			if (str !== "")
				str += " - ";
			str += item.name;
		}
		drawCell(gr, l, t, w, 7 * ratio, str, "Iransans", fs2, true, "center", "middle", "#000", "transparent", "transparent", "0", "none", true);
		t += 7 * ratio;
	} else {
		t += 2 * ratio;
	}
	if (labelInfo.showBarcode) {
		var bh = 8 * ratio;
		var th = 4 * ratio;
		drawBarcode(gr, item.barcode, mm(l + 5), mm(t), mm(w - 10), mm(bh));
		//barcode(gr, item.barcode, mm(l + 5), mm(t), mm(w - 10), mm(bh));
		drawCell(gr, l, t + bh, w, th, item.barcode, "Iransans", fs3, false, "center", "middle", "#000", "transparent", "transparent", "0", "none", true);
		t += (bh + th);
	}
	if (labelInfo.showPrice) {
		drawCell(gr, l, t, w, 8 * ratio, Hesabfa.money(item.price) + " " + currency, "Iransans", fs4, true, "center", "middle", "#000", "transparent", "transparent", "0", "none", true);
	}
}


// رسید دریافت و پرداخت
function getReceiptPrintImage(info) {
	var rect = info.rect;
	var pageSize = info.pageSize;
	var pageNumber = info.pageNumber;
	var settings = info.settings;
	var reachPageEnd = false;

	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = "iransans";
	var fontX = 0;
	var fontSize = 10;
	var titleFontSize = fontSize + fontX + 3 + "pt";
	var subtitleFontSize = fontSize + fontX + 2 + "pt";
	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);
	gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);
	var topMargin = 10;
	var bottomMargin = 20;
	var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
	var n = 0;

	gr.drawText(r.left, r.top, r.width, r.height, info.title, font, titleFontSize, "#000", "top", "center", true, false, false, true);
	gr.drawText(r.left, r.top + 30, r.width, r.height, info.subTitle, font, subtitleFontSize, "gray", "top", "center", true, false, false, true);
	if (info.docNumber !== 0)
		gr.drawText(r.left, r.top, r.width, r.height, "شماره سند: " + info.docNumber, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);
	gr.drawText(r.left, r.top + 25, r.width, r.height, "تاریخ: " + info.date, font, fontSize + fontX + "pt", "#000", "top", "left", false, false, false, true);
	n += 75;
	gr.drawLine(r.left, r.top + n, r.width + 35, r.top + n, "black", "1px", "solid");
	n += 20;

	if (info.pageNumber === 1)   // متن رسید، چاپ فقط در صفحه نخست
		gr.drawText(r.left, r.top + n, r.width, r.height, info.text, font, (fontSize + fontX + 1) + "pt", "#000", "top", "right", false, false, false, true);

	var rowCount = Math.ceil(gr.getTextSize(info.text, font, (fontSize + fontX + 1) + "pt", false, false).width / r.width);
	if (rowCount > 1) n += 30 * rowCount;

	// transactions if exist
	if (!reachPageEnd && info.transactions && info.transactions.length > 0 && !info.isTransactionsPrinted && info.showTransactions === true) {
		if (r.top + n + 60 <= r.height) {
			if (!info.itemCursor) info.itemCursor = 0;
			var rowFontSize2 = fontSize + fontX - 2 + "pt";
			n += 40;
			var start2 = r.top + n;
			//            gr.fillRect(r.width - 50, r.top + n, 90, 23, "#dcdcdc", "1px", 0);
			gr.drawText(r.left, r.top + n + 1, r.width, 25, "ریز تراکنش ها", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			n += 5;
			while (true) {
				if (info.itemCursor + 1 > info.transactions.length) break;
				if (r.top + n + 25 > r.height) {
					reachPageEnd = true; break;
				}
				var payment = info.transactions[info.itemCursor];
				n += 25;
				var t = "مبلغ " + Hesabfa.money(payment.Amount) + " " + info.moneyUnit;
				//                t += " در تاریخ " + Hesabfa.farsiDigit(payment.DateTimeShamsi);
				if (payment.Cheque == null || payment.Cheque.BankName === "") {
					t += " بصورت نقدی";
					if (payment.Cash != null)
						t += " ،صندوق: " + payment.Cash.Name;
					if (payment.Bank != null)
						t += " ،بانک: " + payment.Bank.Name;
				}
				if (payment.Cheque != null && payment.Cheque.BankName !== "") {
					t += " طی چک شماره " + payment.Cheque.ChequeNumber;
					t += payment.Cheque.BankName ? " بانک " + payment.Cheque.BankName : "";
					t += payment.Cheque.BankBranch ? " شعبه " + payment.Cheque.BankBranch : "";
					t += payment.Cheque.DateShamsi ? " به تاریخ " + payment.Cheque.DateShamsi : "";
				}
					
				if (payment.Reference) t += ". شماره ارجاع: " + payment.Reference;
				gr.drawText(r.left, r.top + n, r.width - 10, 25, (info.itemCursor + 1), font, fontSize - 1 + "pt", "black", "top", "right", false, false, false, true);
				gr.drawText(r.left, r.top + n, r.width - 40, 25, t, font, fontSize - 1 + "pt", "black", "top", "right", false, false, false, true);
				info.itemCursor++;
				gr.drawRect(r.left, r.top + n - 3, r.width + 1, 25, "black", "1px", "solid");
				gr.drawLine(r.width + 10, r.top + n - 3, r.width + 10, r.top + n - 3 + 25, "black", "1px", "solid");
			}
			if (!reachPageEnd && r.top + n + 25 <= r.height) {
				n += 25;
				gr.fillRect(r.left, r.top + n, r.width + 1, 30, "#dcdcdc", "1px", 0);
				gr.drawText(r.left, r.top + n + 5, r.width - 10, 25, info.totalStr, font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
				gr.drawRect(r.left, r.top + n, r.width + 1, 30, "black", "1px", "solid");
				info.isTransactionsPrinted = true;
			} else reachPageEnd = true;
		} else reachPageEnd = true;
	}
	else if (!info.transactions || info.transactions.length === 0 || info.showTransactions === false) info.isTransactionsPrinted = true;

	// footer note if exist
	if (!reachPageEnd && info.isTransactionsPrinted && info.note && !info.isNotePrinted) {
		if (r.top + n + 40 <= r.height) {
			n += 40;
			gr.drawText(r.left, r.top + n, r.width, r.height, info.note, font, fontSize + fontX - 1 + "pt", "black", "top", "right", false, false, false, true);
			info.isNotePrinted = true;
		} else reachPageEnd = true;
	}
	else if (!info.note) info.isNotePrinted = true;

	// signature place if true
	if (!reachPageEnd && info.isNotePrinted) {
		if (r.top + n + 60 <= r.height) {
			n += 70;
			gr.drawText(r.left + (r.width / 2), r.top + n, (r.width / 2), r.height, "مهر و امضای دریافت کننده", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
			gr.drawText(r.left, r.top + n, (r.width / 2), r.height, "مهر و امضای پرداخت کننده", font, fontSize + fontX - 1 + "pt", "black", "top", "center", false, false, false, true);
			info.done = true;
		} else reachPageEnd = true;
	}

	if (pageNumber > 1 || !info.done)
		gr.drawText(r.left, r.height, r.width, 35, "صفحه " + pageNumber, font, fontSize + fontX - 1 + "pt", "black", "top", "left", true, false, false, true);

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
};
function printReceipt(info) {
	var pageSize = {};
	if (info.settings.pageSize === "A4" || info.settings.pageSize === "A4Portrait") {
		pageSize.name = "A4";
		pageSize.width = 210;
		pageSize.height = 297;
	}
	if (info.settings.pageSize === "A5") {
		pageSize.name = "A5";
		pageSize.width = 148;
		pageSize.height = 210;
	}
	else if (info.settings.pageSize === "A5Landscape") {
		pageSize.name = "A5Landscape";
		pageSize.width = 210;
		pageSize.height = 148;
	}

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	var wnd = window.open("");
	var printImg = [];
	var printDiv = [];

	info.rect = rect;
	info.pageSize = pageSize;
	info.pageNumber = 1;
	info.done = false;
	info.itemCursor = 0;
	info.isTransactionsPrinted = false;

	while (!info.done) {
		var canvas = getReceiptPrintImage(info);
		var n = info.pageNumber - 1;

		printImg[n] = wnd.document.createElement("img");
		printImg[n].width = rect.width;
		printImg[n].height = rect.height;

		printImg[n].src = canvas.toDataURL("image/png");
		wnd.document.body.appendChild(printImg[n]);

		printDiv[n] = wnd.document.createElement("div");
		$(printDiv[n]).css("page-break-after", "always");
		wnd.document.body.appendChild(printDiv[n]);

		document.body.removeChild(canvas);
		delete canvas;
		info.pageNumber++;
	}

	wnd.document.title = "چاپ رسید";
	setTimeout(function () {
		wnd.print();
	}, 1000);
};
function pdfReceipt(info) {
	var pageSize = {};
	if (info.settings.pageSize === "A4" || info.settings.pageSize === "A4Portrait") {
		pageSize.name = "A4";
		pageSize.width = 210;
		pageSize.height = 297;
	}
	if (info.settings.pageSize === "A5") {
		pageSize.name = "A5";
		pageSize.width = 148;
		pageSize.height = 210;
	}
	else if (info.settings.pageSize === "A5Landscape") {
		pageSize.name = "A5Landscape";
		pageSize.width = 210;
		pageSize.height = 148;
	}

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	var pdf = new jsPDF("p", "mm", [pageWidthInMM, pageHeightInMM]);

	info.rect = rect;
	info.pageSize = pageSize;
	info.pageNumber = 1;
	info.done = false;
	info.itemCursor = 0;
	info.isTransactionsPrinted = false;

	while (!info.done) {
		var canvas = getReceiptPrintImage(info);
		pdf.addRawImage(canvas.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, pageWidthInMM, pageHeightInMM);
		document.body.removeChild(canvas);
		delete canvas;
		info.pageNumber++;
	}

	pdf.save("receipt.pdf");
};


function drawBarcode(gr, barcode, left, top, width, height) {
	var CODE39MAP = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
					"A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
					"K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
					"U", "V", "W", "X", "Y", "Z", "-", ".", " ", "$",
					"/", "+", "%"];
	function getCode39Value(inputchar) {
		for (var i = 0; i < 43; i++) {
			if (inputchar === CODE39MAP[i])
				return i;
		}
		return -1;
	}
	function generateCheckDigit(data) {
		var sumValue = 0;
		for (var i = 0; i < data.length; i++) {
			sumValue = sumValue + getCode39Value(data[i]);
		}
		sumValue = sumValue % 43;
		return CODE39MAP[sumValue];
	}
	function filterInput(data) {
		var ret = "";
		for (var i = 0; i < data.length; i++) {
			if (getCode39Value(data[i]) !== -1)
				ret = ret + data[i];
		}
		return ret;
	}
	function EncodeCode39(data, checkDigit) {
		var fontOutput = filterInput(data.toUpperCase());
		if (checkDigit)
			fontOutput += generateCheckDigit(ret);
		fontOutput = "*" + fontOutput + "*";
		var output = "";
		var pattern = "";
		for (var x = 0; x < fontOutput.length; x++) {
			switch (fontOutput.substr(x, 1)) {
				case '1':
					pattern = "wttwttttwt";
					break;
				case '2':
					pattern = "ttwwttttwt";
					break;
				case '3':
					pattern = "wtwwtttttt";
					break;
				case '4':
					pattern = "tttwwtttwt";
					break;
				case '5':
					pattern = "wttwwttttt";
					break;
				case '6':
					pattern = "ttwwwttttt";
					break;
				case '7':
					pattern = "tttwttwtwt";
					break;
				case '8':
					pattern = "wttwttwttt";
					break;
				case '9':
					pattern = "ttwwttwttt";
					break;
				case '0':
					pattern = "tttwwtwttt";
					break;
				case 'A':
					pattern = "wttttwttwt";
					break;
				case 'B':
					pattern = "ttwttwttwt";
					break;
				case 'C':
					pattern = "wtwttwtttt";
					break;
				case 'D':
					pattern = "ttttwwttwt";
					break;
				case 'E':
					pattern = "wtttwwtttt";
					break;
				case 'F':
					pattern = "ttwtwwtttt";
					break;
				case 'G':
					pattern = "tttttwwtwt";
					break;
				case 'H':
					pattern = "wttttwwttt";
					break;
				case 'I':
					pattern = "ttwttwwttt";
					break;
				case 'J':
					pattern = "ttttwwwttt";
					break;
				case 'K':
					pattern = "wttttttwwt";
					break;
				case 'L':
					pattern = "ttwttttwwt";
					break;
				case 'M':
					pattern = "wtwttttwtt";
					break;
				case 'N':
					pattern = "ttttwttwwt";
					break;
				case 'O':
					pattern = "wtttwttwtt";
					break;
				case 'P':
					pattern = "ttwtwttwtt";
					break;
				case 'Q':
					pattern = "ttttttwwwt";
					break;
				case 'R':
					pattern = "wtttttwwtt";
					break;
				case 'S':
					pattern = "ttwtttwwtt";
					break;
				case 'T':
					pattern = "ttttwtwwtt";
					break;
				case 'U':
					pattern = "wwttttttwt";
					break;
				case 'V':
					pattern = "twwtttttwt";
					break;
				case 'W':
					pattern = "wwwttttttt";
					break;
				case 'X':
					pattern = "twttwtttwt";
					break;
				case 'Y':
					pattern = "wwttwttttt";
					break;
				case 'Z':
					pattern = "twwtwttttt";
					break;
				case '-':
					pattern = "twttttwtwt";
					break;
				case '.':
					pattern = "wwttttwttt";
					break;
				case ' ':
					pattern = "twwtttwttt";
					break;
				case '*':
					pattern = "twttwtwttt";
					break;
				case '$':
					pattern = "twtwtwtttt";
					break;
				case '/':
					pattern = "twtwtttwtt";
					break;
				case '+':
					pattern = "twtttwtwtt";
					break;
				case '%':
					pattern = "tttwtwtwtt";
					break;
				default: break;
			}
			output = output + pattern;
		}
		return output;
	}


	var foreColor = "black";
	var backColor = "white";
	var barWidthRatio = 3;

	var encodedData = EncodeCode39(barcode, false);
	var encodedLength = 0;
	var thinLength = 0;
	var thickLength = 0.0;
	var totalLength = 0.0;
	var swing = 1;
	var result = "";
	var barWidth = 0;
	var thickWidth = 0.0;
	var x;
	for (x = 0; x < encodedData.length; x++) {
		if (encodedData[x] === 't') {
			thinLength++;
			encodedLength++;
		}
		else if (encodedData[x] === 'w') {
			thickLength = thickLength + barWidthRatio;
			encodedLength = encodedLength + 3;
		}
	}
	totalLength = totalLength + thinLength + thickLength;
	barWidth = width / totalLength;
	thickWidth = barWidth * barWidthRatio;

	var l = left;
	for (x = 0; x < encodedData.length; x++) {
		var brush;
		if (swing === 0)
			brush = backColor;
		else
			brush = foreColor;

		if (encodedData[x] === 't') {
			gr.ctx.beginPath();
			gr.ctx.rect(l, top, barWidth, height);
			gr.ctx.fillStyle = brush;
			gr.ctx.fill();
			gr.ctx.closePath();

			l += barWidth;
		}
		else if (encodedData[x] === 'w') {
			gr.ctx.beginPath();
			gr.ctx.rect(l, top, thickWidth, height);
			gr.ctx.fillStyle = brush;
			gr.ctx.fill();
			gr.ctx.closePath();

			l += thickWidth;
		}

		if (swing === 0)
			swing = 1;
		else
			swing = 0;
	}

	return result;
}

// گزارش دفتر روزنامه در سطح معین
function getJournalTotalAccountsPrintImage(transactions, info) {
	//    debugger;
	var rect = info.rect;
	var pageSize = info.pageSize;
	var pageNumber = info.pageNumber;
	var sumDebitBefore = pageNumber > 1 ? info.summation[pageNumber - 1][0] : 0;
	var sumCreditBefore = pageNumber > 1 ? info.summation[pageNumber - 1][1] : 0;
	var sumDebitAfter = info.summation[pageNumber][0];
	var sumCreditAfter = info.summation[pageNumber][1];

	var mm = rect.height / pageSize.height; // page height in mm
	var scale = 3;
	var tempCanvas = document.createElement("canvas");
	document.body.appendChild(tempCanvas);
	tempCanvas.style.display = "none";
	tempCanvas.width = rect.width * scale;
	tempCanvas.height = rect.height * scale;

	var font = "iransans";
	var fontSize = 10;

	var gr = new graphics(tempCanvas);
	gr.scale(scale, scale);
	gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);

	var topMargin = 10;
	var bottomMargin = 10;
	var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
	var n = 0;
	//	var strName = info.business.LegalName === "" ? info.business.Name : info.business.LegalName;
	var rptTitle = "دفتر روزنامه";

	var titleFontSize = fontSize + 2 + "pt";

	gr.drawText(r.left, r.top + 20, r.width, r.height, rptTitle, font, titleFontSize, "gray", "top", "center", true, false, false, true);
	n += 20;

	// table
	n += 45;

	gr.fillRect(r.left, r.top + n, r.width, 50, "#dcdcdc", "1px", 0);
	gr.drawLine(r.left, r.top + n, r.width + 40, r.top + n, "black", "1px", "solid");
	gr.drawLine(r.left, r.top + n + 50, r.width + 40, r.top + n + 50, "black", "1px", "solid");
	//        gr.drawRect(r.left, r.top + n, r.width, 50, "black", "1px", "solid", 0);
	var rowStart = n;
	n += 5;
	// عنوان ستون ها
	gr.drawText(r.width - 5, r.top + n, 40, r.height, "شماره ردیف", font, fontSize + "pt", "black", "top", "center", true, false, false, true);
	gr.drawText(r.width - 50, r.top + n, 40, r.height, "شماره سند", font, fontSize + "pt", "black", "top", "center", true, false, false, true);
	gr.drawText(r.left, r.top + n, r.width - 110, r.height, "تاریخ", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
	gr.drawText(r.left, r.top + n, r.width - 170, r.height, "کد حساب", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
	gr.drawText(r.left, r.top + n, r.width - 280, r.height, "شـــرح", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
	gr.drawText(r.left, r.top + n, r.width - 545, r.height, "بدهکار " + "(" + info.currency + ")", font, fontSize + "pt", "black", "top", "right", true, false, false, true);
	gr.drawText(r.left, r.top + n, r.width - 650, r.height, "بستانکار " + "(" + info.currency + ")", font, fontSize + "pt", "black", "top", "right", true, false, false, true);

	n += 55;

	var rowFontSize = fontSize - 1 + "pt";
	var isBold = false;
	// نقل از صفحه قبل
	gr.fillRect(r.left, r.top + n - 9, r.width, 39, "#dcdcdc", "1px", 0);
	if(pageNumber > 1)
		gr.drawText(r.width - 430, r.top + n, 223, 500, "منقول از صفحه " + (parseInt(pageNumber) - 1), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
	else
		gr.drawText(r.width - 430, r.top + n, 223, 500, "منقول از صفحه", font, rowFontSize, "black", "top", "right", isBold, false, false, true);
	gr.drawText(r.left, r.top + n, r.width - 540, r.height, Hesabfa.money(sumDebitBefore), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
	gr.drawText(r.left, r.top + n, r.width - 650, r.height, Hesabfa.money(sumCreditBefore), font, rowFontSize, "black", "top", "right", isBold, false, false, true);

	gr.drawLine(r.left, r.top + n + (30), r.width + 40, r.top + n + (30), "black", "1px", "solid");
	n += 10 + (30);

	for (var i = 0; i < transactions.length; i++) {
		var trans = transactions[i];
		if (pageSize.name === "A4landscape") { isBold = true; rowFontSize = fontSize - 1 + "pt" }

		info.itemCursor++;

		// فیلد ها
		gr.drawText(r.width - 10, r.top + n, 50, r.height, info.itemCursor, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 60, r.height, Hesabfa.farsiDigit(trans.Number), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 103, r.height, Hesabfa.farsiDigit(trans.DisplayDate), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 185, r.height, Hesabfa.farsiDigit(trans.Code), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
		gr.drawText(r.width - 430, r.top + n, 223, 500, trans.Name, font, rowFontSize, "black", "top", "right", isBold, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 540, r.height, Hesabfa.money(trans.Debit), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 650, r.height, Hesabfa.money(trans.Credit), font, rowFontSize, "black", "top", "right", isBold, false, false, true);

		gr.drawLine(r.left, r.top + n + 30, r.width + 40, r.top + n + 30, "black", "1px", "solid");
		n += 10 + 30;
	}

	// نقل به صفحه بعد
	gr.fillRect(r.left, r.top + n - 9, r.width, 39, "#dcdcdc", "1px", 0);
	gr.drawText(r.width - 430, r.top + n, 223, 500, "نقل به صفحه " + (parseInt(pageNumber) + 1), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
	gr.drawText(r.left, r.top + n, r.width - 540, r.height, Hesabfa.money(sumDebitAfter), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
	gr.drawText(r.left, r.top + n, r.width - 650, r.height, Hesabfa.money(sumCreditAfter), font, rowFontSize, "black", "top", "right", isBold, false, false, true);

	gr.drawLine(r.left, r.top + n + (30), r.width + 40, r.top + n + (30), "black", "1px", "solid");
	n += 10 + (30);

	// خطوط عمودی گزارش
	gr.drawLine(r.width + 40, r.top + rowStart, r.width + 40, r.top + n - 10, "black", "1px", "solid"); // شماره ردیف
	gr.drawLine(r.width - 10, r.top + rowStart, r.width - 10, r.top + n - 10, "black", "1px", "solid"); // شماره سند
	gr.drawLine(r.width - 55, r.top + rowStart, r.width - 55, r.top + n - 10, "black", "1px", "solid"); // تاریخ
	gr.drawLine(r.width - 130, r.top + rowStart, r.width - 130, r.top + n - 10, "black", "1px", "solid"); //کد حساب
	gr.drawLine(r.width - 200, r.top + rowStart, r.width - 200, r.top + n - 10, "black", "1px", "solid"); //کد حساب
	gr.drawLine(r.width - 490, r.top + rowStart, r.width - 490, r.top + n - 10, "black", "1px", "solid"); // شرح
	gr.drawLine(r.width - 600, r.top + rowStart, r.width - 600, r.top + n - 10, "black", "1px", "solid"); // بدهكار

	gr.drawLine(r.left, r.top + rowStart, r.left, r.top + n - 10, "black", "1px", "solid");

	// حساب فا
	if (n <= r.height - 10)
		gr.drawText(r.left, r.height - 10, r.width, r.height, "حساب فا - www.hesabfa.com", font, "10pt", "silver", "top", "center", true, false, false, true);

	if (pageNumber > 1 || !info.done)
		gr.drawText(r.left, r.height, r.width, 35, "صفحه " + pageNumber, font, fontSize + "pt", "black", "top", "left", true, false, false, true);

	tempCanvas.ctx = gr.ctx;
	return tempCanvas;
}
function printJournalTotalAccounts(transactions, summation, rpp, pageCount, business, currency) {
	var pageSize = {};
	pageSize.name = "A4portrait";
	pageSize.width = 210;
	pageSize.height = 297;

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	var wnd = window.open("");
	var printImg = [];
	var printDiv = [];

	var info = {
		rect: rect,
		pageSize: pageSize,
		pageNumber: 1,
		done: false,
		itemCursor: 0,
		business: business,
		currency: currency,
		pageCount: pageCount,
		rpp: rpp,
		summation: summation
	};

	for (var pageNumber = 0; pageNumber < pageCount; pageNumber++) {
		var pageTransactions;
		if (pageNumber < pageCount - 1)
			pageTransactions = transactions.slice(pageNumber * rpp, (pageNumber + 1) * rpp);
		else
			pageTransactions = transactions.slice(pageNumber * rpp);

		var canvas = getJournalTotalAccountsPrintImage(pageTransactions, info);
		var n = info.pageNumber - 1;

		printImg[n] = wnd.document.createElement("img");
		printImg[n].width = rect.width;
		printImg[n].height = rect.height;

		printImg[n].src = canvas.toDataURL("image/png");
		wnd.document.body.appendChild(printImg[n]);

		printDiv[n] = wnd.document.createElement("div");
		$(printDiv[n]).css("page-break-after", "always");
		wnd.document.body.appendChild(printDiv[n]);

		document.body.removeChild(canvas);
		delete canvas;
		info.pageNumber++;
	}

	wnd.document.title = "چاپ دفتر روزنامه";
	setTimeout(function () {
		wnd.print();
	}, 1000);
};
function pdfJournalTotalAccounts(transactions, summation, rpp, pageCount, business, currency) {
	var pageSize = {};
	pageSize.name = "A4portrait";
	pageSize.width = 210;
	pageSize.height = 297;

	var pageWidthInMM = pageSize.width;
	var pageHeightInMM = pageSize.height;
	var rect = { left: 0, top: 0, width: Math.round(pageWidthInMM * 3.9381), height: Math.round(pageHeightInMM * 3.9381) };

	var pdf = new jsPDF("p", "mm", [pageWidthInMM, pageHeightInMM]);

	var info = {
		rect: rect,
		pageSize: pageSize,
		pageNumber: 1,
		done: false,
		itemCursor: 0,
		business: business,
		currency: currency,
		pageCount: pageCount,
		rpp: rpp,
		summation: summation
	};

	for (var pageNumber = 0; pageNumber < pageCount; pageNumber++) {
		var pageTransactions;
		if (pageNumber < pageCount - 1)
			pageTransactions = transactions.slice(pageNumber * rpp, (pageNumber + 1) * rpp);
		else
			pageTransactions = transactions.slice(pageNumber * rpp);

		var canvas = getJournalTotalAccountsPrintImage(pageTransactions, info);
		var n = info.pageNumber - 1;

		pdf.addRawImage(canvas.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, pageWidthInMM, pageHeightInMM);
		document.body.removeChild(canvas);
		delete canvas;
		info.pageNumber++;
	}
	pdf.save("journal.pdf");
};

function isChrome() {
	var isChromium = window.chrome;
	var winNav = window.navigator;
	var vendorName = winNav.vendor;
	var isOpera = typeof window.opr !== "undefined";
	var isIEedge = winNav.userAgent.indexOf("Edge") > -1;
	var isIOSChrome = winNav.userAgent.match("CriOS");

	if (isIOSChrome) {
		// is Google Chrome on IOS
	} else if (
	  isChromium !== null &&
	  typeof isChromium !== "undefined" &&
	  vendorName === "Google Inc." &&
	  isOpera === false &&
	  isIEedge === false
	) {
		return true;
	} else {
		return false;
	}
}