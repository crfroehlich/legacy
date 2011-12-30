/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />

(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypePropertyReference';

    var methods = {
        init: function(o) { 
                
            var $Div = $(this);
            $Div.contents().remove();
                
            var propVals = o.propData.values;
            var text = (false === o.Multi) ? tryParseString(propVals.value).trim() : CswMultiEditDefaultValue;
            text += '&nbsp;';

            /* Static Div */
            $('<div id="'+ o.ID +'" class="staticvalue">' + text + '</div>' )
                            .appendTo($Div); 
        },
        save: function(o) { //$propdiv, $xml
            preparePropJsonForSave(o.propData);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypePropertyReference = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
