using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Actions
{
    /// <summary>
    /// Utility class for Create Material logic
    /// </summary>
    public class CswNbtActCreateMaterial
    {
        /// <summary>
        /// Helper class for creating a New Material node with an eye toward strict validation
        /// </summary>
        public class NewMaterial
        {
            private CswNbtResources _NbtResources;
            private Int32 _NodeTypeId;
            private string _TradeName;
            private CswPrimaryKey _SupplierId;
            private string _PartNo;
            private CswNbtObjClassMaterial _ExistingNode = null;
            private CswNbtMetaDataNodeType _MaterialNt = null;
            private CswNbtObjClassVendor _Supplier = null;
            private string _NodeTypeName;

            /// <summary>
            /// Standard constructor for validating required properties
            /// </summary>
            public NewMaterial( CswNbtResources CswNbtResources, Int32 NodeTypeId, string TradeName, CswPrimaryKey SupplierId, string PartNo = "" )
            {
                _NbtResources = CswNbtResources;
                this.NodeTypeId = NodeTypeId;
                this.TradeName = TradeName;
                this.SupplierId = SupplierId;
                this.PartNo = PartNo;
            }

            /// <summary>
            /// Secondary constructor for continuing work on a new Material node
            /// </summary>
            public NewMaterial( CswNbtResources CswNbtResources, CswNbtNode Node )
            {
                _NbtResources = CswNbtResources;
                if( Node.ObjClass.ObjectClass.ObjectClass != NbtObjectClass.MaterialClass )
                {
                    throw new CswDniException( ErrorType.Warning, "Cannot create a new Material object without a valid Material Type.", "Could not resolve NodeType for NodeTypeId: " + NodeTypeId + "." );
                }
                this.Node = Node;
            }

            public Int32 NodeTypeId
            {
                get { return _NodeTypeId; }
                set
                {
                    Int32 Id = value;
                    CswNbtMetaDataNodeType PotentialNt = _NbtResources.MetaData.getNodeType( Id );
                    if( null == PotentialNt )
                    {
                        throw new CswDniException( ErrorType.Warning, "Cannot create a new Material object without a valid Material Type.", "Could not resolve NodeType for NodeTypeId: " + NodeTypeId + "." );
                    }
                    if( PotentialNt.getObjectClass().ObjectClass != NbtObjectClass.MaterialClass )
                    {
                        throw new CswDniException( ErrorType.Warning, "Cannot create a new Material object without a valid Material Type.", "Cannot make a Material for Object Class: " + PotentialNt.getObjectClass().ObjectClass + "." );
                    }
                    _NodeTypeName = PotentialNt.NodeTypeName;
                    _MaterialNt = PotentialNt;
                    _NodeTypeId = Id;
                }
            }

            public string NodeTypeName
            {
                get { return _NodeTypeName; }
            }

            public string TradeName
            {
                get { return _TradeName; }
                set
                {
                    string PotentialName = value;
                    if( String.IsNullOrEmpty( PotentialName ) )
                    {
                        throw new CswDniException( ErrorType.Warning, "A Tradename is required to create a new Material.", "Provided Tradename was null or empty." );
                    }

                    _TradeName = PotentialName;
                }
            }

            public CswPrimaryKey SupplierId
            {
                get { return _SupplierId; }
                set
                {
                    CswNbtObjClassVendor PotentialSupplier = _NbtResources.Nodes[value];
                    if( null == PotentialSupplier || PotentialSupplier.ObjectClass.ObjectClass != NbtObjectClass.VendorClass )
                    {
                        throw new CswDniException( ErrorType.Warning, "Cannot create a new Material object without a valid Supplier.", "Provided SupplierId was invalid." );
                    }

                    _Supplier = PotentialSupplier;
                    _SupplierId = PotentialSupplier.NodeId;
                }
            }

            public string SupplierName
            {
                get { return _Supplier.VendorName.Text; }
            }

            public string PartNo
            {
                get { return _PartNo; }
                set { _PartNo = value; }
            }

            public bool existsInDb( bool ForceRecalc = false )
            {
                return null != existingMaterial( ForceRecalc );
            }

            public CswNbtObjClassMaterial existingMaterial( bool ForceRecalc = false )
            {
                if( ForceRecalc || null == _ExistingNode )
                {
                    _ExistingNode = CswNbtObjClassMaterial.getExistingMaterial( _NbtResources, NodeTypeId, SupplierId, TradeName, PartNo );
                }
                return _ExistingNode;
            }

            public CswNbtObjClassMaterial commit( bool UpversionTemp = false )
            {
                CswNbtObjClassMaterial Ret = null;
                if( null == Node ) //Don't commit twice
                {
                    if( existsInDb() )
                    {
                        throw new CswDniException( ErrorType.Warning, "A material with the same Type, Tradename, Supplier and PartNo already exists.", "A material with this configuration already exists. Name: " + _ExistingNode.NodeName + " , ID: " + _ExistingNode.NodeId + "." );
                    }
                    if( false == existsInDb() && Int32.MinValue != NodeTypeId )
                    {
                        Ret = _NbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp, OverrideUniqueValidation: false );
                        Node = Ret.Node;
                        //TODO: Improve default handling here
                        Ret.PhysicalState.Value = CswNbtObjClassMaterial.PhysicalStates.Solid;
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Warning, "Cannot create a new Material object without a valid Supplier.", "Provided SupplierId was invalid." );
                    }
                }
                else
                {
                    Ret = Node;
                }

                Ret.TradeName.Text = TradeName;
                Ret.PartNumber.Text = PartNo;
                Ret.Supplier.RelatedNodeId = SupplierId;

                Ret.IsTemp = ( false == UpversionTemp );
                Ret.postChanges( ForceUpdate: false );

                return Ret;
            }


            public CswNbtNode Node { get; private set; }
        }


        #region ctor

        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Base Constructor
        /// </summary>
        public CswNbtActCreateMaterial( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            if( false == _CswNbtResources.Permit.can( CswNbtActionName.Create_Material, _CswNbtResources.CurrentNbtUser ) )
            {
                throw new CswDniException( ErrorType.Error, "You do not have permission to use the Create Material wizard.", "Attempted to access the Create Material wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }
        }

        #endregion ctor

        private JObject _tryCreateMaterial( Int32 MaterialNodeTypeId, CswPrimaryKey SupplierId, string TradeName, string PartNo )
        {
            JObject Ret = new JObject();

            NewMaterial PotentialMaterial = new NewMaterial( _CswNbtResources, MaterialNodeTypeId, TradeName, SupplierId, PartNo );

            Ret["materialexists"] = PotentialMaterial.existsInDb();
            if( false == PotentialMaterial.existsInDb() )
            {
                CswNbtObjClassMaterial NodeAsMaterial = PotentialMaterial.commit();
                if( null != NodeAsMaterial )
                {
                    Ret["materialid"] = NodeAsMaterial.NodeId.ToString();
                    Ret["tradename"] = NodeAsMaterial.TradeName.Text;
                    Ret["partno"] = NodeAsMaterial.PartNumber.Text;
                    Ret["supplier"] = NodeAsMaterial.Supplier.CachedNodeName;
                    Ret["nodetypeid"] = NodeAsMaterial.NodeTypeId;
                    _CswNbtResources.EditMode = NodeEditMode.Temp;
                    CswNbtSdTabsAndProps SdProps = new CswNbtSdTabsAndProps( _CswNbtResources );
                    Ret["properties"] = SdProps.getProps( NodeAsMaterial.Node, string.Empty, null, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                    Int32 DocumentNodeTypeId = CswNbtActReceiving.getMaterialDocumentNodeTypeId( _CswNbtResources, NodeAsMaterial );
                    if( Int32.MinValue != DocumentNodeTypeId )
                    {
                        Ret["documenttypeid"] = DocumentNodeTypeId;
                    }
                }
                else
                {
                    Ret["noderef"] = NodeAsMaterial.Node.NodeLink; //for the link
                }
            }

            return Ret;
        }


        #region Public

        /// <summary>
        /// Creates a new material, if one does not already exist, and returns the material nodeid
        /// </summary>
        public JObject createMaterial( Int32 NodeTypeId, string SupplierId, string Tradename, string PartNo )
        {
            return _tryCreateMaterial( NodeTypeId, CswConvert.ToPrimaryKey( SupplierId ), Tradename, PartNo );
        }

        public static JObject getSizeNodeProps( CswNbtResources CswNbtResources, Int32 SizeNodeTypeId, string SizeDefinition, bool WriteNode )
        {
            JObject SizeObj = CswConvert.ToJObject( SizeDefinition, true, "size" );
            CswNbtNode SizeNode;
            return getSizeNodeProps( CswNbtResources, SizeNodeTypeId, SizeObj, WriteNode, out SizeNode );
        }

        public static JObject getSizeNodeProps( CswNbtResources CswNbtResources, Int32 SizeNodeTypeId, string SizeDefinition, bool WriteNode, out CswNbtNode SizeNode )
        {
            JObject SizeObj = CswConvert.ToJObject( SizeDefinition, true, "size" );
            return getSizeNodeProps( CswNbtResources, SizeNodeTypeId, SizeObj, WriteNode, out SizeNode );
        }

        public static JObject getSizeNodeProps( CswNbtResources CswNbtResources, Int32 SizeNodeTypeId, JObject SizeObj, bool WriteNode, out CswNbtNode SizeNode )
        {
            JObject Ret = new JObject();

            SizeNode = CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, true );
            CswPrimaryKey UnitIdPK = CswConvert.ToPrimaryKey( SizeObj["unitid"].ToString() );
            if( null != UnitIdPK )
            {
                CswNbtObjClassSize NodeAsSize = (CswNbtObjClassSize) SizeNode;
                NodeAsSize.InitialQuantity.Quantity = CswConvert.ToDouble( SizeObj["quantity"] );
                NodeAsSize.InitialQuantity.UnitId = UnitIdPK;
                NodeAsSize.CatalogNo.Text = SizeObj["catalogNo"].ToString();
                NodeAsSize.QuantityEditable.Checked = CswConvert.ToTristate( SizeObj["quantEditableChecked"] );
                NodeAsSize.Dispensable.Checked = CswConvert.ToTristate( SizeObj["dispensibleChecked"] );
                NodeAsSize.UnitCount.Value = CswConvert.ToDouble( SizeObj["unitCount"] );

                JArray Row = new JArray();
                Ret["row"] = Row;

                Row.Add( "(New Size)" );
                Row.Add( NodeAsSize.InitialQuantity.Gestalt );
                Row.Add( NodeAsSize.Dispensable.Gestalt );
                Row.Add( NodeAsSize.QuantityEditable.Gestalt );
                Row.Add( NodeAsSize.CatalogNo.Gestalt );
                Row.Add( NodeAsSize.UnitCount.Gestalt );

                if( ( Tristate.False == NodeAsSize.QuantityEditable.Checked && false == CswTools.IsDouble( NodeAsSize.InitialQuantity.Quantity ) )
                    || false == CswTools.IsDouble( NodeAsSize.UnitCount.Value ) )
                {
                    SizeNode = null; //Case 27665 - instead of throwing a serverside warning, just throw out the size
                }
                else if( WriteNode )
                {
                    SizeNode.postChanges( true );
                }
            }
            else
            {
                SizeNode = null;
            }

            return Ret;
        }

        public static JObject getMaterialSizes( CswNbtResources CswNbtResources, CswPrimaryKey MaterialId )
        {
            JObject Ret = new JObject();

            if( null == MaterialId )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get material's sizes without a valid materialid.",
                                           "Attempted to call getMaterialSizes with invalid or empty parameters." );
            }

            CswNbtNode MaterialNode = CswNbtResources.Nodes.GetNode( MaterialId );

            if( null == MaterialNode ||
                MaterialNode.getObjectClass().ObjectClass != NbtObjectClass.MaterialClass )
            {
                throw new CswDniException( ErrorType.Error,
                                           "The provided node was not a valid material.",
                                           "Attempted to call getMaterialSizes with a node that was not valid." );
            }

            CswNbtView SizesView = new CswNbtView( CswNbtResources ); //MaterialNode.getNodeType().CreateDefaultView();
            SizesView.ViewMode = NbtViewRenderingMode.Grid;
            CswNbtMetaDataObjectClass SizeOc = CswNbtResources.MetaData.getObjectClass( NbtObjectClass.SizeClass );

            CswNbtMetaDataObjectClassProp SizeMaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
            CswNbtViewRelationship SizeRel = SizesView.AddViewRelationship( SizeOc, false );

            SizesView.AddViewPropertyAndFilter( SizeRel, SizeMaterialOcp, MaterialId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
            SizesView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.InitialQuantity ) );
            SizesView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Dispensable ) );
            SizesView.AddViewProperty( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.QuantityEditable ) );

            return Ret;
        }

        private CswNbtNode _commitMaterialNode( JObject MaterialObj )
        {
            CswNbtNode Ret = null;

            JObject MaterialProperties = (JObject) MaterialObj["properties"];
            CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );

            Int32 MaterialNodeTypeId = CswConvert.ToInt32( MaterialObj["materialnodetypeid"] );
            if( Int32.MinValue != MaterialNodeTypeId )
            {
                CswNbtMetaDataNodeType MaterialNt = _CswNbtResources.MetaData.getNodeType( MaterialNodeTypeId );
                if( null != MaterialNt )
                {
                    _CswNbtResources.EditMode = NodeEditMode.Edit;
                    Ret = _CswNbtResources.Nodes[CswConvert.ToString( MaterialObj["materialId"] )];
                    if( null != Ret )
                    {
                        Ret.IsTemp = false;
                        SdTabsAndProps.saveProps( Ret.NodeId, Int32.MinValue, MaterialProperties.ToString(), Ret.NodeTypeId, null, IsIdentityTab: false );

                        NewMaterial FinalMaterial = new NewMaterial( _CswNbtResources, Ret );
                        FinalMaterial.TradeName = CswConvert.ToString( MaterialObj["tradename"] );
                        FinalMaterial.SupplierId = CswConvert.ToPrimaryKey( CswConvert.ToString( MaterialObj["supplierid"] ) );
                        FinalMaterial.PartNo = CswConvert.ToString( MaterialObj["partno"] );

                        CswNbtObjClassMaterial NodeAsMaterial = FinalMaterial.commit();

                        JObject RequestObj = CswConvert.ToJObject( MaterialObj["request"] );
                        if( RequestObj.HasValues )
                        {
                            CswNbtObjClassRequestMaterialCreate RequestCreate = _CswNbtResources.Nodes[CswConvert.ToString( RequestObj["requestitemid"] )];
                            if( null != RequestCreate )
                            {
                                RequestCreate.Material.RelatedNodeId = FinalMaterial.Node.NodeId;
                                RequestCreate.Status.Value = CswNbtObjClassRequestMaterialCreate.Statuses.Created;
                                RequestCreate.Fulfill.State = CswNbtObjClassRequestMaterialCreate.FulfillMenu.Complete;
                                RequestCreate.Fulfill.MenuOptions = CswNbtObjClassRequestMaterialCreate.FulfillMenu.Complete;
                                RequestCreate.postChanges( ForceUpdate: false );
                            }
                        }
                        CswNbtActReceiving.commitDocumentNode( _CswNbtResources, NodeAsMaterial, MaterialObj );
                    }
                }

                if( null == Ret )
                {
                    throw new CswDniException( ErrorType.Error,
                                               "Failed to create new material.",
                                               "Attempted to call _commitMaterialNode failed." );
                }
            }
            return Ret;
        }



        /// <summary>
        /// Finalize the new Material
        /// </summary>
        public JObject commitMaterial( string MaterialDefinition )
        {
            JObject RetObj = new JObject();

            JObject MaterialObj = CswConvert.ToJObject( MaterialDefinition );
            if( MaterialObj.HasValues )
            {
                JArray SizesArray = CswConvert.ToJArray( MaterialObj["sizeNodes"] );
                CswPrimaryKey MaterialId = new CswPrimaryKey();
                MaterialId.FromString( CswConvert.ToString( MaterialObj["materialId"] ) );
                if( CswTools.IsPrimaryKey( MaterialId ) )
                {
                    CswNbtNode MaterialNode = _CswNbtResources.Nodes[MaterialId];
                    if( null != MaterialNode )
                    {
                        /* 1. Validate the new material and get its properties and sizes */
                        MaterialNode = _commitMaterialNode( MaterialObj );
                        RetObj["createdmaterial"] = true;

                        /* 2. Add the sizes */
                        SizesArray = _removeDuplicateSizes( SizesArray );
                        _addMaterialSizes( SizesArray, MaterialNode );
                        RetObj["sizescount"] = SizesArray.Count;

                        /* 3. Add landingpage data */
                        RetObj["landingpagedata"] = _getLandingPageData( MaterialNode );
                    }
                }
            }
            return RetObj;
        }

        private JArray _removeDuplicateSizes( JArray SizesArray )
        {
            JArray UniqueSizesArray = new JArray();
            foreach( JObject SizeObj in SizesArray )
            {
                bool addSizeToCompare = true;
                if( SizeObj.HasValues )
                {
                    foreach( JObject UniqueSizeObj in UniqueSizesArray )
                    {
                        if( UniqueSizeObj["unitid"].ToString() == SizeObj["unitid"].ToString() &&
                            UniqueSizeObj["quantity"].ToString() == SizeObj["quantity"].ToString() &&
                            UniqueSizeObj["catalogNo"].ToString() == SizeObj["catalogNo"].ToString() )
                        {
                            addSizeToCompare = false;
                        }
                    }
                    if( addSizeToCompare )
                    {
                        UniqueSizesArray.Add( SizeObj );
                    }
                }
            }
            return UniqueSizesArray;
        }

        /// <summary>
        /// Make nodes for defined sizes, else remove undefinable sizes from the JArray
        /// </summary>
        private void _addMaterialSizes( JArray SizesArray, CswNbtNode MaterialNode )
        {
            JArray ArrayToIterate = (JArray) SizesArray.DeepClone();
            foreach( JObject SizeObj in ArrayToIterate )
            {
                if( SizeObj.HasValues )
                {
                    CswNbtNode SizeNode;
                    Int32 SizeNtId = CswConvert.ToInt32( SizeObj["nodetypeid"] );
                    if( Int32.MinValue != SizeNtId )
                    {
                        getSizeNodeProps( _CswNbtResources, SizeNtId, SizeObj, false, out SizeNode );
                        if( null != SizeNode )
                        {
                            CswNbtObjClassSize NodeAsSize = SizeNode;
                            NodeAsSize.Material.RelatedNodeId = MaterialNode.NodeId;
                            SizeNode.postChanges( true );
                        }
                        else
                        {
                            SizesArray.Remove( SizeObj );
                        }
                    }
                    else
                    {
                        SizesArray.Remove( SizeObj );
                    }
                }
                else
                {
                    SizesArray.Remove( SizeObj );
                }
            }
        }

        private JObject _getLandingPageData( CswNbtNode MaterialNode )
        {
            return getLandingPageData( _CswNbtResources, MaterialNode );
        }

        /// <summary>
        /// Get a landing page for a Material
        /// </summary>
        public static JObject getLandingPageData( CswNbtResources NbtResources, CswNbtNode MaterialNode, CswNbtView MaterialNodeView = null )
        {
            JObject Ret = new JObject();
            if( null != MaterialNode )
            {
                MaterialNodeView = MaterialNodeView ?? CswNbtObjClassMaterial.getMaterialNodeView( NbtResources, MaterialNode );
                MaterialNodeView.SaveToCache( IncludeInQuickLaunch: false );

                Ret["ActionId"] = NbtResources.Actions[CswNbtActionName.Create_Material].ActionId.ToString();
                //Used for Tab and Button items
                Ret["NodeId"] = MaterialNode.NodeId.ToString();
                Ret["NodeViewId"] = MaterialNodeView.SessionViewId.ToString();
                //Used for node-specific Add items
                Ret["RelatedNodeId"] = MaterialNode.NodeId.ToString();
                Ret["RelatedNodeName"] = MaterialNode.NodeName;
                Ret["RelatedNodeTypeId"] = MaterialNode.NodeTypeId.ToString();
                Ret["RelatedObjectClassId"] = MaterialNode.getObjectClassId().ToString();
                //If (and when) action landing pages are slated to be roleId-specific, remove this line
                Ret["isConfigurable"] = NbtResources.CurrentNbtUser.IsAdministrator();
                //Used for viewing new material
                Ret["ActionLinks"] = new JObject();
                string ActionLinkName = MaterialNode.NodeId.ToString();
                Ret["ActionLinks"][ActionLinkName] = new JObject();
                Ret["ActionLinks"][ActionLinkName]["Text"] = MaterialNode.NodeName;
                Ret["ActionLinks"][ActionLinkName]["ViewId"] = MaterialNodeView.SessionViewId.ToString();
            }
            return Ret;
        }

        public static JObject getMaterialUnitsOfMeasure( string MaterialId, CswNbtResources CswNbtResources )
        {
            JObject ret = new JObject();
            string PhysicalState = CswNbtObjClassMaterial.PhysicalStates.Solid;
            CswNbtObjClassMaterial Material = CswNbtResources.Nodes[MaterialId];
            if( null != Material &&
                false == string.IsNullOrEmpty( Material.PhysicalState.Value ) )
            {
                PhysicalState = Material.PhysicalState.Value;
            }

            CswNbtUnitViewBuilder unitViewBuilder = new CswNbtUnitViewBuilder( CswNbtResources );

            CswNbtView unitsView = unitViewBuilder.getQuantityUnitOfMeasureView( PhysicalState );

            Collection<CswNbtNode> _UnitNodes = new Collection<CswNbtNode>();
            ICswNbtTree UnitsTree = CswNbtResources.Trees.getTreeFromView( CswNbtResources.CurrentNbtUser, unitsView, true, false, false );
            UnitsTree.goToRoot();
            for( int i = 0; i < UnitsTree.getChildNodeCount(); i++ )
            {
                UnitsTree.goToNthChild( i );
                _UnitNodes.Add( UnitsTree.getNodeForCurrentPosition() );
                UnitsTree.goToParentNode();
            }

            foreach( CswNbtNode unitNode in _UnitNodes )
            {
                CswNbtObjClassUnitOfMeasure nodeAsUnitOfMeasure = (CswNbtObjClassUnitOfMeasure) unitNode;
                ret[nodeAsUnitOfMeasure.NodeId.ToString()] = nodeAsUnitOfMeasure.Name.Gestalt;
            }

            return ret;
        }

        public JObject getSizeLogicalsVisibility( int SizeNodeTypeId )
        {
            JObject ret = new JObject();
            ret["showQuantityEditable"] = "false";
            ret["showDispensable"] = "false";
            CswNbtMetaDataNodeType SizeNt = _CswNbtResources.MetaData.getNodeType( SizeNodeTypeId );
            if( null != SizeNt )
            {
                CswNbtMetaDataNodeTypeProp QuantityEditable = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.QuantityEditable );
                if( null != QuantityEditable.AddLayout )
                {
                    ret["showQuantityEditable"] = "true";
                }
                CswNbtMetaDataNodeTypeProp Dispensable = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.Dispensable );
                if( null != Dispensable.AddLayout )
                {
                    ret["showDispensable"] = "true";
                }
            }
            return ret;
        }

        #endregion Public


    }


}