﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionFieldType : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswNbtMetaDataCollectionImpl _CollImpl;

        public CswNbtMetaDataCollectionFieldType( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _CollImpl = new CswNbtMetaDataCollectionImpl( _CswNbtMetaDataResources,
                                                          "fieldtypeid",
                                                          _CswNbtMetaDataResources.FieldTypeTableUpdate,
                                                          makeFieldType );
        }

        public void clearCache()
        {
            _CollImpl.clearCache();
        }

        public CswNbtMetaDataFieldType makeFieldType( CswNbtMetaDataResources Resources, DataRow Row )
        {
            return new CswNbtMetaDataFieldType( Resources, Row );
        }

        public Collection<Int32> getFieldTypeIds()
        {
            return _CollImpl.getPks();
        }

        public IEnumerable<CswNbtMetaDataFieldType> getFieldTypes()
        {
            return _CollImpl.getAll().Cast<CswNbtMetaDataFieldType>();
        }

        private Dictionary<CswNbtMetaDataFieldType.NbtFieldType, CswNbtMetaDataFieldType> _ByType = null;
        public CswNbtMetaDataFieldType getFieldType( CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            return (CswNbtMetaDataFieldType) _CollImpl.getWhereFirst( "where lower(fieldtype)='" + FieldType.ToString().ToLower() + "'" );
        }

        private Dictionary<Int32, CswNbtMetaDataFieldType> _ById = null;
        public CswNbtMetaDataFieldType getFieldType( Int32 FieldTypeId )
        {
            return (CswNbtMetaDataFieldType) _CollImpl.getByPk( FieldTypeId );
        }
    } // class CswNbtMetaDataCollectionFieldType
} // namespace ChemSW.Nbt.MetaData