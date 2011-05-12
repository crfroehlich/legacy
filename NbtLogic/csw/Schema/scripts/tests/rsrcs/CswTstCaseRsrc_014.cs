using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{
    //9102
    //when you retrieve the data table using a list of select columns, 
    //the update performed with that Table results in the deletion of the record.
    public class CswTstCaseRsrc_014
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswTestCaseRsrc _CswTestCaseRsrc = null;
        public CswTstCaseRsrc_014( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );

        }//ctor


        public string Purpose = "Update From Selected Columns";

        public Int32 InsertedMaterialsRecordPk = Int32.MinValue; 


        public string FakeTestTableName { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable01 ) ); } }
        public string FakeValColumnName01 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ); } }
        public string FakeValColumnName02 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) ); } }
        public string FakePkColumnName { get { return ( FakeTestTableName + "id" ); } }

        public string LocalAribtiraryValue01 { get { return ( this.GetType().Name + "1" ); } }
        public string LocalAribtiraryValue01Delta { get { return ( this.GetType().Name + "2" ); } }
        public string LocalAribtiraryValue02 { get { return ( this.GetType().Name + "X" ); } }
        public string LocalAribtiraryValue02Delta { get { return ( this.GetType().Name + "Y" ); } }


        public DataTable TheSuspectUpdateTable = null;
        public CswTableUpdate TheSuspectUpdateTablesUpdater = null; 


    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
