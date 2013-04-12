/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.composites.schedRulesTimeline = Csw.composites.schedRulesTimeline ||
        Csw.composites.register('schedRulesTimeline', function (cswParent, options) {
            /// <summary>
            /// 
            ///</summary>
            'use strict';
            var cswPrivate = {
                optsPopulated: false,
                FilterSchemaTo: '',
                FilterOpTo: '',
                FilterStartTo: '',
                FilterEndTo: ''
            };
            var cswPublic = {
                plot: null
            };

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                cswPrivate.mainDiv = cswParent.div();
                cswPrivate.filterTbl = cswPrivate.mainDiv.table();

                cswPrivate.mainDiv.br({ number: 2 });

                cswPrivate.tbl = cswPrivate.mainDiv.table({
                    cellspacing: 15,
                    cellpadding: 15
                });

                cswPrivate.legendCell = cswPrivate.tbl.cell(3, 1);
                cswPrivate.chartCell = cswPrivate.tbl.cell(3, 2).css({ height: '450px', width: '1375px' }).text('Fetching timeline data...');
                cswPrivate.xAxisCell = cswPrivate.tbl.cell(4, 2).text('Time (S)').css('text-align', 'center');

            }());

            cswPrivate.makeFilters = function (opts) {
                if (false == cswPrivate.optsPopulated) {

                    var onFilterChange = function () {
                        cswPrivate.FilterOpTo = cswPrivate.filterOperation.val();
                        cswPrivate.FilterSchemaTo = cswPrivate.filterSchema.val();

                        var start = cswPrivate.filterStartDate.val();
                        var end = cswPrivate.filterEndDate.val();

                        cswPrivate.FilterStartTo = start.date + ' ' + start.time;
                        cswPrivate.FilterEndTo = end.date + ' ' + end.time;
                    };

                    cswPrivate.filterTbl.cell(1, 1).setLabelText('Schema: ', false, false);
                    cswPrivate.filterSchema = cswPrivate.filterTbl.cell(1, 2).multiSelect({
                        name: 'schemaFilter',
                        values: opts.Schema,
                        onChange: onFilterChange
                    });

                    cswPrivate.filterTbl.cell(2, 1).setLabelText('Operation: ', false, false);
                    cswPrivate.filterOperation = cswPrivate.filterTbl.cell(2, 2).multiSelect({
                        name: 'opFilter',
                        values: opts.Operations,
                        onChange: onFilterChange
                    });

                    cswPrivate.filterTbl.cell(1, 3).css({ 'width': '75px' }); //for UI prettyness

                    cswPrivate.filterTbl.cell(1, 4).setLabelText('Start Date: ', false, false);
                    cswPrivate.filterStartDate = cswPrivate.filterTbl.cell(1, 5).dateTimePicker({
                        DisplayMode: 'DateTime',
                        onChange: onFilterChange
                    });

                    cswPrivate.filterTbl.cell(2, 4).setLabelText('End Date: ', false, false);
                    cswPrivate.filterEndDate = cswPrivate.filterTbl.cell(2, 5).dateTimePicker({
                        DisplayMode: 'DateTime',
                        onChange: onFilterChange
                    });
                    cswPrivate.applyFilterBtn = cswPrivate.filterTbl.cell(3, 1).buttonExt({
                        enabledText: 'Apply Filters',
                        disabledText: 'Applying...',
                        onClick: function () {
                            cswPrivate.init();
                            cswPrivate.applyFilterBtn.enable();
                        }
                    });

                    cswPrivate.optsPopulated = true;
                }
            };

            cswPrivate.plot = function (data) {
                cswPrivate.chartCell.empty();
                cswPublic.plot = $.plot($('#' + cswPrivate.chartCell.getId()), data,
                    {
                        legend: { container: $('#' + cswPrivate.legendCell.getId()) },
                        yaxis: {
                            show: false,
                            autoscaleMargin: .5,
                        },
                        series: {
                            lines: {
                                lineWidth: 10,
                                show: true
                            },
                            bars: {
                                show: false,
                                horizontal: true,
                                fill: false
                            },
                            points: {
                                show: true
                            }
                        },
                        grid: {
                            clickable: true,
                            hoverable: true
                        }
                    });

                var hoverDiv;
                $("#" + cswPrivate.chartCell.getId()).bind("plotclick", function (event, pos, item) {
                    $("#x").text(pos.x.toFixed(2));
                    $("#y").text(pos.y.toFixed(2));

                    function showTooltip(x, y, schema, op, startime, executiontime) {
                        hoverDiv = cswPrivate.mainDiv.div().css({
                            width: '275px',
                            height: '70px',
                            position: 'absolute',
                            display: 'none',
                            top: y + 5,
                            left: x + 5,
                            border: '1px solid #282828',
                            padding: '2px',
                            'background-color': '#707070',
                            opacity: 0.90,
                        });

                        var infoTbl = hoverDiv.table();
                        infoTbl.cell(1, 1).setLabelText("Schema: ");
                        infoTbl.cell(1, 2).text(schema);
                        infoTbl.cell(2, 1).setLabelText("Operation: ");
                        infoTbl.cell(2, 2).text(op);
                        infoTbl.cell(3, 1).setLabelText("Start Date: ");
                        infoTbl.cell(3, 2).text(startime);
                        infoTbl.cell(4, 1).setLabelText("Execution Time: ");
                        infoTbl.cell(4, 2).text(executiontime + " (s)");

                        hoverDiv.$.appendTo("body").fadeIn(200);
                    };

                    if (item) {
                        if (hoverDiv) {
                            hoverDiv.remove();
                        }

                        var executionTime = NaN;
                        if (null != item.series.data[item.dataIndex + 1]) {
                            executionTime = item.series.data[item.dataIndex + 1][0] - item.series.data[item.dataIndex][0];
                        } else {
                            executionTime = item.series.data[item.dataIndex][0] - item.series.data[item.dataIndex - 1][0];
                        }

                        var thisTimeSpan = item.series.DataPoints[item.dataIndex];

                        showTooltip(item.pageX, item.pageY, item.series.SchemaName, item.series.OpName, thisTimeSpan.StartDate, executionTime);
                    }
                    else {
                        if (hoverDiv) {
                            hoverDiv.$.fadeOut(200, function () {
                                hoverDiv.remove();
                            });
                        }
                    }
                });
            };

            cswPrivate.init = function () {
                Csw.ajaxWcf.post({
                    urlMethod: 'Scheduler/getTimeline',
                    data: {
                        FilterSchemaTo: cswPrivate.FilterSchemaTo,
                        FilterOpTo: cswPrivate.FilterOpTo,
                        FilterStartTimeTo: cswPrivate.FilterStartTo,
                        FilterEndTimeTo: cswPrivate.FilterEndTo,
                    },
                    success: function (data) {
                        cswPrivate.makeFilters(data.FilterData);
                        cswPrivate.plot(data.Series);
                    }
                });
            };

            (function () {
                cswPrivate.init();
            }());

            return cswPublic;
        });

}());
