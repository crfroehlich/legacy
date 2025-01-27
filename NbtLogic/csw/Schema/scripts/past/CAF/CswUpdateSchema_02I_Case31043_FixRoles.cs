﻿using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31043_FixRoles: CswUpdateNbtMasterSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31043; }
        }

        public override string AppendToScriptName()
        {
            return "Roles_V2";
        }

        public override string Title
        {
            get
            {
                return "Roles now uses view; added adminstrator binding";
            }
        }

        public override void doUpdate()
        {
            CswNbtSchemaUpdateImportMgr RoleMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            RoleMgr.removeImportOrder( "CAF", "Role" );
            RoleMgr.CAFimportOrder( "Role", "roles", "roles_view", "roleid" );

            RoleMgr.importBinding( "administrator", CswNbtObjClassRole.PropertyName.Administrator, "" );

            RoleMgr.finalize();

        } // update()
    }

}//namespace ChemSW.Nbt.Schema