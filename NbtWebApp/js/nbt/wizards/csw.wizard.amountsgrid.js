/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {
    Csw.nbt.wizard.amountsGrid = Csw.nbt.wizard.amountsGrid ||
        Csw.nbt.wizard.register('amountsGrid', function (cswParent, options) {
            'use strict';
            ///<summary>Creates an amounts thin grid with an Add form.</summary>
            var cswPublic = {
                quantities: [],
                countControl: null,
                qtyControl: null,
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
                    relatedNodeId: null
                };
                if (options) $.extend(cswPrivate, options);

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
                        Csw.ajax.post({
                            urlMethod: 'getQuantity',
                            async: false,
                            data: { SizeId: cswPrivate.selectedSizeId },
                            success: function(data) {
                                cswPrivate.quantity = data;
                                ret = false === Csw.isNullOrEmpty(cswPrivate.quantity);
                            }
                        });
                    }
                    if(false === ret) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Wizard amounts grid without the Initial Quantity of a Size.', '', 'csw.wizard.amountsgrid.js', 68));
                    }
                    return ret;
                };

                (function _pre() {
                    if (Csw.isNullOrEmpty(cswParent)) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Wizard amounts grid without a parent.', '', 'csw.wizard.amountsgrid.js', 22));
                    }
                    if (Csw.isNullOrEmpty(cswPrivate.quantity)) {
                        cswPrivate.getQuantity();
                    }
                    cswParent.span({ text: 'Enter the Amounts to ' + cswPrivate.action + ':' });
                    cswParent.br({ number: 2 });

                    cswParent.br();
                    cswPublic.amountForm = cswParent.form();
                    cswPrivate.amountsTable = cswPublic.amountForm.table();
                    cswPrivate.count = 0;
                    cswPublic.amountForm.br({ number: 2 });
                    cswPublic.thinGrid = cswPublic.amountForm.thinGrid({
                        linkText: '',
                        hasHeader: true, 
                        rows: [['#', 'Quantity', 'Unit', 'Barcode(s)']],
                        allowDelete: true,
                        onDelete: function (rowid) {
                            Csw.debug.assert(false === Csw.isNullOrEmpty(rowid), 'Rowid is null.');
                            var reducedQuantities = cswPublic.quantities.filter(function (quantity, index, array) { return quantity.rowid !== rowid; });
                            Csw.debug.assert(reducedQuantities !== cswPublic.quantities, 'Rowid is null.');
                            cswPublic.quantities = reducedQuantities;
                            if(cswPublic.quantities.length < 1) {
                                cswPublic.thinGrid.hide();
                            }
                            Csw.tryExec(cswPrivate.onDelete, cswPublic.quantities.length);
                        }
                    });
                    cswPublic.thinGrid.hide();
                } ());

                cswPrivate.makeAddAmount = function () {
                    'use strict';
                    cswPrivate.amountsTable.empty();

                    var thisAmount = {
                        rowid: 1,
                        containerNo: 1,
                        quantity: '',
                        unit: '',
                        unitid: '',
                        barcodes: ''
                    };
                    
                    //# of containers
                    cswPublic.countControl = cswPrivate.amountsTable.cell(1, 1).numberTextBox({
                        ID: Csw.tryExec(cswPrivate.makeId, 'containerCount'),
                        labelText: 'Number of Containers: ',
                        useWide: true,
                        value: thisAmount.containerNo,
                        MinValue: cswPrivate.containerMinimum,
                        MaxValue: cswPrivate.containerlimit,
                        ceilingVal: cswPrivate.containerlimit,
                        Precision: 0,
                        Required: true,
                        onChange: function (value) {
                            thisAmount.containerNo = value;
                        }
                    });

                    //Quantity
                    cswPrivate.quantity.labelText = 'Container Quantity: ';
                    cswPrivate.quantity.useWide = true;
                    cswPrivate.quantity.ID = Csw.tryExec(cswPrivate.makeId, 'containerQuantity');
                    cswPublic.qtyControl = cswPrivate.amountsTable.cell(2, 1).quantity(cswPrivate.quantity);

                    //Barcodes
                    cswPublic.barcodeControl = cswPrivate.amountsTable.cell(3, 1).textArea({
                        ID: Csw.tryExec(cswPrivate.makeId, 'containerBarcodes'),
                        labelText: 'Barcodes (Optional): ',
                        useWide: true,
                        onChange: function (value) {
                            thisAmount.barcodes = value;
                        }
                    });

                    //Add
                    cswPrivate.amountsTable.cell(4, 1).buttonExt({
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                        ID: Csw.tryExec(cswPrivate.makeId, 'addBtn'),
                        enabledText: 'Add',
                        disableOnClick: false,
                        onClick: function () {

                            var newCount = cswPrivate.count + Csw.number(thisAmount.containerNo);
                            if (newCount <= cswPrivate.containerlimit) {
                                cswPrivate.count = newCount;
                                
                                var parseBarcodes = function (anArray) {
                                    if (anArray.length > thisAmount.containerNo) {
                                        anArray.splice(0, anArray.length - thisAmount.containerNo);
                                    }
                                    thisAmount.barcodes = barcodeToParse.join(',');
                                };
                                var barcodeToParse = Csw.delimitedString(thisAmount.barcodes).array;
                                parseBarcodes(barcodeToParse);

                                if (cswPublic.amountForm.isFormValid()) {
                                    if (cswPrivate.count > 0) {
                                        cswPublic.thinGrid.show();
                                    }
                                    thisAmount.quantity = cswPublic.qtyControl.quantityValue;
                                    thisAmount.unit = cswPublic.qtyControl.unitText;
                                    thisAmount.unitid = cswPublic.qtyControl.unitVal;
                                    thisAmount.rowid = cswPublic.thinGrid.addRows([thisAmount.containerNo, thisAmount.quantity, thisAmount.unit, thisAmount.barcodes]);
                                    cswPublic.quantities.push(thisAmount);
                                    Csw.tryExec(cswPrivate.onAdd);
                                    cswPrivate.makeAddAmount();
                                }
                            } else {
                                $.CswDialog('AlertDialog', 'The limit for containers created at receipt is [' + cswPrivate.containerlimit + ']. You have already added [' + cswPrivate.count + '] containers.', 'Cannot add [' + newCount + '] containers.');
                            }
                        }
                    });
                };

                (function _post() {
                    cswPrivate.makeAddAmount();
                } ());

            });

            return cswPublic;

        });
} ());

