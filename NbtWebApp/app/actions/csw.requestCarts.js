
(function () {

    Csw.actions.requestCarts = Csw.actions.requestCarts ||
        Csw.actions.register('requestCarts', function (cswParent, cswPrivate) {
            'use strict';

            //#region _preCtor

            var cswPublic = {};

            if (Csw.isNullOrEmpty(cswParent)) {
                Csw.error.throwException('Cannot create a Submit Request action without a valid Csw Parent object.', 'Csw.actions.requestCarts', 'csw.requestCarts.js', 14);
            }
            (function _preCtor() {
                cswPrivate.name = cswPrivate.name || 'CswSubmitRequest';
                cswPrivate.onSubmit = cswPrivate.onSubmit || function () { };
                cswPrivate.onCancel = cswPrivate.onCancel || function () { };
                cswPrivate.materialnodeid = cswPrivate.materialnodeid || '';
                cswPrivate.containernodeid = cswPrivate.containernodeid || '';

                cswParent.empty();
            }());

            cswPrivate.requestName = Csw.cookie.get(Csw.cookie.cookieNames.Username) + ' ' + Csw.todayAsString();
            cswPrivate.gridOpts = {};

            cswPrivate.currentTab = 'Pending';

            cswPrivate.restoreState = function () {
                var state = Csw.clientDb.getItem(cswPrivate.name + '_state');
                cswPrivate.state = state || {
                    pendingItemsViewId: '',
                    favoritesListViewId: '',
                    favoriteItemsViewId: '',
                    recurringItemsViewId: '',
                    submittedItemsViewId: '',
                    pendingCartId: '',
                    cartCounts: {
                        PendingRequestItems: 0,
                        SubmittedRequestItems: 0,
                        RecurringRequestItems: 0,
                        FavoriteRequestItems: 0
                    }
                };
            };
            cswPrivate.restoreState();

            cswPrivate.saveState = function () {
                Csw.clientDb.setItem(cswPrivate.name + '_state', cswPrivate.state);
            };

            cswPrivate.clearState = function () {
                Csw.clientDb.removeItem(cswPrivate.name + '_state');
            };

            //#endregion _preCtor

            //#region AJAX methods

            cswPrivate.getCart = function () {
                Csw.ajaxWcf.get({
                    urlMethod: 'Requests/cart',
                    success: function (data) {
                        cswPrivate.state.pendingItemsViewId = data.PendingItemsView.SessionViewId;
                        cswPrivate.state.favoritesListViewId = data.FavoritesView.SessionViewId;
                        cswPrivate.state.favoriteItemsViewId = data.FavoriteItemsView.SessionViewId;
                        cswPrivate.state.recurringItemsViewId = data.RecurringItemsView.SessionViewId;
                        cswPrivate.state.submittedItemsViewId = data.SubmittedItemsView.SessionViewId;
                        cswPrivate.state.pendingCartId = data.CurrentRequest.NodeId;

                        cswPrivate.saveState();
                        cswPrivate.onTabSelect(cswPrivate.currentTab);
                    }
                });
            };

            cswPrivate.getCartCounts = function () {
                Csw.ajaxWcf.get({
                    urlMethod: 'Requests/counts',
                    data: {
                        CartId: cswPrivate.state.pendingCartId
                    },
                    success: function (data) {
                        Csw.publish(Csw.enums.events.main.refreshHeader);
                        
                        cswPrivate.state.cartCounts = data.Counts;
                        cswPrivate.saveState();
                        cswPrivate.pendingTab.ext.setTitle('Pending (' + cswPrivate.state.cartCounts.PendingRequestItems + ')');
                        cswPrivate.submittedTab.ext.setTitle('Submitted (' + cswPrivate.state.cartCounts.SubmittedRequestItems + ')');
                        cswPrivate.recurringTab.ext.setTitle('Recurring (' + cswPrivate.state.cartCounts.RecurringRequestItems + ')');
                        cswPrivate.favoritesTab.ext.setTitle('Favorites (' + cswPrivate.state.cartCounts.FavoriteRequestItems + ')');
                        cswPrivate.tabs.resetWidth();
                    }
                });
            };

            cswPrivate.makeRequestCreateMaterial = function (grid) {
                Csw.ajaxWcf.get({
                    urlMethod: 'Requests/findMaterialCreate',
                    success: function (data) {
                        if (data.NodeTypeId) {
                            $.CswDialog('AddNodeDialog', {
                                text: 'New Create Material Request',
                                nodetypeid: data.NodeTypeId,
                                onAddNode: function () {
                                    grid.reload();
                                    cswPrivate.getCartCounts();
                                }
                            });
                        }
                    }
                });
            };

            cswPrivate.submitRequest = function () {
                Csw.ajaxWcf.post({
                    urlMethod: 'Requests/place',
                    data: {
                        NodeId: cswPrivate.state.pendingCartId,
                        NodeName: cswPrivate.requestName
                    },
                    success: function (json) {
                        if (json.Succeeded) {
                            cswPrivate.tabs.setActiveTab(1);                            
                            cswPrivate.onTabSelect('Submitted');
                            Csw.tryExec(cswPrivate.onSubmit);
                        }
                    }
                });
            }; // cswPrivate.submitRequest

            cswPrivate.addFavorite = function (callBack) {
                Csw.ajaxWcf.put({
                    urlMethod: 'Requests/Favorites/create',
                    success: function (json) {
                        if (json.Succeeded) {
                            $.CswDialog('EditNodeDialog', {
                                currentNodeId: json.RequestId,
                                filterToPropId: json.RequestName,
                                title: 'Add new Favorite',
                                onEditNode: function (nodeid, nodekey, close) {
                                    cswPrivate.lastCreatedFavorite = nodeid;
                                    Csw.ajaxWcf.put({
                                        urlMethod: 'Requests/Favorites/create',
                                        data: {
                                            RequestId: cswPrivate.lastCreatedFavorite
                                        },
                                        success: callBack
                                    });
                                }
                            });
                        }
                    }
                });
            };

            cswPrivate.copyToRequest = function (copyToRequest, copyRequestItems, onSuccess) {
                if (copyToRequest && copyRequestItems) {
                    Csw.ajaxWcf.post({
                        urlMethod: 'Requests/Favorites/copy',
                        data: {
                            RequestItems: copyRequestItems,
                            RequestId: copyToRequest
                        },
                        success: function () {
                            cswPrivate.getCartCounts();
                            onSuccess();
                        }
                    });
                }
            }; // copyRequest()  

            //#endregion AJAX methods

            //#region Tab construction

            cswPrivate.destroyOtherTabs = function (preserveTabName) {
                if (preserveTabName !== 'Favorites' && cswPublic.favoritesGrid) {
                    cswPublic.favoritesGrid.destroy();
                    cswPrivate.favoritesTab.csw.empty();
                }
                if (preserveTabName !== 'Recurring' && cswPublic.recurringGrid) {
                    cswPublic.recurringGrid.destroy();
                    cswPrivate.recurringTab.csw.empty();
                }
                if (preserveTabName !== 'Submitted' && cswPublic.submittedGrid) {
                    cswPublic.submittedGrid.destroy();
                    cswPrivate.submittedTab.csw.empty();
                }
                if (preserveTabName !== 'Pending' && cswPublic.pendingGrid) {
                    cswPublic.pendingGrid.destroy();
                    cswPrivate.pendingTab.csw.empty();
                }
            };

            cswPrivate.tabNames = ['Pending', 'Submitted', 'Recurring', 'Favorites'];

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
                var tgtTxt = null;
                if (el) {
                    tgtTxt = el.target.innerText;
                }
                var evtTxt = null;
                if (eventObj) {
                    evtTxt = eventObj.innerText;
                }
                var tab = cswPrivate.tryParseTabName(tabName, tgtTxt, evtTxt);
                if (tab.length > 0) {
                    cswPrivate.currentTab = tab;
                    cswPrivate.destroyOtherTabs(cswPrivate.currentTab);
                    switch (cswPrivate.currentTab) {
                        case 'Pending':
                            cswPrivate.makePendingTab();
                            break;
                        case 'Submitted':
                            cswPrivate.makeSubmittedTab();
                            break;
                        case 'Recurring':
                            cswPrivate.makeRecurringTab();
                            break;
                        case 'Favorites':
                            cswPrivate.makeFavoritesTab();
                            break;
                    }
                    cswPrivate.getCartCounts();
                }
            };

            cswPrivate.makeGridForTab = function (opts) {
                opts = opts || {
                    onSuccess: function () { },
                    onSelectChange: function () { }
                };

                return opts.parent.grid({
                    name: opts.gridId,
                    storeId: opts.gridId + '_store',
                    stateId: opts.gridId,
                    title: opts.title,
                    usePaging: true,
                    height: 350,
                    width: cswPrivate.tabs.getWidth() - 20,
                    ajax: {
                        urlMethod: 'getRequestItemGrid',
                        data: {
                            SessionViewId: opts.sessionViewId
                        }
                    },
                    showCheckboxes: opts.showCheckboxes,
                    showActionColumn: true,
                    canSelectRow: false,
                    groupField: opts.groupField || '',
                    onLoad: function (grid, json) {
                        Csw.tryExec(opts.onSuccess);
                    },
                    onEdit: function (rows) {
                        // this works for both Multi-edit and regular
                        var nodekeys = Csw.delimitedString(),
                            nodeids = Csw.delimitedString(),
                            nodenames = [],
                            currentNodeId, currentNodeKey;


                        Csw.each(rows, function (row) {
                            currentNodeId = currentNodeId || row.nodeid;
                            currentNodeKey = currentNodeKey || row.nodekey;
                            nodekeys.add(row.nodekey);
                            nodeids.add(row.nodeid);
                            nodenames.push(row.nodename);
                        });

                        $.CswDialog('EditNodeDialog', {
                            currentNodeId: currentNodeId,
                            currentNodeKey: currentNodeKey,
                            selectedNodeIds: nodeids,
                            selectedNodeKeys: nodekeys,
                            nodenames: nodenames,
                            Multi: (nodeids.length > 1),
                            title: 'Request',
                            onEditNode: function () {
                                Csw.tryExec(opts.onEditNode); //Case 27619--don't pass the function by reference, because we want to control the parameters with which it is called
                                cswPrivate.getCartCounts();
                            }
                        });
                    }, // onEdit
                    onDelete: function (rows) {
                        // this works for both Multi-edit and regular
                        var cswnbtnodekeys = [],
                            nodeids = [],
                            nodenames = [];

                        Csw.each(rows, function (row) {
                            cswnbtnodekeys.push(row.nodekey);
                            nodeids.push(row.nodeid);
                            nodenames.push(row.nodename);
                        });

                        $.CswDialog('DeleteNodeDialog', {
                            nodeids: nodeids,
                            nodepks: nodeids,
                            nodekeys: cswnbtnodekeys,
                            nodenames: nodenames,
                            onDeleteNode: Csw.method(function () {
                                Csw.tryExec(opts.onEditNode);
                                cswPrivate.getCartCounts();
                            }),
                            Multi: (nodeids.length > 1),
                            publishDeleteEvent: false
                        });
                    }, // onDelete
                    onSelectChange: function (rowCount) {
                        Csw.tryExec(opts.onSelectChange, rowCount);
                    }
                });
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

            cswPrivate.makePendingTab = function (opts) {
                var ol = cswPrivate.prepTab(cswPrivate.pendingTab, 'Request Items Pending Submission', 'Edit any of the Request Items in your cart. When you are finished, click "Place Request" to submit your cart.');

                var inpTbl = ol.li().table({
                    width: '99%',
                    cellpadding: '5px'
                });

                inpTbl.cell(1, 1).span().setLabelText('Request Name: ');
                inpTbl.cell(1, 1).input({
                    value: cswPrivate.requestName,
                    onChange: function (val) {
                        cswPrivate.requestName = val;
                    }
                });

                inpTbl.cell(1, 2)
                    .css({ 'text-align': 'right' })
                    .buttonExt({
                        enabledText: 'Request Create Material',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.cart),
                        onClick: function () {
                            if (cswPublic.pendingGrid) {
                                cswPrivate.makeRequestCreateMaterial(cswPublic.pendingGrid);
                            }
                        }
                    });

                ol.li().br({ number: 1 });

                var hasOneRowSelected = false;
                var favoriteSelect;

                var toggleSaveBtn = function () {
                    if (favoriteSelect) {
                        var nodeId = favoriteSelect.selectedNodeId();
                        if (nodeId) {
                            cswPrivate.selectedFavoriteId = nodeId;
                        }
                    }
                    if (hasOneRowSelected && cswPrivate.selectedFavoriteId) {
                        saveBtn.enable();
                    } else {
                        saveBtn.disable();
                    }
                };

                opts = opts || {
                    onSuccess: function () { }
                };
                opts.sessionViewId = cswPrivate.state.pendingItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_pendingGrid';
                opts.showCheckboxes = true;
                opts.title = 'Select any Pending items to save to a Favorite list';
                opts.onSelectChange = function (rowCount) {
                    hasOneRowSelected = rowCount > 0;
                    toggleSaveBtn();
                };
                opts.onEditNode = cswPrivate.makePendingTab;

                cswPublic.pendingGrid = cswPrivate.makeGridForTab(opts);
                
                var btmTbl = ol.li().table({cellpadding: '2px'});
                var saveBtn = btmTbl.cell(1, 1).buttonExt({
                    enabledText: 'Save to Favorites',
                    disabledText: 'Save to Favorites',
                    disableOnClick: true,
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                    onClick: function () {
                        var nodes = [];
                        cswPublic.pendingGrid.getSelectedRowsVals('nodeid').forEach(function (nodeId) {
                            nodes.push({ NodeId: nodeId });
                        });
                        cswPrivate.copyToRequest(cswPrivate.selectedFavoriteId, nodes, function () {
                            cswPublic.pendingGrid.deselectAll();
                            toggleSaveBtn();
                        });
                        saveBtn.disable();
                    }
                }).disable();

                var picklistCell = btmTbl.cell(1, 2);

                var makePicklist = function () {
                    picklistCell.empty();
                    favoriteSelect = picklistCell.nodeSelect({
                        selectedNodeId: cswPrivate.lastCreatedFavorite || '',
                        showSelectOnLoad: true,
                        viewid: cswPrivate.state.favoritesListViewId,
                        allowAdd: false,
                        onSelectNode: toggleSaveBtn,
                        onSuccess: toggleSaveBtn
                    });
                };
                
                makePicklist();
                btmTbl.cell(1, 3).buttonExt({
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                    onClick: function () {
                        Csw.tryExec(cswPrivate.addFavorite, makePicklist);
                    }
                });

                ol.li().br({ number: 2 });

                cswPrivate.addBtnGroup(ol.li(), true);

            }; // cswPrivate.makePendingTab()

            cswPrivate.makeSubmittedTab = function () {
                var ol = cswPrivate.prepTab(cswPrivate.submittedTab, 'Submitted Request Items', 'View any previously submitted Request Items.');

                Csw.nbt.viewFilters({
                    name: 'submitted_viewfilters',
                    isTreeView: false,
                    parent: ol.li(),
                    viewid: cswPrivate.state.submittedItemsViewId,
                    onEditFilters: function (newviewid) {
                        cswPrivate.state.submittedItemsViewId = newviewid;
                        cswPrivate.makeSubmittedTab();
                    } // onEditFilters
                }); // viewFilters

                ol.br();

                var opts = {
                    onSuccess: function () { }
                };
                opts.showCheckboxes = false;
                opts.sessionViewId = cswPrivate.state.submittedItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_submittedItemsGrid';

                opts.onEditNode = cswPrivate.makeSubmittedTab;

                cswPublic.submittedGrid = cswPrivate.makeGridForTab(opts);

                ol.li().br();

                cswPrivate.addBtnGroup(ol.li(), false);
            };

            cswPrivate.makeRecurringTab = function () {
                var ol = cswPrivate.prepTab(cswPrivate.recurringTab, 'Recurring Request Items', 'View any recurring Request Items.');
                ol.br();

                var opts = {
                    onSuccess: function () { }
                };
                opts.showCheckboxes = false;
                opts.sessionViewId = cswPrivate.state.recurringItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_recurringItemsGrid';

                opts.onEditNode = cswPrivate.makeRecurringTab;

                cswPublic.recurringGrid = cswPrivate.makeGridForTab(opts);

                ol.li().br();

                cswPrivate.addBtnGroup(ol.li(), false);

            };

            cswPrivate.makeFavoritesTab = function () {
                var ol = cswPrivate.prepTab(cswPrivate.favoritesTab, 'Favorite Request Itens', 'View any favorite Request Items.');
                Csw.nbt.viewFilters({
                    name: 'favorites_viewfilters',
                    isTreeView: false,
                    parent: ol.li(),
                    viewid: cswPrivate.state.favoriteItemsViewId,
                    onEditFilters: function (newviewid) {
                        cswPrivate.state.favoriteItemsViewId = newviewid;
                        cswPrivate.makeFavoritesTab();
                    } // onEditFilters
                }); // viewFilters

                ol.br();

                var hasOneRowSelected = false;

                var toggleCopyBtn = function () {
                    if (hasOneRowSelected) {
                        copyBtn.enable();
                    } else {
                        copyBtn.disable();
                    }
                };

                var opts = {
                    onSuccess: function () { }
                };
                opts.showCheckboxes = true;
                opts.sessionViewId = cswPrivate.state.favoriteItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_favoriteItemsGrid';

                opts.onEditNode = cswPrivate.makeFavoritesTab;
                opts.onSelectChange = function (rowCount) {
                    hasOneRowSelected = rowCount > 0;
                    toggleCopyBtn();
                };
                cswPublic.favoritesGrid = cswPrivate.makeGridForTab(opts);

                ol.li().br();

                var copyBtn = ol.li().buttonExt({
                    enabledText: 'Copy to Current Cart',
                    disabledText: 'Copy to Current Cart',
                    disableOnClick: true,
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.copy),
                    onClick: function () {
                        var nodes = [];
                        cswPublic.favoritesGrid.getSelectedRowsVals('nodeid').forEach(function (nodeId) {
                            nodes.push({ NodeId: nodeId });
                        });
                        cswPrivate.copyToRequest(cswPrivate.state.pendingCartId, nodes, function () {
                            cswPublic.favoritesGrid.deselectAll();
                            toggleCopyBtn();
                        });
                        copyBtn.disable();
                    }
                }).disable();

                ol.li().br();

                cswPrivate.addBtnGroup(ol.li(), false);

            };

            cswPrivate.addBtnGroup = function (el, hasFinish) {
                var tbl = el.table({ width: '99%', cellpadding: '5px' });

                if (hasFinish) {
                    tbl.cell(1, 1).buttonExt({
                        enabledText: 'Place Request',
                        disabledText: 'Place Request',
                        disableOnClick: true,
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.check),
                        onClick: function () {
                            cswPrivate.submitRequest();
                            cswPrivate.clearState();
                        }
                    });
                }
                tbl.cell(1, 2).css({ 'text-align': 'right' }).buttonExt({
                    enabledText: 'Close',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.cancel),
                    onClick: function () {
                        Csw.tryExec(cswPrivate.onCancel);
                        cswPrivate.clearState();
                    }
                });
            };

            //#endregion Tab construction

            //#region _postCtor            

            (function _postCtor() {

                cswParent.empty();
                cswPrivate.tabs = cswParent.tabStrip({
                    onTabSelect: cswPrivate.onTabSelect
                });
                cswPrivate.tabs.setTitle('Requests');

                cswPrivate.pendingTab = cswPrivate.tabs.addTab({
                    title: 'Pending (' + cswPrivate.state.cartCounts.PendingRequestItems + ')'
                });

                cswPrivate.submittedTab = cswPrivate.tabs.addTab({
                    title: 'Submitted (' + cswPrivate.state.cartCounts.SubmittedRequestItems + ')'
                });

                cswPrivate.recurringTab = cswPrivate.tabs.addTab({
                    title: 'Recurring (' + cswPrivate.state.cartCounts.RecurringRequestItems + ')'
                });

                cswPrivate.favoritesTab = cswPrivate.tabs.addTab({
                    title: 'Favorites (' + cswPrivate.state.cartCounts.FavoriteRequestItems + ')'
                });

                cswPrivate.tabs.setActiveTab(0);

                cswPrivate.getCart();

            }());

            return cswPublic;

            //#endregion _postCtor
        });
}());