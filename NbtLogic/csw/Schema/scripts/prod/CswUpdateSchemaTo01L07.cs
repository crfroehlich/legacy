﻿using System;
using System.Data;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-07
    /// </summary>
    public class CswUpdateSchemaTo01L07 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 07 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 23687

            CswTableUpdate ActionsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + "_actions_update", "actions" );
            CswCommaDelimitedString DoomedActions = new CswCommaDelimitedString()
                                                        {
                                                            "Import Fire Extinguisher Data",
                                                            "Receiving",
                                                            "Load Mobile Data",
                                                            "View By Location",
                                                            "Assign Tests",
                                                            "Enter Results",
                                                            "Split Samples"
                                                        };
            CswCommaDelimitedString DoomedActionIds = new CswCommaDelimitedString();

            //Collect and delete doomed actions
            DataTable ActionsTable = ActionsUpdate.getTable();
            foreach( DataRow ActionRow in ActionsTable.Rows )
            {
                string Name = CswConvert.ToString( ActionRow["actionname"] );
                if( DoomedActions.Contains( Name ) )
                {
                    DoomedActionIds.Add( CswConvert.ToString( ActionRow["actionid"] ) );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswConvert.ToInt32( ActionRow["actionid"] ), false );
                    ActionRow.Delete();
                }
                else
                {
                    //These aspx pages are doomed but the Actions aren't, dereference them
                    if( Name == "Create Inspection" || Name == "Edit View" )
                    {
                        ActionRow["url"] = "";
                    }
                }
            }

            //Remove permissions on doomed actions
            CswNbtMetaDataObjectClass RoleOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
            foreach( CswNbtNode Node in RoleOc.getNodes( true, false ) )
            {
                CswNbtObjClassRole NodeAsRole = CswNbtNodeCaster.AsRole( Node );
                foreach( string DoomedActionId in DoomedActionIds )
                {
                    NodeAsRole.ActionPermissions.RemoveValue( "act_" + DoomedActionId );
                }
                Node.postChanges( true );
            }

            //Delete jct rows
            CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + "_jct_modules_actions_update", "jct_modules_actions" );
            DataTable JctTable = JctUpdate.getTable( "where actionid in (" + DoomedActionIds.ToString() + ")" );
            foreach( DataRow Row in JctTable.Rows )
            {
                Row.Delete();
            }
            JctUpdate.update( JctTable );

            //Commit action delete
            ActionsUpdate.update( ActionsTable );

            #endregion Case 23687

            #region Case 24023

            CswNbtMetaDataObjectClass MailReportOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            foreach( CswNbtMetaDataNodeType NodeType in MailReportOc.NodeTypes )
            {
                CswNbtMetaDataNodeTypeProp RunNowNtp = NodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassMailReport.RunNowPropertyName );
                RunNowNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }

            CswNbtMetaDataObjectClass GeneratortOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );

            foreach( CswNbtMetaDataNodeType NodeType in GeneratortOc.NodeTypes )
            {
                CswNbtMetaDataNodeTypeProp RunNowNtp = NodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.RunNowPropertyName );
                RunNowNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }

            #endregion Case 24023

            #region Case 23641

            //Convert the NodeTypePermissions prop to the NodeTypePermissions field type so we can beautify later.
            CswNbtMetaDataObjectClassProp NodeTypePermsOcp = RoleOc.getObjectClassProp( CswNbtObjClassRole.NodeTypePermissionsPropertyName );
            CswNbtMetaDataFieldType NodeTypePermFt = _CswNbtSchemaModTrnsctn.MetaData.makeNewFieldType( CswNbtMetaDataFieldType.NbtFieldType.NodeTypePermissions, CswNbtMetaDataFieldType.DataType.TEXT );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NodeTypePermsOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fieldtypeid, NodeTypePermFt.FieldTypeId );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( RoleOc.ObjectClass,
                                               CswNbtObjClassRole.CopyFromRolePropertyName,
                                               CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                               false, false, true, CswNbtViewRelationship.RelatedIdType.Unknown, RoleOc.ObjectClassId, false, false, false, true, string.Empty, Int32.MinValue, Int32.MinValue,
                                               string.Empty,
                                               false,
                                               AuditLevel.NoAudit,
                                               string.Empty );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( RoleOc.ObjectClass,
                       CswNbtObjClassRole.CopiedFromRolePropertyName,
                       CswNbtMetaDataFieldType.NbtFieldType.Static,
                       false, true, false, CswNbtViewRelationship.RelatedIdType.Unknown, Int32.MinValue, false, false, false, true, string.Empty, Int32.MinValue, Int32.MinValue,
                       string.Empty,
                       false,
                       AuditLevel.NoAudit,
                       string.Empty );

            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

            foreach( CswNbtMetaDataNodeType NodeType in RoleOc.NodeTypes )
            {
                CswNbtMetaDataNodeTypeTab CopyTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( NodeType, "Copy From Role", NodeType.NodeTypeTabs.Count + 1 );

                CswNbtMetaDataNodeTypeProp CopyFromRoleNtp = NodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassRole.CopyFromRolePropertyName );
                CswNbtMetaDataNodeTypeProp CopiedFromRoleNtp = NodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassRole.CopiedFromRolePropertyName );

                CopyFromRoleNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, CopyTab.TabId, CopyFromRoleNtp.EditLayout.DisplayRow, CopiedFromRoleNtp.EditLayout.DisplayColumn );
                CopyFromRoleNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                CopyFromRoleNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview );

                CopiedFromRoleNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, CopyTab.TabId, CopiedFromRoleNtp.EditLayout.DisplayRow, CopiedFromRoleNtp.EditLayout.DisplayColumn );
                CopiedFromRoleNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                CopiedFromRoleNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview );

                CswNbtPermit.NodeTypeTabPermission[] Permits = new[]
                                                                    {
                                                                        CswNbtPermit.NodeTypeTabPermission.Edit, 
                                                                        CswNbtPermit.NodeTypeTabPermission.View
                                                                    };

                foreach( CswNbtNode RoleNode in RoleOc.getNodes( true, false ) )
                {
                    CswNbtObjClassRole NodeAsRole = CswNbtNodeCaster.AsRole( RoleNode );
                    bool CanViewAndEdit = ( NodeAsRole.Name.Text == CswNbtObjClassRole.ChemSWAdminRoleName || NodeAsRole.Administrator.Checked == Tristate.True );
                    _CswNbtSchemaModTrnsctn.Permit.set( Permits, CopyTab, NodeAsRole, CanViewAndEdit );
                }

            }


            #endregion Case 23641

        }//Update()

    }//class CswUpdateSchemaTo01L07

}//namespace ChemSW.Nbt.Schema


