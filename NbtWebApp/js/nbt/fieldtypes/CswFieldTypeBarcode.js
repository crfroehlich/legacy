/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeBarcode';

    var methods = {
        init: function (o) { 

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? Csw.string(propVals.barcode).trim() : Csw.enums.multiEditDefaultValue;

            if (o.ReadOnly) {
                propDiv.text(value);
            }
            else {
                var table = propDiv.table({
                    ID: Csw.makeId(o.ID, 'tbl')
                });

                var cell1 = table.cell(1, 1);
                var textBox = cell1.input({ ID: o.ID,
                    type: Csw.enums.inputTypes.text,
                    cssclass: 'textinput',
                    onChange: o.onChange,
                    value: value
                });
                if (false === o.Multi) {
                    table.cell(1, 2).div()
                         .imageButton({ ButtonType: Csw.enums.imageButton_ButtonType.Print,
                             AlternateText: '',
                             ID: Csw.makeId(o.ID, 'print'),
                             onClick: function () {
                                 $.CswDialog('PrintLabelDialog', { 'nodeid': o.nodeid, 'propid': o.ID });
                             }
                         });
                }
                if (o.Required) {
                    textBox.addClass('required');
                }

                textBox.clickOnEnter(o.saveBtn);
            }
        },
        save: function (o) {
            var attributes = { barcode: null };
            var textBox = o.propDiv.find('input');
            if (false === Csw.isNullOrEmpty(textBox)) {
                attributes.barcode = textBox.val();
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeBarcode = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);