/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.assigninventorygroups = Csw.actions.assigninventorygroups ||
        Csw.actions.register('assigninventorygroups', function (cswParent, options) {
            'use strict';
            var o = {
                saveUrlMethod: 'saveassigninventorygroups',
                name: 'action_assigninventorygroups',
                actionjson: null,
                onQuotaChange: null // function () { }
            };

            if (options) {
                Csw.extend(o, options);
            }

            o.action = Csw.layouts.action( cswParent, {
                title: 'Assign Inventory Group To Locations',
                useFinish: false,
                useCancel: false
                } );

    
            o.action.actionDiv.css( { padding: '10px' } ); 
            o.action.actionDiv.append( "You can assign the selected <b>Inventory Group</b> to any location(s). Just click the locations' checkbox, and then click <b>Set To</b>.<BR><BR>" ); 

            //HTML table kung-fu
            var action_table = o.action.actionDiv.table();

            //action_table.p
//            debugger;
//            action_table.propDom( 'border', '1'); 
//            action_table.css({ width: '100%' }); 
            var tree_cell = action_table.cell(2, 1);
            //tree_cell.css( {'width' : '1500px' } ); 

             
            var include_children_table = action_table.cell(1, 1).table();
            action_table.cell(1, 1).css( { 'vertical-align' : 'bottom' } );
            var include_children_checkbox_label_cell = include_children_table.cell(1, 1);
            var include_children_checkbox_cell = include_children_table.cell(1, 2);


            var close_button_cell = action_table.cell(3, 1);

            var right_side_table = action_table.cell(1, 2).table();
            action_table.cell(1, 2).propDom( 'rowspan', 2 ); 
            action_table.cell(1, 2).css( { 'vertical-align' : 'top' } );

            //cells for labels (row must correspond to control cell in next group)
//            var inventory_group_label_cell = right_side_table.cell(1, 1);
//            var storage_compatability_label_cell = right_side_table.cell(2, 1);
//            var allow_inventory_label_cell = right_side_table.cell(3, 1);
//            var control_zone_label_cell = right_side_table.cell(4, 1);

            //cells for controls
//            var select_inventory_group_cell = right_side_table.cell(1, 2);
//            var select_storage_compatability_cell = right_side_table.cell(2, 2);
//            var select_allow_inventory_cell = right_side_table.cell(3, 2);
//            var select_control_zone_cell = right_side_table.cell(4, 2);
            var monster_controls_cell = right_side_table.cell(1, 1);
            var save_button_cell = right_side_table.cell(2, 1);
            save_button_cell.css( { 'padding-left' : '400px' } );


            var mainTree = null;
            var inventory_group_select = null;
            var storage_compatability_select = null;
            var monster_properties = null; 
            var allow_inventory_select = null;
            var control_zone_select = null;

            var check_children_of_current_check_box = null;

            function initTree() {

                Csw.ajaxWcf.get({
                    urlMethod: "Trees/locations",
                    success: function (data) {
                        tree_cell.empty();
                        mainTree = Csw.nbt.nodeTreeExt(tree_cell, {
                            width: 500,
                            overrideBeforeSelect: true,
                            ExpandAll: true,
                            forSearch: false,
                            PropsToShow: ['inventory group'],
                            onBeforeSelectNode: function ( node , tree ) 
                            { 

                                if( null != check_children_of_current_check_box && true == check_children_of_current_check_box.checked() ) 
                                {
                                    if( false == node.raw.checked ) //in other words, we are now toggling it to checked :-( 
                                    {
                                        mainTree.nodeTree.checkChildrenOfNode( node , true );

                                    } else 
                                    {
                                        mainTree.nodeTree.checkChildrenOfNode( node , false );
                                    }//if the client says to check children of checked node

                                }//if the next state of the node is checked

                                return (false);  //allow selection of multiple node types
                            }, 
                            isMulti: true, //checkboxes
                            state: {
                                viewId: data.NewViewId,
                                viewMode: "tree",
                                includeInQuickLaunch: false
                            }
                        });
                        

                    } //success
                }); //ajaxget

                
            } //initTree()

            function initPropValSelectors() {
            
                    var selected_node_id = null;
                    if( ( null !== o.actionjson ) && ( null !== o.actionjson.ivgnodeid ) ) 
                    {
                        selected_node_id = o.actionjson.ivgnodeid;
                    }

                    //Labels
//                    inventory_group_label_cell.span({ text: 'Inventory Group:' }).addClass('propertylabel');
//                    storage_compatability_label_cell.span({ text: 'Storage Compatability:' }).addClass('propertylabel');
//                    allow_inventory_label_cell.span({ text: 'Allow Inventory:' }).addClass('propertylabel');
//                    control_zone_label_cell.span({ text: 'Control Zone:' }).addClass('propertylabel');
//                


                    //Retrieve the node data for the currently selected node
                    monster_properties = Csw.layouts.tabsAndProps(monster_controls_cell, {
                        tabState: {
                            excludeOcProps: ['name', 'child location type', 'location template', 'location', 'order', 'rows', 'columns', 'barcode', 'location code', 'containers', 'save', 'inventory levels' ],
//                            propertyData: cswPrivate.state.properties,
                            nodeid: 24704,
                            ShowAsReport: false,
                            nodetypeid: 969,
                            EditMode: Csw.enums.editMode.Temp, //sic.
                            showSaveButton: true
                        },
                        ReloadTabOnSave: false,
                        async: false,
                        onPropertyChange: function (propid, propName, propData) {
                            //TODO: This seems like a really bad plan. Why are we doing this?
                            var foo = "";
                            
//                            if (propName === "Physical State") {
//                                //cswPrivate.setPhysicalStateValue();
//                                cswPrivate.physicalStateModified = true;
//                            }
                        } 
                    });
                    

                    /*
                    Csw.ajax.post({
                        watchGlobal: true,
                        urlMethod: 'getProps',
                        data: {
                            EditMode: false,
                            NodeId: 24704 ,
                            TabId: 1100,
                            SafeNodeKey: null,
                            NodeTypeId: 969,
                            Date: null,
                            Multi: true,
                            filterToPropId: null,
                            ConfigMode: false,
                            RelatedNodeId: null,
                            RelatedNodeTypeId: null,
                            RelatedObjectClassId: null,
                            GetIdentityTab: false,
                            ForceReadOnly: false 
                        },
                        success: function (data) {
                            var nu_data = data;
//                            if (Csw.isNullOrEmpty(data) && cswPrivate.tabState.EditMode === Csw.enums.editMode.Edit) {
//                                cswPrivate.onEmptyProps();
//                            }
//                            cswPrivate.setNode(data.node);
//                            cswPrivate.tabState.propertyData = data.properties;
//                            makePropLayout();
                        } // success{}
                    }); // ajax                
                    */

                    /*
                    //Inventory Group Select
                    inventory_group_select = select_inventory_group_cell.span().nodeSelect({
                        name: 'Inventory Group',
                        objectClassName: 'InventoryGroupClass',
                        selectedNodeId: selected_node_id,
                        allowAdd: true,
                        isRequired: true,
                        showSelectOnLoad: true,
                        isMulti: false,
                        onSuccess: function () {
                        }

                    });//nodeSelct()


                    });//nodeSelct()
                */
    

//            var select_storage_compatability_cell = right_side_table.cell(2, 2);
//            var select_allow_inventory_cell = right_side_table.cell(3, 2);
//            var select_control_zone_cell = right_side_table.cell(4, 2);


//            var storage_compatability_select = null;
//            var allow_inventory_select = null;
//            var control_zone_select = null;



                 

            } //initPropValSelectors()

            //check_children_of_current_check_box.checked 
            function initCheckBox() {

                include_children_checkbox_label_cell.empty();
                include_children_checkbox_label_cell.span({ text: 'Include Children:' }).addClass('propertylabel');

                include_children_checkbox_cell.empty();
                check_children_of_current_check_box = include_children_checkbox_cell.input({
                    name: "include_children",
                    type: Csw.enums.inputTypes.checkbox,
                    checked: Csw.bool("false"),
                })
            }; //initCheckBox()

            function initButtons() {

                save_button_cell.buttonExt({
                    name: 'save_action',
                    disableOnClick: false,
                    onClick: function () {

//                        var inventory_group_node_id = inventory_group_select.selectedVal();
                        var selected_locations_node_keys = '';

                        Csw.each( mainTree.checkedNodes() , function ( node ) {
                            if( null !== node.nodeid ) 
                            {
                                selected_locations_node_keys += node.nodekey + ',';
                            }
                        });

                        var AssignRequest = {  
                            LocationNodeKeys : selected_locations_node_keys,
                            SelectedInventoryGroupNodeId : inventory_group_node_id
                        }

                        Csw.ajaxWcf.post({
                            urlMethod: 'Locations/assignInventoryGroupToLocations',
                            data: AssignRequest,
                            success: function (ajaxdata) { 
                                    initCheckBox();
                                    initTree();
                                }
                            });

                    },
                    enabledText: 'Set To'
                });

                close_button_cell.buttonExt({
                    name: 'close_action',
                    disableOnClick: false,
                    onClick: function () {
                        Csw.tryExec(options.onCancel);
                    },
                    enabledText: 'Close'
                });

            } //initButtons() 


            initTree();
            initPropValSelectors();
            initCheckBox();
            initButtons();


            //initTable();
        }); // methods
} ());
