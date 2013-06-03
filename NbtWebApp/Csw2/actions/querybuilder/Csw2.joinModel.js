/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function () {

    var fields = Csw2.grids.fields.fields();
    fields.add(Csw2.grids.fields.field('id', 'string'))
          .add(Csw2.grids.fields.field('leftTableId', 'string'))
          .add(Csw2.grids.fields.field('rightTableId', 'string'))
          .add(Csw2.grids.fields.field('leftTableField', 'string'))
          .add(Csw2.grids.fields.field('rightTableField', 'string'))
          .add(Csw2.grids.fields.field('joinCondition', 'string'))
          .add(Csw2.grids.fields.field('joinType', 'boolean'));

    var fieldDef = Csw2.classDefinition({
        name: 'Ext.Csw2.SQLJoin',
        extend: 'Ext.data.Model',
        onDefine: function (def) {
            def.fields = fields.value;
            delete def.initComponent;
        }
    });
    

    /**
     * Instance a collection of fields to describe a JOIN in the SQL output table
    */

    var joinModel = fieldDef.init();

    Csw2.lift('joinModel', joinModel);
}());