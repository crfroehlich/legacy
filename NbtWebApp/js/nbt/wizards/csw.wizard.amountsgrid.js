/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {
    Csw.nbt.wizard.amountsGrid = Csw.nbt.wizard.amountsGrid ||
        Csw.nbt.wizard.register('amountsGrid', function (cswParent, options) {
            'use strict';
            ///<summary>Creates an amounts thin grid with an Add form.</summary>
            var cswPublic = {
                quantities: [],
                qtyControl: null,
                sizeControl: null,
                barcodeControl: null,
                amountForm: null,
                thinGrid: null,
                selectedSizeId: null
            };

            Csw.tryExec(function () {

                var cswPrivate = {
                    ID: 'wizardAmountsThinGrid',
                    onAdd: null,
                    onDelete: null, //function(quantities.length)
                    quantity: {},
                    containerlimit: 25,
                    makeId: function (text) {
                        return text;
                    },
                    containerMinimum: 1,
                    action: 'Receive',
                    selectedSizeId: null,
                    relatedNodeId: null,
                    materialId: null,
                    rows: [],
                    config: {
                        numberName: 'No. Containers *',
                        sizeName: 'Size *',
                        quantityName: 'Net Quantity *',                                                
                        barcodeName: 'Barcodes (Optional)'
                    },
                    customBarcodes: false
                };
                if (options) {
                    $.extend(cswPrivate, options);
                }

                cswPrivate.header = [cswPrivate.config.numberName];
                if (false === Csw.isNullOrEmpty(cswPrivate.materialId) && cswPrivate.action === 'Receive') {
                    cswPrivate.header = cswPrivate.header.concat([cswPrivate.config.sizeName]);
                }
                cswPrivate.header = cswPrivate.header.concat([cswPrivate.config.quantityName]);
                if (cswPrivate.customBarcodes) {
                    cswPrivate.header = cswPrivate.header.concat([cswPrivate.config.barcodeName]);
                }

                if (cswPrivate.rows.length === 0) {
                    cswPrivate.rows.push(cswPrivate.header);
                } else {
                    var firstRow = cswPrivate.rows.splice(0, 1, cswPrivate.header);
                    cswPrivate.rows.push(firstRow);
                }

                cswPrivate.getQuantity = function () {
                    var ret = false;
                    if (Csw.isNullOrEmpty(cswPrivate.selectedSizeId) && false === Csw.isNullOrEmpty(cswPrivate.relatedNodeId)) {
                        Csw.ajax.post({
                            urlMethod: 'getSize',
                            async: false,
                            data: { RelatedNodeId: cswPrivate.relatedNodeId },
                            success: function (data) {
                                cswPrivate.selectedSizeId = data.sizeid;
                                ret = false === Csw.isNullOrEmpty(cswPrivate.selectedSizeId);
                            }
                        });
                    }
                    if (false === Csw.isNullOrEmpty(cswPrivate.selectedSizeId)) {
                        Csw.ajax.post({//TODO - for receive, make only Unit Readonly - if not quantityeditable, make both quantity and unit readonly
                            urlMethod: 'getQuantity',
                            async: false,
                            data: { SizeId: cswPrivate.selectedSizeId, Action: cswPrivate.action },
                            success: function (data) {
                                cswPrivate.quantity = data;
                                ret = false === Csw.isNullOrEmpty(cswPrivate.quantity);
                            }
                        });
                    }
                    if (false === ret) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Wizard amounts grid without the Initial Quantity of a Size.', '', 'csw.wizard.amountsgrid.js', 68));
                    }
                    return ret;
                };

                (function _pre() {
                    if (Csw.isNullOrEmpty(cswParent)) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Wizard amounts grid without a parent.', '', 'csw.wizard.amountsgrid.js', 22));
                    }
                    cswPrivate.count = 0;
                    cswParent.br({ number: 2 });
                    cswParent.span({ text: '<b>Enter the Amounts to ' + cswPrivate.action + ':</b>' });
                    cswParent.br({ number: 2 });

                    //This object will be mutated on each call to makeAddRow below, but the reference to the object is constant--so we can pass it to onAdd.
                    //There is a fragility here in that if you were to pass newAmount to an external function, the values of the properties of the object would be unknowable
                    //This is a traditional MVC problem which would easily by templating libraries (Spine, Ember, Backbone, Knockout, etc);
                    //however, as these two modules are so tightly coupled, the fragility should be an acceptable risk for now.
                    var newAmount = {
                        rowid: 1,
                        containerNo: 1,
                        quantity: '',
                        sizeid: '',
                        sizename: '',
                        unit: '',
                        unitid: '',
                        barcodes: ''
                    };

                    var extendNewAmount = function (object) {
                        //To mitigate the risk of unknowingly passing the outer scope thisAmount, we're explicitly mapping the values down
                        $.extend(newAmount, object);
                    };

                    var extractNewAmount = function (object) {
                        var ret = $.extend(true, {}, object);
                        return ret;
                    };

                    cswPublic.amountForm = cswParent.form();
                    cswPublic.thinGrid = cswPublic.amountForm.thinGrid({
                        linkText: '',
                        hasHeader: true,
                        rows: cswPrivate.rows,
                        allowDelete: true,
                        allowAdd: true,
                        makeAddRow: function (cswCell, columnName, rowid) {
                            'use strict';
                            var thisAmount = {
                                rowid: rowid,
                                containerNo: 1,
                                quantity: '',
                                sizeid: '',
                                sizename: '',
                                unit: '',
                                unitid: '',
                                barcodes: ''
                            };

                            switch (columnName) {
                                case cswPrivate.config.numberName:
                                    var countControl = cswCell.numberTextBox({
                                        ID: Csw.tryExec(cswPrivate.makeId, 'containerCount'),
                                        value: thisAmount.containerNo,
                                        MinValue: cswPrivate.containerMinimum,
                                        MaxValue: (cswPrivate.containerlimit - Csw.number(cswPrivate.count, 0)),
                                        ceilingVal: (cswPrivate.containerlimit - Csw.number(cswPrivate.count, 0)),
                                        width: (3 * 8) + 'px', //3 characters wide, 8 is the characters-to-pixels ratio
                                        Precision: 0,
                                        Required: true,
                                        onChange: function (value) {
                                            extendNewAmount({ containerNo: value });
                                        }
                                    });
                                    break;
                                case cswPrivate.config.sizeName:
                                    cswPublic.sizeControl = cswCell.nodeSelect({
                                        ID: Csw.tryExec(cswPrivate.makeId, 'sizes'),
                                        async: false,
                                        objectClassName: 'SizeClass',
                                        relatedTo: {
                                            objectClassName: 'MaterialClass',
                                            nodeId: cswPrivate.materialId
                                        },
                                        onSuccess: function () {

                                        },
                                        onSelect: function () {
                                            cswPrivate.selectedSizeId = cswPublic.sizeControl.selectedNodeId();
                                            cswPrivate.getQuantity();
                                            cswPublic.qtyControl.quantityTextBox.val(cswPrivate.quantity.value);
                                            cswPublic.qtyControl.quantityValue = cswPrivate.quantity.value;
                                            cswPublic.qtyControl.unitSelect.val(cswPrivate.quantity.nodeid);
                                            cswPublic.qtyControl.unitText = cswPrivate.quantity.name;
                                        }
                                    });
                                    cswPrivate.selectedSizeId = cswPublic.sizeControl.selectedNodeId();                                    
                                    break;
                                case cswPrivate.config.quantityName:
                                    cswPrivate.getQuantity();
                                    cswPrivate.quantity.ID = Csw.tryExec(cswPrivate.makeId, 'containerQuantity');
                                    cswPrivate.quantity.qtyWidth = (7 * 8) + 'px'; //7 characters wide, 8 is the characters-to-pixels ratio                                    
                                    cswPublic.qtyControl = cswCell.quantity(cswPrivate.quantity);
                                    break;
                                case cswPrivate.config.barcodeName:
                                    cswPublic.barcodeControl = cswCell.textArea({
                                        ID: Csw.tryExec(cswPrivate.makeId, 'containerBarcodes'),
                                        rows: 1,
                                        cols: 14,
                                        onChange: function (value) {
                                            extendNewAmount({ barcodes: value });
                                        }
                                    });
                                    break;
                            }
                            extendNewAmount(thisAmount);
                        },
                        onAdd: function () {
                            var newCount = cswPrivate.count + Csw.number(newAmount.containerNo);
                            if (newCount <= cswPrivate.containerlimit) {
                                cswPrivate.count = newCount;

                                var parseBarcodes = function (anArray) {
                                    if (anArray.length > newAmount.containerNo) {
                                        anArray.splice(0, anArray.length - newAmount.containerNo);
                                    }
                                    newAmount.barcodes = barcodeToParse.join(',');
                                };
                                var barcodeToParse = Csw.delimitedString(newAmount.barcodes).array;
                                parseBarcodes(barcodeToParse);

                                if (cswPublic.amountForm.isFormValid()) {
                                    newAmount.quantity = cswPublic.qtyControl.quantityValue;
                                    newAmount.unit = cswPublic.qtyControl.unitText;
                                    newAmount.unitid = cswPublic.qtyControl.unitVal;
                                    //we need to make sure the columns here match the header columns
                                    var formCols = [newAmount.containerNo];
                                    if (false === Csw.isNullOrEmpty(cswPrivate.materialId) && cswPrivate.action === 'Receive') {
                                        newAmount.sizeid = cswPublic.sizeControl.selectedNodeId();
                                        newAmount.sizename = cswPublic.sizeControl.selectedText();
                                        formCols = formCols.concat([newAmount.sizename]);
                                    }
                                    formCols = formCols.concat([newAmount.quantity + ' ' + newAmount.unit]);
                                    if (cswPrivate.customBarcodes) {
                                        formCols = formCols.concat([newAmount.barcodes]);
                                    }
                                    newAmount.rowid = cswPublic.thinGrid.addRows(formCols);
                                    cswPublic.quantities.push(extractNewAmount(newAmount));
                                }
                            } else {
                                $.CswDialog('AlertDialog', 'The limit for containers created at receipt is [' + cswPrivate.containerlimit + ']. You have already added [' + cswPrivate.count + '] containers.', 'Cannot add [' + newCount + '] containers.');
                            }
                            Csw.tryExec(cswPrivate.onAdd, (cswPrivate.count > 0));
                        },
                        onDelete: function (rowid) {
                            Csw.debug.assert(false === Csw.isNullOrEmpty(rowid), 'Rowid is null.');
                            var quantityToRemove = cswPublic.quantities.filter(function (quantity, index, array) { return quantity.rowid === rowid; });
                            if (false === Csw.isNullOrEmpty(quantityToRemove, true)) {
                                cswPrivate.count -= Csw.number(quantityToRemove[0].containerNo, 0);
                                var reducedQuantities = cswPublic.quantities.filter(function (quantity, index, array) { return quantity.rowid !== rowid; });
                                Csw.debug.assert(reducedQuantities !== cswPublic.quantities, 'Rowid is null.');
                                cswPublic.quantities = reducedQuantities;
                                Csw.tryExec(cswPrivate.onDelete, (cswPrivate.count > 0));
                            }
                        }
                    });
                } ());

                (function _post() {

                } ());

            });

            return cswPublic;

        });
} ());

