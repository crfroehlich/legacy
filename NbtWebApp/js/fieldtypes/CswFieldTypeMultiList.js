/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeMultiList';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var gestalt = Csw.string(o.propData.gestalt).trim();
            var options = propVals.options;

            if (o.ReadOnly) {
                propDiv.append(gestalt);
            } else {
                /* Select Box */
                propDiv.multiSelect({ //$.CswMultiSelect({
                    ID: o.ID,
                    cssclass: 'selectinput',
                    values: options,
                    isMultiEdit: o.Multi
                });
            }
        },
        save: function (o) { //$propdiv, $xml

            var attributes = { value: null },
                multi = o.propDiv.find('#' + o.ID),
                cachedVals,
                distinctVals = [];

            if (false === Csw.isNullOrEmpty(multi)) {
                attributes.value = multi.val().split(',').sort().join(',');
            }

            //multiSelect sorts the val for us, sort o.propData.values.value to make comparision work
            if (false === Csw.isNullOrEmpty(o.propData.values.value)) {
                cachedVals = o.propData.values.value.split(',');
                Csw.each(cachedVals, function (value) {
                    //Guarantee the values are distinct locally
                    if (false === Csw.contains(distinctVals, value)) {
                        distinctVals.push(value);
                    }
                });
            }
            o.propData.values.value = distinctVals.sort().join(',');

            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeMultiList = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);