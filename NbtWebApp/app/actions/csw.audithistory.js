/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.auditHistory = Csw.actions.auditHistory ||
        Csw.actions.register('auditHistory', function (cswParent, options) {
            'use strict';

            var cswPrivate = {
                urlMethod: 'getAuditHistoryGrid',
                ID: '',
                nodeid: '',
                EditMode: Csw.enums.editMode.Edit,
                JustDateColumn: false,
                onEditRow: null, //function (date) {},
                onSelectRow: null, //function (date) {},
                selectedDate: '',
                allowEditRow: true,
                width: 180,
                preventSelectTrigger: false //,
//                canEdit: true,
//                pagermode: 'default',
//                gridOpts: {
//                    height: 180,
//                    rowNum: 10,
//                    onSelectRow: function (selRowid) {
//                        if (false === cswPrivate.preventSelectTrigger && false === Csw.isNullOrEmpty(selRowid)) {
//                            var cellVal = cswPublic.grid.getValueForColumn('CHANGEDATE', selRowid);
//                            Csw.tryExec(cswPrivate.onSelectRow, cellVal);
//                        }
//                    },
//                    add: false,
//                    view: false,
//                    del: false,
//                    refresh: false,
//                    search: false
//                }
            };
            if (options) Csw.extend(cswPrivate, options);

            var cswPublic = { };
            cswParent.empty();

            var gridId = cswPrivate.ID + '_auditGrid';
            cswParent.grid({
                ID: gridId,
                storeId: gridId,
                title: 'History',
                stateId: '',
                width: cswPrivate.width,

                showActionColumn: cswPrivate.allowEditRow,
                showView: false,
                showLock: false,
                showEdit: true,
                showDelete: false,
                canSelectRow: cswPrivate.JustDateColumn,

                usePaging: (cswPrivate.EditMode !== Csw.enums.editMode.PrintReport),
                ajax: {
                    urlMethod: cswPrivate.urlMethod,
                    data: {
                        NodeId: Csw.string(cswPrivate.nodeid),
                        NbtNodeKey: Csw.string(cswPrivate.cswnbtnodekey),
                        JustDateColumn: cswPrivate.JustDateColumn
                    }
                },
                onLoad: function(grid)
                {
                    if (false === Csw.isNullOrEmpty(cswPrivate.selectedDate)) {
                        cswPrivate.preventSelectTrigger = true;
                        var rowid = grid.getRowIdForVal('changedate', cswPrivate.selectedDate.toString());
                        grid.setSelection(rowid);
                        cswPrivate.preventSelectTrigger = false;
                    }
                },
                onSelect: function (row) {
                    if (false === cswPrivate.preventSelectTrigger && false === Csw.isNullOrEmpty(row)) {
                        Csw.tryExec(cswPrivate.onSelectRow, row.changedate);
                    }
                },
                onEdit: function (rows) {
                    Csw.tryExec(cswPrivate.onEditRow, rows[0].changedate);
                }
            });
            // set selected row by date
        });
} ());