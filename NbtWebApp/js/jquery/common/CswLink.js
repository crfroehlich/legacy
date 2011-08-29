﻿/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	
	var PluginName = "CswLink";
	
	var methods = {
	
		'init': function(options) 
		{
			var o = {
				'ID': '',
				'value': '',
				'type': '', //MIME type
				'media': 'all',
				'target': '',
				'rel': '',
				'cssclass': '',
                'href': '',
				'onClick': function() {}
			};
			if (options) $.extend(o, options);
			
			var $parent = $(this);
			var $link = $('<a></a>');
			
			var elementId = tryParseString(o.ID,'');
			if( elementId !== '' ) $link.CswAttrDom('id',elementId);
			if( !isNullOrEmpty( o.href ) ) $link.CswAttrDom('href', o.href);
			if( !isNullOrEmpty( o.value ) ) $link.text(o.value);
			if( !isNullOrEmpty( o.cssclass ) ) $link.addClass(o.cssclass);
			if( !isNullOrEmpty( o.type ) ) $link.CswAttrDom('type',o.type);
			if( !isNullOrEmpty( o.rel ) ) $link.CswAttrDom('rel',o.rel);
			if( !isNullOrEmpty( o.media ) ) $link.CswAttrDom('media',o.media);
			if( !isNullOrEmpty( o.target ) ) $link.CswAttrDom('target',o.target);
			if( !isNullOrEmpty( o.onClick ) ) 
			{
				$link.click( function() {
							 o.onClick();
				});
			}
					
			$parent.append($link);
			return $link;
		}
	};
		// Method calling logic
	$.fn.CswLink = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};


})(jQuery);
