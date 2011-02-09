﻿; (function ($) {
    $.fn.CswViewTree = function (options) {

        var o = {
            ViewUrl: '/NbtWebApp/wsNBT.asmx/getViewTree',
            viewid: '',
            onSelect: function (itemid, text, iconurl) { }
        };

        if (options) {
            $.extend(o, options);
        }

        var $viewsdiv = $(this);
        $viewsdiv.children().remove();

        CswAjaxXml({
                url: o.ViewUrl,
                data: '',
                success: function ($xml)
                {
                    var strTypes = $xml.find('types').text();
                    var jsonTypes = $.parseJSON(strTypes);
                    var $treexml = $xml.find('tree').children('root')
                    var treexmlstring = xmlToString($treexml);

                    $viewsdiv.jstree({
                        "xml_data": {
                            "data": treexmlstring,
                            "xsl": "nest"
                        },
                        "ui": {
                            "select_limit": 1
                        },
                        "core": {
                            "initially_open": ["root"]
                        },
                        "types": {
                            "types": jsonTypes
                        },
                        "plugins": ["themes", "xml_data", "ui", "types"]
                    }).bind('select_node.jstree', 
                                function (e, data) {
                                    var Selected = jsTreeGetSelected($viewsdiv); 
                                    o.onSelect(Selected.SelectedId, Selected.SelectedText, Selected.SelectedIconUrl);
                                });

                } // success{}
            });
        
        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);
