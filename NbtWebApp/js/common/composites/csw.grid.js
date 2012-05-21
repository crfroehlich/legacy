/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.composites.grid = Csw.composites.grid ||
        Csw.composites.register('grid', function (cswParent, options) {
            ///<summary>Generates a grid</summary>
            ///<param name="cswParent" type="Csw.literals">Parent element to attach grid to.</param>
            ///<param name="options" type="Object">Object defining paramaters for jqGrid construction.</param>
            ///<returns type="Csw.composites.grid">Object representing a CswGrid</returns>
            'use strict';
            var cswPrivate = {
                canEdit: false,
                canDelete: false,
                pagermode: 'default',
                ID: '',
                resizeWithParent: false,
                onSuccess: null,
                gridOpts: {
                    autoencode: true,
                    autowidth: true,
                    altRows: false, //window.internetExplorerVersionNo === -1,
                    caption: '',
                    datatype: 'local',
                    emptyrecords: 'No Results',
                    height: '300',
                    hidegrid: false,
                    loadtext: 'Loading...',
                    multiselect: false,
                    toppager: false,
                    //forceFit: true,
                    shrinkToFit: true,
                    sortname: '',
                    sortorder: 'asc',
                    //width: '600px',
                    rowNum: 10,
                    rowList: [10, 25, 50],        /* page size dropdown */
                    pgbuttons: true,     /* page control like next, back button */
                    /*pgtext: null,         pager text like 'Page 0 of 10' */
                    viewrecords: true,    /* current view record text like 'View 1-10 of 100' */
                    onSelectRow: function () { }
                },
                optSearch: {
                    caption: "Search...",
                    Find: "Find",
                    Reset: "Reset",
                    odata: ['equal',
                        'not equal',
                        'less',
                        'less or equal',
                        'greater',
                        'greater or equal',
                        'begins with',
                        'does not begin with',
                        'is in',
                        'is not in',
                        'ends with',
                        'does not end with',
                        'contains',
                        'does not contain'],
                    groupOps: [{ op: "AND", text: "all" }, { op: "OR", text: "any"}],
                    matchText: "match",
                    rulesText: "rules"
                },
                optNavEdit: {
                    edit: true,
                    edittext: "",
                    edittitle: "Edit",
                    editfunc: null
                },
                optNavDelete: {
                    del: true,
                    deltext: "",
                    deltitle: "Delete",
                    delfunc: null
                },
                optNav: {
                    cloneToTop: false,

                    add: false,
                    del: false,
                    edit: false,

                    //search
                    search: false,
                    searchtext: "",
                    searchtitle: "Find records",


                    refresh: false,

                    alertcap: "Warning",
                    alerttext: "Please, select row",

                    //view
                    view: false,
                    viewtext: "",
                    viewtitle: "View"
                    //viewfunc: none--use jqGrid built-in function for read-only
                }
            };
            var cswPublic = {};

            cswPrivate.insertWhiteSpace = function (num) {
                var ret = '', i;
                for (i = 0; i < num; i += 1) {
                    ret += '&nbsp;';
                }
                return ret;
            };

            cswPrivate.makeCustomPager = function (pagerDef) {
                var prevButton = {
                    caption: cswPrivate.insertWhiteSpace(2),
                    buttonicon: 'ui-icon-seek-prev',
                    position: 'last',
                    title: '',
                    cursor: '',
                    id: Csw.makeId(cswPrivate.gridPagerId, 'prevBtn')
                };
                if (false === Csw.isNullOrEmpty(pagerDef) && Csw.isFunction(pagerDef.onPrevPageClick)) {
                    prevButton.onClickButton = function (eventObj) {
                        var nodes = cswPublic.gridTable.$.jqGrid('getDataIDs'),
                            firstNodeId = nodes[0],
                            lastNodeId = nodes[nodes.length],
                            firstRow = cswPublic.gridTable.$.jqGrid('getRowData', firstNodeId),
                            lastRow = cswPublic.gridTable.$.jqGrid('getRowData', lastNodeId);

                        pagerDef.onPrevPageClick(eventObj, firstRow, lastRow);
                    };
                }

                var spacer = {
                    sepclass: 'ui-separator',
                    sepcontent: cswPrivate.insertWhiteSpace(24)
                };

                var nextButton = {
                    caption: cswPrivate.insertWhiteSpace(2),
                    buttonicon: 'ui-icon-seek-next',
                    onClickButton: '',
                    position: 'last',
                    title: 'Next',
                    cursor: '',
                    id: Csw.makeId(cswPrivate.gridPagerId, 'nextBtn')
                };
                if (false === Csw.isNullOrEmpty(pagerDef) && Csw.isFunction(pagerDef.onNextPageClick)) {
                    nextButton.onClickButton = function (eventObj) {
                        var nodes = cswPublic.gridTable.$.jqGrid('getDataIDs'),
                            firstNodeId = nodes[0],
                            lastNodeId = nodes[nodes.length - 1],
                            firstRow = cswPublic.gridTable.$.jqGrid('getRowData', firstNodeId),
                            lastRow = cswPublic.gridTable.$.jqGrid('getRowData', lastNodeId);

                        pagerDef.onNextPageClick(eventObj, firstRow, lastRow);
                    };
                }
                cswPublic.gridTable.$.jqGrid('navSeparatorAdd', '#' + cswPrivate.gridPagerId, spacer)
                    .jqGrid('navButtonAdd', '#' + cswPrivate.gridPagerId, prevButton)
                    .jqGrid('navButtonAdd', '#' + cswPrivate.gridPagerId, nextButton);
            };

            cswPrivate.makeGrid = function () {
                cswPrivate.multiEdit = cswPrivate.gridOpts.multiselect;
                /* Case 25809 */
                cswPrivate.gridDiv.empty();

                cswPublic.gridTable = cswPrivate.gridDiv.table({
                    ID: cswPrivate.gridTableId
                });

                cswPublic.gridPager = cswPrivate.gridDiv.div({ ID: cswPrivate.gridPagerId });

                cswPrivate.gridOpts.pager = cswPublic.gridPager.$;
                if (cswPrivate.canEdit) {
                    $.extend(true, cswPrivate.optNav, cswPrivate.optNavEdit);
                }
                if (cswPrivate.canDelete) {
                    $.extend(true, cswPrivate.optNav, cswPrivate.optNavDelete);
                }

                if (cswPrivate.pagermode === 'default' || cswPrivate.pagermode === 'custom') {
                    try {
                        if (false === Csw.contains(cswPrivate.gridOpts, 'colNames') ||
                            cswPrivate.gridOpts.colNames.length === 0 ||
                                (Csw.contains(cswPrivate.gridOpts, 'colModel') && cswPrivate.gridOpts.colNames.length !== cswPrivate.gridOpts.colModel.length)) {
                            throw new Error('Cannot create a grid without at least one column defined.');
                        }
                        cswPublic.gridTable.$.jqGrid(cswPrivate.gridOpts)
                            .jqGrid('navGrid', '#' + cswPrivate.gridPagerId, cswPrivate.optNav, {}, {}, {}, {}, {}); //Case 24032: Removed jqGrid search
                    } catch (e) {
                        Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, e.message));
                    }
                    if (cswPrivate.pagermode === 'custom') {
                        cswPrivate.makeCustomPager(cswPrivate.customPager);
                    }
                } else {
                    cswPublic.gridTable.$.jqGrid(cswPrivate.gridOpts);
                }
                Csw.tryExec(cswPrivate.onSuccess, cswPublic);
            };

            cswPublic.getCell = function (rowid, key) {
                ///<summary>Gets the contents of a jqGrid cell by rowid and column key</summary>
                var ret = '';
                if (false === Csw.isNullOrEmpty(rowid) && false === Csw.isNullOrEmpty(key)) {
                    ret = cswPublic.gridTable.$.jqGrid('getCell', rowid, key);
                }
                return ret;
            };

            cswPublic.getDataIds = function () {
                ///<summary>Gets the contents of a jqGrid column</summary>
                return cswPublic.gridTable.$.jqGrid('getDataIDs');
            };

            cswPublic.getSelectedRowId = function () {
                var rowid = cswPublic.gridTable.$.jqGrid('getGridParam', 'selrow');
                return rowid;
            };

            cswPrivate.getSelectedRowsIds = function () {
                var rowid = cswPublic.gridTable.$.jqGrid('getGridParam', 'selarrrow');
                return rowid;
            };

            cswPrivate.getColumn = function (column, returnType) {
                ///<summary>Gets the contents of a jqGrid column</summary>
                ///<param name="column" type="String">Column name</param>
                ///<param name="returnType" type="Boolean">If false, returns a simple array of values. If true, returns an array [{id: id, value: value},{...}]</param>
                ///<returns type="Array">An array of the columns values</returns>
                var ret = cswPublic.gridTable.$.jqGrid('getCol', column, returnType);
                return ret;
            };

            cswPublic.hideColumn = function (colName) {
                ///<summary>Hides a column by name</summary>
                cswPublic.gridTable.$.jqGrid('hideCol', colName);
            };

            cswPublic.scrollToRow = function (rowid) {
                ///<summary>Scrolls the grid to the specified rowid</summary>
                ///<param name="rowid" type="String">Optional. jqGrid rowid. If null, selected row is assumed.</param>
                ///<returns type="Void"></returns>
                if (Csw.isNullOrEmpty(rowid)) {
                    rowid = cswPublic.getSelectedRowId();
                }
                var rowHeight = cswPublic.getGridRowHeight() || 23; // Default height
                var index = cswPublic.gridTable.$.getInd(rowid);
                cswPublic.gridTable.$.closest(".ui-jqgrid-bdiv").scrollTop(rowHeight * (index - 1));
            };

            cswPublic.getRowIdForVal = function (value, column) {
                ///<summary>Gets a jqGrid rowid by column name and value.</summary>
                ///<param name="value" type="String">Cell value</param>
                ///<param name="column" type="String">Column name</param>
                ///<returns type="String">jqGrid row id.</returns>
                var pks = cswPrivate.getColumn(column, true);
                var rowid = 0;
                Csw.each(pks, function (obj) {
                    if (Csw.contains(obj, 'value') && Csw.string(obj.value) === Csw.string(value)) {
                        rowid = obj.id;
                    }
                });
                return rowid;
            };

            cswPublic.getValueForColumn = function (columnname, rowid) {
                ///<summary>Gets a cell value by column name.</summary>
                ///<param name="columnname" type="String">Grid column name.</param>
                ///<param name="rowid" type="String">Optional. If null, selected row is assumed.</param>
                ///<returns type="String">Value of the cell.</returns>
                if (Csw.isNullOrEmpty(rowid)) {
                    rowid = cswPublic.getSelectedRowId();
                }
                var ret = cswPublic.getCell(rowid, columnname);
                return ret;
            };

            cswPublic.setRowData = function (rowId, columnName, columnData) {
                ///<summary>Update a cell with new content.</summary>
                var cellData = {};
                cellData[columnName] = columnData;
                return cswPublic.gridTable.$.jqGrid('setRowData', rowId, cellData);
            };

            cswPublic.setSelection = function (rowid) {
                ///<summary>Sets the selected row by jqGrid's rowid</summary>
                if (Csw.isNullOrEmpty(rowid)) {
                    rowid = cswPublic.getSelectedRowId();
                }
                if (false === Csw.isNullOrEmpty(rowid)) {
                    cswPublic.gridTable.$.jqGrid('setSelection', rowid);
                }
            };

            cswPublic.resetSelection = function () {
                ///<summary>Deselects all grid rows.</summary>
                cswPublic.gridTable.$.jqGrid('resetSelection');
            };

            cswPublic.changeGridOpts = function (opts, toggleColumns) {
                var delBtn, editBtn;
                $.extend(true, cswPrivate, opts);
                cswPrivate.makeGrid(cswPrivate);

                Csw.each(toggleColumns, function (val) {
                    if (Csw.contains(cswPrivate.gridOpts.colNames, val)) {
                        if (cswPublic.isMulti()) {
                            cswPublic.gridTable.$.jqGrid('hideCol', val);
                        }
                    }
                });
                if (false === cswPublic.isMulti()) {
                    if (false === cswPrivate.canEdit) {
                        editBtn = cswPublic.gridPager.find('#edit_' + cswPrivate.gridTableId);
                        if (Csw.contains(editBtn, 'remove')) {
                            editBtn.remove();
                        }
                    }
                    if (false === cswPrivate.canDelete) {
                        delBtn = cswPublic.gridPager.find('#del_' + cswPrivate.gridTableId).remove();
                        if (Csw.contains(delBtn, 'remove')) {
                            delBtn.remove();
                        }
                    }
                }
                cswPublic.resizeWithParent(cswPrivate.resizeWithParentElement);
            };

            cswPublic.opGridRows = function (opts, rowid, onSelect, onEmpty) {
                var ret = false;
                var haveSelectedRows = false,
                    i;

                var rowids = [];

                function onEachGridRow(prop, key, parent) {
                    if (false === Csw.isFunction(parent[key])) {
                        if (Csw.isArray(parent[key])) {
                            rowid = rowids[i];
                            parent[key].push(cswPublic.getValueForColumn(key, rowid));
                        } else {
                            parent[key] = cswPublic.getValueForColumn(key, rowid);
                        }
                    }
                    return false;
                }

                if (cswPrivate.multiEdit) {
                    rowids = cswPrivate.getSelectedRowsIds();
                } else if (false === Csw.isNullOrEmpty(rowid)) {
                    rowids.push(rowid);
                } else {
                    rowids.push(cswPublic.getSelectedRowId());
                }

                if (rowids.length > 0) {
                    haveSelectedRows = true;
                    for (i = 0; i < rowids.length; i += 1) {
                        Csw.crawlObject(opts, onEachGridRow, false);
                    }
                }

                if (haveSelectedRows) {
                    if (Csw.isFunction(onSelect)) {
                        opts.Multi = cswPrivate.multiEdit;
                        ret = onSelect(opts);
                    }
                } else if (Csw.isFunction(onEmpty)) {
                    onEmpty(opts);
                }
                return ret;
            };

            cswPublic.getAllGridRows = function () {
                return cswPublic.gridTable.$.jqGrid('getRowData');
            };

            cswPublic.print = function (onSuccess) {

                try {

                    var outerDiv = cswParent.div();
                    var newDiv = outerDiv.div({ width: '800px' });

                    var printOpts = {},
                        printTableId = Csw.makeId(cswPrivate.gridTableId, 'printTable'),
                        newGrid, data, i;

                    var addRowsToGrid = function (rowData) {
                        if (rowData) {
                            /* Add the rows to the new newGrid */
                            for (i = 0; i <= rowData.length; i += 1) {
                                newGrid.gridTable.$.jqGrid('addRowData', i + 1, rowData[i]);
                            }
                        }
                    };

                    /* Case 26020 */
                    $.extend(true, printOpts, cswPrivate);

                    /* Nuke anything that might be holding onto a reference */
                    Csw.each(printOpts, function (thisObj, name) {
                        if (Csw.isFunction(thisObj) || Csw.isJQuery(thisObj)) {
                            delete printOpts[name];
                        }
                    });

                    printOpts.ID = printTableId;

                    /* 
                    Nuke any existing options with vanilla defaults.
                    Since jqGrid 3.6, there hasn't been an 'All' rowNum option. Just use a really high number.
                    */
                    delete printOpts.gridOpts.canEdit;
                    delete printOpts.gridOpts.canDelete;
                    delete printOpts.canEdit;
                    delete printOpts.canDelete;

                    printOpts.gridPagerId += '_print';
                    printOpts.gridTableId += '_print';
                    printOpts.gridOpts.rowNum = 100000;
                    printOpts.gridOpts.rowList = [100000];
                    printOpts.gridOpts.add = false;
                    printOpts.gridOpts.del = false;
                    printOpts.gridOpts.edit = false;
                    printOpts.gridOpts.autoencode = true;
                    //printOpts.gridOpts.autowidth = true;
                    printOpts.gridOpts.width = 800;
                    printOpts.gridOpts.altRows = false;
                    printOpts.gridOpts.datatype = 'local';
                    delete printOpts.gridOpts.url;
                    printOpts.gridOpts.emptyrecords = 'No Results';
                    printOpts.gridOpts.height = 'auto';
                    printOpts.gridOpts.multiselect = false;
                    printOpts.gridOpts.toppager = false;
                    //printOpts.gridOpts.forceFit = true;
                    //printOpts.gridOpts.shrinkToFit = true;

                    /*
                    jqGrid cannot seem to handle the communication of the data property between window objects.
                    --Just delete it and rebuild instead.
                    DON'T just delete it. You're deleting the reference to the current grid rows. Bad idea.
                    */
                    data = cswPrivate.gridOpts.data;

                    Csw.each(printOpts.gridOpts.colModel, function (column) {
                        /* This provides text wrapping in cells */
                        column.cellattr = function () {
                            return 'style="white-space: normal;"';
                        };
                    });
                    Csw.tryExec(onSuccess, newDiv);
                    /* Get a new Csw.newGrid */
                    newGrid = newDiv.grid(printOpts);
                    newGrid.gridTable.$.jqGrid('hideCol', 'Action');

                    if (Csw.isNullOrEmpty(data) && false === Csw.isNullOrEmpty(printOpts.printUrl)) {
                        Csw.ajax.get({
                            url: printOpts.printUrl,
                            success: function (rows) {
                                addRowsToGrid(rows.rows);
                            }
                        });
                    } else {
                        /* Get the data (rows) from the current grid */
                        addRowsToGrid(data);
                    }



                    Csw.newWindow(outerDiv.$.html());
                    outerDiv.remove();

                } catch (e) {
                    Csw.log(e);
                }

            };

            // Row scrolling adapted from 
            // http://stackoverflow.com/questions/2549466/is-there-a-way-to-make-jqgrid-scroll-to-the-bottom-when-a-new-row-is-added/2549654#2549654
            cswPublic.getGridRowHeight = function () {

                var height = null; // Default
                try {
                    height = cswPublic.gridTable.$.find('tbody').find('tr:first').outerHeight();
                } catch (e) {
                    //catch and just suppress error
                }
                return height;
            };

            cswPublic.isMulti = function () {
                return cswPrivate.multiEdit;
            };

            cswPublic.setWidth = function (width) {
                cswPublic.gridTable.$.jqGrid('setGridWidth', width);
            };

            cswPublic.resizeWithParent = function () {
                var i = 0;
                function handleRestoreDownRecursive($elem) {
                    i += 1;
                    if ($elem.width() !== null &&
                        $elem.parent().width() !== null) {
                        if ($elem.parent().width() < $elem.width()) {
                            element = $elem.parent();
                        } else if (i <= 15) {
                            handleRestoreDownRecursive($elem.parent());
                        }
                    }
                }
                var element = cswPrivate.resizeWithParentElement || cswParent.$;
                handleRestoreDownRecursive(element);
                var width = element.width() - 100;

                cswPublic.gridTable.$.jqGrid('setGridWidth', width);
            };

            /* "Constuctor" */
            (function () {
                $.extend(true, cswPrivate, options);

                switch (cswPrivate.pagermode) {
                    case 'none':
                        delete cswPrivate.gridOpts.pager;
                        //delete cswPrivate.gridOpts.rowNum;
                        //delete cswPrivate.gridOpts.rowList;
                        delete cswPrivate.gridOpts.pgbuttons;
                        delete cswPrivate.gridOpts.viewrecords;
                        delete cswPrivate.gridOpts.pgtext;
                        break;
                    case 'default':
                        //accept defaults
                        break;
                    case 'custom':
                        cswPrivate.gridOpts.rowNum = null;
                        cswPrivate.gridOpts.rowList = [];
                        cswPrivate.gridOpts.pgbuttons = false;
                        cswPrivate.gridOpts.viewrecords = false;
                        cswPrivate.gridOpts.pgtext = null;
                        break;
                }

                cswPrivate.gridPagerId = cswPrivate.gridPagerId || Csw.makeId('cswGridPager', cswPrivate.ID);
                cswPrivate.gridTableId = cswPrivate.gridTableId || Csw.makeId('cswGridTable', cswPrivate.ID);

                cswParent.empty();
                cswPrivate.gridDiv = cswParent.div({
                    isControl: cswPrivate.isControl,
                    ID: cswPrivate.ID
                });
                //$.extend(cswPublic, Csw.literals.div(cswPrivate));
                if (cswPrivate.resizeWithParent) {
                    $(window).bind('resize', cswPublic.resizeWithParent);
                }
                cswPrivate.makeGrid();
                cswPublic.resizeWithParent();
            } ());


            return cswPublic;
        });

} ());
