using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Search
{
    /// <summary>
    /// Represents a Universal Search
    /// </summary>
    [DataContract]
    public class CswNbtSearch : IEquatable<CswNbtSearch>
    {
        /// <summary>
        /// CswNbtResources reference
        /// </summary>
        protected CswNbtResources _CswNbtResources;

        private CswNbtSearchPropOrder _CswNbtSearchPropOrder;

        /// <summary>
        /// Constructor - new search
        /// </summary>
        public CswNbtSearch( CswNbtResources CswNbtResources, string inSearchTerm )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSearchPropOrder = new CswNbtSearchPropOrder( _CswNbtResources );
            SearchTerm = inSearchTerm;
        }

        /// <summary>
        /// Constructor - from session data
        /// </summary>
        public CswNbtSearch( CswNbtResources CswNbtResources, DataRow SessionDataRow )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSearchPropOrder = new CswNbtSearchPropOrder( _CswNbtResources );
            FromSessionData( SessionDataRow );
        }

        #region Search Data

        [DataMember]
        public string SearchTerm;
        [DataMember]
        public Collection<CswNbtSearchFilter> FiltersApplied = new Collection<CswNbtSearchFilter>();

        /// <summary>
        /// A display name for the search
        /// </summary>
        public string Name
        {
            get
            {
                return "Search for: " + SearchTerm;
            }
        }

        #endregion Search Data
        
        #region Serialization

        public void FromJObject( JObject SearchObj )
        {
            SearchTerm = SearchObj["searchterm"].ToString();
            SessionDataId = new CswNbtSessionDataId( SearchObj["sessiondataid"].ToString() );
            JArray FiltersArr = (JArray) SearchObj["filtersapplied"];
            foreach(JObject FilterObj in FiltersArr)
            {
                addFilter( FilterObj );
            }
        } // FromJObject()

        public JObject ToJObject()
        {
            JObject SearchObj = new JObject();
            SearchObj["searchterm"] = SearchTerm;
            SearchObj["sessiondataid"] = SessionDataId.ToString();
            
            JArray FiltersArr =  new JArray();
            foreach(CswNbtSearchFilter Filter in FiltersApplied)
            {
                FiltersArr.Add( Filter.ToJObject() );
            }
            SearchObj["filtersapplied"] = FiltersArr;

            return SearchObj;
        } // ToJObject()

        public void FromSessionData( DataRow SessionDataRow )
        {
            FromJObject( JObject.Parse( SessionDataRow["viewxml"].ToString() ) );
            SessionDataId = new CswNbtSessionDataId( CswConvert.ToInt32( SessionDataRow["sessiondataid"] ) );
        }

        public override string ToString()
        {
            return ToJObject().ToString();
        }

        /// <summary>
        /// Save this View to Session's data cache
        /// </summary>
        public void SaveToCache( bool IncludeInQuickLaunch, bool ForceCache = false, bool KeepInQuickLaunch = false )
        {
            // don't cache twice
            if( SessionDataId == null || ForceCache || IncludeInQuickLaunch )  // case 23999
            {
                SessionDataId = _CswNbtResources.SessionDataMgr.saveSessionData( this, IncludeInQuickLaunch, KeepInQuickLaunch );
            }
        } // SaveToCache()

        public void clearSessionDataId()
        {
            SessionDataId = null;
        }

        /// <summary>
        /// Key for retrieving the view from the Session's data cache
        /// </summary>
        public CswNbtSessionDataId SessionDataId;

        #endregion JSON Serialization

        #region Search Functions

        public bool IsSingleNodeType()
        {
            return ( FiltersApplied.Any( Filter => Filter.Type == CswNbtSearchFilterType.nodetype ) );
        } // IsSingleNodeType()

        private Collection<Int32> _FilteredPropIds = null;
        public Collection<Int32> getFilteredPropIds()
        {
            if( _FilteredPropIds == null )
            {
                _FilteredPropIds = new Collection<Int32>();
                foreach( CswNbtSearchFilter Filter in FiltersApplied.Where( Filter => Filter.Type == CswNbtSearchFilterType.propval ) )
                {
                    _FilteredPropIds.Add( Filter.FirstPropVersionId );
                }
            }
            return _FilteredPropIds;
        } // getFilteredPropIds()

        public void addFilter( Int32 NodeTypeId, bool Removeable )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            if( null != NodeType )
            {
                addFilter( NodeType, Removeable );
            }
        } // addNodeTypeFilter()

        public void addFilter( CswNbtSearchFilter Filter )
        {
            FiltersApplied.Add( Filter );
            _FilteredPropIds = null;
        } // addFilter()

        public void addFilter( JObject FilterObj )
        {
            addFilter( new CswNbtSearchFilter( FilterObj ) );
        } // addFilter()

        public void addFilter( CswNbtMetaDataNodeType NodeType, bool Removeable )
        {
            addFilter( makeFilter( NodeType, Int32.MinValue, Removeable, CswNbtSearchPropOrder.PropOrderSourceType.Unknown ) );
        } // addFilter()

        public void addFilter( CswNbtMetaDataObjectClass ObjectClass, bool Removeable )
        {
            addFilter( makeFilter( ObjectClass, Int32.MinValue, Removeable, CswNbtSearchPropOrder.PropOrderSourceType.Unknown ) );
        } // addFilter()


        public void removeFilter( JObject FilterObj )
        {
            removeFilter( new CswNbtSearchFilter( FilterObj ) );
        } // removeFilter()

        public void removeFilter( CswNbtSearchFilter Filter )
        {
            if( Filter.Type == CswNbtSearchFilterType.nodetype )
            {
                // Clear all filters
                FiltersApplied.Clear();
            }
            else
            {
                Collection<CswNbtSearchFilter> FiltersToRemove = new Collection<CswNbtSearchFilter>();
                foreach( CswNbtSearchFilter MatchingFilterObj in FiltersApplied.Where( AppliedFilter => AppliedFilter == Filter ) )
                {
                    FiltersToRemove.Add( MatchingFilterObj );
                }
                foreach( CswNbtSearchFilter DoomedFilter in FiltersToRemove )
                {
                    FiltersApplied.Remove( DoomedFilter );
                }
            }
            _FilteredPropIds = null;
        } // removeFilter()

        public ICswNbtTree Results()
        {
            // Filters to apply
            string WhereClause = string.Empty;
            //bool SingleNodeType = false;
            //Collection<Int32> FilteredPropIds = new Collection<Int32>();
            foreach( CswNbtSearchFilter Filter in FiltersApplied )
            {
                if( Filter.Type == CswNbtSearchFilterType.nodetype )
                {
                    // NodeType filter
                    Int32 NodeTypeFirstVersionId = Filter.FirstVersionId;
                    if( NodeTypeFirstVersionId != Int32.MinValue )
                    {
                        WhereClause += " and t.nodetypeid in (select nodetypeid from nodetypes where firstversionid = " + NodeTypeFirstVersionId.ToString() + @") ";
                    }
                }
                else if( Filter.Type == CswNbtSearchFilterType.objectclass )
                {
                    // Object Class filter
                    Int32 ObjectClassId = Filter.ObjectClassId;
                    if( ObjectClassId != Int32.MinValue )
                    {
                        WhereClause += " and t.nodetypeid in (select nodetypeid from nodetypes where objectclassid = " + ObjectClassId.ToString() + @") ";
                    }
                }
                else if( Filter.Type == CswNbtSearchFilterType.propval )
                {
                    // Property Filter
                    // Someday we may need to do this in a view instead
                    Int32 NodeTypePropFirstVersionId = Filter.FirstPropVersionId;
                    string FilterStr = Filter.FilterValue;
                    if( FilterStr == CswNbtSearchFilter.BlankValue )
                    {
                        FilterStr = " is null";
                    }
                    else
                    {
                        FilterStr = CswTools.SafeSqlLikeClause( FilterStr, CswTools.SqlLikeMode.Begins, false );
                    }

                    if( NodeTypePropFirstVersionId != Int32.MinValue )
                    {
                        WhereClause += @" and n.nodeid in (select nodeid 
                                                             from jct_nodes_props 
                                                            where nodetypepropid in (select nodetypepropid 
                                                                                       from nodetype_props 
                                                                                      where firstpropversionid = (select firstpropversionid 
                                                                                                                    from nodetype_props 
                                                                                                                   where nodetypepropid = " + NodeTypePropFirstVersionId.ToString() + @" ))
                                                              and gestalt " + FilterStr + @") ";
                    }
                }
            } // foreach( CswNbtSearchFilter Filter in FiltersApplied )

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromSearch( SearchTerm, WhereClause, true, false, false );
            return Tree;
        }

        /// <summary>
        /// New Filters to offer, based on Results
        /// </summary>
        public JArray FilterOptions( ICswNbtTree Tree )
        {
            JArray FiltersArr = new JArray();
            Tree.goToRoot();
            bool SingleNodeType = IsSingleNodeType();
            if( false == SingleNodeType )
            {
                // Filter on NodeTypes only
                Dictionary<Int32, Int32> NodeTypeIds = new Dictionary<Int32, Int32>();
                Int32 ChildCnt = Tree.getChildNodeCount();
                for( Int32 n = 0; n < ChildCnt; n++ )
                {
                    Tree.goToNthChild( n );
                    CswNbtNodeKey NodeKey = Tree.getNodeKeyForCurrentPosition();
                    if( NodeKey != null )
                    {
                        if( false == NodeTypeIds.ContainsKey( NodeKey.NodeTypeId ) )
                        {
                            NodeTypeIds[NodeKey.NodeTypeId] = 0;
                        }
                        NodeTypeIds[NodeKey.NodeTypeId] += 1;
                    }
                    Tree.goToParentNode();
                } // for( Int32 n = 0; n < ChildCnt; n++ )

                if( NodeTypeIds.Keys.Count == 1 )
                {
                    if( false == IsSingleNodeType() )
                    {
                        // If we have uniform results but no nodetype filter applied
                        // add the filter to the filters list for display
                        Int32 NodeTypeId = NodeTypeIds.Keys.First();
                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                        CswNbtSearchFilter NodeTypeFilter = makeFilter( NodeType, NodeTypeIds[NodeTypeId], false, CswNbtSearchPropOrder.PropOrderSourceType.Unknown );
                        addFilter( NodeTypeFilter );
                    }
                    SingleNodeType = true;
                }
                else
                {
                    JArray FilterSet = new JArray();
                    FiltersArr.Add( FilterSet );

                    // Sort by count descending, then (unfortunately) by nodetypeid
                    Dictionary<Int32, Int32> sortedDict = ( from entry
                                                              in NodeTypeIds
                                                            orderby entry.Value descending, entry.Key ascending
                                                            select entry
                                                           ).ToDictionary( pair => pair.Key, pair => pair.Value );
                    foreach( Int32 NodeTypeId in sortedDict.Keys )
                    {
                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                        Int32 Count = sortedDict[NodeTypeId];
                        CswNbtSearchFilter NodeTypeFilter = makeFilter( NodeType, Count, true, CswNbtSearchPropOrder.PropOrderSourceType.Unknown );
                        FilterSet.Add( NodeTypeFilter.ToJObject() );
                    }
                }
            } // if( false == SingleNodeType )

            if( SingleNodeType )
            {
                // Filter on property values in the results
                Collection<Int32> FilteredPropIds = getFilteredPropIds();
                Dictionary<Int32, Dictionary<string, Int32>> PropCounts = new Dictionary<Int32, Dictionary<string, Int32>>();
                SortedSet<CswNbtSearchPropOrder.SearchOrder> PropOrder = new SortedSet<CswNbtSearchPropOrder.SearchOrder>();
                Int32 ChildCnt = Tree.getChildNodeCount();
                for( Int32 n = 0; n < ChildCnt; n++ )
                {
                    Tree.goToNthChild( n );

                    if( 0 == PropOrder.Count )
                    {
                        PropOrder = _CswNbtSearchPropOrder.getPropOrderDict( Tree.getNodeKeyForCurrentPosition() );
                    }
                    JArray Props = Tree.getChildNodePropsOfNode();
                    foreach( JObject Prop in Props )
                    {
                        Int32 NodeTypePropId = CswConvert.ToInt32( Prop["nodetypepropid"] );
                        CswNbtMetaDataFieldType FieldType = _CswNbtResources.MetaData.getFieldType( CswConvert.ToString( Prop["fieldtype"] ) );
                        if( false == FilteredPropIds.Contains( NodeTypePropId ) && FieldType.Searchable )
                        {
                            string Gestalt = Prop["gestalt"].ToString();
                            if( Gestalt.Length > 50 )
                            {
                                Gestalt = Gestalt.Substring( 0, 50 );
                            }

                            if( false == PropCounts.ContainsKey( NodeTypePropId ) )
                            {
                                PropCounts[NodeTypePropId] = new Dictionary<string, Int32>();
                            }
                            if( false == PropCounts[NodeTypePropId].ContainsKey( Gestalt ) )
                            {
                                PropCounts[NodeTypePropId][Gestalt] = 0;
                            }
                            PropCounts[NodeTypePropId][Gestalt] += 1;
                        }
                    }

                    Tree.goToParentNode();
                } // for( Int32 n = 0; n < ChildCnt; n++ )

                foreach( Int32 NodeTypePropId in PropCounts.Keys.OrderBy( NodeTypePropId => PropOrder.First( Order => Order.NodeTypePropId == NodeTypePropId ).Order ) )
                {
                    CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypePropLatestVersion( NodeTypePropId );
                    if( false == NodeTypeProp.IsUnique() )   // case 27649
                    {
                        CswNbtSearchPropOrder.SearchOrder order = PropOrder.First( Order => Order.NodeTypePropId == NodeTypePropId );

                        JArray FilterSet = new JArray();
                        FiltersArr.Add( FilterSet );

                        // Sort by count descending, then alphabetically by gestalt
                        Dictionary<string, Int32> sortedDict = ( from entry
                                                                     in PropCounts[NodeTypePropId]
                                                                 orderby entry.Value descending , entry.Key ascending
                                                                 select entry
                                                               ).ToDictionary( pair => pair.Key, pair => pair.Value );
                        foreach( string Value in sortedDict.Keys )
                        {
                            Int32 Count = sortedDict[Value];
                            CswNbtSearchFilter Filter = makeFilter( NodeTypeProp, Value, Count, true, order.Source );
                            FilterSet.Add( Filter.ToJObject() );
                        }
                    } // if( false == NodeTypeProp.IsUnique() )
                } // foreach( Int32 NodeTypePropId in PropCounts.Keys.OrderBy( NodeTypePropId => PropOrder.First( Order => Order.NodeTypePropId == NodeTypePropId ).Order ) )
            } // if( SingleNodeType )

            return FiltersArr;
        } // FilterOptions()

        private CswNbtSearchFilter makeFilter( CswNbtMetaDataNodeType NodeType, Int32 ResultCount, bool Removeable, CswNbtSearchPropOrder.PropOrderSourceType Source )
        {
            CswNbtSearchFilter ret = new CswNbtSearchFilter( "Filter To",
                                                  CswNbtSearchFilterType.nodetype,
                                                  "NT_" + NodeType.NodeTypeId.ToString(),
                                                  NodeType.NodeTypeName,
                                                  ResultCount,
                                                  NodeType.IconFileName,
                                                  Removeable,
                                                  Source );
            ret.FirstVersionId = NodeType.FirstVersionNodeTypeId;
            return ret;
        }

        private CswNbtSearchFilter makeFilter( CswNbtMetaDataObjectClass ObjectClass, Int32 ResultCount, bool Removeable, CswNbtSearchPropOrder.PropOrderSourceType Source )
        {
            CswNbtSearchFilter ret = new CswNbtSearchFilter( "Filter To",
                                                  CswNbtSearchFilterType.objectclass,
                                                  "OC_" + ObjectClass.ObjectClassId.ToString(),
                                                  "All " + ObjectClass.ObjectClass.ToString(),
                                                  ResultCount,
                                                  ObjectClass.IconFileName,
                                                  Removeable,
                                                  Source );
            ret.ObjectClassId = ObjectClass.ObjectClassId;
            return ret;
        }

        private CswNbtSearchFilter makeFilter( CswNbtMetaDataNodeTypeProp NodeTypeProp, string Value, Int32 ResultCount, bool Removeable, CswNbtSearchPropOrder.PropOrderSourceType Source )
        {
            CswNbtSearchFilter ret = new CswNbtSearchFilter( NodeTypeProp.PropName,
                                                  CswNbtSearchFilterType.propval,
                                                  NodeTypeProp.PropId.ToString() + "_" + Value,
                                                  Value,
                                                  ResultCount,
                                                  string.Empty,
                                                  Removeable,
                                                  Source );
            ret.FirstPropVersionId = NodeTypeProp.FirstPropVersionId;
            return ret;
        }


        #endregion Search Functions
        
        #region IEquatable
        /// <summary>
        /// IEquatable: ==
        /// </summary>
        public static bool operator ==( CswNbtSearch view1, CswNbtSearch view2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( view1, view2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) view1 == null ) || ( (object) view2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( view1.ToString() == view2.ToString() )
                return true;
            else
                return false;
        }

        /// <summary>
        /// IEquatable: !=
        /// </summary>
        public static bool operator !=( CswNbtSearch view1, CswNbtSearch view2 )
        {
            return !( view1 == view2 );
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtSearch ) )
                return false;
            return this == (CswNbtSearch) obj;
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public bool Equals( CswNbtSearch obj )
        {
            return this == (CswNbtSearch) obj;
        }

        /// <summary>
        /// IEquatable: GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            int hashcode = 0;
            //if( this.ViewId.isSet() )
            //{
            //    // Positive hashes are for saved views
            //    hashcode = this.ViewId.get();
            //}
            //else
            //{
            // Negative hashes are for dynamic views or searches
            hashcode = Math.Abs( this.ToString().GetHashCode() ) * -1;
            //}
            return hashcode;
        }
        #endregion IEquatable

    } // class CswNbtSearch


} // namespace ChemSW.Nbt



