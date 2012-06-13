﻿using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25759
    /// </summary>
    public class CswUpdateSchemaCase25759 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //Set Container.Quantity to ServerManaged
            CswNbtMetaDataObjectClass ContainerObjClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp QuantityProp = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( ContainerObjClass.ObjectClassId, CswNbtObjClassContainer.QuantityPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuantityProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );

            //Add "N/A" to Material.PhysicalState
            CswNbtMetaDataObjectClass MaterialObjClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp PhysicalStateProp = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( MaterialObjClass.ObjectClassId, CswNbtObjClassMaterial.PhysicalStatePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PhysicalStateProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, "solid,liquid,gas,n/a" );

            //Set Supplies.PhysicalState DefaultValue to "N/A", ServerManaged = true
            CswNbtMetaDataNodeType SupplyNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Supply" );
            CswNbtMetaDataNodeTypeProp PhysicalStateNTProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( SupplyNodeType.NodeTypeId, CswNbtObjClassMaterial.PhysicalStatePropertyName );
            PhysicalStateNTProp.DefaultValue.AsList.Value = "N/A";
            PhysicalStateNTProp.ServerManaged = true;

            //Set Size.Capacity SetValOnAdd = false (if necessary)
            CswNbtMetaDataObjectClass SizeObjClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp CapacityProp = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( SizeObjClass.ObjectClassId, CswNbtObjClassSize.CapacityPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CapacityProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, false );

        }//Update()

    }//class CswUpdateSchemaCase25759

}//namespace ChemSW.Nbt.Schema