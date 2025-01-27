/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


    Csw.nbt.register('viewFilters', function (options) {
        'use strict';

        var cswPrivate = {
            name: '',
            parent: '',
            filtersMethod: 'getRuntimeViewFilters',
            applyMethod: 'updateRuntimeViewFilters',
            viewid: '',
            viewstring: '',
            onEditFilters: null,
            isTreeView: true
        };
        var cswPublic = {};


        cswPrivate.renderLinks = function (data) {
            var editbtn;
            var outertbl = cswPrivate.div.table();
            var outercell11 = outertbl.cell(1, 1);
            var outercell12 = outertbl.cell(1, 2);
            var outercell11div = outercell11.div()
                .addClass('viewfiltersdiv');
            if (cswPrivate.isTreeView) {
                outercell11div.addClass('viewfiltersdivtree');
            }
            var tbl = outercell11div.table({ cellpadding: '2px' });

            outercell12.css({ width: '100%', textAlign: 'right' });

            var editbtn = outercell12.icon({
                name: 'editfilterbtn',
                iconType: Csw.enums.iconType.pencil,
                hovertext: 'Edit Filters',
                size: 16,
                isButton: true,
                onClick: function () {
                    cswPrivate.renderDialog(data);
                } // onClick
            }); // imageButton

            var row = 1;
            var viewPropFilters = {};
            var isFirst = true;
            Csw.each(data, function (propJson) {
                Csw.each(propJson.filters, function (filtJson) {
                    viewPropFilters[filtJson.arbitraryid] = Csw.nbt.viewPropFilter({
                        parent: tbl,
                        viewid: cswPrivate.viewid,
                        viewJson: '',
                        propsData: propJson,
                        propname: propJson.name,
                        proparbitraryid: propJson.arbitraryid,
                        filtarbitraryid: filtJson.arbitraryid,
                        viewbuilderpropid: '',
                        propRow: row,
                        firstColumn: 1,
                        showConjunction: (false === isFirst),
                        showPropertyName: true,
                        showSubfield: false,
                        readOnly: true,
                        selectedConjunction: filtJson.conjunction,
                        selectedSubFieldName: filtJson.subfieldname,
                        selectedFilterMode: filtJson.filtermode,
                        selectedValue: filtJson.value,
                        autoFocusInput: false//,
                        //$clickOnEnter: filterbtn.$
                    });
                    isFirst = false;
                    row++;
                }); //each()
            }); //each()

            if (row > 1) {   // at least one filter
                cswPrivate.div.show();
            }
        }; // cswPrivate.renderLinks()


        cswPrivate.renderDialog = function (data) {
            var filterbtn;
            var dialogdiv = Csw.literals.div({ name: cswPrivate.name })
                            //.addClass('viewfilters')
                            .hide();

            var tbl = dialogdiv.table({ cellpadding: '2px' });

            var row = 1;
            var viewPropFilters = {};
            var isFirst = true;
            Csw.each(data, function (propJson) {
                Csw.each(propJson.filters, function (filtJson) {
                    viewPropFilters[filtJson.arbitraryid] = Csw.nbt.viewPropFilter({
                        parent: tbl,
                        viewid: cswPrivate.viewid,
                        viewJson: '',
                        propsData: null,   // to fully populate the filter options
                        proparbitraryid: propJson.arbitraryid,
                        filtarbitraryid: filtJson.arbitraryid,
                        viewbuilderpropid: '',
                        propRow: row,
                        firstColumn: 1,
                        showConjunction: (false === isFirst),
                        showPropertyName: true,
                        showSubfield: false,
                        readOnly: cswPrivate.readOnly,
                        selectedConjunction: filtJson.conjunction,
                        selectedSubFieldName: filtJson.subfieldname,
                        selectedFilterMode: filtJson.filtermode,
                        selectedValue: filtJson.value,
                        autoFocusInput: false//,
                        //$clickOnEnter: filterbtn.$
                    });
                    isFirst = false;
                    row++;
                }); //each()
            }); //each()

            $.CswDialog('GenericDialog', {
                div: dialogdiv,
                title: 'Edit Filters',
                onOk: function () {
                    var filtersJson = {};

                    var PropsValidated = true;
                    
                    Csw.each(viewPropFilters, function(filter1) {
                        Csw.each(viewPropFilters, function (filter2) {
                            if (filter1 != filter2) {

                                var filter1Json = filter1.getFilterJson();
                                var filter2Json = filter2.getFilterJson();
                                if (filter1Json.filter == filter2Json.filter &&
                                    filter1Json.proparbitraryid == filter2Json.proparbitraryid &&
                                    filter1Json.filtervalue == filter2Json.filtervalue) {
                                       PropsValidated = false;
                                }
                            }//if (filter1 != filter2) {
                        });//Csw.each(viewPropFilters, function (filter2) {
                    });//Csw.each(viewPropFilters, function(filter1) {


                    if (false == PropsValidated) {
                        Csw.error.showError(
                            Csw.error.makeErrorObj(
                                Csw.enums.errorType.warning.name,
                                'Please avoid entering duplicate filters.',
                                'Attempted to send updateRuntimeViewFilters() two identical filters from csw.viewfilters.js'
                            )//Csw.error.makeErrorObj()
                        );//Csw.error.showError()
                        return false;
                    }


                    Csw.each(data, function (propJson) {
                        Csw.each(propJson.filters, function (filtJson) {
                            filtersJson[filtJson.arbitraryid] = viewPropFilters[filtJson.arbitraryid].getFilterJson();
                        });
                    });

                    Csw.ajax.deprecatedWsNbt({
                        urlMethod: cswPrivate.applyMethod,
                        data: {
                            ViewId: cswPrivate.viewid,
                            FiltersJson: JSON.stringify(filtersJson)
                        },
                        success: function (data) {
                            Csw.tryExec(cswPrivate.onEditFilters, data.newviewid);
                        }
                    });
                } // onOk
            }); // CswDialog
        }; // cswPrivate.renderDialog()


        // constructor
        (function () {

            if (options) Csw.extend(cswPrivate, options);

            cswPrivate.div = cswPrivate.parent.div({ name: cswPrivate.name })
                            .addClass('viewfilters')
                            .hide();

            Csw.ajax.deprecatedWsNbt({
                urlMethod: cswPrivate.filtersMethod,
                data: {
                    ViewId: cswPrivate.viewid,
                    ViewString: cswPrivate.viewstring
                },
                success: function (data) {

                    // case 26331
                    // render as text and links first
                    // edit filters in dialog
                    cswPrivate.renderLinks(data);

                } // success
            }); // ajax


        })(); // constructor
    }); // register
})();
