var icomsBase = 'img/';
var map;
var adresses = $('#address').data('address');
function initMap() {
    if ($('*').is('#map') && adresses != 'undefined') {
        geocoder = new google.maps.Geocoder();
        map = new google.maps.Map(document.getElementById('map'), {
            center: {
                lat: 50.458961,
                lng: 30.337648
            },
            zoom: 11
        });

        adresses.forEach(addPoint);
        function addPoint(data, index) {
            geocoder.geocode({ 'address': data.addr }, function (results, status) {
                if (status == 'OK') {
                    console.log(data.addr);
                    console.log(results[0]);

                    var marker = new google.maps.Marker({
                        map: map,
                        icon: icomsBase + 'map_logo.png',
                        position: results[0].geometry.location,
                        title: 'Bed4you'
                    });

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
}
