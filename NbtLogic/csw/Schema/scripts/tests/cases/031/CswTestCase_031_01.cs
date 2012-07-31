﻿
namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_031_01 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_031.Purpose, "Enable auditing on " + CswTstCaseRsrc_031.TestNodeTypeName + " node type" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_031 _CswTstCaseRsrc_031 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_031_01( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswTstCaseRsrc_031 = (CswTstCaseRsrc_031) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_031.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            _CswTstCaseRsrc_031.enableAuditing();

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema