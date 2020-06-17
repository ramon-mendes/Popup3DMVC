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
(function(){var c;c=jQuery;c.bootstrapGrowl=function(f,a){var b,e,d;a=c.extend({},c.bootstrapGrowl.default_options,a);b=c("<div>");b.attr("class","bootstrap-growl alert");a.type&&b.addClass("alert-"+a.type);a.allow_dismiss&&(b.addClass("alert-dismissible"),b.append('<button class="close" data-dismiss="alert" type="button"><span aria-hidden="true">&#215;</span><span class="sr-only">Close</span></button>'));b.append(f);a.top_offset&&(a.offset={from:"top",amount:a.top_offset});d=a.offset.amount;c(".bootstrap-growl").each(function(){return d= Math.max(d,parseInt(c(this).css(a.offset.from))+c(this).outerHeight()+a.stackup_spacing)});e={position:"body"===a.ele?"fixed":"absolute",margin:0,"z-index":"9999",display:"none"};e[a.offset.from]=d+"px";b.css(e);"auto"!==a.width&&b.css("width",a.width+"px");c(a.ele).append(b);switch(a.align){case "center":b.css({left:"50%","margin-left":"-"+b.outerWidth()/2+"px"});break;case "left":b.css("left","20px");break;default:b.css("right","20px")}b.fadeIn();0<a.delay&&b.delay(a.delay).fadeOut(function(){return c(this).alert("close")}); return b};c.bootstrapGrowl.default_options={ele:"body",type:"info",offset:{from:"top",amount:20},align:"right",width:250,delay:4E3,allow_dismiss:!0,stackup_spacing:10}}).call(this);