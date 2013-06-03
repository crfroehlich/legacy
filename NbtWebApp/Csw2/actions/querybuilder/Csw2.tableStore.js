/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

/**
* The Fields Store represents the data bound to a grid
*/
(function _joinsStoreIIFE() {

    //Csw2.dependsOn(['Csw2.fieldsModel'], function () {

    /**
     * Define the proxy
    */
    var proxy = Csw2.grids.stores.proxy('memory');

    /**
     * Define the store
    */
    var store = Csw2.grids.stores.store('Ext.Csw2.SqlTableStore', proxy, 'Ext.Csw2.SqlTableModel');

    /**
     * Create the ExtJs class
    */
    var sqlTableStore = store.init();

    /**
     * Put the class into the namespace
    */
    Csw2.lift('sqlTableStore', sqlTableStore);

    // });

}());