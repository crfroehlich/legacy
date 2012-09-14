using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the CISPro Module
    /// </summary>
    public class CswNbtModuleRuleCISPro : CswNbtModuleRule
    {
        public CswNbtModuleRuleCISPro( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.CISPro; } }
        public override void OnEnable()
        {
            // When CISPro is enabled, display the following properties:
            //   Location.Inventory Group
            //   Location.Storage Compatibility
            //   User.WorkUnit
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LocationInvGrpNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                LocationInvGrpNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                LocationInvGrpNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationNT.getFirstNodeTypeTab().TabId );
                
                CswNbtMetaDataNodeTypeProp LocationStorCompatNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.StorageCompatability );
                //LocationStorCompatNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                LocationStorCompatNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationNT.getFirstNodeTypeTab().TabId );

                CswNbtMetaDataNodeTypeProp LocationInvLevelsNTP = LocationNT.getNodeTypeProp( "Inventory Levels" );
                if( null != LocationInvLevelsNTP )
                {
                    CswNbtMetaDataNodeTypeTab LocationInvLevelsTab = LocationNT.getNodeTypeTab( "Inventory Levels" );
                    if( LocationInvLevelsTab == null )
                    {
                        LocationInvLevelsTab = _CswNbtResources.MetaData.makeNewTab( LocationNT, "Inventory Levels", 100 );
                    }
                    LocationInvLevelsNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationInvLevelsTab.TabId );
                }
            }
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserWorkUnitNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.WorkUnit );
                UserWorkUnitNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                UserWorkUnitNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, UserNT.getFirstNodeTypeTab().TabId );
            }
        }

        public override void OnDisable()
        {
            // case 26717 - When CISPro is disabled, hide the following properties:
            //   Location.Inventory Group
            //   Location.Storage Compatibility
            //   User.WorkUnit
            //   Location.Inventory Levels
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LocationInvGrpNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                LocationInvGrpNTP.removeFromAllLayouts();
                
                CswNbtMetaDataNodeTypeProp LocationStorCompatNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.StorageCompatability );
                LocationStorCompatNTP.removeFromAllLayouts();
                
                CswNbtMetaDataNodeTypeProp LocationInvLevelsNTP = LocationNT.getNodeTypeProp( "Inventory Levels" );
                if( null != LocationInvLevelsNTP )
                {
                    LocationInvLevelsNTP.removeFromAllLayouts();
                    CswNbtMetaDataNodeTypeTab LocationInvLevelsTab = LocationNT.getNodeTypeTab( "Inventory Levels" );
                    if( LocationInvLevelsTab != null )
                    {
                        _CswNbtResources.MetaData.DeleteNodeTypeTab( LocationInvLevelsTab );
                    }
                }
            }
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserWorkUnitNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.WorkUnit );
                UserWorkUnitNTP.removeFromAllLayouts();
            }
        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt