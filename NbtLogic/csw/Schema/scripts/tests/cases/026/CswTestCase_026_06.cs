﻿
namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_026_06 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_026.Purpose, "clean up test data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_026 _CswTstCaseRsrc_026 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_026_06( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_026 = (CswTstCaseRsrc_026) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_026.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			//CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
            //_CswNbtSchemaModTrnsctn.dropTable( _CswTstCaseRsrc_026.ArbitraryTableName_01 );
            //_CswNbtSchemaModTrnsctn.dropTable( CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_026.ArbitraryTableName_01 ) );
            //_CswTstCaseRsrc_026.restoreAuditSetting(); 
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
