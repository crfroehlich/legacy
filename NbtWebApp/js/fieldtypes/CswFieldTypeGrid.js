﻿; (function ($) {
		
	var PluginName = 'CswFieldTypeGrid';

	var methods = {
		init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

			var $Div = $(this);
			$Div.contents().remove();

			var ViewId = o.$propxml.children('viewid').text().trim();

			$($Div).CswNodeGrid({'viewid': ViewId, 'nodeid': o.nodeid, 'cswnbtnodekey': o.cswnbtnodekey, 'readonly': o.ReadOnly} );

		},
		save: function(o) {
//                var $TextBox = $propdiv.find('input');
//                $xml.children('barcode').text($TextBox.val());
			}
	};
	
	// Method calling logic
	$.fn.CswFieldTypeGrid = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};
})(jQuery);
