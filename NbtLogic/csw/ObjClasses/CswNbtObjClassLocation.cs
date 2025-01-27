using System;
using System.Collections.Generic;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassLocation : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string ChildLocationType = "Child Location Type";
            public const string LocationTemplate = "Location Template";
            public const string Location = "Location";
            public const string Order = "Order";
            public const string Rows = "Rows";
            public const string Columns = "Columns";
            public const string Barcode = "Barcode";
            public const string Name = "Name";
            public const string InventoryGroup = "Inventory Group";
            public const string LocationCode = "Location Code";
            public const string AllowInventory = "Allow Inventory";
            public const string StorageCompatibility = "Storage Compatibility";
            public const string ControlZone = "Control Zone";
            public const string Containers = "Containers";
            public const string InventoryLevels = "Inventory Levels";
            public const string Responsible = "Responsible";
            public const string RequestDeliveryLocation = "Request Delivery Location";
            public const string FullPath = "Full Path";
        }

        public CswNbtObjClassLocation( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassLocation
        /// </summary>
        public static implicit operator CswNbtObjClassLocation( CswNbtNode Node )
        {
            CswNbtObjClassLocation ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.LocationClass ) )
            {
                ret = (CswNbtObjClassLocation) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) &&
                Location.wasAnySubFieldModified() &&
                _CswNbtResources.EditMode != CswEnumNbtNodeEditMode.Add )
            {
                CswNbtNodePropWrapper LocationWrapper = Node.Properties[PropertyName.Location];
                string PrevLocationId = LocationWrapper.GetOriginalPropRowValue( ( (CswNbtFieldTypeRuleLocation) _CswNbtResources.MetaData.getFieldTypeRule( LocationWrapper.getFieldTypeValue() ) ).NodeIdSubField.Column );

                CswPrimaryKey PrevLocationPk = null;
                CswPrimaryKey CurrLocationPk = null;
                if( false == String.IsNullOrEmpty( PrevLocationId ) )
                {
                    PrevLocationPk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( PrevLocationId ) );
                }
                if( null != Location.SelectedNodeId )
                {
                    CurrLocationPk = Location.SelectedNodeId;
                }
                if( PrevLocationPk != null && PrevLocationPk != CurrLocationPk )
                {
                    //Case 26849 - Executing even if one of the locations is Top or null so that the other location can still be updated
                    CswNbtBatchOpInventoryLevels BatchOp = new CswNbtBatchOpInventoryLevels( _CswNbtResources );
                    BatchOp.makeBatchOp( PrevLocationPk, CurrLocationPk );
                }
            }

            //Case 27495 - Sites can only be at "Top"
            if( NodeType.NodeTypeName == "Site" )
            {
                Location.SelectedNodeId = null;
                Location.RefreshNodeName();
                Location.SyncGestalt();
            }
        }//beforeWriteNode()     

        protected override void afterPopulateProps()
        {
            // Hide the Child Location Type and Location Template controls
            if( _CswNbtResources.ConfigVbls.getConfigVariableValue( "loc_use_images" ) == "0" )
            {
                this.ChildLocationType.setHidden( value: true, SaveToDb: false );
                this.Rows.setHidden( value: true, SaveToDb: false );
                this.Columns.setHidden( value: true, SaveToDb: false );
                this.LocationTemplate.setHidden( value: true, SaveToDb: false );
            }
        }//afterPopulateProps()

        #endregion  Inherited Events

        #region Object class specific properties

        public CswNbtNodePropList ChildLocationType { get { return ( _CswNbtNode.Properties[PropertyName.ChildLocationType] ); } }
        public CswNbtNodePropList LocationTemplate { get { return ( _CswNbtNode.Properties[PropertyName.LocationTemplate] ); } }
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[PropertyName.Location] ); } }
        public CswNbtNodePropNumber Order { get { return ( _CswNbtNode.Properties[PropertyName.Order] ); } }
        public CswNbtNodePropNumber Rows { get { return ( _CswNbtNode.Properties[PropertyName.Rows] ); } }
        public CswNbtNodePropNumber Columns { get { return ( _CswNbtNode.Properties[PropertyName.Columns] ); } }
        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[PropertyName.Barcode] ); } }
        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }
        public CswNbtNodePropRelationship InventoryGroup { get { return ( _CswNbtNode.Properties[PropertyName.InventoryGroup] ); } }
        public CswNbtNodePropText LocationCode { get { return ( _CswNbtNode.Properties[PropertyName.LocationCode] ); } }
        public CswNbtNodePropLogical AllowInventory { get { return ( _CswNbtNode.Properties[PropertyName.AllowInventory] ); } }
        public CswNbtNodePropImageList StorageCompatibility { get { return ( _CswNbtNode.Properties[PropertyName.StorageCompatibility] ); } }
        public CswNbtNodePropRelationship ControlZone { get { return ( _CswNbtNode.Properties[PropertyName.ControlZone] ); } }
        public CswNbtNodePropGrid Containers { get { return ( _CswNbtNode.Properties[PropertyName.Containers] ); } }
        public CswNbtNodePropGrid InventoryLevels { get { return ( _CswNbtNode.Properties[PropertyName.InventoryLevels] ); } }
        public CswNbtNodePropRelationship Responsible { get { return ( _CswNbtNode.Properties[PropertyName.Responsible] ); } }
        public CswNbtNodePropLogical RequestDeliveryLocation { get { return ( _CswNbtNode.Properties[PropertyName.RequestDeliveryLocation] ); } }
        public CswNbtNodePropComposite FullPath { get { return ( _CswNbtNode.Properties[PropertyName.FullPath] ); } }

        #endregion Object class specific properties

        #region Custom Logic

        public static void makeLocationsTreeView( ref CswNbtView LocationsView, CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, Int32 loc_max_depth = Int32.MinValue, CswPrimaryKey NodeIdToFilterOut = null, bool RequireAllowInventory = false, IEnumerable<CswPrimaryKey> InventoryGroupIds = null, bool DisableLowestLevel = false, CswEnumNbtFilterResultMode ResultMode = null, string FullPathFilter = "" )
        {
            _makeLocationsTreeView( ref LocationsView, CswNbtSchemaModTrnsctn.MetaData, CswNbtSchemaModTrnsctn.ConfigVbls, loc_max_depth, NodeIdToFilterOut, RequireAllowInventory, InventoryGroupIds, DisableLowestLevel, ResultMode ?? CswEnumNbtFilterResultMode.Disabled, FullPathFilter );
        }

        public static void makeLocationsTreeView( ref CswNbtView LocationsView, CswNbtResources CswNbtResources, Int32 loc_max_depth = Int32.MinValue, CswPrimaryKey NodeIdToFilterOut = null, bool RequireAllowInventory = false, IEnumerable<CswPrimaryKey> InventoryGroupIds = null, bool DisableLowestLevel = false, CswEnumNbtFilterResultMode ResultMode = null, string FullPathFilter = "" )
        {
            _makeLocationsTreeView( ref LocationsView, CswNbtResources.MetaData, CswNbtResources.ConfigVbls, loc_max_depth, NodeIdToFilterOut, RequireAllowInventory, InventoryGroupIds, DisableLowestLevel, ResultMode ?? CswEnumNbtFilterResultMode.Disabled, FullPathFilter );
        }

        private static void _makeLocationsTreeView( ref CswNbtView LocationsView, CswNbtMetaData MetaData, CswConfigurationVariables ConfigVbls, Int32 loc_max_depth, CswPrimaryKey NodeIdToFilterOut, bool RequireAllowInventory, IEnumerable<CswPrimaryKey> InventoryGroupIds, bool DisableLowestLevel, CswEnumNbtFilterResultMode ResultMode, string FullPathFilter = "" )
        {
            if( null != LocationsView )
            {
                CswNbtMetaDataObjectClass LocationOC = MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
                CswNbtMetaDataObjectClassProp LocationLocationOCP = LocationOC.getObjectClassProp( PropertyName.Location );
                CswNbtMetaDataObjectClassProp LocationOrderOCP = LocationOC.getObjectClassProp( PropertyName.Order );
                CswNbtMetaDataObjectClassProp LocationAllowInventoryOCP = LocationOC.getObjectClassProp( PropertyName.AllowInventory );
                CswNbtMetaDataObjectClassProp LocationInventoryGroupOCP = LocationOC.getObjectClassProp( PropertyName.InventoryGroup );
                CswNbtMetaDataObjectClassProp LocationNameOCP = LocationOC.getObjectClassProp( PropertyName.Name );
                CswNbtMetaDataObjectClassProp LocationFullPathOCP = LocationOC.getObjectClassProp( PropertyName.FullPath );

                LocationsView.Root.ChildRelationships.Clear();

                CswNbtViewRelationship LocReln = LocationsView.AddViewRelationship( LocationOC, true );
                LocationsView.AddViewProperty( LocReln, LocationLocationOCP );

                if( null != NodeIdToFilterOut )
                {
                    LocReln.NodeIdsToFilterOut.Add( NodeIdToFilterOut );
                }

                CswNbtViewProperty InGroupVp = LocationsView.AddViewProperty( LocReln, LocationInventoryGroupOCP );
                InGroupVp.Width = 100;

                if( null != InventoryGroupIds )
                {
                    CswCommaDelimitedString Pks = new CswCommaDelimitedString();
                    foreach( CswPrimaryKey InventoryGroupId in InventoryGroupIds )
                    {
                        Pks.Add( InventoryGroupId.PrimaryKey.ToString() );
                    }

                    LocationsView.AddViewPropertyFilter( InGroupVp,
                                                            Conjunction: CswEnumNbtFilterConjunction.And,
                                                            ResultMode: ResultMode,
                                                            FilterMode: CswEnumNbtFilterMode.In,
                                                            SubFieldName: CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                                                            Value: Pks.ToString() );
                }

                CswNbtViewProperty OrderVPn = LocationsView.AddViewProperty( LocReln, LocationOrderOCP );
                LocationsView.setSortProperty( OrderVPn, CswEnumNbtViewPropertySortMethod.Ascending, false );

                if( RequireAllowInventory )
                {
                    LocationsView.AddViewPropertyAndFilter( LocReln, LocationAllowInventoryOCP,
                                                            Conjunction: CswEnumNbtFilterConjunction.And,
                                                            ResultMode: ResultMode,
                                                            FilterMode: CswEnumNbtFilterMode.Equals,
                                                            Value: CswEnumTristate.True.ToString() );
                }

                // Filter on Full Path property
                if( false == string.IsNullOrEmpty( FullPathFilter ) )
                {
                    LocationsView.AddViewPropertyAndFilter( LocReln, LocationFullPathOCP,
                                                           Conjunction: CswEnumNbtFilterConjunction.And,
                                                           SubFieldName: CswEnumNbtSubFieldName.Value,
                                                           FilterMode: CswEnumNbtFilterMode.Contains,
                                                           Value: FullPathFilter );
                }
                else
                {
                    LocationsView.AddViewProperty( LocReln, LocationFullPathOCP );
                }

                // Add the Name property to the view
                    LocationsView.AddViewProperty( LocReln, LocationNameOCP );

            } // if( null != LocationsView )
        } // makeLocationsTreeView()

        #endregion Custom Logic

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
