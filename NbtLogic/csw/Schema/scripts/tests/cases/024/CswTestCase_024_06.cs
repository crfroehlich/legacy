﻿using ChemSW.Audit;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_024_06 : CswUpdateSchemaTo
    {
        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_024.Purpose, "verify test data was cleaned up" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_024 _CswTstCaseRsrc_024 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_024_06( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_024 = (CswTstCaseRsrc_024) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_024.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
            _CswTstCaseRsrc.assertTableIsAbsent( _CswTstCaseRsrc_024.ArbitraryTableName_01 );
            _CswTstCaseRsrc.assertTableIsAbsent( CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_024.ArbitraryTableName_01 ) );
            _CswTstCaseRsrc_024.assertAuditSettingIsRestored(); 
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
