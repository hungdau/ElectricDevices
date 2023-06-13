let map;

async function initMap() {
    const position = { lat: parseFloat($('#hidLat').val().replace(',', '.')), lng: parseFloat($('#hidLng').val().replace(',', '.')) };
    const { Map } = await google.maps.importLibrary("maps");
    const { AdvancedMarkerView } = await google.maps.importLibrary("marker");
    map = new Map(document.getElementById("map"), {
        zoom: 17,
        center: position
    });

    var contentString = $('#hidAddress').val();
    var infowindow = new google.maps.InfoWindow({
        content: contentString
    });

    var marker = new google.maps.Marker({
        position: position,
        map: map,
        title: $('#hidName').val()
    });
    infowindow.open(map, marker);
}

initMap();