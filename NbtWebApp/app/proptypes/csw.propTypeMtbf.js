/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.mtbf = Csw.properties.mtbf ||
        Csw.properties.register('mtbf',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };
                //var attributes = {
                //    startdatetime: {
                //        date: null,
                //        time: null
                //    },
                //    units: null
                //};
                //var compare = {};
                //var startDate = o.propDiv.find('#' + o.ID + '_sd_date'),
                //    dateVal;

                //if (false === Csw.isNullOrEmpty(startDate)) {
                //    dateVal = startDate.val();
                //    attributes.startdatetime.date = dateVal;
                //    compare = attributes;
                //    //attributes.startdatetime.time = dateVal.time;
                //}

                //var units = o.propDiv.find('#' + o.ID + '_units');
                //if (false === Csw.isNullOrEmpty(units)) {
                //    attributes.units = units.val();
                //    compare = attributes;
                //}
                //Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPrivate.startDate = (false === cswPublic.data.isMulti()) ? Csw.string(cswPrivate.propVals.startdatetime.date) : Csw.enums.multiEditDefaultValue;
                    cswPrivate.dateFormat = Csw.serverDateFormatToJQuery(cswPrivate.propVals.startdatetime.dateformat);

                    cswPrivate.value = (false === cswPublic.data.isMulti()) ? Csw.string(cswPrivate.propVals.value).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.units = (false === cswPublic.data.isMulti()) ? Csw.string(cswPrivate.propVals.units).trim() : Csw.enums.multiEditDefaultValue;

                    cswPublic.control = cswPrivate.parent.table({
                        ID: Csw.makeId(cswPublic.data.ID, 'tbl')
                    });

                    cswPrivate.mtbfStatic = (cswPrivate.units !== Csw.enums.multiEditDefaultValue) ? cswPrivate.value + '&nbsp;' + cswPrivate.units : cswPrivate.value;
                    cswPublic.control.cell(1, 1).append(cswPrivate.mtbfStatic);

                    cswPrivate.cell12 = cswPublic.control.cell(1, 2);

                    if (false === cswPublic.data.isReadOnly()) {
                        cswPrivate.cell12.icon({
                            ID: cswPublic.data.ID,
                            iconType: Csw.enums.iconType.pencil,
                            hovertext: 'Edit',
                            size: 16,
                            isButton: true,
                            onClick: function() {
                                cswPrivate.editTable.show();
                            }
                        });

                        cswPrivate.editTable = cswPublic.control.cell(2, 2).table({ ID: Csw.makeId(cswPublic.data.ID, 'edittbl') });
                        cswPrivate.editTable.cell(1, 1).text('Start Date');

                        cswPrivate.datePicker = cswPrivate.editTable.cell(1, 2)
                            .dateTimePicker({
                                ID: cswPublic.data.ID + '_sd',
                                Date: cswPrivate.startDate,
                                DateFormat: cswPrivate.dateFormat,
                                DisplayMode: 'Date',
                                ReadOnly: cswPublic.data.isReadOnly(),
                                Required: cswPublic.data.isRequired(),
                                onChange: function() {
                                    var val = cswPrivate.datePicker.val();
                                    Csw.tryExec(cswPublic.data.onChange, val);
                                    cswPublic.data.onPropChange({ startdatetime: val });
                                }
                            });

                        cswPrivate.editTable.cell(3, 1).text('Units');
                        cswPrivate.unitVals = ['hours', 'days'];
                        if (cswPublic.data.isMulti()) {
                            cswPrivate.unitVals.push(Csw.enums.multiEditDefaultValue);
                        }
                        cswPrivate.unitSelect = cswPrivate.editTable.cell(3, 2).select({
                            ID: cswPublic.data.ID + '_units',
                            onChange: function () {
                                var val = cswPrivate.unitSelect.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ unit: val });
                            },
                            values: cswPrivate.unitVals,
                            selected: cswPrivate.units
                        });

                        cswPrivate.editTable.hide();
                    }
                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());