/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../controls/CswGrid.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    $.fn.CswInspectionStatus = function (options) {
        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/getInspectionStatusGrid',
            onEditNode: function () {},
            ID: 'CswInspectionStatus'
        };
        if (options) $.extend(o, options);

        var $parent = $(this);
        var $div = $('<div></div>')
            .appendTo($parent);


        Csw.ajax({
                url: o.Url,
                data: { },
                success: function (gridJson) {

                    var inspGridId = o.ID + '_csw_inspGrid_outer';
                    var $inspGrid = $div.find('#' + inspGridId);
                    if (Csw.isNullOrEmpty($inspGrid) || $inspGrid.length === 0) {
                        $inspGrid = $('<div id="' + o.ID + '"></div>').appendTo($div);
                    } else {
                        $inspGrid.empty();
                    }

                    var g = {
                        Id: o.ID,
                        gridOpts: {
                            autowidth: true,
                            rowNum: 10
                        },
                        pagermode: 'none',
                        optNav: {
                            add: false,
                            view: false,
                            del: false,
                            refresh: false,
                            edit: true,
                            edittext: "",
                            edittitle: "Edit row",
                            editfunc: function (rowid) {
                                var editOpt = {
                                    nodeids: [],
                                    onEditNode: o.onEditNode
                                };
                                if (false === Csw.isNullOrEmpty(rowid)) {
                                    editOpt.nodeids.push(grid.getValueForColumn('NODEPK', rowid));
                                    $.CswDialog('EditNodeDialog', editOpt);
                                } else {
                                    $.CswDialog('AlertDialog', 'Please select a row to edit');
                                }
                            }
                        }
                    };


                    $.extend(g.gridOpts, gridJson);

                    var grid = CswGrid(g, $inspGrid);
                    grid.hideColumn('NODEID');
                    grid.hideColumn('NODEPK');

                }, // success
                'error': function ()
                {
                }
            });

        return $div;

    }; // $.fn.CswInspectionStatus
}) (jQuery);

