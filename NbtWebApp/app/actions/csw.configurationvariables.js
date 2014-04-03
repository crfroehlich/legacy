/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


   Csw.actions.register('configurationvariables', function (cswParent, options) {
        'use strict';
        var cswPrivate = {
            name: 'action_config_vars',
            modules: {},
            onConfigVarChange: null // function() {}
        };
        if (options) Csw.extend(cswPrivate, options);

        cswPrivate.update = function (module) {
            Csw.ajaxWcf.post({
                urlMethod: 'Modules/HandleModule',
                data: module,
                success: function (response) {
                    cswPrivate.render(response);
                    Csw.tryExec(cswPrivate.onConfigVarChange);
                }
            });
        };

        cswPrivate.render = function (response) {
            cswPrivate.modules = {};
            cswPrivate.div.empty();
            cswPrivate.div.css({
                width: '500px'
            });

            var modulesTree = cswParent.tree({
                root: response.Modules,
                expandAll: true,
                title: 'Config Vars',
                useCheckboxes: true,
                rootVisible: false,
                forceSelected: false,
                width: 700,
                height: 700,
                onAfterCheckNode: function (node) {
                    modulesTree.tree.destroy();
                    cswPrivate.update(node.raw);
                }
            });
        };

        // constructor
        cswPrivate.init = function () {

            cswParent.$.empty();

            cswPrivate.div = cswParent.div();

            return Csw.ajaxWcf.post({
                urlMethod: 'ConfigurationVariables/Initialize',
                success: function (response) {
                    cswPrivate.render(response);
                }
            });
        }; // cswPrivate.init()

        return cswPrivate.init();

    }); // register()
}());
