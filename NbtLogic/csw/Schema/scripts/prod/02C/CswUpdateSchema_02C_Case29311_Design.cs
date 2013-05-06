﻿using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29311
    /// </summary>
    public class CswUpdateSchema_02C_Case29311_Design : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29311; }
        }

        public override void update()
        {
            // Clear defunct existing rows from jct_dd_ntp
            CswTableUpdate jctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29311_jctddntp_update", "jct_dd_ntp" );
            DataTable jctClearTable = jctUpdate.getTable();
            foreach( DataRow jctRow in jctClearTable.Rows )
            {
                jctRow.Delete();
            }
            jctUpdate.update( jctClearTable );


            // Create new Super-MetaData Design nodetypes
            CswNbtMetaDataObjectClass NodeTypeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass );
            CswNbtMetaDataObjectClass NodeTypeTabOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass );
            CswNbtMetaDataObjectClass NodeTypePropOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );

            CswNbtMetaDataNodeType NodeTypeNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( NodeTypeOC )
                {
                    NodeTypeName = "Design NodeType",
                    Category = "Design"
                } );
            NodeTypeNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeType.PropertyName.NodeTypeName ) );

            CswNbtMetaDataNodeType NodeTypeTabNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( NodeTypeTabOC )
            {
                NodeTypeName = "Design NodeTypeTab",
                Category = "Design"
            } );
            NodeTypeTabNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeTypeTab.PropertyName.TabName ) );

            foreach( CswNbtMetaDataFieldType FieldType in _CswNbtSchemaModTrnsctn.MetaData.getFieldTypes() )
            {
                CswNbtMetaDataNodeType NodeTypePropNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( NodeTypePropOC )
                    {
                        NodeTypeName = "Design " + FieldType.FieldType.ToString() + " NodeTypeProp",
                        Category = "Design"
                    } );
                NodeTypePropNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName ) );
            }


            CswNbtMetaDataNodeTypeProp NTAuditLevelNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.AuditLevel );
            CswNbtMetaDataNodeTypeProp NTCategoryNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.Category );
            CswNbtMetaDataNodeTypeProp NTDeferSearchToNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.DeferSearchTo );
            CswNbtMetaDataNodeTypeProp NTIconFileNameNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.IconFileName );
            CswNbtMetaDataNodeTypeProp NTLockedNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.Locked );
            CswNbtMetaDataNodeTypeProp NTNameTemplateNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NameTemplate );
            CswNbtMetaDataNodeTypeProp NTNameTemplateAddNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NameTemplateAdd );
            CswNbtMetaDataNodeTypeProp NTNodeTypeNameNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.NodeTypeName );
            //CswNbtMetaDataNodeTypeProp NTObjectClassNameNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.ObjectClassName );
            CswNbtMetaDataNodeTypeProp NTObjectClassValueNTP = NodeTypeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.ObjectClassValue );

            CswNbtMetaDataNodeTypeProp NTTNodeTypeNTP = NodeTypeTabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.NodeTypeValue );
            CswNbtMetaDataNodeTypeProp NTTOrderNTP = NodeTypeTabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.Order );
            CswNbtMetaDataNodeTypeProp NTTIncludeInReportNTP = NodeTypeTabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.IncludeInReport );
            CswNbtMetaDataNodeTypeProp NTTTabNameNTP = NodeTypeTabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.TabName );

            CswNbtMetaDataObjectClassProp NTPNodeTypeOCP = NodeTypePropOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue );
            CswNbtMetaDataObjectClassProp NTPFieldTypeOCP = NodeTypePropOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.FieldType );




            Dictionary<Int32, CswNbtObjClassDesignNodeType> NTNodes = new Dictionary<Int32, CswNbtObjClassDesignNodeType>();

            DataTable jctTable = jctUpdate.getEmptyTable();

            // NodeTypeNT Props
            {
                Int32 TabId = NodeTypeNT.getFirstNodeTypeTab().TabId;

                // Set view for NameTemplateAddNTP
                CswNbtView NameView = _CswNbtSchemaModTrnsctn.restoreView( NTNameTemplateAddNTP.ViewId );
                NameView.Root.ChildRelationships.Clear();
                CswNbtViewRelationship NameViewRel1 = NameView.AddViewRelationship( NodeTypeNT, false );
                CswNbtViewRelationship NameViewRel2 = NameView.AddViewRelationship( NameViewRel1, CswEnumNbtViewPropOwnerType.Second, NTPNodeTypeOCP, false );
                NameView.save();

                // Set view for DeferSearchToNTP
                CswNbtView DeferView = _CswNbtSchemaModTrnsctn.restoreView( NTDeferSearchToNTP.ViewId );
                DeferView.Root.ChildRelationships.Clear();
                CswNbtViewRelationship DeferViewRel1 = DeferView.AddViewRelationship( NodeTypeNT, false );
                CswNbtViewRelationship DeferViewRel2 = DeferView.AddViewRelationship( DeferViewRel1, CswEnumNbtViewPropOwnerType.Second, NTPNodeTypeOCP, false );
                DeferView.AddViewPropertyAndFilter( DeferViewRel2, NTPFieldTypeOCP,
                                                    FilterMode: CswEnumNbtFilterMode.Equals,
                                                    Value: "Relationship" );
                DeferView.AddViewPropertyAndFilter( DeferViewRel2, NTPFieldTypeOCP,
                                                    Conjunction: CswEnumNbtFilterConjunction.Or,
                                                    FilterMode: CswEnumNbtFilterMode.Equals,
                                                    Value: "Location" );
                DeferView.save();

                // Edit Layout
                NTNodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 1 );
                NTObjectClassValueNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 1 );
                NTCategoryNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 3, DisplayColumn: 1 );
                NTIconFileNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 4, DisplayColumn: 1 );
                NTNameTemplateNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 2 );
                NTNameTemplateAddNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 2 );
                NTAuditLevelNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 6, DisplayColumn: 1 );
                NTDeferSearchToNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 7, DisplayColumn: 1 );
                NTLockedNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 8, DisplayColumn: 1 );

                // Add Layout
                NTObjectClassValueNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
                NTNodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
                NTCategoryNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );
                NTIconFileNameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 4, DisplayColumn: 1 );
                NTAuditLevelNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTDeferSearchToNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTLockedNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTNameTemplateNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                NTNameTemplateAddNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

                // Table Layout
                NTNodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
                NTObjectClassValueNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );

                // Preview Layout
                NTNodeTypeNameNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
                NTObjectClassValueNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );


                // Populate nodes
                // Very important that this happens BEFORE we map to the nodetypes table, or else we'll end up duplicating rows!
                foreach( CswNbtMetaDataNodeType thisNodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
                {
                    CswNbtObjClassDesignNodeType node = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NodeTypeNT.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode, true );
                    node.AuditLevel.Value = thisNodeType.AuditLevel;
                    node.Category.Text = thisNodeType.Category;
                    //node.DeferSearchTo.RelatedNodeId = thisNodeType.SearchDeferPropId;
                    node.IconFileName.Value = new CswCommaDelimitedString() { thisNodeType.IconFileName };
                    node.Locked.Checked = CswConvert.ToTristate( thisNodeType.IsLocked );
                    node.NameTemplate.Text = thisNodeType.getNameTemplateText();
                    node.NodeTypeName.Text = thisNodeType.NodeTypeName;
                    //node.ObjectClassName.Text = thisNodeType.getObjectClass().ObjectClass.ToString();
                    node.ObjectClassValue.Value = thisNodeType.ObjectClassId.ToString();
                    node.RelationalId = new CswPrimaryKey( "nodetypes", thisNodeType.NodeTypeId );
                    node.postChanges( false );

                    NTNodes.Add( thisNodeType.NodeTypeId, node );
                }

                // Here's where the extra special super-secret magic comes in

                NodeTypeNT.TableName = "nodetypes";

                _addJctRow( jctTable, NTNodeTypeNameNTP, NodeTypeNT.TableName, "nodetypename" );
                _addJctRow( jctTable, NTObjectClassValueNTP, NodeTypeNT.TableName, "objectclassid" );
                _addJctRow( jctTable, NTCategoryNTP, NodeTypeNT.TableName, "category" );
                _addJctRow( jctTable, NTIconFileNameNTP, NodeTypeNT.TableName, "iconfilename" );
                _addJctRow( jctTable, NTNameTemplateNTP, NodeTypeNT.TableName, "nametemplate" );
                _addJctRow( jctTable, NTAuditLevelNTP, NodeTypeNT.TableName, "auditlevel" );
                _addJctRow( jctTable, NTDeferSearchToNTP, NodeTypeNT.TableName, "searchdeferpropid", CswEnumNbtSubFieldName.NodeID );
                _addJctRow( jctTable, NTLockedNTP, NodeTypeNT.TableName, "islocked" );
            }


            // NodeTypeTabNT Props
            {
                Int32 TabId = NodeTypeTabNT.getFirstNodeTypeTab().TabId;

                // Edit Layout
                NTTTabNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 1 );
                NTTNodeTypeNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 1 );
                NTTOrderNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 3, DisplayColumn: 1 );
                NTTIncludeInReportNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 4, DisplayColumn: 1 );

                // Add Layout
                NTTTabNameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
                NTTNodeTypeNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
                NTTOrderNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );
                NTTIncludeInReportNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

                // Table Layout
                NTTTabNameNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
                NTTNodeTypeNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );

                // Preview Layout
                NTTTabNameNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
                NTTNodeTypeNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );

                // Populate nodes
                // Very important that this happens BEFORE we map to the nodetype_tabset table, or else we'll end up duplicating rows!
                foreach( CswNbtMetaDataNodeType thisNodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
                {
                    foreach( CswNbtMetaDataNodeTypeTab thisTab in thisNodeType.getNodeTypeTabs() )
                    {
                        CswNbtObjClassDesignNodeTypeTab node = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NodeTypeTabNT.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode, true );
                        node.IncludeInReport.Checked = CswConvert.ToTristate( thisTab.IncludeInNodeReport );
                        node.NodeTypeValue.RelatedNodeId = NTNodes[thisNodeType.NodeTypeId].NodeId;
                        node.Order.Value = thisTab.TabOrder;
                        node.TabName.Text = thisTab.TabName;
                        node.RelationalId = new CswPrimaryKey( "nodetype_tabset", thisTab.TabId );
                        node.postChanges( false );
                    }
                }

                // Here's where the extra special super-secret magic comes in

                NodeTypeTabNT.TableName = "nodetype_tabset";

                _addJctRow( jctTable, NTTIncludeInReportNTP, NodeTypeTabNT.TableName, "includeinnodereport" );
                _addJctRow( jctTable, NTTNodeTypeNTP, NodeTypeTabNT.TableName, "nodetypeid", CswEnumNbtSubFieldName.NodeID );
                _addJctRow( jctTable, NTTOrderNTP, NodeTypeTabNT.TableName, "taborder" );
                _addJctRow( jctTable, NTTTabNameNTP, NodeTypeTabNT.TableName, "tabname" );
            }



            // NodeTypePropNT Props
            {
                Dictionary<CswEnumNbtFieldType, CswNbtMetaDataNodeType> propNTDict = new Dictionary<CswEnumNbtFieldType, CswNbtMetaDataNodeType>();

                foreach( CswNbtMetaDataFieldType FieldType in _CswNbtSchemaModTrnsctn.MetaData.getFieldTypes() )
                {
                    CswNbtMetaDataNodeType NodeTypePropNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Design " + FieldType.FieldType.ToString() + " NodeTypeProp" );

                    Int32 TabId = NodeTypePropNT.getFirstNodeTypeTab().TabId;

                    CswNbtMetaDataNodeTypeProp NTPAuditLevelNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.AuditLevel );
                    CswNbtMetaDataNodeTypeProp NTPCompoundUniqueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.CompoundUnique );
                    CswNbtMetaDataNodeTypeProp NTPDisplayConditionFilterNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionFilter );
                    CswNbtMetaDataNodeTypeProp NTPDisplayConditionPropertyNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionProperty );
                    CswNbtMetaDataNodeTypeProp NTPDisplayConditionSubfieldNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionSubfield );
                    CswNbtMetaDataNodeTypeProp NTPDisplayConditionValueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionValue );
                    CswNbtMetaDataNodeTypeProp NTPFieldTypeNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.FieldType );
                    CswNbtMetaDataNodeTypeProp NTPHelpTextNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.HelpText );
                    CswNbtMetaDataNodeTypeProp NTPNodeTypeValueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue );
                    CswNbtMetaDataNodeTypeProp NTPObjectClassPropNameNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.ObjectClassPropName );
                    CswNbtMetaDataNodeTypeProp NTPPropNameNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName );
                    CswNbtMetaDataNodeTypeProp NTPReadOnlyNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.ReadOnly );
                    CswNbtMetaDataNodeTypeProp NTPRequiredNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.Required );
                    CswNbtMetaDataNodeTypeProp NTPUniqueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.Unique );
                    CswNbtMetaDataNodeTypeProp NTPUseNumberingNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.UseNumbering );

                    // Edit layout
                    NTPPropNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 1, DisplayColumn: 1 );
                    NTPObjectClassPropNameNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 2, DisplayColumn: 1 );
                    NTPFieldTypeNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 3, DisplayColumn: 1 );
                    NTPNodeTypeValueNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 4, DisplayColumn: 1 );
                    NTPDisplayConditionPropertyNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 5, DisplayColumn: 1 );
                    NTPDisplayConditionSubfieldNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 6, DisplayColumn: 1 );
                    NTPDisplayConditionFilterNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 7, DisplayColumn: 1 );
                    NTPDisplayConditionValueNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 8, DisplayColumn: 1 );
                    NTPRequiredNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 9, DisplayColumn: 1 );
                    NTPUniqueNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 10, DisplayColumn: 1 );
                    NTPCompoundUniqueNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 11, DisplayColumn: 1 );
                    NTPReadOnlyNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 12, DisplayColumn: 1 );
                    NTPUseNumberingNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 13, DisplayColumn: 1 );
                    NTPHelpTextNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 14, DisplayColumn: 1 );
                    NTPAuditLevelNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId, DisplayRow: 15, DisplayColumn: 1 );

                    // Add layout
                    NTPNodeTypeValueNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
                    NTPPropNameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
                    NTPFieldTypeNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );
                    NTPAuditLevelNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPCompoundUniqueNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPDisplayConditionFilterNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPDisplayConditionPropertyNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPDisplayConditionSubfieldNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPDisplayConditionValueNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPHelpTextNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPObjectClassPropNameNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPReadOnlyNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPRequiredNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPUniqueNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                    NTPUseNumberingNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

                    // Table layout
                    NTPNodeTypeValueNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
                    NTPPropNameNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );
                    NTPObjectClassPropNameNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 3, DisplayColumn: 1 );
                    NTPFieldTypeNTP.updateLayout( CswEnumNbtLayoutType.Table, true, DisplayRow: 4, DisplayColumn: 1 );

                    // Preview layout
                    NTPNodeTypeValueNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
                    NTPPropNameNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );
                    NTPObjectClassPropNameNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 3, DisplayColumn: 1 );
                    NTPFieldTypeNTP.updateLayout( CswEnumNbtLayoutType.Preview, true, DisplayRow: 4, DisplayColumn: 1 );


                    // Set default value of "Field Type" to this fieldtype
                    NTPFieldTypeNTP.DefaultValue.AsList.Value = FieldType.FieldTypeId.ToString();
                    NTPFieldTypeNTP.ServerManaged = true;


                    ICswNbtFieldTypeRule Rule = FieldType.getFieldTypeRule();

                    // Make all the attribute properties
                    foreach( CswNbtFieldTypeAttribute Attr in Rule.getAttributes() )
                    {
                        _makePropNTP( NodeTypePropNT, TabId, Attr );
                    }

                    // Handle special configurations
                    switch( FieldType.FieldType )
                    {
                        case CswEnumNbtFieldType.Composite:
                            CswNbtMetaDataNodeTypeProp addTemplateNTP = _makePropNTP( NodeTypePropNT, TabId, CswEnumNbtFieldType.Relationship, "Add To Template", "" );
                            addTemplateNTP.SetFK( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), NodeTypePropOC.ObjectClassId, string.Empty, Int32.MinValue );

                            CswNbtView addTemplateView = _CswNbtSchemaModTrnsctn.restoreView( addTemplateNTP.ViewId );
                            addTemplateView.Root.ChildRelationships.Clear();
                            CswNbtViewRelationship rel1 = addTemplateView.AddViewRelationship( NodeTypePropNT, false );
                            CswNbtViewRelationship rel2 = addTemplateView.AddViewRelationship( rel1, CswEnumNbtViewPropOwnerType.Second, NTPNodeTypeOCP, false );
                            addTemplateView.save();
                            break;

                        case CswEnumNbtFieldType.DateTime:
                            CswNbtMetaDataNodeTypeProp defaultToTodayNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.DefaultToToday.ToString() );
                            defaultToTodayNTP.IsRequired = true;
                            
                            CswNbtMetaDataNodeTypeProp dateTypeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.DateType.ToString() );
                            dateTypeNTP.ListOptions = new CswCommaDelimitedString()
                                {
                                    CswEnumNbtDateDisplayMode.Date.ToString(),
                                    CswEnumNbtDateDisplayMode.Time.ToString(),
                                    CswEnumNbtDateDisplayMode.DateTime.ToString()
                                }.ToString();
                            break;

                        case CswEnumNbtFieldType.Grid:
                            CswNbtMetaDataNodeTypeProp displaymodeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.DisplayMode.ToString() );
                            displaymodeNTP.ListOptions = new CswCommaDelimitedString()
                                    {
                                        CswEnumNbtGridPropMode.Full.ToString(),
                                        CswEnumNbtGridPropMode.Small.ToString(),
                                        CswEnumNbtGridPropMode.Link.ToString(),
                                    }.ToString();

                            CswNbtMetaDataNodeTypeProp maxrowsNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.MaximumRows.ToString() );
                            maxrowsNTP.setFilter( displaymodeNTP, displaymodeNTP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswEnumNbtGridPropMode.Small.ToString() );

                            CswNbtMetaDataNodeTypeProp showheadersNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.ShowHeaders.ToString() );
                            showheadersNTP.setFilter( displaymodeNTP, displaymodeNTP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswEnumNbtGridPropMode.Small.ToString() );
                            break;

                        case CswEnumNbtFieldType.ImageList:
                            CswNbtMetaDataNodeTypeProp imagenamesNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.ImageNames.ToString() );
                            imagenamesNTP.TextAreaRows = 5;
                            imagenamesNTP.TextAreaColumns = 100;

                            CswNbtMetaDataNodeTypeProp imageurlsNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.ImageUrls.ToString() );
                            imageurlsNTP.TextAreaRows = 5;
                            imageurlsNTP.TextAreaColumns = 100;
                            break;

                        case CswEnumNbtFieldType.Location:
                            CswNbtMetaDataNodeTypeProp locfktypeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.FKType.ToString() );
                            locfktypeNTP.ServerManaged = true;
                            locfktypeNTP.DefaultValue.AsText.Text = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString();
                            locfktypeNTP.removeFromAllLayouts();

                            CswNbtMetaDataNodeTypeProp locfkvalueNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.FKValue.ToString() );
                            locfkvalueNTP.ServerManaged = true;
                            locfkvalueNTP.DefaultValue.AsNumber.Value = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass ).ObjectClassId;
                            locfkvalueNTP.removeFromAllLayouts();
                            break;

                        case CswEnumNbtFieldType.NFPA:
                            CswNbtMetaDataNodeTypeProp nfpadisplaymodeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.DisplayMode.ToString() );
                            nfpadisplaymodeNTP.ListOptions = new CswCommaDelimitedString()
                                {
                                    CswEnumNbtNFPADisplayMode.Diamond.ToString(),
                                    CswEnumNbtNFPADisplayMode.Linear.ToString(),
                                }.ToString();
                            break;

                        case CswEnumNbtFieldType.NodeTypeSelect:
                            CswNbtMetaDataNodeTypeProp selectmodeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.SelectMode.ToString() );
                            selectmodeNTP.ListOptions = new CswCommaDelimitedString()
                                {
                                    CswEnumNbtPropertySelectMode.Single.ToString(),
                                    CswEnumNbtPropertySelectMode.Multiple.ToString(),
                                }.ToString();

                            CswNbtMetaDataNodeTypeProp ntsfktypeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.FKType.ToString() );
                            ntsfktypeNTP.ServerManaged = true;
                            ntsfktypeNTP.DefaultValue.AsText.Text = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString();
                            ntsfktypeNTP.removeFromAllLayouts();
                            break;

                        case CswEnumNbtFieldType.PropertyReference:
                            CswNbtMetaDataNodeTypeProp prfktypeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.FKType.ToString() );
                            prfktypeNTP.ServerManaged = true;
                            prfktypeNTP.DefaultValue.AsText.Text = CswEnumNbtViewPropIdType.NodeTypePropId.ToString();
                            prfktypeNTP.removeFromAllLayouts();

                            CswNbtMetaDataNodeTypeProp relNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.Relationship.ToString() );
                            relNTP.SetFK( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), NodeTypePropOC.ObjectClassId, string.Empty, Int32.MinValue );

                            CswNbtView relView = _CswNbtSchemaModTrnsctn.restoreView( relNTP.ViewId );
                            relView.Root.ChildRelationships.Clear();
                            CswNbtViewRelationship prrel1 = relView.AddViewRelationship( NodeTypePropNT, false );
                            CswNbtViewRelationship prrel2 = relView.AddViewRelationship( prrel1, CswEnumNbtViewPropOwnerType.Second, NTPNodeTypeOCP, false );
                            relView.AddViewPropertyAndFilter( prrel2, NTPFieldTypeOCP, Value: "Relationship" );
                            relView.AddViewPropertyAndFilter( prrel2, NTPFieldTypeOCP, Conjunction: CswEnumNbtFilterConjunction.Or, Value: "Location" );
                            relView.save();

                            CswNbtMetaDataNodeTypeProp propNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.RelatedProperty.ToString() );
                            propNTP.setFilter( relNTP, relNTP.getFieldTypeRule().SubFields[CswEnumNbtSubFieldName.NodeID], CswEnumNbtFilterMode.NotNull, null );
                            propNTP.SetFK( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), NodeTypePropOC.ObjectClassId, string.Empty, Int32.MinValue );

                            CswNbtMetaDataNodeTypeProp useseqNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.UseSequence.ToString() );
                            CswNbtMetaDataNodeTypeProp prsequenceNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.Sequence.ToString() );
                            prsequenceNTP.setFilter( useseqNTP, useseqNTP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswEnumTristate.True );
                            break;

                        case CswEnumNbtFieldType.ViewPickList:
                            CswNbtMetaDataNodeTypeProp vplselectmodeNTP = NodeTypePropNT.getNodeTypeProp( CswEnumNbtPropertyAttributeName.SelectMode.ToString() );
                            vplselectmodeNTP.ListOptions = new CswCommaDelimitedString()
                                {
                                    CswEnumNbtPropertySelectMode.Single.ToString(),
                                    CswEnumNbtPropertySelectMode.Multiple.ToString(),
                                }.ToString();
                            break;
                    } // switch( FieldType.FieldType )

                    propNTDict.Add( FieldType.FieldType, NodeTypePropNT );

                } // foreach( CswNbtMetaDataFieldType FieldType in _CswNbtSchemaModTrnsctn.MetaData.getFieldTypes() )


                // Populate nodes
                // Very important that this happens BEFORE we map to the nodetype_props table, or else we'll end up duplicating rows!
                foreach( CswNbtMetaDataNodeType thisNodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
                {
                    Dictionary<Int32, CswNbtObjClassDesignNodeTypeProp> propsDict = new Dictionary<int, CswNbtObjClassDesignNodeTypeProp>();
                    foreach( CswNbtMetaDataNodeTypeProp thisProp in thisNodeType.getNodeTypeProps() )
                    {
                        CswNbtMetaDataNodeType NodeTypePropNT = propNTDict[thisProp.getFieldTypeValue()];

                        CswNbtObjClassDesignNodeTypeProp node = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NodeTypePropNT.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode, true );
                        node.RelationalId = new CswPrimaryKey( "nodetype_props", thisProp.PropId );

                        node.AuditLevel.Value = thisProp.AuditLevel;
                        node.CompoundUnique.Checked = CswConvert.ToTristate( thisProp.IsCompoundUnique() );
                        node.FieldType.Value = thisProp.getFieldTypeValue();
                        node.HelpText.Text = thisProp.HelpText;
                        node.NodeTypeValue.RelatedNodeId = NTNodes[thisNodeType.NodeTypeId].NodeId;
                        node.ObjectClassPropName.Text = thisProp.getObjectClassPropName();
                        node.PropName.Text = thisProp.PropName;
                        node.ReadOnly.Checked = CswConvert.ToTristate( thisProp.ReadOnly );
                        node.Required.Checked = CswConvert.ToTristate( thisProp.IsRequired );
                        node.UseNumbering.Checked = CswConvert.ToTristate( thisProp.UseNumbering );
                        node.Unique.Checked = CswConvert.ToTristate( thisProp.IsUnique() );

                        ICswNbtFieldTypeRule Rule = thisProp.getFieldTypeRule();
                        foreach( CswNbtFieldTypeAttribute Attr in Rule.getAttributes() )
                        {
                            CswNbtMetaDataNodeTypeProp prop = NodeTypePropNT.getNodeTypeProp( Attr.Name.ToString() );
                            CswNbtNodePropWrapper wrapper = node.Node.Properties[prop];
                            switch( Attr.AttributeFieldType )
                            {
                                case CswEnumNbtFieldType.DateTime:
                                    wrapper.AsDateTime.DateTimeValue = CswConvert.ToDateTime( prop[Attr.Column].ToString() );
                                    break;
                                case CswEnumNbtFieldType.Link:
                                    wrapper.AsLink.Href = prop[Attr.Column].ToString();
                                    break;
                                case CswEnumNbtFieldType.List:
                                    wrapper.AsList.Value = prop[Attr.Column].ToString();
                                    break;
                                case CswEnumNbtFieldType.Logical:
                                    wrapper.AsLogical.Checked = CswConvert.ToTristate( prop[Attr.Column] );
                                    break;
                                case CswEnumNbtFieldType.Memo:
                                    wrapper.AsMemo.Text = prop[Attr.Column].ToString();
                                    break;
                                case CswEnumNbtFieldType.MultiList:
                                    CswCommaDelimitedString val = new CswCommaDelimitedString();
                                    val.FromString( prop[Attr.Column].ToString() );
                                    wrapper.AsMultiList.Value = val;
                                    break;
                                case CswEnumNbtFieldType.NodeTypeSelect:
                                    CswCommaDelimitedString ntsval = new CswCommaDelimitedString();
                                    ntsval.FromString( prop[Attr.Column].ToString() );
                                    wrapper.AsNodeTypeSelect.SelectedNodeTypeIds = ntsval;
                                    break;
                                case CswEnumNbtFieldType.Number:
                                    wrapper.AsNumber.Value = CswConvert.ToDouble( prop[Attr.Column] );
                                    break;
                                case CswEnumNbtFieldType.Relationship:
                                    // Need to decode the relationship value
                                    _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "nodetype_props", Attr.Column.ToString() );
                                    if( false == string.IsNullOrEmpty( _CswNbtSchemaModTrnsctn.CswDataDictionary.ForeignKeyTable ) )
                                    {
                                        CswPrimaryKey Fk = new CswPrimaryKey( _CswNbtSchemaModTrnsctn.CswDataDictionary.ForeignKeyTable, CswConvert.ToInt32( prop[Attr.Column] ) );
                                        CswNbtNode FkNode = _CswNbtSchemaModTrnsctn.Nodes.getNodeByRelationalId( Fk );
                                        if( null != FkNode )
                                        {
                                            wrapper.AsRelationship.RelatedNodeId = FkNode.NodeId;
                                        }
                                    }
                                    break;
                                case CswEnumNbtFieldType.Static:
                                    wrapper.AsStatic.StaticText = prop[Attr.Column].ToString();
                                    break;
                                case CswEnumNbtFieldType.Text:
                                    wrapper.AsText.Text = prop[Attr.Column].ToString();
                                    break;
                                case CswEnumNbtFieldType.ViewReference:
                                    // Can't set because it's private    
                                    //wrapper.AsViewReference.ViewId = new CswNbtViewId( CswConvert.ToInt32( prop[Attr.Column].ToString() ) );
                                    wrapper.SetPropRowValue( CswEnumNbtPropColumn.Field1_FK, prop[Attr.Column] );
                                    break;
                            }
                        }

                        node.postChanges( false );

                        propsDict.Add( thisProp.PropId, node );
                    } // foreach( CswNbtMetaDataNodeTypeProp thisProp in thisNodeType.getNodeTypeProps() )


                    // Conditional Filters
                    foreach( CswNbtMetaDataNodeTypeProp thisProp in thisNodeType.getNodeTypeProps() )
                    {
                        if( thisProp.hasFilter() )
                        {
                            CswNbtObjClassDesignNodeTypeProp node = propsDict[thisProp.PropId];
                            CswNbtSubField SubField = null;
                            CswEnumNbtFilterMode FilterMode = null;
                            string FilterValue = string.Empty;
                            thisProp.getFilter( ref SubField, ref FilterMode, ref FilterValue );
                            node.DisplayConditionProperty.RelatedNodeId = propsDict[thisProp.FilterNodeTypePropId].NodeId;
                            node.DisplayConditionSubfield.Value = SubField.Name.ToString();
                            node.DisplayConditionFilter.Value = FilterValue;
                            node.DisplayConditionValue.Text = FilterValue;
                        }
                    } // foreach( CswNbtMetaDataNodeTypeProp thisProp in thisNodeType.getNodeTypeProps() )
                } // foreach( CswNbtMetaDataNodeType thisNodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )


                // Here's where the extra special super-secret magic comes in
                foreach( CswEnumNbtFieldType FieldType in propNTDict.Keys )
                {
                    CswNbtMetaDataNodeType NodeTypePropNT = propNTDict[FieldType];

                    CswNbtMetaDataNodeTypeProp NTPAuditLevelNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.AuditLevel );
                    CswNbtMetaDataNodeTypeProp NTPCompoundUniqueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.CompoundUnique );
                    CswNbtMetaDataNodeTypeProp NTPDisplayConditionFilterNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionFilter );
                    CswNbtMetaDataNodeTypeProp NTPDisplayConditionPropertyNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionProperty );
                    CswNbtMetaDataNodeTypeProp NTPDisplayConditionSubfieldNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionSubfield );
                    CswNbtMetaDataNodeTypeProp NTPDisplayConditionValueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionValue );
                    CswNbtMetaDataNodeTypeProp NTPFieldTypeNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.FieldType );
                    CswNbtMetaDataNodeTypeProp NTPHelpTextNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.HelpText );
                    CswNbtMetaDataNodeTypeProp NTPNodeTypeValueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue );
                    CswNbtMetaDataNodeTypeProp NTPObjectClassPropNameNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.ObjectClassPropName );
                    CswNbtMetaDataNodeTypeProp NTPPropNameNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName );
                    CswNbtMetaDataNodeTypeProp NTPReadOnlyNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.ReadOnly );
                    CswNbtMetaDataNodeTypeProp NTPRequiredNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.Required );
                    CswNbtMetaDataNodeTypeProp NTPUniqueNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.Unique );
                    CswNbtMetaDataNodeTypeProp NTPUseNumberingNTP = NodeTypePropNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.UseNumbering );

                    NodeTypePropNT.TableName = "nodetype_props";

                    _addJctRow( jctTable, NTPAuditLevelNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Auditlevel );
                    _addJctRow( jctTable, NTPCompoundUniqueNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Iscompoundunique );
                    //_addJctRow( jctTable, NTPDisplayConditionFilterNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Displayconditionfilter );
                    _addJctRow( jctTable, NTPDisplayConditionPropertyNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Filterpropid, CswEnumNbtSubFieldName.NodeID );
                    //_addJctRow( jctTable, NTPDisplayConditionSubfieldNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Displayconditionsubfield );
                    //_addJctRow( jctTable, NTPDisplayConditionValueNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Displayconditionvalue );
                    _addJctRow( jctTable, NTPFieldTypeNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Fieldtypeid );
                    _addJctRow( jctTable, NTPHelpTextNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Helptext );
                    _addJctRow( jctTable, NTPNodeTypeValueNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Nodetypeid, CswEnumNbtSubFieldName.NodeID );
                    _addJctRow( jctTable, NTPObjectClassPropNameNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Objectclasspropid );
                    _addJctRow( jctTable, NTPPropNameNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Propname );
                    _addJctRow( jctTable, NTPReadOnlyNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Readonly );
                    _addJctRow( jctTable, NTPRequiredNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Isrequired );
                    _addJctRow( jctTable, NTPUniqueNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Isunique );
                    _addJctRow( jctTable, NTPUseNumberingNTP, NodeTypePropNT.TableName, CswEnumNbtPropertyAttributeColumn.Usenumbering );

                    ICswNbtFieldTypeRule Rule = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( FieldType ).getFieldTypeRule();
                    foreach( CswNbtFieldTypeAttribute Attr in Rule.getAttributes() )
                    {
                        CswNbtMetaDataNodeTypeProp thisNTP = NodeTypePropNT.getNodeTypeProp( Attr.Name );
                        _addJctRow( jctTable, thisNTP, NodeTypePropNT.TableName, Attr.Column );
                    }
                } // foreach( CswEnumNbtFieldType FieldType in propNTDict.Keys )
            } // PROPS


            jctUpdate.update( jctTable );


            // Create a temporary view for debugging (REMOVE ME)
            CswNbtView DesignView = _CswNbtSchemaModTrnsctn.makeView();
            DesignView.saveNew( "Design", CswEnumNbtViewVisibility.Global );
            DesignView.Category = "Design";
            CswNbtViewRelationship NtViewRel = DesignView.AddViewRelationship( NodeTypeOC, false );
            DesignView.AddViewRelationship( NtViewRel, CswEnumNbtViewPropOwnerType.Second, NTPNodeTypeOCP, false );
            DesignView.AddViewRelationship( NtViewRel, CswEnumNbtViewPropOwnerType.Second, NTTNodeTypeNTP, false );
            DesignView.Root.GroupBySiblings = true;
            DesignView.save();

        } // update()


        private void _addJctRow( DataTable JctTable, CswNbtMetaDataNodeTypeProp Prop, string TableName, string ColumnName, CswEnumNbtSubFieldName SubFieldName = null )
        {
            _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( TableName, ColumnName );
            DataRow NodeTypeNameRow = JctTable.NewRow();
            NodeTypeNameRow["nodetypepropid"] = Prop.PropId;
            NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
            if( null != SubFieldName )
            {
                NodeTypeNameRow["subfieldname"] = SubFieldName.ToString();
            }
            else if( null != Prop.getFieldTypeRule().SubFields.Default )
            {
                NodeTypeNameRow["subfieldname"] = Prop.getFieldTypeRule().SubFields.Default.Name;
            }
            JctTable.Rows.Add( NodeTypeNameRow );
        }

        private CswNbtMetaDataNodeTypeProp _makePropNTP( CswNbtMetaDataNodeType NodeTypePropNT, Int32 TabId, CswNbtFieldTypeAttribute Attribute )
        {
            return _makePropNTP( NodeTypePropNT, TabId, Attribute.AttributeFieldType, Attribute.Name, Attribute.Column );
        }
        private CswNbtMetaDataNodeTypeProp _makePropNTP( CswNbtMetaDataNodeType NodeTypePropNT, Int32 TabId, CswEnumNbtFieldType FieldType, string PropName, CswEnumNbtPropertyAttributeColumn ColumnName )
        {
            CswNbtMetaDataNodeTypeProp newNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( NodeTypePropNT, FieldType, PropName, TabId );
            newNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

            if( ColumnName == CswEnumNbtPropertyAttributeColumn.Isfk )
            {
                newNTP.ServerManaged = true;
                newNTP.DefaultValue.AsLogical.Checked = CswEnumTristate.True;
                newNTP.removeFromAllLayouts();
            }

            // TO DO: do something with Attribute.Attribute (Columnname) here!

            return newNTP;
        }

    }//class CswUpdateSchema_02C_Case29311_Design

}//namespace ChemSW.Nbt.Schema