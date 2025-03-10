﻿using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// CswUpdateSchemaPLSQLTypes
    /// </summary>    
    public class CswUpdateSchemaPLSQLTypes
    {
        public sealed class TypeHeaders : CswEnum<TypeHeaders>
        {
            #region Properties and ctor

            private TypeHeaders( string Dev, Int32 CaseNo, string Title, string Name ) : base( Name )
            {
                _Dev = Dev;
                _CaseNo = CaseNo;
                _Title = Title;
            }
            static TypeHeaders() { }
            public string _Dev { get; private set; }
            public Int32 _CaseNo { get; private set; }
            public string _Title { get; private set; }
            public static IEnumerable<TypeHeaders> _All { get { return All; } }
            public static implicit operator TypeHeaders( string str )
            {
                TypeHeaders ret = Parse( str );
                return ret;
            }

            #endregion Properties and ctor

            #region TIER_II_LOCATION

            public static readonly TypeHeaders TIER_II_LOCATION = new TypeHeaders( CswEnumDeveloper.BV, 28247, "TIER_II_LOCATION",
            @"create or replace
TYPE TIER_II_LOCATION AS OBJECT 
(
  LOCATIONID number,
  PARENTLOCATIONID number
)" );

            #endregion TIER_II_LOCATION

            #region TIER_II_MATERIAL

            public static readonly TypeHeaders TIER_II_MATERIAL = new TypeHeaders( CswEnumDeveloper.BV, 28247, "TIER_II_MATERIAL",
            @"create or replace
TYPE TIER_II_MATERIAL AS OBJECT 
(
  MATERIALID number,
  CASNO varchar2(255),
  QUANTITY number,  
  TOTALQUANTITY number,
  UNITID number,
  UNITTYPE varchar2(50),
  SPECIFICGRAVITY number
)" );

            #endregion TIER_II_MATERIAL

            #region TIER_II_ROW

            public static readonly TypeHeaders TIER_II = new TypeHeaders( CswEnumDeveloper.BV, 31910, "TIER_II_ROW",
            @"create or replace
TYPE TIER_II_ROW AS OBJECT 
(
  MATERIALID number,
  TRADENAME varchar2(255),
  CASNO varchar2(255),
  MATERIALTYPE varchar2(255),  
  PHYSICALSTATE varchar2(255),
  EHS varchar2(1),
  TRADESECRET varchar2(1),
  HAZARDCATEGORIES varchar2(255),
  MAXQTY number,  
  MAXQTYRANGECODE varchar2(10),  
  AVGQTY number,
  AVGQTYRANGECODE varchar2(10),
  DAYSONSITE number,
  USETYPE varchar2(255),  
  PRESSURE varchar2(255),
  TEMPERATURE varchar2(255),  
  STORAGELOCATIONS varchar2(4000)
)" );

            #endregion TIER_II_ROW
        }

        public sealed class NestedTables : CswEnum<NestedTables>
        {
            #region Properties and ctor

            private NestedTables( string Dev, Int32 CaseNo, string Title, string Name ) : base( Name )
            {
                _Dev = Dev;
                _CaseNo = CaseNo;
                _Title = Title;
            }
            static NestedTables() { }
            public string _Dev { get; private set; }
            public Int32 _CaseNo { get; private set; }
            public string _Title { get; private set; }
            public static IEnumerable<NestedTables> _All { get { return All; } }
            public static implicit operator NestedTables( string str )
            {
                NestedTables ret = Parse( str );
                return ret;
            }

            #endregion Properties and ctor

            #region CSW_NUMBER_TABLE

            public static readonly NestedTables CSW_NUMBER_TABLE = new NestedTables( CswEnumDeveloper.BV, 29885, "CSW_NUMBER_TABLE",
            @"create or replace
type CSW_NUMBER_TABLE as table of NUMBER;" );

            #endregion CSW_NUMBER_TABLE

            #region TIER_II_LOCATION_TABLE

            public static readonly NestedTables TIER_II_LOCATION_TABLE = new NestedTables( CswEnumDeveloper.BV, 28247, "TIER_II_LOCATION_TABLE",
            @"create or replace
type TIER_II_LOCATION_TABLE as table of TIER_II_LOCATION;" );

            #endregion TIER_II_LOCATION_TABLE

            #region TIER_II_MATERIAL_TABLE

            public static readonly NestedTables TIER_II_MATERIAL_TABLE = new NestedTables( CswEnumDeveloper.BV, 28247, "TIER_II_MATERIAL_TABLE",
            @"create or replace
type TIER_II_MATERIAL_TABLE as table of TIER_II_MATERIAL;" );

            #endregion TIER_II_MATERIAL_TABLE

            #region TIER_II_TABLE

            public static readonly NestedTables TIER_II_TABLE = new NestedTables( CswEnumDeveloper.BV, 28247, "TIER_II_TABLE",
            @"create or replace
type TIER_II_TABLE as table of TIER_II_ROW;" );

            #endregion TIER_II_TABLE
        }

    }//class CswUpdateSchemaPLSQLTypes

}//namespace ChemSW.Nbt.Schema