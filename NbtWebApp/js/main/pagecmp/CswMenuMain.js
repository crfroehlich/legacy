/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";
    $.fn.CswMenuMain = function (options) {
    /// <summary>
    ///   Generates an action menu for the current view
    /// </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - options.viewid: a viewid
    ///     &#10;2 - options.nodeid: nodeid
    ///     &#10;3 - options.cswnbtnodekey: a node key
    ///     &#10;4 - options.onAddNode: function () {}
    ///     &#10;5 - options.onMultiEdit: function () {}
    ///     &#10;6 - options.onSearch: { onViewSearch: function () {}, onGenericSearch: function () {} }
    ///     &#10;7 - options.onEditView: function () {}
    /// </param>
        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/getMainMenu',
            viewid: '',
            nodeid: '',
            cswnbtnodekey: '',
            propid: '',
            onAddNode: null, // function (nodeid, cswnbtnodekey) { },
            onMultiEdit: null, // function () { },
            onSearch: {
                 onViewSearch: null, // function () {}, 
                 onGenericSearch: null // function () {}
            },
            onEditView: null, // function (viewid) { },
            onSaveView: null, // function (newviewid) { },
            Multi: false,
            NodeCheckTreeId: '',
            limitMenuTo: ''
        };
        if (options) $.extend(o, options);

        var $MenuDiv = $(this);

        var jsonData = {
            ViewId: o.viewid,
            SafeNodeKey: o.cswnbtnodekey,
            PropIdAttr: o.propid,
            LimitMenuTo: o.limitMenuTo
        };

        Csw.ajax.post({
            url: o.Url,
            data: jsonData,
            stringify: false,
            success: function (data) {
                var $ul = $('<ul class="topnav"></ul>');

                $MenuDiv.text('')
                    .append($ul);

                for (var itemKey in data) {
                    if (data.hasOwnProperty(itemKey)) {

                        var menuItem = data[itemKey];
                        if (!Csw.isNullOrEmpty(itemKey))
                        {
                            var menuItemOpts = {
                                $ul: $ul,
                                itemKey: itemKey,
                                itemJson: menuItem,
                                onAlterNode: o.onAddNode,
                                onMultiEdit: o.onMultiEdit,
                                onEditView: o.onEditView,
                                onSaveView: o.onSaveView,
                                onSearch: o.onSearch,
                                onPrintView: o.onPrintView,
                                Multi: o.Multi,
                                NodeCheckTreeId: o.NodeCheckTreeId
                            };
                            var $li = Csw.handleMenuItem(menuItemOpts);

                            if (Csw.bool(menuItem.haschildren)) {
                                delete menuItem.haschildren;
                                var $subul = $('<ul class="subnav"></ul>')
                                    .appendTo($li);
                                for (var childItem in menuItem) {
                                    if (menuItem.hasOwnProperty(childItem)) {
                                        var thisChild = menuItem[childItem];
                                        var subMenuItemOpts = {
                                            $ul: $subul,
                                            itemKey: childItem,
                                            itemJson: thisChild
                                        };
                                        $.extend(menuItemOpts, subMenuItemOpts);
                                        Csw.handleMenuItem(menuItemOpts);
                                    }
                                }
                            }
                        }
                    }
                }

                $ul.CswMenu();

            } // success{}
        }); // $.ajax({

        // For proper chaining support
        return this;

    }; // function (options) {
})(jQuery);

