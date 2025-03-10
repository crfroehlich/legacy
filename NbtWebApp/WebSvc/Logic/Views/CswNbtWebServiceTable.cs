using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceTable
    {
        private const Int32 _MaxLength = 35;
        private const Int32 _NodePerNodeTypeLimit = 3;

        private Int32 _FilterToNodeTypeId;
        private readonly CswNbtResources _CswNbtResources;
        private CswNbtView _View;
        private readonly CswNbtStatisticsEvents _CswNbtStatisticsEvents;
        private readonly CswNbtSearchPropOrder _CswNbtSearchPropOrder;

        public CswNbtWebServiceTable( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents, Int32 NodeTypeId )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
            _CswNbtResources.EditMode = CswEnumNbtNodeEditMode.Table;
            _CswNbtSearchPropOrder = new CswNbtSearchPropOrder( _CswNbtResources );
            _FilterToNodeTypeId = NodeTypeId;
        }

        /// <summary>
        /// Get table from a universal search.
        /// </summary>
        /// <param name="View"></param>
        /// <returns></returns>
        public JObject getTable( CswNbtView View )
        {
            _View = View;
            JObject ret = new JObject();

            if( _View != null )
            {
                // Find current max order set in view
                Int32 maxOrder = ( from ViewRel in _View.Root.ChildRelationships
                                   from ViewProp in ViewRel.Properties
                                   select ViewProp.Order ).Concat( new[] { 0 } ).Max();  // thanks Resharper!

                // Set default order for properties in the view without one
                foreach( CswNbtViewProperty ViewProp in from ViewRel in _View.Root.ChildRelationships
                                                        from ViewProp in ViewRel.Properties
                                                        where Int32.MinValue == ViewProp.Order
                                                        select ViewProp )
                {
                    ViewProp.Order = maxOrder + 1;
                    maxOrder++;
                }

                // Add 'default' Table layout elements for the nodetype to the view for efficiency
                // Set the order to be after properties in the view
                foreach( CswNbtViewRelationship ViewRel in _View.Root.ChildRelationships )
                {
                    if( ViewRel.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                    {
                        IEnumerable<CswNbtMetaDataNodeTypeProp> Props = _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( ViewRel.SecondId, Int32.MinValue, CswEnumNbtLayoutType.Table );
                        foreach( CswNbtMetaDataNodeTypeProp NTProp in Props )
                        {
                            bool AlreadyExists = false;
                            foreach( CswNbtViewProperty ViewProp in ViewRel.Properties )
                            {
                                if( ViewProp.NodeTypePropId == NTProp.PropId )
                                {
                                    AlreadyExists = true;
                                }
                            }

                            if( false == AlreadyExists )
                            {
                                CswNbtViewProperty NewViewProp = _View.AddViewProperty( ViewRel, NTProp );
                                CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout propTableLayout = NTProp.getTableLayout();
                                NewViewProp.Order = maxOrder + propTableLayout.DisplayRow;
                            }
                        } // foreach( CswNbtMetaDataNodeTypeProp NTProp in Props )
                    } // if( ViewRel.SecondType == RelatedIdType.NodeTypeId )
                } // foreach( CswNbtViewRelationship ViewRel in View.Root.ChildRelationships )

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, true, false, false );
                ret = makeTableFromTree( Tree, null );
            } // if( _View != null )
            return ret;
        } // getTable()

        /// <summary>
        /// Get table from a CswRetObjSearchResult (the return object from C3 searches).
        /// </summary>
        /// <param name="C3SearchResultsObj"></param>
        /// <returns></returns>
        public JObject getTable( CswRetObjSearchResults C3SearchResultsObj, string SearchField, string DataService, bool filtered = false )
        {

            JObject ret = new JObject();

            Collection<string> PropsToHide = new Collection<string>();
            foreach( string Property in C3SearchResultsObj.AllC3ProductProperties )
            {
                PropsToHide.Add( Property );
            }

            PropsToHide.Remove( SearchField );
            if( "ACD" == DataService )
            {
                if( filtered )
                {
                    PropsToHide.Add( "CatalogNo" ); //Filtered ACD searches should NEVER show "CatalogNo"

                    PropsToHide.Remove( "CatalogName" );
                    PropsToHide.Remove( "CatalogNumbers" );
                    PropsToHide.Remove( "SupplierName" );
                }
                PropsToHide.Remove( "Formula" );
            }
            else
            {
                PropsToHide.Remove( "SourceName" );
                PropsToHide.Remove( "SupplierName" );
                PropsToHide.Remove( "CatalogNo" );
            }

            ret = makeTableFromWebServiceObj( C3SearchResultsObj, PropsToHide, DataService );

            return ret;
        }

        public JObject makeTableFromTree( ICswNbtTree Tree, Collection<Int32> PropsToHide, Int32 Page = 0, Int32 PageLimit = 0 )
        {

            JObject ret = new JObject();
            if( Tree != null )
            {
                _populateDictionary( Tree, PropsToHide, Page, PageLimit );

                ret["results"] = Tree.getChildNodeCount().ToString();
                ret["nodetypecount"] = _TableDict.Keys.Count;
                ret["truncated"] = Tree.getCurrentNodeChildrenTruncated();
                ret["pagesize"] = _CswNbtResources.CurrentNbtUser.PageSize;
                ret["nodetypes"] = _dictionaryToJson();
                ret["searchtarget"] = "universal";
            }
            return ret;
        } // makeTableFromTree()

        private string _getThumbnailUrl( string defaultIconFileName )
        {
            string ret = "";
            // default image, overridden below
            if( defaultIconFileName != string.Empty )
            {
                ret = CswNbtMetaDataObjectClass.IconPrefix100 + defaultIconFileName;
            }
            else
            {
                ret = "Images/icons/300/_placeholder.gif";
            }
            return ret;
        }

        /// <summary>
        /// Make a table from a C3 search result object.
        /// </summary>
        /// <param name="C3SearchResultsObj"></param>
        /// <param name="PropsToHide"></param>
        /// <returns></returns>
        public JObject makeTableFromWebServiceObj( CswRetObjSearchResults C3SearchResultsObj, Collection<string> PropsToHide, string DataService )
        {
            JObject ret = new JObject();

            if( C3SearchResultsObj != null )
            {
                Int32 results = _populateDictionary( C3SearchResultsObj, PropsToHide, DataService );

                ret["results"] = results;
                ret["nodetypecount"] = _TableDict.Keys.Count;
                ret["truncated"] = null;
                ret["pagesize"] = _CswNbtResources.CurrentNbtUser.PageSize;
                ret["nodetypes"] = _dictionaryToJson();
                ret["importmenu"] = CswNbtWebServiceC3Search.getImportBtnItems( _CswNbtResources );
                ret["searchtarget"] = "chemcatcentral";
            }
            return ret;
        }

        private class TableNode
        {
            public CswPrimaryKey NodeId;
            public CswNbtNodeKey NodeKey;
            public CswNbtMetaDataNodeType NodeType;
            public Int32 C3ProductId;
            public Int32 ACDCdbregno;
            public string NodeName;
            public bool Locked;
            public bool Disabled;
            public bool IsFavorite;
            public string ThumbnailUrl;
            public string ThumbnailBase64Str;

            public bool AllowView;
            public bool AllowEdit;
            public bool AllowDelete;

            public bool AllowImport;
            public bool AllowRequest;

            public SortedList<Int32, TableProp> Props = new SortedList<Int32, TableProp>();

            public JObject ToJson()
            {
                JObject NodeObj = new JObject();
                NodeObj["nodename"] = NodeName;
                if( null == NodeId )
                {
                    NodeObj["nodeid"] = "";
                }
                else
                {
                    NodeObj["nodeid"] = NodeId.ToString();
                }
                if( null == NodeKey )
                {
                    NodeObj["nodekey"] = "";
                }
                else
                {
                    NodeObj["nodekey"] = NodeKey.ToString();
                }
                NodeObj["nodelink"] = CswNbtNode.getNodeLink( NodeId, NodeName );
                NodeObj["c3productid"] = C3ProductId.ToString();
                NodeObj["acdcdbregno"] = ACDCdbregno.ToString();
                NodeObj["locked"] = Locked.ToString().ToLower();
                NodeObj["disabled"] = Disabled.ToString().ToLower();
                NodeObj["isFavorite"] = IsFavorite.ToString().ToLower();
                NodeObj["nodetypeid"] = NodeType.NodeTypeId;
                NodeObj["nodetypename"] = NodeType.NodeTypeName;
                NodeObj["thumbnailurl"] = ThumbnailUrl;
                NodeObj["thumbnailbase64str"] = ThumbnailBase64Str;
                NodeObj["allowview"] = AllowView;
                NodeObj["allowedit"] = AllowEdit;
                NodeObj["allowdelete"] = AllowDelete;

                NodeObj["allowimport"] = AllowImport;
                NodeObj["allowrequest"] = AllowRequest;

                // Props in the View
                JArray PropsArray = new JArray();
                NodeObj["props"] = PropsArray;
                foreach( TableProp thisProp in Props.Values )
                {
                    PropsArray.Add( thisProp.ToJson() );
                }
                return NodeObj;
            } // ToJson()
        } // class TableNode

        private class TableProp
        {
            public CswPropIdAttr PropId;
            public Int32 NodeTypePropId;
            public Int32 ObjectClassPropId;
            public string FieldType;
            public string PropName;
            public string Gestalt;
            public Int32 JctNodePropId;
            public JObject PropData;
            public CswEnumNbtSearchPropOrderSourceType Source;

            public JObject ToJson()
            {
                JObject ThisProp = new JObject();
                if( null != PropId )
                {
                    ThisProp["propid"] = PropId.ToString();
                }

                ThisProp["propname"] = PropName;
                ThisProp["gestalt"] = Gestalt;
                ThisProp["fieldtype"] = FieldType;
                ThisProp["propData"] = PropData;

                if( null != Source )
                {
                    ThisProp["source"] = Source.ToString();
                }

                return ThisProp;
            } // ToJson()
        } // class TableProp

        private Dictionary<CswNbtMetaDataNodeType, Collection<TableNode>> _TableDict = new Dictionary<CswNbtMetaDataNodeType, Collection<TableNode>>();

        private Int32 _populateDictionary( ICswNbtTree Tree, Collection<Int32> PropsToHide, Int32 Page = 0, Int32 PageLimit = 0 )
        {
            Int32 results = 0;
            for( Int32 c = Math.Max( 0, ( Page - 1 ) * PageLimit ); ( c < Tree.getChildNodeCount() && ( PageLimit < 1 || results < PageLimit ) ); c++ )
            {
                Tree.goToNthChild( c );

                TableNode thisNode = new TableNode();

                thisNode.NodeKey = Tree.getNodeKeyForCurrentPosition();

                // Note on FilterToNodeTypeId: 
                // It would be better to filter inside the view, 
                // but it's also much more work, and I'm not even sure this feature will be used.

                if( null != thisNode.NodeKey &&
                    ( Int32.MinValue == _FilterToNodeTypeId || _FilterToNodeTypeId == thisNode.NodeKey.NodeTypeId ) )
                {
                    thisNode.NodeType = _CswNbtResources.MetaData.getNodeType( thisNode.NodeKey.NodeTypeId );
                    if( null != thisNode.NodeType )
                    {
                        thisNode.NodeId = Tree.getNodeIdForCurrentPosition();
                        thisNode.NodeName = Tree.getNodeNameForCurrentPosition();
                        thisNode.Locked = Tree.getNodeLockedForCurrentPosition();
                        thisNode.Disabled = ( false == Tree.getNodeIncludedForCurrentPosition() );
                        thisNode.IsFavorite = Tree.getNodeFavoritedForCurrentPosition();

                        thisNode.ThumbnailUrl = _getThumbnailUrl( Tree.getNodeIconForCurrentPosition() );

                        thisNode.AllowView = _CswNbtResources.Permit.canAnyTab( Security.CswEnumNbtNodeTypePermission.View, thisNode.NodeType );
                        thisNode.AllowEdit = _CswNbtResources.Permit.canAnyTab( Security.CswEnumNbtNodeTypePermission.Edit, thisNode.NodeType );
                        thisNode.AllowDelete = _CswNbtResources.Permit.canNodeType( Security.CswEnumNbtNodeTypePermission.Delete, thisNode.NodeType );

                        // Properties
                        SortedSet<CswNbtSearchPropOrder.SearchOrder> orderDict = _CswNbtSearchPropOrder.getPropOrderDict( thisNode.NodeKey, _View );

                        foreach( CswNbtTreeNodeProp PropElm in Tree.getChildNodePropsOfNode() )
                        {
                            TableProp thisProp = new TableProp();
                            if( false == PropElm.Hidden )
                            {
                                thisProp.NodeTypePropId = PropElm.NodeTypePropId;
                                thisProp.ObjectClassPropId = PropElm.ObjectClassPropId;
                                if( PropsToHide == null || false == PropsToHide.Contains( thisProp.NodeTypePropId ) )
                                {
                                    thisProp.PropId = new CswPropIdAttr( thisNode.NodeId, thisProp.NodeTypePropId );
                                    thisProp.FieldType = PropElm.FieldType;
                                    thisProp.PropName = PropElm.PropName;
                                    thisProp.Gestalt = _Truncate( PropElm.Gestalt );
                                    thisProp.JctNodePropId = PropElm.JctNodePropId;

                                    // Special case: Image becomes thumbnail
                                    if( thisProp.FieldType == CswEnumNbtFieldType.Image )
                                    {
                                        thisNode.ThumbnailUrl = CswNbtNodePropImage.getLink( thisProp.JctNodePropId, thisNode.NodeId );
                                    }

                                    if( thisProp.FieldType == CswEnumNbtFieldType.MOL )
                                    {
                                        thisNode.ThumbnailUrl = CswNbtNodePropMol.getLink( thisProp.JctNodePropId, thisNode.NodeId );
                                    }
                                    else
                                    {
                                        CswNbtSearchPropOrder.SearchOrder thisOrder = orderDict.FirstOrDefault( Order => Order.NodeTypePropId == thisProp.NodeTypePropId ||
                                                                                                                         ( thisProp.ObjectClassPropId != Int32.MinValue &&
                                                                                                                           Order.ObjectClassPropId == thisProp.ObjectClassPropId ) );
                                        if( null != thisOrder )
                                        {
                                            thisProp.Source = thisOrder.Source;

                                            if( thisProp.FieldType == CswEnumNbtFieldType.Button )
                                            {
                                                // Include full info for rendering the button
                                                // This was done in such a way as to prevent instancing the CswNbtNode object, 
                                                // which we don't need for Buttons.
                                                CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( thisProp.NodeTypePropId );

                                                CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                                                Int32 TabId = Int32.MaxValue;
                                                if( null != NodeTypeProp.FirstEditLayout )
                                                {
                                                    TabId = NodeTypeProp.FirstEditLayout.TabId;
                                                }
                                                JProperty JpPropData = ws.makePropJson( thisNode.NodeId, TabId, NodeTypeProp, null, Int32.MinValue, Int32.MinValue, string.Empty, thisNode.Locked );
                                                thisProp.PropData = (JObject) JpPropData.Value;

                                                JObject PropValues = new JObject();
                                                CswNbtNodePropButton.AsJSON( NodeTypeProp, PropValues, PropElm.Field2, PropElm.Field1 );
                                                thisProp.PropData["values"] = PropValues;
                                            }
                                            if( false == thisNode.Props.ContainsKey( thisOrder.Order ) && false == thisNode.Props.ContainsValue( thisProp ) )
                                            {
                                                thisNode.Props.Add( thisOrder.Order, thisProp );
                                            }
                                            else
                                            {
                                                throw new CswDniException( CswEnumErrorType.Error, "A search result with the same value and position already exists in the result set.",
                                                   "{" + thisNode.NodeType.NodeTypeName + "} entity {" + thisNode.NodeName + "}, Id: {" + thisNode.NodeId + ", has a duplicate {" + thisProp.FieldType + "} property record for {" + thisProp.PropName + "} PropId: {" + thisProp.NodeTypePropId + "}" );
                                            }
                                        }
                                    }
                                } // if( false == PropsToHide.Contains( NodeTypePropId ) )
                            } //if (false == CswConvert.ToBoolean(PropElm["hidden"]))
                        } // foreach( XElement PropElm in NodeElm.Elements() )

                        if( false == _TableDict.ContainsKey( thisNode.NodeType ) )
                        {
                            _TableDict.Add( thisNode.NodeType, new Collection<TableNode>() );
                        }
                        _TableDict[thisNode.NodeType].Add( thisNode );
                        results++;

                    } // if( thisNode.NodeType != null )
                } // if(null != thisNode.NodeKey && ( Int32.MinValue == _FilterToNodeTypeId || _FilterToNodeTypeId == thisNode.NodeKey.NodeTypeId ) )
                Tree.goToParentNode();
            } // for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            return results;
        } // _populateDictionary()

        /// <summary>
        /// FOR DISPLAYING CHEMCAT RESULTS
        /// </summary>
        /// <param name="C3SearchResultsObj"></param>
        /// <param name="PropsToHide"></param>
        /// <returns></returns>
        private Int32 _populateDictionary( CswRetObjSearchResults C3SearchResultsObj, Collection<string> PropsToHide, string DataService )
        {
            Int32 results = 0;

            for( int i = 0; i < C3SearchResultsObj.CswC3SearchResults.Count(); i++ )//todo: if results are null
            {
                TableNode thisNode = new TableNode();

                //Note: For now, we are hardcoding the nodetype as "Chemical" for each results from ChemCatCentral.
                thisNode.NodeType = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass ).FirstNodeType;
                if( null != thisNode.NodeType )
                {
                    // default image, overridden below
                    if( thisNode.NodeType.IconFileName != string.Empty )
                    {
                        thisNode.ThumbnailUrl = CswNbtMetaDataObjectClass.IconPrefix100 + thisNode.NodeType.IconFileName;
                    }
                    else
                    {
                        thisNode.ThumbnailUrl = "Images/icons/300/_placeholder.gif";
                    }

                    thisNode.AllowView = _CswNbtResources.Permit.canAnyTab( Security.CswEnumNbtNodeTypePermission.View, thisNode.NodeType );
                    thisNode.AllowEdit = _CswNbtResources.Permit.canAnyTab( Security.CswEnumNbtNodeTypePermission.Edit, thisNode.NodeType );

                    //C3 results are not nodes and hence they can't be deleted.
                    thisNode.AllowDelete = false;
                    //C3 results CAN however be imported into Nbt IF the user has Create Material Permissions
                    thisNode.AllowImport = _CswNbtResources.Permit.can( CswEnumNbtActionName.Create_Material, _CswNbtResources.CurrentNbtUser );
                    thisNode.AllowRequest = _CswNbtResources.Permit.can( CswEnumNbtActionName.Submit_Request, _CswNbtResources.CurrentNbtUser );

                    // Properties
                    int propIndex = 0;
                    CswC3Product[] products = C3SearchResultsObj.CswC3SearchResults;

                    CswC3Product product = products[i];
                    JObject productObject = JObject.FromObject( product );
                    IEnumerable properties = productObject.Properties();
                    foreach( JProperty prop in properties )
                    {
                        string name = prop.Name;
                        string value = prop.Value.ToString();

                        if( prop.Name == "TradeName" )
                        {
                            thisNode.NodeName = prop.Value.ToString();
                        }

                        if( prop.Name == "ProductId" )
                        {
                            thisNode.C3ProductId = CswConvert.ToInt32( prop.Value );
                        }

                        if( prop.Name == "Cdbregno" )
                        {
                            thisNode.ACDCdbregno = CswConvert.ToInt32( prop.Value );
                        }

                        TableProp thisProp = new TableProp();
                        if( PropsToHide == null || false == PropsToHide.Contains( name ) )
                        {
                            thisProp.PropName = name;
                            thisProp.Gestalt = value;
                            thisNode.Props.Add( propIndex, thisProp );
                        }

                        propIndex++;

                    }

                    // Thumbnail image -- set to molimage if we have one
                    if( DataService.Equals( "C3" ) && false == String.IsNullOrEmpty( product.MolImage ) )
                    {
                        thisNode.ThumbnailBase64Str = "data:image/jpeg;base64," + product.MolImage;
                    }
                    else if( DataService.Equals( "ACD" ) )
                    {
                        thisNode.ThumbnailUrl = "Services/BlobData/getExternalImage?cdbregno=" + thisNode.ACDCdbregno + "&productid=" + product.ProductId + "&uid=" + CswRandom.RandomString();
                    }

                    if( false == _TableDict.ContainsKey( thisNode.NodeType ) )
                    {
                        _TableDict.Add( thisNode.NodeType, new Collection<TableNode>() );
                    }
                    _TableDict[thisNode.NodeType].Add( thisNode );

                    results++;

                }//if (null != thisNode.NodeType)

            }//for( int i = 0; i < C3SearchResultsObj.CswC3SearchResults.Count(); i++ )

            return results;
        } // _populateDictionary()

        public JArray _dictionaryToJson()
        {
            JArray ret = new JArray();
            foreach( CswNbtMetaDataNodeType NodeType in _TableDict.Keys.OrderBy( NodeType => NodeType.NodeTypeName ) )
            {
                JObject NodeTypeObj = new JObject();
                ret.Add( NodeTypeObj );

                NodeTypeObj["nodetypeid"] = NodeType.NodeTypeId;
                NodeTypeObj["nodetypename"] = NodeType.NodeTypeName;
                NodeTypeObj["results"] = _TableDict[NodeType].Count;

                JArray NodesArray = new JArray();
                NodeTypeObj["nodes"] = NodesArray;
                foreach( TableNode thisNode in _TableDict[NodeType] )
                {
                    // Limit nodes per nodetype, if there is more than one nodetype
                    if( _TableDict.Keys.Count <= 1 || NodesArray.Count < _NodePerNodeTypeLimit )
                    {
                        NodesArray.Add( thisNode.ToJson() );
                    }
                }
            }
            return ret;
        } // _dictionaryToJson()

        private string _Truncate( string InStr )
        {
            string OutStr = InStr;
            if( OutStr.Length > _MaxLength )
            {
                OutStr = OutStr.Substring( 0, _MaxLength ) + "...";
            }
            return OutStr;
        } // _Truncate()

    } // class CswNbtWebServiceTable
} // namespace ChemSW.Nbt.WebServices