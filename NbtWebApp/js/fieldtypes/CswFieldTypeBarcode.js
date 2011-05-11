﻿/// <reference path="../jquery/jquery-1.6-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var PluginName = 'CswFieldTypeBarcode';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly  == nodeid,propxml,onchange

            var $Div = $(this);
            $Div.contents().remove();

            var Value = o.$propxml.children('barcode').text().trim();

            if(o.ReadOnly)
            {
                $Div.append(Value);
            }
            else 
            {
                var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });

                var $cell1 = $table.CswTable('cell', 1, 1);
                var $TextBox = $cell1.CswInput('init',{ID: o.ID,
                                                       type: CswInput_Types.text,
                                                       cssclass: 'textinput',
                                                       onChange: o.onchange,
                                                       value: Value
                                               });

                var $cell2 = $table.CswTable('cell', 1, 2);
                var $PrintButton = $('<div/>' )
                                        .appendTo($cell2)
                                        .CswImageButton({  ButtonType: CswImageButton_ButtonType.Print,
                                                        AlternateText: '',
                                                        ID: o.ID + '_print',
                                                        onClick: function ($ImageDiv) { 
															$.CswDialog('PrintLabelDialog', { 'nodeid': o.nodeid, 'propid': o.ID });
                                                            return CswImageButton_ButtonType.None; 
                                                        }
                                                        });

                if(o.Required)
                {
                    $TextBox.addClass("required");
                }
            }
        },
        save: function(o) {
                var $TextBox = o.$propdiv.find('input');
                o.$propxml.children('barcode').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeBarcode = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
