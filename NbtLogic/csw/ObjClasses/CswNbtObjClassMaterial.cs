using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMaterial : CswNbtObjClass
    {
        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassMaterial( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass ); }
        }

        public sealed class PropertyName
        {
            public const string Supplier = "Supplier";
            public const string ApprovalStatus = "Approval Status";
            public const string PartNumber = "Part Number";
            public const string SpecificGravity = "Specific Gravity";
            public const string PhysicalState = "Physical State";
            public const string CasNo = "CAS No";
            public const string RegulatoryLists = "Regulatory Lists";
            public const string Tradename = "Tradename";
            public const string StorageCompatibility = "Storage Compatibility";
            public const string ExpirationInterval = "Expiration Interval";
            public const string Request = "Request";
            public const string Receive = "Receive";
            public const string MaterialId = "Material Id";
            public const string Approved = "Approved";
            public const string ManufacturingSites = "Manufacturing Sites";
            public const string UNCode = "UN Code";
            public const string IsTierII = "Is Tier II";
            public const string ViewSDS = "View SDS";
        }

        public sealed class PhysicalStates
        {
            public const string NA = "n/a";
            public const string Liquid = "liquid";
            public const string Solid = "solid";
            public const string Gas = "gas";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Solid, Liquid, NA };
        }

        public sealed class Requests
        {
            public const string Bulk = "Request By Bulk";
            public const string Size = "Request By Size";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Bulk, Size };
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMaterial
        /// </summary>
        public static implicit operator CswNbtObjClassMaterial( CswNbtNode Node )
        {
            CswNbtObjClassMaterial ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.MaterialClass ) )
            {
                ret = (CswNbtObjClassMaterial) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            Request.MenuOptions = Requests.Options.ToString();
            Request.State = Requests.Size;

            if( ApprovalStatus.WasModified )
            {
                Receive.setHidden( value: ApprovalStatus.Checked != Tristate.True, SaveToDb: true );
            }

            if( CasNo.WasModified )
            {
                CswCommaDelimitedString ParentMaterials = new CswCommaDelimitedString();
                getParentMaterials( ref ParentMaterials );
                if( ParentMaterials.Count > 0 ) //this material is used by others as a component...we have no idea how deep the rabbit hole is...make a batch op to update 
                {
                    ParentMaterials.Add( NodeId.ToString() ); //we need to update this material too, so add it to the list
                    CswNbtBatchOpUpdateRegulatoryListsForMaterials BatchOp = new CswNbtBatchOpUpdateRegulatoryListsForMaterials( _CswNbtResources );
                    BatchOp.makeBatchOp( ParentMaterials );
                }
                else //this material isn't used as a component anywhere, so just update it by its self
                {
                    _updateRegulatoryLists();
                }
            }

            CswNbtMetaDataNodeTypeProp ChemicalHazardClassesNTP = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypeId, "Hazard Classes" );
            if( null != ChemicalHazardClassesNTP )
            {
                CswNbtMetaDataObjectClass FireClassExemptAmountOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountClass );
                CswNbtMetaDataNodeType FireClassExemptAmountNT = FireClassExemptAmountOC.FirstNodeType;
                if( null != FireClassExemptAmountNT )
                {
                    CswNbtMetaDataNodeTypeProp FireClassHazardTypesNTP =
                        _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp(
                            FireClassExemptAmountNT.NodeTypeId,
                            CswNbtObjClassFireClassExemptAmount.PropertyName.FireHazardClassType
                            );
                    ChemicalHazardClassesNTP.ListOptions = FireClassHazardTypesNTP.ListOptions;
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
            _CswNbtResources.StructureSearchManager.DeleteFingerprintRecord( this.NodeId.PrimaryKey );

            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _toggleButtonVisibility();
            PhysicalState.SetOnPropChange( _physicalStatePropChangeHandler );
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        private void _toggleButtonVisibility()
        {
            Receive.setHidden( value: false == _CswNbtResources.Permit.can( CswNbtActionName.Receiving ), SaveToDb: false );
            Request.setHidden( value: false == _CswNbtResources.Permit.can( CswNbtActionName.Submit_Request ), SaveToDb: false );
        }

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                bool HasPermission = false;
                switch( OCP.PropName )
                {
                    case PropertyName.Request:
                        if( _CswNbtResources.Permit.can( CswNbtActionName.Submit_Request ) )
                        {
                            HasPermission = true;
                            CswNbtActRequesting RequestAct = new CswNbtActRequesting( _CswNbtResources );

                            CswNbtPropertySetRequestItem NodeAsPropSet = RequestAct.makeMaterialRequestItem( new CswNbtActRequesting.RequestItem( CswNbtActRequesting.RequestItem.Material ), NodeId, ButtonData );
                            NodeAsPropSet.postChanges( false );

                            ButtonData.Data["requestaction"] = OCP.PropName;
                            ButtonData.Data["titleText"] = ButtonData.SelectedText + " for " + TradeName.Text;
                            ButtonData.Data["requestItemProps"] = RequestAct.getRequestItemAddProps( NodeAsPropSet );
                            ButtonData.Data["requestItemNodeTypeId"] = NodeAsPropSet.NodeTypeId;
                            ButtonData.Action = NbtButtonAction.request;
                        }
                        break;
                    case PropertyName.Receive:
                        if( _CswNbtResources.Permit.can( CswNbtActionName.Receiving ) )
                        {
                            HasPermission = true;
                            ButtonData.Data["state"] = new JObject();
                            ButtonData.Data["state"]["materialId"] = NodeId.ToString();
                            ButtonData.Data["state"]["materialNodeTypeId"] = NodeTypeId;
                            ButtonData.Data["state"]["tradeName"] = TradeName.Text;
                            CswNbtActReceiving Act = new CswNbtActReceiving( _CswNbtResources, ObjectClass, NodeId );
                            //ButtonData.Data["sizesViewId"] = Act.SizesView.SessionViewId.ToString();
                            Int32 ContainerLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.container_receipt_limit.ToString() ) );
                            ButtonData.Data["state"]["containerlimit"] = ContainerLimit;
                            CswNbtObjClassContainer Container = Act.makeContainer();
                            Container.Location.SelectedNodeId = _CswNbtResources.CurrentNbtUser.DefaultLocationId;
                            ButtonData.Data["state"]["containerNodeTypeId"] = Container.NodeTypeId;
                            ButtonData.Data["state"]["containerAddLayout"] = Act.getContainerAddProps( Container );
                            bool customBarcodes = CswConvert.ToBoolean( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.custom_barcodes.ToString() ) );
                            ButtonData.Data["state"]["customBarcodes"] = customBarcodes;
                            ButtonData.Data["state"]["nodetypename"] = this.NodeType.NodeTypeName;

                            CswDateTime CswDate = new CswDateTime( _CswNbtResources, getDefaultExpirationDate() );
                            if( false == CswDate.IsNull )
                            {
                                foreach( JProperty child in ButtonData.Data["state"]["containerAddLayout"].Children() )
                                {
                                    JToken name = child.First.SelectToken( "name" );
                                    if( name.ToString() == "Expiration Date" )
                                    {
                                        ButtonData.Data["state"]["containerAddLayout"][child.Name]["values"]["value"] = CswDate.ToClientAsDateTimeJObject();
                                    }
                                }
                            }

                            Int32 DocumentNodeTypeId = CswNbtActReceiving.getMaterialDocumentNodeTypeId( _CswNbtResources, this );
                            if( Int32.MinValue != DocumentNodeTypeId )
                            {
                                ButtonData.Data["state"]["documentTypeId"] = DocumentNodeTypeId;
                            }

                            ButtonData.Action = NbtButtonAction.receive;
                        }
                        break;
                    case PropertyName.ViewSDS:
                        HasPermission = true;

                        CswNbtMetaDataObjectClass documentOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.DocumentClass );
                        CswNbtMetaDataObjectClassProp archivedOCP = documentOC.getObjectClassProp( CswNbtObjClassDocument.PropertyName.Archived );
                        CswNbtMetaDataObjectClassProp docClassOCP = documentOC.getObjectClassProp( CswNbtObjClassDocument.PropertyName.DocumentClass );

                        CswNbtView docView = new CswNbtView( _CswNbtResources );
                        CswNbtViewRelationship parent = docView.AddViewRelationship( documentOC, true );
                        docView.AddViewPropertyAndFilter( parent,
                            MetaDataProp: docClassOCP,
                            Value: CswNbtObjClassDocument.DocumentClasses.SDS,
                            FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

                        if( ButtonData.SelectedText.Equals( ViewSDS.PropName ) ) //get the SDS that matches users jurisdiction
                        {
                            CswNbtObjClassUser currentUserNode = _CswNbtResources.Nodes[_CswNbtResources.CurrentNbtUser.UserId];
                            CswNbtObjClassJurisdiction userJurisdictionNode = _CswNbtResources.Nodes[currentUserNode.Jurisdiction.RelatedNodeId];

                            if( null != userJurisdictionNode )
                            {
                                CswNbtMetaDataObjectClassProp formatOCP = documentOC.getObjectClassProp( CswNbtObjClassDocument.PropertyName.Format );
                                CswNbtMetaDataObjectClassProp languageOCP = documentOC.getObjectClassProp( CswNbtObjClassDocument.PropertyName.Language );

                                docView.AddViewPropertyAndFilter( parent,
                                    MetaDataProp: formatOCP,
                                    Value: userJurisdictionNode.Format.Value,
                                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

                                docView.AddViewPropertyAndFilter( parent,
                                    MetaDataProp: languageOCP,
                                    Value: userJurisdictionNode.Language.Value,
                                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                            }
                            else
                            {
                                ButtonData.Message = "User does not have a Jurisdiction selected, cannot find matching SDS";
                                ButtonData.Action = NbtButtonAction.nothing;
                            }
                        }
                        else //get the SDS the user selected
                        {
                            CswNbtMetaDataObjectClassProp titleOCP = documentOC.getObjectClassProp( CswNbtObjClassDocument.PropertyName.Title );
                            docView.AddViewPropertyAndFilter( parent,
                                MetaDataProp: titleOCP,
                                Value: ButtonData.SelectedText,
                                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                        }

                        if( string.IsNullOrEmpty( ButtonData.Message ) ) //only iterate the tree and find an SDS if there isn't an error msg
                        {
                            ICswNbtTree docTree = _CswNbtResources.Trees.getTreeFromView( docView, false, false, false );
                            int childCount = docTree.getChildNodeCount();
                            if( childCount > 0 )
                            {
                                for( int i = 0; i < childCount; i++ )
                                {
                                    docTree.goToNthChild( i );
                                    ButtonData.Data["nodeid"] = docTree.getNodeIdForCurrentPosition().ToString();
                                    ButtonData.Data["title"] = docTree.getNodeNameForCurrentPosition();
                                    ButtonData.Action = NbtButtonAction.editprop;
                                    docTree.goToParentNode();
                                }
                            }
                            else
                            {
                                ButtonData.Message = "Could not find a matching SDS for your Jurisdiction";
                                ButtonData.Action = NbtButtonAction.nothing;
                            }
                        }

                        break;
                }
                if( false == HasPermission )
                {
                    throw new CswDniException( ErrorType.Warning, "You do not have permission to the " + OCP.PropName + " action.", "You do not have permission to the " + OCP.PropName + " action." );
                }
            }

            return true;
        }
        #endregion

        #region Custom Logic

        /// <summary>
        /// Calculates the expiration date from today based on the Material's Expiration Interval
        /// </summary>
        public DateTime getDefaultExpirationDate()
        {
            DateTime DefaultExpDate = DateTime.Now;
            switch( this.ExpirationInterval.CachedUnitName.ToLower() )
            {
                case "seconds":
                    DefaultExpDate = DefaultExpDate.AddSeconds( this.ExpirationInterval.Quantity );
                    break;
                case "minutes":
                    DefaultExpDate = DefaultExpDate.AddMinutes( this.ExpirationInterval.Quantity );
                    break;
                case "hours":
                    DefaultExpDate = DefaultExpDate.AddHours( this.ExpirationInterval.Quantity );
                    break;
                case "days":
                    DefaultExpDate = DefaultExpDate.AddDays( this.ExpirationInterval.Quantity );
                    break;
                case "weeks":
                    DefaultExpDate = DefaultExpDate.AddDays( this.ExpirationInterval.Quantity * 7 );
                    break;
                case "months":
                    DefaultExpDate = DefaultExpDate.AddMonths( CswConvert.ToInt32( this.ExpirationInterval.Quantity ) );
                    break;
                case "years":
                    DefaultExpDate = DefaultExpDate.AddYears( CswConvert.ToInt32( this.ExpirationInterval.Quantity ) );
                    break;
                default:
                    DefaultExpDate = DateTime.MinValue;
                    break;
            }
            return DefaultExpDate;
        }

        private void _updateRegulatoryLists()
        {
            CswNbtMetaDataObjectClass regListOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RegulatoryListClass );
            foreach( CswNbtObjClassRegulatoryList nodeAsRegList in regListOC.getNodes( false, false ) )
            {
                CswCommaDelimitedString CASNos = new CswCommaDelimitedString();
                CASNos.FromString( nodeAsRegList.CASNumbers.Text );
                if( CASNos.Contains( CasNo.Text ) )
                {
                    RegulatoryLists.StaticText += "," + nodeAsRegList.Name.Text;
                }
            }
        }

        /// <summary>
        /// Gets all the node ids of materials that use this material as a component
        /// </summary>
        /// <returns></returns>
        public void getParentMaterials( ref CswCommaDelimitedString MachingMaterialIDs )
        {
            CswNbtMetaDataObjectClass materialComponentOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClassProp constituentOCP = materialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            CswNbtMetaDataObjectClassProp mixtureOCP = materialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );

            CswNbtView componentsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = componentsView.AddViewRelationship( materialComponentOC, false );
            componentsView.AddViewPropertyAndFilter( parent, constituentOCP,
                Value: NodeId.PrimaryKey.ToString(),
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                SubFieldName: CswNbtSubField.SubFieldName.NodeID );
            componentsView.AddViewRelationship( parent, NbtViewPropOwnerType.First, mixtureOCP, false );

            ICswNbtTree componentsTree = _CswNbtResources.Trees.getTreeFromView( componentsView, false, false, false );
            int nodesCount = componentsTree.getChildNodeCount();
            for( int i = 0; i < nodesCount; i++ )
            {
                componentsTree.goToNthChild( i );
                int childNodesCount = componentsTree.getChildNodeCount();
                for( int c = 0; c < childNodesCount; c++ )
                {
                    componentsTree.goToNthChild( c );
                    MachingMaterialIDs.Add( componentsTree.getNodeIdForCurrentPosition().ToString() ); //the mixture node id
                    componentsTree.goToParentNode();
                }
                componentsTree.goToParentNode();
            }
        }

        public static CswNbtView getMaterialNodeView( CswNbtResources NbtResources, CswNbtNode MaterialNode )
        {
            CswNbtView Ret = null;
            if( MaterialNode != null )
            {
                Ret = MaterialNode.getViewOfNode();
                CswNbtMetaDataObjectClass SizeOc = NbtResources.MetaData.getObjectClass( NbtObjectClass.SizeClass );
                CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
                Ret.AddViewRelationship( Ret.Root.ChildRelationships[0], NbtViewPropOwnerType.Second, MaterialOcp, false );
                Ret.ViewName = "New Material: " + MaterialNode.NodeName;
            }
            return Ret;
        }

        public static CswNbtView getMaterialNodeView( CswNbtResources NbtResources, Int32 NodeTypeId, string Tradename, CswPrimaryKey SupplierId, string PartNo = "" )
        {
            if( Int32.MinValue == NodeTypeId ||
                false == CswTools.IsPrimaryKey( SupplierId ) ||
                String.IsNullOrEmpty( Tradename ) )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get a material without a type, a supplier and a tradename.",
                                           "Attempted to call _getMaterialNodeView with invalid or empty parameters. Type: " + NodeTypeId + ", Tradename: " + Tradename + ", SupplierId: " + SupplierId );
            }

            CswNbtView Ret = new CswNbtView( NbtResources );
            Ret.ViewMode = NbtViewRenderingMode.Tree;
            Ret.Visibility = NbtViewVisibility.User;
            Ret.VisibilityUserId = NbtResources.CurrentNbtUser.UserId;
            CswNbtMetaDataNodeType MaterialNt = NbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtViewRelationship MaterialRel = Ret.AddViewRelationship( MaterialNt, false );
            CswNbtMetaDataNodeTypeProp TradeNameNtp = MaterialNt.getNodeTypePropByObjectClassProp( PropertyName.Tradename );
            CswNbtMetaDataNodeTypeProp SupplierNtp = MaterialNt.getNodeTypePropByObjectClassProp( PropertyName.Supplier );
            CswNbtMetaDataNodeTypeProp PartNoNtp = MaterialNt.getNodeTypePropByObjectClassProp( PropertyName.PartNumber );

            Ret.AddViewPropertyAndFilter( MaterialRel, TradeNameNtp, Tradename );
            Ret.AddViewPropertyAndFilter( MaterialRel, SupplierNtp, SupplierId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
            Ret.AddViewPropertyAndFilter( MaterialRel, PartNoNtp, PartNo );

            CswNbtMetaDataObjectClass SizeOc = NbtResources.MetaData.getObjectClass( NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
            Ret.AddViewRelationship( MaterialRel, NbtViewPropOwnerType.Second, MaterialOcp, false );

            Ret.ViewName = "New Material: " + Tradename;

            return Ret;
        }

        /// <summary>
        /// Fetch a Material node by NodeTypeId, TradeName, Supplier and PartNo (Optional). This method will throw if required parameters are null or empty.
        /// </summary>
        public static CswNbtObjClassMaterial getExistingMaterial( CswNbtResources NbtResources, Int32 MaterialNodeTypeId, CswPrimaryKey SupplierId, string TradeName, string PartNo )
        {
            CswNbtObjClassMaterial Ret = null;

            CswNbtView MaterialNodeView = getMaterialNodeView( NbtResources, MaterialNodeTypeId, TradeName, SupplierId, PartNo );
            ICswNbtTree Tree = NbtResources.Trees.getTreeFromView( MaterialNodeView, false, false, false );
            bool MaterialExists = Tree.getChildNodeCount() > 0;

            if( MaterialExists )
            {
                Tree.goToNthChild( 0 );
                Ret = Tree.getNodeForCurrentPosition();
            }
            return Ret;
        }

        /// <summary>
        /// Gets all material documents of this material and adds them to the View SDS menu button options. Does NOT post changes!!
        /// </summary>
        public void UpdateViewSDSButtonOpts()
        {
            CswNbtMetaDataObjectClass documentOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.DocumentClass );
            CswNbtMetaDataObjectClassProp materialOCP = documentOC.getObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
            CswNbtMetaDataObjectClassProp archivedOCP = documentOC.getObjectClassProp( CswNbtObjClassDocument.PropertyName.Archived );
            CswNbtMetaDataObjectClassProp documentClassOCP = documentOC.getObjectClassProp( CswNbtObjClassDocument.PropertyName.DocumentClass );

            CswNbtView sdsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = sdsView.AddViewRelationship( documentOC, true );

            sdsView.AddViewPropertyAndFilter( parent,
                MetaDataProp: materialOCP,
                SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                Value: NodeId.PrimaryKey.ToString() );

            sdsView.AddViewPropertyAndFilter( parent,
                MetaDataProp: archivedOCP,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                Value: CswConvert.ToDbVal( false ).ToString() );

            sdsView.AddViewPropertyAndFilter( parent,
                MetaDataProp: documentClassOCP,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                Value: CswNbtObjClassDocument.DocumentClasses.SDS );

            CswCommaDelimitedString viewSDSMenuOpts = new CswCommaDelimitedString();
            viewSDSMenuOpts.Add( ViewSDS.PropName );

            ICswNbtTree sdsTree = _CswNbtResources.Trees.getTreeFromView( sdsView, false, false, false );
            int childCount = sdsTree.getChildNodeCount();
            for( int i = 0; i < childCount; i++ )
            {
                sdsTree.goToNthChild( i );
                CswNbtObjClassDocument doc = sdsTree.getNodeForCurrentPosition();
                viewSDSMenuOpts.Add( doc.Title.Text );
                sdsTree.goToParentNode();
            }

            ViewSDS.MenuOptions = viewSDSMenuOpts.ToString();
        }

        #endregion Custom Logic

        #region Object class specific properties

        public CswNbtNodePropRelationship Supplier { get { return ( _CswNbtNode.Properties[PropertyName.Supplier] ); } }
        public CswNbtNodePropLogical ApprovalStatus { get { return ( _CswNbtNode.Properties[PropertyName.ApprovalStatus] ); } }
        public CswNbtNodePropText PartNumber { get { return ( _CswNbtNode.Properties[PropertyName.PartNumber] ); } }
        public CswNbtNodePropNumber SpecificGravity { get { return ( _CswNbtNode.Properties[PropertyName.SpecificGravity] ); } }
        public CswNbtNodePropList PhysicalState { get { return ( _CswNbtNode.Properties[PropertyName.PhysicalState] ); } }
        public CswNbtNodePropCASNo CasNo { get { return ( _CswNbtNode.Properties[PropertyName.CasNo] ); } }
        public CswNbtNodePropStatic RegulatoryLists { get { return ( _CswNbtNode.Properties[PropertyName.RegulatoryLists] ); } }
        public CswNbtNodePropText TradeName { get { return ( _CswNbtNode.Properties[PropertyName.Tradename] ); } }
        public CswNbtNodePropImageList StorageCompatibility { get { return ( _CswNbtNode.Properties[PropertyName.StorageCompatibility] ); } }
        public CswNbtNodePropQuantity ExpirationInterval { get { return ( _CswNbtNode.Properties[PropertyName.ExpirationInterval] ); } }
        public CswNbtNodePropButton Request { get { return ( _CswNbtNode.Properties[PropertyName.Request] ); } }
        private void _physicalStatePropChangeHandler( CswNbtNodeProp prop )
        {
            if( false == String.IsNullOrEmpty( PhysicalState.Value ) )
            {
                CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                CswNbtView unitsOfMeasureView = Vb.getQuantityUnitOfMeasureView( _CswNbtNode.NodeId );
                if( null != unitsOfMeasureView )
                {
                    unitsOfMeasureView.save();
                }
            }
        }
        public CswNbtNodePropButton Receive { get { return ( _CswNbtNode.Properties[PropertyName.Receive] ); } }
        public CswNbtNodePropSequence MaterialId { get { return ( _CswNbtNode.Properties[PropertyName.MaterialId] ); } }
        public CswNbtNodePropLogical Approved { get { return ( _CswNbtNode.Properties[PropertyName.Approved] ); } }
        public CswNbtNodePropGrid ManufacturingSites { get { return ( _CswNbtNode.Properties[PropertyName.ManufacturingSites] ); } }
        public CswNbtNodePropRelationship UNCode { get { return ( _CswNbtNode.Properties[PropertyName.UNCode] ); } }
        public CswNbtNodePropLogical IsTierII { get { return ( _CswNbtNode.Properties[PropertyName.IsTierII] ); } }
        public CswNbtNodePropButton ViewSDS { get { return ( _CswNbtNode.Properties[PropertyName.ViewSDS] ); } }

        #endregion
    }//CswNbtObjClassMaterial

}//namespace ChemSW.Nbt.ObjClasses
