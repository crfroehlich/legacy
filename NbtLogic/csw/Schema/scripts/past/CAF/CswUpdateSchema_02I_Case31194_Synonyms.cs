﻿using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31194_Synonyms: CswUpdateNbtMasterSchemaTo
    {
        public override string Title { get { return "Setup Material Synonym import bindings"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31194; }
        }

        public override string AppendToScriptName()
        {
            return "Synonyms";
        }

        public override void doUpdate()
        {
            // CAF bindings definitions for Vendors
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );
            ImpMgr.CAFimportOrder( "Material Synonym", "materials_synonyms", "synonyms_view", "legacyid", false );

            //simple props
            ImpMgr.importBinding( "synonymname", CswNbtObjClassMaterialSynonym.PropertyName.Name, "" );
            ImpMgr.importBinding( "synonymclass", CswNbtObjClassMaterialSynonym.PropertyName.Type, "" );

            //relationships
            ImpMgr.importBinding( "packageid",CswNbtObjClassMaterialSynonym.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );

            //Legacy Id for Synonyms is "<SynonymId>_<PackageId>" (ex: "123_343")
            ImpMgr.importBinding( "legacyid", "Legacy Id", "" );

            ImpMgr.finalize();

        }
    }
}