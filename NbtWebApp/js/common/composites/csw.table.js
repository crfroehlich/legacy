/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.composites.table = Csw.composites.table ||
        Csw.composites.register('table', function (cswParent, options) {
            /// <summary>
            /// Create or extend an HTML table and return a Csw.composites.table object
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.$parent: An element to attach to.</para>
            /// <para>options.ID: An ID for the table.</para>
            /// <para>options.align: Align value</para>
            /// <para>options.width: Table width</para>
            /// </param>
            /// <returns type="table">A table object</returns>
            'use strict';
            var internal = {
                ID: '',
                TableCssClass: '',
                CellCssClass: '',
                cellpadding: 0,
                cellspacing: 0,
                align: '',
                width: '',
                cellalign: 'left',
                cellvalign: 'top',
                onCreateCell: function () {
                },
                FirstCellRightAlign: false,
                OddCellRightAlign: false,
                border: 0
            };
            var external = {};

            (function () {
                var table = '<table id="' + internal.ID + '"></table>';
                var isjQuery = Csw.isJQuery(cswParent);
                if (isjQuery) {
                    table = cswParent;
                } 
                if(options) {
                    $.extend(internal, options);
                }

                internal.table = cswParent.attach(table);
                external.$ = internal.table.$;
 
                internal.table.bind('CswTable_onCreateCell', function (e, cell, row, column) {
                    Csw.tryExec(internal.onCreateCell(e, cell, row, column));
                    e.stopPropagation(); // prevents events from triggering in nested tables
                });
                internal.table.trigger('CswTable_onCreateCell', [internal.table.find('td'), 1, 1]);

                if (false === isjQuery) {
                    internal.table.addClass(internal.TableCssClass);
                    internal.table.propDom({
                        width: internal.width,
                        align: internal.align
                    });
                    internal.table.propNonDom({
                        cellpadding: internal.cellpadding,
                        cellspacing: internal.cellspacing,
                        border: internal.border,
                        cellalign: internal.cellalign,
                        cellvalign: internal.cellvalign,
                        cellcssclass: internal.CellCssClass,
                        FirstCellRightAlign: internal.FirstCellRightAlign,
                        OddCellRightAlign: internal.OddCellRightAlign
                    });
                    internal.table.css('text-align', internal.align);
                }
            } ());

            external.cell = function (row, col) {
                /// <summary>Get a cell from the table</summary>
                /// <param name="row" type="Number">Row number</param>
                /// <param name="col" type="Number">Column number</param>
                /// <returns type="Object">A Csw table cell object.</returns>
                var thisRow, align, newCell, retCell = {}, html,
                    thisCol, id,
                    attr = Csw.makeAttr();

                if (internal.table.length() > 0 &&
                    false === Csw.isNullOrEmpty(row) &&
                        false === Csw.isNullOrEmpty(col)) {
                    if (row <= 0) {
                        Csw.log("table.cell() error: row must be greater than 1, got: " + row);
                        row = 1;
                    }
                    if (col <= 0) {
                        Csw.log("table.cell() error: col must be greater than 1, got: " + col);
                        col = 1;
                    }

                    if (internal.ID) {
                        retCell = internal.table.find('#' + Csw.makeId(internal.ID, 'row_' + row, 'col_' + col, '', false));
                    }
                    if (Csw.isNullOrEmpty(retCell)) {
                        retCell = internal.table.children('tbody')
                            .children('tr:eq(' + Csw.number(row - 1) + ')')
                            .children('td:eq(' + Csw.number(col - 1) + ')');
                    }

                    if (Csw.isNullOrEmpty(retCell)) {
                        while (row > internal.table.children('tbody').children('tr').length()) {
                            internal.table.append('<tr></tr>');
                        }
                        thisRow = internal.table.children('tbody').children('tr:eq(' + Csw.number(row - 1) + ')');
                        thisCol = thisRow.children('td').length();

                        while (col > thisCol) {
                            html = '';
                            thisCol += 1;
                            id = Csw.makeId(internal.ID, 'row_' + row, 'col_' + thisCol, '', false);
                            align = internal.table.propNonDom('cellalign');
                            if ((thisRow.children('td').length() === 0 && Csw.bool(internal.table.propNonDom('FirstCellRightAlign'))) ||
                                (thisRow.children('td').length() % 2 === 0 && Csw.bool(internal.table.propNonDom('OddCellRightAlign')))) {
                                align = 'right';
                            }
                            html += '<td ';
                            if (false === Csw.isNullOrEmpty(id)) {
                                attr.add('id', id);
                            }
                            attr.add('realrow', row);
                            attr.add('realcol', thisCol);
                            attr.add('class', internal.table.propNonDom('cellcssclass'));
                            attr.add('align', align);
                            attr.add('valign', internal.table.propNonDom('cellvalign'));
                            html += attr.get();
                            html += '>';
                            html += '</td>';
                            newCell = thisRow.attach(html);

                            internal.table.trigger('CswTable_onCreateCell', [newCell, row, thisCol]);
                            if (thisCol === col) {
                                retCell = newCell;
                            }
                        }
                    }

                    retCell.align = function (alignTo) {
                        retCell.css('text-align', alignTo);
                        retCell.propDom('align', alignTo);
                        return retCell;
                    };
                }
                return retCell;
            };

            //        external.add = function (row, col, content, id) {
            //            /// <summary>Add content to a cell of this table.</summary>
            //            /// <param name="row" type="Number">Row number.</param>
            //            /// <param name="col" type="Number">Column number.</param>
            //            /// <param name="content" type="String">Content to add.</param>
            //            /// <returns type="Object">The specified cell.</returns>
            //            var retCell = external.cell(row, col, id);
            //            retCell.append(content);
            //            return retCell;
            //        };

            external.maxrows = function () {
                /// <summary>Get the maximum table row number</summary>
                /// <returns type="Number">Number of rows</returns>
                var rows = internal.table.children('tbody').children('tr');
                return rows.length();
            };

            external.maxcolumns = function () {
                /// <summary>Get the maximum table column number</summary>
                /// <returns type="Number">Number of columns</returns>
                var body = internal.table.children('tbody'),
                    maxrows = external.maxrows(),
                    maxcolumns = 0,
                    r, c, columns, row;

                for (r = 0; r < maxrows; r += 1) {
                    row = body.children('tr:eq(' + r + ')');
                    columns = row.children('td');
                    if (columns.length() > maxcolumns) {
                        maxcolumns = columns.length();
                    }
                }
                return maxcolumns;
            };

            external.finish = function (onEmptyCell, startingRow, startingCol) {
                /// <summary>Finish</summary>
                /// <returns type="undefined"></returns>
                var maxrows = external.maxrows(),
                    maxcolumns = external.maxcolumns(),
                    r, c, cell;

                // make missing cells, and add &nbsp; to empty cells
                for (r = Csw.number(startingRow, 1); r <= maxrows; r += 1) {
                    for (c = Csw.number(startingCol, 1); c <= maxcolumns; c += 1) {
                        cell = external.cell(r, c);
                        if (cell.length() === 0) {
                            if (onEmptyCell !== null) {
                                onEmptyCell(cell, r, c);
                            } else {
                                cell.append('&nbsp;');
                            }
                        }
                    }
                }
            };

            // These are safe for nested tables, since using $.find() is not
            external.findRow = function (criteria) {
                /// <summary>Find a row by jQuery search criteria</summary>
                /// <param name="criteria" type="String"></param>
                /// <returns type="Object">Rows matching search</returns>
                var rows = internal.table.children('tbody').children('tr'),
                    ret = {};
                if (false === Csw.isNullOrEmpty(criteria)) {
                    ret = rows.filter(criteria);
                }
                return ret;
            };

            external.findCell = function (criteria) {
                /// <summary>Find a cells by jQuery search criteria</summary>
                /// <param name="criteria" type="String"></param>
                /// <returns type="Object">Cells matching search</returns>
                var cells, ret = {};
                if (Csw.contains(criteria, 'row') &&
                    Csw.contains(criteria, 'column')) {
                    ret = internal.table.jquery($(external.$[0].rows[criteria.row].cells[criteria.column]));
                } else {
                    cells = internal.table.children('tbody').children('tr').children('td');
                    if (cells.isValid && false === Csw.isNullOrEmpty(criteria)) {
                        ret = cells.filter(criteria);
                    }
                }
                return ret;
            };

            external.rowFindCell = function (row, criteria) {
                /// <summary>Given a row, find a cell by jQuery search criteria</summary>
                /// <param name="row" type="Object"></param>
                /// <param name="criteria" type="String"></param>
                /// <returns type="Object">Cells matching search</returns>
                var cells = row.children('td'),
                    $cells, ret = {};

                if (false === Csw.isNullOrEmpty(criteria)) {
                    $cells = cells.$.filter(criteria);
                    ret = internal.table.jquery($cells);
                }
                return ret;
            };

            return external;
        });

} ());
