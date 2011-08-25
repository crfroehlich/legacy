﻿/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../controls/CswCheckBoxArray.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeLogicalSet';
    var nameCol = 'name';
    var keyCol = 'key';
  

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();

            var logicalSetJson = o.propData.logicalsetjson;
            
            var $cbaDiv = $('<div />')
                            .appendTo($Div)
                            .CswCheckBoxArray('transmorgify', {
                                dataAry: logicalSetJson,
			                    nameCol: nameCol,
			                    keyCol: keyCol
                            })
                            .CswCheckBoxArray('init', {
                                ID: o.ID + '_cba',
                                onchange: o.onchange,
								ReadOnly: o.ReadOnly
                            });
            return $Div;

        },
        save: function(o) { //$propdiv, $xml
            var $CBADiv = o.$propdiv.children('div').first();
            var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
            o.propData.logicalsetjson = formdata;
            return $(this);
        } // save()
    };
    

    // Method calling logic
    $.fn.CswFieldTypeLogicalSet = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);





