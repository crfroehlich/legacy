using System;
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
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
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
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassLocation( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

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

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) &&
                Location.WasModified &&
                _CswNbtResources.EditMode != NodeEditMode.Add )
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
                if( PrevLocationPk != CurrLocationPk )
                {
                    //Case 26849 - Executing even if one of the locations is Top or null so that the other location can still be updated
                    CswNbtBatchOpInventoryLevels BatchOp = new CswNbtBatchOpInventoryLevels( _CswNbtResources );
                    BatchOp.makeBatchOp( PrevLocationPk, CurrLocationPk );
                }
            }

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            // BZ 6744
            // Hide the Child Location Type and Location Template controls
            if( _CswNbtResources.ConfigVbls.getConfigVariableValue( "loc_use_images" ) == "0" )
            {
                this.ChildLocationType.setHidden( value: true, SaveToDb: false );
                this.Rows.setHidden( value: true, SaveToDb: false );
                this.Columns.setHidden( value: true, SaveToDb: false );
                this.LocationTemplate.setHidden( value: true, SaveToDb: false );
            }

            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties


        public CswNbtNodePropList ChildLocationType
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.ChildLocationType] );
            }
        }

        public CswNbtNodePropList LocationTemplate
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.LocationTemplate] );
            }
        }

        public CswNbtNodePropLocation Location
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Location] );
            }
        }

        public CswNbtNodePropNumber Order
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Order] );
            }
        }

        public CswNbtNodePropNumber Rows
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Rows] );
            }
        }

        public CswNbtNodePropNumber Columns
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Columns] );
            }
        }

        public CswNbtNodePropBarcode Barcode
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Barcode] );
            }
        }
        public CswNbtNodePropText Name
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Name] );
            }
        }
        public CswNbtNodePropRelationship InventoryGroup
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.InventoryGroup] );
            }
        }
        public CswNbtNodePropText LocationCode
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.LocationCode] );
            }
        }
        public CswNbtNodePropLogical AllowInventory
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.AllowInventory] );
            }
        }
        public CswNbtNodePropImageList StorageCompatibility
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.StorageCompatibility] );
            }
        }
        public CswNbtNodePropRelationship ControlZone
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.ControlZone] );
            }
        }
        public CswNbtNodePropGrid Containers
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Containers] );
            }
        }

        #endregion

        
        public static void makeLocationsTreeView( ref CswNbtView LocationsView, CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, Int32 loc_max_depth = Int32.MinValue, CswPrimaryKey NodeIdToFilterOut = null, bool RequireAllowInventory = false )
        {
            _makeLocationsTreeView( ref LocationsView, CswNbtSchemaModTrnsctn.MetaData, CswNbtSchemaModTrnsctn.ConfigVbls, loc_max_depth, NodeIdToFilterOut, RequireAllowInventory );
        }
        
        public static void makeLocationsTreeView( ref CswNbtView LocationsView, CswNbtResources CswNbtResources, Int32 loc_max_depth = Int32.MinValue, CswPrimaryKey NodeIdToFilterOut = null, bool RequireAllowInventory = false )
        {
            _makeLocationsTreeView( ref LocationsView, CswNbtResources.MetaData, CswNbtResources.ConfigVbls, loc_max_depth, NodeIdToFilterOut, RequireAllowInventory );
        }

        private static void _makeLocationsTreeView( ref CswNbtView LocationsView, CswNbtMetaData MetaData, CswConfigurationVariables ConfigVbls, Int32 loc_max_depth, CswPrimaryKey NodeIdToFilterOut, bool RequireAllowInventory )
        {
            if( null != LocationsView )
            {
                CswNbtMetaDataObjectClass LocationOC = MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
                CswNbtMetaDataObjectClassProp LocationLocationOCP = LocationOC.getObjectClassProp( PropertyName.Location );
                CswNbtMetaDataObjectClassProp LocationOrderOCP = LocationOC.getObjectClassProp( PropertyName.Order );
                CswNbtMetaDataObjectClassProp LocationAllowInventoryOCP = LocationOC.getObjectClassProp( PropertyName.AllowInventory );
                CswNbtMetaDataObjectClassProp LocationInventoryGroupOCP = LocationOC.getObjectClassProp( PropertyName.InventoryGroup );
 
                if( loc_max_depth == Int32.MinValue )
                {
                    loc_max_depth = CswConvert.ToInt32( ConfigVbls.getConfigVariableValue( "loc_max_depth" ) );
                }
                if( loc_max_depth < 1 )
                {
                    loc_max_depth = 5;
                }

                LocationsView.Root.ChildRelationships.Clear();

                CswNbtViewRelationship LocReln = null;
                for( Int32 i = 1; i <= loc_max_depth; i++ )
                {
                    if( null == LocReln )
                    {
                        // Top level: Only Locations with null parent locations at the root
                        LocReln = LocationsView.AddViewRelationship( LocationOC, true );
                        LocationsView.AddViewPropertyAndFilter( LocReln, LocationLocationOCP,
                                                                Conjunction: CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                                SubFieldName: CswEnumNbtSubFieldName.NodeID,
                                                                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null );
                    }
                    else
                    {
                        LocReln = LocationsView.AddViewRelationship( LocReln, NbtViewPropOwnerType.Second, LocationLocationOCP, true );
                    }
                    if( null != NodeIdToFilterOut )
                    {
                        LocReln.NodeIdsToFilterOut.Add( NodeIdToFilterOut );
                    }

                    CswNbtViewProperty InGroupVp = LocationsView.AddViewProperty( LocReln, LocationInventoryGroupOCP );
                    InGroupVp.Width = 100;
                    CswNbtViewProperty OrderVPn = LocationsView.AddViewProperty( LocReln, LocationOrderOCP );
                    LocationsView.setSortProperty( OrderVPn, NbtViewPropertySortMethod.Ascending, false );

                    if( RequireAllowInventory )
                    {
                        LocationsView.AddViewPropertyAndFilter( LocReln, LocationAllowInventoryOCP,
                                                                Conjunction: CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                                ResultMode: CswNbtPropFilterSql.FilterResultMode.Disabled,
                                                                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                                Value: Tristate.True.ToString() );
                    }
                } // for( Int32 i = 1; i <= loc_max_depth; i++ )
            } // if( null != LocationsView )
        } // makeLocationsTreeView()

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
