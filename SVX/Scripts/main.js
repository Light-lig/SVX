Dropzone.autoDiscover = false;
$(function () {
	$('.wizard-content .wizard > .actions > ul > li > a').last().attr('id', 'btnSubmit');
	$('.letras').keypress(function (e) {
		return /^[a-zA-ZñÑáéíóú]*\ ?[a-zA-ZñÑáéíóú]*$/.test($(this).val() + e.key);
	});
	$('.money').keypress(function (e) {
		return /^[0-9]*\.?[0-9]{0,2}$/.test($(this).val() + e.key);
	});
	var myDropzone = new Dropzone('#dropzoneForm', {
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
});
