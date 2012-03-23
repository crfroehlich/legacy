/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

(function () {

    Csw.components.factory = Csw.components.factory ||
        Csw.components.register('factory',
            function (cswParent, external) {
                /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
                /// <param name="cswParent" type="Csw.controls">An Csw Control to bind to.</param>
                /// <param name="external" type="Object">An options collection to extend.</param>
                /// <returns type="Csw.components">The options object with DOM methods attached.</returns> 
                'use strict';
                //#region internal

                var internal = {};
                external = external || {};
                if (Csw.isNullOrEmpty(cswParent)) {
                    throw new Error('Cannot create a Csw component without a Csw control');
                }

                internal.controlPreProcessing = function (opts, controlName) {
                    opts = opts || {};
                    return opts;
                };

                //#endregion internal

                //#region Csw DOM classes

                external.checkBoxArray = function (opts) {
                    /// <summary> Creates a Csw.checkBoxArray on this element</summary>
                    /// <param name="opts" type="Object">Options to define the checkBoxArray.</param>
                    /// <returns type="Csw.components.checkBoxArray">A Csw.components.checkBoxArray</returns>
                    opts = internal.controlPreProcessing(opts, 'checkBoxArray');
                    return Csw.components.checkBoxArray(cswParent, opts);
                };

                external.comboBox = function (opts) {
                    /// <summary> Creates a Csw.comboBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the comboBox.</param>
                    /// <returns type="Csw.components.comboBox">A Csw.components.comboBox</returns>
                    opts = internal.controlPreProcessing(opts, 'comboBox');
                    return Csw.components.comboBox(cswParent, opts);
                };

                external.dateTimePicker = function (opts) {
                    /// <summary> Creates a Csw.dateTimePicker on this element</summary>
                    /// <param name="opts" type="Object">Options to define the dateTimePicker.</param>
                    /// <returns type="Csw.components.dateTimePicker">A Csw.components.dateTimePicker</returns>
                    opts = internal.controlPreProcessing(opts, 'dateTimePicker');
                    return Csw.components.dateTimePicker(cswParent, opts);
                };

                external.grid = function (opts) {
                    /// <summary> Creates a Csw.grid on this element</summary>
                    /// <param name="opts" type="Object">Options to define the grid.</param>
                    /// <returns type="Csw.components.grid">A Csw.components.grid</returns>
                    opts = internal.controlPreProcessing(opts, 'grid');
                    return Csw.components.grid(cswParent, opts);
                };

                external.imageButton = function (opts) {
                    /// <summary> Creates a Csw.imageButton on this element</summary>
                    /// <param name="opts" type="Object">Options to define the imageButton.</param>
                    /// <returns type="Csw.components.imageButton">A Csw.components.imageButton</returns>
                    opts = internal.controlPreProcessing(opts, 'imageButton');
                    return Csw.components.imageButton(cswParent, opts);
                };

                external.layoutTable = function (opts) {
                    /// <summary> Creates a Csw.layoutTable on this element</summary>
                    /// <param name="opts" type="Object">Options to define the table.</param>
                    /// <returns type="Csw.components.layoutTable">A Csw.components.layoutTable</returns> 
                    opts = internal.controlPreProcessing(opts, 'layoutTable');
                    return Csw.components.layoutTable(cswParent, opts);
                };

                external.multiSelect = function (opts) {
                    /// <summary> Creates a Csw.multiSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the multiSelect.</param>
                    /// <returns type="Csw.components.multiSelect">A Csw.components.multiSelect</returns>
                    opts = internal.controlPreProcessing(opts, 'multiSelect');
                    return Csw.components.multiSelect(cswParent, opts);
                };

                external.nodeTypeSelect = function (opts) {
                    /// <summary> Creates a Csw.numberTextBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the numberTextBox.</param>
                    /// <returns type="Csw.components.nodeTypeSelect">A Csw.components.nodeTypeSelect</returns>
                    opts = internal.controlPreProcessing(opts, 'nodeTypeSelect');
                    return Csw.components.nodeTypeSelect(cswParent, opts);
                };

                external.thinGrid = function (opts) {
                    /// <summary> Creates a Csw.thinGrid on this element</summary>
                    /// <param name="opts" type="Object">Options to define the thinGrid.</param>
                    /// <returns type="Csw.components.thinGrid">A Csw.components.thinGrid</returns>
                    opts = internal.controlPreProcessing(opts, 'thinGrid');
                    return Csw.components.thinGrid(cswParent, opts);
                };

                external.timeInterval = function (opts) {
                    /// <summary> Creates a Csw.timeInterval on this element</summary>
                    /// <param name="opts" type="Object">Options to define the timeInterval.</param>
                    /// <returns type="Csw.components.timeInterval">A Csw.components.timeInterval</returns>
                    opts = internal.controlPreProcessing(opts, 'timeInterval');
                    return Csw.components.timeInterval(cswParent, opts);
                };

                external.triStateCheckBox = function (opts) {
                    /// <summary> Creates a Csw.triStateCheckBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the triStateCheckBox.</param>
                    /// <returns type="Csw.components.triStateCheckBox">A Csw.components.triStateCheckBox</returns>
                    opts = internal.controlPreProcessing(opts, 'triStateCheckBox');
                    return Csw.components.triStateCheckBox(cswParent, opts);
                };

                external.viewSelect = function (opts) {
                    /// <summary> Creates a Csw.viewSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the viewSelect.</param>
                    /// <returns type="Csw.components.viewSelect">A Csw.components.viewSelect</returns>
                    opts = internal.controlPreProcessing(opts, 'viewSelect');
                    return Csw.components.viewSelect(cswParent, opts);
                };

                //#endregion Csw DOM classes

                return external;
            });


} ());


