﻿; (function ($) {
        
    var PluginName = 'CswFieldTypeDate';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.children().remove();

            var Value = o.$propxml.children('value').text().trim();
            if(Value == '1/1/0001')
                Value = '';

            if(o.ReadOnly)
            {
                $Div.append(Value);
            }
            else 
            {
                var $TextBox = $('<input type="text" class="textinput date" id="'+ o.ID +'" name="' + o.ID + '" value="'+ Value +'" />"' )
                                    .appendTo($Div)
                                    .change(o.onchange)
                                    .datepicker();
                if(o.Required)
                {
                    $TextBox.addClass("required");
                }
            }
        },
        save: function(o) { //$propdiv, $xml
                var $TextBox = o.$propdiv.find('input');
                o.$propxml.children('value').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeDate = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
