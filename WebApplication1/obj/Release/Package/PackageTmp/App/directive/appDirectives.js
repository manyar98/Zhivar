function convertNumbersToAdaptive(x, digitGroup) {
	if (typeof x == "undefined" || x == null)
		x = "";
	x = x + "";
	if (digitGroup)
		x = x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
	return Hesabfa.farsiDigit(x);
}
function convertNumbersToActual(x, digitGroup) {
	if (typeof x == "undefined" || x == null)
		x = "";
	x = x + "";
	if (digitGroup)
		x = x.replace(/\,/g, '').replace(/\،/g, '');
	return Hesabfa.englishDigit(x);
}
//app.filter('number', [function () {
//	return function (value) {
//		return Hesabfa.farsiDigit(value);
//	}
//}]);
//app.filter('digitGroup', [function () {
//	return function (value) {
//		return Hesabfa.money(value);
//	}
//}]);
//app.filter('trustAsHtml', ['$sce', function ($sce) {
//	return function (text) {
//		return $sce.trustAsHtml(text);
//	};
//}]);


