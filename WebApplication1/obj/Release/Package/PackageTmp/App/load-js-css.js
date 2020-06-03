var isDevelopment = true, reloadStatus = false;
var version = "1";//getVersion("/app/api/AuthenticateUrl/GetVersion?t=" + Math.random());

var cookie = getCookie();
if (cookie != "" && cookie != version) {
    var agent = navigator.userAgent;
    if (agent.includes("Windows"))
        reloadStatus = true;
}
setCookie(version);

loadcssfile("/Content/General.css", "css");
loadcssfile("/Content/UI-View-Effect.css", "css");
loadcssfile("/Content/leaflet/leaflet.css", "css");
loadcssfile("Scripts/core/require.js", "js");

function setCookie(cvalue) {
    var d = new Date();
    d.setTime(d.getTime() + (11111 * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = "ver-cache-prj" + "=" + cvalue + "; " + expires;
}

function getCookie() {
    var name = "ver-cache-prj" + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}

function loadcssfile(filename, filetype) {
    if (filetype == "js") {
        var fileref = document.createElement("script")
        fileref.setAttribute("data-main", "App/main.js")
        fileref.setAttribute("src", filename + "?v=" + version)
    }
    if (filetype == "css") {
        var fileref = document.createElement("link")
        fileref.setAttribute("rel", "stylesheet")
        fileref.setAttribute("type", "text/css")
        fileref.setAttribute("href", filename + "?v=" + version)
    }
    if (typeof fileref != "undefined")
        document.getElementsByTagName("head")[0].appendChild(fileref)
}

function getVersion(url) {
    var req = createXMLHTTPObject();
    if (!req) return;
    var method = "GET";
    req.open(method, url, false);
    req.setRequestHeader('User-Agent', 'XMLHTTP/1.0');
    if (req.readyState == 4) return;
    req.send();
    return req.responseText;
}

function createXMLHTTPObject() {
    var XMLHttpFactories = [
        function () { return new XMLHttpRequest() },
        function () { return new ActiveXObject("Msxml2.XMLHTTP") },
        function () { return new ActiveXObject("Msxml3.XMLHTTP") },
        function () { return new ActiveXObject("Microsoft.XMLHTTP") }
    ];

    var xmlhttp = false;
    for (var i = 0; i < XMLHttpFactories.length; i++) {
        try {
            xmlhttp = XMLHttpFactories[i]();
        }
        catch (e) {
            continue;
        }
        break;
    }
    return xmlhttp;
}