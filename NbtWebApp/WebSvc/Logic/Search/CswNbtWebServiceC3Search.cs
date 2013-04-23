using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceC3Search
    {
        #region Ctor

        private static CswNbtResources _CswNbtResources;

        public CswNbtWebServiceC3Search( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #endregion Ctor

        #region DataContracts

        [DataContract]
        public class CswNbtC3SearchReturn : CswWebSvcReturn
        {
            public CswNbtC3SearchReturn()
            {
                Data = new C3SearchResponse();
            }

            [DataMember]
            public C3SearchResponse Data;
        }

        [DataContract]
        public class C3SearchResponse
        {
            [DataMember]
            public Collection<DataSource> AvailableDataSources = new Collection<DataSource>();

            [DataMember]
            public List<string> SearchTypes = new List<string>();

            [DataMember]
            public string SearchResults = string.Empty;

            [DataMember]
            public CswC3Product ProductDetails = null;
        }

        [DataContract]
        public class DataSource
        {
            [DataMember]
            public string value = string.Empty;

            [DataMember]
            public string display = string.Empty;
        }

        [DataContract]
        public class CswNbtC3CreateMaterialReturn : CswWebSvcReturn
        {
            public CswNbtC3CreateMaterialReturn()
            {
                Data = new C3CreateMaterialResponse();
            }

            [DataMember]
            public C3CreateMaterialResponse Data;
        }

        [DataContract]
        public class C3CreateMaterialResponse
        {
            [DataMember]
            public string actionname = string.Empty;

            [DataMember]
            public State state = null;

            [DataMember]
            public bool success;

            [DataContract]
            public class State
            {
                [DataMember]
                public string materialId = string.Empty;

                [DataMember]
                public string partNo = string.Empty;

                [DataMember]
                public string tradeName = string.Empty;

                //[DataMember]
                //public bool useExistingTempNode;

                [DataMember]
                public Collection<Collection<SizeColumnValue>> sizes;

                [DataMember]
                public Supplier supplier = null;

                [DataMember]
                public bool addNewC3Supplier = false;

                [DataMember]
                public MaterialType materialType = null;

                [DataMember]
                public string documentId = string.Empty;

                [DataContract]
                public class MaterialType
                {
                    [DataMember]
                    public string name = string.Empty;

                    [DataMember]
                    public Int32 val = Int32.MinValue;
                }

                [DataContract]
                public class Supplier
                {
                    [DataMember]
                    public string name = string.Empty;

                    [DataMember]
                    public string val = string.Empty;
                }

                [DataContract]
                public class SizeColumnValue
                {
                    [DataMember]
                    public string value { get; set; }

                    [DataMember]
                    public bool hidden { get; set; }
                }

            }
        }

        #endregion

        public static void GetAvailableDataSources( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3Params CswC3Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3Params );
            ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

            CswRetObjSearchResults SourcesList = C3SearchClient.getDataSources( CswC3Params );

            //todo: catch error when SourcesList returns null

            Collection<DataSource> AvailableDataSources = new Collection<DataSource>();

            //Create the "All Sources" option
            CswCommaDelimitedString AllSources = new CswCommaDelimitedString();
            AllSources.FromArray( SourcesList.AvailableDataSources );

            DataSource allSourcesDs = new DataSource();
            allSourcesDs.value = AllSources.ToString();
            allSourcesDs.display = "All Sources";
            AvailableDataSources.Add( allSourcesDs );

            //Add available data source options
            foreach( string DataSource in SourcesList.AvailableDataSources )
            {
                DataSource dS = new DataSource();
                dS.value = DataSource;
                dS.display = DataSource;
                AvailableDataSources.Add( dS );
            }

            Return.Data.AvailableDataSources = AvailableDataSources;

        }

        public static void GetSearchTypes( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3Params CswC3Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3Params );
            ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

            List<string> newlist = new List<string>();

            foreach( CswC3SearchParams.SearchFieldType SearchType in Enum.GetValues( typeof( CswC3SearchParams.SearchFieldType ) ) )
            {
                newlist.Add( SearchType.ToString() );
            }

            Return.Data.SearchTypes = newlist;

        }

        public static void GetC3ProductDetails( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3SearchParams CswC3SearchParams )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

            CswRetObjSearchResults SearchResults = C3SearchClient.getProductDetails( CswC3SearchParams );
            if( SearchResults.CswC3SearchResults.Length > 0 )
            {
                ChemCatCentral.CswC3Product C3ProductDetails = SearchResults.CswC3SearchResults[0];
                Return.Data.ProductDetails = C3ProductDetails;
            }
        }

        public static void RunChemCatCentralSearch( ICswResources CswResources, CswNbtC3SearchReturn Return, CswC3SearchParams CswC3SearchParams )
        {
            JObject ret = new JObject();

            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

            CswRetObjSearchResults SearchResults = C3SearchClient.search( CswC3SearchParams );

            CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources, null, Int32.MinValue );
            ret["table"] = wsTable.getTable( SearchResults );
            ret["filters"] = "";
            ret["searchterm"] = CswC3SearchParams.Query;
            ret["filtersapplied"] = "";
            //Search.SaveToCache( true );
            ret["sessiondataid"] = "";
            ret["searchtype"] = "chemcatcentral";

            Return.Data.SearchResults = ret.ToString();

        }

        public static void importC3Product( ICswResources CswResources, CswNbtC3CreateMaterialReturn Return, Int32 ProductId )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
            ChemCatCentral.CswC3Product C3ProductDetails = new CswC3Product();

            CswC3SearchParams.Field = "ProductId";
            CswC3SearchParams.Query = CswConvert.ToString( ProductId );

            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

            // Perform C3 search to get the product details
            CswRetObjSearchResults SearchResults = C3SearchClient.getProductDetails( CswC3SearchParams );
            if( SearchResults.CswC3SearchResults.Length > 0 )
            {
                C3ProductDetails = SearchResults.CswC3SearchResults[0];
            }

            // When a product is imported, the nodetype will default to 'Chemical'
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataNodeType ChemicalNT = ChemicalOC.FirstNodeType;
            if( null != ChemicalNT )
            {
                // Instance the ImportManger
                ImportManager C3Import = new ImportManager( _CswNbtResources, C3ProductDetails );

                // Create the temporary material node
                CswNbtObjClassChemical C3ProductTempNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId, CswEnumNbtMakeNodeOperation.MakeTemp );

                //Set the c3productid property
                C3ProductTempNode.C3ProductId.Text = C3ProductDetails.ProductId.ToString();

                // Add props to the tempnode
                C3Import.addNodeTypeProps( C3ProductTempNode.Node );

                // Assign hazard classes if they exist and if FireDb Sync is enabled
                C3ProductTempNode.syncFireDbData();

                C3ProductTempNode.postChanges( false );

                // Get or create a vendor node
                C3CreateMaterialResponse.State.Supplier Supplier = C3Import.createVendorNode( C3ProductDetails.SupplierName );

                // Create size node(s)
                Collection<Collection<C3CreateMaterialResponse.State.SizeColumnValue>> ProductSizes = C3Import.createSizeNodes( C3ProductTempNode );

                // Create synonyms node(s)
                C3Import.createMaterialSynonyms( C3ProductTempNode );

                // Create a document node
                CswPrimaryKey SDSDocumentNodeId = C3Import.createMaterialDocument( C3ProductTempNode );

                #region Return Object

                Return.Data.success = true;
                Return.Data.actionname = "create material";

                C3CreateMaterialResponse.State.MaterialType MaterialType = new C3CreateMaterialResponse.State.MaterialType();
                MaterialType.name = ChemicalNT.NodeTypeName;
                MaterialType.val = ChemicalNT.NodeTypeId;

                C3CreateMaterialResponse.State State = new C3CreateMaterialResponse.State();
                State.materialId = C3ProductTempNode.NodeId.ToString();
                State.tradeName = C3ProductTempNode.TradeName.Text;
                State.partNo = C3ProductTempNode.PartNumber.Text;
                //State.useExistingTempNode = true;
                State.supplier = Supplier;
                if( string.IsNullOrEmpty( State.supplier.val ) )
                {
                    State.addNewC3Supplier = true;
                }
                State.materialType = MaterialType;
                State.sizes = ProductSizes;
                if( null != SDSDocumentNodeId )
                {
                    State.documentId = SDSDocumentNodeId.ToString();
                }

                Return.Data.state = State;

                #endregion Return Object
            }

        }

        #region Import Mananger

        private class ImportManager
        {
            private CswNbtResources _CswNbtResources;
            private Dictionary<string, C3Mapping> _Mappings;
            private CswC3Product _ProductToImport;

            public ImportManager( CswNbtResources CswNbtResources, CswC3Product ProductToImport )
            {
                _CswNbtResources = CswNbtResources;
                _ProductToImport = ProductToImport;
                _initMappings();
            }

            #region Private helper methods

            private void _initMappings()
            {
                _Mappings = new Dictionary<string, C3Mapping>();

                #region Chemical

                CswNbtMetaDataNodeType ChemicalNT = _CswNbtResources.MetaData.getNodeType( "Chemical" );
                if( null != ChemicalNT )
                {
                    #region Object Class Properties

                    const string Tradename = CswNbtObjClassChemical.PropertyName.TradeName;
                    _Mappings.Add( Tradename, new C3Mapping
                    {
                        NBTNodeTypeId = ChemicalNT.NodeTypeId,
                        C3ProductPropertyValue = _ProductToImport.TradeName,
                        NBTNodeTypePropId = ChemicalNT.getNodeTypePropIdByObjectClassProp( Tradename ),
                        NBTSubFieldPropColName = "field1"
                    } );

                    const string CasNo = CswNbtObjClassChemical.PropertyName.CasNo;
                    _Mappings.Add( CasNo, new C3Mapping
                    {
                        NBTNodeTypeId = ChemicalNT.NodeTypeId,
                        C3ProductPropertyValue = _ProductToImport.CasNo,
                        NBTNodeTypePropId = ChemicalNT.getNodeTypePropIdByObjectClassProp( CasNo ),
                        NBTSubFieldPropColName = "field1"
                    } );

                    const string PartNumber = CswNbtObjClassChemical.PropertyName.PartNumber;
                    _Mappings.Add( PartNumber, new C3Mapping
                    {
                        NBTNodeTypeId = ChemicalNT.NodeTypeId,
                        C3ProductPropertyValue = _ProductToImport.PartNo,
                        NBTNodeTypePropId = ChemicalNT.getNodeTypePropIdByObjectClassProp( PartNumber ),
                        NBTSubFieldPropColName = "field1"
                    } );

                    // THIS IS DEFAULTING TO SOLID FOR NOW
                    //todo: write a method that attempts to figure out the physical state by looking at the incoming UOM
                    const string PhysicalState = CswNbtObjClassChemical.PropertyName.PhysicalState;
                    _Mappings.Add( PhysicalState, new C3Mapping
                    {
                        NBTNodeTypeId = ChemicalNT.NodeTypeId,
                        C3ProductPropertyValue = CswNbtPropertySetMaterial.CswEnumPhysicalState.Liquid,
                        NBTNodeTypePropId = ChemicalNT.getNodeTypePropIdByObjectClassProp( PhysicalState ),
                        NBTSubFieldPropColName = "field1"
                    } );

                    #endregion Object Class Properties

                    #region NodeType Properties

                    CswNbtMetaDataNodeTypeProp Formula = ChemicalNT.getNodeTypeProp( "Formula" );
                    if( null != Formula )
                    {
                        _Mappings.Add( "Formula", new C3Mapping
                        {
                            NBTNodeTypeId = ChemicalNT.NodeTypeId,
                            C3ProductPropertyValue = _ProductToImport.Formula,
                            NBTNodeTypePropId = Formula.PropId,
                            NBTSubFieldPropColName = "field1"
                        } );
                    }

                    CswNbtMetaDataNodeTypeProp Structure = ChemicalNT.getNodeTypeProp( "Structure" );
                    if( null != Structure )
                    {
                        _Mappings.Add( "Structure", new C3Mapping
                        {
                            NBTNodeTypeId = ChemicalNT.NodeTypeId,
                            C3ProductPropertyValue = _ProductToImport.MolData,
                            NBTNodeTypePropId = Structure.PropId,
                            NBTSubFieldPropColName = "ClobData"
                        } );
                    }


                    CswNbtMetaDataNodeTypeProp PhysicalDescription = ChemicalNT.getNodeTypeProp( "Physical Description" );
                    if( null != PhysicalDescription )
                    {
                        _Mappings.Add( "Physical Description", new C3Mapping
                        {
                            NBTNodeTypeId = ChemicalNT.NodeTypeId,
                            C3ProductPropertyValue = _ProductToImport.Description,
                            NBTNodeTypePropId = PhysicalDescription.PropId,
                            NBTSubFieldPropColName = "gestalt"
                        } );
                    }

                    // Add any additional properties
                    foreach( CswC3Product.TemplateSlctdExtData NameValuePair in _ProductToImport.TemplateSelectedExtensionData )
                    {
                        string PropertyName = NameValuePair.attribute;
                        CswNbtMetaDataNodeTypeProp ChemicalNTP = ChemicalNT.getNodeTypeProp( PropertyName );
                        if( null != ChemicalNTP )
                        {
                            _Mappings.Add( PropertyName, new C3Mapping
                            {
                                NBTNodeTypeId = ChemicalNT.NodeTypeId,
                                C3ProductPropertyValue = NameValuePair.value,
                                NBTNodeTypePropId = ChemicalNTP.PropId,
                                NBTSubFieldPropColName = "field1"
                            } );
                        }
                    }

                    #endregion
                }

                //TODO: In the future, add the MSDS link if it exists

                #endregion

                #region Vendor

                CswNbtMetaDataNodeType VendorNT = _CswNbtResources.MetaData.getNodeType( "Vendor" );
                if( null != VendorNT )
                {
                    const string VendorName = CswNbtObjClassVendor.PropertyName.VendorName;
                    _Mappings.Add( VendorName, new C3Mapping
                    {
                        NBTNodeTypeId = VendorNT.NodeTypeId,
                        C3ProductPropertyValue = _ProductToImport.SupplierName,
                        NBTNodeTypePropId = VendorNT.getNodeTypePropIdByObjectClassProp( VendorName ),
                        NBTSubFieldPropColName = "field1"
                    } );
                }

                #endregion

                #region Sizes

                CswNbtMetaDataNodeType SizeNT = _CswNbtResources.MetaData.getNodeType( "Size" );
                if( null != SizeNT )
                {
                    const string InitialQuantity = CswNbtObjClassSize.PropertyName.InitialQuantity;
                    _Mappings.Add( InitialQuantity, new C3Mapping
                    {
                        NBTNodeTypeId = SizeNT.NodeTypeId,
                        NBTNodeTypePropId = SizeNT.getNodeTypePropIdByObjectClassProp( InitialQuantity ),
                        NBTSubFieldPropColName = "field1_numeric",
                        NBTSubFieldPropColName2 = "field1"
                    } );

                    //const string CatalogNo = CswNbtObjClassSize.PropertyName.CatalogNo;
                    //_Mappings.Add( CatalogNo, new C3Mapping
                    //{
                    //    NBTNodeTypeId = SizeNT.NodeTypeId,
                    //    C3ProductPropertyValue = _ProductToImport.CatalogNo,
                    //    NBTNodeTypePropId = SizeNT.getNodeTypePropIdByObjectClassProp( CatalogNo ),
                    //    NBTSubFieldPropColName = "field1"
                    //} );
                }

                #endregion

            }//_initMappings()

            /// <summary>
            /// Class that contains the variables necessary for the mapping.
            /// </summary>
            private class C3Mapping
            {
                public Int32 NBTNodeTypeId = Int32.MinValue;
                public Int32 NBTNodeTypePropId = Int32.MinValue;
                public string C3ProductPropertyValue = string.Empty;
                public string NBTSubFieldPropColName = string.Empty;
                public string NBTSubFieldPropColName2 = string.Empty;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="unitOfMeasurementName"></param>
            /// <returns></returns>
            private CswNbtObjClassUnitOfMeasure _getUnitOfMeasure( string unitOfMeasurementName )
            {
                CswNbtObjClassUnitOfMeasure UnitOfMeasureNode = null;

                if( false == string.IsNullOrEmpty( unitOfMeasurementName ) )
                {

                    CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
                    CswNbtMetaDataObjectClassProp NameOCP = UnitOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );

                    CswNbtView UnitsView = new CswNbtView( _CswNbtResources );
                    CswNbtViewRelationship Parent = UnitsView.AddViewRelationship( UnitOfMeasureOC, false );

                    UnitsView.AddViewPropertyAndFilter( Parent,
                                                        MetaDataProp: NameOCP,
                                                        Value: unitOfMeasurementName,
                                                        FilterMode: CswEnumNbtFilterMode.Equals );


                    ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( UnitsView, false, false, true );
                    int Count = Tree.getChildNodeCount();

                    for( int i = 0; i < Count; i++ )
                    {
                        Tree.goToNthChild( i );
                        UnitOfMeasureNode = Tree.getNodeForCurrentPosition();
                        Tree.goToParentNode();
                    }

                }

                return UnitOfMeasureNode;

            }//_getUnitOfMeasure

            #endregion Private helper methods

            public CswPrimaryKey createMaterialDocument( CswNbtObjClassChemical MaterialNode )
            {
                CswPrimaryKey NewSDSDocumentNodeId = null;

                string MsdsUrl = _ProductToImport.MsdsUrl;
                if( false == string.IsNullOrEmpty( MsdsUrl ) )
                {
                    CswNbtMetaDataNodeType SDSDocumentNT = _CswNbtResources.MetaData.getNodeType( "SDS Document" );
                    if( null != SDSDocumentNT )
                    {
                        CswNbtObjClassDocument NewSDSDocumentNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( SDSDocumentNT.NodeTypeId, CswEnumNbtMakeNodeOperation.MakeTemp );
                        NewSDSDocumentNode.Title.Text = "SDS: " + MaterialNode.TradeName.Text;
                        NewSDSDocumentNode.FileType.Value = CswNbtObjClassDocument.FileTypes.Link;
                        NewSDSDocumentNode.Link.Href = MsdsUrl;
                        NewSDSDocumentNode.Link.Text = MsdsUrl;
                        NewSDSDocumentNode.Owner.RelatedNodeId = MaterialNode.NodeId;
                        NewSDSDocumentNode.IsTemp = false;
                        NewSDSDocumentNode.postChanges( true );

                        // Set the return object
                        NewSDSDocumentNodeId = NewSDSDocumentNode.NodeId;
                    }
                }

                return NewSDSDocumentNodeId;
            }

            /// <summary>
            /// Creates a new Vendor node if the vendor doesn't already exist otherwise uses the pre-existing Vendor node.
            /// </summary>
            /// <param name="VendorName"></param>
            /// <returns>A C3CreateMaterialResponse.State.Supplier object with the Vendor name and Vendor nodeid set.</returns>
            public C3CreateMaterialResponse.State.Supplier createVendorNode( string VendorName )
            {
                C3CreateMaterialResponse.State.Supplier Supplier = new C3CreateMaterialResponse.State.Supplier();

                CswNbtView VendorView = new CswNbtView( _CswNbtResources );
                VendorView.ViewName = "VendorWithNameEquals";

                CswNbtMetaDataObjectClass VendorOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
                CswNbtViewRelationship Parent = VendorView.AddViewRelationship( VendorOC, true );

                CswNbtMetaDataObjectClassProp VendorOCP = VendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorName );

                CswNbtViewProperty ViewProperty1 = VendorView.AddViewProperty( Parent, VendorOCP );

                CswNbtViewPropertyFilter Filter1 = VendorView.AddViewPropertyFilter( ViewProperty1,
                                                          CswEnumNbtFilterConjunction.And,
                                                          CswEnumNbtFilterResultMode.Hide,
                                                          CswEnumNbtSubFieldName.Text,
                                                          CswEnumNbtFilterMode.Equals,
                                                          VendorName,
                                                          false,
                                                          false );

                ICswNbtTree VendorsTree = _CswNbtResources.Trees.getTreeFromView( VendorView, false, true, true );
                if( VendorsTree.getChildNodeCount() > 0 )
                {
                    VendorsTree.goToNthChild( 0 );

                    // Add to the return object
                    Supplier.name = VendorsTree.getNodeNameForCurrentPosition();
                    Supplier.val = VendorsTree.getNodeIdForCurrentPosition().ToString();
                }
                else
                {
                    // Don't create a new node just return an empty value in the return object - Case 28687
                    Supplier.name = VendorName;
                    Supplier.val = string.Empty;
                }

                return Supplier;
            }//createVendorNode()

            public Collection<Collection<C3CreateMaterialResponse.State.SizeColumnValue>> createSizeNodes( CswNbtObjClassChemical ChemicalNode )
            {
                // Return object
                Collection<Collection<C3CreateMaterialResponse.State.SizeColumnValue>> ProductSizes = new Collection<Collection<C3CreateMaterialResponse.State.SizeColumnValue>>();

                CswNbtMetaDataNodeType SizeNT = _CswNbtResources.MetaData.getNodeType( "Size" );
                if( null != SizeNT )
                {
                    for( int index = 0; index < _ProductToImport.ProductSize.Length; index++ )
                    {
                        CswC3Product.Size CurrentSize = _ProductToImport.ProductSize[index];
                        CswNbtObjClassSize sizeNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNT.NodeTypeId, CswEnumNbtMakeNodeOperation.MakeTemp );
                        // Don't forget to send in the index so that the correct values get added to the NTPs
                        addNodeTypeProps( sizeNode.Node, index );
                        sizeNode.Material.RelatedNodeId = ChemicalNode.NodeId;

                        bool duplicateFound = false;
                        foreach( CswNbtObjClassSize existingSizeNode in SizeNT.getNodes( false, false, false, true ) )
                        {
                            if( existingSizeNode.Material.RelatedNodeId == sizeNode.Material.RelatedNodeId &&
                                existingSizeNode.CatalogNo.Text.Equals( sizeNode.CatalogNo.Text ) &&
                                existingSizeNode.InitialQuantity.Quantity.Equals( sizeNode.InitialQuantity.Quantity ) &&
                                existingSizeNode.InitialQuantity.CachedUnitName.Equals( sizeNode.InitialQuantity.CachedUnitName ) )
                            {
                                duplicateFound = true;
                            }
                        }
                        if( false == duplicateFound )
                        {
                            sizeNode.IsTemp = false;
                            sizeNode.postChanges( true );

                            //Set the return object
                            Collection<C3CreateMaterialResponse.State.SizeColumnValue> Size = new Collection<C3CreateMaterialResponse.State.SizeColumnValue>();

                            C3CreateMaterialResponse.State.SizeColumnValue UnitCount = new C3CreateMaterialResponse.State.SizeColumnValue();
                            UnitCount.value = CswConvert.ToString( sizeNode.UnitCount.Value );
                            UnitCount.hidden = false;
                            Size.Add( UnitCount );

                            C3CreateMaterialResponse.State.SizeColumnValue InitialQuantity = new C3CreateMaterialResponse.State.SizeColumnValue();
                            InitialQuantity.value = sizeNode.InitialQuantity.Gestalt;
                            InitialQuantity.hidden = false;
                            Size.Add( InitialQuantity );

                            C3CreateMaterialResponse.State.SizeColumnValue CatalogNo = new C3CreateMaterialResponse.State.SizeColumnValue();
                            CatalogNo.value = sizeNode.CatalogNo.Text;
                            CatalogNo.hidden = false;
                            Size.Add( CatalogNo );

                            C3CreateMaterialResponse.State.SizeColumnValue NodeId = new C3CreateMaterialResponse.State.SizeColumnValue();
                            NodeId.value = sizeNode.NodeId.ToString();
                            NodeId.hidden = true;
                            Size.Add( NodeId );

                            ProductSizes.Add( Size );
                        }
                    }
                } // if( null != SizeNT )

                return ProductSizes;
            }//createSizeNodes()

            public void createMaterialSynonyms( CswNbtObjClassChemical ChemicalNode )
            {
                CswNbtMetaDataNodeType MaterialSynonymNT = _CswNbtResources.MetaData.getNodeType( "Material Synonym" );
                if( null != MaterialSynonymNT )
                {
                    for( int index = 0; index < _ProductToImport.Synonyms.Length; index++ )
                    {
                        CswNbtObjClassMaterialSynonym MaterialSynonymOC = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( MaterialSynonymNT.NodeTypeId, CswEnumNbtMakeNodeOperation.MakeTemp );
                        MaterialSynonymOC.Name.Text = _ProductToImport.Synonyms[index];
                        MaterialSynonymOC.Material.RelatedNodeId = ChemicalNode.NodeId;
                        MaterialSynonymOC.IsTemp = false;
                        MaterialSynonymOC.postChanges( true );
                    }
                }
            }

            /// <summary>
            /// Add values to the NodeType Properties of a Node.
            /// </summary>
            /// <param name="Node">The Node whose properties are being filled in.</param>
            /// <param name="CurrentIndex">The current index in the ProductSize array in a CswC3Product object. This is ONLY needed for Size Nodes.</param>
            public void addNodeTypeProps( CswNbtNode Node, int CurrentIndex = 0 )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Node.NodeTypeId );
                foreach( CswNbtMetaDataNodeTypeProp NTP in NodeType.getNodeTypeProps() )
                {
                    if( null != Node.Properties[NTP] && _Mappings.ContainsKey( NTP.PropName ) )
                    {
                        C3Mapping C3Mapping = _Mappings[NTP.PropName];
                        switch( Node.Properties[NTP].getFieldTypeValue() )
                        {
                            case CswEnumNbtFieldType.Quantity:
                                CswNbtObjClassUnitOfMeasure unitOfMeasure = _getUnitOfMeasure( _ProductToImport.ProductSize[CurrentIndex].pkg_qty_uom );
                                if( null != unitOfMeasure )
                                {
                                    Node.Properties[NTP].SetPropRowValue( (CswEnumNbtPropColumn) C3Mapping.NBTSubFieldPropColName, _ProductToImport.ProductSize[CurrentIndex].pkg_qty );
                                    Node.Properties[NTP].SetPropRowValue( (CswEnumNbtPropColumn) C3Mapping.NBTSubFieldPropColName2, unitOfMeasure.Name.Text );
                                    Node.Properties[NTP].SetPropRowValue( CswEnumNbtPropColumn.Field1_FK, unitOfMeasure.NodeId.PrimaryKey );
                                    string sizeGestalt = _ProductToImport.ProductSize[CurrentIndex].pkg_qty + " " + unitOfMeasure.Name.Text;
                                    Node.Properties[NTP].SetPropRowValue( CswEnumNbtPropColumn.Gestalt, sizeGestalt );

                                    // Note: This is a hackadoodle for now since importer is getting changed... soon...
                                    // Assumption: We are working with a node that is of NodeType Size
                                    if( NodeType.NodeTypeName == "Size" )
                                    {
                                        // Set the Unit Count
                                        CswNbtMetaDataNodeTypeProp UnitCountNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.UnitCount );
                                        Node.Properties[UnitCountNTP].SetPropRowValue( (CswEnumNbtPropColumn) C3Mapping.NBTSubFieldPropColName, _ProductToImport.ProductSize[CurrentIndex].case_qty );
                                        Node.Properties[UnitCountNTP].SetPropRowValue( CswEnumNbtPropColumn.Gestalt, _ProductToImport.ProductSize[CurrentIndex].case_qty );

                                        // Set the Catalog No
                                        // This needs to be here because each size has a unique catalogno
                                        CswNbtMetaDataNodeTypeProp CatalogNoNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.CatalogNo );
                                        Node.Properties[CatalogNoNTP].SetPropRowValue( (CswEnumNbtPropColumn) C3Mapping.NBTSubFieldPropColName2, _ProductToImport.ProductSize[CurrentIndex].catalog_no );
                                        Node.Properties[CatalogNoNTP].SetPropRowValue( CswEnumNbtPropColumn.Gestalt, _ProductToImport.ProductSize[CurrentIndex].catalog_no );
                                    }
                                }
                                break;
                            case CswEnumNbtFieldType.MOL:
                                if( false == string.IsNullOrEmpty( C3Mapping.C3ProductPropertyValue ) )
                                {
                                    string propAttr = new CswPropIdAttr( Node, NTP ).ToString();
                                    string molData = C3Mapping.C3ProductPropertyValue;

                                    string Href;
                                    CswNbtSdBlobData SdBlobData = new CswNbtSdBlobData( _CswNbtResources );
                                    SdBlobData.saveMol( molData, propAttr, out Href );
                                }
                                break;
                            default:
                                Node.Properties[NTP].SetPropRowValue( (CswEnumNbtPropColumn) C3Mapping.NBTSubFieldPropColName, C3Mapping.C3ProductPropertyValue );
                                Node.Properties[NTP].SetPropRowValue( CswEnumNbtPropColumn.Gestalt, C3Mapping.C3ProductPropertyValue );
                                break;
                        }
                    }//if( null != Node.Properties[NTP] && _Mappings.ContainsKey( NTP.PropName ) )

                }//foreach( CswNbtMetaDataNodeTypeProp NTP in NodeType.getNodeTypeProps() )

            }//addNodeTypeProps()

        }//class ImportManager

        #endregion Import Mananger

    }

}