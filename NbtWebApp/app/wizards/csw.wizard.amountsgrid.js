
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    Csw.wizard.register('amountsGrid', function (cswParent, options) {
        'use strict';
        ///<summary>Creates an amounts thin grid with an Add form.</summary>
        var cswPublic = {
            rows: {},
            quantities: function () {
                var qtyVals = [];
                Csw.each(cswPublic.rows, function (row) {
                    qtyVals.push(row.quantityValues);
                });
                return qtyVals;
            },
            thinGrid: null,
            containerlimit: 25,
            containerCount: ''
        };

        Csw.tryExec(function () {

            var cswPrivate = {
                name: 'wizardAmountsThinGrid',
                onAdd: null,
                onDelete: null,
                onChange: null,
                quantity: {},
                containerMinimum: 1,
                action: 'Receive',
                selectedSizeId: null,
                relatedNodeId: null,
                materialId: null,
                rows: [],
                config: {
                    numberName: 'No. Containers',
                    sizeName: 'Size',
                    quantityName: 'Net Quantity',
                    balanceName: 'Read Balance',
                    barcodeName: 'Barcodes (Optional)'
                },
                customBarcodes: false,
                nodeTypeName: '',
                sizeSelectOpts: [],
                sizeViewId: '',
                balancesDefined: false,
            };
            Csw.extend(cswPrivate, options);

            cswPrivate.header = [{ "value": cswPrivate.config.numberName, "isRequired": true }];
            if (false === Csw.isNullOrEmpty(cswPrivate.materialId) && cswPrivate.action === 'Receive') {
                cswPrivate.header = cswPrivate.header.concat([{ "value": cswPrivate.config.sizeName, "isRequired": true }]);
            }
            cswPrivate.header = cswPrivate.header.concat([{ "value": cswPrivate.config.quantityName, "isRequired": true }]);
            if (cswPrivate.customBarcodes) {
                cswPrivate.header = cswPrivate.header.concat([{ "value": cswPrivate.config.barcodeName, "isRequired": false }]);
            }
            if (cswPrivate.balancesDefined) {
                cswPrivate.header = cswPrivate.header.concat([{ "value": cswPrivate.config.balanceName, "isRequired": false }]);
            }
            if (cswPrivate.rows.length === 0) {
                cswPrivate.rows.push(cswPrivate.header);
            } else {
                var firstRow = cswPrivate.rows.splice(0, 1, cswPrivate.header);
                cswPrivate.rows.push(firstRow);
            }

            cswPrivate.getQuantity = function (onSuccess) {
                if (false === Csw.isNullOrEmpty(cswPrivate.selectedSizeId)) {
                    Csw.ajax.deprecatedWsNbt({
                        urlMethod: 'getQuantity',
                        data: { SizeId: cswPrivate.selectedSizeId, Action: cswPrivate.action },
                        success: function (data) {
                            cswPrivate.quantity = data;
                            Csw.tryExec(onSuccess);
                        }
                    });
                }
            };

            (function _pre() {
                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Wizard amounts grid without a parent.', '', 'csw.wizard.amountsgrid.js', 22));
                }
                cswPublic.containerlimit = cswPrivate.containerlimit || cswPublic.containerlimit;
                cswPublic.containerCount = 0;

                var containerNoSpan = cswParent.span();
                containerNoSpan.setLabelText('Total number of containers: ' + cswPublic.containerCount);

                var containerLimitExceededSpan = cswParent.span({ cssclass: 'CswErrorMessage_ValidatorError', text: ' The limit for containers created at receipt is [' + cswPublic.containerlimit + '].' });
                containerLimitExceededSpan.hide();

                cswParent.br({ number: 2 });

                var getTotalContainerQuantity = function () {
                    var totalContainerQuantity = 0;
                    Csw.each(cswPublic.rows, function (row) {
                        if (false === Csw.isNullOrEmpty(row.quantityValues)) {
                            totalContainerQuantity += Csw.number(row.quantityValues.containerNo, 0);
                        }
                    });
                    return totalContainerQuantity;
                };

                var updateTotalContainerCount = function () {
                    cswPublic.containerCount = getTotalContainerQuantity();
                    containerNoSpan.empty().setLabelText('Total number of containers: ' + cswPublic.containerCount);
                    if (cswPublic.containerCount > cswPublic.containerlimit) {
                        containerLimitExceededSpan.show();
                    } else {
                        containerLimitExceededSpan.hide();
                    }
                };

                var makeGrid = function () {
                    cswPublic.thinGrid = cswParent.thinGrid({
                        linkText: '',
                        hasHeader: true,
                        rows: cswPrivate.rows,
                        allowDelete: true,
                        allowAdd: true,
                        makeAddRow: function (cswCell, columnName, rowid) {
                            'use strict';
                            var updateSizeVals = function () {
                                cswPrivate.selectedSizeId = cswPublic.rows[rowid].sizeControl.selectedVal();
                                cswPublic.rows[rowid].quantityValues.sizeid = cswPublic.rows[rowid].sizeControl.selectedVal();
                                cswPublic.rows[rowid].quantityValues.sizename = cswPublic.rows[rowid].sizeControl.selectedText();
                            };
                            var updateColumnVals = function (changeContainerNo) {
                                if (false === Csw.isNullOrEmpty(cswPublic.rows[rowid].qtyControl)) {
                                    cswPublic.rows[rowid].quantityValues.quantity = cswPublic.rows[rowid].qtyControl.value();
                                    cswPublic.rows[rowid].quantityValues.unitid = cswPublic.rows[rowid].qtyControl.selectedUnit();
                                }
                                if (changeContainerNo) {
                                    cswPublic.rows[rowid].containerNoControl.val(cswPrivate.quantity.unitCount);
                                    cswPublic.rows[rowid].quantityValues.containerNo = cswPrivate.quantity.unitCount;
                                    updateTotalContainerCount();
                                }
                                Csw.tryExec(cswPrivate.onChange, cswPublic.quantities());
                            };


                            var buildQtyCtrl = function (cellOverride) {
                                cswPrivate.quantity.minvalue = 0;
                                cswPrivate.quantity.excludeRangeLimits = true;
                                cswPrivate.quantity.onNumberChange = function () {
                                    updateColumnVals(false);
                                };
                                cswPrivate.quantity.onQuantityChange = function () {
                                    updateColumnVals(false);
                                };
                                cswPrivate.quantity.quantity = cswPrivate.quantity.value;
                                cswPrivate.quantity.selectedNodeId = cswPrivate.quantity.nodeid;
                                cswPrivate.quantity.name = 'containerQuantity';
                                cswPrivate.quantity.qtyWidth = (7 * 8) + 'px'; //7 characters wide, 8 is the characters-to-pixels ratio
                                cswPrivate.quantity.isReadOnly = cswPrivate.quantity.qtyReadonly;
                                cswPrivate.quantity.quantityoptional = false;

                                if (null != Csw.currentUser.defaults().DefaultBalanceId) {
                                    Csw.ajaxWcf.post({
                                        urlMethod: 'Balances/getBalanceInformation',
                                        data: Csw.currentUser.defaults().DefaultBalanceId,
                                        success: function (data) {
                                            var Balance = data.BalanceList[0];
                                            if (Balance.IsActive) {
                                                cswPublic.rows[rowid].balanceControl.setText(Balance.NbtName);
                                                cswPublic.rows[rowid].balanceControl.setHandler(function () { getBalanceInformation(Balance.NodeId); });
                                                updateBalanceInterface(Balance);
                                            } else {
                                                cswPublic.rows[rowid].balanceControl.setText(Balance.NbtName + " (Inactive)");
                                            }
                                        }//success
                                    });
                                }//if there is a default balance


                                if (cellOverride) {
                                    cswPublic.rows[rowid].qtyControl = cellOverride.quantity(cswPrivate.quantity);
                                } else {
                                    cswPublic.rows[rowid].qtyControl = cswCell.quantity(cswPrivate.quantity);
                                }
                                updateColumnVals(true);
                            };
                            var updateBarcodes = function (value) {
                                var parseBarcodes = function (anArray) {
                                    if (anArray.length > cswPublic.rows[rowid].quantityValues.containerNo) {
                                        anArray.splice(cswPublic.rows[rowid].quantityValues.containerNo, anArray.length - cswPublic.rows[rowid].quantityValues.containerNo);
                                    }
                                    value = anArray.join(',');
                                    if (',' === value[value.length - 1]) { //string out trailing commas
                                        value = value.substring(0, value.length - 1);
                                    }
                                };
                                var barcodeToParse = Csw.delimitedString(Csw.string(value).trim(), { removeDuplicates: false }).array;
                                parseBarcodes(barcodeToParse);
                                cswPublic.rows[rowid].quantityValues.barcodes = value;
                            };

                            var onSizeChange = function (checkControl) {
                                updateSizeVals();
                                cswPrivate.getQuantity(function () {
                                    if (checkControl && !cswPublic.rows[rowid].qtyControl) {
                                        buildQtyCtrl(cswPublic.rows[rowid].qtyCell);
                                    }

                                    cswPublic.rows[rowid].qtyControl.refresh(cswPrivate.quantity);
                                    updateColumnVals(true);
                                });
                            };

                            var updateBalanceInterface = function (balanceData) {
                                cswPublic.rows[rowid].qtyControl.setQtyVal(balanceData.CurrentWeight);

                                Csw.each(cswPrivate.quantity.options, function (option) {
                                    if (balanceData.UnitOfMeasurement == option.value) {
                                        cswPublic.rows[rowid].qtyControl.setUnitVal(option.id);
                                    }
                                }); //Csw.each( cswPrivate.state.initialQuantity.options

                                updateColumnVals(false);
                            };//updateInterface


                            var updateBalanceMenuInfo = function (button, showMenu) {
                                button.hideMenu();
                                Csw.ajaxWcf.post({
                                    urlMethod: 'Balances/ListConnectedBalances',
                                    success: function (data) {
                                        button.menu.removeAll();

                                        Csw.each(data.BalanceList, function (balance) {
                                            if (false === Csw.isNullOrEmpty(balance.NbtName)) {
                                                button.menu.add({
                                                    text: balance.NbtName + ' - ' + balance.CurrentWeight + balance.UnitOfMeasurement,
                                                    handler: function () {
                                                        updateBalanceInterface(balance);
                                                        cswPublic.rows[rowid].balanceControl.setText(balance.NbtName);
                                                        cswPublic.rows[rowid].balanceControl.setHandler(function () { getBalanceInformation(balance.NodeId); });
                                                    }
                                                });//button.menu.add -- Csw.each(data.BalanceList
                                            }//if (false === Csw.isNullOrEmpty(balance.NbtName)) {
                                        });//Csw.each(data.BalanceList)
                                        if (true == showMenu) {
                                            button.showMenu();
                                        }
                                    }//success -- ListConnectedBalances
                                }); //Csw.ajaxWcf.post
                            };//updateBalanceMenuInfo


                            var getBalanceInformation = function (balanceIn) {
                                Csw.ajaxWcf.post({
                                    urlMethod: 'Balances/getBalanceInformation',
                                    data: balanceIn,
                                    success: function (data) {
                                        var balance = data.BalanceList[0];
                                        updateBalanceInterface(balance);
                                    }//success
                                }); //Csw.ajaxWcf.post({
                            };//getDefaultBalanceInformation

                            switch (columnName) {
                                case cswPrivate.config.numberName:
                                    cswPublic.rows[rowid].containerNoControl = cswCell.numberTextBox({
                                        name: 'containerCount',
                                        value: '',
                                        isRequired: true,
                                        MinValue: cswPrivate.containerMinimum,
                                        MaxValue: cswPublic.containerlimit,
                                        width: (3 * 8) + 'px', //3 characters wide, 8 is the characters-to-pixels ratio
                                        Precision: 0,
                                        onChange: function (value) {
                                            cswPublic.rows[rowid].quantityValues.containerNo = value;
                                            updateTotalContainerCount();
                                            if (false === Csw.isNullOrEmpty(cswPublic.rows[rowid].barcodeControl)) {
                                                updateBarcodes(cswPublic.rows[rowid].barcodeControl.val());
                                            }
                                            Csw.tryExec(cswPrivate.onChange, cswPublic.quantities());
                                        }
                                    });
                                    cswPublic.rows[rowid].quantityValues.containerNo = cswPublic.rows[rowid].containerNoControl.val();
                                    break;
                                case cswPrivate.config.sizeName:
                                    cswPublic.rows[rowid].sizeControl = cswCell.nodeSelect({
                                        name: 'Size',
                                        isRequired: true,
                                        overrideNodelinkValidation: true,
                                        doGetNodes: false, //Case 31096 - if the options below are empty, they're empty for a reason.
                                        options: cswPrivate.sizeSelectOpts,
                                        showSelectOnLoad: true,
                                        objectClassName: 'SizeClass',
                                        addNodeDialogTitle: 'Size',
                                        relatedTo: {
                                            objectClassName: 'MaterialClass',
                                            relatednodeid: cswPrivate.materialId
                                        },
                                        onChange: function () {
                                            onSizeChange(false); //Case 31328 - on change we don't want to create the quantity control, it should be there
                                        },
                                        onSuccess: function () {
                                            //onSizeChange();
                                        },
                                        onAfterAdd: function () {
                                            onSizeChange(true); //Case 31328 - when we add a size we need to check if the qty ctrl is there if there were no sizes before
                                        },
                                        allowAdd: true,
                                        viewid: cswPrivate.sizeViewId
                                    });
                                    updateSizeVals();
                                    break;
                                case cswPrivate.config.quantityName:
                                    cswPublic.rows[rowid].qtyCell = cswCell;
                                    cswPrivate.getQuantity(function () {
                                        buildQtyCtrl();
                                    });
                                    break;
                                case cswPrivate.config.barcodeName:
                                    cswPublic.rows[rowid].barcodeControl = cswCell.textArea({
                                        name: 'containerBarcodes',
                                        rows: 1,
                                        cols: 14,
                                        onKeyUp: function (value, keyVal) {
                                            value = value.replace(/[\r\n]/g, ",");
                                            cswPublic.rows[rowid].barcodeControl.val(value);
                                            updateBarcodes(value);
                                            if (keyVal == 13) {
                                                var newHeight = 7 + cswPublic.rows[rowid].barcodeControl[0].scrollHeight;
                                                if (newHeight < 45) {
                                                    cswPublic.rows[rowid].barcodeControl.css('height', newHeight + "px");
                                                }
                                            }
                                        }
                                    });
                                    break;
                                case cswPrivate.config.balanceName:
                                    cswPublic.rows[rowid].balanceControl = cswCell.buttonSplit({
                                        buttonText: 'Read from Balance',
                                        width: 137,
                                        arrowHandler: function (button) { updateBalanceMenuInfo(button, true); },
                                    });

                                    //check if user has a default balance. If so, change the behavior of clicking the button
                                    var userDefaultBalance = Csw.currentUser.defaults().DefaultBalanceId;

                                    if (null != userDefaultBalance) {
                                        Csw.ajaxWcf.post({
                                            urlMethod: 'Balances/getBalanceInformation',
                                            data: userDefaultBalance,
                                            success: function (data) {
                                                var Balance = data.BalanceList[0];
                                                if (Balance.IsActive) {
                                                    cswPublic.rows[rowid].balanceControl.setText(Balance.NbtName);
                                                    cswPublic.rows[rowid].balanceControl.setHandler(function () { getBalanceInformation(Balance.NodeId); });
                                                    updateBalanceInterface(Balance);
                                                } else {
                                                    cswPublic.rows[rowid].balanceControl.setText(Balance.NbtName + " (Inactive)");
                                                }
                                            }//success
                                        });
                                    } //if (null != userDefaultBalance) 


                                    break;
                            }
                        },
                        onAdd: function (newRowid) {
                            var newAmount = {};
                            //This while loop serves as a buffer to remove the +1/-1 issues when comparing the data with the table cell rows in the thingrid.
                            //This puts the burden on the user of thingrid to ensure their data lines up with the table cells.
                            //Also, undefined quantity values break the serverside foreach loop, so an empty one is inserted in each element (including deleted elements).
                            while (cswPublic.rows.length < newRowid) {
                                cswPublic.rows[newRowid] = { quantityValues: {} };
                            }
                            newAmount = {
                                containerNo: '',
                                quantity: '',
                                sizeid: '',
                                sizename: '',
                                unit: '',
                                unitid: '',
                                barcodes: '',
                                containerids: []
                            };
                            var extractNewAmount = function (object) {
                                var ret = Csw.extend({}, object, true);
                                return ret;
                            };
                            cswPublic.rows[newRowid] = { quantityValues: extractNewAmount(newAmount) };
                        },
                        onDelete: function (rowid) {
                            delete cswPublic.rows[rowid];
                            cswPublic.rows[rowid] = { quantityValues: {} };
                            updateTotalContainerCount();
                            Csw.tryExec(cswPrivate.onChange, cswPublic.quantities());
                        }
                    });

                    cswParent.br();
                };

                var ready = Csw.promises.all();

                if (false === Csw.isNullOrEmpty(cswPrivate.materialId)) {
                    //pre-fill the size select
                    ready.push(Csw.ajaxWcf.post({
                        urlMethod: 'Nodes/getSizes',
                        data: {
                            NodeId: cswPrivate.materialId
                        },
                        success: function (response) {
                            response.Nodes.forEach(function (obj) {
                                cswPrivate.sizeSelectOpts.push({ id: obj.NodeId, value: obj.NodeName, nodelink: obj.NodeLink });
                            });

                            if (cswPrivate.sizeSelectOpts.length > 0) {
                                cswPrivate.sizeSelectOpts[0].isSelected = true;
                                cswPrivate.selectedSizeId = cswPrivate.sizeSelectOpts[0].id;
                            }
                            cswPrivate.sizeViewId = response.ViewId;
                        }
                    }));
                }

                ready.then(makeGrid);

            }());

            (function _post() { }());

        });

        return cswPublic;

    });
}());

