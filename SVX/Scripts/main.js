Dropzone.autoDiscover = false;
$(function () {
	$('.wizard-content .wizard > .actions > ul > li > a').last().attr('id', 'btnSubmit');
	$('.letras').keypress(function (e) {
		return /^[a-zA-ZñÑáéíóú]*\ ?[a-zA-ZñÑáéíóú]*$/.test($(this).val() + e.key);
	});
	$('.money').keypress(function (e) {
		return /^[0-9]*\.?[0-9]{0,2}$/.test($(this).val() + e.key);
	});
	var myDropzone = new Dropzone('#dropzoneForm, #formEditarAnuncio', {
		autoProcessQueue: false,
		maxFilesize: 5,
		uploadMultiple: true,
		paramName: 'files',
		addRemoveLinks: true,
		previewsContainer: '#previews',
		dictDefaultMessage:
			'<i class="fas fa-upload"></i><strong> Arrastra y suelta los archivos aquí o haz clic.</strong>',
		dictFallbackMessage: 'Navegador invalido',
		dictResponseError: 'Server not Configured',
		dictInvalidFileType: 'Tipo de archivo invalido',
		dictRemoveFile: 'Remover archivo',
		dictRemoveFileConfirmation: 'Estas seguro que quieres remover archivo?',
		acceptedFiles: '.jpeg, .jpg, .png',
		maxFiles: 5,
		parallelUploads: 5,
		dictMaxFilesExceeded: 'No puedes subir más de 5 archivos a la vez.',
		error: function (file, response) {
			file.previewElement.classList.add('dz-error');
			$(file.previewElement).find('.dz-error-message').text(response);
			mostrarAlerta('error', response);
		},
	});
	$('#btnSubmit').click(function () {
		myDropzone.processQueue();
		$(this).closest('form').submit();
		myDropzone.on('complete', function (file) {
			myDropzone.removeFile(file);
		});
		playAudio('../Audio/mario-coin.mp3');
	});
	function playAudio(url) {
		new Audio(url).play();
	}
	$('#titulo').click(function () {
		playAudio('../Audio/mario-coin.mp3');
	});
	$('#estadoProducto').change(function () {
		let aux = $(this).is(':checked')
		if (aux) {
			$('#estadoProducto').val(1);
		}
		else
			$('#estadoProducto').val(0);
	});
	//const objeto = new google.maps.LatLng(13.7056522, -89.2124169);

	function Dibujado() {
		var mapProp = {
			center: { lat: 13.79069, lng: -88.9018 },
			zoom: 12,
			disableDoubleClickZoom: true,
			styles: [
				{
					elementType: 'geometry',
					stylers: [
						{
							color: '#242f3e',
						},
					],
				},
				{
					elementType: 'labels.text.fill',
					stylers: [
						{
							color: '#746855',
						},
					],
				},
				{
					elementType: 'labels.text.stroke',
					stylers: [
						{
							color: '#242f3e',
						},
					],
				},
				{
					featureType: 'administrative.locality',
					elementType: 'labels.text.fill',
					stylers: [
						{
							color: '#d59563',
						},
					],
				},
				{
					featureType: 'poi',
					elementType: 'labels.text.fill',
					stylers: [
						{
							color: '#d59563',
						},
					],
				},
				{
					featureType: 'poi.park',
					elementType: 'geometry',
					stylers: [
						{
							color: '#263c3f',
						},
					],
				},
				{
					featureType: 'poi.park',
					elementType: 'labels.text.fill',
					stylers: [
						{
							color: '#6b9a76',
						},
					],
				},
				{
					featureType: 'road',
					elementType: 'geometry',
					stylers: [
						{
							color: '#38414e',
						},
					],
				},
				{
					featureType: 'road',
					elementType: 'geometry.stroke',
					stylers: [
						{
							color: '#212a37',
						},
					],
				},
				{
					featureType: 'road',
					elementType: 'labels.text.fill',
					stylers: [
						{
							color: '#9ca5b3',
						},
					],
				},
				{
					featureType: 'road.highway',
					elementType: 'geometry',
					stylers: [
						{
							color: '#746855',
						},
					],
				},
				{
					featureType: 'road.highway',
					elementType: 'geometry.stroke',
					stylers: [
						{
							color: '#1f2835',
						},
					],
				},
				{
					featureType: 'road.highway',
					elementType: 'labels.text.fill',
					stylers: [
						{
							color: '#f3d19c',
						},
					],
				},
				{
					featureType: 'transit',
					elementType: 'geometry',
					stylers: [
						{
							color: '#2f3948',
						},
					],
				},
				{
					featureType: 'transit.station',
					elementType: 'labels.text.fill',
					stylers: [
						{
							color: '#d59563',
						},
					],
				},
				{
					featureType: 'water',
					elementType: 'geometry',
					stylers: [
						{
							color: '#17263c',
						},
					],
				},
				{
					featureType: 'water',
					elementType: 'labels.text.fill',
					stylers: [
						{
							color: '#515c6d',
						},
					],
				},
				{
					featureType: 'water',
					elementType: 'labels.text.stroke',
					stylers: [
						{
							color: '#17263c',
						},
					],
				},
			],
		};

		var map = new google.maps.Map(document.getElementById('mapa'), mapProp);
		const marker = new google.maps.Marker({
			map: map,
			draggable: true,
			icon: 'https://www.google.com/mapfiles/marker_green.png',
		});
		map.addListener('dblclick', function (e) {
			placeMarker(e.latLng, map);
		});

		function placeMarker(position) {
			if (marker == null) {
				marker = new google.maps.Marker({
					position: position,
					map: map,
				});
			} else {
				marker.setPosition(position);
			}
			let lat = marker.getPosition().lat();
			let lng = marker.getPosition().lng();
			$('#latitud').val(lat.toFixed(8));
			$('#longitud').val(lng.toFixed(8));
			map.panTo(marker.getPosition());
		}
		google.maps.event.addListener(marker, 'dragend', function (evt) {
			$('#latitud').val(evt.latLng.lat().toFixed(8));
			$('#longitud').val(evt.latLng.lng().toFixed(8));
			map.panTo(evt.latLng);
		});
		marker.setMap(map);
	}
	google.maps.event.addDomListener(window, 'load', Dibujado);
});
