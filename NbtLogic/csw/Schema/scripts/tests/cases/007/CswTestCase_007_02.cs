using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_007_02 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_007.Purpose, "test that columns are there and cleanup" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_007 _CswTstCaseRsrc_007 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_007_02( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_007 = new CswTstCaseRsrc_007( _CswNbtSchemaModTrnsctn );
        }//ctor

        public void update()
        {
            if( !_CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _CswTstCaseRsrc.getRealTestTableName( TestTableNamesReal.DataDictionary ), _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) + " was not created in data base " ) );

            if( !_CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _CswTstCaseRsrc.getRealTestTableName( TestTableNamesReal.DataDictionary ), _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) + " was not created in data base " ) );

            if( !_CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _CswTstCaseRsrc.getRealTestTableName( TestTableNamesReal.DataDictionary ), _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) + " was not created in meta data " ) );

            if( !_CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _CswTstCaseRsrc.getRealTestTableName( TestTableNamesReal.DataDictionary ), _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) + " was not created in meta data " ) );

            _CswNbtSchemaModTrnsctn.dropColumn( _CswTstCaseRsrc.getRealTestTableName( TestTableNamesReal.DataDictionary ), _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) );
            _CswNbtSchemaModTrnsctn.dropColumn( _CswTstCaseRsrc.getRealTestTableName( TestTableNamesReal.DataDictionary ), _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) );
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
