using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.UnitsOfMeasure;
using ChemSW.Nbt.csw.Conversion;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassContainer : CswNbtObjClass
    {
        public const string BarcodePropertyName = "Barcode";
        public const string MaterialPropertyName = "Material";
        public const string LocationPropertyName = "Location";
        public const string LocationVerifiedPropertyName = "Location Verified";
        public const string StatusPropertyName = "Status";
        public const string MissingPropertyName = "Missing";
        public const string DisposedPropertyName = "Disposed";
        public const string SourceContainerPropertyName = "Source Container";
        public const string QuantityPropertyName = "Quantity";
        public const string ExpirationDatePropertyName = "Expiration Date";
        public const string SizePropertyName = "Size";
        public const string RequestDispensePropertyName = "Request Dispense";
        public const string RequestDisposePropertyName = "Request Dispose";
        public const string RequestMovePropertyName = "Request Move";
        public const string DispensePropertyName = "Dispense";
        public const string DisposePropertyName = "Dispose";
        public const string UndisposePropertyName = "Undispose";
        public const string OwnerPropertyName = "Owner";

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassContainer( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassContainer
        /// </summary>
        public static implicit operator CswNbtObjClassContainer( CswNbtNode Node )
        {
            CswNbtObjClassContainer ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass ) )
            {
                ret = (CswNbtObjClassContainer) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            RequestDispose.setHidden( value: ( Disposed.Checked == Tristate.True ), SaveToDb: true );
            RequestDispense.setHidden( value: ( Disposed.Checked == Tristate.True || Missing.Checked == Tristate.True || Quantity.Quantity <= 0 ), SaveToDb: true );
            RequestMove.setHidden( value: ( Disposed.Checked == Tristate.True ), SaveToDb: true );

            if( Material.RelatedNodeId != null )
            {
                CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
                if( MaterialNode != null )
                {
                    CswNbtObjClassMaterial MaterialNodeAsMaterial = (CswNbtObjClassMaterial) MaterialNode;

                    // case 24488 - Expiration Date default is Today + Expiration Interval of the Material
                    // I'd like to do this on beforeCreateNode(), but the Material isn't set yet.
                    if( ExpirationDate.DateTimeValue == DateTime.MinValue )
                    {
                        DateTime DefaultExpDate = DateTime.Now;
                        switch( MaterialNodeAsMaterial.ExpirationInterval.CachedUnitName.ToLower() )
                        {
                            case "hours":
                                DefaultExpDate = DefaultExpDate.AddHours( MaterialNodeAsMaterial.ExpirationInterval.Quantity );
                                break;
                            case "days":
                                DefaultExpDate = DefaultExpDate.AddDays( MaterialNodeAsMaterial.ExpirationInterval.Quantity );
                                break;
                            case "months":
                                DefaultExpDate = DefaultExpDate.AddMonths( CswConvert.ToInt32( MaterialNodeAsMaterial.ExpirationInterval.Quantity ) );
                                break;
                            case "years":
                                DefaultExpDate = DefaultExpDate.AddYears( CswConvert.ToInt32( MaterialNodeAsMaterial.ExpirationInterval.Quantity ) );
                                break;
                            default:
                                DefaultExpDate = DateTime.MinValue;
                                break;
                        }
                        ExpirationDate.DateTimeValue = DefaultExpDate;
                    }

                    // case 24488 - When Location is modified, verify that:
                    //  the Material's Storage Compatibility is null,
                    //  or the Material's Storage Compatibility is one the selected values in the new Location.
                    if( Location.WasModified )
                    {
                        CswNbtNodePropImageList materialStorageCompatibilty = MaterialNode.Properties[CswNbtObjClassMaterial.StorageCompatibilityPropertyName];
                        CswNbtNode locationNode = _CswNbtResources.Nodes.GetNode( Location.SelectedNodeId );
                        if( null != locationNode ) //what if the user didn't specify a location?
                        {
                            CswNbtNodePropImageList locationStorageCompatibility = locationNode.Properties[CswNbtObjClassLocation.StorageCompatabilityPropertyName];
                            if( false == _isStorageCompatible( materialStorageCompatibilty.Value, locationStorageCompatibility.Value ) )
                            {
                                throw new CswDniException( ErrorType.Warning,
                                    "Storage compatibilities do not match, cannot move this container to specified location",
                                    "Storage compatibilities do not match, cannot move this container to specified location" );
                            }
                        }
                    }
                }
            }

            if( _CswNbtResources.EditMode == NodeEditMode.Add )
            {
                CswNbtSdInventoryLevelMgr Mgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
                Mgr.addToCurrentQuantity( Quantity.Quantity, Quantity.UnitId, "Container  [" + Barcode.Barcode + "] created.", Material.RelatedNodeId, Location.SelectedNodeId );
            }
            else
            {
                if( Location.WasModified &&
                    false == string.IsNullOrEmpty( Location.CachedNodeName ) &&
                    Location.CachedNodeName != CswNbtNodePropLocation.TopLevelName )
                {
                    if( CswConvert.ToInt32( Quantity.Quantity ) != 0 )
                    {
                        CswNbtSdInventoryLevelMgr Mgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
                        CswNbtNodePropWrapper LocationWrapper = Node.Properties[LocationPropertyName];
                        string PrevLocationId = LocationWrapper.GetOriginalPropRowValue( ( (CswNbtFieldTypeRuleLocation) _CswNbtResources.MetaData.getFieldTypeRule( LocationWrapper.getFieldType().FieldType ) ).NodeIdSubField.Column );
                        string Reason = "Container  [" + Barcode.Barcode + "] moved to new location: " + Location.CachedNodeName;
                        if( false == string.IsNullOrEmpty( PrevLocationId ) )
                        {
                            CswPrimaryKey PrevLocationPk = new CswPrimaryKey();
                            PrevLocationPk.FromString( PrevLocationId );
                            if( PrevLocationPk != Location.SelectedNodeId )
                            {
                                Mgr.changeLocationOfQuantity( Quantity.Quantity, Quantity.UnitId, Reason, Material.RelatedNodeId, PrevLocationPk, Location.SelectedNodeId );
                            }
                        }
                        else
                        {
                            Mgr.addToCurrentQuantity( Quantity.Quantity, Quantity.UnitId, Reason, Material.RelatedNodeId, Location.SelectedNodeId );
                        }
                    }
                }
                if( Quantity.WasModified )
                {
                    CswNbtSdInventoryLevelMgr Mgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
                    CswNbtNodePropWrapper QuantityWrapper = Node.Properties[QuantityPropertyName];
                    double PrevQuantity = CswConvert.ToDouble( QuantityWrapper.GetOriginalPropRowValue( ( (CswNbtFieldTypeRuleQuantity) _CswNbtResources.MetaData.getFieldTypeRule( QuantityWrapper.getFieldType().FieldType ) ).QuantitySubField.Column ) );
                    if( false == CswTools.IsDouble( PrevQuantity ) )
                    {
                        PrevQuantity = 0;
                    }
                    double Diff = Quantity.Quantity - PrevQuantity;
                    if( CswConvert.ToInt32( Diff ) != 0 )
                    {
                        string Reason = "Container [" + Barcode.Barcode + "] quantity changed by: " + Diff + " " + Quantity.CachedUnitName;
                        if( Disposed.Checked == Tristate.True )
                        {
                            Reason += " on disposal.";
                        }
                        Mgr.addToCurrentQuantity( Diff, Quantity.UnitId, Reason, Material.RelatedNodeId, Location.SelectedNodeId );
                    }
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

        public override void afterPopulateProps()
        {
            this.Dispense.setHidden( value: ( false == _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.Actions[CswNbtActionName.DispenseContainer] ) ), SaveToDb: false );
            if( this.Disposed.Checked == Tristate.False )
            {
                this.Undispose.setHidden( value: true, SaveToDb: true );
                this.Dispose.setHidden( value: ( false == _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Edit, _CswNbtResources.Actions[CswNbtActionName.DisposeContainer] ) ), SaveToDb: false );
            }
            else if( this.Disposed.Checked == Tristate.True )
            {
                this.Dispose.setHidden( value: true, SaveToDb: true );
                this.Undispose.setHidden( value: ( false == _CswNbtResources.Permit.canContainer( NodeId, CswNbtPermit.NodeTypePermission.Edit, _CswNbtResources.Actions[CswNbtActionName.UndisposeContainer] ) ), SaveToDb: false );
            }
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            // Disposed == false
            CswNbtMetaDataObjectClassProp DisposedOCP = ObjectClass.getObjectClassProp( DisposedPropertyName );

            //ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, DisposedOCP, Tristate.False.ToString() );

            CswNbtViewProperty viewProp = ParentRelationship.View.AddViewProperty( ParentRelationship, DisposedOCP );
            viewProp.ShowInGrid = false;
            ParentRelationship.View.AddViewPropertyFilter( viewProp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals, Value: Tristate.False.ToString() );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                if( OCP.PropName == DisposePropertyName )
                {
                    DisposeContainer();//case 26665
                    postChanges( true );
                    ButtonData.Action = NbtButtonAction.refresh;
                }
                else if( OCP.PropName == UndisposePropertyName )
                {
                    UndisposeContainer();
                    postChanges( true );
                    ButtonData.Action = NbtButtonAction.refresh;
                }
                else if( OCP.PropName == DispensePropertyName )
                {
                    //ActionData = this.NodeId.ToString();
                    ButtonData.Data = _getDispenseActionData();
                    ButtonData.Action = NbtButtonAction.dispense;
                }
                else
                {
                    CswNbtActSubmitRequest RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CswNbtActSystemViews.SystemViewName.CISProRequestCart );

                    CswNbtObjClassRequestItem NodeAsRequestItem = RequestAct.makeRequestItem( new CswNbtActSubmitRequest.RequestItem(), NodeId, OCP );
                    NodeAsRequestItem.Material.RelatedNodeId = Material.RelatedNodeId;
                    NodeAsRequestItem.Material.setReadOnly( value: true, SaveToDb: false );

                    NodeAsRequestItem.Location.SelectedNodeId = Location.SelectedNodeId;
                    NodeAsRequestItem.Location.RefreshNodeName();

                    switch( OCP.PropName )
                    {
                        case RequestDispensePropertyName:
                            NodeAsRequestItem.Quantity.UnitId = Quantity.UnitId;
                            break;
                        case RequestDisposePropertyName:
                            NodeAsRequestItem.Material.setHidden( value: true, SaveToDb: false );
                            //Not sure why this was here, but it creates duplicate dispose requests
                            //NodeAsRequestItem.postChanges( true ); /* This is the only condition in which we want to commit the node upfront. */
                            break;
                        case RequestMovePropertyName:
                            NodeAsRequestItem.Material.setHidden( value: true, SaveToDb: false );
                            break;
                    }

                    ButtonData.Data["titleText"] = OCP.PropName + " [" + Barcode.Barcode + "]";
                    ButtonData.Data["requestaction"] = OCP.PropName;
                    ButtonData.Data["requestItemProps"] = RequestAct.getRequestItemAddProps( NodeAsRequestItem );
                    ButtonData.Data["requestItemNodeTypeId"] = RequestAct.RequestItemNt.NodeTypeId;

                    ButtonData.Action = NbtButtonAction.request;
                }
            }
            return true;
        }
        #endregion Inherited Events

        #region Custom Logic


        public void DisposeContainer()
        {
            _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispose, -this.Quantity.Quantity, this.Quantity.UnitId );
            this.Quantity.Quantity = 0;
            this.Disposed.Checked = Tristate.True;
            _setDisposedReadOnly( true );
        }

        public void UndisposeContainer()
        {
            CswNbtMetaDataNodeType ContDispTransNT = _CswNbtResources.MetaData.getNodeType( "Container Dispense Transaction" );
            CswNbtObjClassContainerDispenseTransaction ContDispTransNode = _getMostRecentDisposeTransaction( ContDispTransNT );

            if( ContDispTransNode != null )
            {
                this.Quantity.Quantity = ContDispTransNode.QuantityDispensed.Quantity;
                this.Quantity.UnitId = ContDispTransNode.QuantityDispensed.UnitId;
                ContDispTransNode.Node.delete();
            }
            this.Disposed.Checked = Tristate.False;
            _setDisposedReadOnly( false );
        }

        /// <summary>
        /// Dispense out of this container.
        /// </summary>
        /// <param name="DispenseType"></param>
        /// <param name="QuantityToDeduct">Positive quantity to subtract</param>
        /// <param name="UnitId"></param>
        /// <param name="RequestItemId"></param>
        /// <param name="DestinationContainer"></param>
        public void DispenseOut( CswNbtObjClassContainerDispenseTransaction.DispenseType DispenseType, double QuantityToDeduct, CswPrimaryKey UnitId,
                                 CswPrimaryKey RequestItemId = null, CswNbtObjClassContainer DestinationContainer = null, bool RecordTransaction = true )
        {
            double RealQuantityToDeduct = _getDispenseAmountInProperUnits( QuantityToDeduct, UnitId, this.Quantity.UnitId );
            this.Quantity.Quantity = this.Quantity.Quantity - RealQuantityToDeduct;

            if( DestinationContainer != null )
            {
                DestinationContainer.DispenseIn( DispenseType, QuantityToDeduct, UnitId, RequestItemId, this, false );  // false, because we do not want another duplicate transaction record
            }
            if( RecordTransaction )
            {
                _createContainerTransactionNode( DispenseType, -RealQuantityToDeduct, this.Quantity.UnitId, RequestItemId, this, DestinationContainer );
            }
        } // DispenseOut()

        /// <summary>
        /// Dispense into this container.  
        /// </summary>
        /// <param name="DispenseType"></param>
        /// <param name="QuantityToAdd">Positive quantity to add</param>
        /// <param name="UnitId"></param>
        /// <param name="RequestItemId"></param>
        /// <param name="SourceContainer"></param>
        public void DispenseIn( CswNbtObjClassContainerDispenseTransaction.DispenseType DispenseType, double QuantityToAdd, CswPrimaryKey UnitId,
                                CswPrimaryKey RequestItemId = null, CswNbtObjClassContainer SourceContainer = null, bool RecordTransaction = true )
        {
            double RealQuantityToAdd = _getDispenseAmountInProperUnits( QuantityToAdd, UnitId, this.Quantity.UnitId );
            this.Quantity.Quantity = this.Quantity.Quantity + RealQuantityToAdd;
            if( RecordTransaction )
            {
                _createContainerTransactionNode( DispenseType, RealQuantityToAdd, this.Quantity.UnitId, RequestItemId, SourceContainer, this );
            }
        } // DispenseIn()

        #endregion Custom Logic

        #region Private Helper Methods

        private double _getDispenseAmountInProperUnits( double Quantity, CswPrimaryKey OldUnitId, CswPrimaryKey NewUnitId )
        {
            double convertedValue = Quantity;
            if( null != OldUnitId && null != NewUnitId )
            {
                CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( _CswNbtResources, OldUnitId, NewUnitId, this.Material.RelatedNodeId );
                convertedValue = ConversionObj.convertUnit( Quantity );
            }
            return convertedValue;
        }

        private JObject _getDispenseActionData()
        {
            JObject ActionDataObj = new JObject();
            ActionDataObj["sourceContainerNodeId"] = this.NodeId.ToString();
            ActionDataObj["barcode"] = Barcode.Barcode;
            ActionDataObj["materialname"] = Material.CachedNodeName;
            ActionDataObj["location"] = Location.CachedFullPath;

            CswNbtObjClassUnitOfMeasure unitNode = _CswNbtResources.Nodes.GetNode( this.Quantity.UnitId );
            if( null != unitNode )
            {
                ActionDataObj["currentQuantity"] = this.Quantity.Quantity;
                ActionDataObj["currentUnitName"] = unitNode.Name.Text;
            }
            JObject CapacityObj = _getCapacityJSON();
            ActionDataObj["capacity"] = CapacityObj.ToString();
            return ActionDataObj;
        }

        private JObject _getCapacityJSON()
        {
            JObject CapacityObj = new JObject();
            CswNbtObjClassSize sizeNode = _CswNbtResources.Nodes.GetNode( this.Size.RelatedNodeId );
            if( null != sizeNode )
            {
                CswNbtNodePropQuantity Capacity = sizeNode.Capacity;
                Capacity.ToJSON( CapacityObj );
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Cannot dispense container: Container's size is undefined.", "Dispense fail - null Size relationship." );
            }
            return CapacityObj;
        }

        private CswNbtObjClassContainerDispenseTransaction _getMostRecentDisposeTransaction( CswNbtMetaDataNodeType ContDispTransNT )
        {
            CswNbtObjClassContainerDispenseTransaction ContDispTransNode = null;
            if( ContDispTransNT != null )
            {
                CswNbtView DisposedContainerTransactionsView = new CswNbtView( _CswNbtResources );
                DisposedContainerTransactionsView.ViewName = "ContDispTransDisposed";
                CswNbtViewRelationship ParentRelationship = DisposedContainerTransactionsView.AddViewRelationship( ContDispTransNT, false );

                DisposedContainerTransactionsView.AddViewPropertyAndFilter(
                    ParentRelationship,
                    ContDispTransNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.SourceContainerPropertyName ),
                    this.NodeId.PrimaryKey.ToString(),
                    CswNbtSubField.SubFieldName.NodeID,
                    false,
                    CswNbtPropFilterSql.PropertyFilterMode.Equals
                    );

                DisposedContainerTransactionsView.AddViewPropertyAndFilter(
                    ParentRelationship,
                    ContDispTransNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.TypePropertyName ),
                    CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispose.ToString(),
                    CswNbtSubField.SubFieldName.Value,
                    false,
                    CswNbtPropFilterSql.PropertyFilterMode.Equals
                    );

                ICswNbtTree DispenseTransactionTree = _CswNbtResources.Trees.getTreeFromView( DisposedContainerTransactionsView, false, true );
                int NumOfTransactions = DispenseTransactionTree.getChildNodeCount();
                if( NumOfTransactions > 0 )
                {
                    DispenseTransactionTree.goToNthChild( 0 );
                    ContDispTransNode = DispenseTransactionTree.getNodeForCurrentPosition();
                }
            }
            return ContDispTransNode;
        }

        /// <summary>
        /// Record a container dispense transaction
        /// </summary>
        /// <param name="DispenseType"></param>
        /// <param name="Quantity">Quantity adjustment (negative for dispenses, disposes, and wastes, positive for receiving and add)</param>
        /// <param name="UnitId"></param>
        /// <param name="RequestItemId"></param>
        /// <param name="SourceContainer"></param>
        /// <param name="DestinationContainer"></param>
        private void _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType DispenseType, double Quantity, CswPrimaryKey UnitId, CswPrimaryKey RequestItemId = null,
                                                      CswNbtObjClassContainer SourceContainer = null, CswNbtObjClassContainer DestinationContainer = null )
        {
            CswNbtMetaDataNodeType ContDispTransNT = _CswNbtResources.MetaData.getNodeType( "Container Dispense Transaction" );
            if( ContDispTransNT != null )
            {
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContDispTransNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );

                if( SourceContainer != null )
                {
                    ContDispTransNode.SourceContainer.RelatedNodeId = SourceContainer.NodeId;
                    ContDispTransNode.RemainingSourceContainerQuantity.Quantity = SourceContainer.Quantity.Quantity;
                    ContDispTransNode.RemainingSourceContainerQuantity.UnitId = SourceContainer.Quantity.UnitId;
                }
                if( DestinationContainer != null )
                {
                    ContDispTransNode.DestinationContainer.RelatedNodeId = DestinationContainer.NodeId;
                }
                ContDispTransNode.QuantityDispensed.Quantity = Quantity;
                ContDispTransNode.QuantityDispensed.UnitId = UnitId;
                ContDispTransNode.Type.Value = DispenseType.ToString();
                ContDispTransNode.DispensedDate.DateTimeValue = DateTime.Now;
                if( null != RequestItemId && Int32.MinValue != RequestItemId.PrimaryKey )
                {
                    ContDispTransNode.RequestItem.RelatedNodeId = RequestItemId;
                }
                ContDispTransNode.postChanges( false );
            } // if( ContDispTransNT != null )
        } // _createContainerTransactionNode


        private void _setDisposedReadOnly( bool isReadOnly )//case 25814
        {
            this.Barcode.setReadOnly( value: isReadOnly, SaveToDb: true );
            this.Material.setReadOnly( value: isReadOnly, SaveToDb: true );
            this.Location.setReadOnly( value: isReadOnly, SaveToDb: true );
            this.Status.setReadOnly( value: isReadOnly, SaveToDb: true );
            this.Missing.setReadOnly( value: isReadOnly, SaveToDb: true );
            this.SourceContainer.setReadOnly( value: isReadOnly, SaveToDb: true );
            this.ExpirationDate.setReadOnly( value: isReadOnly, SaveToDb: true );
            this.Size.setReadOnly( value: isReadOnly, SaveToDb: true );
            this.RequestDispense.setReadOnly( value: isReadOnly, SaveToDb: true );
            this.RequestMove.setReadOnly( value: isReadOnly, SaveToDb: true );
            this.Dispense.setReadOnly( value: isReadOnly, SaveToDb: true );
        }

        private bool _isStorageCompatible( CswDelimitedString materialStorageCompatibility, CswDelimitedString locationStorageCompatibilities )
        {
            bool ret = false;
            if( materialStorageCompatibility.Count == 0 ) //if storage compatibility on the material is null, it can go anywhere
            {
                ret = true;
            }
            foreach( string matComp in materialStorageCompatibility ) //loop through the materials storage compatibilities
            {
                if( matComp.Contains( "0w.gif" ) ) //if it has '0-none' selected, it can go anywhere
                {
                    ret = true;
                }
            }
            foreach( string comp in locationStorageCompatibilities )
            {
                if( materialStorageCompatibility.Contains( comp ) || comp.Contains( "0w.gif" ) ) //if the locations storage compatibility matches OR it has '0-none', it can house the material
                {
                    ret = true;
                }
            }
            return ret;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[BarcodePropertyName] ); } }
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[LocationPropertyName] ); } }
        public CswNbtNodePropDateTime LocationVerified { get { return ( _CswNbtNode.Properties[LocationVerifiedPropertyName] ); } }
        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[MaterialPropertyName] ); } }
        public CswNbtNodePropList Status { get { return ( _CswNbtNode.Properties[StatusPropertyName] ); } }
        public CswNbtNodePropLogical Missing { get { return ( _CswNbtNode.Properties[MissingPropertyName] ); } }
        public CswNbtNodePropLogical Disposed { get { return ( _CswNbtNode.Properties[DisposedPropertyName] ); } }
        public CswNbtNodePropRelationship SourceContainer { get { return ( _CswNbtNode.Properties[SourceContainerPropertyName] ); } }
        public CswNbtNodePropQuantity Quantity { get { return ( _CswNbtNode.Properties[QuantityPropertyName] ); } }
        public CswNbtNodePropDateTime ExpirationDate { get { return ( _CswNbtNode.Properties[ExpirationDatePropertyName] ); } }
        public CswNbtNodePropRelationship Size { get { return ( _CswNbtNode.Properties[SizePropertyName] ); } }
        public CswNbtNodePropButton RequestDispense { get { return ( _CswNbtNode.Properties[RequestDispensePropertyName] ); } }
        public CswNbtNodePropButton RequestDispose { get { return ( _CswNbtNode.Properties[RequestDisposePropertyName] ); } }
        public CswNbtNodePropButton RequestMove { get { return ( _CswNbtNode.Properties[RequestMovePropertyName] ); } }
        public CswNbtNodePropButton Dispense { get { return ( _CswNbtNode.Properties[DispensePropertyName] ); } }
        public CswNbtNodePropButton Dispose { get { return ( _CswNbtNode.Properties[DisposePropertyName] ); } }
        public CswNbtNodePropButton Undispose { get { return ( _CswNbtNode.Properties[UndisposePropertyName] ); } }
        public CswNbtNodePropRelationship Owner { get { return ( _CswNbtNode.Properties[OwnerPropertyName] ); } }
        #endregion


    }//CswNbtObjClassContainer

}//namespace ChemSW.Nbt.ObjClasses
