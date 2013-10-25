
(function () {


    Csw.actions.register('explorer', function (cswParent, cswPrivate) {
        'use strict';
        var cswPublic = {};

        if (Csw.isNullOrEmpty(cswParent)) {
            Csw.error.throwException('Cannot create an action without a valid Csw Parent object.', 'Csw.actions.template', '_template.js', 10);
        }

        (function _preCtor() {
            //set default values on cswPrivate if none are supplied
            cswPrivate.name = cswPrivate.name || 'Explorer';
            cswPrivate.onSubmit = cswPrivate.onSubmit || function _onSubmit() {
            };
            cswPrivate.onCancel = cswPrivate.onCancel || function _onCancel() {
            };

            cswPrivate.startingNodeId = cswPrivate.startingNodeId;
            cswPrivate.sys = null;
            cswPrivate.extPanel = null;
            cswPrivate.maxDept = 2;

            cswParent.empty();
        }());

        cswPrivate.initPanel = function () {

            cswPrivate.extPanel = window.Ext.create('Ext.panel.Panel', {
                renderTo: cswPrivate.mainDiv.getId(),
                height: 650,
                header: false,
                layout: 'border',
                items: [{
                    title: 'Filters',
                    region: 'south',
                    xtype: 'panel',
                    height: 100,
                    items: [{
                        xtype: 'slider',
                        hideLabel: false,
                        fieldLabel: 'Depth',
                        width: 400,
                        minValue: 1,
                        maxValue: 4,
                        value: cswPrivate.maxDept,
                        increment: 1,
                        listeners: {
                            'change': function (slider, newValue, thumb, eOpts) {
                                cswPrivate.maxDept = newValue;
                                cswPrivate.initSystem();
                            }
                        }
                    }]
                }, {
                    title: 'Properties',
                    region: 'east',
                    xtype: 'panel',
                    width: 300,
                    id: 'west-region-container',
                }, {
                    title: 'Relationships',
                    region: 'center',
                    xtype: 'panel',
                    items: {
                        xtype: 'box',
                        autoEl: {
                            tag: 'canvas',
                            height: 510,
                            width: 1250
                        }
                    }
                }]
            });
        };

        cswPrivate.initSystem = function () {
            cswPrivate.sys = arbor.ParticleSystem(50, 300, 0.7);
            cswPrivate.sys.parameters({ gravity: true });

            if (Csw.isNullOrEmpty(cswPrivate.startingNodeId)) {
                cswPrivate.startingNodeId = 'nodes_2';
            }

            Csw.ajaxWcf.post({
                urlMethod: "Explorer/Initialize",
                data: {
                    Depth: cswPrivate.maxDept,
                    NodeId: cswPrivate.startingNodeId
                },
                success: function (response) {
                    var i = 3;
                    var graph = {};

                    Csw.iterate(response.Nodes, function (arborNode) {
                        var secretCell = cswPrivate.actionTbl.cell(1, i);
                        var img = secretCell.img({
                            src: arborNode.Data.Icon
                        }).hide();
                        arborNode.Data.iconId = img.getId();
                        i++;

                        cswPrivate.sys.addNode(arborNode.NodeId, arborNode.Data);
                        graph[arborNode.NodeId] = [];
                    });

                    Csw.iterate(response.Edges, function (arborEdge) {
                        graph[arborEdge.OwnerNodeId].push(arborEdge.TargetNodeId);
                        graph[arborEdge.TargetNodeId].push(arborEdge.OwnerNodeId);
                        cswPrivate.sys.addEdge(arborEdge.OwnerNodeId, arborEdge.TargetNodeId, arborEdge.Data);
                    });

                    cswPrivate.sys.renderer = Csw.ArborRenderer(cswPrivate.extPanel.items.items[2].items.items[0].el.dom.id, response.Nodes, response.Edges, graph, '2');
                }
            });
        };

        (function _postCtor() {

            cswPrivate.action = Csw.layouts.action(cswParent, {
                title: 'Explorer',
                finishText: 'Exit',
                onCancel: cswPrivate.onCancel
            });

            cswPrivate.actionTbl = cswPrivate.action.actionDiv.table({
                name: cswPrivate.name + '_tbl',
                align: 'center'
            }).css('width', '95%');

            cswPrivate.mainDiv = cswPrivate.actionTbl.cell(1, 1).div();

            cswPrivate.initPanel();
            cswPrivate.initSystem();
        }());

        return cswPublic;
    });
}());