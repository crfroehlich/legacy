/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.nbt.vieweditor = Csw.nbt.vieweditor ||
        Csw.nbt.register('vieweditor', function (cswParent, options) {
            'use strict';

            //#region Properties

            var cswPrivate = {
                name: 'cswViewEditor',
                exitFunc: null,
                startingStep: 1,
                wizard: null,
                wizardSteps: {
                    1: 'Choose a View',
                    2: 'Build a View',
                    3: 'Add to View',
                    4: 'Set Filters',
                    5: 'View Attributes',
                    6: 'Fine Tuning (Advanced)'
                },
                stepCount: 6,
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                stepOneComplete: false,
                stepTwoComplete: false,
                stepThreeComplete: false,
                stepFourComplete: false,
                stepFiveComplete: false,
                stepSixComplete: false,
                selectedViewId: '',

                onFinish: function () { },
                onCancel: function () { },
                onDeleteView: function () { }
            };

            var cswPublic = {};

            //#endregion Properties

            //#region Wizard Functions

            cswPrivate.reinitSteps = function (startWithStep) {
                if (startWithStep === 2) {
                    cswPrivate.stepThreeComplete = false;
                }

                if (startWithStep === 1) {
                    cswPrivate.stepTwoComplete = false;
                }
            };

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

            cswPrivate.handleStep = function (newStepNo) {
                if (1 === newStepNo) {
                    cswPrivate.makeStep1();
                } else if (2 === newStepNo) {
                    cswPrivate.makeStep2();
                } else if (3 === newStepNo) {
                    cswPrivate.makeStep3();
                } else if (4 === newStepNo) {
                    cswPrivate.makeStep4();
                } else if (5 === newStepNo) {
                    cswPrivate.makeStep5();
                } else if (6 === newStepNo) {
                    cswPrivate.makeStep6();
                }
            };

            //#endregion Wizard Functions

            cswPrivate.makeStep1 = (function () {
                return function () {
                    cswPrivate.View = null;
                    cswPrivate.currentStepNo = 1;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    if (false == Csw.isNullOrEmpty(cswPrivate.selectedViewId)) {
                        cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                    } else {
                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    }

                    cswPrivate.step1Div = cswPrivate.step1Div || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.step1Div.empty();

                    cswPrivate.step1Div.span({
                        text: "A View controls the arrangement of information you see in a tree or grid. " +
                        "Views are useful for defining a user's workflow or for creating elaborate search criteria. " +
                            "This wizard will take you step by step through the process of creating a new View or " +
                                "editing an existing View.",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step1Div.br({ number: 2 });

                    cswPrivate.viewsDiv = cswPrivate.step1Div.div();
                    cswPrivate.buttonsTbl = cswPrivate.step1Div.table({
                        cellspacing: 2,
                        cellpadding: 2
                    });
                    cswPrivate.copyViewBtn = cswPrivate.buttonsTbl.cell(1, 1).buttonExt({
                        name: 'vieweditor_step1_copyviewbtn',
                        enabledText: 'Copy View',
                        onClick: function () {
                            Csw.ajax.post({
                                urlMethod: 'copyView',
                                data: {
                                    ViewId: cswPrivate.selectedViewId,
                                    CopyToViewId: ''
                                },
                                success: function (gridJson) {
                                    cswPrivate.selectedViewId = gridJson.copyviewid;
                                    makeViewsGrid(cswPrivate.showAllChkBox.checked());
                                },
                                error: function () {
                                    cswPrivate.copyViewBtn.enable();
                                }
                            });
                        }
                    });
                    cswPrivate.deleteViewBtn = cswPrivate.buttonsTbl.cell(1, 2).buttonExt({
                        name: 'vieweditor_step1_deleteviewbtn',
                        enabledText: 'Delete View',
                        onClick: function () {
                            Csw.ajax.post({
                                urlMethod: 'deleteView',
                                data: {
                                    ViewId: cswPrivate.selectedViewId
                                },
                                success: function () {
                                    makeViewsGrid(cswPrivate.showAllChkBox.checked());
                                    cswPrivate.copyViewBtn.disable();
                                    cswPrivate.deleteViewBtn.disable();
                                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                                    Csw.tryExec(cswPrivate.onDeleteView, cswPrivate.selectedViewId);
                                },
                                error: function () {
                                    cswPrivate.deleteViewBtn.enable();
                                    cswPrivate.copyViewBtn.enable();
                                }
                            });
                        }
                    });
                    cswPrivate.buttonsTbl.cell(1, 3).buttonExt({
                        name: 'vieweditor_step1_createviewbtn',
                        enabledText: 'Create New View',
                        onClick: function () {
                            $.CswDialog('AddViewDialog', {
                                onAddView: function (newViewId, viewMode) {
                                    cswPrivate.selectedViewId = newViewId;
                                    makeViewsGrid(cswPrivate.showAllChkBox.checked());
                                }
                            });
                        }
                    });
                    cswPrivate.showAllDiv = cswPrivate.step1Div.div().css({
                        'float': 'right'
                    });
                    cswPrivate.showAllChkBox = cswPrivate.showAllDiv.input({
                        name: 'vieweditor_step1_showallchkbox',
                        type: Csw.enums.inputTypes.checkbox,
                        labelText: 'Show All Roles/Users',
                        canCheck: true,
                        onClick: function () {
                            makeViewsGrid(cswPrivate.showAllChkBox.checked());
                        }
                    });

                    var makeViewsGrid = function (all) {
                        Csw.ajax.post({
                            urlMethod: 'getViewGrid',
                            data: {
                                All: all,
                                SelectedViewId: cswPrivate.selectedViewId
                            },
                            success: function (gridData) {
                                cswPrivate.viewsDiv.empty();
                                cswPrivate.viewsDiv.grid({
                                    name: 'vieweditor_grid',
                                    storeId: 'vieweditor_store',
                                    title: '',
                                    stateId: 'vieweditor_gridstate',
                                    usePaging: false,
                                    showActionColumn: false,
                                    height: 225,
                                    fields: gridData.grid.fields,
                                    columns: gridData.grid.columns,
                                    data: gridData.grid.data,
                                    pageSize: gridData.grid.pageSize,
                                    canSelectRow: true,
                                    onSelect: function (row) {
                                        cswPrivate.selectedViewId = row.viewid;
                                        cswPrivate.deleteViewBtn.enable();
                                        cswPrivate.copyViewBtn.enable();
                                        cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                                    },
                                    onDeselect: function (row) {
                                        cswPrivate.selectedViewId = '';
                                        cswPrivate.deleteViewBtn.disable();
                                        cswPrivate.copyViewBtn.disable();
                                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                                    },
                                    onLoad: function (grid) {
                                        if (false === Csw.isNullOrEmpty(cswPrivate.selectedViewId)) {
                                            var rowid = grid.getRowIdForVal('viewid', cswPrivate.selectedViewId);
                                            grid.setSelection(rowid);
                                            grid.scrollToRow(rowid);
                                        } else {
                                            cswPrivate.deleteViewBtn.disable();
                                            cswPrivate.copyViewBtn.disable();
                                        }
                                    }
                                });
                            }
                        });
                    };
                    makeViewsGrid(false);
                };
            }());

            cswPrivate.makeStep2 = (function () {
                return function () {
                    cswPrivate.currentStepNo = 2;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    cswPrivate.step2Div = cswPrivate.step2Div || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.step2Div.css({
                        'width': '100%'
                    });
                    cswPrivate.step2Div.empty();

                    cswPrivate.step2Div.span({
                        text: "What do you want in your View?",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step2Div.br({ number: 3 });

                    cswPrivate.step2Tbl = cswPrivate.step2Div.table({
                        cellpadding: 1,
                        cellspacing: 1
                    });

                    var propsCell = cswPrivate.step2Tbl.cell(1, 1).css({
                        'width': '40%'
                    });
                    cswPrivate.propsScrollable = propsCell.div().css({
                        'overflow': 'auto'
                    });
                    cswPrivate.propsDiv = cswPrivate.propsScrollable.div().css({
                        height: '270px'
                    });
                    var previewCell = cswPrivate.step2Tbl.cell(1, 2).css({
                        'padding-left': '50px',
                        'border-left': '1px solid #A7D3FF'
                    });
                    cswPrivate.previewDiv = previewCell.div();

                    var getStep2Data = function () {
                        Csw.ajaxWcf.post({
                            urlMethod: 'ViewEditor/HandleStep',
                            data: {
                                ViewId: cswPrivate.selectedViewId,
                                StepNo: cswPrivate.currentStepNo,
                                CurrentView: cswPrivate.View
                            },
                            success: function (response) {
                                cswPrivate.View = response.CurrentView;

                                cswPrivate.propsDiv.empty();

                                cswPrivate.selectTbl = cswPrivate.propsDiv.table({
                                    cellspacing: 5
                                });
                                cswPrivate.relSelect = cswPrivate.selectTbl.cell(1, 1).select({
                                    name: 'vieweditor_step2_relationshipselect',
                                    onChange: function () {
                                        if (cswPrivate.relSelect.selectedText() !== 'Select...') {
                                            var selected = cswPrivate.relSelect.selectedVal();
                                            cswPrivate.relationships[selected].Checked = true;
                                            cswPrivate.View.Root.ChildRelationships.push(cswPrivate.relationships[selected].Relationship);
                                            cswPrivate.makeCells();

                                            cswPrivate.relSelect.removeOption('Select...');
                                            cswPrivate.relSelect.addOption({ value: 'Select...', display: 'Select...' }, true);

                                            cswPrivate.relSelect.removeOption(cswPrivate.relationships[selected].Relationship.ArbitraryId);
                                            cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                                        }
                                    }
                                });
                                cswPrivate.propsTbl = cswPrivate.propsDiv.table({
                                    cellspacing: 5,
                                    cellpadding: 5
                                });
                                cswPrivate.relationships = {};
                                var selectOpts = [];
                                Csw.each(response.Step2.Relationships, function (ViewRel) {
                                    var newOpt = {
                                        display: ViewRel.Relationship.TextLabel,
                                        value: ViewRel.Relationship.ArbitraryId
                                    };
                                    selectOpts.push(newOpt);
                                    cswPrivate.relationships[ViewRel.Relationship.ArbitraryId] = ViewRel;
                                });
                                cswPrivate.relSelect.setOptions(selectOpts, true);
                                cswPrivate.relSelect.addOption({ value: 'Select...', display: 'Select...' }, true);
                                cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);

                                cswPrivate.makeCells = function () {
                                    var row = 1;
                                    cswPrivate.propsTbl.empty();
                                    Csw.iterate(cswPrivate.relationships, function (thisRel) {
                                        if (thisRel.Checked) {
                                            cswPrivate.propsTbl.cell(row, 1).icon({
                                                hovertext: 'Remove this from view',
                                                isButton: true,
                                                iconType: Csw.enums.iconType.x,
                                                onClick: function () {
                                                    cswPrivate.relationships[thisRel.Relationship.ArbitraryId].Checked = false;
                                                    var cleansedRelationships = [];
                                                    Csw.iterate(cswPrivate.View.Root.ChildRelationships, function (childRel) {
                                                        if (childRel.ArbitraryId !== thisRel.Relationship.ArbitraryId) {
                                                            cleansedRelationships.push(childRel);
                                                        }
                                                    });
                                                    cswPrivate.View.Root.ChildRelationships = cleansedRelationships;
                                                    cswPrivate.makeCells();
                                                    cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                                                    var newOpt = {
                                                        display: thisRel.Relationship.TextLabel,
                                                        value: thisRel.Relationship.ArbitraryId
                                                    };
                                                    cswPrivate.relSelect.addOption(newOpt, false);
                                                }
                                            });
                                            cswPrivate.propsTbl.cell(row, 2).text(thisRel.Relationship.TextLabel);
                                            row++;
                                        }
                                    });
                                };
                                cswPrivate.makeCells();

                            }
                        });
                    };
                    getStep2Data();
                };
            }());

            cswPrivate.makeStep3 = (function () {
                return function () {
                    cswPrivate.currentStepNo = 3;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    cswPrivate.step3Div = cswPrivate.step3Div || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.step3Div.css({
                        'width': '100%'
                    });
                    cswPrivate.step3Div.empty();

                    var txt = '';
                    if (cswPrivate.View.ViewMode === 'Grid') {
                        txt = 'What columns do you want in your Grid?';
                    } else {
                        txt = "What else do you want in your Tree?";
                    }
                    cswPrivate.step3Div.span({
                        text: txt,
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step3Div.br({ number: 3 });

                    cswPrivate.step3Tbl = cswPrivate.step3Div.table({
                        cellpadding: 1,
                        cellspacing: 1
                    });

                    var propsCell = cswPrivate.step3Tbl.cell(1, 1).css({
                        'width': '40%'
                    });
                    cswPrivate.propsScrollable = propsCell.css({
                        'overflow': 'auto'
                    });
                    cswPrivate.propsDiv = cswPrivate.propsScrollable.div().css({
                        'height': '270px'
                    });
                    var previewCell = cswPrivate.step3Tbl.cell(1, 2).css({
                        'padding-left': '40px',
                        'border-left': '1px solid #A7D3FF'
                    });
                    cswPrivate.previewDiv = previewCell.div();

                    var getStep3Data = function () {
                        Csw.ajaxWcf.post({
                            urlMethod: 'ViewEditor/HandleStep',
                            data: {
                                CurrentView: cswPrivate.View,
                                StepNo: cswPrivate.currentStepNo
                            },
                            success: function (response) {
                                cswPrivate.View = response.CurrentView;
                                cswPrivate.propsDiv.br({ number: 2 });

                                if ('Grid' === cswPrivate.View.ViewMode) {
                                    cswPrivate.propsDiv.empty();

                                    cswPrivate.propSelect = cswPrivate.propsDiv.select({
                                        name: 'vieweditor_step3_propselect',
                                        onChange: function () {
                                            if (cswPrivate.propSelect.selectedText() !== 'Select...') {
                                                var selectedProp = cswPrivate.properties[cswPrivate.propSelect.selectedVal()];
                                                selectedProp.Checked = true;
                                                cswPrivate.makePropsTbl();

                                                Csw.ajaxWcf.post({
                                                    urlMethod: 'ViewEditor/AddProp',
                                                    data: {
                                                        CurrentView: cswPrivate.View,
                                                        Relationship: cswPrivate.secondRelationships[selectedProp.Property.ParentArbitraryId],
                                                        Property: selectedProp.Property
                                                    },
                                                    success: function (addPropResponse) {
                                                        cswPrivate.View = addPropResponse.CurrentView;
                                                        cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                                                    }
                                                });

                                                cswPrivate.propSelect.removeOption(selectedProp.Property.ArbitraryId);
                                                cswPrivate.propSelect.removeOption('Select...');
                                                cswPrivate.propSelect.addOption({ value: 'Select...', display: 'Select...' }, true);
                                                cswPrivate.makePropsTbl();
                                            }
                                        }
                                    });

                                    var propsTbl = cswPrivate.propsDiv.table({
                                        cellspacing: 3,
                                        cellpadding: 3
                                    });
                                    cswPrivate.propsDiv.br({ number: 2 });

                                    cswPrivate.properties = {};
                                    cswPrivate.selectOpts = [];
                                    cswPrivate.selectOpts.push({ value: 'Select...', display: 'Select...', isSelected: true });
                                    Csw.each(response.Step3.Properties, function (ViewProp) {
                                        cswPrivate.properties[ViewProp.Property.ArbitraryId] = ViewProp;
                                        var newOpt = {
                                            value: ViewProp.Property.ArbitraryId,
                                            display: ViewProp.Property.TextLabel
                                        };
                                        cswPrivate.selectOpts.push(newOpt);
                                    });
                                    cswPrivate.propSelect.setOptions(cswPrivate.selectOpts, true);

                                    cswPrivate.secondRelationships = {};
                                    Csw.each(response.Step3.SecondRelationships, function (secondRel) {
                                        cswPrivate.secondRelationships[secondRel.ArbitraryId] = secondRel;
                                    });

                                    cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);

                                    cswPrivate.makePropsTbl = function () {
                                        var row = 1;
                                        propsTbl.empty();
                                        Csw.iterate(cswPrivate.properties, function (prop) {
                                            if (prop.Checked) {
                                                propsTbl.cell(row, 1).icon({
                                                    hovertext: 'Remove this from view',
                                                    isButton: true,
                                                    iconType: Csw.enums.iconType.x,
                                                    onClick: function () {
                                                        Csw.ajaxWcf.post({
                                                            urlMethod: 'ViewEditor/RemoveProp',
                                                            data: {
                                                                CurrentView: cswPrivate.View,
                                                                Relationship: cswPrivate.secondRelationships[prop.Property.ParentArbitraryId],
                                                                Property: prop.Property
                                                            },
                                                            success: function (removePropResponse) {
                                                                cswPrivate.View = removePropResponse.CurrentView;
                                                                cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                                                            }
                                                        });

                                                        var newOpt = {
                                                            value: prop.Property.ArbitraryId,
                                                            display: prop.Property.TextLabel
                                                        };
                                                        cswPrivate.propSelect.addOption(newOpt, false);
                                                        prop.Checked = false;
                                                        cswPrivate.makePropsTbl();
                                                    }
                                                });
                                                propsTbl.cell(row, 2).text(prop.Property.TextLabel);
                                                var thisOrderInput = propsTbl.cell(row, 3).input({
                                                    //TODO: this MUST be a number or blank
                                                    name: 'vieweditor_step3_orderinput_' + prop.Property.ArbitraryId,
                                                    size: 3,
                                                    onChange: function () {
                                                        //TODO: update props order, update preview
                                                    }
                                                });
                                                row++;
                                            }
                                        });
                                    };
                                    cswPrivate.makePropsTbl();
                                } else if ('Tree' === cswPrivate.View.ViewMode) {
                                    cswPrivate.relationships = response.Step2.Relationships;
                                    var makeRelsTbl = function (thisRelTbl, innerRow, selectedRel, thisSel) {
                                        var thisRow = thisRelTbl.cell(innerRow, 1).icon({
                                            hovertext: 'Remove this from view',
                                            isButton: true,
                                            iconType: Csw.enums.iconType.x,
                                            onClick: function () {
                                                var relToRemoveFrom = cswPrivate.findRelationshipByArbitraryId(selectedRel.ParentArbitraryId);
                                                var newRels = [];
                                                Csw.each(relToRemoveFrom.ChildRelationships, function (childRel) {
                                                    if (childRel.ArbitraryId !== selectedRel.ArbitraryId) {
                                                        newRels.push(childRel);
                                                    }
                                                });
                                                relToRemoveFrom.ChildRelationships = newRels;
                                                thisRow.remove();
                                                thisRowTxt.remove();
                                                thisSel.addOption({ value: selectedRel.ArbitraryId, display: selectedRel.TextLabel });
                                                innerRow--;
                                                cswPrivate.makeStep3();
                                            }
                                        });
                                        var thisRowTxt = thisRelTbl.cell(innerRow, 2).text(selectedRel.TextLabel);
                                    };

                                    cswPrivate.propsDiv.empty();

                                    var opts = {};
                                    var rels = {};
                                    Csw.each(response.Step3.SecondRelationships, function (rel) {
                                        rels[rel.ArbitraryId] = rel;
                                        var viewContains = cswPrivate.findRelationshipByArbitraryId(rel.ArbitraryId);
                                        if (opts[rel.ParentArbitraryId]) {
                                            if (!viewContains) { //we don't care that this coerces type
                                                opts[rel.ParentArbitraryId].push({ display: rel.TextLabel, value: rel.ArbitraryId });
                                            }
                                        } else {
                                            opts[rel.ParentArbitraryId] = [{ display: 'Select...', value: 'Select...' }];
                                            if (!viewContains) { //we don't care this this coerces type
                                                opts[rel.ParentArbitraryId].push({ display: rel.TextLabel, value: rel.ArbitraryId });
                                            }
                                        }
                                    });

                                    Csw.each(cswPrivate.relationships, function (relProp) {
                                        if (relProp.Checked) {
                                            var relDiv = cswPrivate.propsDiv.div();
                                            relDiv.setLabelText('Under ' + relProp.Relationship.TextLabel + '&nbsp;', false, false);
                                            var thisSel = relDiv.select({
                                                name: 'vieweditor_relselect_' + relProp.Relationship.ArbitraryId,
                                                values: opts[relProp.Relationship.ArbitraryId],
                                                onChange: function () {
                                                    if (thisSel.selectedVal() !== 'Select...') {
                                                        var selectedRel = rels[thisSel.selectedVal()];
                                                        var relToAddTo = cswPrivate.findRelationshipByArbitraryId(relProp.Relationship.ArbitraryId);
                                                        relToAddTo.ChildRelationships.push(selectedRel);
                                                        thisSel.removeOption(selectedRel.ArbitraryId);

                                                        thisSel.removeOption('Select...');
                                                        thisSel.addOption({ display: 'Select...', value: 'Select...' }, true);

                                                        cswPrivate.makeStep3();
                                                    }
                                                }
                                            });
                                            cswPrivate.propsDiv.br({ number: 2 });
                                            var thisRelTbl = cswPrivate.propsDiv.div().table({
                                                cellpadding: 3,
                                                cellspacing: 2
                                            });
                                            var row = 1;
                                            var viewRel = cswPrivate.findRelationshipByArbitraryId(relProp.Relationship.ArbitraryId);
                                            Csw.each(viewRel.ChildRelationships, function (childRel) {
                                                makeRelsTbl(thisRelTbl, row, childRel, thisSel);
                                                row++;
                                            });
                                            cswPrivate.propsDiv.br({ number: 2 });
                                        }
                                    });

                                    cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                                }
                            }
                        });
                    };
                    getStep3Data();
                };
            }());

            cswPrivate.makeStep4 = (function () {
                return function () {
                    cswPrivate.currentStepNo = 4;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    cswPrivate.step4Div = cswPrivate.step4Div || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.step4Div.css({
                        'width': '100%'
                    });
                    cswPrivate.step4Div.empty();

                    cswPrivate.step4Div.span({
                        text: "How do you want to filter your results?",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step4Div.br({ number: 3 });

                    cswPrivate.step4Tbl = cswPrivate.step4Div.table({
                        cellpadding: 1,
                        cellspacing: 1
                    });

                    cswPrivate.propsCell = cswPrivate.step4Tbl.cell(1, 1).css({
                        'width': '40%'
                    });

                    cswPrivate.propsCell.br({ number: 2 });
                    cswPrivate.propsScrollable = cswPrivate.propsCell.div().css({
                        'overflow': 'auto'
                    });
                    cswPrivate.filtersDiv = cswPrivate.propsScrollable.div().css({
                        height: '230px'
                    });
                    cswPrivate.filtersTbl = cswPrivate.filtersDiv.table({
                        cellpadding: 4,
                        cellspacing: 4
                    });
                    var previewCell = cswPrivate.step4Tbl.cell(1, 2).css({
                        'padding-left': '40px',
                        'border-left': '1px solid #A7D3FF'
                    });
                    cswPrivate.previewDiv = previewCell.div();

                    cswPrivate.relationships = {};
                    var getStep4Data = function () {
                        Csw.ajaxWcf.post({
                            urlMethod: 'ViewEditor/HandleStep',
                            data: {
                                CurrentView: cswPrivate.View,
                                StepNo: cswPrivate.currentStepNo
                            },
                            success: function (response) {
                                handleStep4Data(response);
                            }
                        });
                    };

                    var handleStep4Data = function (response) {
                        if (cswPrivate.filterSelectDiv) {
                            cswPrivate.filterSelectDiv.remove();
                        }
                        var selectOpts = [];
                        cswPrivate.View = response.CurrentView;
                        cswPrivate.ViewJson = response.Step4.ViewJson;

                        var row = 1;
                        cswPrivate.filtersTbl.empty();
                        Csw.iterate(response.Step4.Filters, function (filter) {
                            cswPrivate.filtersTbl.cell(row, 1).icon({
                                hovertext: 'Remove filter',
                                isButton: true,
                                iconType: Csw.enums.iconType.x,
                                onClick: function () {
                                    Csw.ajaxWcf.post({
                                        urlMethod: 'ViewEditor/RemoveFilter',
                                        data: {
                                            FilterToRemove: filter,
                                            CurrentView: cswPrivate.View
                                        },
                                        success: function (removeFilterResponse) {
                                            handleStep4Data(removeFilterResponse);
                                        }
                                    });
                                }
                            });
                            Csw.nbt.viewPropFilter({
                                name: 'vieweditor_filter_' + filter.ArbitraryId,
                                parent: cswPrivate.filtersTbl,
                                viewId: cswPrivate.View.ViewId,
                                viewJson: response.Step4.ViewJson,
                                proparbitraryid: filter.ParentArbitraryId,
                                propname: filter.PropName,
                                selectedConjunction: filter.Conjunction,
                                selectedSubFieldName: filter.SubfieldName,
                                selectedFilterMode: filter.FilterMode,
                                selectedValue: filter.Value,
                                doStringify: false,
                                readOnly: true,
                                propRow: row,
                                firstColumn: 2
                            });
                            row++;
                        });

                        cswPrivate.filterSelectDiv = cswPrivate.filtersDiv.div();
                        cswPrivate.filterSelect = cswPrivate.filterSelectDiv.select({
                            name: 'vieweditor_filter_relSelect',
                            onChange: function () {
                                if (cswPrivate.propSelect) {
                                    cswPrivate.propSelect.remove();
                                    if (cswPrivate.propFilterTbl) {
                                        cswPrivate.propFilterTbl.remove();
                                        cswPrivate.addFilterBtn.remove();
                                    }
                                }
                                if (cswPrivate.filterSelect.selectedText() !== 'Add Filter On...') {
                                    cswPrivate.propSelect = cswPrivate.filterSelectDiv.select({
                                        name: 'vieweditor_propfilter_select',
                                        onChange: function () {
                                            if (cswPrivate.propFilterTbl) {
                                                cswPrivate.propFilterTbl.remove();
                                                cswPrivate.addFilterBtn.remove();
                                            }
                                            if (cswPrivate.propSelect.selectedText() !== 'Select...') {
                                                cswPrivate.propFilterTbl = cswPrivate.filterSelectDiv.table();
                                                var selectedProp = properties[cswPrivate.propSelect.selectedVal()];

                                                var currentFilter = Csw.nbt.viewPropFilter({
                                                    name: 'vieweditor_filter_' + selectedProp.ArbitraryId,
                                                    parent: cswPrivate.propFilterTbl,
                                                    viewJson: cswPrivate.viewJson,
                                                    proparbitraryid: selectedProp.ArbitraryId,
                                                    propname: selectedProp.PropName,
                                                    showPropertyName: false,
                                                    showOwnerName: false,
                                                    doStringify: false
                                                });

                                                cswPrivate.addFilterBtn = cswPrivate.filterSelectDiv.buttonExt({
                                                    name: 'vieweditor_applyfilter_btn',
                                                    enabledText: 'Apply Filter',
                                                    onClick: function () {
                                                        var filterData = currentFilter.getFilterJson();
                                                        var ajaxData = {
                                                            CurrentView: cswPrivate.View,
                                                            Property: selectedProp,
                                                            PropArbId: filterData.proparbitraryid,
                                                            FilterSubfield: filterData.subfieldname,
                                                            FilterValue: filterData.filtervalue,
                                                            FilterMode: filterData.filter,
                                                            FilterConjunction: filterData.conjunction
                                                        };
                                                        Csw.ajaxWcf.post({
                                                            urlMethod: 'ViewEditor/AddFilter',
                                                            data: ajaxData,
                                                            success: function (addFilterResponse) {
                                                                handleStep4Data(addFilterResponse);
                                                            }
                                                        });
                                                    }
                                                });
                                            }
                                        }
                                    });

                                    var propOpts = [];
                                    var properties = {};
                                    Csw.ajaxWcf.post({
                                        urlMethod: 'ViewEditor/GetFilterProps',
                                        data: {
                                            Relationship: cswPrivate.relationships[cswPrivate.filterSelect.selectedVal()],
                                            CurrentView: cswPrivate.View
                                        },
                                        success: function (filterPropsresponse) {
                                            cswPrivate.viewJson = filterPropsresponse.Step4.ViewJson;
                                            Csw.iterate(filterPropsresponse.Step4.Properties, function (prop) {
                                                properties[prop.ArbitraryId] = prop;
                                                var newOpt = {
                                                    value: prop.ArbitraryId,
                                                    display: prop.TextLabel
                                                };
                                                propOpts.push(newOpt);
                                            });
                                            cswPrivate.propSelect.setOptions(propOpts, true);
                                            cswPrivate.propSelect.addOption({ display: 'Select...', value: 'Select...' }, true);
                                        }
                                    });
                                }
                            }
                        });

                        selectOpts.push({ display: 'Add Filter On...', value: 'Add Filter On...', isSelected: true });
                        Csw.iterate(response.Step4.Relationships, function (relationship) {
                            cswPrivate.relationships[relationship.ArbitraryId] = relationship;
                            var newOpt = {
                                value: relationship.ArbitraryId,
                                display: relationship.TextLabel
                            };
                            selectOpts.push(newOpt);
                        });
                        cswPrivate.filterSelect.setOptions(selectOpts, false);

                        cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                    };

                    getStep4Data();

                };
            }());

            cswPrivate.makeStep5 = (function () {
                return function () {
                    cswPrivate.currentStepNo = 5;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    cswPrivate.step5Div = cswPrivate.step5Div || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.step5Div.css({
                        'width': '100%'
                    });
                    cswPrivate.step5Div.empty();

                    cswPrivate.step5Div.span({
                        text: "Set who can access this view and where it can be found in the View Selector.",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step5Div.br({ number: 3 });

                    cswPrivate.step5Tbl = cswPrivate.step5Div.table({
                        cellpadding: 1,
                        cellspacing: 1
                    });

                    var attrCell = cswPrivate.step5Tbl.cell(1, 1).css({
                        'width': '40%'
                    });
                    cswPrivate.attributesTbl = attrCell.div().table({
                        cellpadding: 5,
                        cellspacing: 5
                    });

                    cswPrivate.attributesTbl.cell(1, 1).setLabelText('View Name', false, false);
                    cswPrivate.ViewNameInput = cswPrivate.attributesTbl.cell(1, 2).input({
                        name: 'vieweditor_viewname_input',
                        value: cswPrivate.View.ViewName,
                        onChange: function () {
                            handleAttributeChange();
                        }
                    });

                    cswPrivate.attributesTbl.cell(2, 1).setLabelText('Category', false, false);
                    cswPrivate.CategoryInput = cswPrivate.attributesTbl.cell(2, 2).input({
                        name: 'vieweditor_category_input',
                        value: cswPrivate.View.Category,
                        onChange: function () {
                            handleAttributeChange();
                        }
                    });

                    cswPrivate.attributesTbl.cell(3, 1).setLabelText('View Visibility', false, false);
                    cswPrivate.visibilitySelect = Csw.composites.makeViewVisibilitySelect(cswPrivate.attributesTbl, 3, '', {
                        visibility: cswPrivate.View.Visibility,
                        roleid: cswPrivate.View.VisibilityRoleId,
                        userid: cswPrivate.View.VisibilityUserId,
                        onChange: function () {
                            handleAttributeChange();
                        }
                    });

                    cswPrivate.attributesTbl.cell(4, 1).setLabelText('Display Mode', false, false);
                    cswPrivate.attributesTbl.cell(4, 2).text(cswPrivate.View.ViewMode);

                    cswPrivate.attributesTbl.cell(5, 1).setLabelText('Width', false, false);
                    cswPrivate.WidthInput = cswPrivate.attributesTbl.cell(5, 2).input({
                        name: 'vieweditor_width_input',
                        value: cswPrivate.View.Width,
                        onChange: function () {
                            handleAttributeChange();
                        }
                    });

                    var handleAttributeChange = function () {
                        //It's better to send this to the server to modify - in some cases (ex: ViewName) we need DB resources which are not available during the "blackbox" deserialization events
                        var visibilityData = cswPrivate.visibilitySelect.getSelected();
                        Csw.ajaxWcf.post({
                            urlMethod: 'ViewEditor/UpdateViewAttributes',
                            data: {
                                NewViewName: cswPrivate.ViewNameInput.val(),
                                NewViewCategory: cswPrivate.CategoryInput.val(),
                                NewViewWidth: cswPrivate.WidthInput.val(),
                                NewViewVisibility: visibilityData.visibility,
                                NewVisibilityRoleId: visibilityData.roleid,
                                NewVisbilityUserId: visibilityData.userid,
                                CurrentView: cswPrivate.View
                            },
                            success: function (response) {
                                cswPrivate.View = response.CurrentView;
                            }
                        });
                    };

                    var previewCell = cswPrivate.step5Tbl.cell(1, 2).css({
                        'padding-left': '40px',
                        'border-left': '1px solid #A7D3FF'
                    });
                    cswPrivate.previewDiv = previewCell.div();
                    cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);

                };
            }());

            cswPrivate.makeStep6 = (function () {
                return function () {
                    cswPrivate.currentStepNo = 6;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);

                    cswPrivate.step6Div = cswPrivate.step6Div || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.step6Div.css({
                        'width': '100%'
                    });
                    cswPrivate.step6Div.empty();

                    cswPrivate.step6Div.span({
                        text: "Edit or add any property or relationship attributes in your view.",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step6Div.br({ number: 3 });

                    cswPrivate.step6Tbl = cswPrivate.step6Div.table({
                        cellpadding: 1,
                        cellspacing: 1
                    });

                    var contentCell = cswPrivate.step6Tbl.cell(1, 1).css({
                        'width': '40%'
                    });
                    var viewContentDiv = contentCell.div();
                    var previewCell = cswPrivate.step6Tbl.cell(1, 2).css({
                        'padding-left': '40px',
                        'border-left': '1px solid #A7D3FF'
                    });
                    var previewDiv = previewCell.div();

                    Csw.ajaxWcf.post({
                        urlMethod: 'ViewEditor/HandleStep',
                        data: {
                            StepNo: 4, //intentionally recycle method
                            CurrentView: cswPrivate.View
                        },
                        success: function (response) {
                            viewContentDiv.$.CswViewContentTree({
                                viewstr: response.Step4.ViewJson,
                                onClick: function (node, ref_node) {
                                    onNodeClick(ref_node.rslt.obj[0].id);
                                }
                            });
                        }
                    });

                    cswPrivate.buildPreview(previewDiv, cswPrivate.View);

                    var onNodeClick = function (arbitraryId) {
                        Csw.ajaxWcf.post({
                            urlMethod: 'ViewEditor/HandleNodeClick',
                            data: {
                                ArbitraryId: arbitraryId,
                                CurrentView: cswPrivate.View
                            },
                            success: function (response) {

                                if (false === Csw.isNullOrEmpty(response.Step6.FilterNode)) {
                                    $.CswDialog('ViewEditorFilterEdit', {
                                        filterNode: response.Step6.FilterNode,
                                        view: cswPrivate.View,
                                        onFilterEdit: function (updatedView) {
                                            cswPrivate.View = updatedView;
                                            cswPrivate.buildPreview(previewDiv, cswPrivate.View);
                                        }
                                    });
                                } else if (false === Csw.isNullOrEmpty(response.Step6.RelationshipNode)) {
                                    $.CswDialog('ViewEditorRelationshipEdit', {
                                        relationshipNode: response.Step6.RelationshipNode,
                                        view: cswPrivate.View,
                                        findRelationshipByArbitraryId: cswPrivate.findRelationshipByArbitraryId,
                                        onRelationshipEdit: function (updatedView) {
                                            cswPrivate.View = updatedView;
                                            cswPrivate.buildPreview(previewDiv, cswPrivate.View);
                                        }
                                    });
                                }
                            }
                        });
                    };

                };
            }());

            cswPrivate.buildPreview = function (previewDiv, view) {
                Csw.ajaxWcf.post({
                    urlMethod: 'ViewEditor/GetPreview',
                    data: {
                        CurrentView: view
                    },
                    success: function (response) {
                        previewDiv.empty();
                        previewDiv.setLabelText('Preview: ', false, false);
                        previewDiv.br({ number: 2 });
                        var previewData = JSON.parse(response.Preview);
                        if (cswPrivate.View.ViewMode === 'Grid') {
                            previewDiv.grid({
                                name: 'vieweditor_previewgrid',
                                storeId: 'vieweditor_store',
                                title: '',
                                stateId: 'vieweditor_gridstate',
                                usePaging: false,
                                showActionColumn: false,
                                height: 210,
                                width: 700,
                                fields: previewData.grid.fields,
                                columns: previewData.grid.columns,
                                data: previewData.grid.data,
                                pageSize: previewData.grid.pageSize,
                                canSelectRow: false
                            });
                        } else if (cswPrivate.View.ViewMode === 'Tree') {
                            var tree = Csw.nbt.nodeTree({
                                name: 'vieweditor_treepreview',
                                height: '250px',
                                width: '700px',
                                parent: previewDiv
                            });
                            tree.makeTree(previewData);
                            tree.expandAll();
                        }
                    }
                });
            };

            cswPrivate.findRelationshipByArbitraryId = function (arbitraryId) {
                var ret = null;
                var recurse = function (relationship) {
                    var innerRet = null;
                    if (relationship.ArbitraryId === arbitraryId) {
                        innerRet = relationship;
                    } else {
                        Csw.each(relationship.ChildRelationships, function (childRel) {
                            var found = recurse(childRel);
                            if (found) {
                                innerRet = found;
                            }
                        });
                    }
                    return innerRet;
                };
                ret = recurse(cswPrivate.View.Root);
                return ret;
            };

            //#region ctor

            (function () {
                Csw.extend(cswPrivate, options, true);
                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.finalize = function () {
                    Csw.ajaxWcf.post({
                        urlMethod: 'ViewEditor/Finalize',
                        data: {
                            CurrentView: cswPrivate.View
                        },
                        success: function (response) {
                            cswPrivate.View = response.CurrentView;
                            cswPrivate.onFinish(cswPrivate.View.ViewId, cswPrivate.View.ViewMode);
                        }
                    });
                };

                cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                    Title: 'View Editor',
                    StepCount: cswPrivate.stepCount,
                    Steps: cswPrivate.wizardSteps,
                    StartingStep: cswPrivate.startingStep,
                    FinishText: 'Finish',
                    onNext: cswPrivate.handleStep,
                    onPrevious: cswPrivate.handleStep,
                    onCancel: function () {
                        Csw.tryExec(cswPrivate.onCancel);
                    },
                    onFinish: cswPrivate.finalize,
                    doNextOnInit: false
                });

                cswPrivate.makeStep1();

            }());

            //#endregion ctor



            return cswPublic;
        });
}());