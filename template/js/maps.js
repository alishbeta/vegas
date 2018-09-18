var points= [];
points.kiev = [50.459149, 30.337648];



var map;
function initMap() {
    console.log(points.kiev);
    map = new google.maps.Map(document.getElementById('map'), {
        center: {lat: points.kiev[0], lng: points.kiev[1]},
        zoom: 17
    });
    var marker = new google.maps.Marker({position: {lat: points.kiev[0], lng: points.kiev[1]}, map: map});
}