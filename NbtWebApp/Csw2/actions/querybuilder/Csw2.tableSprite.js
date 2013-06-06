///* jshint undef: true, unused: true */
///* global Csw2:true, window:true, Ext:true, $: true */

//(function () {

//    var spriteDef = Csw2.classDefinition({
//        name: 'Ext.Csw2.SqlTableSprite',
//        extend: 'Ext.draw.Sprite',
//        alias: ['widget.sqltablesprite']
            
//    });
    
//    spriteDef.addProp('bConnections', false);
//    spriteDef.addProp('startDrag', function(id) {
//        var me = this,
//            win, qbTablePanel, xyParentPos, xyChildPos;

//        // get a reference to a sqltable
//        win = Ext.getCmp(id);

//        // get the main qbTablePanel
//        qbTablePanel = Ext.getCmp('qbTablePanel');

//        // get the main qbTablePanel position
//        xyParentPos = qbTablePanel.el.getXY();

//        // get the size of the previously added sqltable
//        xyChildPos = win.el.getXY();

//        me.prev = me.surface.transformToViewBox(xyChildPos[0] - xyParentPos[0] + 2, xyChildPos[1] - xyParentPos[1] + 2);
//    });

//    spriteDef.addProp('onDrag', function(relPosMovement) {
//        var xy, me = this,
//            attr = this.attr,
//            newX, newY;
//        // move the sprite
//        // calculate new x and y position
//        newX = me.prev[0] + relPosMovement[0];
//        newY = me.prev[1] + relPosMovement[1];
//        // set new x and y position and redraw sprite
//        me.setAttributes({
//            x: newX,
//            y: newY
//        }, true);
//    });

//    var sprite = spriteDef.init();

//    Csw2.lift('tableSprite', sprite);


//    //   });

//}());


/* global window, Ext:true */

(function () {

    Ext.define('Ext.Csw2.SqlTableSprite', {
        extend: 'Ext.draw.Sprite',
        alias: ['widget.sqltablesprite'],
        bConnections: false,
        startDrag: function (id) {
            var me = this,
                win, qbTablePanel, xyParentPos, xyChildPos;

            // get a reference to a sqltable
            win = Ext.getCmp(id);

            // get the main qbTablePanel
            qbTablePanel = Ext.getCmp('qbTablePanel');

            // get the main qbTablePanel position
            xyParentPos = qbTablePanel.el.getXY();

            // get the size of the previously added sqltable
            xyChildPos = win.el.getXY();

            me.prev = me.surface.transformToViewBox(xyChildPos[0] - xyParentPos[0] + 2, xyChildPos[1] - xyParentPos[1] + 2);
        },

        onDrag: function (relPosMovement) {
            var xy, me = this,
                attr = this.attr,
                newX, newY;
            // move the sprite
            // calculate new x and y position
            newX = me.prev[0] + relPosMovement[0];
            newY = me.prev[1] + relPosMovement[1];
            // set new x and y position and redraw sprite
            me.setAttributes({
                x: newX,
                y: newY

            }, true);
        }
    });

}());