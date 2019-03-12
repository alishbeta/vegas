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

            geocoder.geocode({ 'address': adresses[0].city }, function(results, status){
                if (status == 'OK') {
                    map = new google.maps.Map(document.getElementById('map'), {
                        center: results[0].geometry.location,
                        zoom: 11
                    });
                }
            });
            
            console.log(adresses[0].city);
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
                        let phone = '';
                        let work = '';
                        let name = '';

                        console.log(data);
                        

                        if (typeof data.name != 'undefined'){
                            name = '<div>'+ data.name +'</div>';
                        }
                        if (typeof data.phone != 'undefined'){
                            phone = '<div><span>Телефон</span>: '+ data.phone +'</div>';
                        }
                        if (typeof data.workTime != 'undefined'){
                            work = '<div><span>График</span>: '+ data.workTime +'</div>';
                        }
                            

                        var contentString = '<div id="content">' +
                            '<div id="siteNotice">' +
                            '</div>' +
                            '<h3 id="firstHeading" class="firstHeading">' + results[0].address_components[3].long_name + '</h1>' +
                            '<div id="bodyContent">' +
                             name +
                            '<div>'+ data.addr +'</div>' +
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
                    } else {
                        console.log('Geocode was not successful for the following reason: ' + status);
                    }
                });
            }
        }
    }
}
