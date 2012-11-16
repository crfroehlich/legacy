/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    var cswReconciliationWizardStateName = 'cswReconciliationWizardStateName';

    Csw.nbt.ReconciliationWizard = Csw.nbt.ReconciliationWizard ||
        Csw.nbt.register('ReconciliationWizard', function (cswParent, options) {
            'use strict';

            var cswPrivate = {
                name: 'cswReconciliationWizard',
                exitFunc: null,
                numOfSteps: 3,
                startingStep: 1,
                wizard: null,
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                divStep1: '',
                divStep2: '',
                divStep3: '',
                stepOneComplete: false,
                stepTwoComplete: false,
                stepThreeComplete: false,
                state: {
                    LocationId: '',
                    LocationName: '',
                    IncludeChildLocations: false,
                    StartDate: '',
                    EndDate: ''
                }
            };

            var cswPublic = {};

            cswPrivate.reinitSteps = function (startWithStep) {
                cswPrivate.stepThreeComplete = false;
                if (startWithStep <= 2) {
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

            cswPrivate.toggleStepButtons = function () {
                cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                cswPrivate.toggleButton(cswPrivate.buttons.prev, Csw.bool(cswPrivate.currentStepNo > 1));
                cswPrivate.toggleButton(cswPrivate.buttons.finish, Csw.bool(cswPrivate.currentStepNo === cswPrivate.numOfSteps));
                cswPrivate.toggleButton(cswPrivate.buttons.next, Csw.bool(cswPrivate.currentStepNo < cswPrivate.numOfSteps));
            };

            //State Functions
            cswPrivate.validateState = function () {
                var state;
                if (Csw.isNullOrEmpty(cswPrivate.state.locationId)) {
                    state = cswPrivate.getState();
                    Csw.extend(cswPrivate.state, state);
                }
                cswPrivate.setState();
            };

            cswPrivate.getState = function () {
                return Csw.clientDb.getItem(cswPrivate.name + '_' + cswReconciliationWizardStateName);
            };

            cswPrivate.setState = function () {
                Csw.clientDb.setItem(cswPrivate.name + '_' + cswReconciliationWizardStateName, cswPrivate.state);
            };

            cswPrivate.clearState = function () {
                Csw.clientDb.removeItem(cswPrivate.name + '_' + cswReconciliationWizardStateName);
            };

            //Step 1: Locations and Dates
            cswPrivate.makeStep1 = (function () {
                cswPrivate.stepOneComplete = false;
                return function () {
                    cswPrivate.toggleStepButtons();

                    if (false === cswPrivate.stepOneComplete) {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();

                        cswPrivate.divStep1.label({
                            text: "Choose the location of the Containers you wish to examine, and select a timeline with which to filter Reconciliation Scans.",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep1.br({ number: 2 });
                        //Props Table
                        var locationDatesTable = cswPrivate.divStep1.table({
                            name: 'setLocationDatesTable',
                            cellpadding: '5px',
                            cellalign: 'left',
                            cellvalign: 'top',
                            FirstCellRightAlign: true
                        });
                        //Location
                        locationDatesTable.cell(1, 1).span({ text: 'Location:' }).addClass('propertylabel');
                        var locationControl = locationDatesTable.cell(1, 2).location({
                            name: '',
                            onChange: function (locationId, locationName) {
                                cswPrivate.state.LocationId = locationId;
                                cswPrivate.state.LocationName = locationName;
                                cswPrivate.reinitSteps(2);
                            }
                        });
                        cswPrivate.state.LocationId = locationControl.val();
                        cswPrivate.state.LocationName = locationControl.selectedName();
                        locationDatesTable.cell(1, 2).br();
                        //IncludeChildLocations
                        var checkBoxTable = locationDatesTable.cell(1, 2).table({
                            name: 'checkboxTable',
                            cellalign: 'left',
                            cellvalign: 'middle'
                        });
                        cswPrivate.childLocationsCheckBox = checkBoxTable.cell(1, 1).checkBox({
                            onChange: Csw.method(function () {
                                cswPrivate.state.IncludeChildLocations = cswPrivate.childLocationsCheckBox.checked();
                                cswPrivate.reinitSteps(2);
                            })
                        });
                        checkBoxTable.cell(1, 2).span({ text: ' Include child locations' });
                        //StartDate
                        locationDatesTable.cell(2, 1).span({ text: 'Start Date:' }).addClass('propertylabel');
                        var startDatePicker = locationDatesTable.cell(2, 2).dateTimePicker({
                            name: 'startDate',
                            Date: cswPrivate.getCurrentDate(),
                            isRequired: true,
                            maxDate: cswPrivate.getCurrentDate(),
                            onChange: function () {
                                cswPrivate.state.StartDate = startDatePicker.val().date;
                                cswPrivate.reinitSteps(2);
                            }
                        });
                        cswPrivate.state.StartDate = startDatePicker.val().date;
                        //EndDate
                        locationDatesTable.cell(3, 1).span({ text: 'End Date:' }).addClass('propertylabel');
                        var endDatePicker = locationDatesTable.cell(3, 2).dateTimePicker({
                            name: 'endDate',
                            Date: cswPrivate.getCurrentDate(),
                            isRequired: true,
                            maxDate: cswPrivate.getCurrentDate(),
                            onChange: function () {
                                cswPrivate.state.EndDate = endDatePicker.val().date;
                                startDatePicker.setMaxDate(cswPrivate.state.EndDate);
                                cswPrivate.reinitSteps(2);
                            }
                        });
                        cswPrivate.state.EndDate = endDatePicker.val().date;

                        cswPrivate.stepOneComplete = true;
                    }
                };
            } ()); //Step 1

            //Step 2: Statistics
            cswPrivate.makeStep2 = (function () {
                cswPrivate.stepTwoComplete = false;
                return function () {                    
                    if (false === cswPrivate.stepTwoComplete) {
                        cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.empty();
                        cswPrivate.divStep2.label({
                            text: "Loading Container Statistics...",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep2.br({ number: 2 });

                        Csw.ajaxWcf.post({
                            urlMethod: 'Containers/getContainerStatistics',
                            data: cswPrivate.state,
                            success: function (ajaxdata) {
                                cswPrivate.data = {
                                    ContainerStatistics: [{
                                        Status: '',
                                        ContainerCount: '',
                                        AmountScanned: '',
                                        PercentScanned: ''
                                    }],
                                    ContainerStatuses: [{
                                        ContainerId: '',
                                        ContainerBarcode: '',
                                        ContainerStatus: '',
                                        Action: '',
                                        ActionApplied: ''
                                    }]
                                };
                                Csw.extend(cswPrivate.data, ajaxdata);

                                var StatusMetricsGridFields = [];
                                var StatusMetricsGridColumns = [];
                                var addColumn = function (colName, displayName) {
                                    StatusMetricsGridColumns.push({
                                        dataIndex: colName,
                                        filterable: false,
                                        format: null,
                                        header: displayName,
                                        id: 'reconciliation_status_' + colName
                                    });
                                    StatusMetricsGridFields.push({
                                        name: colName,
                                        type: 'string',
                                        useNull: true
                                    });
                                };
                                addColumn('status', 'Status');
                                addColumn('numofcontainers', 'Number of Containers');
                                addColumn('percentscanned', 'Percent Scanned');
                                addColumn('percentunscanned', 'Percent Unscanned');

                                var StatusMetricsGridData = [];
                                Csw.each(cswPrivate.data.ContainerStatistics, function (row) {
                                    StatusMetricsGridData.push({
                                        status: row.Status,
                                        numofcontainers: row.ContainerCount,
                                        percentscanned: Csw.number(row.PercentScanned) + '%',
                                        percentunscanned: Csw.number(100 - Csw.number(row.PercentScanned)) + '%'
                                    });
                                });

                                var StatusMetricsGridId = 'ReconciliationStatusGrid';
                                cswPrivate.gridOptions = {
                                    name: StatusMetricsGridId,
                                    storeId: StatusMetricsGridId,
                                    title: 'Container Statistics in ' + cswPrivate.state.LocationName
                                        + ' from ' + cswPrivate.state.StartDate
                                        + ' to ' + cswPrivate.state.EndDate,
                                    stateId: StatusMetricsGridId,
                                    usePaging: false,
                                    showActionColumn: false,
                                    canSelectRow: false,
                                    onLoad: null,
                                    onEdit: null,
                                    onDelete: null,
                                    onSelect: null,
                                    onDeselect: null,
                                    height: 200,
                                    forcefit: true,
                                    width: '100%',
                                    fields: StatusMetricsGridFields,
                                    columns: StatusMetricsGridColumns,
                                    data: StatusMetricsGridData
                                };
                                cswPrivate.ReconciliationStatusGrid = cswPrivate.divStep2.grid(cswPrivate.gridOptions);
                                cswPrivate.toggleStepButtons();
                            }
                        });
                        cswPrivate.stepTwoComplete = true;
                    } else {
                        cswPrivate.toggleStepButtons();
                    }
                };
            } ()); //Step 2

            //Step 3: Containers
            cswPrivate.makeStep3 = (function () {
                cswPrivate.stepThreeComplete = false;
                return function () {
                    cswPrivate.toggleStepButtons();

                    if (false === cswPrivate.stepThreeComplete) {
                        cswPrivate.divStep3 = cswPrivate.divStep3 || cswPrivate.wizard.div(3);
                        cswPrivate.divStep3.empty();
                        cswPrivate.divStep3.label({
                            text: "Loading Containers and their Statuses...",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep3.br({ number: 2 });
                        //TODO - if end time = Today, include an ActionSelect control in each row, along with a save button outside the grid to update action
                        Csw.ajaxWcf.post({
                            urlMethod: 'Containers/getContainerStatuses',
                            data: cswPrivate.state,
                            success: function (ajaxdata) {
                                cswPrivate.data.ContainerStatuses = [{
                                    ContainerId: '',
                                    ContainerBarcode: '',
                                    ContainerStatus: '',
                                    Action: '',
                                    ActionApplied: ''
                                }];
                                Csw.extend(cswPrivate.data.ContainerStatuses, ajaxdata.ContainerStatuses);

                                var ContainersGridFields = [];
                                var ContainersGridColumns = [];
                                var addColumn = function (colName, displayName, hidden, filter) {
                                    ContainersGridColumns.push({
                                        dataIndex: colName,
                                        filterable: true,
                                        format: null,
                                        hidden: hidden,
                                        header: displayName,
                                        id: 'reconciliation_container_' + colName,
                                        filter: filter
                                    });
                                    ContainersGridFields.push({
                                        name: colName,
                                        type: 'string',
                                        useNull: true
                                    });
                                };
                                addColumn('containerid', 'Container Id', true);
                                addColumn('containerbarcode', 'Container Barcode', false);
                                var StatusOptions = [];
                                Csw.each(cswPrivate.data.ContainerStatistics, function(row) {
                                    StatusOptions.push( row.Status );
                                });
                                addColumn('status', 'Status', false, {
                                    type: 'list',
                                    options: StatusOptions
                                });
                                addColumn('action', 'Action', false);

                                var ContainersGridData = [];
                                Csw.each(cswPrivate.data.ContainerStatuses, function (row) {
                                    ContainersGridData.push({
                                        containerid: row.ContainerId,
                                        containerbarcode: row.ContainerBarcode,
                                        status: row.ContainerStatus,
                                        action: row.Action
                                    });
                                });

                                var ContainersGridId = 'ReconciliationContainersGrid';
                                cswPrivate.gridOptions = {
                                    name: ContainersGridId,
                                    storeId: ContainersGridId,
                                    title: 'Containers in ' + cswPrivate.state.LocationName
                                        + ' from ' + cswPrivate.state.StartDate
                                        + ' to ' + cswPrivate.state.EndDate,
                                    stateId: ContainersGridId,
                                    usePaging: false,
                                    showActionColumn: false,
                                    canSelectRow: false,
                                    onLoad: null,
                                    onEdit: null,
                                    onDelete: null,
                                    onSelect: null,
                                    onDeselect: null,
                                    height: 200,
                                    forcefit: true,
                                    width: '100%',
                                    fields: ContainersGridFields,
                                    columns: ContainersGridColumns,
                                    data: ContainersGridData
                                };
                                cswPrivate.ReconciliationContainersGrid = cswPrivate.divStep3.grid(cswPrivate.gridOptions);
                                cswPrivate.toggleStepButtons();
                            }
                        });

                        cswPrivate.stepThreeComplete = true;
                    }
                };
            } ()); //Step 3

            //Helper Functions
            cswPrivate.getCurrentDate = function () {
                var today = new Date();
                var dd = today.getDate();
                var mm = today.getMonth() + 1; //January is 0!
                var yyyy = today.getFullYear();
                if (dd < 10) {
                    dd = '0' + dd;
                } if (mm < 10) {
                    mm = '0' + mm;
                }
                today = mm + '/' + dd + '/' + yyyy;
                return today;
            };

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                cswPrivate.validateState();
                cswPrivate.wizardSteps = {
                    1: 'Choose Location and Dates',
                    2: 'Statistics',
                    3: 'Containers'
                };
                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.handleStep = function (newStepNo) {
                    cswPrivate.setState();
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    if (Csw.contains(cswPrivate, 'makeStep' + newStepNo)) {
                        cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                        cswPrivate.currentStepNo = newStepNo;
                        cswPrivate['makeStep' + newStepNo]();
                    }
                };

                cswPrivate.finalize = function () {
                    //TODO - Save any unsaved container statuses left over from step 3
                    Csw.tryExec(cswPrivate.onFinish);
                };

                cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                    Title: 'Reconciliation',
                    StepCount: cswPrivate.numOfSteps,
                    Steps: cswPrivate.wizardSteps,
                    StartingStep: cswPrivate.startingStep,
                    FinishText: 'Finish',
                    onNext: cswPrivate.handleStep,
                    onPrevious: cswPrivate.handleStep,
                    onCancel: function () {
                        cswPrivate.clearState();
                        Csw.tryExec(cswPrivate.onCancel);
                    },
                    onFinish: cswPrivate.finalize,
                    doNextOnInit: false
                });

            } ());

            cswPrivate.makeStep1();

            return cswPublic;
        });
} ());