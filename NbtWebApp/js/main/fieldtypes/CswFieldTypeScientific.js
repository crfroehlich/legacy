﻿; (function ($) {
        
    var PluginName = 'CswFieldTypeScientific';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);

			if(isTrue(o.ReadOnly))
			{
				$Div.append(o.propData.CswAttrXml('gestalt'));
			} 
			else 
			{
				var $ValueNTB = $Div.CswNumberTextBox({
					'ID': o.ID + '_val',
					'Value': o.propData.children('base').text().trim(),
					'Precision': 6,
					'ReadOnly': o.ReadOnly,
					'Required': o.Required,
					'onchange': o.onchange,
					'width': '60px'
				});
				$Div.append('E');
				var $ExponentNTB = $Div.CswNumberTextBox({
					'ID':  o.ID + '_exp',
					'Value': o.propData.children('exponent').text().trim(),
					'ReadOnly': o.ReadOnly,
					'Required': o.Required,
					'onchange': o.onchange,
					'width': '25px'
				});

				if(!isNullOrEmpty($ValueNTB) && $ValueNTB.length > 0)
				{
					$ValueNTB.clickOnEnter(o.$savebtn);
				}
				if(!isNullOrEmpty($ExponentNTB) && $ExponentNTB.length > 0)
				{
					$ExponentNTB.clickOnEnter(o.$savebtn);
				}
			}
        },
        save: function(o) { //$propdiv, $xml
				o.propData.children('base').text(o.$propdiv.CswNumberTextBox('value', o.ID + '_val'));
				o.propData.children('exponent').text(o.$propdiv.CswNumberTextBox('value', o.ID + '_exp'));
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeScientific = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
