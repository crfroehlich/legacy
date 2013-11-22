/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {
    Csw.nbt.register('mergeWizard', function (cswParent, options) {
        'use strict';

        //#region Properties
        var cswPrivate = {
            name: 'mergeWizard',
            startingStep: 1,
            stepCount: 0,
            wizard: null,
            buttons: {
                next: 'next',
                prev: 'previous',
                finish: 'finish',
                cancel: 'cancel'
            },
//            node1: null,
//            node2: null,
            node1: { nodeid: 'nodes_51984', nodename: 'acenaphthrene Fluka' },  // DEBUG ONLY
            node2: { nodeid: 'nodes_54960', nodename: 'Acesulfame K' },     // DEBUG ONLY
            propertychoices: {}
        };

        var cswPublic = {};
        //#endregion Properties

        //#region Wizard Functions
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
            return false;
        };

        cswPrivate.toggleStepButtons = function (StepNo) {
            cswPrivate.toggleButton(cswPrivate.buttons.prev, StepNo > 1);
            cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
            cswPrivate.toggleButton(cswPrivate.buttons.finish, StepNo === cswPrivate.stepCount);
            cswPrivate.toggleButton(cswPrivate.buttons.next, StepNo !== cswPrivate.stepCount);
        };

        cswPrivate.reinitSteps = function (startWithStep) {
            for (var i = startWithStep; i <= cswPrivate.stepCount; i++) {
                cswPrivate['step' + i + 'Complete'] = false;
            }
        };

        cswPrivate.handleStep = function (newStepNo) {
            //cswPrivate.setState();
            if (false === Csw.isNullOrEmpty(cswPrivate.onStepChange[cswPrivate.currentStepNo])) {
                cswPrivate.onStepChange[cswPrivate.currentStepNo](cswPrivate.currentStepNo);
            }
            cswPrivate.lastStepNo = cswPrivate.currentStepNo;
            cswPrivate.currentStepNo = newStepNo;
            if (false === Csw.isNullOrEmpty(cswPrivate.stepFunc[newStepNo])) {
                cswPrivate.stepFunc[newStepNo](newStepNo);
            }
        };

        cswPrivate.setWizardSteps = function () {
            var wizardSteps = {};
            cswPrivate.stepFunc = {};
            cswPrivate.onStepChange = {};
            cswPrivate.stepCount = 0;
            var setWizardStep = function (wizardStep) {
                cswPrivate.stepCount++;
                cswPrivate.stepFunc[cswPrivate.stepCount] = wizardStep.makeStep;
                cswPrivate.onStepChange[cswPrivate.stepCount] = wizardStep.onStepChange;
                wizardStep.stepNo = cswPrivate.stepCount;
                wizardSteps[cswPrivate.stepCount] = wizardStep.stepName;
            };
            //Add steps here:
            setWizardStep(cswPrivate.wizardStepMergeWhat);
            setWizardStep(cswPrivate.wizardStepMergeWith);
            setWizardStep(cswPrivate.wizardStepPerformMerge);
            setWizardStep(cswPrivate.wizardStepPreviewResult);
            cswPrivate.reinitSteps(1);
            return wizardSteps;
        };

        cswPrivate.setStepHeader = function (StepNo, Header) {
            cswPrivate['divStep' + StepNo] = cswPrivate['divStep' + StepNo] || cswPrivate.wizard.div(StepNo);
            cswPrivate['divStep' + StepNo].empty();
            cswPrivate['divStep' + StepNo].span({
                text: Header,
                cssclass: 'wizardHelpDesc'
            });
            cswPrivate['divStep' + StepNo].br({ number: 2 });
        };
        //#endregion Wizard Functions

        //#region ctor preInit
        (function _pre() {
            if (options) {
                Csw.extend(cswPrivate, options);
            }
            if (Csw.isNullOrEmpty(cswParent)) {
                Csw.error.throwException(Csw.error.exception('Cannot create a Material Receiving wizard without a parent.', '', 'csw.receivematerial.js', 57));
            }
            //cswPrivate.validateState();
        }());
        //#endregion ctor preInit

        //#region Steps
        cswPrivate.wizardStepMergeWhat = {
            stepName: 'Merge What',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);

                    if (null === cswPrivate.node1) {
                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    }

                    cswPrivate.setStepHeader(StepNo, 'What do you want to merge?');

                    var div = cswPrivate['divStep' + StepNo];
                    
                    cswPrivate.searchWhat = Csw.composites.universalSearch(div, {
                        name: 'searchWhat',
                        nodetypeid: '',
                        objectclassid: '',
                        onBeforeSearch: function () { },
                        onAfterSearch: function () { },
                        onAfterNewSearch: function (searchid) { },
                        onAddView: function (viewid, viewmode) { },
                        onLoadView: function (viewid, viewmode) { },
                        showSave: false,
                        allowEdit: false,
                        allowDelete: false,
                        compactResults: true,
                        suppressButtons: true,
                        extraAction: 'Select',
                        extraActionIcon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.check),
                        universalSearchOnly: true, //No C3 or Structure Search here
                        onExtraAction: function (nodeObj) {
                            cswPrivate.node1 = nodeObj;
                            cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                            cswPrivate.wizard.next.click();
                        }
                    });
                };
            }()),
            onStepChange: function () {}
        }; // wizardStepMergeWhat
        

        cswPrivate.wizardStepMergeWith = {
            stepName: 'Merge With',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);

                    if (null === cswPrivate.node2) {
                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    }

                    cswPrivate.setStepHeader(StepNo, 'What do you want to merge with ' + cswPrivate.node1.nodename + '?');

                    var div = cswPrivate['divStep' + StepNo];
                    
                    cswPrivate.searchWith = Csw.composites.universalSearch(div, {
                        name: 'searchWith',
                        nodetypeid: cswPrivate.node1.nodetypeid,
                        objectclassid: '',
                        onBeforeSearch: function () { },
                        onAfterSearch: function () { },
                        onAfterNewSearch: function (searchid) { },
                        onAddView: function (viewid, viewmode) { },
                        onLoadView: function (viewid, viewmode) { },
                        showSave: false,
                        allowEdit: false,
                        allowDelete: false,
                        compactResults: true,
                        suppressButtons: true,
                        extraAction: 'Select',
                        extraActionIcon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.check),
                        universalSearchOnly: true, //No C3 or Structure Search here
                        onExtraAction: function (nodeObj) {
                            cswPrivate.node2 = nodeObj;
                            cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                            cswPrivate.wizard.next.click();
                        }
                    });
                };
            }())
        }; // wizardStepMergeWith
        

        cswPrivate.wizardStepPerformMerge = {
            stepName: 'Perform Merge',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);
                    cswPrivate.setStepHeader(StepNo, 'Merging ' + cswPrivate.node1.nodename + ' with ' + cswPrivate.node2.nodename + '.' );

                    var div = cswPrivate['divStep' + StepNo];

                    Csw.ajaxWcf.post({
                        urlMethod: 'Nodes/getMergeInfo',
                        data: {
                            NodeId1: cswPrivate.node1.nodeid,
                            NodeId2: cswPrivate.node2.nodeid
                        },
                        success: function (data) {
                            var table = div.table();
                            var row = 1;

                            cswPrivate.mergeData = data;

                            table.addClass('MergeTable');
                            
                            // Header Row
                            var evenodd = "Even";
                            table.cell(row, 1).addClass('MergeTable_Cell MergeTable_LabelCell_' + evenodd + ' MergeTable_LabelCell MergeTable_Header').text('Property');
                            table.cell(row, 2).addClass('MergeTable_Cell MergeTable_LeftCell_' + evenodd + ' MergeTable_PropLeftCell MergeTable_Header').text(cswPrivate.node1.nodename);
                            table.cell(row, 3).addClass('MergeTable_Cell MergeTable_LeftCell_' + evenodd + ' MergeTable_RadioLeftCell MergeTable_Header');
                            table.cell(row, 4).addClass('MergeTable_Cell MergeTable_RightCell_' + evenodd + ' MergeTable_RadioRightCell MergeTable_Header');
                            table.cell(row, 5).addClass('MergeTable_Cell MergeTable_RightCell_' + evenodd + ' MergeTable_PropRightCell MergeTable_Header').text(cswPrivate.node2.nodename);
                            evenodd === "Even" ? evenodd = "Odd" : evenodd = "Even";
                            row++;
                            
                            // Property Rows
                            Csw.iterate(data.Properties, function(prop) {

                                cswPrivate.propertychoices[prop.NodeTypePropId] = 1;

                                var cell1 = table.cell(row, 1).addClass('MergeTable_Cell MergeTable_LabelCell_' + evenodd + ' MergeTable_LabelCell').text('Property');
                                var cell2 = table.cell(row, 2).addClass('MergeTable_Cell MergeTable_LeftCell_' + evenodd + ' MergeTable_PropLeftCell');
                                var cell3 = table.cell(row, 3).addClass('MergeTable_Cell MergeTable_LeftCell_' + evenodd + ' MergeTable_RadioLeftCell');
                                var cell4 = table.cell(row, 4).addClass('MergeTable_Cell MergeTable_RightCell_' + evenodd + ' MergeTable_RadioRightCell');
                                var cell5 = table.cell(row, 5).addClass('MergeTable_Cell MergeTable_RightCell_' + evenodd + ' MergeTable_PropRightCell');
                            
                                cell1.text(prop.PropName);
                                cell2.text(prop.Node1Value);
                                cell5.text(prop.Node2Value);

                                cell3.input({
                                    name: prop.NodeTypePropId,
                                    type: Csw.enums.inputTypes.radio,
                                    onClick: function() {
                                        prop.Choice = 1;
                                    },
                                    checked: true
                                });
                                cell4.input({
                                    name: prop.NodeTypePropId,
                                    type: Csw.enums.inputTypes.radio,
                                    onClick: function() {
                                        prop.Choice = 2;
                                    },
                                    checked: false
                                });

                                evenodd === "Even" ? evenodd = "Odd" : evenodd = "Even";
                                row++;
                            });
                        } // success()
                    }); // ajax
                };
            }()) // makeStep func
        }; // wizardStepPerformMerge
        

        cswPrivate.wizardStepPreviewResult = {
            stepName: 'Preview Result',
            stepNo: '',
            makeStep: (function () {
                return function(StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);
                    cswPrivate.setStepHeader(StepNo, 'Preview results of merging ' + cswPrivate.node1.nodename + ' with ' + cswPrivate.node2.nodename + '.');

                    var div = cswPrivate['divStep' + StepNo];

                    Csw.ajaxWcf.post({
                        urlMethod: 'Nodes/applyMergeChoices',
                        data: {
                            NodeId1: cswPrivate.node1.nodeid,
                            NodeId2: cswPrivate.node2.nodeid,
                            Choices: cswPrivate.mergeData
                        },
                        success: function(data) {
                            Csw.layouts.tabsAndProps(div, {
                                forceReadOnly: true,
                                tabState: {
                                    nodeid: data,
                                    ReadOnly: true,
                                    EditMode: Csw.enums.editMode.Edit
                                }
                            });
                        } // success()
                    }); // ajax
                };
            }()) // makeStep func
        }; // wizardStepPreviewResult
        
        //#endregion Steps

        //#region Finish
        cswPrivate.finalize = function () {
            cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

        };
        //#endregion Finish

        //#region ctor _post
        (function _post() {
            var wizardSteps = cswPrivate.setWizardSteps();
            cswPrivate.currentStepNo = cswPrivate.startingStep;

            cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                Title: 'Merge',
                StepCount: cswPrivate.stepCount,
                Steps: wizardSteps,
                StartingStep: cswPrivate.startingStep,
                FinishText: 'Finish',
                onNext: cswPrivate.handleStep,
                onPrevious: cswPrivate.handleStep,
                onCancel: cswPrivate.onCancel,
                onFinish: cswPrivate.finalize,
                doNextOnInit: false
            });
            cswPrivate.stepFunc[1](1);
        }());
        //#endregion ctor _post

        return cswPublic;
    });
}());