﻿using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.Nbt.LandingPage;
using System.Data;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27881
    /// </summary>
    public class CswUpdateSchema_01T_Case27881 : CswUpdateSchemaTo
    {
        public override void update()
        {
            #region Init

            CswNbtLandingPageTable LandingPageObj = _CswNbtSchemaModTrnsctn.getLandingPageTable();
            string CreateMaterialActionId = _CswNbtSchemaModTrnsctn.Actions[CswNbtActionName.Create_Material].ActionId.ToString();
            string RoleId = "nodes_1";
            CswNbtObjClassRole AdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( null != AdminRole )
            {
                RoleId = AdminRole.NodeId.ToString();
            }

            #endregion Init

            #region Create Another Material

            LandingPageData.Request Request = new LandingPageData.Request
            {
                Type = "Link",
                ViewType = "Action",
                PkValue = CreateMaterialActionId,
                NodeTypeId = String.Empty,
                Text = "Create Another Material",
                RoleId = RoleId,
                ActionId = CreateMaterialActionId
            };
            LandingPageObj.addLandingPageItem( Request );

            #endregion Create Another Material

            #region Enter GHS Data for this Material

            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            {
                if( MaterialNt.NodeTypeName == "Chemical" )
                {
                    CswNbtMetaDataNodeTypeTab GHSTab = MaterialNt.getNodeTypeTab( "GHS" );
                    if( null != GHSTab )
                    {
                        Request = new LandingPageData.Request
                        {
                            Type = "Tab",
                            ViewType = "View",
                            PkValue = GHSTab.TabId.ToString(),
                            NodeTypeId = MaterialNt.NodeTypeId.ToString(),
                            Text = "Enter GHS Data for this Material",
                            RoleId = RoleId,
                            ActionId = CreateMaterialActionId
                        };
                        LandingPageObj.addLandingPageItem( Request );
                    }
                }
            }

            #endregion Enter GHS Data for this Material

            //todo - add all default createMaterial landing page items:
            //Receive this Material (Action + Material NodeId)
            //Request This Material (Action + Material NodeId)
            //Defines Sizes for this Material (Add + Material NodeId || Material NodeTypeId + TabId + Material NodeId)
        } //Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27881; }
        }

    }//class CswUpdateSchema_01T_Case27881

}//namespace ChemSW.Nbt.Schema