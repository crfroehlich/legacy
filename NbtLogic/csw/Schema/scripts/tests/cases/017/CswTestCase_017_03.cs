﻿using System.Collections.Generic;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_017_03 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_017.Purpose, "rename foreign key column" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_017 _CswTstCaseRsrc_017 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_017_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswTstCaseRsrc_017 = (CswTstCaseRsrc_017) CswTstCaseRsc;
        }//ctor

        public override void update()
        {
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_017.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            List<PkFkPair> PairList = _CswTstCaseRsrc_017.getPkFkPairs();
            foreach( PkFkPair CurrentPair in PairList )
            {
                _CswNbtSchemaModTrnsctn.renameColumn( CurrentPair.FkTableName, CurrentPair.FkTablePkColumnName, CurrentPair.FkTablePkColumnName + _CswTstCaseRsrc_017.PkTablePkColumnNameNewNameSuffix );
            }

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
