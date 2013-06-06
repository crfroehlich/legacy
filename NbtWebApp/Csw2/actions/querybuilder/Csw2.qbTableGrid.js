/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function () {

    var onInit = function (thisGrid) {
        thisGrid.columns = [Csw2.grids.columns.rendererColumn({
            width: 16,
            dataIndex: 'key',
            onRender: function (val, meta, model) {
                if (val == 'PRI') {
                    meta.style = 'background-image:url(../images/sqlbuilder/key.gif) !important;background-position:2px 3px;background-repeat:no-repeat;';
                }
                return '&nbsp;';
            }
        }),
            Csw2.grids.columns.rendererColumn({
                flex: 1,
                dataIndex: 'field',
                onRender: function (val, meta, model) {
                    if (model.get('key') == 'PRI') {
                        return '<span style="font-weight: bold;">' + val + '</span>&nbsp;&nbsp;<span style="color:#aaa;">' + model.get('type') + '</span>';
                    }
                    return val + '&nbsp;&nbsp;<span style="color:#999;">' + model.get('type') + '</span>';

                }
            })];

        var SelModel = Csw2.stores.selectionModel({
            checkOnly: true,
            onSelect: function (selModel, data) {
                // add new rows to the SqlFineTuningGrid after a selection change
                Csw2.actions.sql.manager.select.fields.addFieldRecord(data, true);
            },
            onDeselect: function (selModel, data) {
                // remove row from SqlFineTuningGrid after deselection
                Csw2.actions.sql.manager.select.fields.removeFieldById(data.get('id'));
            }
        });

        thisGrid.selModel = SelModel.ExtSelModel;
        return thisGrid;
    };

    /**
     * Define the grid
    */
    var grid = Csw2.grids.grid({
        name: 'Ext.Csw2.qbTableGrid',
        alias: ['widget.qbTableGrid'],
        onInit: onInit
    });

    grid.addProp(Csw2.grids.constants.properties.hideHeaders, true);
    grid.addProp(Csw2.grids.constants.properties.border, false);


    /**
     * Add the listeners
    */
    grid.listeners.add(Csw2.grids.constants.listeners.bodyscroll, function () {
        var scrollOffset, sqlTable;
        // the bodyscroll event of the view was fired
        // get scroll information
        scrollOffset = this.el.getScroll();
        // get the parent sqltable
        sqlTable = this.up('sqltable');
        // change shadowSprites scrollTop property
        sqlTable.shadowSprite.scrollTop = scrollOffset.top;
        // redraw all connections to reflect scroll action
        for (var i = Csw2.actions.sql.manager.connections.length; i--;) {
            sqlTable.connection(Csw2.actions.sql.manager.connections[i]);
        }
    });

    grid.listeners.add(Csw2.grids.constants.listeners.render, function (view) {
        this.dd = {};
        // init the view as a DragZone
        this.dd.dragZone = new Ext.view.DragZone({
            view: view,
            ddGroup: 'SQLTableGridDDGroup',
            dragText: '{0} selected table column{1}',
            onInitDrag: function (x, y) {
                var me = this,
                    data = me.dragData,
                    view = data.view,
                    selectionModel = view.getSelectionModel(),
                    record = view.getRecord(data.item),
                    e = data.event;
                data.records = [record];
                me.ddel.update(me.getDragText());
                me.proxy.update(me.ddel.dom);
                me.onStartDrag(x, y);
                return true;
            }
        });
        // init the view as a DropZone
        this.dd.dropZone = new Ext.grid.ViewDropZone({
            view: view,
            ddGroup: 'SQLTableGridDDGroup',
            handleNodeDrop: function (data, record, position) {
                // Was soll nach dem Drop passieren?
            },
            onNodeOver: function (node, dragZone, e, data) {
                var me = this,
                    view = me.view,
                    pos = me.getPosition(e, node),
                    overRecord = view.getRecord(node),
                    draggingRecords = data.records;

                if (!Ext.Array.contains(data.records, me.view.getRecord(node))) {
                    if (!Ext.Array.contains(draggingRecords, overRecord) && data.records[0].get('field') != '*') {
                        me.valid = true;
                        // valid drop target
                        // todo show drop invitation
                    } else {
                        // invalid drop target
                        me.valid = false;
                    }
                }
                return me.valid ? me.dropAllowed : me.dropNotAllowed;
            },
            onContainerOver: function (dd, e, data) {
                var me = this;
                // invalid drop target
                me.valid = false;
                return me.dropNotAllowed;
            }
        });
    });

    grid.listeners.add(Csw2.grids.constants.listeners.drop, function (node, data, dropRec, dropPosition) {
        var sqlTable1, sqlTable2, showJoinCM, connection, aBBPos, join, joinCondition = '',
            dropTable, targetTable;

        showJoinCM = function (event, el) {
            var cm;
            // stop the browsers event bubbling
            event.stopEvent();
            // create context menu
            cm = Ext.create('Ext.menu.Menu', {
                items: [{
                    text: 'Edit Join',
                    icon: 'resources/images/document_edit16x16.gif',
                    handler: Ext.Function.bind(function () {
                        throw new Error('Edit Join has not been implemented.');
                    }, this)
                }, {
                    text: 'Remove Join',
                    icon: 'resources/images/remove.gif',
                    handler: Ext.Function.bind(function () {
                        // remove any connection lines from surface and from array Csw2.actions.sql.manager.connections
                        Csw2.actions.sql.manager.connections = Ext.Array.filter(Csw2.actions.sql.manager.connections, function (connection) {
                            var bRemove = true;
                            if (this.uuid == connection.uuid) {
                                this.line.remove();
                                this.bgLine.remove();
                                this.miniLine1.remove();
                                this.miniLine2.remove();
                                bRemove = false;
                            }
                            return bRemove;
                        }, this);
                        Csw2.actions.sql.manager.select.joins.removeJoinById(this.uuid);
                    }, this)
                }, {
                    text: 'Close Menu',
                    icon: 'resources/images/cross.gif',
                    handler: Ext.emptyFn
                }]
            });
            // show the contextmenu next to current mouse position
            cm.showAt(event.getXY());
        };

        if (node.boundView) {
            sqlTable1 = data.view.up('window');
            sqlTable1.shadowSprite.bConnections = true;

            sqlTable2 = Ext.getCmp(node.boundView).up('window');
            sqlTable2.shadowSprite.bConnections = true;

            dropTable = Csw2.actions.sql.manager.select.tables.getTableById(sqlTable1.tableId);
            targetTable = Csw2.actions.sql.manager.select.tables.getTableById(sqlTable2.tableId);

            aBBPos = [data.item.viewIndex, node.viewIndex];

            connection = sqlTable2.connection(sqlTable2, sqlTable1.shadowSprite, sqlTable2.shadowSprite, "#000", aBBPos);

            sqlTable1.connectionUUIDs.push(connection.uuid);
            sqlTable2.connectionUUIDs.push(connection.uuid);

            Csw2.actions.sql.manager.connections.push(connection);

            // bgLine is white(invisble) and its stroke-width is 10
            // so it is easier to capture the dblclick event
            connection.bgLine.el.on('contextmenu', showJoinCM, connection);

            // line is black and its stroke-width is 1
            connection.line.el.on('contextmenu', showJoinCM, connection);

            // create an instance of the join model
            //join = Ext.create('Ext.Csw2.SqlDragDropTableJoinModel');
            join = new Csw2.actions.querybuilder.SqlDragDropTableJoinModel();
            // set join id
            join.set('id', connection.uuid);
            // sqlTable1 is the left table
            join.set('leftTableId', sqlTable1.tableId);
            // data.records[0] represents the model of the dragged node
            join.set('leftTableField', data.records[0].get('field'));
            // sqlTable1 is the left table
            join.set('rightTableId', sqlTable2.tableId);
            // node.viewIndex is the index of the target node
            join.set('rightTableField', sqlTable2.down('grid').store.getAt(node.viewIndex).get('field'));
            // set the defaul join type to INNER
            join.set('joinType', 'INNER');

            if (dropTable.get('tableAlias') != '') {
                joinCondition = joinCondition + dropTable.get('tableAlias') + '.' + join.get('leftTableField') + '=';
            } else {
                joinCondition = joinCondition + dropTable.get('tableName') + '.' + join.get('leftTableField') + '=';
            }

            if (targetTable.get('tableAlias') != '') {
                joinCondition = joinCondition + targetTable.get('tableAlias') + '.' + join.get('rightTableField');
            } else {
                joinCondition = joinCondition + targetTable.get('tableName') + '.' + join.get('rightTableField');
            }

            join.set('joinCondition', joinCondition);
            Csw2.actions.sql.manager.select.joins.addJoin(join);
        }

    });

    grid.init();

}());




//(function () {

//    Ext.define('Ext.csw2.qbTableGrid', {
//        extend: 'Ext.grid.Panel',
//        alias: ['widget.qbTableGrid'],
//        border: false,
//        hideHeaders: true,
//        viewConfig: {
//            listeners: {
//                bodyscroll: function () {
//                    var scrollOffset, sqlTable;
//                    // the bodyscroll event of the view was fired
//                    // get scroll information
//                    scrollOffset = this.el.getScroll();
//                    // get the parent sqltable
//                    sqlTable = this.up('sqltable');
//                    // change shadowSprites scrollTop property
//                    sqlTable.shadowSprite.scrollTop = scrollOffset.top;
//                    // redraw all connections to reflect scroll action
//                    for (var i = Csw2.actions.sql.manager.connections.length; i--;) {
//                        sqlTable.connection(Csw2.actions.sql.manager.connections[i]);
//                    }
//                },
//                render: function (view) {
//                    this.dd = {};
//                    // init the view as a DragZone
//                    this.dd.dragZone = new Ext.view.DragZone({
//                        view: view,
//                        ddGroup: 'SQLTableGridDDGroup',
//                        dragText: '{0} selected table column{1}',
//                        onInitDrag: function (x, y) {
//                            var me = this,
//                                data = me.dragData,
//                                view = data.view,
//                                selectionModel = view.getSelectionModel(),
//                                record = view.getRecord(data.item),
//                                e = data.event;
//                            data.records = [record];
//                            me.ddel.update(me.getDragText());
//                            me.proxy.update(me.ddel.dom);
//                            me.onStartDrag(x, y);
//                            return true;
//                        }
//                    });
//                    // init the view as a DropZone
//                    this.dd.dropZone = new Ext.grid.ViewDropZone({
//                        view: view,
//                        ddGroup: 'SQLTableGridDDGroup',
//                        handleNodeDrop: function (data, record, position) {
//                            // Was soll nach dem Drop passieren?
//                        },
//                        onNodeOver: function (node, dragZone, e, data) {
//                            var me = this,
//                                view = me.view,
//                                pos = me.getPosition(e, node),
//                                overRecord = view.getRecord(node),
//                                draggingRecords = data.records;

//                            if (!Ext.Array.contains(data.records, me.view.getRecord(node))) {
//                                if (!Ext.Array.contains(draggingRecords, overRecord) && data.records[0].get('field') != '*') {
//                                    me.valid = true;
//                                    // valid drop target
//                                    // todo show drop invitation
//                                }
//                                else {
//                                    // invalid drop target
//                                    me.valid = false;
//                                }
//                            }
//                            return me.valid ? me.dropAllowed : me.dropNotAllowed;
//                        },
//                        onContainerOver: function (dd, e, data) {
//                            var me = this;
//                            // invalid drop target
//                            me.valid = false;
//                            return me.dropNotAllowed;
//                        }
//                    });
//                },
//                drop: function (node, data, dropRec, dropPosition) {
//                    var sqlTable1, sqlTable2, showJoinCM, connection, aBBPos, join, joinCondition = '',
//                        dropTable, targetTable;

//                    showJoinCM = function (event, el) {
//                        var cm;
//                        // stop the browsers event bubbling
//                        event.stopEvent();
//                        // create context menu
//                        cm = Ext.create('Ext.menu.Menu', {
//                            items: [{
//                                text: 'Edit Join',
//                                icon: 'resources/images/document_edit16x16.gif',
//                                handler: Ext.Function.bind(function () {
//                                    throw new Error('Edit Join has not been implemented.');
//                                }, this)
//                            }, {
//                                text: 'Remove Join',
//                                icon: 'resources/images/remove.gif',
//                                handler: Ext.Function.bind(function () {
//                                    // remove any connection lines from surface and from array Csw2.actions.sql.manager.connections
//                                    Csw2.actions.sql.manager.connections = Ext.Array.filter(Csw2.actions.sql.manager.connections, function (connection) {
//                                        var bRemove = true;
//                                        if (this.uuid == connection.uuid) {
//                                            this.line.remove();
//                                            this.bgLine.remove();
//                                            this.miniLine1.remove();
//                                            this.miniLine2.remove();
//                                            bRemove = false;
//                                        }
//                                        return bRemove;
//                                    }, this);
//                                    Csw2.actions.sql.manager.sqlSelect.removeJoinById(this.uuid);
//                                }, this)
//                            }, {
//                                text: 'Close Menu',
//                                icon: 'resources/images/cross.gif',
//                                handler: Ext.emptyFn
//                            }]
//                        });
//                        // show the contextmenu next to current mouse position
//                        cm.showAt(event.getXY());
//                    };

//                    if (node.boundView) {
//                        sqlTable1 = data.view.up('window');
//                        sqlTable1.shadowSprite.bConnections = true;

//                        sqlTable2 = Ext.getCmp(node.boundView).up('window');
//                        sqlTable2.shadowSprite.bConnections = true;

//                        dropTable = Csw2.actions.sql.manager.sqlSelect.getTableById(sqlTable1.tableId);
//                        targetTable = Csw2.actions.sql.manager.sqlSelect.getTableById(sqlTable2.tableId);

//                        aBBPos = [data.item.viewIndex, node.viewIndex];

//                        connection = sqlTable2.connection(sqlTable1.shadowSprite, sqlTable2.shadowSprite, "#000", aBBPos);

//                        sqlTable1.connectionUUIDs.push(connection.uuid);
//                        sqlTable2.connectionUUIDs.push(connection.uuid);

//                        Csw2.actions.sql.manager.connections.push(connection);

//                        // bgLine is white(invisble) and its stroke-width is 10
//                        // so it is easier to capture the dblclick event
//                        connection.bgLine.el.on('contextmenu', showJoinCM, connection);

//                        // line is black and its stroke-width is 1
//                        connection.line.el.on('contextmenu', showJoinCM, connection);

//                        // create an instance of the join model
//                        join = Ext.create('Ext.Csw2.SqlDragDropTableJoinModel');
//                        // set join id
//                        join.set('id', connection.uuid);
//                        // sqlTable1 is the left table
//                        join.set('leftTableId', sqlTable1.tableId);
//                        // data.records[0] represents the model of the dragged node
//                        join.set('leftTableField', data.records[0].get('field'));
//                        // sqlTable1 is the left table
//                        join.set('rightTableId', sqlTable2.tableId);
//                        // node.viewIndex is the index of the target node
//                        join.set('rightTableField', sqlTable2.down('grid').store.getAt(node.viewIndex).get('field'));
//                        // set the defaul join type to INNER
//                        join.set('joinType', 'INNER');

//                        if (dropTable.get('tableAlias') != '') {
//                            joinCondition = joinCondition + dropTable.get('tableAlias') + '.' + join.get('leftTableField') + '=';
//                        }
//                        else {
//                            joinCondition = joinCondition + dropTable.get('tableName') + '.' + join.get('leftTableField') + '=';
//                        }

//                        if (targetTable.get('tableAlias') != '') {
//                            joinCondition = joinCondition + targetTable.get('tableAlias') + '.' + join.get('rightTableField');
//                        }
//                        else {
//                            joinCondition = joinCondition + targetTable.get('tableName') + '.' + join.get('rightTableField');
//                        }

//                        join.set('joinCondition', joinCondition);
//                        Csw2.actions.sql.manager.sqlSelect.addJoin(join);
//                    }

//                }
//            }
//        },
//        initComponent: function () {

//            this.columns = [{
//                xtype: 'gridcolumn',
//                width: 16,
//                dataIndex: 'key',
//                renderer: function (val, meta, model) {
//                    if (val == 'PRI') {
//                        meta.style = 'background-image:url(../images/sqlbuilder/key.gif) !important;background-position:2px 3px;background-repeat:no-repeat;';
//                    }
//                    return '&nbsp;';
//                }
//            }, {
//                xtype: 'gridcolumn',
//                flex: 1,
//                dataIndex: 'field',
//                renderer: function (val, meta, model) {
//                    if (model.get('key') == 'PRI') {
//                        return '<span style="font-weight: bold;">' + val + '</span>&nbsp;&nbsp;<span style="color:#aaa;">' + model.get('type') + '</span>';
//                    }
//                    return val + '&nbsp;&nbsp;<span style="color:#999;">' + model.get('type') + '</span>';

//                }
//            }];

//            this.selModel = Ext.create('Ext.selection.CheckboxModel', {
//                mode: 'SIMPLE',
//                checkOnly: true,
//                listeners: {
//                    select: function (selModel, data) {
//                        // add new rows to the SqlFineTuningGrid after a selection change
//                        Csw2.actions.sql.manager.sqlSelect.addFieldRecord(data, true);
//                    },
//                    deselect: function (selModel, data) {
//                        var store, model;
//                        // remove row from SqlFineTuningGrid after deselection
//                        Csw2.actions.sql.manager.sqlSelect.removeFieldById(data.get('id'));
//                    }
//                }
//            });

//            this.callParent(arguments);
//        }
//    });

//}());