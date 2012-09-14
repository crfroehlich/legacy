
/// <reference path="~/app/CswApp-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswNodeGrid';

    var cswPrivate = {
        selectedRowId: ''
    };

    var methods = {

        'init': function (options) {

            var o = {
                runGridUrl: 'runGrid',
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
                Csw.extend(o, options);
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
                                        urlMethod: o.runGridUrl,
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
                                            cswnbtnodekeys.push(row.nodekey);
                                            nodeids.push(row.nodeid);
                                            nodenames.push(row.nodename);
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
                                            cswnbtnodekeys.push(row.nodekey);
                                            nodeids.push(row.nodeid);
                                            nodenames.push(row.nodename);
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
