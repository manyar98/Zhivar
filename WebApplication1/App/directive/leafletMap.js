define(['application', 'leaflet', 'geocoder'], function (app) {
    app.register.directive('leafletMap', ['$rootScope', "$window", "$parse", 'messageService', '$timeout', '$q', function ($rootScope, $window, $parse, messageService, $timeout, $q) {

        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
        }

        function guidGenerate() {
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
        }

        function controller($scope, $element, $attrs) {
            this.guidGenerate = function () {
                $scope.id = guidGenerate();
            }
        }

        function isValidCenter(center) {
            return angular.isDefined(center) && angular.isNumber(center.lat) &&
                angular.isNumber(center.lng) && angular.isNumber(center.zoom);
        }

        function link(iScope, iElem, iAttrs, controller) {
            controller.guidGenerate();
            var template = $("<div class='leaflet-iran-map'></div>");
            var width = (iElem.outerWidth() * $(window).innerWidth()) / 100;
            iScope.htmlOptions = iScope.htmlOptions || { class: "", tabindex: "", style: "", width: "1290px", height: "320px" }, iScope.kOptions = iScope.kOptions || {}; //style: "width: " + width + "px; height: 100%;"
            if (iScope.htmlOptions.tabindex)
                template.attr('tabindex', iScope.htmlOptions.tabindex);

            template.attr('id', iScope.id).attr('style', iScope.htmlOptions.style).addClass(iScope.htmlOptions.class);

            if (iScope.htmlOptions.width) {
                iScope.$watch("htmlOptions.width", function (newValue) {
                    if (angular.isUndefined(newValue))
                        return;
                    updateWidth(newValue);
                    map.invalidateSize();
                });
            }

            if (iScope.htmlOptions.height) {
                iScope.$watch("htmlOptions.height", function (newValue) {
                    if (angular.isUndefined(newValue))
                        return;
                    updateHeight(newValue);
                    map.invalidateSize();
                });
            }

            iElem.html(template);

            var temp = {
                tileLayer: "//{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", //'http://{s}.tile.opencyclemap.org/cycle/{z}/{x}/{y}.png',
                tileLayerOptions: {
                    opacity: 0.9,
                    detectRetina: true,
                    reuseTiles: true
                },
                center: iScope.kOptions.center || {
                    lat: 32.757552763570196,
                    lng: 53.41000747680664,
                    zoom: 5
                },
                zoomControlPosition: iScope.kOptions.zoomControlPosition || "topleft",
                currentMarker: null,
                markers: [],
                success: "موقعیت جغرافیایی یافت شد.",
                notFoundMsg: "موقعیت جغرافیایی یافت نشد!",
                outOfZoneMsg: "موقعیت جغرافیایی در کشور ایران نمی باشد!"
            }

            //initilizing map into div#id
            var map = L.map(iScope.id, {
                attributionControl: angular.isDefined(iScope.kOptions.attributionControl) ? iScope.kOptions.attributionControl : false,
                zoomControl: angular.isDefined(iScope.kOptions.zoomControl) ? iScope.kOptions.zoomControl : true,
                closePopupOnClick: angular.isDefined(iScope.kOptions.closePopupOnClick) ? iScope.kOptions.closePopupOnClick : true,
                dragging: angular.isDefined(iScope.kOptions.dragging) ? iScope.kOptions.dragging : true,
                doubleClickZoom: angular.isDefined(iScope.kOptions.doubleClickZoom) ? iScope.kOptions.doubleClickZoom : true,
                tap: true,
                touchZoom: true,
                //center: [51.505, -0.09],
                worldCopyJump: false,
                zoom: 13,
                minZoom: angular.isDefined(iScope.kOptions.minZoom) ? iScope.kOptions.minZoom : 5, //"*"
                maxZoom: angular.isDefined(iScope.kOptions.maxZoom) ? iScope.kOptions.maxZoom : 20, //"*"
                zoomAnimation: true,
                fadeAnimation: true,
                markerZoomAnimation: true,
                scrollWheelZoom: angular.isDefined(iScope.kOptions.scrollWheelZoom) ? iScope.kOptions.scrollWheelZoom : true, //'center'
                wheelPxPerZoomLevel: angular.isDefined(iScope.kOptions.wheelPxPerZoomLevel) ? iScope.kOptions.wheelPxPerZoomLevel : 60,
                renderer: L.canvas({ padding: 0.5 }),
                //crs: {
                //    Simple: {
                //        projection: {},
                //        transformation: {
                //            _a: 1,
                //            _b: 0,
                //            _c: -1,
                //            _d: 0
                //        }
                //    },
                //    code: "EPSG:3857",
                //    projection: {
                //        MAX_LATITUDE: 85.0511287798
                //    },
                //    transformation: {
                //        _a: 0.15915494309189534,
                //        _b: 0.5,
                //        _c: -0.15915494309189534,
                //        _d: 0.5
                //    }
                //}
            }), geocoderControl = new L.Control.Geocoder({ geocoder: null, defaultMarkGeocode: false });

            /*********************** map events ***********************/
            map.on('load', load);
            map.on('unload', unload);
            map.on('zoomlevelschange', zoomLevelsChange);
            map.on('viewreset', viewReset);
            map.on('zoomstart', zoomStart);
            map.on('movestart', moveStart);
            map.on('zoom', zoom);
            map.on('move', move);
            map.on('zoomend', zoomEnd);
            map.on('moveend', moveEnd);
            map.on('popupopen', popupOpen);
            map.on('popupclose', popupClose);
            map.on('autopanstart', autopanStart);
            map.on('tooltipopen', tooltipOpen);
            map.on('tooltipclose', tooltipClose);
            map.on('locationerror', locationError);
            map.on('locationfound', locationFound);
            map.on('click', click);
            map.on('dblclick', dblclick);
            map.on('mousedown', mouseDown);
            map.on('mouseup', mouseUp);
            map.on('mouseover', mouseOver);
            map.on('mouseout', mouseOut);
            map.on('mousemove', mouseMove);
            map.on('contextmenu', contextMenu);
            map.on('keypress', keypress);
            map.on('preclick', preclick);

            function load(e) {
                //Fired when the map is initialized (when its center and zoom are set for the first time).
                typeof (iScope.kOptions.load) == "function" && iScope.kOptions.load.call(this, e);
            }

            function unload(e) {
                //Fired when the map is destroyed with remove method.
                typeof (iScope.kOptions.unload) == "function" && iScope.kOptions.unload.call(this, e);
            }

            function zoomLevelsChange(e) {
                //Fired when the number of zoomlevels on the map is changed due to adding or removing a layer.
                typeof (iScope.kOptions.zoomLevelsChange) == "function" && iScope.kOptions.zoomLevelsChange.call(this, e);
            }

            function viewReset(e) {
                //Fired when the map needs to redraw its content (this usually happens on map zoom or load). Very useful for creating custom overlays.
                typeof (iScope.kOptions.viewReset) == "function" && iScope.kOptions.viewReset.call(this, e);
            }

            function zoomStart(e) {
                //Fired when the map zoom is about to change (e.g. before zoom animation).
                typeof (iScope.kOptions.zoomStart) == "function" && iScope.kOptions.zoomStart.call(this, e);
            }

            function moveStart(e) {
                //Fired when the view of the map starts changing (e.g. user starts dragging the map).
                typeof (iScope.kOptions.moveStart) == "function" && iScope.kOptions.moveStart.call(this, e);
            }

            function zoom(e) {
                //Fired repeatedly during any change in zoom level, including zoom and fly animations.
                typeof (iScope.kOptions.zoom) == "function" && iScope.kOptions.zoom.call(this, e);
            }

            function move(e) {
                //Fired repeatedly during any movement of the map, including pan and fly animations.
                typeof (iScope.kOptions.move) == "function" && iScope.kOptions.move.call(this, e);
            }

            function zoomEnd(e) {
                //Fired when the map has changed, after any animations.
                typeof (iScope.kOptions.zoomEnd) == "function" && iScope.kOptions.zoomEnd.call(this, e);
            }

            function moveEnd(e) {
                //Fired when the center of the map stops changing (e.g. user stopped dragging the map).
                typeof (iScope.kOptions.moveEnd) == "function" && iScope.kOptions.moveEnd.call(this, e);
            }

            function popupOpen(e) {
                //Fired when a popup is opened in the map
                typeof (iScope.kOptions.popupOpen) == "function" && iScope.kOptions.popupOpen.call(this, e);
            }

            function popupClose(e) {
                //Fired when a popup is closed in the map
                typeof (iScope.kOptions.popupClose) == "function" && iScope.kOptions.popupClose.call(this, e);
            }

            function autopanStart(e) {
                //Fired when the map starts autopanning when opening a popup.
                typeof (iScope.kOptions.autopanStart) == "function" && iScope.kOptions.autopanStart.call(this, e);
            }

            function tooltipOpen(e) {
                //Fired when a tooltip is opened in the map.
                typeof (iScope.kOptions.tooltipOpen) == "function" && iScope.kOptions.tooltipOpen.call(this, e);
            }

            function tooltipClose(e) {
                //Fired when a tooltip in the map is closed.
                typeof (iScope.kOptions.tooltipClose) == "function" && iScope.kOptions.tooltipClose.call(this, e);
            }

            function locationError(e) {
                //e.message , e.code
                //Fired when geolocation (using the locate method) failed.
                typeof (iScope.kOptions.locationError) == "function" && iScope.kOptions.locationError.call(this, e, e.message, e.code);
            }

            function locationFound(e) {
                //e.latlng
                typeof (iScope.kOptions.locationFound) == "function" && iScope.kOptions.locationFound.call(this, e);
            }

            function click(e) {//e: MouseEvent in this case
                if (!iScope.kOptions.marker)
                    return;
                //reverse: function(location, scale, callback, context)
                geocoderControl.options.geocoder.reverse(e.latlng, map.options.crs.scale(map.getZoom()), function (results) {
                    var result = results[0];
                    if (!result) {
                        var code = 10; var msg = createErrorMsgByCode(code);
                        createLocationError(e, code, msg, "click");
                        messageService.error(msg, "خطا");
                    }
                    else if (result.properties.address.country.toLowerCase() != "iran" && result.properties.address.country.toLowerCase() != "ایران") {
                        var code = 11; var msg = createErrorMsgByCode(code);
                        createLocationError(e, code, msg, "click");
                        messageService.error(msg, "خطا");
                    }
                    else {
                        var marker = setDefaultMarker({ latlng: e.latlng });
                        typeof (iScope.kOptions.click) == "function" && iScope.kOptions.click.call(this, e, marker, temp.markers);
                    }
                });
            }

            function dblclick(e) {
                //e.latlng
                typeof (iScope.kOptions.dblclick) == "function" && iScope.kOptions.dblclick.call(this, e);
            }

            function mouseDown(e) {
                //e.latlng
                typeof (iScope.kOptions.mouseDown) == "function" && iScope.kOptions.mouseDown.call(this, e);
            }

            function mouseUp(e) {
                //e.latlng
                typeof (iScope.kOptions.mouseUp) == "function" && iScope.kOptions.mouseUp.call(this, e);
            }

            function mouseOver(e) {
                //e.latlng
                typeof (iScope.kOptions.mouseOver) == "function" && iScope.kOptions.mouseOver.call(this, e);
            }

            function mouseOut(e) {
                //e.latlng
                typeof (iScope.kOptions.mouseOut) == "function" && iScope.kOptions.mouseOut.call(this, e);
            }

            function mouseMove(e) {
                //e.latlng
                typeof (iScope.kOptions.mouseMove) == "function" && iScope.kOptions.mouseMove.call(this, e);
            }

            function contextMenu(e) {
                //e.latlng
                //Fired when the user pushes the right mouse button on the map, 
                //prevents default browser context menu from showing if there are listeners on this event.
                //Also fired on mobile when the user holds a single touch for a second(also called long press).
                typeof (iScope.kOptions.contextMenu) == "function" && iScope.kOptions.contextMenu.call(this, e);
            }

            function keypress(e) {
                //e.latlng
                typeof (iScope.kOptions.keypress) == "function" && iScope.kOptions.keypress.call(this, e);
            }

            function preclick(e) {
                //e.latlng
                //Fired before mouse click on the map 
                //(sometimes useful when you want something to happen on click before any existing click handlers start running).
                typeof (iScope.kOptions.preclick) == "function" && iScope.kOptions.preclick.call(this, e);
            }

            /*********************** map options ***********************/
            setMapView(temp.center.lat, temp.center.lng, temp.center.zoom);
            var attributionControl = L.control.attribution(), tileLayerObj = L.tileLayer(temp.tileLayer, temp.tileLayerOptions);
            tileLayerObj.addTo(map);
            var geocoder = L.Control.Geocoder.nominatim({ geocodingQueryParams: { countrycodes: 'ir' } });
            geocoderControl.options.geocoder = geocoder;
            //geocoderControl.on('markgeocode', function (e) {
            //    //var bbox = e.geocode.bbox;
            //    //var poly = L.polygon([
            //    //    bbox.getSouthEast(),
            //    //    bbox.getNorthEast(),
            //    //    bbox.getNorthWest(),
            //    //    bbox.getSouthWest()
            //    //]).addTo(map);
            //    //map.fitBounds(poly.getBounds());
            //}).addTo(map);
            map.zoomControl.setPosition(temp.zoomControlPosition);
            if (!map.zoomControl)
                map.zoomControl.removeFrom(map);

            //whenReady(<Function> fn, <Object> context?) 	return this, Runs the given function fn when the map gets initialized with a view
            //  (center and zoom) and at least one layer, or immediately if it's already initialized, optionally passing a function context.
            map.whenReady(function (e) {
                iScope.kOptions.createMarkerOptions = createMarkerOptions;
                iScope.kOptions.setDefaultMarker = setDefaultMarker;
                iScope.kOptions.setMarker = setMarker;
                iScope.kOptions.setMarkerList = setMarkerList;
                iScope.kOptions.getMarkers = getMarkers;
                iScope.kOptions.getMarkersLatLng = getMarkersLatLng;
                iScope.kOptions.getCurrentMarker = getCurrentMarker;
                iScope.kOptions.getCurrentMarkerLatLng = getCurrentMarkerLatLng;
                iScope.kOptions.geocodeAddress = geocodeAddress;
                iScope.kOptions.resetAllMarker = resetAllMarker;
                if (iScope.kOptions.tabularPage) {
                    $('.nav.nav-tabs').on('click', function (e) {
                       // if ($(map.getContainer()).closest(".tab-pane").hasClass("active")) {
                            $timeout(function () {
                                $(map.getContainer()).show(function () {
                                    map.invalidateSize();
                                })
                            }, 100);
                       // }
                    });
                }
                typeof (iScope.kOptions.ready) == "function" && iScope.kOptions.ready.call(this, e, iScope.kOptions);
            });

            attributionControl.setPrefix("");

            iScope.$on('$destroy', function () {
                //reset;
                map.remove(); //return this, Destroys the map and clears all related event listeners.
            });

            /*********************** methods ***********************/
            function updateWidth(width) {
                if (isNaN(+width)) {
                    template.css('width', width);
                }
                else {
                    template.css('width', width + 'px');
                }
            }

            function updateHeight(height) {
                if (isNaN(+height)) {
                    template.css('height', height);
                }
                else {
                    template.css('height', height + 'px');
                }
            }

            function setMapView(lat, lng, zoom) {
                map.setView([lat, lng]);
                if (angular.isDefined(zoom) && !isNaN(+zoom))
                    map.setZoom(+zoom);
            }

            function flyToLatlng(latlng, zoom) {
                if (angular.isDefined(zoom) && !isNaN(+zoom))
                    map.flyTo(latlng, +zoom);
            }

            function addMarker(latlng, markerOptions) {
                var marker = null;
                if (markerOptions)
                    marker = L.marker(latlng, markerOptions).addTo(map);
                else
                    marker = L.marker(latlng).addTo(map);
                return marker;
            }

            function removeMarker(marker) {
                if (marker && !angular.equals(marker, {}))
                    map.removeLayer(marker);
            }

            function resetMarkers() {
                if (temp.markers.length) {
                    temp.markers.forEach(function (marker, i, a) {
                        removeMarker(marker);
                    });
                    temp.markers = [];
                }
            }

            function resetCurrentMarker() {
                if (temp.currentMarker) {
                    removeMarker(temp.currentMarker);
                    temp.currentMarker = null;
                }
            }

            function resetAllMarker() {
                resetMarkers();
                resetCurrentMarker();
            }

            function setMarkerList(markers) {
                if (!iScope.kOptions.markers)
                    return;
                resetAllMarker();
                var markerOptions = {
                    draggable: iScope.kOptions.markers.draggable,
                    opacity: iScope.kOptions.markers.opacity,
                    icon: {}
                };

                temp.markers = markers.forEach(function (markerItem, i, a) {
                    markerOptions.icon = {};

                    if (iScope.kOptions.markers.icon) {
                        if (markerItem.MarkerColor && iScope.kOptions.markers.icon.markerColors && iScope.kOptions.markers.icon.markerColors[markerItem.MarkerColor - 1])
                            markerOptions.icon.iconUrl = iScope.kOptions.markers.icon.markerColors[markerItem.MarkerColor - 1];
                        if (iScope.kOptions.markers.icon.iconSize)
                            markerOptions.icon.iconSize = iScope.kOptions.markers.icon.iconSize;
                        if (iScope.kOptions.markers.icon.iconAnchor)
                            markerOptions.icon.iconAnchor = iScope.kOptions.markers.icon.iconAnchor;
                        if (iScope.kOptions.markers.icon.popupAnchor)
                            markerOptions.icon.popupAnchor = iScope.kOptions.markers.icon.popupAnchor;
                    }

                    var marker = addMarker([markerItem.Latitude, markerItem.Longtitude], createMarkerOptions(markerOptions));
                    if (iScope.kOptions.markers.popup && iScope.kOptions.markers.popup.template) {
                        if (typeof (iScope.kOptions.markers.popup.template) == "function")
                            marker.bindPopup(iScope.kOptions.markers.popup.template(markerItem));
                        else
                            marker.bindPopup(iScope.kOptions.markers.popup.template);
                        if (iScope.kOptions.markers.popup.open)
                            marker.openPopup();
                    }
                    return marker;
                });
            }

            function setMarker(setOptions, markerOptions) {//{ latlng: { lat: , lng: }, zoom: }
                if (!iScope.kOptions.marker)
                    return;

                if (iScope.kOptions.marker.count == 1) {
                    if (temp.currentMarker) {
                        //temp.currentMarker.setLatLng(setOptions.latlng);
                        removeMarker(temp.currentMarker);
                        temp.markers = [];
                        temp.currentMarker = null;
                    }
                    temp.currentMarker = addMarker(setOptions.latlng, markerOptions);
                    temp.markers.push(temp.currentMarker);
                }
                else if (iScope.kOptions.marker.count == 2) {
                    if (temp.currentMarker) {
                        if (temp.markers.length == 2) {
                            removeMarker(temp.currentMarker);
                            temp.markers = temp.markers.filter(function (markerItem, i, a) {
                                var markerItemLatLng = markerItem.getLatLng();
                                var currentMarkerLatLng = temp.currentMarker.getLatLng();
                                return markerItemLatLng.lat != currentMarkerLatLng.lat && markerItemLatLng.lng != currentMarkerLatLng.lng;
                            });
                        }
                        temp.currentMarker = null;
                    }
                    temp.currentMarker = addMarker(setOptions.latlng, markerOptions);
                    temp.markers.push(temp.currentMarker);
                }
                else {
                    temp.currentMarker = addMarker(setOptions.latlng, markerOptions);
                    temp.markers.push(temp.currentMarker);
                }

                if (setOptions.zoom && !isNaN(+setOptions.zoom)) {
                    flyToLatlng(setOptions.latlng, setOptions.zoom);
                }

                return temp.currentMarker;
            }

            function setDefaultMarker(setOptions) { //{ latlng: { lat: , lng: }, setView: { zoom }}
                if (!iScope.kOptions.marker)
                    return;

                var markerOptions = {
                    draggable: iScope.kOptions.marker.draggable,
                    opacity: iScope.kOptions.marker.opacity,
                    icon: {}
                }

                if (iScope.kOptions.marker.icon) {
                    if (iScope.kOptions.marker.icon.iconUrl)
                        markerOptions.icon.iconUrl = iScope.kOptions.marker.icon.iconUrl;
                    if (iScope.kOptions.marker.icon.iconSize)
                        markerOptions.icon.iconSize = iScope.kOptions.marker.icon.iconSize;
                    if (iScope.kOptions.marker.icon.iconAnchor)
                        markerOptions.icon.iconAnchor = iScope.kOptions.marker.icon.iconAnchor;
                    if (iScope.kOptions.marker.icon.popupAnchor)
                        markerOptions.icon.popupAnchor = iScope.kOptions.marker.icon.popupAnchor;
                }

                return setMarker(setOptions, createMarkerOptions(markerOptions));
            }

            function createMarkerOptions(optionsObject) {
                optionsObject = optionsObject || {};
                return {
                    icon: createMarkerIcon(optionsObject.icon),
                    draggable: angular.isUndefined(optionsObject.draggable) ? false : optionsObject.draggable,
                    opacity: optionsObject.opacity || 1.0
                }
            }

            function createMarkerIcon(iconObject) {
                iconObject = iconObject || {};
                return L.icon({
                    iconUrl: iconObject.iconUrl || 'Content/images/map/red.png',
                    iconSize: iconObject.iconSize || [22, 33],
                    iconAnchor: iconObject.iconAnchor || [12, 32],
                    popupAnchor: iconObject.popupAnchor || [-2, -30],
                    //shadowUrl: 'my-icon-shadow.png',
                    //shadowSize: [68, 95],
                    //shadowAnchor: [22, 94]
                });
            }

            function getMarkers() {
                return temp.markers;
            }

            function getMarkersLatLng() {
                var markers = getMarkers();
                if (!markers || !markers.length)
                    return [];
                return markers.map(function (marker, i, a) {
                    markerLatLng = marker.getLatLng();
                    return {
                        lat: markerLatLng.lat,
                        lng: markerLatLng.lng
                    }
                });
            }

            function getCurrentMarker() {
                return temp.currentMarker;
            }

            function getCurrentMarkerLatLng() {
                var marker = getCurrentMarker();
                if (!marker)
                    return null;
                return marker.getLatLng();
            }

            function geocodeAddress(model, geocoder, zoom) {
                geocoder = geocoder || geocoderControl.options.geocoder;
                if (!geocoder)
                    return;

                var address = getAddressByPositionIndex(model, iScope.kOptions.positions, iScope.kOptions.positions.length)
                if (!address.length)
                    return;

                var promises = [];
                for (var i = 0; i < address.length; i++)
                    promises.push(geocode(address[i]).then(function (response) { return response }));
                var defered = $q.defer();
                $q.all(promises).then(function (responses) {
                    var geocodeSuccess = responses.filter(function (respose, i, a) { return respose.results.length != 0; });
                    var iranLocation = geocodeSuccess.filter(function (respose, i, a) { return respose.results[0].properties.address.country.toLowerCase() == "iran" || respose.results[0].properties.address.country.toLowerCase() == "ایران"; });
                    if (!geocodeSuccess.length) {
                        var code = 10; var msg = createErrorMsgByCode(code);
                        createLocationError({}, code, msg, "geocode");
                        defered.reject({ code: code, message: msg, fn: "geocode" });
                    }
                    else if (!iranLocation.length) {
                        var code = 11; var msg = createErrorMsgByCode(code);
                        createLocationError({}, code, msg, "geocode");
                        defered.reject({ code: code, message: msg, fn: "geocode" });
                    }
                    else {
                        locationFound.call(map, { type: "geocode" });
                        iranLocation.forEach(function (response) {
                            flyToLatlng(response.results[0].center, zoom || 7);
                        });
                        var code = 0; var msg = createErrorMsgByCode(code);
                        defered.resolve({ result: iranLocation[iranLocation.length - 1].results[0], code: code, message: msg, fn: "geocode" });
                    }
                });
                return defered.promise;
            }

            function geocode(address) {
                var defer = $q.defer();
                geocoder.geocode(address, function (results) {
                    defer.resolve({ results: results, address: address });
                });
                return defer.promise;
            }

            function getAddressByPositionIndex(model, positions, length) {
                var address = [], index = length - 1, prefix = "";
                while (index >= 0 && positions[index]) {
                    address.push(prefix + model[positions[index]]);
                    prefix = address[length - (index + 1)] + " ";
                    index--;
                }
                return address;
            }

            function createLocationError(e, code, msg, fn) {
                var errorArgs = angular.copy(e);
                errorArgs.code = code;
                errorArgs.message = msg;
                errorArgs.fn = fn;
                locationError.call(map, errorArgs);
            }

            function createErrorMsgByCode(code) {
                var message = undefined;
                switch (code) {
                    case 0:
                        message = temp.success;
                        break;
                    case 10:
                        message = temp.notFoundMsg;
                        break;
                    case 11:
                        message = temp.outOfZoneMsg;
                        break;
                    default:
                        message = "";
                }
                return message;
            }

            //centerParam = {
            //    lat: parseFloat(cParam[0]),
            //    lng: parseFloat(cParam[1]),
            //    zoom: parseInt(cParam[2], 10),
            //};

            //to track address
            //map.locate({
            //    setView: true,
            //    maxZoom: center.zoom,
            //});

            //map.locate({ watch: true });
            //addHandler(<String> name, <Function> HandlerClass) 	return this, Adds a new Handler to the map, given its name and constructor function.

            //GeocodingResult by reverse or geocode:
            //  name 	String 	Name of found location
            //  bbox 	L.LatLngBounds 	The bounds of the location
            //  center 	L.LatLng 	The center coordinate of the location
            //  icon 	String 	URL for icon representing result; optional
            //  html 	String(optional) HTML formatted representation of the name
        }

        return {
            restrict: 'A',
            scope: {
                kOptions: '=',
                htmlOptions: '=?'
            },
            controller: controller,
            link: link
        }

    }])
})

/* methods:
 *    iScope.kOptions.createMarkerOptions()
 *    iScope.kOptions.setDefaultMarker()
 *    iScope.kOptions.setMarker()
 *    iScope.kOptions.getMarkers()
 *    iScope.kOptions.getMarkersLatLng()
 *    iScope.kOptions.getCurrentMarker()
 *    iScope.kOptions.getCurrentMarkerLatLng()
 *    iScope.kOptions.geocodeAddress()
 *    iScope.kOptions.setMarkerList()
 */