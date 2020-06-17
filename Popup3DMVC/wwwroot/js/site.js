$.fn.serializeObject = function() {
	var o = {};
	var a = this.serializeArray();
	$.each(a, function() {
		if(o[this.name]) {
			if(!o[this.name].push) {
				o[this.name] = [o[this.name]];
			}
			o[this.name].push(this.value || '');
		} else {
			o[this.name] = this.value || '';
		}
	});
	return o;
};

let orig = $.fn.popover,
	proto = $.extend({}, $.fn.popover.Constructor.prototype);

$.fn.popover = function(options) {
	return this.each(function() {
		orig.call($(this), options);
		if(typeof options.show === 'function') {
			$(this).data('bs.popover').show = function() {
				options.show.call(this.tip, this);
				proto.show.call(this);
			};
		}
	});
}


// nav popovers
let els_a_pop = $('nav#nav-main .dropdown > a.pop')
let el_cur_a_pop;
let el_cur_popover;
els_a_pop
	.popover({
		//animation: false,
		container: 'nav#nav-main',
		placement: 'bottom',
		trigger: 'manual',
		html: true,
		content: function() {
			return this.nextElementSibling.innerHTML;
		},
		show: function() {
			el_cur_popover = this;
			$(this)
				.css({ marginTop: 30 })
				.animate({ marginTop: 15 }, 250);
			setTimeout(() => {
				let input = this.querySelector('input');
				if(input) input.focus();
			}, 10);
		}
	})
	.click(function(e) {
		e.preventDefault();
		els_a_pop.popover('hide');
		if(el_cur_a_pop === this)
			el_cur_a_pop = undefined;
		else {
			el_cur_a_pop = this;
			$(this).popover('show');
		}
	})

$('nav#nav-main')
	.on('focusout', function(e) {
		console.log(e);
		if($(e.relatedTarget).closest(el_cur_popover).length === 0) {// && $(e.target).closest(el_cur_popover).length === 0
			els_a_pop.popover('hide');
			el_cur_a_pop = undefined;
			//console.log("HIDEEE", e.relatedTarget, e);
		}
	});

// nav account popover
$('nav#nav-main')
	.on('submit', 'form.login-form', function(e) {
		let params = $(this).serializeObject();// call it before disabling controls
		let el_fieldset = $('fieldset', this);
		el_fieldset.prop('disabled', true);
		let el_alert = $('.alert', this);
		el_alert.hide();

		$.post('/Usuario/Login', params)
			.done(function(data) {
				location.reload();
			})
			.fail(function() {
				el_fieldset.prop('disabled', false);
				el_alert.show();
			});

		e.preventDefault();
		return false;
	});




function AjaxUploadFile(file) {
	var ext = file.name.split('.').pop().toLowerCase();
	if(ext !== 'stl') {
		alert('Selecione um arquivo com extensão .stl');
		return;
	}

	$('.overlay-upload').addClass('active');

	var formData = new FormData();
	formData.append("FileUpload", file);

	var jqXHR = $.ajax({
		type: 'post',
		url: '/Home/Upload',
		data: formData,
		contentType: false,
		processData: false,
		success: function(response) {
			window.location = "/Imprimir/Upload/" + response.id
		},
		error: function(error) {
			alert("Erro ao processar o seu arquivo .stl");
			$('.overlay-upload').removeClass('active');
		},
		xhr: function() {
			var jqXHR = null;
			if(window.ActiveXObject)
				jqXHR = new window.ActiveXObject("Microsoft.XMLHTTP");
			else
				jqXHR = new window.XMLHttpRequest();

			//Upload progress
			jqXHR.upload.addEventListener("progress", function(evt) {
				if(evt.lengthComputable) {
					var percentComplete = Math.round((evt.loaded * 100) / evt.total);
					$('#prog').text(percentComplete + '%');
				}
			}, false);
			return jqXHR;
		},
	});
}

$('#file-select').on('change', function(evt) {
	AjaxUploadFile(this.files[0]);
});

// DnD .stl files
var isAdvancedUpload = function() {
	var div = document.createElement('div');
	return (('draggable' in div) || ('ondragstart' in div && 'ondrop' in div)) && 'FormData' in window && 'FileReader' in window;
}();

if(isAdvancedUpload) {
	var el_html = $('html');
	el_html
		.on('drag dragstart dragend dragover dragenter dragleave drop', function(e) {
			e.preventDefault();
			e.stopPropagation();
		})
		.on('dragover dragenter', function() {
			$('.overlay-dnd').show();
		})
		.on('dragleave dragend drop', function() {
			$('.overlay-dnd').hide();
		})
		.on('drop', function(e) {
			files = e.originalEvent.dataTransfer.files;
			if(files.length !== 1) {
				alert('Você deve soltar somente um arquivo');
				return;
			}

			AjaxUploadFile(files[0]);
		});
}