/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


    Csw.composites.dateTimePicker = Csw.composites.dateTimePicker ||
        Csw.composites.register('dateTimePicker', function (cswParent, options) {
            ///<summary>Generates a dateTimePicker</summary>
            ///<param name="cswParent" type="Csw.literals">Parent element to attach dateTimePicker to.</param>
            ///<param name="options" type="Object">Object defining paramaters for dateTimePicker construction.</param>
            ///<returns type="Csw.composites.dateTimePicker">Object representing a dateTimePicker</returns>
            'use strict';
            var cswPrivate = {
                name: '',
                Date: '',
                Time: '',
                DateFormat: 'mm/dd/yyyy',
                TimeFormat: '',
                DisplayMode: 'Date',    // Date, Time, DateTime
                ReadOnly: false,
                isRequired: false,
                onChange: null,
                showTodayButton: false,
                changeYear: true,
                minDate: null,
                maxDate: null
            };
            var cswPublic = {};

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                cswPrivate.dateTimeTbl = cswParent.table({
                    isControl: cswPrivate.isControl,
                    name: cswPrivate.id
                });
                cswPublic = Csw.dom({}, cswPrivate.dateTimeTbl);
                //Csw.extend(cswPublic, Csw.literals.div(cswPrivate));

                if (cswPrivate.ReadOnly) {
                    switch (cswPrivate.DisplayMode) {
                        case 'Date':
                            cswPrivate.dateTimeTbl.cell(1,1).div({ name: cswPrivate.name + '_date', value: cswPrivate.Date });
                            break;
                        case 'Time':
                            cswPrivate.dateTimeTbl.cell(1,1).div({ name: cswPrivate.name + '_time', value: cswPrivate.Time });
                            break;
                        case 'DateTime':
                            cswPrivate.dateTimeTbl.cell(1,1).div({ name: cswPrivate.name + '_time', value: cswPrivate.Date + ' ' + cswPrivate.Time });
                            break;
                    }
                } else {
                    var changeEvent = function() {
                        Csw.tryExec(cswPrivate.onChange, cswPublic.val());
                    };

                    if (cswPrivate.DisplayMode === 'Date' || cswPrivate.DisplayMode === 'DateTime') {
                        cswPrivate.dateBox = cswPrivate.dateTimeTbl.cell(1,1).input({
                            name: cswPrivate.name + '_date',
                            type: Csw.enums.inputTypes.text,
                            value: cswPrivate.Date,
                            onChange: changeEvent,
                            width: '80px',
                            cssclass: 'textinput'
                        });
                        if (cswPrivate.Date.substr(0, 'today'.length) !== 'today') {
                            cswPrivate.dateBox.$.datepicker({
                                'dateFormat': Csw.serverDateFormatToJQuery(cswPrivate.DateFormat), 
                                'changeYear': cswPrivate.changeYear,
                                'minDate': cswPrivate.minDate,
                                'maxDate': cswPrivate.maxDate
                            });
                        }
                        cswPrivate.dateBox.required(cswPrivate.isRequired);
                    }

                    if (cswPrivate.DisplayMode === 'Time' || cswPrivate.DisplayMode === 'DateTime') {
                        cswPrivate.timeBox = cswPrivate.dateTimeTbl.cell(1,3).input({
                            name: cswPrivate.name + '_time',
                            type: Csw.enums.inputTypes.text,
                            cssclass: 'textinput',
                            onChange: changeEvent,
                            value: cswPrivate.Time,
                            width: '80px'
                        });
                        cswPrivate.dateTimeTbl.cell(1,4).button({
                            name: cswPrivate.name + '_now',
                            disableOnClick: false,
                            onClick: function () {
                                cswPrivate.timeBox.val(Csw.getTimeString(new Date(), cswPrivate.TimeFormat));
                                changeEvent();
                            },
                            enabledText: 'Now'
                        });
                        cswPrivate.timeBox.required(cswPrivate.isRequired);
                    }

                    if (Csw.bool(cswPrivate.showTodayButton)) {
                        cswPrivate.dateTimeTbl.cell(1,2).button({
                            name: cswPrivate.name + '_today',
                            disableOnClick: false,
                            onClick: function () {
                                cswPrivate.dateBox.$.datepicker('destroy');
                                cswPrivate.dateBox.val('today');  // this doesn't trigger onchange
                                changeEvent();
                            },
                            enabledText: 'Today'
                        });
                    }
                } // if-else(o.ReadOnly)
            }());

            cswPublic.setMaxDate = function(maxDate) {
                cswPrivate.dateBox.$.datepicker('option', 'maxDate', maxDate);
            };

            cswPublic.val = function (readOnly, value) {
                var ret = Csw.object();
                ret.date = '';
                ret.time = '';
                
                if (cswPrivate.dateBox && cswPrivate.dateBox.length() > 0) {
                    ret.date = (false === Csw.bool(readOnly)) ? cswPrivate.dateBox.val() : cswPrivate.dateBox.text();
                }
                if (cswPrivate.timeBox && cswPrivate.timeBox.length() > 0) {
                    ret.time = (false === Csw.bool(readOnly)) ? cswPrivate.timeBox.val() : cswPrivate.timeBox.text();
                }
                if (value) {
                    if (cswPrivate.dateBox) {
                        cswPrivate.dateBox.val(value.date);
                    }
                    if (cswPrivate.timeBox) {
                        cswPrivate.timeBox.val(value.time);
                    }
                }
                return ret;
            };

            return cswPublic;
        });

} ());