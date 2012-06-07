﻿using System;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24508B
    /// </summary>
    public class CswUpdateSchemaCase24508B : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerObjClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp DispenseProp = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                ContainerObjClass,
                new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    PropName = CswNbtObjClassContainer.DispensePropertyName
                }
            );
            foreach( CswNbtMetaDataNodeType ContainerNodeType in ContainerObjClass.getNodeTypes() )
            {
                Int32 FirstTabId = Int32.MinValue;
                foreach( CswNbtMetaDataNodeTypeTab Tab in from _Tab in ContainerNodeType.getNodeTypeTabs() orderby _Tab.TabOrder, _Tab.TabId select _Tab )
                {
                    FirstTabId = Tab.TabId;
                    break;
                }
                CswNbtMetaDataNodeTypeProp RequestNtp = ContainerNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.DispensePropertyName );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FirstTabId );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, FirstTabId );
            }

        }//Update()

    }//class CswUpdateSchemaCase24508B

}//namespace ChemSW.Nbt.Schema