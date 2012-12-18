using System;
using System.Collections;
using ChemSW.Core;
using ChemSW.Nbt.Logic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceSearch
    {
        private readonly CswNbtResources _CswNbtResources;
        private CswNbtStatisticsEvents _CswNbtStatisticsEvents;
        private CswNbtViewBuilder _ViewBuilder;

        /// <summary>
        /// Searching against these field types is not yet supported
        /// </summary>
        private ArrayList _ProhibittedFieldTypes
        {
            get
            {
                ArrayList InvalidFieldTypes = new ArrayList();
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Button ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.LogicalSet ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.ViewPickList ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.ViewReference ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.MOL ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.MTBF ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Grid ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Password ) );
                return InvalidFieldTypes;
            }
        }

        public CswNbtWebServiceSearch( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
            _ViewBuilder = new CswNbtViewBuilder( _CswNbtResources, _ProhibittedFieldTypes );
        }//ctor

        #region UniversalSearch

        public JObject doUniversalSearch( string SearchTerm, Int32 NodeTypeId, Int32 ObjectClassId )
        {
            CswNbtSearch Search = new CswNbtSearch( _CswNbtResources )
                                      {
                                          SearchTerm = SearchTerm
                                      };
            if( Int32.MinValue != NodeTypeId )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( null != NodeType )
                {
                    Search.addFilter( NodeType, false );
                }
            }
            if( Int32.MinValue != ObjectClassId )
            {
                CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
                if( null != ObjectClass )
                {
                    Search.addFilter( ObjectClass, false );
                }
            }
            return _finishUniversalSearch( Search );
        }

        public JObject restoreUniversalSearch( CswNbtSessionDataId SessionDataId )
        {
            JObject ret = new JObject();
            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionDataId );
            if( SessionDataItem.DataType == CswNbtSessionDataItem.SessionDataType.Search )
            {
                CswNbtSearch Search = SessionDataItem.Search;
                ret = _finishUniversalSearch( Search );
            }
            return ret;
        } // restoreUniversalSearch()

        public JObject filterUniversalSearch( CswNbtSessionDataId SessionDataId, JObject Filter, string Action )
        {
            JObject ret = new JObject();
            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionDataId );
            if( SessionDataItem.DataType == CswNbtSessionDataItem.SessionDataType.Search )
            {
                CswNbtSearch Search = SessionDataItem.Search;
                if( Action == "add" )
                {
                    Search.addFilter( Filter );
                }
                else
                {
                    Search.removeFilter( Filter );
                }
                ret = _finishUniversalSearch( Search );
            }
            return ret;
        }

        public JObject filterUniversalSearchByNodeType( CswNbtSessionDataId SessionDataId, Int32 NodeTypeId )
        {
            JObject ret = new JObject();
            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionDataId );
            if( SessionDataItem.DataType == CswNbtSessionDataItem.SessionDataType.Search )
            {
                CswNbtSearch Search = SessionDataItem.Search;
                Search.addFilter( NodeTypeId, true );
                ret = _finishUniversalSearch( Search );
            }
            return ret;
        }
        private JObject _finishUniversalSearch( CswNbtSearch Search )
        {
            ICswNbtTree Tree = Search.Results();
            CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources, _CswNbtStatisticsEvents, null, Int32.MinValue );

            Search.SaveToCache( true );

            JObject ret = Search.ToJObject();
            ret["table"] = wsTable.makeTableFromTree( Tree, Search.getFilteredPropIds() );
            ret["filters"] = Search.FilterOptions( Tree );
            return ret;
        }

        public JObject saveSearch( CswNbtSessionDataId SessionDataId, string Name, string Category )
        {
            JObject ret = new JObject();
            CswNbtSessionDataItem SessionDataItem = _CswNbtResources.SessionDataMgr.getSessionDataItem( SessionDataId );
            if( SessionDataItem.DataType == CswNbtSessionDataItem.SessionDataType.Search )
            {
                CswNbtSearch Search = SessionDataItem.Search;
                Search.Name = Name;
                Search.Category = Category;
                Search.SaveToDb();
                ret = _finishUniversalSearch( Search );
            }
            return ret;
        } // saveSearch

        #endregion UniversalSearch


    } // class CswNbtWebServiceSearch


} // namespace ChemSW.Nbt.WebServices
