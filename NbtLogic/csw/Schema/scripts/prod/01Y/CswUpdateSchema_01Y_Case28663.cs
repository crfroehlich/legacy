﻿using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28663
    /// </summary>
    public class CswUpdateSchema_01Y_Case28663 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28663; }
        }

        public override void update()
        {

            //Remove ability to add Inv Levels on Location Views Inventory Levels prop
            CswNbtMetaDataObjectClass locationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );

            CswNbtMetaDataObjectClass invLvlOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InventoryLevelClass );
            CswNbtMetaDataObjectClassProp locationOCP = invLvlOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Location );
            CswNbtMetaDataObjectClassProp currentQuantOCP = invLvlOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.CurrentQuantity );
            CswNbtMetaDataObjectClassProp levelOCP = invLvlOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Level );
            CswNbtMetaDataObjectClassProp materialOCP = invLvlOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Material );
            CswNbtMetaDataObjectClassProp statusOCP = invLvlOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Status );
            CswNbtMetaDataObjectClassProp typeOCP = invLvlOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Type );

            foreach( CswNbtMetaDataNodeType locationNT in locationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp invLevelsNTP = locationNT.getNodeTypeProp( "Inventory Levels" );
                if( null != invLevelsNTP )
                {
                    CswNbtView invLevelsView = _CswNbtSchemaModTrnsctn.restoreView( invLevelsNTP.ViewId );

                    CswNbtView newInvLevelsView = _CswNbtSchemaModTrnsctn.makeNewView( invLevelsView.ViewName, invLevelsView.Visibility );
                    newInvLevelsView.SetViewMode( invLevelsView.ViewMode );
                    invLevelsView.Delete();

                    CswNbtViewRelationship locationParent = newInvLevelsView.AddViewRelationship( locationNT, true );
                    CswNbtViewRelationship invLvlParent = newInvLevelsView.AddViewRelationship( locationParent, NbtViewPropOwnerType.Second, locationOCP, true );
                    invLvlParent.AddChildren = NbtViewAddChildrenSetting.None; //cannot add Inv Levels

                    int order = 1;
                    _addProp( newInvLevelsView, invLvlParent, currentQuantOCP, order );
                    order++;
                    _addProp( newInvLevelsView, invLvlParent, levelOCP, order );
                    order++;
                    _addProp( newInvLevelsView, invLvlParent, materialOCP, order );
                    order++;
                    _addProp( newInvLevelsView, invLvlParent, statusOCP, order );
                    order++;
                    _addProp( newInvLevelsView, invLvlParent, typeOCP, order );

                    newInvLevelsView.save();
                    invLevelsNTP.ViewId = newInvLevelsView.ViewId;
                }
            }


        } //Update()

        private void _addProp( CswNbtView view, CswNbtViewRelationship parent, CswNbtMetaDataObjectClassProp prop, int order )
        {
            CswNbtViewProperty viewProp = view.AddViewProperty( parent, prop );
            viewProp.Order = order;
        }

    }//class CswUpdateSchema_01Y_Case28663

}//namespace ChemSW.Nbt.Schema