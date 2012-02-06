/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _cswTimeInterval() {
    'use strict';

    var timeInterval = function (options) {
        var o = {
            ID: '',
            $parent: '',
            values: {},
            Multi: false,
            ReadOnly: false,
            Required: false,
            onchange: null
        };
        if (options) {
            $.extend(true, o, options);
        }

        var now = new Date(),
            nowString = (now.getMonth() + 1) + '/' + now.getDate() + '/' + now.getFullYear(),
            rateType, $WeeklyDiv, $MonthlyDiv, $YearlyDiv, dateFormat, rateInterval = {}, $pickerCell, $interval;

        var saveRateInterval = function () {
            Csw.clientDb.setItem(o.ID + '_rateIntervalSave', rateInterval);
        };

                var toggleIntervalDiv = function (interval, $weeklyradio, $monthlyradio, $yearlyradio) {

            if (window.abandonHope) {
                $weeklyradio.attr('checked', false);
                $monthlyradio.attr('checked', false);
                $yearlyradio.attr('checked', false);
            }
            if (false === Csw.isNullOrEmpty($WeeklyDiv, true)) {
                $WeeklyDiv.hide();
            }
            if (false === Csw.isNullOrEmpty($MonthlyDiv, true)) {
                $MonthlyDiv.hide();
            }
            if (false === Csw.isNullOrEmpty($YearlyDiv, true)) {
                $YearlyDiv.hide();
            }
            switch (interval) {
                case Csw.enums.rateIntervalTypes.WeeklyByDay:
                    $WeeklyDiv.show();
                    if (window.abandonHope) {
                        $weeklyradio.attr('checked', true);
                    }
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByDate:
                    $MonthlyDiv.show();
                    if (window.abandonHope) {
                        $monthlyradio.attr('checked', true);
                    }
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                    $MonthlyDiv.show();
                    if (window.abandonHope) {
                        $monthlyradio.attr('checked', true);
                    }
                    break;
                case Csw.enums.rateIntervalTypes.YearlyByDate:
                    $YearlyDiv.show();
                    if (window.abandonHope) {
                        $yearlyradio.attr('checked', true);
                    }
                    break;
            }
        };

        var weekDayDef = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

        var makeWeekDayPicker = function (thisRateType) {
            //return (function () {
            var weeklyDayPickerComplete = false,
                $ret, weekdays, $startingDate,
                isWeekly = (thisRateType === Csw.enums.rateIntervalTypes.WeeklyByDay),
                dayPropName = 'weeklyday';

            if (false === isWeekly) {
                dayPropName = 'monthlyday';
            }
            
            return function ($parent, onchange, useRadio, elemId) {

                function isChecked (day) {
                    var thisDay = weekDayDef[day - 1];
                    return false === o.Multi && Csw.contains(weekdays, thisDay);
                }

                function saveWeekInterval () {
                    if (isWeekly) {
                        Csw.each(rateInterval, function (prop, key) {
                            if (key !== 'dateformat' && key !== 'ratetype' && key !== 'startingdate' && key !== 'weeklyday') {
                                delete rateInterval[key];
                            }
                        });
                        rateInterval.startingdate = $startingDate.CswDateTimePicker('value');
                        rateInterval.startingdate.dateformat = dateFormat;
                    }
                    rateInterval.ratetype = thisRateType;
                    rateInterval.dateformat = dateFormat;
                    rateInterval[dayPropName] = weekdays.join(',');
                    saveRateInterval();
                }

                function dayChange() {
                    if (Csw.isFunction(o.onchange)) {
                        o.onchange();
                    }
                    var $this = $(this),
                        day = weekDayDef[$this.val() - 1];
                    if ($this.is(':checked')) {
                        if (false === isWeekly) {
                            weekdays = [];
                        }
                        if (false === Csw.contains(weekdays, day)) {
                            weekdays.push(day);
                        }
                    } else {
                        weekdays.splice(weekdays.indexOf(day), 1);
                    }
                    saveWeekInterval();
                }

                if (false === weeklyDayPickerComplete) {
                    $ret = $('<div />').appendTo($parent);
                    var id = elemId || o.ID + '_weeklyday',
                        $picker, $table, i, type, $pickercell, weeklyStartDate,
                        $WeeklyTable = $ret.CswTable('init', {
                            ID: o.ID + '_weeklytbl',
                            cellalign: 'center',
                            FirstCellRightAlign: true
                        });

                    weekdays = Csw.string(rateInterval[dayPropName]).split(',');

                    $picker = $WeeklyTable.CswTable('cell', 1, 2);
                    $table = $picker.CswTable('init', {
                        ID: id,
                        cellalign: 'center'
                    });

                    $table.CswTable('cell', 1, 1).append('Su');
                    $table.CswTable('cell', 1, 2).append('M');
                    $table.CswTable('cell', 1, 3).append('Tu');
                    $table.CswTable('cell', 1, 4).append('W');
                    $table.CswTable('cell', 1, 5).append('Th');
                    $table.CswTable('cell', 1, 6).append('F');
                    $table.CswTable('cell', 1, 7).append('Sa');
                    
                    for (i = 1; i <= 7; i += 1) {
                        type = Csw.enums.inputTypes.checkbox;
                        if (useRadio) {
                            type = Csw.enums.inputTypes.radio;
                        }
                        $pickercell = $table.CswTable('cell', 2, i);
                        $pickercell.CswInput('init', {
                            ID: id + '_' + i,
                            name: id,
                            type: type,
                            onChange: dayChange,
                            value: i
                        })
                            .CswAttrDom('checked', isChecked(i));

                    } //for (i = 1; i <= 7; i += 1)

                    //Starting Date
                    if (isWeekly) {
                        if (false === o.Multi && Csw.contains(rateInterval, 'startingdate')) {
                            weeklyStartDate = Csw.string(rateInterval.startingdate.date);
                        }
                        if (Csw.isNullOrEmpty(weeklyStartDate)) {
                            rateInterval.startingdate = {date: nowString, dateformat: dateFormat};
                            saveRateInterval();
                        }
                        $WeeklyTable.CswTable('cell', 1, 1).append('Every:');

                        $WeeklyTable.CswTable('cell', 2, 1).append('Starting On:');
                        $startingDate = $WeeklyTable.CswTable('cell', 2, 2)
                            .CswDateTimePicker('init', {
                                ID: o.ID + '_weekly_sd',
                                Date: weeklyStartDate,
                                DateFormat: dateFormat,
                                DisplayMode: 'Date',
                                ReadOnly: o.ReadOnly,
                                Required: o.Required,
                                OnChange: function () {
                                    if (Csw.isFunction(o.onchange)) {
                                        o.onchange();
                                    }
                                    saveWeekInterval();
                                }
                            });
                    } //if(isWeekly)

                    saveWeekInterval();

                    weeklyDayPickerComplete = true;
                    $ret.addClass('CswFieldTypeTimeInterval_Div');
                } // if (false === weeklyDayComplete)

                return $ret;
            };
            // } ()); // makeWeekDayPicker()
        };

        var weeklyWeekPicker = makeWeekDayPicker(Csw.enums.rateIntervalTypes.WeeklyByDay),
            monthlyWeekPicker = makeWeekDayPicker(Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay);

        var makeMonthlyPicker = (function () {
            var monthlyPickerComplete = false,
                $ret;

            return function ($parent) {
                var $MonthlyRateSelect, $MonthlyDateSelect, $MonthlyWeekSelect, $startOnMonth, $startOnYear,
                    monthlyRadioId = Csw.makeId({prefix: o.ID, ID: 'monthly'}),
                    monthlyDayPickerId = Csw.makeId({prefix: o.ID, ID: 'monthly', suffix: 'day'});

                function saveMonthInterval () {
                    Csw.each(rateInterval, function (prop, key) {
                        if (key !== 'dateformat' && key !== 'ratetype' && key !== 'monthlyday' && key !== 'monthlydate' && key !== 'monthlyfrequency' && key !== 'startingmonth' && key !== 'startingyear') {
                            delete rateInterval[key];
                        }
                    });
                    if (rateType === Csw.enums.rateIntervalTypes.MonthlyByDate) {
                        delete rateInterval.monthlyday;
                        delete rateInterval.monthlyweek;
                        rateInterval.monthlydate = $MonthlyDateSelect.find(':selected').val();
                    } else {
                        delete rateInterval.monthlydate;
                        rateInterval.monthlyweek = $MonthlyWeekSelect.find(':selected').val();
                    }

                    rateInterval.ratetype = rateType;
                    rateInterval.dateformat = dateFormat;
                    rateInterval.monthlyfrequency = $MonthlyRateSelect.find(':selected').val();
                    rateInterval.startingmonth = $startOnMonth.find(':selected').val();
                    rateInterval.startingyear = $startOnYear.find(':selected').val();
                    saveRateInterval();
                }

                function makeMonthlyByDateSelect () {
                    var $byDate = $('<div />'),
                        daysInMonth = ChemSW.makeSequentialArray(1, 31), selectedDay = '';

                    if (Csw.bool(o.Multi)) {
                        selectedDay = Csw.enums.multiEditDefaultValue;
                        daysInMonth.unshift(Csw.enums.multiEditDefaultValue);
                    } else if (Csw.contains(rateInterval, 'monthlydate')) {
                        selectedDay = Csw.number(rateInterval.monthlydate, 1);
                    }

                    $byDate.CswInput('init', {
                        ID: Csw.makeId({prefix: o.ID, ID: 'monthly', suffix: 'by_date'}),
                        name: monthlyRadioId,
                        type: Csw.enums.inputTypes.radio,
                        onChange: function () {
                            if (Csw.isFunction(o.onchange)) {
                                o.onchange();
                            }
                            rateType = $ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                            saveMonthInterval();
                        },
                        value: Csw.enums.rateIntervalTypes.MonthlyByDate
                    })
                        .CswAttrDom('checked', (rateType === Csw.enums.rateIntervalTypes.MonthlyByDate));

                    $byDate.append('On Day of Month:&nbsp;');

                    $MonthlyDateSelect = $byDate.CswSelect('init', {
                        ID: Csw.makeId({prefix: o.ID, ID: 'monthly', suffix: 'date'}),
                        onChange: function () {
                            if (Csw.isFunction(o.onchange)) {
                                o.onchange();
                            }
                            saveMonthInterval();
                        },
                        values: daysInMonth,
                        selected: selectedDay
                    });

                    return $byDate;
                }

                function makeEveryMonthSelect () {
                    var $every = $('<div>Every </div>'),
                        frequency = ChemSW.makeSequentialArray(1, 12),
                        selected;

                    if (Csw.bool(o.Multi)) {
                        frequency.unshift(Csw.enums.multiEditDefaultValue);
                        selected = Csw.enums.multiEditDefaultValue;
                    } else {
                        selected = Csw.number(rateInterval.monthlyfrequency, 1);
                    }

                    $MonthlyRateSelect = $every.CswSelect('init', {
                        ID: Csw.makeId({prefix: o.ID, ID: 'monthly', suffix: 'rate'}),
                        onChange: function () {
                            if (Csw.isFunction(o.onchange)) {
                                o.onchange();
                            }
                            saveMonthInterval();
                        },
                        values: frequency,
                        selected: selected
                    });

                    $every.append(' Month(s)<br/>');
                    return $every;
                }

                function makeMonthlyByDayOfWeek () {
                    var $byDay = $('<div />'),
                        monthlyWeekId = Csw.makeId({prefix: o.ID, ID: 'monthly', suffix: 'week'}),
                        monthlyByDayId = Csw.makeId({prefix: o.ID, ID: 'monthly', suffix: 'by_day'}),
                        selected,
                        weeksInMonth = [
                            {value: 1, display: 'First:'},
                            {value: 2, display: 'Second:'},
                            {value: 3, display: 'Third:'},
                            {value: 4, display: 'Fourth:'}
                        ];

                    $byDay.CswInput('init', {
                        ID: monthlyByDayId,
                        name: monthlyRadioId,
                        type: Csw.enums.inputTypes.radio,
                        onChange: function () {
                            if (Csw.isFunction(o.onchange)) {
                                o.onchange();
                            }
                            rateType = $ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                            saveMonthInterval();
                        },
                        value: Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay
                    })
                        .CswAttrDom('checked', (rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay));

                    $byDay.append('Every&nbsp;');

                    if (o.Multi) {
                        weeksInMonth.unshift({value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue});
                        selected = Csw.enums.multiEditDefaultValue;
                    } else {
                        selected = Csw.number(rateInterval.monthlyweek, 1);
                    }

                    $MonthlyWeekSelect = $byDay.CswSelect('init', {
                        ID: monthlyWeekId,
                        values: weeksInMonth,
                        selected: selected,
                        onChange: function () {
                            if (Csw.isFunction(o.onchange)) {
                                o.onchange();
                            }
                            saveMonthInterval();
                        }
                    });
                    $byDay.append('<br/>');

                    monthlyWeekPicker($byDay, o.onchange, true, monthlyDayPickerId);
                    return $byDay;
                }

                function makeStartOnSelects () {
                    var $startOn = $('<div />'),
                        monthsInYear = ChemSW.makeSequentialArray(1, 12),
                        year = now.getFullYear(),
                        yearsToAllow = ChemSW.makeSequentialArray(year - 10, year + 10),
                        selectedMonth, selectedYear;

                    $startOn.append('<br/>Starting On:&nbsp;');

                    if (o.Multi) {
                        monthsInYear.unshift(Csw.enums.multiEditDefaultValue);
                        yearsToAllow.unshift(Csw.enums.multiEditDefaultValue);
                        selectedMonth = Csw.enums.multiEditDefaultValue;
                        selectedYear = Csw.enums.multiEditDefaultValue;
                    } else {
                        selectedMonth = Csw.number(rateInterval.startingmonth, (now.getMonth() + 1));
                        selectedYear = Csw.number(rateInterval.startingyear, year);
                    }

                    $startOnMonth = $startOn.CswSelect('init', {
                        ID: Csw.makeId({prefix: o.ID, ID: 'monthly', suffix: 'startMonth'}),
                        values: monthsInYear,
                        selected: selectedMonth,
                        onChange: function () {
                            if (Csw.isFunction(o.onchange)) {
                                o.onchange();
                            }
                            saveMonthInterval();
                        }
                    });

                    $startOn.append('/');

                    $startOnYear = $startOn.CswSelect('init', {
                        ID: Csw.makeId({prefix: o.ID, ID: 'monthly', suffix: 'startYear'}),
                        values: yearsToAllow,
                        selected: selectedYear,
                        onChange: function () {
                            if (Csw.isFunction(o.onchange)) {
                                o.onchange();
                            }
                            saveMonthInterval();
                        }
                    });
                    return $startOn;
                }

                if (false === monthlyPickerComplete) {
                    $ret = $('<div />').appendTo($parent);
                    $ret.append(makeEveryMonthSelect());
                    $ret.append(makeMonthlyByDateSelect());
                    $ret.append('<br/>');
                    $ret.append(makeMonthlyByDayOfWeek());
                    $ret.append(makeStartOnSelects());
                    $ret.addClass('CswFieldTypeTimeInterval_Div');

                    saveMonthInterval();

                    monthlyPickerComplete = true;
                } // if (false === monthlyPickerComplete)

                return $ret;
            };
        }());

        var makeYearlyDatePicker = (function () {
            var yearlyDatePickerComplete = false,
                $ret, $yearlyDate;
            return function ($parent) {

                function saveYearInterval () {
                    Csw.each(rateInterval, function (prop, key) {
                        if (key !== 'dateformat' && key !== 'ratetype' && key !== 'yearlydate') {
                            delete rateInterval[key];
                        }
                    });

                    rateInterval.ratetype = rateType;
                    rateInterval.dateformat = dateFormat;
                    rateInterval.yearlydate = $yearlyDate.CswDateTimePicker('value');
                    rateInterval.yearlydate.dateformat = dateFormat;
                    saveRateInterval();
                }

                if (false === yearlyDatePickerComplete) {
                    $ret = $('<div />').appendTo($parent);

                    var yearlyStartDate = '';

                    if (Csw.bool(o.Multi)) {
                        yearlyStartDate = Csw.enums.multiEditDefaultValue;
                    } else if (Csw.contains(rateInterval, 'yearlydate')) {
                        yearlyStartDate = Csw.string(rateInterval.yearlydate.date);
                    }
                    if (Csw.isNullOrEmpty(yearlyStartDate)) {
                        rateInterval.yearlydate = {date: nowString, dateformat: dateFormat};
                    }

                    $ret.append('Every Year, Starting On:<br/>');

                    $yearlyDate = $ret.CswDateTimePicker('init', {
                        ID: Csw.makeId({prefix: o.ID, ID: 'yearly', suffix: 'sd'}),
                        Date: yearlyStartDate,
                        DateFormat: dateFormat,
                        DisplayMode: 'Date',
                        ReadOnly: o.ReadOnly,
                        Required: o.Required,
                        OnChange: function () {
                            if (Csw.isFunction(o.onchange)) {
                                o.onchange();
                            }
                            saveYearInterval();
                        }
                    });

                    $ret.appendTo($parent);

                    saveYearInterval();

                    yearlyDatePickerComplete = true;
                } // if (false === yearlyDatePickerComplete)
                return $ret.addClass('CswFieldTypeTimeInterval_Div');
            };
        }());

                var makeRateType = function ($table) {
            var $weeklyradiocell = $table.CswTable('cell', 1, 1),
                $weeklyradio = $weeklyradiocell.CswInput('init', {
                    ID: o.ID + '_type_weekly',
                    name: o.ID + '_type',
                    type: Csw.enums.inputTypes.radio,
                    value: 'weekly'
                }).CswAttrDom('checked', (rateType === Csw.enums.rateIntervalTypes.WeeklyByDay)),
                $monthlyradiocell = $table.CswTable('cell', 2, 1),
                $monthlyradio = $monthlyradiocell.CswInput('init', {
                    ID: o.ID + '_type_monthly',
                    name: o.ID + '_type',
                    type: Csw.enums.inputTypes.radio,
                    value: 'monthly'
                }).CswAttrDom('checked', (rateType === Csw.enums.rateIntervalTypes.MonthlyByDate || rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay)),
                $yearlyradiocell = $table.CswTable('cell', 3, 1),
                $yearlyradio = $yearlyradiocell.CswInput('init', {
                    ID: o.ID + '_type_yearly',
                    name: o.ID + '_type',
                    type: Csw.enums.inputTypes.radio,
                    value: 'yearly'
                }).CswAttrDom('checked', (rateType === Csw.enums.rateIntervalTypes.YearlyByDate));

            function onChange () {
                if (Csw.isFunction(o.onchange)) {
                    o.onchange();
                }
                toggleIntervalDiv(rateType, $weeklyradio, $monthlyradio, $yearlyradio);
                saveRateInterval();
            }

            //Weekly
            $table.CswTable('cell', 1, 2).append('<span>&nbsp;Weekly</span>');
            $weeklyradio.click(function () {
                rateType = Csw.enums.rateIntervalTypes.WeeklyByDay;
                rateInterval.ratetype = rateType;
                $WeeklyDiv = $WeeklyDiv || weeklyWeekPicker($pickerCell, o.onchange, false);
                onChange();
            });

            //Monthly
            $table.CswTable('cell', 2, 2).append('<span>&nbsp;Monthly</span>');
            $monthlyradio.click(function () {
                rateType = Csw.enums.rateIntervalTypes.MonthlyByDate;
                rateInterval.ratetype = rateType;
                $MonthlyDiv = $MonthlyDiv || makeMonthlyPicker($pickerCell);
                onChange();
            });

            //Yearly
            $table.CswTable('cell', 3, 2).append('<span>&nbsp;Yearly</span>');
            $yearlyradio.click(function () {
                rateType = Csw.enums.rateIntervalTypes.YearlyByDate;
                rateInterval.ratetype = rateType;
                $YearlyDiv = $YearlyDiv || makeYearlyDatePicker($pickerCell);
                onChange();
            });
        };

        var validateRateInterval = function () {
            var retVal = false, errorString = '';
            switch (rateType) {
                case Csw.enums.rateIntervalTypes.WeeklyByDay:
                    if (false === Csw.contains(rateInterval, 'startingdate') ||
                        false === Csw.contains(rateInterval.startingdate, 'date') ||
                            Csw.isNullOrEmpty(rateInterval.startingdate.date)) {
                        errorString += 'Cannot add a Weekly time interval without a starting date. ';
                    }
                    if (false === Csw.contains(rateInterval, 'weeklyday') ||
                        Csw.isNullOrEmpty(rateInterval.weeklyday)) {
                        errorString += 'Cannot add a Weekly time interval without at least one weekday selected. ';
                    }
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByDate:
                    if (false === Csw.contains(rateInterval, 'monthlydate') ||
                        Csw.isNullOrEmpty(rateInterval.monthlydate)) {
                        errorString += 'Cannot add a Monthly time interval without an \'On Day of Month\' selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'monthlyfrequency') ||
                        Csw.isNullOrEmpty(rateInterval.monthlyfrequency)) {
                        errorString += 'Cannot add a Monthly time interval without a frequency selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'startingmonth') ||
                        Csw.isNullOrEmpty(rateInterval.startingmonth)) {
                        errorString += 'Cannot add a Monthly time interval without a Starting Month selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'startingyear') ||
                        Csw.isNullOrEmpty(rateInterval.startingyear)) {
                        errorString += 'Cannot add a Monthly time interval without a Starting Year selected. ';
                    }
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                    if (false === Csw.contains(rateInterval, 'monthlyfrequency') ||
                        Csw.isNullOrEmpty(rateInterval.monthlyfrequency)) {
                        errorString += 'Cannot add a Monthly time interval without a frequency selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'monthlyday') ||
                        Csw.isNullOrEmpty(rateInterval.monthlyday)) {
                        errorString += 'Cannot add a Monthly time interval without a Weekday selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'monthlyweek') ||
                        Csw.isNullOrEmpty(rateInterval.monthlyweek)) {
                        errorString += 'Cannot add a Monthly time interval without a Weekly frequency selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'startingmonth') ||
                        Csw.isNullOrEmpty(rateInterval.startingmonth)) {
                        errorString += 'Cannot add a Monthly time interval without a starting month selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'startingyear') ||
                        Csw.isNullOrEmpty(rateInterval.startingyear)) {
                        errorString += 'Cannot add a Monthly time interval without a starting year selected. ';
                    }
                    break;
                case Csw.enums.rateIntervalTypes.YearlyByDate:
                    if (false === Csw.contains(rateInterval, 'yearlydate') ||
                        false === Csw.contains(rateInterval.yearlydate, 'date') ||
                            Csw.isNullOrEmpty(rateInterval.yearlydate.date)) {
                        errorString += 'Cannot addd a Yearly time interval without a starting date. ';
                    }
                    break;
            }
            if (false === Csw.isNullOrEmpty(errorString)) {
                retVal = Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, errorString);
            }
            return retVal;
        };

        (function () {
            var $Div = o.$parent,
                propVals = o.propVals,
                textValue,
                $table;

            //globals
            if (o.Multi) {
                //rateInterval = Csw.enums.multiEditDefaultValue;
                textValue = Csw.enums.multiEditDefaultValue;
                rateType = Csw.enums.rateIntervalTypes.WeeklyByDay;
            } else {
                $.extend(true, rateInterval, propVals.Interval.rateintervalvalue);
                textValue = Csw.string(propVals.Interval.text).trim();
                rateType = rateInterval.ratetype;
            }
            dateFormat = Csw.string(rateInterval.dateformat, 'M/d/yyyy');
            $interval = $('<div id="' + Csw.makeId({ID: o.ID, suffix: '_cswTimeInterval'}) + '"></div>')
                .appendTo($Div);

            //Page Components
            $interval.append('<span id="' + o.ID + '_textvalue">' + textValue + '</span>');
            $table = $interval.CswTable('init', {'ID': o.ID + '_tbl', cellspacing: 5});

            makeRateType($table);

            $pickerCell = $table.CswTable('cell', 1, 3)
                .CswAttrDom('rowspan', '3');

            // Set selected values
            switch (rateType) {
                case Csw.enums.rateIntervalTypes.WeeklyByDay:
                    $WeeklyDiv = weeklyWeekPicker($pickerCell, o.onchange, false);
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByDate:
                    $MonthlyDiv = makeMonthlyPicker($pickerCell);
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                    $MonthlyDiv = makeMonthlyPicker($pickerCell);
                    break;
                case Csw.enums.rateIntervalTypes.YearlyByDate:
                    $YearlyDiv = makeYearlyDatePicker($pickerCell);
                    break;
            } // switch(RateType)

            return $interval;
        }());

        var ret = {
            $interval: $interval,
            rateType: function () {
                return rateType;
            },
            rateInterval: function () {
                return rateInterval;
            },
            validateRateInterval: validateRateInterval
        };

        return ret;
    };
    Csw.register('timeInterval', timeInterval);

}());