/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {

    var cswDispenseWizardStateName = 'cswDispenseWizardStateName';
    
    Csw.nbt.dispenseContainerWizard = Csw.nbt.dispenseContainerWizard ||
        Csw.nbt.register('dispenseContainerWizard', function (cswParent, options) {
            'use strict';

            //#region Variable Declaration
            var cswPrivate = {
                ID: 'cswDispenseContainerWizard',
                state: {
                    sourceContainerNodeId: '',
                    currentQuantity: '',
                    currentUnitName: '',
                    capacity: '',
                    dispenseType: 'Dispense into a Child Container',
                    quantity: '',
                    unitId: '',
                    sizeId: '',
                    containerNodeTypeId: '',
                    materialname: '',
                    barcode: '',
                    location: '',
                    requestItemId: ''
                },
                onCancel: null,
                onFinish: null,
                startingStep: 1,
                wizard: '',
                wizardSteps: {
                    1: 'Select a Dispense Type',
                    2: 'Select Amount(s)'
                },
                stepOneComplete: false, stepTwoComplete: false,
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                dispenseTypes: {
                    Dispense: 'Dispense into a Child Container',
                    Use: 'Dispense for Use',
                    Waste: 'Waste Material',
                    Add: 'Add Material to Container'
                },
                divStep1: '', divStep2: '',
                quantityControl: null,
                title: 'Dispense from Container',
            };
            if (options) $.extend(cswPrivate, options);

            var cswPublic = cswParent.div();
            cswPrivate.currentStepNo = cswPrivate.startingStep;

            cswPrivate.toggleButton = function (button, isEnabled, doClick) {
                var btn;
                if (Csw.bool(isEnabled)) {
                    btn = cswPrivate.wizard[button].enable();
                    if (Csw.bool(doClick)) {
                        btn.click();
                    }
                } else {
                    cswPrivate.wizard[button].disable();
                }
            };

            cswPrivate.makeStepId = function (suffix, stepNo) {
                var step = stepNo || cswPrivate.currentStepNo;
                return Csw.makeId({ prefix: 'step_' + step, ID: cswPrivate.ID, suffix: suffix });
            };

            cswPrivate.validationFailed = function () {
                cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
            };

            //Step 1. Select a Dispense Type.
            cswPrivate.makeStepOne = (function () {
                return function () {

                    var dispenseTypeTable;

                    var toggleNext = function() {
                        return cswPrivate.toggleButton(cswPrivate.buttons.next, false === Csw.isNullOrEmpty(cswPrivate.state.sourceContainerNodeId));
                    };
                    var resetStepTwo = function() {
                        cswPrivate.stepTwoComplete = false;
                    };
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

                    var initStepOne = Csw.method(function () {
                        var dispenseTypeSelect;

                        var makeTypeSelect = function () {

                            dispenseTypeTable.cell(5, 1).br({ number: 2 });
                            dispenseTypeTable.cell(6, 1).span({ text: 'Select a dispense type:' });

                            dispenseTypeSelect = dispenseTypeTable.cell(7, 1).select({
                                ID: cswPrivate.makeStepId('setDispenseTypePicklist'),
                                cssclass: 'selectinput',
                                values: cswPrivate.dispenseTypes,
                                selected: cswPrivate.dispenseTypes.Dispense,
                                onChange: function () {
                                    if (false === Csw.isNullOrEmpty(dispenseTypeSelect.val())) {
                                        if(dispenseTypeSelect.val() !== cswPrivate.state.dispenseType) {
                                            resetStepTwo();
                                        }
                                        cswPrivate.state.dispenseType = dispenseTypeSelect.val();
                                    }
                                    toggleNext();
                                }
                            });
                            cswPrivate.state.dispenseType = dispenseTypeSelect.val();
                        };

                        var makeContainerGrid = function() {
                            Csw.ajax.post({
                                urlMethod: 'getDispenseContainerView',
                                data: {
                                    RequestItemId: cswPrivate.state.requestItemId
                                },
                                success: function (data) {
                                    if (Csw.isNullOrEmpty(data.viewid)) {
                                        Csw.error.throwException(Csw.error.exception('Could not get a grid of containers for this request item.', '', 'csw.dispensecontainer.js', 141));
                                    }

                                    cswPrivate.containerGrid = Csw.nbt.wizard.nodeGrid(dispenseTypeTable.cell(5, 1), {
                                        hasMenu: false,
                                        viewid: data.viewid,
                                        ReadOnly: true,
                                        onSelect: function () {
                                            if (cswPrivate.state.sourceContainerNodeId !== cswPrivate.containerGrid.getSelectedNodeId()) {
                                                resetStepTwo();
                                            }
                                            cswPrivate.state.sourceContainerNodeId = cswPrivate.containerGrid.getSelectedNodeId();
                                            toggleNext();
                                        },
                                        onSuccess: makeTypeSelect
                                    });
                                }
                            });
                        };

                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();

                        cswPrivate.divStep1.span({ text: 'Confirm the container to use for this dispense, and select a type of dispense to perform.' });

                        cswPrivate.divStep1.br({number: 2});

                        dispenseTypeTable = cswPrivate.divStep1.table({
                            ID: cswPrivate.makeStepId('setDispenseTypeTable'),
                            width: '100%',
                            cellpadding: '1px',
                            cellalign: 'left',
                            cellvalign: 'middle'
                        });

                        if (false === Csw.isNullOrEmpty(cswPrivate.state.barcode)) {
                            dispenseTypeTable.cell(1, 1).span({ text: 'Barcode: [' + Csw.string(cswPrivate.state.barcode) + ']' });
                        }
                        if (false === Csw.isNullOrEmpty(cswPrivate.state.materialname)) {
                            dispenseTypeTable.cell(2, 1).span({ text: 'On Material: ' + Csw.string(cswPrivate.state.materialname) });
                        }
                        if (false === Csw.isNullOrEmpty(cswPrivate.state.location)) {
                            dispenseTypeTable.cell(3, 1).span({ text: 'At Location: ' + Csw.string(cswPrivate.state.location) });
                        }
                        
                        if (false === Csw.isNullOrEmpty(cswPrivate.state.requestItemId)) {
                            makeContainerGrid();
                        }
                        else if(Csw.isNullOrEmpty(cswPrivate.state.sourceContainerNodeId)) {
                            Csw.error.throwException(Csw.error.exception('Cannot dispense without a source container.', '', 'csw.dispensecontainer.js', 173));
                        } else {
                            makeTypeSelect();
                        }

                    });

                    if (false === cswPrivate.stepOneComplete) {
                        initStepOne();
                        toggleNext();
                        cswPrivate.stepOneComplete = true;
                    }
                };
            }());

            //Step 2. Select Amount
            //state.dispenseType != Dispense ? 
            //Select a state.quantity :
            //Select the number of destination containers and their quantities.
            cswPrivate.makeStepTwo = (function () {
                return function() {
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    if (false === cswPrivate.stepTwoComplete) {
                        var quantityTable,
                            blankText = '[Select One]';

                        cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.empty();
                        cswPrivate.divStep2.span({ text: 'Confirm the source container and define the amounts to dispense:' });
                        cswPrivate.divStep2.br();

                        quantityTable = cswPrivate.divStep2.table({
                            ID: cswPrivate.makeStepId('setQuantityTable'),
                            cellpadding: '1px',
                            cellvalign: 'middle'
                        });

                        quantityTable.cell(1, 1).br();
                        if (false === Csw.isNullOrEmpty(cswPrivate.state.barcode)) {
                            quantityTable.cell(2, 1).span({ text: 'Barcode: [' + Csw.string(cswPrivate.state.barcode) + ']' });
                        }
                        if (false === Csw.isNullOrEmpty(cswPrivate.state.materialname)) {
                            quantityTable.cell(3, 1).span({ text: 'On Material: ' + Csw.string(cswPrivate.state.materialname) });
                        }
                        if (false === Csw.isNullOrEmpty(cswPrivate.state.location)) {
                            quantityTable.cell(4, 1).span({ text: 'At Location: ' + Csw.string(cswPrivate.state.location) });
                        }
                        if (false === Csw.isNullOrEmpty(cswPrivate.state.currentQuantity)) {
                            quantityTable.cell(5, 1).span({ text: 'Current quantity: ' + cswPrivate.state.currentQuantity + ' ' + cswPrivate.state.currentUnitName }).br();
                        }
                        quantityTable.cell(6, 1).br({ number: 1 });

                        var makeContainerSelect = function() {
                            var containerTypeTable = quantityTable.cell(7, 1).table({
                                ID: cswPrivate.makeStepId('setContainerTypeTable'),
                                cellpadding: '1px',
                                cellvalign: 'middle'
                            }).hide();

                            containerTypeTable.cell(1, 1).span({ text: 'Select a Container Type' });

                            var containerTypeSelect = containerTypeTable.cell(2, 1).nodeTypeSelect({
                                ID: Csw.makeSafeId('nodeTypeSelect'),
                                objectClassName: 'ContainerClass',
                                blankOptionText: blankText,
                                onSelect: function(data, nodeTypeCount) {
                                    if (blankText !== containerTypeSelect.val()) {
                                        cswPrivate.state.containerNodeTypeId = containerTypeSelect.val();
                                    }
                                },
                                onSuccess: function(data, nodeTypeCount, lastNodeTypeId) {
                                    if (Csw.number(nodeTypeCount) > 1) {
                                        containerTypeTable.show();
                                    } else {
                                        cswPrivate.state.containerNodeTypeId = lastNodeTypeId;
                                    }
                                }
                            });
                            containerTypeTable.cell(3, 1).br();
                        };

                        var makeQuantityForm = function() {

                            cswPrivate.amountsGrid = Csw.nbt.wizard.amountsGrid(quantityTable.cell(8, 1), {
                                ID: cswPrivate.wizard.makeStepId('wizardAmountsThinGrid'),
                                onAdd: function() {
                                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                                },
                                quantity: cswPrivate.state.capacity,
                                containerlimit: cswPrivate.containerlimit,
                                makeId: cswPrivate.wizard.makeStepId,
                                containerMinimum: 0,
                                action: 'Dispense',
                                relatedNodeId: cswPrivate.state.sourceContainerNodeId,
                                selectedSizeId: cswPrivate.state.sizeId
                            });
                        };

                        if (cswPrivate.state.dispenseType === cswPrivate.dispenseTypes.Dispense) {
                            makeContainerSelect();
                            makeQuantityForm();
                        } else {
                            quantityTable.cell(8, 1).span({ text: 'Set quantities for dispense:' });
                            cswPrivate.quantityControl = quantityTable.cell(9, 1).quantity(cswPrivate.state.capacity);

                            cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                        }
                        cswPrivate.stepTwoComplete = true;
                    }

                };
            }());

            cswPrivate.handleNext = function (newStepNo) {
                cswPrivate.currentStepNo = newStepNo;
                Csw.clientDb.setItem(cswPrivate.ID + '_' + cswDispenseWizardStateName, cswPrivate.state);
                switch (newStepNo) {
                    case 2:
                        if (Csw.isNullOrEmpty(cswPrivate.state.sourceContainerNodeId)) {
                            Csw.error.throwException(Csw.error.exception('Cannot dispense without a source container.', '', 'csw.dispensecontainer.js', 283));
                        } else {
                            if (Csw.isNullOrEmpty(cswPrivate.state.barcode) ||
                                Csw.isNullOrEmpty(cswPrivate.state.materialname) ||
                                Csw.isNullOrEmpty(cswPrivate.state.location) ||
                                Csw.isNullOrEmpty(cswPrivate.state.containerNodeTypeId)) {

                                Csw.ajax.post({
                                    urlMethod: 'getDispenseSourceContainerData',
                                    data: {
                                        ContainerId: cswPrivate.state.sourceContainerNodeId
                                    },
                                    async: false,
                                    success: function (data) {
                                        cswPrivate.state.barcode = data.barcode;
                                        cswPrivate.state.materialname = data.materialname;
                                        cswPrivate.state.location = data.location;
                                        cswPrivate.state.containerNodeTypeId = data.nodetypeid;
                                        cswPrivate.state.unitId = data.unitid;
                                        cswPrivate.state.currentQuantity = data.quantity;
                                        cswPrivate.state.currentUnitName = data.unit;
                                        cswPrivate.state.sizeId = data.sizeid;
                                    }
                                });
                            }
                            cswPrivate.makeStepTwo(true);
                        }
                        break;
                }
            };

            cswPrivate.handlePrevious = function (newStepNo) {
                cswPrivate.currentStepNo = newStepNo;
                Csw.clientDb.setItem(cswPrivate.ID + '_' + cswDispenseWizardStateName, cswPrivate.state);
                switch (newStepNo) {
                    case 1:
                        cswPrivate.makeStepOne();
                        break;
                }
            };

            cswPrivate.onConfirmFinish = function () {
                cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                cswPrivate.toggleButton(cswPrivate.buttons.cancel, false);
                cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

                var designGrid = 'Unknown';
                if (false === Csw.isNullOrEmpty(cswPrivate.amountsGrid)) {
                    designGrid = Csw.serialize(cswPrivate.amountsGrid.quantities);
                }
                if (false === Csw.isNullOrEmpty(cswPrivate.quantityControl)) {
                    cswPrivate.state.quantity = cswPrivate.quantityControl.quantityValue;
                    cswPrivate.state.unitId = cswPrivate.quantityControl.unitVal;
                }

                var jsonData = {
                    SourceContainerNodeId: Csw.string(cswPrivate.state.sourceContainerNodeId),
                    DispenseType: Csw.string(cswPrivate.state.dispenseType),
                    Quantity: Csw.string(cswPrivate.state.quantity),
                    UnitId: Csw.string(cswPrivate.state.unitId),
                    ContainerNodeTypeId: Csw.string(cswPrivate.state.containerNodeTypeId),
                    DesignGrid: designGrid,
                    RequestItemId: Csw.string(cswPrivate.state.requestItemId)
                };

                Csw.ajax.post({
                    urlMethod: 'finalizeDispenseContainer',
                    data: jsonData,
                    success: function (data) {
                        var viewId = data.viewId;
                        Csw.tryExec(cswPrivate.onFinish, viewId);
                        Csw.clientDb.removeItem(cswPrivate.ID + '_' + cswDispenseWizardStateName);
                        if (false === Csw.isNullOrEmpty(data.barcodeId)) {
                            $.CswDialog('GenericDialog', {
                                'div': Csw.literals.div().span({ text: 'Would you like to print Labels for the new Containers?' }),
                                'title': 'Print Labels?',
                                'onOk': function () {
                                    $.CswDialog('PrintLabelDialog', { 'nodeid': cswPrivate.state.sourceContainerNodeId, 'propid': data.barcodeId });
                                },
                                'okText': 'Yes',
                                'cancelText': 'No'
                            });

                        }
                    },
                    error: function () {
                        cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                        cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    }
                });
            };

            (function () {
                var state = { };
                if(Csw.isNullOrEmpty(cswPrivate.state.sourceContainerNodeId)) {
                    state = Csw.clientDb.getItem(cswPrivate.ID + '_' + cswDispenseWizardStateName);
                    $.extend(cswPrivate.state, state);
                }

                cswPrivate.wizard = Csw.layouts.wizard(cswPublic, {
                    ID: Csw.makeId({ ID: cswPrivate.ID, suffix: 'wizard' }),
                    sourceContainerNodeId: cswPrivate.state.sourceContainerNodeId,
                    currentQuantity: cswPrivate.state.currentQuantity,
                    currentUnitName: cswPrivate.state.currentUnitName,
                    capacity: cswPrivate.state.capacity,
                    Title: Csw.string(cswPrivate.title),
                    StepCount: 2,
                    Steps: cswPrivate.wizardSteps,
                    StartingStep: cswPrivate.startingStep,
                    FinishText: 'Finish',
                    onNext: cswPrivate.handleNext,
                    onPrevious: cswPrivate.handlePrevious,
                    onCancel: cswPrivate.onCancel,
                    onFinish: cswPrivate.onConfirmFinish,
                    doNextOnInit: false
                });

                cswPrivate.makeStepOne();
            }());

            return cswPublic;
        });
}());

