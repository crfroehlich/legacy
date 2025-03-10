﻿using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28535
    /// </summary>
    public class CswUpdateSchema_02A_Case28535 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 28535; }
        }

        public override void update()
        {
            // Make the VendorName property unique
            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            if( null != VendorOC )
            {
                CswNbtMetaDataObjectClassProp VendorNameOCP = VendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorName );
                if( null != VendorNameOCP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( VendorNameOCP, CswEnumNbtObjectClassPropAttributes.isunique, true );
                }
            }
        } // update()

    }//class CswUpdateSchema_02A_Case28535

}//namespace ChemSW.Nbt.Schema