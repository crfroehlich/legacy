﻿using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30743_Materials: CswUpdateSchemaTo
    {
        public override string Title { get { return "Setup Materials import bindings"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30679; }
        }

        public override string ScriptName
        {
            get { return "Case30743_Materials"; }
        }

        public override void update()
        {
            // CAF bindings definitions for Vendors
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "packages", "Chemical", ViewName : "Chemicals_View" ); //PACKAGES not MATERIALS (intentional)

            //Simple Props
            ImpMgr.importBinding( "aqueous_solubility", CswNbtObjClassChemical.PropertyName.AqueousSolubility, "" );
            ImpMgr.importBinding( "casno", CswNbtObjClassChemical.PropertyName.CasNo, "" );
            ImpMgr.importBinding( "boiling_point", CswNbtObjClassChemical.PropertyName.BoilingPoint, "" );
            ImpMgr.importBinding( "formula", CswNbtObjClassChemical.PropertyName.Formula, "" );
            ImpMgr.importBinding( "materialname", CswNbtObjClassChemical.PropertyName.TradeName, "" );
            ImpMgr.importBinding( "melting_point", CswNbtObjClassChemical.PropertyName.MeltingPoint, "" );
            ImpMgr.importBinding( "molecular_weight", CswNbtObjClassChemical.PropertyName.MolecularWeight, "" );
            ImpMgr.importBinding( "ph", CswNbtObjClassChemical.PropertyName.pH, "" );
            ImpMgr.importBinding( "physical_description", CswNbtObjClassChemical.PropertyName.PhysicalDescription, "" );
            ImpMgr.importBinding( "ppe", CswNbtObjClassChemical.PropertyName.PPE, "" );
            ImpMgr.importBinding( "specific_gravity", CswNbtObjClassChemical.PropertyName.SpecificGravity, "" );
            ImpMgr.importBinding( "vapor_density", CswNbtObjClassChemical.PropertyName.VaporDensity, "" );
            ImpMgr.importBinding( "vapor_pressure", CswNbtObjClassChemical.PropertyName.VaporPressure, "" );
            ImpMgr.importBinding( "storage_conditions", CswNbtObjClassChemical.PropertyName.StorageAndHandling, "" );
            ImpMgr.importBinding( "istier2", CswNbtObjClassChemical.PropertyName.IsTierII, "" );
            ImpMgr.importBinding( "productno", CswNbtObjClassChemical.PropertyName.PartNumber, "" );

            //NFPA
            ImpMgr.importBinding( "firecode", CswNbtObjClassChemical.PropertyName.NFPA, CswEnumNbtSubFieldName.Flammability.ToString() );
            ImpMgr.importBinding( "healthcode", CswNbtObjClassChemical.PropertyName.NFPA, CswEnumNbtSubFieldName.Health.ToString() );
            ImpMgr.importBinding( "reactivecode", CswNbtObjClassChemical.PropertyName.NFPA, CswEnumNbtSubFieldName.Reactivity.ToString() );
            ImpMgr.importBinding( "nfpacode", CswNbtObjClassChemical.PropertyName.NFPA, CswEnumNbtSubFieldName.Special.ToString() );

            //Relationships
            ImpMgr.importBinding( "vendorid", CswNbtObjClassChemical.PropertyName.Supplier, CswEnumNbtSubFieldName.NodeID.ToString() );

            //Transformed props
            ImpMgr.importBinding( "physical_state_trans", CswNbtObjClassChemical.PropertyName.PhysicalState, "" );
            ImpMgr.importBinding( "nonhazardous3e_trans", CswNbtObjClassChemical.PropertyName.Hazardous, "" );
            //TODO: find/replace substrings in PPE

            //TODO: lob data
            ImpMgr.importBinding( "struct_pict", CswNbtObjClassChemical.PropertyName.Structure, "", BlobTableName: "materials", LobDataPkColOverride: "materialid" );
            //disposal                 -> new prop "Disposal Instructions as memo
            
                                       
            //TODO: new props          
            //dot_code                 -> "DOT Code" as list
            //hazards                  -> "Hazard Info" as memo
            //einecs                   -> "EINCES" as text?
            //compressed_gas           -> "Compressed Gas" as bool?
            //smiles                   -> "SMILES" as text?
            //openexpireinterval       -> "Open Expire Interval" as time? (same as Expire Interval?)
            //openexpireintervalunits  -> "Open Expire Interval" as time? (same as Expire Interval?)
            //creation_date            -> "Legacy Creation Date" as server managed text
            //creationsiteid           -> "Legacy Creation Site Id" as server managed number
            //materialvarietyid        -> need more info


            ImpMgr.finalize( UseView : true );

        }
    }
}