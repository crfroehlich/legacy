/* jshint undef: true, unused: true */
/* global n$:true, window:true, Ext:true, $: true */

(function _selectionModelClassIIFE(n$) {

    var gridSelectionMode = Object.create(null);
    gridSelectionMode.simple = 'SIMPLE';
    gridSelectionMode.single = 'SINGLE';
    gridSelectionMode.multi = 'MULTI';
    n$.constant(n$.grids, 'selectionMode', gridSelectionMode);
    
    /**
     * Internal class to define a Selection DataModel. This class cannot be directly instanced.
     */
    var SelectionModel = function(mode, checkOnly, onSelect, onDeselect) {
        if (!(n$.grids.constants.selectionMode.has(mode))) {
            throw new Error('Grid selection Selection Model does not support mode "' + mode + '".');
        }
        var that = this;
        n$.property(that, 'mode', mode);
        n$.property(that, 'checkOnly', checkOnly);

        //Until we need more subscribers on the Selection DataModel, let's define them ad hoc.
        //This'll be right until it isn't.
        if (onSelect || onDeselect) {
            n$.property(that, 'subscribers', {});
            if (onSelect) {
                n$.property(that.subscribers, 'select', onSelect);
            }
            if (onDeselect) {
                n$.property(that.subscribers, 'deselect', onDeselect);
            }
        }

        n$.property(that, 'ExtSelModel', Ext.create('Ext.selection.CheckboxModel', that));

        return that;
    };

    n$.instanceOf.register('SelectionModel', SelectionModel);

    /**
     * Instance a new Selection Model. Selection Models are the constraints upon which elements from grids can be selected.
     * @param selDef {Object} Object describing the Selection Model
     */
    n$.stores.register('selectionModel', function(selDef) {
        if (!selDef) {
            throw new Error('Cannot create a selection Selection Model without a definition.');
        }
        selDef.mode = selDef.mode || n$.grids.constants.selectionMode.simple;
        var ret = new SelectionModel(selDef.mode, selDef.checkOnly, selDef.onSelect, selDef.onDeselect);
        
        return ret;
    });

}(window.$nameSpace$));
