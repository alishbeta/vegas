var icomsBase = '/img/';
var map;
var adresses = $('#address').data('address');

function initMap() {
    let loc = location.pathname.split('/');
    if (loc[2] == 'contactus') {
        map = new google.maps.Map(document.getElementById('map'), {
            center: {lat: -34.397, lng: 150.644},
            zoom: 8
        });
    } else {
        if ($('*').is('#map') && adresses != undefined) {
            geocoder = new google.maps.Geocoder();

            let isMain = $('.blue-menu ul li:first-child a').hasClass('active');

            if (isMain) {
                map = new google.maps.Map(document.getElementById('map'), {
                    center: {lat: 49.443363, lng: 32.062298},
                    zoom: 6
                });
            } else {

                map = new google.maps.Map(document.getElementById('map'), {
                    center: adresses[0].position,
                    zoom: 10
                });

            }

            adresses.forEach(addPoint);

            function addPoint(data, index) {
                console.log(data.position);
                let marker = new google.maps.Marker({
                    map: map,
                    icon: icomsBase + 'map_logo.png',
                    position: data.position,
                    title: 'Bed4you'
                });
                let phone = '';
                let work = '';
                let name = '';

                if (typeof data.name != 'undefined') {
                    name = '<div>' + data.name + '</div>';
                }
                if (typeof data.phone != 'undefined') {
                    phone = '<div><span>Телефон</span>: ' + data.phone + '</div>';
                }
                if (typeof data.workTime != 'undefined') {
                    work = '<div><span>График</span>: ' + data.workTime + '</div>';
                }

                var contentString = '<div id="content">' +
                    '<div id="siteNotice">' +
                    '</div>' +
                    '<h3 id="firstHeading" class="firstHeading"></h1>' +
                    '<div id="bodyContent">' + name +
                    '<div>' + data.addr + '</div>' +
                    phone +
                    work +
                    '</div>' +
                    '</div>';
                var infowindow = new google.maps.InfoWindow({
                    content: contentString
                });

                marker.addListener('click', function () {
                    infowindow.open(map, marker);
                });
            }
        }
    }
}
