/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswNodeGrid';

    var cswPrivate = {
        selectedRowId: ''
    };

    var methods = {

        'init': function (options) {

            var o = {
                runGridUrl: '/NbtWebApp/wsNBT.asmx/runGrid',
                viewid: '',
                showempty: false,
                ID: '',
                nodeid: '',
                cswnbtnodekey: '',
                reinit: false,
                forceFit: false,
                EditMode: Csw.enums.editMode.Edit,
                readonly: false,
                onEditNode: null,
                onDeleteNode: null,
                canSelectRow: false,
                onSelect: null,
                onSuccess: null,
                onEditView: null,
                onRefresh: null,
                height: '',
                includeInQuickLaunch: true
            };

            if (options) {
                $.extend(o, options);
            }
            var $parent = $(this);

            if (o.reinit) $parent.empty();

            var forReporting = (o.EditMode === Csw.enums.editMode.PrintReport),
                ret;

            /* fetchGridSkeleton */
            (function () {

                var parent = Csw.literals.factory($parent);
                ret = parent.grid({
                                    ID: o.ID,
                                    stateId: o.viewid,
                                    ajax: {
                                        url: o.runGridUrl,
                                        data: {
                                            ViewId: o.viewid,
                                            IncludeNodeKey: o.cswnbtnodekey,
                                            IncludeInQuickLaunch: o.includeInQuickLaunch,
                                            ForReport: forReporting
                                        }
                                    },
                                    forceFit: o.forceFit,
                                    usePaging: false === forReporting,
                                    showActionColumn: false === forReporting && false === o.readonly,
                                    height: o.height,
                                    canSelectRow: o.canSelectRow,
                                    onSelect: o.onSelect,
                                    onEdit: function(rows) {
                                        // this works for both Multi-edit and regular
                                        var cswnbtnodekeys = [],
                                            nodeids = [],
                                            nodenames = [];
    
                                        Csw.each(rows, function(row) {
                                            var keyName = Csw.string(o.viewid + '_nodekey').toLowerCase();
                                            var idName = Csw.string(o.viewid + '_nodeid').toLowerCase();
                                            var nameName = Csw.string(o.viewid + '_nodename').toLowerCase();
                                            cswnbtnodekeys.push(Csw.string(row[keyName]));
                                            nodeids.push(Csw.string(row[idName]));
                                            nodenames.push(Csw.string(row[nameName]));
                                        });

                                        $.CswDialog('EditNodeDialog', {
                                            nodeids: nodeids,
                                            nodepks: nodeids,
                                            nodekeys: cswnbtnodekeys,
                                            nodenames: nodenames,
                                            Multi: (nodeids.length > 1),
                                            onEditNode: o.onEditNode,
                                            onEditView: o.onEditView,
                                            onRefresh:  o.onRefresh
                                        });
                                    }, // onEdit
                                    onDelete: function(rows) {
                                        // this works for both Multi-edit and regular
                                        var cswnbtnodekeys = [],
                                            nodeids = [],
                                            nodenames = [];
    
                                        Csw.each(rows, function(row) {
                                            var keyName = Csw.string(o.viewid + '_nodekey').toLowerCase();
                                            var idName = Csw.string(o.viewid + '_nodeid').toLowerCase();
                                            var nameName = Csw.string(o.viewid + '_nodename').toLowerCase();
                                            cswnbtnodekeys.push(Csw.string(row[keyName]));
                                            nodeids.push(Csw.string(row[idName]));
                                            nodenames.push(Csw.string(row[nameName]));
                                        });

                                        $.CswDialog('DeleteNodeDialog', {
                                            nodeids: nodeids,
                                            nodepks: nodeids,
                                            nodekeys: cswnbtnodekeys,
                                            nodenames: nodenames,
                                            onDeleteNode: o.onDeleteNode,
                                            Multi: (nodeids.length > 1),
                                            publishDeleteEvent: false
                                        });
                                    } // onDelete
                                }); // grid()

                Csw.tryExec(o.onSuccess, ret);

            })();
            return ret;
        } // 'init'
    }; // methods

    $.fn.CswNodeGrid = function (method) {
        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }
    };

})(jQuery);

