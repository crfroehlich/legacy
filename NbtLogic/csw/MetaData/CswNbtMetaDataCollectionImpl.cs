﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionImpl
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswTableSelect _TableSelect;
        private CswTableUpdate _TableUpdate;
        private string _PkColumnName;
        private string _NameColumnName;

        public delegate ICswNbtMetaDataObject MakeMetaDataObjectHandler( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row );
        private MakeMetaDataObjectHandler _MetaDataObjectMaker = null;

        public CswNbtMetaDataCollectionImpl( CswNbtMetaDataResources CswNbtMetaDataResources,
                                             string PkColumnName,
                                             string NameColumnName,
                                             CswTableSelect TableSelect,
                                             CswTableUpdate TableUpdate,
                                             MakeMetaDataObjectHandler MetaDataObjectMaker )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _TableSelect = TableSelect;
            _TableUpdate = TableUpdate;
            _PkColumnName = PkColumnName;
            _NameColumnName = NameColumnName;
            _MetaDataObjectMaker = MetaDataObjectMaker;
        } // constructor

        /// <summary>
        ///  Add an ICswNbtMetaDataObject to the Cache 
        ///  (for use by MetaData for newly created objects)
        /// </summary>
        public void AddToCache(ICswNbtMetaDataObject NewObj)
        {
            if( false == _Cache.ContainsKey( NewObj.UniqueId ) )
            {
                _Cache.Add( NewObj.UniqueId, NewObj );
            }
        }

        private Dictionary<Int32, ICswNbtMetaDataObject> _Cache = new Dictionary<Int32, ICswNbtMetaDataObject>();
        private ICswNbtMetaDataObject _makeObj( DataRow Row )
        {
            ICswNbtMetaDataObject ret = null;
            Int32 PkValue = CswConvert.ToInt32( Row[_PkColumnName] );
            if( _Cache.ContainsKey( PkValue ) )
            {
                // In order to guarantee only one reference per row, use the existing reference
                // and, to prevent dirty writes, remove the row
                ret = _Cache[PkValue];
                Row.Table.Rows.Remove( Row );
            }
            else
            {
                ret = _MetaDataObjectMaker( _CswNbtMetaDataResources, Row );
                _Cache[PkValue] = ret;
            }
            return ret;
        }


        private Collection<ICswNbtMetaDataObject> _makeObjs( DataTable Table )
        {
            Collection<ICswNbtMetaDataObject> Coll = new Collection<ICswNbtMetaDataObject>();
            Collection<DataRow> RowsToIterate = new Collection<DataRow>();
            // We have to iterate rows separately, because _makeObj() can remove a row
            foreach( DataRow Row in Table.Rows )
            {
                RowsToIterate.Add( Row );
            }
            foreach( DataRow Row in RowsToIterate )
            {
                Coll.Add( _makeObj( Row ) );
            }
            return Coll;
        } // _makeObjs()

        public void clearCache()
        {
            _All = null;
            _Pks = null;
            _PksWhere = null;
            _ByPk = null;
            _getWhere = null;

            // Don't clear this one
            // _Cache = null;
        }

        private Collection<ICswNbtMetaDataObject> _All = null;
        public Collection<ICswNbtMetaDataObject> getAll()
        {
            if( _All == null )
            {
                DataTable Table = _TableUpdate.getTable();
                _All = _makeObjs( Table );
            }
            return _All;
        } // getAll()

        private Collection<Int32> _Pks = null;
        public Collection<Int32> getPks()
        {
            if( _Pks == null )
            {
                _Pks = getPks( string.Empty );
            }
            return _Pks;
        } // getPks()

        private Dictionary<string, Collection<Int32>> _PksWhere = null;
        public Collection<Int32> getPks( string Where )
        {
            if( _PksWhere == null )
            {
                _PksWhere = new Dictionary<string, Collection<Int32>>();
            }
            if( false == _PksWhere.ContainsKey( Where ) )
            {
                CswCommaDelimitedString Select = new CswCommaDelimitedString();
                Select.Add( _PkColumnName );
                DataTable Table = _TableSelect.getTable( Select, string.Empty, Int32.MinValue, Where, false );

                Collection<Int32> Coll = new Collection<Int32>();
                foreach( DataRow Row in Table.Rows )
                {
                    Coll.Add( CswConvert.ToInt32( Row[_PkColumnName] ) );
                }
                _PksWhere[Where] = Coll;
            }
            return _PksWhere[Where];
        } // getPks(Where)

        public Int32 getPksFirst( string WhereClause )
        {
            Int32 ret = Int32.MinValue;
            Collection<Int32> Coll = getPks( WhereClause );
            if( Coll.Count > 0 )
            {
                ret = Coll[0];
            }
            return ret;
        } // getPksFirst()



        private Dictionary<string, Int32> _PkDict = null;
        public Dictionary<string, Int32> getPkDict()
        {
            if( _PkDict == null )
            {
                _PkDict = getPkDict( string.Empty );
            }
            return _PkDict;
        } // getPkDict()

        private Dictionary<string, Dictionary<string, Int32>> _PkDictsWhere = null;
        public Dictionary<string, Int32> getPkDict( string Where )
        {
            if( _PkDictsWhere == null )
            {
                _PkDictsWhere = new Dictionary<string, Dictionary<string, Int32>>();
            }
            if( false == _PkDictsWhere.ContainsKey( Where ) )
            {
                CswCommaDelimitedString Select = new CswCommaDelimitedString();
                Select.Add( _PkColumnName );
                Select.Add( _NameColumnName );
                DataTable Table = _TableSelect.getTable( Select, string.Empty, Int32.MinValue, Where, false );

                Dictionary<string, Int32> Coll = new Dictionary<string, Int32>();
                foreach( DataRow Row in Table.Rows )
                {
                    Coll.Add( CswConvert.ToString( Row[_NameColumnName] ), CswConvert.ToInt32( Row[_PkColumnName] ) );
                }
                _PkDictsWhere[Where] = Coll;
            }
            return _PkDictsWhere[Where];
        } // _PkDictsWhere(Where)

        private Dictionary<Int32, ICswNbtMetaDataObject> _ByPk = null;
        public ICswNbtMetaDataObject getByPk( Int32 Pk )
        {
            ICswNbtMetaDataObject ret = null;
            if( Pk != Int32.MinValue )
            {
                if( _ByPk == null )
                {
                    _ByPk = new Dictionary<Int32, ICswNbtMetaDataObject>();
                }
                if( false == _ByPk.ContainsKey( Pk ) )
                {
                    DataTable Table = _TableUpdate.getTable( _PkColumnName, Pk );
                    if( Table.Rows.Count > 0 )
                    {
                        _ByPk[Pk] = _makeObj( Table.Rows[0] );
                    }
                    else
                    {
                        _ByPk[Pk] = null;
                    }
                }
                ret = _ByPk[Pk];
            } // if( Pk != Int32.MinValue )
            return ret;
        } // getByPk()
        
        private Dictionary<string, Collection<ICswNbtMetaDataObject>> _getWhere = null;
        public Collection<ICswNbtMetaDataObject> getWhere( string WhereClause )
        {
            if( _getWhere == null )
            {
                _getWhere = new Dictionary<string, Collection<ICswNbtMetaDataObject>>();
            }
            if( false == _getWhere.ContainsKey( WhereClause ) )
            {
                DataTable Table = _TableUpdate.getTable( WhereClause );
                _getWhere[WhereClause] = _makeObjs( Table );
            }
            return _getWhere[WhereClause];
        } // getWhere()

        public ICswNbtMetaDataObject getWhereFirst( string WhereClause )
        {
            ICswNbtMetaDataObject ret = null;
            Collection<ICswNbtMetaDataObject> Coll = getWhere( WhereClause );
            if( Coll.Count > 0 )
            {
                ret = Coll[0];
            }
            return ret;
        } // getWhereFirst()

        private Dictionary<string, string> _getNameWhere = null;
        public string getNameWhereFirst( string WhereClause )
        {
            if( _getNameWhere == null )
            {
                _getNameWhere = new Dictionary<string, string>();
            }
            if( false == _getNameWhere.ContainsKey( WhereClause ) )
            {
                CswCommaDelimitedString SelectCols = new CswCommaDelimitedString();
                SelectCols.Add( _NameColumnName );
                DataTable Table = _TableSelect.getTable( SelectCols, string.Empty, Int32.MinValue, WhereClause, false );
                if( Table.Rows.Count > 0 )
                {
                    _getNameWhere[WhereClause] = Table.Rows[0][_NameColumnName].ToString();
                }
                else
                {
                    _getNameWhere[WhereClause] = string.Empty;
                }
            }
            return _getNameWhere[WhereClause];
        } // getNameWhereFirst()


    } // public class CswNbtMetaDataCollectionImpl
} // namespace ChemSW.Nbt.MetaData