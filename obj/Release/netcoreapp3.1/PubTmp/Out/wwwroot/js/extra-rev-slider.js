(function($) {
  "use strict";
	if(typeof revslider_showDoubleJqueryError === "undefined") {
		function revslider_showDoubleJqueryError(sliderID) {
			var err = "<div class='rs_error_message_box'>";
			err += "<div class='rs_error_message_oops'>Oops...</div>";
			err += "<div class='rs_error_message_content'>";
			err += "You have some jquery.js library include that comes after the Slider Revolution files js inclusion.<br>";
			err += "To fix this, you can:<br>&nbsp;&nbsp;&nbsp; 1. Set 'Module General Options' -> 'Advanced' -> 'jQuery & OutPut Filters' -> 'Put JS to Body' to on";
			err += "<br>&nbsp;&nbsp;&nbsp; 2. Find the double jQuery.js inclusion and remove it";
			err += "</div>";
		err += "</div>";
			jQuery(sliderID).show().html(err);
		}
	}
  var	revapi3,
			tpj;
	jQuery(function() {
		tpj = jQuery;
		if(tpj("#rev_slider_3_1").revolution == undefined){
			revslider_showDoubleJqueryError("#rev_slider_3_1");
		}else{
			revapi3 = tpj("#rev_slider_3_1").show().revolution({
				jsFileLocation:"js/",
				sliderLayout:"fullwidth",
				visibilityLevels:"1240,1024,778,480",
				gridwidth:"1300,1024,778,480",
				gridheight:"820,768,660,720",
				spinner:"spinner0",
				editorheight:"820,768,660,720",
				responsiveLevels:"1240,1024,778,480",
				navigation: {
					onHoverStop:false,
					arrows: {
						enable:true,
						style:"custom",
						hide_onmobile:true,
						hide_under:"767px",
						left: {
							h_offset:30
						},
						right: {
							h_offset:30
						}
					},
					bullets: {
						enable:true,
						tmp:"",
						style:"hesperiden"
					}
				},
				parallax: {
					levels:[5,10,15,20,25,30,35,40,45,46,47,48,49,50,51,30],
					type:"mouse",
					origo:"slidercenter",
					speed:0
				},
				fallbacks: {
					allowHTML5AutoPlayOnAndroid:true
				},
			});
		}

	});
})(jQuery);