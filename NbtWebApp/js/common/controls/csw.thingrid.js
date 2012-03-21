/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

(function () {
    'use strict';
    function thinGrid(options) {
        'use strict';
        /// <summary>
        /// Create a thin grid (simple HTML table with a 'More..' link) and return a Csw.thinGrid object
        ///     &#10;1 - table(options)
        ///</summary>
        /// <param name="options" type="Object">
        /// <para>A JSON Object</para>
        /// <para>options.$parent: An element to attach to.</para>
        /// <para>options.ID: An ID for the thinGrid.</para>
        /// <para>options.rows: Row values of grid. 
        ///     <para>[1,2,3] An array of values (single column)</para>	  
        ///     <para>[[a,b,c],[1,2,3]] An array of arrays (multiple columns)</para>
        /// </para>
        /// <para>options.width: thinGrid width</para>
        /// <para>options.height: thinGrid height</para>
        /// </param>
        /// <returns type="thinGrid">A thinGrid object</returns>

        var internal = {
            $parent: '',
            ID: '',
            cssclass: '',
            rows: [],
            width: '',
            height: '',
            cellpadding: 2,
            linkText: 'More...',
            onLinkClick: null
        };
        var external = {};

        (function () {
            var row = 1,
                col;
            if (options) {
                $.extend(internal, options);
            }

            $.extend(external, Csw.controls.table(internal));

            /* Ignore the header row for now */
            if (internal.rows.length > 0) {
                internal.rows.splice(0, 1);
            }

            Csw.each(internal.rows, function (value) {
                col = 1;
                if (Csw.isArray(value)) {
                    Csw.each(value, function (subVal) {
                        var valString = Csw.string(subVal, '&nbsp;');
                        external.cell(row, col).append(valString);
                        col += 1;
                    });
                } else {
                    external.cell(row, col).append(value);
                }
                row += 1;
            });

            external.cell(row, 1).link({
                text: internal.linkText,
                onClick: internal.onLinkClick
            });

        } ());

        return external;
    }

    Csw.controls.register('thinGrid', thinGrid);
    Csw.controls.thinGrid = Csw.controls.thinGrid || thinGrid;

} ());
