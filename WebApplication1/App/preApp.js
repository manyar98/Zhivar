  var Hesabfa = {};
    (function initHesabfa() {
	Hesabfa.jsVersion = "3.1";
	Hesabfa.orgTypes = ['شرکت', 'مغازه', 'فروشگاه', 'اتحادیه', 'کلوب', 'موسسه', 'شخصی'];
	Hesabfa.lineOfBusiness = ['شرکت', 'مغازه', 'فروشگاه', 'اتحادیه', 'کلوب', 'موسسه', 'شخصی'];
	Hesabfa.currencies = ['ریال', 'تومان', 'افغانی', 'سامانی', 'سوم', 'دلار', 'درهم', 'یورو', 'یوان', 'پوند',
    'لیر', 'لاری', 'رینگیت', 'ین', 'روپیه', 'دینار', 'وون'];
	Hesabfa.calendars = ["شمسی", "میلادی"];
	Hesabfa.contactTypes = ["نامشخص", "حقیقی", "حقوقی"];
	Hesabfa.dayNumbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31];
	Hesabfa.monthNumbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
	Hesabfa.seasons = ["بهار", "تابستان", "پاییز", "زمستان"];
	Hesabfa.monthNames = ["فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"];
	Hesabfa.yearNumbers = ['1392', '1393', '1394', '1395', '1396'];
	Hesabfa.FinanAccTypeCategories = ['دارایی', 'بدهی', 'سرمایه', 'درآمد', 'مخارج', 'هیچکدام'];
	Hesabfa.InvoiceStatusSale = ["ذخیره موقت", "منتظر تایید", "منتظر دریافت", "دریافت شده"];
	Hesabfa.InvoiceStatus = ["ذخیره موقت", "منتظر تایید", "منتظر پرداخت", "پرداخت شده"];
	Hesabfa.ChequeStatus = ["عادی", "در جریان وصول", "وصول شده", "برگشت خورده", "عودت شده", "خرج شده"];
	Hesabfa.ChequeStatusPaid = ["عادی", "در جریان وصول", "پاس شده", "برگشت خورده", "عودت شده"];
	Hesabfa.DefaultProfileImage = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEYAAABGCAIAAAD+THXTAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAABQ9JREFUeNrsmlkodV8Yxo95yFiizDKFC9Mx5MIQIVOGUOa4QGYXLhBlKkXGEhlCkjJFipO4cGEeSoZSpihjyDFk/D9ZtZPv38nhbPb3td+rZ5+z3332b6+13v28C7GbmxvOvxXinH8uWCQWiUVikVgkFolFYpFYJBbpLwhJun/g4eFhaWlpY2Pj7OwMh2pqamZmZra2tlJSUn8f0ubmZk1NTX9/P5/P//CVgoJCSEhIRkaGqampyH9XjI4W8O7uLj8/v6mp6eXlRcBpEhISiYmJxcXFsrKyjEY6OjoKDAxcXV2lPjE0NHRwcNDU1IQ+PDycnZ3d3t6mvrW0tBwYGNDQ0GAo0vn5ubu7+9bWFjn08vLKzc3lcrkfTpubmystLR0fHyeHJiYmExMTqqqqjEN6fX0NCAjAzUFj9dfW1sbExAg4v7W1NSsr6+npCdrT0xNjxbgi3tnZSXjExcU7OjoE8yDi4+Pb29vFxMSgeTxed3c3s5DwsMvKyojOzs7GcH0mC6sOdY9o1AkyYkyZeCMjI2FhYRBY6Gtra3Jycp9MvL29NTc3Pz09he7r6/P29mbKKAGJiNjY2M/zIOTl5akpSl2EEUgLCwtUlRM2l0qZn59nENLe3h4R8DvC5mLiEbG7u8sgJCwJIhQVFYXNpVJEsrBFhoQlQcT19bWwuVQKdRFGIOnp6RGxvr4ubC4q5IeLMALJzs6OiNHRUWFzx8bGiLC3t2cQkq+vLxHwDXDin0/E+kEK0f7+/gxCgknT1dWFODk5oWzEZ6KkpIR0hwYGBh4eHgxCkpSURI9EdFVV1eDg4GeyYBfq6uqILigoQAfFLNsaERFBHjMsOTxES0uL4PPRI8K54mRoPz8/4qeYhQRP3dbWhuaHuNj09HSYVzR8f545PT0NBqqzQCLaW+Z2tcfHx8HBwSsrK9Qn+vr66Gq1tbWhDw4OZmZmKKvxPoKCgsrLy7W0tJi493B/f19YWNjQ0PD8/Cx4+dnY2MDXkblHbERNTU14ePhvTjzcNPqcuLi49w9eVlYWzxtGFp8rKSn9maWsrJyQkLC4uDg5Obm8vAwG0gjCRmB15eTkCN6HoXGUHh8fo6Ojh4eHoaOiohobG/+3NcRNw1KgI8KNqqurW1hYWFlZfdjHw2xMSkqiNi1CQ0Obm5sxjD+KhNmCQejt7SWHeBdR/emXjS+oUNbJYWRkJEriF64jkZeX97U7AAM1LJmZmbm5ud+cwxg39O1XV1eka1pdXZWRkXFycvqhUZqamvLx8SEzPjk5uaKiQoTVJS0trbW1lfO2d8nj8RwdHWlHQkHjcrk7OzvQbm5uQ0ND4uKifL9h+eGthacGbWxsPDc3Jy0tTW/Fq6+vJzyqqqqwCKLlIcUdl0VVhEbBwMuA3iLO5/Orq6uJLioqEuHG7/vACxeWj+jKykqhppLQSF1dXRcXFxBGRkYwchzaAi8u2A7O26Z0T08PjUhUbwMLJxLjLKAApqSkEN3e3k5Xedjd3cWLEkJOTg5aQUGBQ2dcXl6iiXp4eOC8/bVKR0dH9KNEtrwRrq6udPMgVFRUnJ2diYZ1omXiUVuHLi4unB8JPDsiUMppQdrY2CACJu1nkKytrYXdeBIOaX9/nwhM8Z9Bon4IjRYt5QHvVrJYfz5QkMiui4hH6TttzPddEi0TDy+Kr/Uw37dIqampv9mo/26w/0PEIrFILBKLxCKxSCwSi8Qi/QXxnwADAAf6KVtY/NneAAAAAElFTkSuQmCC";
	Hesabfa.DefaultBusinessLogo = "data:image/png;base64,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";
	Hesabfa.AccountLevel = ["گروه", "کل", "معین", "تفصیلی"];
	Hesabfa.moneyUnit = "";
	Hesabfa.accessDeniedString = "شما مجوز لازم برای دسترسی به این قسمت یا انجام عملیات مورد نظر را ندارید.";
	Hesabfa.servicePlans = ['آزمایشی', 'طرح یک - ۵۰ سند در ماه', 'طرح دو - ۱۵۰ سند در ماه', 'طرح سه - ۳۰۰ سند در ماه', 'طرح چهار - بدون محدودیت'];
	Hesabfa.pagePreTitle = "ژیور";
	Hesabfa.comboContactTemplate = "<div class='hesabfa-combobox-title-blue'> {DetailAccount.Code} &nbsp;-&nbsp; {Name} </div>" +
        "<div class='small'> {DetailAccount.Node.FamilyTree} </div>";
    Hesabfa.comboContactTemplate2 = "<div class='hesabfa-combobox-title-blue'> {DetailAccount.Code} &nbsp;-&nbsp; {Title}</div>" +
        "<div class='small'> {DetailAccount.Node.FamilyTree} </div>";
	Hesabfa.comboItemTemplate = "<div class='hesabfa-combobox-title-blue' style='overflow:hidden;white-space: nowrap;'> {DetailAccount.Code} &nbsp;-&nbsp; {Name} &nbsp;&rlm;<span class='orange'>({Stock})</span></div>" +
        "<div class='small'> {DetailAccount.Node.FamilyTree} </div>";
    Hesabfa.comboSazeTemplate = "<div class='hesabfa-combobox-title-blue' style='overflow:hidden;white-space: nowrap;'> {DetailAccount.Code} &nbsp;-&nbsp; {Title} &nbsp;&rlm;<span class='orange'>(نوع رسانه: {NoeSazeName}, نوع اجاره: {NoeEjareName})</span></div>" +
        "<div class='small'> {DetailAccount.Node.FamilyTree} </div>";
    Hesabfa.comboGroupSazeTemplate = "<div class='hesabfa-combobox-title-blue' style='overflow:hidden;white-space: nowrap;'>  {Title} &nbsp;&rlm;</div>";
	Hesabfa.comboBankTemplate = "<div class='hesabfa-combobox-title-blue'> {Code} &nbsp;-&nbsp; {Name} </div>" +
        "<div class='small'> {Branch} &nbsp;-&nbsp; {AccountNumber} </div>";
    Hesabfa.comboPersonalTemplate = "<div class='hesabfa-combobox-title-blue'>  &nbsp; {Name} </div>" +
        "<div class='small'> سمت &nbsp;:&nbsp; {RoleName} </div>";
    Hesabfa.comboNoeEjareTemplate = "<div class='hesabfa-combobox-title-blue'>  &nbsp; {Title} </div>";
    
	Hesabfa.comboCashTemplate = "<div> {Code} &nbsp;-&nbsp; {Name} </div>";
	Hesabfa.money = function (value) {
		if (value == undefined) return "";
		value = Math.round(value * 100) / 100;
		return Hesabfa.farsiDigit(formatToCurrency(value));
	};
	Hesabfa.getBankLogoClass = function (bankName,size) {
		if (!bankName) return "";
		var pre = "";
		if (size)
			pre = "ibl" + size + " ";
		if (bankName.includes("تجارت")) return pre + 'ibl-tejarat';
		else if (bankName.includes("ملت")) return pre + 'ibl-mellat';
		else if (bankName.includes("توسعه صادرات")) return pre + 'ibl-edbi';
		else if (bankName.includes("صادرات")) return pre + 'ibl-bsi';
		else if (bankName.includes("ملی") || bankName.includes("ملي")) return pre + 'ibl-bmi';
		else if (bankName.includes("سپه")) return pre + 'ibl-sepah';
		else if (bankName.includes("کشاورزی")) return pre + 'ibl-bki';
		else if (bankName.includes("پارسیان")) return pre + 'ibl-parsian';
		else if (bankName.includes("مسکن")) return pre + 'ibl-maskan';
		else if (bankName.includes("رفاه")) return pre + 'ibl-rb';
		else if (bankName.includes("اقتصاد نوین")) return pre + 'ibl-en';
		else if (bankName.includes("انصار")) return pre + 'ibl-ansar';
		else if (bankName.includes("پاسارگاد")) return pre + 'ibl-bpi';
		else if (bankName.includes("سامان")) return pre + 'ibl-sb';
		else if (bankName.includes("سینا")) return pre + 'ibl-sina';
		else if (bankName.includes("پست")) return pre + 'ibl-post';
		else if (bankName.includes("قوامین")) return pre + 'ibl-ghbi';
		else if (bankName.includes("توسعه تعاون")) return pre + 'ibl-tt';
		else if (bankName.includes("شهر")) return pre + 'ibl-shahr';
		else if (bankName.includes("آینده")) return pre + 'ibl-ba';
		else if (bankName.includes("سرمایه")) return pre + 'ibl-sarmayeh';
		else if (bankName.includes("دی")) return pre + 'ibl-day';
		else if (bankName.includes("حکمت")) return pre + 'ibl-hi';
		else if (bankName.includes("کارآفرین")) return pre + 'ibl-kar';
		else if (bankName.includes("صنعت")) return pre + 'ibl-bim';
		else if (bankName.includes("خاورمیانه")) return pre + 'ibl-me';
		else if (bankName.includes("ونزوئلا")) return pre + 'ibl-ivbb';
		else {
			if (size === 64)
				return "fa fa-bank fa-3x";
			else return "fa fa-bank";
		}
	};
	//    this.ss = "makeNumbersFarsi = function(text){return Hesabfa.farsiDigit(text);}"
	Hesabfa.farsiDigit = function (value) {

		if (value == undefined)
			return "";
		if (window.calendarType === 1) return value + "";

		value += "";
		value = value.replace(/0/g, "۰");
		value = value.replace(/1/g, "۱");
		value = value.replace(/2/g, "۲");
		value = value.replace(/3/g, "۳");
		value = value.replace(/4/g, "۴");
		value = value.replace(/5/g, "۵");
		value = value.replace(/6/g, "۶");
		value = value.replace(/7/g, "۷");
		value = value.replace(/8/g, "۸");
		value = value.replace(/9/g, "۹");
		return value;
	};
	Hesabfa.englishDigit = function (value) {
		if (value == undefined)
			return "";
		value += "";
		value = value.replace(/۰/g, "0");
		value = value.replace(/۱/g, "1");
		value = value.replace(/۲/g, "2");
		value = value.replace(/۳/g, "3");
		value = value.replace(/۴/g, "4");
		value = value.replace(/۵/g, "5");
		value = value.replace(/۶/g, "6");
		value = value.replace(/۷/g, "7");
		value = value.replace(/۸/g, "8");
		value = value.replace(/۹/g, "9");
		return value;
	};
	Hesabfa.isDecimalCurrency = function () {
		var currency = window.currency;
		if (currency === "دلار" || currency === "یورو"
			|| currency === "پوند" || currency === "یوان"
			|| currency === "درهم" || currency === "لیر"
			|| currency === "لاری" || currency === "رینگیت"
			|| currency === "روپیه"
			|| currency === "دینار" || currency === "وون") return true;
		return false;
	};
	Hesabfa.newInvoiceItemObj = function () {
		var invoiceItem = {
			Description: "",
			Quantity: 0,
			Unit: "",
			UnitPrice: 0,
			Sum: 0,
			TotalAmount: 0,
			Discount: 0,
			Tax: 0,
			RowNumber: 0
		};
		return invoiceItem;
    };
    Hesabfa.newContractSazeItemObj = function () {

        var invoiceItem = {
            CalcTax: false,
            Contarct_Saze_Bazareabs: null,
            ContractID: 0,
            Contract_Saze_Chapkhanes: null,
            Contract_Saze_Nasabs: null,
            Contract_Saze_Tarahs: null,
            Description: "",
            Discount: 0,
            DisplayTarikhShorou: "",
            HasBazareab: false,
            HasChap: false,
            HasNasab: false,
            HasTarah: false,
            ID: 0,
            ItemInput: "",
            Mah: 0,
            //NoeEjare: {},
            NoeEjareId: 0,
            PriceBazareab: 0,
            PriceChap: 0,
            PriceNasab: 0,
            PriceTarah: 0,
            Quantity: 0,
            RowNumber: 0,
            //Saze: {},
            SazeId: 0,
            Status: 0,
            Sum: 0,
            TarikhShorou: "",
            Tax: 0,
            TotalAmount: 0,
            UnitItem: 0,
            UnitPrice: "",
            calcTax: false
        };
        return invoiceItem;
    };
    Hesabfa.newCostItemObj = function () {
        var costItem = {
            Description: "",
            Title: "",
            Sum: 0,
            Rest: 0,
            RowNumber: 0
        };
        return costItem;
    };
	Hesabfa.newContactObj = function () {
		var c = {
			Name: "",
			ContactType: 0,
			NationalCode: "",
			EconomicCode: "",
			RegistrationNumber: "",
			FirstName: "",
			LastName: "",
			Email: "",
			People: "",
			Address: "",
			City: "",
			State: "",
			PostalCode: "",
			Phone: "",
			Fax: "",
			Mobile: "",
			ContactEmail: "",
			Website: "",
			Rating: 0,
			IsCustomer: false,
			IsVendor: false,
			IsEmployee: false,
			IsShareHolder: false,
			Liability: 0,
			Credits: 0,
			Note: "",
			HesabfaKey: 0,
			SharePercent: 0,
			DetailAccount: null,
			FinanYear: null
		}
		return c;
	};
	Hesabfa.newItemObj = function () {
		var item = {
			Name: "",
			ItemType: 0,
			Unit: "",
			Barcode: "",
			PurchasesTitle: "",
			SalesTitle: "",
			Stock: 0,
			MinStock: 0,
			BuyPrice: 0,
			SellPrice: 0,
			WeightedAveragePrice: 0,
			DetailAccount: null,
			FinanYear: null
		};
		return item;
	};
	Hesabfa.newTransactionObj = function () {
		return {
			AccDocument: null,
			Account: null,
			DetailAccount: null,
			Description: "",
			Amount: 0,
			Type: 1,
			Reference: "",
			RowNumber: 0,
			Stock: 0,
			PaymentMethod: 0,
			Contact: null,
			Invoice: null,
			Cheque: null,
			Debit: 0,
			Credit: 0
		};
	}
	Hesabfa.newReceiveAndPay = function () {
		return {
			Id: 0,
			FinanYear: null,
			Invoice: null,
			Document: null,
			IsReceive: true,
			Number: 0,
			DisplayDate: "",
			Type: 1,
			Contact: null,
			Account: null,
			DetailAccount: null,
			Description: "",
			Amount: 0,
			Items: [],
			PayItems: []
		};
	}
	Hesabfa.newReceiveAndPayItem = function (item) {
		if (!item)
			item = {};
		return {
			Id: item.Id || 0,
			ReceiveAndPay: item.ReceiveAndPay || null,
			Type: item.Type || 0,
			Amount: item.Amount || 0,
			Cash: item.Cash || null,
			Bank: item.Bank || null,
			Reference: item.Reference || "",
			Cheque: item.Cheque || null,
			ChequeBank: item.ChequeBank || null,
		};
	}
})();

    function applyScope($scope) {
	if (!$scope.$$phase) {
		$scope.$apply();
	}
}