var icomsBase = '/img/';
var map;
var adresses = $('#address').data('address');
function initMap() {
    let loc = location.pathname.split('/');
    if (loc[2] == 'contactus') {
            map = new google.maps.Map(document.getElementById('map'), {
                center: { lat: -34.397, lng: 150.644 },
                zoom: 8
            });
    } else {
        if ($('*').is('#map') && adresses != undefined) {
            geocoder = new google.maps.Geocoder();
            geocoder.geocode({ 'address': data.city }, function (results, status) {
                if (status == 'OK') {
                    map = new google.maps.Map(document.getElementById('map'), {
                        center: results[0].geometry.location,
                        zoom: 11
                    });
                } else {
                    console.log('Geocode was not successful for the following reason: ' + status);
                }
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

                        var contentString = '<div id="content">' +
                            '<div id="siteNotice">' +
                            '</div>' +
                            '<h3 id="firstHeading" class="firstHeading">' + results[0].address_components[3].long_name + '</h1>' +
                            '<div id="bodyContent">' +
                            '<div>'+ data.name +'</div>' +
                            '<div><span>ул.</span> '+ data.addr +'</div>' +
                            '<div><span>Телефон</span>: '+ data.phone +'</div>' +
                            '<div><span>График</span>: '+ data.workTime +'</div>' +
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
}
