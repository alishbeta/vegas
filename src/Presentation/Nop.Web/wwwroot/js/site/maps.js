var icomsBase = 'https://' + location.host + '/images/';
var map;
var adresses = $('#address').data('address');
var zoom = 11;
if (adresses[0].zoom) {
	zoom = adresses[0].zoom;
}
function initMap() {
    geocoder = new google.maps.Geocoder();
    map = new google.maps.Map(document.getElementById('map'), {
        center: {
            lat: 50.458961,
            lng: 30.337648
        },
        zoom: zoom
    });

    adresses.forEach(addPoint);

    function addPoint(data, index) {
        geocoder.geocode({ 'address': data.addr }, function (results, status) {
            if (status == 'OK') {

                var marker = new google.maps.Marker({
                    map: map,
                    icon: icomsBase + 'map_logo.png',
                    position: results[0].geometry.location,
                    title: 'Bed4you'
                });

				map.setCenter(results[0].geometry.location);

                var contentString = '<div id="content">' +
                    '<div id="siteNotice">' +
                    '</div>' +
                    '<h3 id="firstHeading" class="firstHeading">' + results[0].address_components[3].long_name + '</h1>' +
                    data.description +
                    '<div id="bodyContent">' +
                    '</div>' +
                    '</div>';
                var infowindow = new google.maps.InfoWindow({
                    content: contentString
                });

                marker.addListener('click', function () {
                    infowindow.open(map, marker);
                });
            } else {
                console.log('Geocode was not successful for the following reason: ' + status);
            }
        });
    }
}
