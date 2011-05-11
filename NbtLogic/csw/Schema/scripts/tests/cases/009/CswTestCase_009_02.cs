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

    public class CswTestCase_009_02 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_009.Purpose, "verify read and tear down" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_009 _CswTstCaseRsrc_009 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_009_02( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_009 = new CswTstCaseRsrc_009( _CswNbtSchemaModTrnsctn );
        }//ctor

        public void update()
        {


            _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( _CswTstCaseRsrc_009.FakeTestTableName, _CswTstCaseRsrc_009.FakeTestColumnName );

            Int32 TableColId = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;

            if( TableColId <= 0 )
            {
                throw ( new CswDniException( "tableclid received from data dictionry is suspect: " + TableColId.ToString() ) );
            }

            _CswNbtSchemaModTrnsctn.dropTable( _CswTstCaseRsrc_009.FakeTestTableName );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
