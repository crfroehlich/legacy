/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswNodeGrid';

    function deleteRows(rowid, grid, func) {
        var delOpt = {
            cswnbtnodekey: [],
            nodename: []
        };
        var delFunc = function (opts) {
            opts.onDeleteNode = func;
            opts.publishDeleteEvent = false;
            Csw.renameProperty(opts, 'cswnbtnodekey', 'cswnbtnodekeys');
            Csw.renameProperty(opts, 'nodename', 'nodenames');
            $.CswDialog('DeleteNodeDialog', opts);
        };
        var emptyFunc = function () {
            $.CswDialog('AlertDialog', 'Please select a row to delete');
        };
        return grid.opGridRows(delOpt, rowid, delFunc, emptyFunc);
    }

    function editRows(rowid, grid, func, editViewFunc) {
        var editOpt = {
            cswnbtnodekey: [],
            nodename: []
        };
        var editFunc = function (opts) {
            opts.onEditNode = func;
            opts.onEditView = editViewFunc;
            Csw.renameProperty(opts, 'cswnbtnodekey', 'nodekeys');
            Csw.renameProperty(opts, 'nodename', 'nodenames');
            $.CswDialog('EditNodeDialog', opts);
        };
        var emptyFunc = function () {
            $.CswDialog('AlertDialog', 'Please select a row to edit');
        };
        return grid.opGridRows(editOpt, rowid, editFunc, emptyFunc);
    }

    var methods = {

        'init': function (optJqGrid) {

            var o = {
                runGridUrl: '/NbtWebApp/wsNBT.asmx/runGrid',
                gridPageUrl: '/NbtWebApp/wsNBT.asmx/getGridRowsByPage',
                gridAllRowsUrl: '/NbtWebApp/wsNBT.asmx/getAllGridRows',
                viewid: '',
                showempty: false,
                ID: '',
                nodeid: '',
                cswnbtnodekey: '',
                reinit: false,
                EditMode: Csw.enums.editMode.Edit,
                onEditNode: null,
                onDeleteNode: null,
                onSuccess: null,
                onEditView: null
            };

            if (optJqGrid) {
                $.extend(o, optJqGrid);
            }
            var $parent = $(this);

            function getGridRowsUrl(isPrint) {
                var url = o.gridPageUrl + '?ViewId=' + o.viewid + '&IsReport=' + Csw.bool(forReporting || isPrint).toString() + '&IncludeNodeKey=' + o.cswnbtnodekey;
                if (isPrint) {
                    url += '&Page=1&Rows=100000000';
                }
                return url;
            }

            if (o.reinit) $parent.empty();

            var forReporting = (o.EditMode === Csw.enums.editMode.PrintReport),
                ret, doPaging = false;

            /* fetchGridSkeleton */
            (function () {

                //Get the grid skeleton definition
                (function () {
                    Csw.ajax.post({
                        url: o.runGridUrl,
                        data: {
                            ViewId: o.viewid,
                            IncludeNodeKey: o.cswnbtnodekey,
                            IncludeInQuickLaunch: true
                        },
                        success: function (data) {
                            buildGrid(data);
                        }
                    });
                } ());

                //jqGrid will handle the rest
                var buildGrid = function (gridJson) {

                    var makeGrid = function (pagerMode, data) {
                        var jqGridOpt = gridJson.jqGridOpt;

                        var cswGridOpts = {
                            ID: o.ID,
                            canEdit: (Csw.bool(jqGridOpt.CanEdit) && false === forReporting),
                            canDelete: (Csw.bool(jqGridOpt.CanDelete) && false === forReporting),
                            pagermode: 'default',
                            gridOpts: {}, //toppager: (jqGridOpt.rowNum >= 50 && Csw.contains(gridJson, 'rows') && gridJson.rows.length >= 49)
                            optNav: {},
                            optSearch: {},
                            optNavEdit: {},
                            optNavDelete: {}
                        };
                        $.extend(cswGridOpts.gridOpts, jqGridOpt);

                        if (Csw.isNullOrEmpty(cswGridOpts.gridOpts.width)) {
                            cswGridOpts.gridOpts.width = '650px';
                        }

                        if (forReporting) {
                            cswGridOpts.gridOpts.caption = '';
                        } else {
                            cswGridOpts.optNavEdit = {
                                editfunc: function (rowid) {
                                    return editRows(rowid, ret, o.onEditNode, o.onEditView);
                                }
                            };

                            cswGridOpts.optNavDelete = {
                                delfunc: function (rowid) {
                                    return deleteRows(rowid, ret, o.onDeleteNode);
                                }
                            };
                        }

                        switch (pagerMode) {
                            case 'local':
                                cswGridOpts.gridOpts.datatype = 'local';
                                cswGridOpts.gridOpts.data = data.rows;
                                break;
                            case 'server':
                                /*
                                This is the root of much suffering. 3rd-party libs which use jQuery.ajax() frequently screw it up such that the request is sent with an invalid return type.
                                .NET will helpfully wrap the response in XML for you. 
                                This is actually not helpful.

                                We can either overwrite jqGrid's ajax implementation: in which case we have to build the _WHOLE_ thing,
                                or we can modify the HTTPContext.Response object directly. 
                                
                                In the case of the latter, we need to guarantee that ONLY jqGrid properties defined in the jsonReader property are returned from the server.

                                cswGridOpts.gridOpts.ajaxGridOptions = {
                                url: o.gridPageUrl,
                                dataType: 'json',
                                contentType: 'application/json; charset=utf-8',
                                type: 'POST',
                                data: JSON.stringify({
                                ViewId: o.viewid, Page: currentPage(), PageSize: 50, IsReport: forReporting  
                                })
                                };

                                */
                                cswGridOpts.gridOpts.datatype = 'json';
                                cswGridOpts.gridOpts.url = getGridRowsUrl();
                                cswGridOpts.gridOpts.jsonReader = {
                                    root: 'rows',
                                    page: 'page',
                                    total: 'total',
                                    records: 'records',
                                    repeatitems: false
                                };
                                break;
                        }

                        cswGridOpts.printUrl = getGridRowsUrl(true);
                        cswGridOpts.$parent = $parent;
                        ret = Csw.controls.grid(cswGridOpts);

                        if (Csw.isFunction(o.onSuccess)) {
                            o.onSuccess(ret);
                        }
                    };

                    if (false === doPaging) {
                        Csw.ajax.post({
                            url: o.gridAllRowsUrl,
                            data: {
                                ViewId: o.viewid,
                                IsReport: forReporting,
                                IncludeNodeKey: o.cswnbtnodekey
                            },
                            success: function (data) {
                                makeGrid('local', data);
                            }
                        });
                    } else {
                        makeGrid('server');
                    }
                };
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
