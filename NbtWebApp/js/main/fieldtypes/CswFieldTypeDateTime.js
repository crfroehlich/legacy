/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";    
    var pluginName = 'CswFieldTypeDateTime';

    var methods = {
        init: function (o) {

            var propDiv  = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var date = (false === o.Multi) ? Csw.string(propVals.value.date).trim() : Csw.enums.multiEditDefaultValue;
            var time = (false === o.Multi) ? Csw.string(propVals.value.time).trim() : Csw.enums.multiEditDefaultValue;
            
            if(o.ReadOnly) {
                propDiv.append(o.propData.gestalt);    
            } else {
                var dtPickerDiv = propDiv.dateTimePicker({
                    ID: o.ID,
                    Date: date,
                    Time: time,
                    DateFormat: Csw.serverDateFormatToJQuery(propVals.value.dateformat),
                    TimeFormat: Csw.serverTimeFormatToJQuery(propVals.value.timeformat),
                    DisplayMode: propVals.displaymode,
                    ReadOnly: o.ReadOnly,
                    Required: o.Required,
                    onChange: o.onChange
                });

                dtPickerDiv.find('input').clickOnEnter(o.saveBtn);
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes, dPickerDiv, tPickerDiv;
            attributes = { 
                value: {
                    date: null,
                    time: null
                } 
            };
            dPickerDiv = o.propDiv.find('#' + o.ID + '_date');
            tPickerDiv = o.propDiv.find('#' + o.ID + '_date');
            if (false === Csw.isNullOrEmpty(dPickerDiv)) {
                attributes.value.date = dPickerDiv.val();
            }
            if (false === Csw.isNullOrEmpty(tPickerDiv)) {
                attributes.value.time = tPickerDiv.val();
            }

            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeDateTime = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
