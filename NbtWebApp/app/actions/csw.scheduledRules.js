/*global Csw:true*/
(function () {


    Csw.actions.register('scheduledRules', function (cswParent, cswPrivate) {
        'use strict';

        //#region _preCtor

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate.name = cswPrivate.name || 'CswScheduledRules';
            cswPrivate.onCancel = cswPrivate.onCancel || function () { };
            cswParent.empty();
        }());


        //#endregion _preCtor

        //#region Tab Functions

        cswPrivate.tabNames = ['Rules', 'Timeline'];

        cswPrivate.tryParseTabName = function (tabName, elTarget, eventObjText) {
            var tab = '', ret = '';
            if (tabName) {
                tab = tabName.split(' ')[0].trim();
                if (cswPrivate.tabNames.indexOf(tab) === -1) {
                    if (eventObjText) {
                        tab = cswPrivate.tryParseTabName(eventObjText, elTarget);
                        if (cswPrivate.tabNames.indexOf(tab) === -1) {
                            tab = cswPrivate.tryParseTabName(elTarget);
                            if (cswPrivate.tabNames.indexOf(tab) !== -1) {
                                ret = tab;
                            }
                        } else {
                            ret = tab;
                        }
                    }
                } else {
                    ret = tab;
                }
            }
            return ret;
        };

        cswPrivate.onTabSelect = function (tabName, el, eventObj, callBack) {
            var tgtTxt = null, evtTxt;
            if (tabName.indexOf('<') === 0 &&
                tabName.lastIndexOf('>') === tabName.length - 1) {
                if (el) {
                    tgtTxt = el.target.innerText;
                }

                if (eventObj) {
                    evtTxt = eventObj.innerText;
                }
            } else {
                if (tabName.length > 20) {
                    // yuck. Clicking anywhere inside the tab fires this event. That includes clicking a grid row whose nodename begins [Tabname].
                    tabName = '';
                }
            }
            var newTabName = cswPrivate.tryParseTabName(tabName, tgtTxt, evtTxt);
            if (cswPrivate.selectedTab !== newTabName) {
                if (newTabName.indexOf('Rules') !== -1) {
                    cswPrivate.selectedTab = 'Rules';
                    cswPrivate.makeRulesTab();
                } else if (newTabName.indexOf('Timeline') !== -1) {
                    cswPrivate.selectedTab = 'Timeline';
                    cswPrivate.makeTimelineTab();
                }
            }
        };

        cswPrivate.prepTab = function (tab, title, headerText) {

            tab.csw.empty().css({ margin: '10px' });

            var ol = tab.csw.ol();

            ol.li().span({
                text: headerText
            });
            ol.li().br({ number: 2 });

            return ol;
        };
        
        //#endregion Tab Functions
        
        //#region Tab construction

        cswPrivate.makeRulesTab = function () {
            var ol = cswPrivate.prepTab(cswPrivate.rulesTab, 'Rules', 'Select a Customer to review and make any necessary edits to the Scheduled Rules for their schema.');

            cswPrivate.makeCustomerIdSelect(ol.li()).then(function () {
                ol.li().br({ number: 2 });

                cswPrivate.makeScheduledRulesGrid(ol.li());

                ol.li().br({ number: 2 });

                cswPrivate.addBtnGroup(ol.li());
            });
        };

        cswPrivate.makeTimelineTab = function () {
            var ol = cswPrivate.prepTab(cswPrivate.timelineTab, 'Timeline', 'View a timeline of scheduled rules.');
            ol.schedRulesTimeline();
        };

        cswPrivate.makeCustomerIdSelect = function (parentDiv) {
            var customerIdTable, customerIdSelect;

            customerIdTable = parentDiv.table({
                name: 'customerIdTable',
                cellpadding: '5px',
                cellvalign: 'middle',
                FirstCellRightAlign: true
            });

            customerIdTable.cell(1, 1).span({ text: 'Customer ID&nbsp' });

            customerIdSelect = customerIdTable.cell(1, 2).select({
                name: 'customerIdSelect',
                onChange: function () {
                    cswPrivate.selectedCustomerId = customerIdSelect.val();
                    cswPrivate.makeScheduledRulesGrid();
                }
            });
            
            customerIdTable.cell(1, 3).span({ text: '&nbsp;' });
            
            customerIdTable.cell(1, 4).buttonExt({
                name: 'refreshGrid',
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.refresh),
                enabledText: 'Refresh',
                disabledText: 'Refresh',
                onClick: function () {
                    cswPrivate.makeScheduledRulesGrid();
                }
            });
            
            customerIdTable.cell(1, 5).span({text:'&nbsp;'});

            cswPrivate.timeStamp = customerIdTable.cell(1, 6).span();
            
            var ret = Csw.ajax.deprecatedWsNbt({
                urlMethod: 'getActiveAccessIds',
                success: function (data) {
                    cswPrivate.customerIds = data.customerids;
                    if (cswPrivate.customerIds.length > 1) {
                        customerIdSelect.setOptions(cswPrivate.customerIds);
                        cswPrivate.selectedCustomerId = customerIdSelect.find(':selected').val();
                    } else {
                        customerIdSelect.empty();
                        customerIdSelect.option({ value: cswPrivate.customerIds[0], display: cswPrivate.customerIds[0], isSelected: true });
                        cswPrivate.selectedCustomerId = cswPrivate.customerIds[0];
                    }
                }
            });

            return ret;
        };
        
        //#region Scheduled Rules Grid

        cswPrivate.makeScheduledRulesGrid = function (parentDiv) {
            var gridId = 'rulesGrid';
            //Case 29587 - always use parentDiv when given (i.e. - when reloading entire tab)
            //else (when refreshing just the grid), keep cswPrivate.gridDiv 
            cswPrivate.gridDiv = parentDiv || cswPrivate.gridDiv;

            cswPrivate.gridAjax = Csw.ajaxWcf.post({
                urlMethod: 'Scheduler/get',
                data: cswPrivate.selectedCustomerId,
                success: function (result) {

                    cswPrivate.schedulerRequest = result;
                    var parsedRows = [];
                    if (result && result.Grid.data && result.Grid.data.items) {
                        result.Grid.data.items.forEach(function (row) {
                            var parsedRow = {
                                RowNo: row.RowNo,
                                canDelete: false,
                                canEdit: false,
                                canView: false,
                                isDisabled: false,
                                isLocked: false
                            };
                            if (row && row.Row) {
                                Object.keys(row.Row).forEach(function (key) {
                                    if (key === 'reprobate' || key === 'disabled') {
                                        parsedRow[key] = Csw.bool(row.Row[key]);
                                    } else {
                                        parsedRow[key] = row.Row[key];
                                    }
                                });
                            }
                            parsedRow.Row = row.Row;
                            parsedRows.push(parsedRow);
                        });
                        result.Grid.data.items = parsedRows;
                    }

                    var columns = result.Grid.columns;
                    columns.forEach(function (col) {
                        col.sortable = false;
                        col.filterable = false;
                        switch (col.header) {
                            case result.ColumnIds.failed_cnt:
                                col.editable = true;
                                Object.defineProperty(col, 'editor', {
                                    writable: true,
                                    configurable: true,
                                    enumerable: true,
                                    value: {
                                        xtype: 'numberfield',
                                        allowBlank: false,
                                        minValue: 0,
                                        maxValue: 10
                                    }
                                });
                                break;
                            case result.ColumnIds.freq:
                                col.editable = true;
                                Object.defineProperty(col, 'editor', {
                                    writable: true,
                                    configurable: true,
                                    enumerable: true,
                                    value: {
                                        xtype: 'numberfield',
                                        allowBlank: false,
                                        //TODO - these min/max values should be within the context of the selected Type (Minutes: 15-60, Daily: 1, DayOfYear: 1-365, etc)
                                        minValue: 1,
                                        maxValue: 365
                                    }
                                });
                                break;
                            case result.ColumnIds.type:
                                col.editable = true;
                                col.filter = {
                                    type: 'list',
                                    options: result.RecurrenceOptions
                                };
                                Object.defineProperty(col, 'editor', {
                                    writable: true,
                                    configurable: true,
                                    enumerable: true,
                                    value: new Ext.form.field.ComboBox({
                                        typeAhead: true,
                                        typeAheadDelay: 0,
                                        triggerAction: 'all',
                                        selectOnTab: true,
                                        allowBlank: false,
                                        valueNotFoundText: 'Could not find that value',
                                        validator: function (val) {
                                            return result.RecurrenceOptions.indexOf(val) !== -1;
                                        },
                                        store: [
                                            [result.RecurrenceOptions[0], result.RecurrenceOptions[0]],//NMinutes
                                            [result.RecurrenceOptions[1], result.RecurrenceOptions[1]],//NHours
                                            [result.RecurrenceOptions[2], result.RecurrenceOptions[2]],//Daily
                                            [result.RecurrenceOptions[3], result.RecurrenceOptions[3]],//DayOfWeek
                                            [result.RecurrenceOptions[4], result.RecurrenceOptions[4]],//DayOfMonth
                                            [result.RecurrenceOptions[5], result.RecurrenceOptions[5]],//DayOfYear
                                            [result.RecurrenceOptions[6], result.RecurrenceOptions[6]] //Never
                                        ],
                                        lazyRender: true
                                    })
                                });
                                break;
                            case result.ColumnIds.max_fail:
                                col.editable = true;
                                Object.defineProperty(col, 'editor', {
                                    writable: true,
                                    configurable: true,
                                    enumerable: true,
                                    value: {
                                        xtype: 'numberfield',
                                        allowBlank: false,
                                        minValue: 0,
                                        maxValue: 100
                                    }
                                });
                                break;
                            case result.ColumnIds.max_ms:
                                col.editable = true;
                                Object.defineProperty(col, 'editor', {
                                    writable: true,
                                    configurable: true,
                                    enumerable: true,
                                    value: {
                                        xtype: 'numberfield',
                                        allowBlank: false,
                                        minValue: 1000,
                                        maxValue: 10000000
                                    }
                                });
                                break;
                            case result.ColumnIds.reprobate:
                                col.editable = true;
                                col.xtype = 'checkcolumn';
                                col.listeners = {
                                    checkchange: function (checkbox, rowNum, isChecked) {
                                        cswPrivate.schedulerRequest.Grid.data.items[rowNum]['reprobate'] = isChecked;
                                        cswPrivate.schedulerRequest.Grid.data.items[rowNum].Row['reprobate'] = isChecked;
                                        cswPrivate.schedulerRequest.Grid.data.items[rowNum].Row['has_changed'] = 'true';
                                    }
                                };
                                col.editor = {
                                    writable: true,
                                    configurable: true,
                                    enumerable: true
                                };
                                break;
                            case result.ColumnIds.rogue_cnt:
                                col.editable = true;
                                Object.defineProperty(col, 'editor', {
                                    writable: true,
                                    configurable: true,
                                    enumerable: true,
                                    value: {
                                        xtype: 'numberfield',
                                        minValue: 0,
                                        maxValue: 10
                                    }
                                });
                                break;
                            case result.ColumnIds.status_message:
                                col.editable = true;
                                Object.defineProperty(col, 'editor', {
                                    writable: true,
                                    configurable: true,
                                    enumerable: true,
                                    value: {
                                        allowBlank: true
                                    }
                                });
                                break;
                            case result.ColumnIds.priority:
                                col.editable = true;
                                Object.defineProperty(col, 'editor', {
                                    writable: true,
                                    configurable: true,
                                    enumerable: true,
                                    value: {
                                        xtype: 'numberfield',
                                        allowBlank: false,
                                        minValue: 0,
                                        maxValue: 100
                                    }
                                });
                                break;
                            case result.ColumnIds.disabled:
                                col.editable = true;
                                col.xtype = 'checkcolumn';
                                col.listeners = {
                                    checkchange: function (checkbox, rowNum, isChecked) {
                                        cswPrivate.schedulerRequest.Grid.data.items[rowNum]['disabled'] = isChecked;
                                        cswPrivate.schedulerRequest.Grid.data.items[rowNum].Row['disabled'] = isChecked;
                                        cswPrivate.schedulerRequest.Grid.data.items[rowNum].Row['has_changed'] = 'true';
                                    }
                                };
                                col.editor = {
                                    writable: true,
                                    configurable: true,
                                    enumerable: true
                                };
                                break;
                            case result.ColumnIds.has_changed:
                                col.editable = true;
                                col.hidden = true;
                                Object.defineProperty(col, 'editor', {
                                    writable: true,
                                    configurable: true,
                                    enumerable: true,
                                    value: {
                                        xtype: 'booleancolumn',
                                        trueText: 'true',
                                        falseText: 'false',
                                    }
                                });
                                break;
                        }
                    });

                    if (cswPrivate.scheduledRulesGrid && cswPrivate.scheduledRulesGrid.destroy) {
                        cswPrivate.scheduledRulesGrid.destroy();
                    }
                    cswPrivate.gridDiv.empty();
                    cswPrivate.scheduledRulesGrid = cswPrivate.gridDiv.grid({
                        name: gridId,
                        storeId: gridId,
                        data: result.Grid,
                        stateId: gridId,
                        height: 400,
                        width: '98%',
                        title: 'Scheduled Rules',
                        usePaging: true,
                        showActionColumn: false,
                        canSelectRow: false,
                        selModel: {
                            selType: 'cellmodel'
                        },
                        plugins: [
                            Ext.create('Ext.grid.plugin.CellEditing', {
                                clicksToEdit: 1,
                                listeners: {
                                    edit: cswPrivate.onGridEdit
                                }
                            })
                        ]
                    });

                    cswPrivate.timeStamp.text('Last refreshed on ' + moment().format('L, HH:mm:ss'));

                } // success
            });
        };

        cswPrivate.onGridEdit = function (grid, row, opts) {
            if (cswPrivate.schedulerRequest.Grid.data.items[row.rowIdx][row.field] !== Csw.string(row.value)) {
                cswPrivate.schedulerRequest.Grid.data.items[row.rowIdx].Row['has_changed'] = 'true';
            }
            cswPrivate.schedulerRequest.Grid.data.items[row.rowIdx][row.field] = row.value;
            cswPrivate.schedulerRequest.Grid.data.items[row.rowIdx].Row[row.field] = row.value;
        };
        
        //#endregion Scheduled Rules Grid

        cswPrivate.addBtnGroup = function (el) {
            var tbl = el.table({ width: '98%', cellpadding: '5px' });
            
            tbl.cell(1, 1).css({ 'text-align': 'left' }).buttonExt({
                name: 'updateRules',
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                enabledText: 'Save Changes',
                disabledText: 'Saving . . . ',
                onClick: function () {
                    var req = Csw.extend({}, cswPrivate.schedulerRequest, true);
                    req.Grid.columns.forEach(function (col) {
                        delete col.editable;
                        delete col.editor;
                    });

                    Csw.ajaxWcf.post({
                        urlMethod: 'Scheduler/save',
                        data: req,
                        success: function () {
                            cswPrivate.makeScheduledRulesGrid();
                        }
                    });
                }
            });

            tbl.cell(1, 2).css({ 'text-align': 'right' }).buttonExt({
                enabledText: 'Close',
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.cancel),
                onClick: function () {
                    Csw.tryExec(cswPrivate.onCancel);
                }
            });
        };

        //#endregion Tab construction

        //#region _postCtor

        (function _postCtor() {

            cswParent.empty();
            cswPrivate.tabs = cswParent.tabStrip({
                onTabSelect: cswPrivate.onTabSelect,
                tabPanel: {
                    height: 700
                }
            });
            cswPrivate.tabs.setTitle('Scheduled Rules by Customer ID');

            cswPrivate.rulesTab = cswPrivate.tabs.addTab({
                title: 'Rules'
            });
            cswPrivate.timelineTab = cswPrivate.tabs.addTab({
                title: 'Timeline'
            });

            cswPrivate.tabs.setActiveTab(0);
            cswPrivate.selectedTab = 'Rules';
            cswPrivate.makeRulesTab();

        }());

        return cswPublic;

        //#endregion _postCtor
    });
}());