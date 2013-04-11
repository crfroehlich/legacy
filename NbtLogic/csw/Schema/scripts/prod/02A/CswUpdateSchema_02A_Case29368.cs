﻿using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29368
    /// </summary>
    public class CswUpdateSchema_02A_Case29368 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29368; }
        }

        public override void update()
        {
            CswNbtPermit.NodeTypePermission[] NTPermissions = 
            { 
                CswNbtPermit.NodeTypePermission.View, 
                CswNbtPermit.NodeTypePermission.Create, 
                CswNbtPermit.NodeTypePermission.Edit, 
                CswNbtPermit.NodeTypePermission.Delete 
            };

            CswNbtObjClassRole RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Admin" );
            if( null != RoleNode )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.HMIS_Reporting, RoleNode, true );
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Tier_II_Reporting, RoleNode, true );
                CswNbtMetaDataNodeType ControlZoneNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Control Zone" );
                if( null != ControlZoneNT )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( NTPermissions, ControlZoneNT, RoleNode, true );
                }
            }

        } // update()
    }//class CswUpdateSchema_02A_Case29368
}//namespace ChemSW.Nbt.Schema