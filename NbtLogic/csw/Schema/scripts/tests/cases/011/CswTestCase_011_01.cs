
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_011_01 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_011.Purpose, "Add a test table" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_011 _CswTstCaseRsrc_011 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_011_01( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswTstCaseRsrc_011 = (CswTstCaseRsrc_011) CswTstCaseRsc;
        }//ctor

        public override void update()
        {
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_011.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            _CswNbtSchemaModTrnsctn.addTable( _CswTstCaseRsrc_011.FakeTestTableName, _CswTstCaseRsrc_011.FakeTestTableName + "id" );
            _CswNbtSchemaModTrnsctn.addStringColumn( _CswTstCaseRsrc_011.FakeTestTableName, _CswTstCaseRsrc_011.FakeTestColumnName, "snot", false, false, 240 );

        }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override string ScriptName
        {
            get { throw new System.NotImplementedException(); }
        }

        public override bool AlwaysRun
        {
            get { throw new System.NotImplementedException(); }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
