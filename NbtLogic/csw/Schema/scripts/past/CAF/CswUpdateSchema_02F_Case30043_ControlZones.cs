﻿using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30043_ControlZones: CswUpdateNbtMasterSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30043; }
        }

        public override string AppendToScriptName()
        {
            return "02F_Case30043_ControlZones";
        }

        public override void doUpdate()
        {
            // Case 30043 - CAF Migration: Sites/Locations/Work Units
            CswNbtSchemaUpdateImportMgr ImportMgr_ControlZones = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );


            ImportMgr_ControlZones.CAFimportOrder( "Control Zone", "cispro_controlzones", PkColumnName: "controlzoneid" );

            // Binding
            ImportMgr_ControlZones.importBinding( "controlzonename", CswNbtObjClassControlZone.PropertyName.Name, "" );
            ImportMgr_ControlZones.importBinding( "exemptqtyfactor", CswNbtObjClassControlZone.PropertyName.MAQOffset, "" );

            // Relationship
            //none

            ImportMgr_ControlZones.finalize();

        } // update()

    } // class CswUpdateSchema_02F_Case30043_ControlZones

}//namespace ChemSW.Nbt.Schema