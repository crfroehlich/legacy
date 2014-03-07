﻿using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52280 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 52280; }
        }

        public override string Title
        {
            get { return "Set default value for Obsolete prop; set viewid for manufacturing sites prop"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialPS.getObjectClasses() )
            {
                // Set default value for Obsolete property on Materials to false
                // Remove obsolete from Add layout
                CswNbtMetaDataObjectClassProp ManufacturerSitesOCP = MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.ManufacturingSites );
                CswNbtView ManufacturersView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( "ManufacturingSites", CswEnumNbtViewVisibility.Property );

                // Set view on nodetypes
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp ObsoleteNTP = MaterialNT.getNodeTypeProp( CswNbtPropertySetMaterial.PropertyName.Obsolete );
                    ObsoleteNTP.getDefaultValue( true ).AsLogical.Checked = CswEnumTristate.False;
                    ObsoleteNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

                    CswNbtMetaDataNodeTypeProp GridNTP = MaterialNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetMaterial.PropertyName.ManufacturingSites );
                    GridNTP.ViewId.set( ManufacturersView.ViewId.get() );
                }
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema