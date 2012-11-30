/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.nodeSelect = Csw.controls.nodeSelect ||
        Csw.controls.register('nodeSelect', function (cswParent, cswPrivate) {
            'use strict';
            
            //#region _preCtor

            var cswPublic = {};

            (function _preCtor() {
                cswPrivate.$parent = cswPrivate.$parent || cswParent.$;
                cswPrivate.name = cswPrivate.name || '';
                cswPrivate.async = cswPrivate.async; // || true;
                cswPrivate.nodesUrlMethod = cswPrivate.nodesUrlMethod || 'Nodes/get';

                cswPrivate.labelText = cswPrivate.labelText || null;
                cswPrivate.excludeNodeTypeIds = cswPrivate.excludeNodeTypeIds || '';
                cswPrivate.selectedNodeId = cswPrivate.selectedNodeId || '';
                cswPrivate.viewid = cswPrivate.viewid || '';

                cswPrivate.nodeTypeId = cswPrivate.nodeTypeId || '';
                cswPrivate.objectClassId = cswPrivate.objectClassId || '';
                cswPrivate.objectClassName = cswPrivate.objectClassName || '';
                cswPrivate.addNodeDialogTitle = cswPrivate.addNodeDialogTitle || '';
                
                cswPrivate.relatedTo = cswPrivate.relatedTo || {};
                cswPrivate.relatedTo.relatednodeid = cswPrivate.relatedTo.relatednodeid || '';
                cswPrivate.relatedTo.relatednodename = cswPrivate.relatedTo.relatednodename || '';
                cswPrivate.relatedTo.relatednodetypeid = cswPrivate.relatedTo.relatednodetypeid || '';
                cswPrivate.relatedTo.relatedobjectclassid = cswPrivate.relatedTo.relatedobjectclassid || '';
                
                cswPrivate.cellCol = cswPrivate.cellCol || 1;
                cswPrivate.width = cswPrivate.width || '200px';
                
                cswPrivate.onSelectNode = cswPrivate.onSelectNode || function() {};
                cswPrivate.onSuccess = cswPrivate.onSuccess || function () { };
                
                cswPrivate.addNewOption = cswPrivate.addNewOption; // || false;
                cswPrivate.allowAdd = cswPrivate.allowAdd; // || false;
                cswPrivate.isRequired = cswPrivate.isRequired; // || false;
                cswPrivate.isMulti = cswPrivate.isMulti; // || false;
                cswPrivate.isReadOnly = cswPrivate.isReadOnly; // || false;
                cswPrivate.showSelectOnLoad = cswPrivate.showSelectOnLoad; // || true;
                
                cswPrivate.options = cswPrivate.options|| [];
                
                cswPrivate.table = cswParent.table();

                // Default to selected node as relationship value for new nodes being added
                if (false === Csw.isNullOrEmpty(cswPrivate.relatedTo.relatednodeid) &&
                    Csw.isNullOrEmpty(cswPrivate.selectedNodeId) &&
                    false === cswPrivate.isMulti &&
                    (Csw.number(cswPrivate.relatedTo.relatednodetypeid) === Csw.number(cswPrivate.nodeTypeId) ||
                      Csw.number(cswPrivate.relatedTo.relatedobjectclassid) === Csw.number(cswPrivate.objectClassId))) {

                    cswPrivate.selectedNodeId = cswPrivate.relatedTo.relatednodeid;
                    cswPrivate.selectedName = cswPrivate.relatedTo.relatednodename;
                }

                cswPrivate.relationships = [];
            }());
            
            //#endregion _preCtor

            //#region AJAX

            cswPrivate.getNodes = function() {
                
                Csw.ajaxWcf.post({
                    urlMethod: cswPrivate.nodesUrlMethod,
                    async: Csw.bool(cswPrivate.async),
                    data: {
                        NodeTypeId: Csw.number(cswPrivate.nodeTypeId, 0),
                        ObjectClassId: Csw.number(cswPrivate.objectClassId, 0),
                        ObjectClass: Csw.string(cswPrivate.objectClassName),
                        RelatedToObjectClass: Csw.string(cswPrivate.relatedTo.objectClassName),
                        RelatedToNodeId: Csw.string(cswPrivate.relatedTo.nodeId),
                        ViewId: Csw.string(cswPrivate.viewid)
                    },
                    success: function (data) {
                        var options = [];
                        data.Nodes.forEach(function(obj) {
                            options.push({id: obj.NodeId, value: obj.NodeName});
                        });
                        cswPrivate.options = options;
                        cswPrivate.canAdd = Csw.bool(cswPrivate.canAdd) && Csw.bool(data.CanAdd);
                        cswPrivate.useSearch = Csw.bool(data.UseSearch);
                        cswPrivate.nodeTypeId = cswPrivate.nodeTypeId || data.NodeTypeId;
                        cswPrivate.objectClassId = cswPrivate.objectClassId || data.ObjectClassId;
                        cswPrivate.relatedTo.objectClassId = cswPrivate.relatedTo.objectClassId || data.RelatedToObjectClassId;

                        Csw.tryExec(cswPrivate.onSuccess, data);
                        
                        cswPrivate.makeControl();
                    }
                });
            };

            cswPrivate.getNodeTypeOptions = function () {
                cswPrivate.blankText = '[Select One]';
                cswPrivate.selectedNodeType = cswPrivate.selectedNodeType ||
                    cswPrivate.table.cell(1, cswPrivate.cellCol)
                             .nodeTypeSelect({
                                 objectClassId: cswPrivate.objectClassId,
                                 onSelect: function () {
                                     if (cswPrivate.blankText !== cswPrivate.selectedNodeType.val()) {
                                         cswPrivate.nodeTypeId = cswPrivate.selectedNodeType.val();
                                         cswPrivate.openAddNodeDialog(cswPrivate.nodeTypeId);
                                     }
                                 },
                                 onSuccess: function (data, nodeTypeCount, lastNodeTypeId) {
                                     if (Csw.number(nodeTypeCount) > 1) {
                                         cswPrivate.selectedNodeType.show();
                                         cswPrivate.addImage.hide();
                                     } else {
                                         cswPrivate.nodeTypeId = lastNodeTypeId;
                                         cswPrivate.selectedNodeType.hide();
                                         cswPrivate.openAddNodeDialog(cswPrivate.nodeTypeId);
                                     }
                                 },
                                 blankOptionText: cswPrivate.blankText,
                                 filterToPermission: 'Create'
                             }).hide();
                cswPrivate.cellCol += 1;
            };

            //#endregion AJAX

            //#region Control Construction

            cswPrivate.makeControl = function() {
                if (cswPrivate.useSearch) {
                    cswPrivate.makeSearch();
                } else {
                    cswPrivate.makeSelect();
                }
                cswPrivate.makeAdd();
            };

            cswPrivate.makeSelect = function() {
                // Select value in a selectbox
                cswPrivate.foundSelected = false;

                Csw.each(cswPrivate.options, function (relatedObj) {
                    if (false === cswPrivate.isMulti && relatedObj.id === cswPrivate.selectedNodeId) {
                        cswPrivate.foundSelected = true;
                    }
                    cswPrivate.relationships.push({ value: relatedObj.id, display: relatedObj.value });
                });
                if (false === cswPrivate.isMulti && false === cswPrivate.foundSelected) {
                    // case 25820 - guarantee selected option appears
                    cswPrivate.relationships.push({ value: cswPrivate.selectedNodeId, display: cswPrivate.selectedName });
                }

                cswPublic = cswPrivate.table.cell(1, cswPrivate.cellCol).select({
                    name: cswPrivate.name,
                    cssclass: 'selectinput',
                    onChange: function () {
                        var val = cswPublic.val();
                        Csw.tryExec(cswPrivate.onSelectNode, { nodeid: val });
                    },
                    values: cswPrivate.relationships,
                    selected: cswPrivate.selectedNodeId
                });

                cswPublic.bind('change', function () {
                    cswPrivate.selectedNodeId = cswPublic.val();
                    Csw.tryExec(cswPrivate.onChange, cswPublic);
                    Csw.tryExec(cswPrivate.onSelect, cswPublic.val());
                });
                
                cswPrivate.cellCol += 1;
                cswPrivate.nodeLinkText = cswPrivate.table.cell(1, cswPrivate.cellCol);
                cswPrivate.cellCol += 1;
                if (false === cswPrivate.isMulti) {
                    cswPrivate.nodeLinkText = cswPrivate.nodeLinkText.nodeLink({
                        text: cswPrivate.selectedNodeLink
                    });
                }

                cswPrivate.toggleButton = cswPrivate.table.cell(1, cswPrivate.cellCol).icon({
                    iconType: Csw.enums.iconType.pencil,
                    isButton: true,
                    onClick: function () {
                        cswPrivate.toggleOptions(true);
                    }
                });
                cswPrivate.cellCol += 1;
                
                cswPrivate.toggleOptions(cswPrivate.showSelectOnLoad);

                cswPublic.required(cswPrivate.isRequired);

                cswPrivate.nodeLinkText.$.hover(function (event) { Csw.nodeHoverIn(event, cswPublic.val()); },
                                function (event) { Csw.nodeHoverOut(event, cswPublic.val()); });
            };

            cswPrivate.makeSearch = function() {
                if (cswPrivate.useSearch) {
                    // Find value by using search in a dialog

                    cswPrivate.nameSpan = cswPrivate.table.cell(1, cswPrivate.cellCol).span({
                        name: 'selectedname',
                        text: cswPrivate.selectedName
                    });

                    cswPrivate.hiddenValue = cswPrivate.table.cell(1, cswPrivate.cellCol).input({
                        name: 'hiddenvalue',
                        type: Csw.enums.inputTypes.hidden,
                        value: cswPrivate.selectedNodeId
                    });
                    cswPrivate.cellCol += 1;

                    cswPrivate.table.cell(1, cswPrivate.cellCol).icon({
                        iconType: Csw.enums.iconType.magglass,
                        hovertext: "Search " + cswPrivate.name,
                        size: 16,
                        isButton: true,
                        onClick: function() {
                            $.CswDialog('SearchDialog', {
                                propname: cswPrivate.name,
                                nodetypeid: cswPrivate.nodeTypeId,
                                objectclassid: cswPrivate.objectClassId,
                                onSelectNode: function(nodeObj) {
                                    cswPrivate.nameSpan.text(nodeObj.nodename);
                                    cswPrivate.hiddenValue.val(nodeObj.nodeid);
                                    Csw.tryExec(cswPrivate.onSelectNode, nodeObj);
                                }
                            });
                        }
                    });
                    cswPrivate.cellCol += 1;

                    cswPrivate.nameSpan.$.hover(function(event) { Csw.nodeHoverIn(event, cswPrivate.hiddenValue.val()); },
                        function(event) { Csw.nodeHoverOut(event, cswPrivate.hiddenValue.val()); });
                }
            };

            cswPrivate.toggleOptions = function (on) {
                if (Csw.bool(on)) {
                    cswPublic.show();
                    cswPrivate.toggleButton.hide();
                    cswPrivate.nodeLinkText.hide();
                } else {
                    cswPublic.hide();
                    cswPrivate.toggleButton.show();
                    cswPrivate.nodeLinkText.show();
                }
            };

            //#endregion Control Construction

            //#region Add

            cswPrivate.onAddNodeFunc = function (nodeid, nodekey, nodename) {
                if (cswPrivate.nameSpan) {
                    cswPrivate.nameSpan.text(nodename);
                }
                if (cswPrivate.hiddenValue) {
                    cswPrivate.hiddenValue.val(nodeid);
                }
                if (cswPublic) {
                    cswPublic.option({ value: nodeid, display: nodename });
                    cswPublic.val(nodeid);
                    cswPrivate.toggleOptions(true);
                    Csw.tryExec(cswPrivate.onSelectNode, { nodeid: nodeid });
                    cswPublic.$.valid();
                }
            };

            cswPrivate.openAddNodeDialog = function (nodetypeToAdd) {
                $.CswDialog('AddNodeDialog', {
                    nodetypeid: nodetypeToAdd,
                    onAddNode: cswPrivate.onAddNodeFunc,
                    text: cswPrivate.name
                });
            };
            
            cswPrivate.makeAddImage = function () {
                cswPrivate.addImage = cswPrivate.table.cell(1, cswPrivate.cellCol).div()
                    .buttonExt({
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                        size: 'small',
                        tooltip: { title: 'Add New ' + cswPrivate.name },
                        onClick: function () {
                            if (Csw.number(cswPrivate.nodeTypeId) > 0) {
                                cswPrivate.openAddNodeDialog(cswPrivate.nodeTypeId);
                            }
                            else {
                                cswPrivate.getNodeTypeOptions();
                            }
                        }
                    });
                cswPrivate.cellCol += 1;
            };

            cswPrivate.makeAdd = function() {
                if (cswPrivate.allowAdd) {
                    cswPrivate.makeAddImage();
                } //if (allowAdd)
            };

            //#endregion Add

            //#region Public
            
            cswPublic.selectedNodeId = function () {
                if(cswPublic && cswPublic.val) {
                    cswPrivate.selectedNodeId = cswPublic.val();
                }
                return cswPrivate.selectedNodeId;
            };

            //#endregion Public

            //#region _postCtor

            (function _relationship() {
                if (cswPrivate.isReadOnly) {
                    cswPrivate.nodeLinkTextCell = cswPrivate.table.cell(1, cswPrivate.cellCol);
                    cswPrivate.nodeLinkText = cswPrivate.nodeLinkTextCell.nodeLink({
                        text: cswPrivate.selectedNodeLink
                    });
                    cswPrivate.parent.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.selectedNodeId); },
                                    function (event) { Csw.nodeHoverOut(event, cswPrivate.selectedNodeId); });
                } else {
                    if (cswPrivate.options.length > 0) {
                        cswPrivate.makeControl();
                    } else {
                        cswPrivate.getNodes();
                    }
                } // if-else (o.ReadOnly) {
            }());

            return cswPublic;
            
            //#endregion _postCtor
        });
} ());

