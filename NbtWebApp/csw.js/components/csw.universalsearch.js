/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    "use strict";

    function universalSearch(params) {
        var internal = {
            ID: 'newsearch',
            $searchbox_parent: null,
            $searchresults_parent: null,
            $searchfilters_parent: null,
            onBeforeSearch: null,
            onAfterSearch: null,
            onAfterNewSearch: null,
            onLoadView: null,
            onAddView: null,
            //searchresults_maxheight: '600',
            searchbox_width: '200px',

            newsearchurl: '/NbtWebApp/wsNBT.asmx/doUniversalSearch',
            filtersearchurl: '/NbtWebApp/wsNBT.asmx/filterUniversalSearch',
            restoresearchurl: '/NbtWebApp/wsNBT.asmx/restoreUniversalSearch',
            saveurl: '/NbtWebApp/wsNBT.asmx/saveSearchAsView',
            //filters: {},
            sessiondataid: '',
            searchterm: '',
            filterHideThreshold: 5,
            buttonSingleColumn: '',
            buttonMultiColumn: ''
        };
        if (params) $.extend(internal, params);

        var external = {};

        // Constructor
        // Adds a searchbox to the form
        (function () {
            var cswtable = Csw.controls.table({
                ID: Csw.controls.dom.makeId(internal.ID, '', '_div'),
                $parent: internal.$searchbox_parent
            });

            internal.searchinput = cswtable.cell(1, 1).input({
                ID: Csw.controls.dom.makeId(internal.ID, '', '_input'),
                type: Csw.enums.inputTypes.text,
                width: internal.searchbox_width
            });

            internal.searchbutton = cswtable.cell(1, 2).button({
                ID: Csw.controls.dom.makeId(internal.ID, '', '_srchbtn'),
                enabledText: 'Search',
                disabledText: 'Searching...',
                onClick: function () {
                    internal.searchterm = internal.searchinput.val();
                    //internal.filters = {};
                    internal.newsearch();
                }
            });

            internal.searchinput.clickOnEnter(internal.searchbutton);
        })();

        // Handle search submission
        internal.newsearch = function () {
            Csw.tryExec(internal.onBeforeSearch);

            Csw.ajax.post({
                url: internal.newsearchurl,
                data: { SearchTerm: internal.searchterm },
                success: function (data) {
                    internal.handleResults(data);
                    Csw.tryExec(internal.onAfterNewSearch, internal.sessiondataid);
                }
            });
        }; // search()

        internal.handleResults = function (data) {
            var fdiv, ftable, filtersdivid;

            internal.sessiondataid = data.sessiondataid;

            // Search results
            function _renderResultsTable(columns) {
                
                internal.$searchresults_parent.css({ paddingTop: '15px' });

                var resultstable = Csw.controls.table({
                    ID: Csw.controls.dom.makeId(internal.ID, '', 'resultstbl'),
                    $parent: internal.$searchresults_parent,
                    width: '100%'
                });

                resultstable.cell(1,1).append('<b>Search Results: (' + data.table.results + ')</b>');
            
                resultstable.cell(1,2).css({ width: '18px' });
                internal.buttonSingleColumn = resultstable.cell(1,2).imageButton({
                    ID: Csw.controls.dom.makeId(internal.ID, '', '_singlecol'),
                    ButtonType: Csw.enums.imageButton_ButtonType.TableSingleColumn,
                    Active: (columns === 1),
                    AlternateText: 'Single Column',
                    onClick: function() {
                        internal.$searchresults_parent.contents().remove();
                        setTimeout(function() {   // so we see the clear immediately
                            _renderResultsTable(1); 
                        }, 0);
                    }
                });
            
                resultstable.cell(1,3).css({ width: '18px' });
                internal.buttonMultiColumn = resultstable.cell(1,3).imageButton({
                    ID: Csw.controls.dom.makeId(internal.ID, '', '_multicol'),
                    ButtonType: Csw.enums.imageButton_ButtonType.TableMultiColumn,
                    Active: (columns !== 1),
                    AlternateText: 'Multi Column',
                    onClick: function() {
                        internal.$searchresults_parent.contents().remove();
                        setTimeout(function() {   // so we see the clear immediately
                            _renderResultsTable(3);
                        }, 0);
                    }
                });

                resultstable.cell(2,1).propDom({ 'colspan': 3 });

                resultstable.cell(2,1).$.CswNodeTable({
                    ID: Csw.controls.dom.makeId(internal.ID, '', 'srchresults'),
                    onEditNode: null,
                    onDeleteNode: function() {
                        // case 25380 - refresh on delete
                        external.restoreSearch(internal.sessiondataid);
                    },
                    //onSuccess: internal.onAfterSearch,
                    onNoResults: function () {
                        resultstable.cell(2,1).text('No Results Found');
                    },
                    tabledata: data.table,
                    //maxheight: internal.searchresults_maxheight
                    columns: columns
                });
            }

            _renderResultsTable(1);

            // Filter panel
            filtersdivid = Csw.controls.dom.makeId(internal.ID, '', 'filtersdiv');
            fdiv = Csw.controls.div({
                ID: filtersdivid,
                $parent: internal.$searchfilters_parent
            }).css({
                paddingTop: '15px'
                //height: internal.searchresults_maxheight + 'px',
                //overflow: 'auto'
            });

            fdiv.span({ text: 'Searched For: ' + data.searchterm }).br();
            ftable = fdiv.table({});

            // Filters in use
            var hasFilters = false;
            var ftable_row = 1;
            function showFilter(thisFilter) {
                var cell1 = ftable.cell(ftable_row, 1);
                cell1.propDom('align', 'right');
                cell1.span({
                    text: thisFilter.filtername + ':&nbsp;'
                });
                ftable.cell(ftable_row, 2).span({
                    text: thisFilter.filtervalue
                });
                if(Csw.bool(thisFilter.removeable)) {
                    ftable.cell(ftable_row, 3).imageButton({
                        ID: Csw.controls.dom.makeId(filtersdivid, '', thisFilter.filterid),
                        ButtonType: Csw.enums.imageButton_ButtonType.Delete,
                        AlternateText: 'Remove Filter',
                        onClick: function () {
                            internal.filter(thisFilter, 'remove');
                        }
                    });
                }
                ftable_row++;
                hasFilters = true;
            }
            Csw.each(data.filtersapplied, showFilter);

            if (hasFilters) {
                fdiv.br();
                fdiv.button({
                    ID: Csw.controls.dom.makeId(filtersdivid, '', "saveview"),
                    enabledText: 'Save as View',
                    disableOnClick: false,
                    onClick: internal.saveAsView
                });
            }
            fdiv.br();
            fdiv.br();

            // Filters to add
            function makeFilterLink(thisFilter, div, filterCount) {
                var flink = div.link({
                    ID: Csw.controls.dom.makeId(filtersdivid, '', thisFilter.filterid),
                    text: thisFilter.filtervalue + ' (' + thisFilter.count + ')',
                    onClick: function () {
                        internal.filter(thisFilter, 'add');
                        return false;
                    }
                });
                div.br();
            }
            function makeFilterSet(thisFilterSet, Name) {

                var thisfilterdivid = Csw.controls.dom.makeId(filtersdivid, '', Name);
                //var thisfilterdiv = fdiv.div({ ID: thisfilterdivid });
                var filterCount = 0;
                var moreDiv = Csw.controls.moreDiv({ 
                    ID: thisfilterdivid, 
                    $parent: fdiv.$ 
                });

                moreDiv.shownDiv.append('<b>' + Name + ':</b>');
                moreDiv.shownDiv.br();
                var thisdiv = moreDiv.shownDiv;
                moreDiv.moreLink.hide();
                Csw.each(thisFilterSet, function (thisFilter) {
                    if (filterCount === internal.filterHideThreshold) {
                        moreDiv.moreLink.show();
                        thisdiv = moreDiv.hiddenDiv;
                    }
                    makeFilterLink(thisFilter, thisdiv, filterCount);
                    filterCount++;
                });
                fdiv.br();
                fdiv.br();
            }

            Csw.each(data.filters, makeFilterSet);

            Csw.tryExec(internal.onAfterSearch);
        } // handleResults()


        internal.filter = function (thisFilter, action) {
            //internal.filters[thisFilter.filterid] = thisFilter;

            Csw.tryExec(internal.onBeforeSearch);
            Csw.ajax.post({
                url: internal.filtersearchurl,
                data: {
                    SessionDataId: internal.sessiondataid,
                    Filter: JSON.stringify(thisFilter),
                    Action: action
                },
                success: internal.handleResults
            });
        }; // filter()

        internal.saveAsView = function () {
            $.CswDialog('AddViewDialog', {
                ID: Csw.controls.dom.makeId(internal.ID, '', 'addviewdialog'),
                //viewmode: 'table',
                category: 'Saved Searches',
                onAddView: function (newviewid, viewmode) {

                    Csw.ajax.post({
                        url: internal.saveurl,
                        data: {
                            SessionDataId: internal.sessiondataid,
                            ViewId: newviewid//,
                            //                            SearchTerm: internal.searchterm,
                            //                            Filters: JSON.stringify(internal.filters)
                        },
                        success: function (data) {
                            Csw.tryExec(internal.onAddView, newviewid, viewmode);
                            Csw.tryExec(internal.onLoadView, newviewid, viewmode);
                        }
                    }); // ajax  

                } // onAddView()
            }); // CswDialog
        }; // saveAsView()

        external.restoreSearch = function (searchid) {

            internal.sessiondataid = searchid;

            Csw.tryExec(internal.onBeforeSearch);
            Csw.ajax.post({
                url: internal.restoresearchurl,
                data: {
                    SessionDataId: internal.sessiondataid
                },
                success: function (data) {
                    internal.handleResults(data);
                    Csw.tryExec(internal.onAfterNewSearch, internal.sessiondataid);
                }
            });
        }; // restoreSearch()

        return external;
    };

    Csw.controls.register('universalSearch', universalSearch);
    Csw.controls.universalSearch = Csw.controls.universalSearch || universalSearch;
})();
