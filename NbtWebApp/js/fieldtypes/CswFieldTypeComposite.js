﻿; (function ($) {

    var PluginName = 'CswFieldTypeComposite';

    var methods = {
        init: function (nodepk, $xml, onchange) {

            var $Div = $(this);
            $Div.children().remove();

            var ID = $xml.attr('id');
            //var Required = ($xml.attr('required') == "true");
            //var ReadOnly = ($xml.attr('readonly') == "true");

            var Value = $xml.children('value').text().trim();
            $Div.append(Value);

        },
        save: function ($propdiv, $xml) {
            // no changes to save
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeComposite = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + PluginName);
        }

    };
})(jQuery);
