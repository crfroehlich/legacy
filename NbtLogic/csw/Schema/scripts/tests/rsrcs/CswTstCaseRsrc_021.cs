﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Schema;
using ChemSW.Audit;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTstCaseRsrc_021
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswTestCaseRsrc _CswTestCaseRsrc = null;
        private CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();
        public CswTstCaseRsrc_021( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {

            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
        }//ctor


        public string Purpose = "Basic audit mechanism";

        public string ArbitraryTableName_01 { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable01 ) ); } }

        public string ArbitraryColumnName_01 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ); } }
        public string ArbitraryColumnName_02 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) ); } }
        public string ArbitraryColumnName_03 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn03 ) ); } }

        public string ArbitraryColumnValue { get { return ( "snot" ); } }

        public Int32 TotalTestRows { get { return ( 10 ); } }



        public void makeArbitraryTable()
        {
            _CswNbtSchemaModTrnsctn.addTable( ArbitraryTableName_01, ( ArbitraryTableName_01 + "id" ).Replace( "_", "" ) );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_01, ArbitraryColumnName_01, false, false, 60 );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_02, ArbitraryColumnName_02, false, false, 60 );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_03, ArbitraryColumnName_03, false, false, 60 );

        }


        private Dictionary<string, List<string>> __ArbitraryTestValues;
        private Dictionary<string, List<string>> _ArbitraryTestValues
        {
            get
            {
                if( null == __ArbitraryTestValues )
                {
                    __ArbitraryTestValues = _CswTestCaseRsrc.makeArbitraryTestValues( 20 );
                }

                return ( __ArbitraryTestValues );

            }//get

        }//_ArbitraryTestValues



        public bool compareTargetAndAuditedData( ref string MisMatchMessage )
        {

            CswCommaDelimitedString CswCommaDelimitedString = new Core.CswCommaDelimitedString();
            string ArbitraryTablePkCol = CswTools.makePkColNameFromTableName( ArbitraryTableName_01 );
            Collection<OrderByClause> OrderByClauses = new Collection<OrderByClause>();
            ;
            foreach( string CurrentColumnName in _CswNbtSchemaModTrnsctn.CswDataDictionary.getColumnNames( ArbitraryTableName_01 ) )
            {
                if( CurrentColumnName.ToLower() != ArbitraryTablePkCol.ToLower() && CurrentColumnName.ToLower() != _CswAuditMetaData.AuditLevelColName )
                {
                    OrderByClauses.Add( new OrderByClause( CurrentColumnName, OrderByType.Ascending ) );
                    CswCommaDelimitedString.Add( CurrentColumnName );
                }
            }

            CswTableSelect CswTableSelectTargetTable = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "Compare table 1", ArbitraryTableName_01 );
            DataTable TargetTable = CswTableSelectTargetTable.getTable( " where 1=1", OrderByClauses );

            string AuditTableName = _CswAuditMetaData.makeAuditTableName( ArbitraryTableName_01 );
            CswTableSelect CswTableSelectAuditTable = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "Compare table 1", AuditTableName );
            DataTable AuditTable = CswTableSelectAuditTable.getTable( "where 1=1", OrderByClauses );


            return ( _CswTestCaseRsrc.doTableValuesMatch( TargetTable, AuditTable, CswCommaDelimitedString, ref MisMatchMessage ) );


        }//compareTargetAndAuditedData()

        public void makeArbitraryTableData()
        {
            _CswTestCaseRsrc.fillTableWithArbitraryData( ArbitraryTableName_01, _ArbitraryTestValues );
        }

        public void dropArbitraryTables()
        {
            _CswNbtSchemaModTrnsctn.dropTable( ArbitraryTableName_01 );
        }

        public void verifyTablesDropped()
        {
            _CswTestCaseRsrc.assertTableIsAbsent( ArbitraryTableName_01 );
        }

        private string _OriginalAuditSetting_Audit = string.Empty;
        public void setAuditingOn()
        {
            _OriginalAuditSetting_Audit = _CswNbtSchemaModTrnsctn.getConfigVariableValue( _CswAuditMetaData.AuditConfgVarName );

            if( "1" != _OriginalAuditSetting_Audit )
            {
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( _CswAuditMetaData.AuditConfgVarName, "1" );
            }

        }//setAuditingOn()

        public void restoreAuditSetting()
        {
            if( _CswNbtSchemaModTrnsctn.getConfigVariableValue( _CswAuditMetaData.AuditConfgVarName ) != _OriginalAuditSetting_Audit )
            {
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( _CswAuditMetaData.AuditConfgVarName, "1" );
            }
        }//setAuditingOn()

        public void assertAuditSettingIsRestored()
        {
            string CurrentAuditSetting = _CswNbtSchemaModTrnsctn.getConfigVariableValue( _CswAuditMetaData.AuditConfgVarName );

            if( _CswNbtSchemaModTrnsctn.getConfigVariableValue( _CswAuditMetaData.AuditConfgVarName ) != _OriginalAuditSetting_Audit )
            {
                throw ( new CswDniException( "Current audit configuration setting (" + CurrentAuditSetting + ") does not match the original setting (" + _OriginalAuditSetting_Audit + ")" ) );
            }

        }//assertAuditSettingIsRestored()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
